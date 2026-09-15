using System;
using System.Collections.Generic;
using Byname.Config;
using Byname.Net;
using Byname.Stats;
using UnityEngine;

namespace Byname.Titles
{
    /// <summary>
    /// The one place a title is decided, stored, published and announced.
    ///
    /// Every trigger calls <see cref="Evaluate"/>. Triggers are opportunities to
    /// reconsider, not orders to change: if the recomputed title matches the standing
    /// one, this does nothing visible. That is what keeps a title from churning on every
    /// login and every night's sleep when the player has done nothing notable.
    /// </summary>
    internal static class TitleService
    {
        /// <summary>Rarity tint, indexed by <see cref="Rarity"/>. Readable on the dark nameplate outline.</summary>
        private static readonly Dictionary<Rarity, string> RarityColors = new Dictionary<Rarity, string>
        {
            { Rarity.Common,    "#C8C8C8" },
            { Rarity.Uncommon,  "#7FC77F" },
            { Rarity.Rare,      "#6FA8DC" },
            { Rarity.Epic,      "#B490D9" },
            { Rarity.Legendary, "#E0A44C" },
        };

        internal static void Evaluate(Player player, string reason)
        {
            if (!BynameConfig.Enabled.Value) return;
            if (player == null || player != Player.m_localPlayer) return;
            if (Game.instance == null) return;

            try
            {
                EvaluateCore(player, reason);
            }
            catch (Exception e)
            {
                // A title is cosmetic. It must never be the reason a spawn or a boss kill
                // throws, so the whole evaluation is contained.
                BynamePlugin.LogError($"title evaluation failed on '{reason}': {e}");
            }
        }

        private static void EvaluateCore(Player player, string reason)
        {
            var stats = BuildStatSource(player);
            var day = EnvMan.instance != null ? EnvMan.instance.GetDay() : 0;
            var lastChangeDay = TitleState.GetLastChangeDay(player);
            var epoch = TitleState.GetEpoch(player);

            var stalenessDays = BynameConfig.StalenessDays.Value;
            var stale = stalenessDays > 0
                        && lastChangeDay >= 0
                        && day - lastChangeDay >= stalenessDays;

            // Staleness is the only thing that forces a change. It advances the epoch,
            // which reshuffles tie-breaks so an equally-worthy title can win instead.
            if (stale) epoch++;

            var composed = TitleEngine.Compose(stats, player.GetPlayerID(), epoch);
            if (composed == null)
            {
                BynamePlugin.LogVerbose($"{reason}: no title could be composed");
                return;
            }

            var standing = TitleState.GetTitle(player);
            var changed = !string.Equals(standing, composed.Text, StringComparison.Ordinal);

            // Republished unconditionally: cheap, and it restores the nameplate for peers
            // after a relog without waiting for the next real trigger.
            TitleNet.Publish(player, composed.Text, (int)composed.Rarity);

            if (!changed && !stale)
            {
                BynamePlugin.LogVerbose($"{reason}: title unchanged ({composed.Describe()})");
                return;
            }

            // Persisted even when a stale reroll produced the same text, otherwise the
            // staleness clock never resets and it would reroll on every trigger forever.
            TitleState.SetEpoch(player, epoch);
            TitleState.SetLastChangeDay(player, day);
            TitleState.SetTitle(player, composed.Text, (int)composed.Rarity);

            if (!changed)
            {
                BynamePlugin.LogVerbose($"{reason}: stale reroll landed on the same title");
                return;
            }

            BynamePlugin.LogInfo(
                $"{reason}: {(standing == null ? "first title" : "retitled from \"" + standing + "\"")} " +
                $"-> {composed.Describe()}");

            Announce(player, composed, firstEver: standing == null);
        }

        private static IStatSource BuildStatSource(Player player)
        {
            var lifetime = new LifetimeStatSource(Game.instance.GetPlayerProfile());
            if (BynameConfig.Scope.Value != StatScope.World) return lifetime;

            var worldUid = ZNet.instance != null ? ZNet.instance.GetWorldUID() : 0L;
            var baseline = TitleState.GetBaseline(player, worldUid);

            if (baseline == null)
            {
                // First time this character has set foot in this world. Snapshot now, so
                // everything from here on is measured against arrival.
                TitleState.SetBaseline(player, worldUid, FragmentCatalog.TrackedStats, lifetime);
                baseline = TitleState.GetBaseline(player, worldUid)
                           ?? new Dictionary<PlayerStatType, float>();
                BynamePlugin.LogInfo($"World scope: baselined {baseline.Count} stat(s) for world {worldUid}.");
            }

            return new WorldStatSource(lifetime, baseline);
        }

        private static void Announce(Player player, ComposedTitle composed, bool firstEver)
        {
            var verb = firstEver ? "You are known as" : "You are now known as";
            var line = $"{verb} {Decorate(composed)}";

            if (BynameConfig.ShowChangeToast.Value)
            {
                player.Message(MessageHud.MessageType.Center, line);
            }

            // The toast fades; the chat line does not. Terminal.AddString appends to the
            // local chat buffer and refreshes the window — it sends nothing over the
            // network, so this stays a private note to the player rather than an
            // announcement to the server.
            if (BynameConfig.ShowChangeInChat.Value)
            {
                if (Chat.instance != null)
                {
                    Chat.instance.AddString(line);
                }
                else
                {
                    BynamePlugin.LogVerbose("chat line skipped: Chat.instance not ready yet");
                }
            }
        }

        /// <summary>
        /// Wraps the title in its rarity colour.
        ///
        /// Only ever the title. Groups tints the player-name label to mark group members,
        /// and a TMP inline colour tag beats the component colour — so colouring a span
        /// that included the name would silently break that mod and look like their bug.
        /// </summary>
        internal static string Decorate(ComposedTitle composed) =>
            Decorate(composed.Text, (int)composed.Rarity);

        internal static string Decorate(string text, int rarity)
        {
            if (string.IsNullOrEmpty(text)) return text;
            if (!BynameConfig.ShowRarityColor.Value) return text;

            var key = (Rarity)Mathf.Clamp(rarity, (int)Rarity.Common, (int)Rarity.Legendary);
            return RarityColors.TryGetValue(key, out var hex)
                ? $"<color={hex}>{text}</color>"
                : text;
        }
    }
}
