using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Byname.Stats
{
    /// <summary>
    /// Byname's own per-character memory: which title is standing, when it last changed,
    /// how many times it has been deliberately rerolled, and the World-scope baselines.
    ///
    /// Stored in <c>Player.m_customData</c>, which Valheim serialises into the character
    /// save (Player.cs:4752 writes it, Player.cs:4977 reads it back). That means this
    /// state travels with the character exactly like the stats it is derived from, with
    /// no side file to fall out of sync, and it survives a world being deleted.
    /// </summary>
    internal static class TitleState
    {
        private const string KeyEpoch = "byname.epoch";
        private const string KeyDay = "byname.day";
        private const string KeyTitle = "byname.title";
        private const string KeyRarity = "byname.rarity";
        private const string BaselinePrefix = "byname.base.";

        /// <summary>
        /// Advances only when staleness forces a reroll. Feeding it into the selection
        /// seed is what makes an ordinary trigger reproduce the same title byte for byte
        /// when nothing has happened, and makes a stale one land somewhere new.
        /// </summary>
        internal static int GetEpoch(Player player) => ReadInt(player, KeyEpoch, 0);

        internal static void SetEpoch(Player player, int epoch) =>
            Write(player, KeyEpoch, epoch.ToString(CultureInfo.InvariantCulture));

        /// <summary>In-game day the standing title was set, for the staleness check.</summary>
        internal static int GetLastChangeDay(Player player) => ReadInt(player, KeyDay, -1);

        internal static void SetLastChangeDay(Player player, int day) =>
            Write(player, KeyDay, day.ToString(CultureInfo.InvariantCulture));

        internal static string GetTitle(Player player) =>
            player != null && player.m_customData.TryGetValue(KeyTitle, out var v) ? v : null;

        internal static int GetRarity(Player player) => ReadInt(player, KeyRarity, 1);

        internal static void SetTitle(Player player, string title, int rarity)
        {
            Write(player, KeyTitle, title ?? "");
            Write(player, KeyRarity, rarity.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// The World-scope baseline for one world, or null if this character has never
        /// been here before. Only stats some fragment actually reads are stored, which is
        /// what keeps this short enough to sit in the character save.
        /// </summary>
        internal static IReadOnlyDictionary<PlayerStatType, float> GetBaseline(Player player, long worldUid)
        {
            if (player == null) return null;
            if (!player.m_customData.TryGetValue(BaselinePrefix + worldUid, out var raw)) return null;

            var result = new Dictionary<PlayerStatType, float>();
            foreach (var entry in raw.Split(';'))
            {
                if (entry.Length == 0) continue;
                var split = entry.Split(':');
                if (split.Length != 2) continue;
                if (!int.TryParse(split[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out var key)) continue;
                if (!float.TryParse(split[1], NumberStyles.Float, CultureInfo.InvariantCulture, out var value)) continue;
                result[(PlayerStatType)key] = value;
            }
            return result;
        }

        internal static void SetBaseline(
            Player player, long worldUid, IEnumerable<PlayerStatType> tracked, LifetimeStatSource lifetime)
        {
            if (player == null) return;

            var parts = tracked
                .Distinct()
                .Select(s => ((int)s).ToString(CultureInfo.InvariantCulture) + ":" +
                             lifetime.Get(s).ToString("R", CultureInfo.InvariantCulture));

            Write(player, BaselinePrefix + worldUid, string.Join(";", parts.ToArray()));
        }

        private static int ReadInt(Player player, string key, int fallback)
        {
            if (player == null) return fallback;
            return player.m_customData.TryGetValue(key, out var raw) &&
                   int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var v)
                ? v
                : fallback;
        }

        private static void Write(Player player, string key, string value)
        {
            if (player == null) return;
            player.m_customData[key] = value;
        }
    }
}
