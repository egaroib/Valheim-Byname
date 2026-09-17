using System;
using System.Collections.Generic;
using System.Globalization;

namespace Byname.Stats
{
    /// <summary>
    /// Who killed this character, and where.
    ///
    /// Valheim records a death to a monster only as <c>DeathByEnemyHit</c> (Player.cs
    /// OnDeath switch): no creature, no biome. So "died to draugr in the swamp five times"
    /// is not knowable from the game's own stats, and Byname keeps its own tally instead.
    ///
    /// Stored in <c>Player.m_customData</c> beside the rest of TitleState, so it travels
    /// with the character. Everything before installation is lost for good — there is
    /// nothing in the save to reconstruct it from.
    ///
    /// Each death is written twice: once to the lifetime tally, once under the world it
    /// happened in, so World scope counts only this server's deaths without a baseline.
    /// </summary>
    internal sealed class DeathLedger
    {
        private const string Prefix = "byname.died.";
        private const string ByPart = "by.";
        private const string InPart = "in.";

        internal static readonly DeathLedger Empty = new DeathLedger(new Dictionary<string, float>());

        private readonly Dictionary<string, float> _counts;

        private DeathLedger(Dictionary<string, float> counts)
        {
            _counts = counts;
        }

        /// <summary>
        /// The tally for one world, or the lifetime tally when <paramref name="worldUid"/>
        /// is null. Scans the character's custom data once per evaluation, which is a few
        /// dozen keys at most.
        /// </summary>
        internal static DeathLedger Read(Player player, long? worldUid)
        {
            if (player == null) return Empty;

            var scope = Scope(worldUid);
            var counts = new Dictionary<string, float>(StringComparer.Ordinal);

            foreach (var pair in player.m_customData)
            {
                if (!pair.Key.StartsWith(scope, StringComparison.Ordinal)) continue;

                var rest = pair.Key.Substring(scope.Length);
                if (!rest.StartsWith(ByPart, StringComparison.Ordinal) &&
                    !rest.StartsWith(InPart, StringComparison.Ordinal)) continue;

                if (float.TryParse(pair.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var v))
                {
                    counts[rest] = v;
                }
            }

            return new DeathLedger(counts);
        }

        /// <summary>
        /// Deaths to one creature. Tries the token with and without its leading '$', for the
        /// same reason LifetimeStatSource.GetEnemyKills does: the key is whatever
        /// Character.m_name held, and that form could not be confirmed from source.
        /// </summary>
        internal float By(string creatureToken)
        {
            if (string.IsNullOrEmpty(creatureToken)) return 0f;
            if (_counts.TryGetValue(ByPart + creatureToken, out var exact)) return exact;

            var alternate = creatureToken[0] == '$' ? creatureToken.Substring(1) : "$" + creatureToken;
            return _counts.TryGetValue(ByPart + alternate, out var v) ? v : 0f;
        }

        internal float In(string biome) =>
            _counts.TryGetValue(InPart + biome, out var v) ? v : 0f;

        /// <summary>
        /// Records one death. <paramref name="killerToken"/> is null when no creature can be
        /// blamed — a fall with nobody chasing you, another player, your own bomb.
        /// </summary>
        internal static void Record(Player player, long worldUid, string killerToken, Heightmap.Biome biome)
        {
            if (player == null) return;

            foreach (var scope in new[] { Scope(null), Scope(worldUid) })
            {
                if (!string.IsNullOrEmpty(killerToken)) Increment(player, scope + ByPart + killerToken);
                if (biome != Heightmap.Biome.None) Increment(player, scope + InPart + biome);
            }
        }

        private static string Scope(long? worldUid) =>
            worldUid.HasValue
                ? Prefix + "w" + worldUid.Value.ToString(CultureInfo.InvariantCulture) + "."
                : Prefix;

        private static void Increment(Player player, string key)
        {
            player.m_customData.TryGetValue(key, out var raw);
            float.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out var current);
            player.m_customData[key] = (current + 1f).ToString(CultureInfo.InvariantCulture);
        }
    }
}
