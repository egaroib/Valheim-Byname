using System.Collections.Generic;
using System.Linq;
using Byname.Config;
using Byname.Net;
using Byname.Titles;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Byname.Patches
{
    /// <summary>
    /// Draws the title on other players' nameplates, on its own line.
    ///
    /// Deliberately NOT done by appending "\n" + title inside a Player.GetHoverName
    /// postfix, which is the obvious approach and works perfectly until Guilds is
    /// installed. Guilds (org.bepinex.plugins.guilds) adds its own "guildname" object
    /// under the hud root and shifts the Name label's pivot to 0.2 to open space directly
    /// below the name — the same space a second line in the name label would occupy.
    ///
    /// So Byname owns its own child object instead, and stacks below Guilds' label when
    /// that mod is present and the player is actually in a guild. Groups
    /// (org.bepinex.plugins.groups) only tints the Name label's colour, which is
    /// untouched here by design.
    /// </summary>
    [HarmonyPatch(typeof(EnemyHud), nameof(EnemyHud.ShowHud))]
    [HarmonyAfter("org.bepinex.plugins.guilds", "org.bepinex.plugins.groups")]
    internal static class NameplatePatch
    {
        private const string ChildName = "bynametitle";
        private const string GuildsChildName = "guildname";
        private const string HealthChildName = "Health";

        /// <summary>Clear space left above and below the title when it is slotted in.</summary>
        private const float Pad = 1f;

        // ShowHud's body only runs once per character, but a postfix runs every frame for
        // every visible character. Caching the label keeps that from being a
        // transform.Find per player per frame.
        private static readonly Dictionary<GameObject, TextMeshProUGUI> Labels =
            new Dictionary<GameObject, TextMeshProUGUI>();

        private static void Postfix(EnemyHud __instance, Character c)
        {
            if (!BynameConfig.Enabled.Value) return;
            if (BynameConfig.Placement.Value != NameplatePlacement.SecondLine) return;

            if (!(c is Player player)) return;
            if (!__instance.m_huds.TryGetValue(c, out var hud)) return;
            if (hud.m_gui == null || hud.m_name == null) return;

            var label = GetOrCreateLabel(hud.m_gui, hud.m_name);
            if (label == null) return;

            var title = TitleNet.Read(player, out var rarity);
            if (string.IsNullOrEmpty(title))
            {
                if (label.text.Length > 0) label.text = "";
                return;
            }

            var rendered = TitleService.Decorate(title, rarity);
            if (label.text != rendered) label.text = rendered;

            Position(hud.m_gui, hud.m_name, label);
        }

        private static TextMeshProUGUI GetOrCreateLabel(GameObject hudBase, TextMeshProUGUI nameLabel)
        {
            if (Labels.TryGetValue(hudBase, out var cached) && cached != null) return cached;

            // Unity objects compare null once destroyed, so a stale entry is replaced
            // rather than trusted. Pruned here too, since huds are created and destroyed
            // continuously as players move in and out of range.
            if (Labels.Count > 64) Prune();

            var existing = hudBase.transform.Find(ChildName);
            if (existing != null)
            {
                var found = existing.GetComponent<TextMeshProUGUI>();
                Labels[hudBase] = found;
                return found;
            }

            var go = new GameObject(ChildName, typeof(RectTransform));
            go.transform.SetParent(hudBase.transform, false);

            // Anchors are copied from the name rather than left at whatever a
            // script-created RectTransform defaults to, so both labels measure from the
            // same corner of the nameplate and Position can compare them directly.
            var nameRect = nameLabel.rectTransform;
            var rect = (RectTransform)go.transform;
            rect.anchorMin = nameRect.anchorMin;
            rect.anchorMax = nameRect.anchorMax;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(300f, LineHeight(nameLabel));

            var text = go.AddComponent<TextMeshProUGUI>();
            text.font = nameLabel.font;
            text.fontSize = nameLabel.fontSize * 0.8f;
            text.alignment = nameLabel.alignment;
            text.color = Color.white;
            text.raycastTarget = false;

            // Matches how Valheim and Guilds keep small HUD text legible against terrain.
            var outline = go.AddComponent<Outline>();
            outline.effectColor = Color.black;
            outline.effectDistance = new Vector2(1f, -1f);

            Labels[hudBase] = text;
            return text;
        }

        /// <summary>
        /// Slots the title into the gap between the bottom of the name and whatever is
        /// drawn beneath it.
        ///
        /// The row immediately below the name is not free space — it belongs to the
        /// health bar, and a title placed there is drawn with the bar running through the
        /// glyphs. So the gap is measured from the live rects every frame rather than
        /// assumed from a constant: the health bar, the guild tag and the raised name
        /// pivot are all toggled at runtime, and a number that reads correctly in one
        /// combination is wrong in the others.
        ///
        /// Positions are read as localPosition, which is the pivot's place in the
        /// nameplate's own space whatever a child's anchors happen to be; anchoredPosition
        /// would only be comparable between children that share an anchor.
        /// </summary>
        private static void Position(GameObject hudBase, TextMeshProUGUI nameLabel, TextMeshProUGUI label)
        {
            var rect = (RectTransform)label.transform;
            var nameRect = nameLabel.rectTransform;

            var height = Mathf.Max(rect.rect.height, 1f);
            var ceiling = nameRect.localPosition.y - nameRect.rect.height * nameRect.pivot.y;

            // Guilds already occupies the first slot below the name, so the title takes
            // the space above the guild tag instead of the space above the bar.
            var floor = TryTopEdge(hudBase.transform, GuildsChildName, out var guildTop, requireText: true)
                ? guildTop
                : TryTopEdge(hudBase.transform, HealthChildName, out var healthTop, requireText: false)
                    ? healthTop
                    : ceiling - height - Pad * 2f;

            // Centred in the gap, but never so far down that it lands on whatever set the
            // floor, and never riding up into the name when the gap is a tight one.
            var y = Mathf.Clamp(
                (ceiling + floor) * 0.5f,
                floor + height * 0.5f + Pad,
                ceiling - height * 0.5f - Pad);

            y += BynameConfig.SecondLineNudge.Value;

            var pos = rect.localPosition;
            var x = nameRect.localPosition.x;
            if (!Mathf.Approximately(pos.y, y) || !Mathf.Approximately(pos.x, x))
            {
                rect.localPosition = new Vector3(x, y, pos.z);
            }

            // When Guilds is installed it owns the name pivot and rewrites it every frame;
            // fighting over it would just flicker. Only claim the pivot when Guilds is not
            // showing anything. Raising it lifts the name clear of the health bar, which is
            // what opens the gap the title is slotted into.
            if (TryTopEdge(hudBase.transform, GuildsChildName, out _, requireText: true)) return;

            var pivot = nameRect.pivot;
            if (!Mathf.Approximately(pivot.y, 0.2f))
            {
                pivot.y = 0.2f;
                nameRect.pivot = pivot;
            }
        }

        /// <summary>
        /// Top edge of a nameplate child in the nameplate's own space, skipping children
        /// that are switched off — and, when <paramref name="requireText"/> is set, ones
        /// that are present but currently showing nothing.
        /// </summary>
        private static bool TryTopEdge(Transform hudBase, string child, out float top, bool requireText)
        {
            top = 0f;

            if (!(hudBase.Find(child) is RectTransform rect)) return false;
            if (!rect.gameObject.activeInHierarchy) return false;

            if (requireText)
            {
                var text = rect.GetComponent<TextMeshProUGUI>();
                if (text == null || string.IsNullOrEmpty(text.text)) return false;
            }

            top = rect.localPosition.y + rect.rect.height * (1f - rect.pivot.y);
            return true;
        }

        /// <summary>
        /// A line's worth of height for the title, from the name's own font size. Falls
        /// back to the name's rect when that label is auto-sizing and reports 0.
        /// </summary>
        private static float LineHeight(TextMeshProUGUI nameLabel)
        {
            var size = nameLabel.fontSize * 0.8f;
            if (size > 1f) return size * 1.2f;

            var rect = nameLabel.rectTransform.rect.height;
            return rect > 1f ? rect : 12f;
        }

        private static void Prune()
        {
            foreach (var dead in Labels.Where(p => p.Key == null || p.Value == null)
                         .Select(p => p.Key).ToList())
            {
                Labels.Remove(dead);
            }
        }
    }

    /// <summary>
    /// The same title, appended to the name on one line.
    ///
    /// This mode adds no vertical space at all, so it cannot collide with Guilds however
    /// that mod lays itself out. It exists as a real option rather than a fallback
    /// because whether a stacked name / guild / title plate looks cramped is a question
    /// only play-testing can answer.
    /// </summary>
    [HarmonyPatch(typeof(Player), nameof(Player.GetHoverName))]
    internal static class HoverNameSuffixPatch
    {
        private static void Postfix(Player __instance, ref string __result)
        {
            if (!BynameConfig.Enabled.Value) return;
            if (BynameConfig.Placement.Value != NameplatePlacement.Suffix) return;

            var title = TitleNet.Read(__instance, out var rarity);
            if (string.IsNullOrEmpty(title)) return;

            __result = $"{__result}, {TitleService.Decorate(title, rarity)}";
        }
    }
}
