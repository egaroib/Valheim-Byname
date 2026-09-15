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
            ((RectTransform)go.transform).sizeDelta = new Vector2(300f, 12f);

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
        /// Places the title under the name, and under Guilds' guild tag when that mod is
        /// present and showing one.
        ///
        /// The offsets are config-exposed rather than hard-coded because the nameplate
        /// layout lives in a Unity prefab, which cannot be read from the decompiled source
        /// — so the right values are whatever looks correct in game.
        /// </summary>
        private static void Position(GameObject hudBase, TextMeshProUGUI nameLabel, TextMeshProUGUI label)
        {
            var offset = BynameConfig.SecondLineOffset.Value;

            var guildLabel = hudBase.transform.Find(GuildsChildName);
            var guildShowing = guildLabel != null
                               && guildLabel.GetComponent<TextMeshProUGUI>() is TextMeshProUGUI g
                               && !string.IsNullOrEmpty(g.text);

            // Guilds already occupies the first slot below the name, so drop one more line.
            var y = guildShowing ? offset * 2f : offset;

            var rect = (RectTransform)label.transform;
            if (!Mathf.Approximately(rect.anchoredPosition.y, y))
            {
                rect.anchoredPosition = new Vector2(0f, y);
            }

            // When Guilds is installed it owns the name pivot and rewrites it every frame;
            // fighting over it would just flicker. Only claim the pivot when Guilds is not
            // showing anything.
            if (guildShowing) return;

            var nameRect = nameLabel.GetComponent<RectTransform>();
            var pivot = nameRect.pivot;
            if (!Mathf.Approximately(pivot.y, 0.2f))
            {
                pivot.y = 0.2f;
                nameRect.pivot = pivot;
            }
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
