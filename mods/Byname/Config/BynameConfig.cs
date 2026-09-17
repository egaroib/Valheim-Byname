using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Configuration;
using Byname.Titles;

namespace Byname.Config
{
    /// <summary>Which pool of stats a title is judged against.</summary>
    internal enum StatScope
    {
        /// <summary>
        /// Everything this character has ever done, in any world. Uses Valheim's own
        /// lifetime tally, so existing characters are titled correctly on first login.
        /// </summary>
        Lifetime,

        /// <summary>
        /// Only what this character has done since first joining this world. Every
        /// player starts untitled on a fresh server, at the cost of deeds done before
        /// the mod was installed being invisible.
        /// </summary>
        World,
    }

    /// <summary>Where the title sits relative to the player name on the nameplate.</summary>
    internal enum NameplatePlacement
    {
        /// <summary>
        /// Its own line beneath the name, in its own GameObject. Reads best, but shares
        /// vertical space with Guilds' guild tag.
        /// </summary>
        SecondLine,

        /// <summary>
        /// Appended to the name on one line. Adds no vertical space, so it cannot
        /// collide with Guilds no matter how that mod lays itself out.
        /// </summary>
        Suffix,
    }

    /// <summary>
    /// Every value the mod can be tuned on.
    ///
    /// The split follows the same rule as the rest of this workspace: anything that
    /// decides *which* title a player can earn is a game rule and is bound admin-only,
    /// so a server dictates it and a client cannot quietly re-enable a category the
    /// admin switched off. Anything that only changes what the local player sees on
    /// their own screen is deliberately not admin-only.
    /// </summary>
    internal static class BynameConfig
    {
        // --- Diagnostics -----------------------------------------------------
        internal static ConfigEntry<bool> VerboseLogging;

        // --- Rules (admin) ---------------------------------------------------
        internal static ConfigEntry<bool> Enabled;
        internal static ConfigEntry<StatScope> Scope;
        internal static ConfigEntry<int> StalenessDays;
        internal static ConfigEntry<int> MaxTitleLength;
        internal static ConfigEntry<string> Blocklist;

        // --- Appearance (local) ----------------------------------------------
        internal static ConfigEntry<NameplatePlacement> Placement;
        internal static ConfigEntry<float> SecondLineOffset;
        internal static ConfigEntry<bool> ShowRarityColor;
        internal static ConfigEntry<bool> ShowChangeToast;
        internal static ConfigEntry<bool> ShowChangeInChat;
        internal static ConfigEntry<bool> ShowOnCharacterPanel;

        private static readonly Dictionary<TitleCategory, ConfigEntry<bool>> Categories =
            new Dictionary<TitleCategory, ConfigEntry<bool>>();

        private static readonly Dictionary<TitleCategory, ConfigEntry<float>> Weights =
            new Dictionary<TitleCategory, ConfigEntry<float>>();

        private static readonly Dictionary<TitleCategory, ConfigEntry<float>> Thresholds =
            new Dictionary<TitleCategory, ConfigEntry<float>>();

        private static HashSet<string> _blockedCache;
        private static string _blockedCacheRaw;

        internal static void Bind(ConfigFile config)
        {
            VerboseLogging = config.Bind(
                "General", "VerboseLogging", false,
                "Log title evaluation in detail: every qualifying fragment, the winner, " +
                "and the closest near-misses with their remaining thresholds. Noisy; " +
                "this is the mod's debugging surface, so turn it on before reporting a " +
                "title that looks wrong.");

            Enabled = config.Bind(
                "General", "Enabled", true,
                new ConfigDescription(
                    "Master switch. When off, no titles are computed, published or drawn.",
                    null, AdminOnly()));

            Scope = config.Bind(
                "Rules", "StatScope", StatScope.Lifetime,
                new ConfigDescription(
                    "Lifetime judges a character on everything it has ever done, in any " +
                    "world, and titles existing characters retroactively. World judges it " +
                    "only on deeds done since first joining this server, so everyone " +
                    "starts unproven and deeds predating the mod do not count.",
                    null, AdminOnly()));

            StalenessDays = config.Bind(
                "Rules", "StalenessDays", 10,
                new ConfigDescription(
                    "In-game days a title may stand unchanged before the mod deliberately " +
                    "rerolls it among the player's equally-worthy titles. This is the only " +
                    "thing that forces a change: ordinary triggers recompute the title and " +
                    "stay silent unless the result actually differs. Set 0 to never reroll.",
                    new AcceptableValueRange<int>(0, 200), AdminOnly()));

            MaxTitleLength = config.Bind(
                "Rules", "MaxTitleLength", 32,
                new ConfigDescription(
                    "Longest rendered title in characters. Templates whose result exceeds " +
                    "this are skipped in favour of a shorter one, which is what keeps " +
                    "connector-heavy patterns from sprawling across the nameplate.",
                    new AcceptableValueRange<int>(8, 64), AdminOnly()));

            Blocklist = config.Bind(
                "Rules", "Blocklist", "",
                new ConfigDescription(
                    "Comma-separated fragment ids never to use, for surgical removal " +
                    "without disabling a whole category. Ids are logged when " +
                    "VerboseLogging is on. Example: drowned,tree_struck",
                    null, AdminOnly()));

            Placement = config.Bind(
                "Appearance", "Placement", NameplatePlacement.SecondLine,
                "SecondLine draws the title on its own line under the name. Suffix appends " +
                "it to the name instead, adding no vertical space — use Suffix if you " +
                "run Guilds and the stacked name/guild/title plate looks cramped.");

            SecondLineOffset = config.Bind(
                "Appearance", "SecondLineOffset", -12f,
                new ConfigDescription(
                    "Vertical offset of the title line below the player name, in nameplate " +
                    "units. Doubled automatically when Guilds is showing a guild tag, so " +
                    "the title sits below it rather than on top of it. Exposed because the " +
                    "nameplate layout lives in a Unity prefab and the right value is " +
                    "whatever looks correct on your screen.",
                    new AcceptableValueRange<float>(-40f, 0f)));

            ShowRarityColor = config.Bind(
                "Appearance", "ShowRarityColor", true,
                "Tint the title by rarity, so a hard-won one is visibly rare. Only the " +
                "title is tinted, never the player name — Groups colours the name to " +
                "mark group members and must keep winning that.");

            ShowChangeToast = config.Bind(
                "Appearance", "ShowChangeToast", true,
                "Show a centre-screen message the moment your own title changes. Without " +
                "this you will rarely see your own title, since Valheim never draws a " +
                "nameplate for the local player.");

            ShowChangeInChat = config.Bind(
                "Appearance", "ShowChangeInChat", true,
                "Also write the change into your chat window, where it stays in the " +
                "scrollback instead of fading like the toast. Written straight to your " +
                "own chat buffer — nothing is sent to the server and no other player " +
                "sees it.");

            ShowOnCharacterPanel = config.Bind(
                "Appearance", "ShowOnCharacterPanel", true,
                "Show your current title under your name on the inventory screen.");

            foreach (TitleCategory category in Enum.GetValues(typeof(TitleCategory)))
            {
                Categories[category] = config.Bind(
                    "Categories", category.ToString(), true,
                    new ConfigDescription(
                        $"Allow titles drawn from {CategoryBlurb(category)}.",
                        null, AdminOnly()));
            }

            foreach (TitleCategory category in Enum.GetValues(typeof(TitleCategory)))
            {
                Weights[category] = config.Bind(
                    "Weights", category.ToString(), CategoryDefaults.Weight(category),
                    new ConfigDescription(
                        $"How strongly titles drawn from {CategoryBlurb(category)} compete for a " +
                        "place. A word's rarity (Common 1 to Legendary 5) is multiplied by this. " +
                        "Lower it to make a category show up less, raise it to make it show up " +
                        "more. It never changes which words are earned or the colour they show in.",
                        new AcceptableValueRange<float>(0.1f, 3f), AdminOnly()));
            }

            foreach (TitleCategory category in Enum.GetValues(typeof(TitleCategory)))
            {
                Thresholds[category] = config.Bind(
                    "Thresholds", category.ToString(), 1f,
                    new ConfigDescription(
                        $"Multiplies every requirement for titles drawn from {CategoryBlurb(category)}. " +
                        "2 means twice the walls, twice the distance, twice the deaths before a " +
                        "word is earned. Rounded up, so it also turns one-off deeds into repeat ones.",
                        new AcceptableValueRange<float>(0.1f, 10f), AdminOnly()));
            }
        }

        /// <summary>The admin's selection weight for a category. See the Weights section.</summary>
        internal static float Weight(TitleCategory category) =>
            Weights.TryGetValue(category, out var entry) ? entry.Value : CategoryDefaults.Weight(category);

        /// <summary>The admin's threshold multiplier for a category. See the Thresholds section.</summary>
        internal static float ThresholdScale(TitleCategory category) =>
            Thresholds.TryGetValue(category, out var entry) ? entry.Value : 1f;

        /// <summary>True when this category may contribute fragments to a title.</summary>
        internal static bool CategoryEnabled(TitleCategory category) =>
            !Categories.TryGetValue(category, out var entry) || entry.Value;

        /// <summary>True when this fragment id has been blocked by the server admin.</summary>
        internal static bool IsBlocked(string fragmentId)
        {
            var raw = Blocklist?.Value ?? "";

            // Reparsed only when the admin actually changes the value, since this is
            // consulted once per fragment per evaluation.
            if (_blockedCache == null || !string.Equals(raw, _blockedCacheRaw, StringComparison.Ordinal))
            {
                _blockedCacheRaw = raw;
                _blockedCache = new HashSet<string>(
                    raw.Split(',').Select(s => s.Trim()).Where(s => s.Length > 0),
                    StringComparer.OrdinalIgnoreCase);
            }

            return _blockedCache.Contains(fragmentId);
        }

        private static string CategoryBlurb(TitleCategory category)
        {
            switch (category)
            {
                case TitleCategory.Combat: return "how and how much you kill";
                case TitleCategory.Bosses: return "the bosses you have felled";
                case TitleCategory.Building: return "what and how much you build";
                case TitleCategory.Death: return "how you keep dying";
                case TitleCategory.Harvest: return "foraging, farming, bees and sap";
                case TitleCategory.Travel: return "distance walked, run, sailed and flown";
                case TitleCategory.Exploration: return "how far out you have pushed the map";
                case TitleCategory.Taming: return "the creatures you have tamed and petted";
                case TitleCategory.Fishing: return "what you have pulled out of the water";
                case TitleCategory.Cooking: return "what you cook and eat";
                case TitleCategory.Crafting: return "what you craft and upgrade";
                default: return "odds and ends that fit nowhere else";
            }
        }

        private static ConfigurationManagerAttributes AdminOnly() =>
            new ConfigurationManagerAttributes { IsAdminOnly = true };
    }
}
