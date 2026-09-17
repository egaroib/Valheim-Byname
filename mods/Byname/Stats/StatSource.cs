using System.Collections.Generic;
using System.Linq;

namespace Byname.Stats
{
    /// <summary>
    /// Everything this character has ever done, in any world.
    ///
    /// Reads <c>m_playerStats[0]</c> directly rather than going through
    /// <c>PlayerProfile.GetStat</c>. That is deliberate. GetStat returns the
    /// achievement-difficulty-indexed copy, which stops accumulating the moment
    /// <c>Achievements.CanGetAchievements()</c> turns false — so a player who once typed
    /// <c>devcommands</c> would have their title silently frozen forever. Index 0 is the
    /// raw tally and is always incremented (PlayerProfile.IncrementStat), cheats or not.
    /// </summary>
    internal sealed class LifetimeStatSource : IStatSource
    {
        private readonly PlayerProfile.PlayerStats _stats;
        private readonly DeathLedger _deaths;

        internal LifetimeStatSource(PlayerProfile profile, DeathLedger deaths)
        {
            _stats = profile.m_playerStats[0];
            _deaths = deaths ?? DeathLedger.Empty;
        }

        internal PlayerProfile.PlayerStats Raw => _stats;

        public float Get(PlayerStatType stat) =>
            _stats.m_stats.TryGetValue(stat, out var v) ? v : 0f;

        public float GetEnemyKills(string prefabName, KillModifiers modifier = KillModifiers.MixedAndTotal)
        {
            var index = (int)modifier;
            if (index < 0 || index >= _stats.m_enemyStats.Length) return 0f;

            var dict = _stats.m_enemyStats[index];
            if (dict == null || string.IsNullOrEmpty(prefabName)) return 0f;

            // The dictionary is keyed by Character.m_name, which Valheim hands to
            // Localization.Localize — so it is a localisation token. Whether that token
            // carries its leading '$' could not be confirmed: the Localization class lives
            // in an assembly this workspace does not decompile. Rather than guess and ship
            // fragments that silently never fire, both forms are tried. Costs one extra
            // dictionary miss; removes the whole failure mode.
            if (dict.TryGetValue(prefabName, out var exact)) return exact;

            var alternate = prefabName[0] == '$' ? prefabName.Substring(1) : "$" + prefabName;
            return dict.TryGetValue(alternate, out var v) ? v : 0f;
        }

        public string TopEnemy()
        {
            var all = _stats.m_enemyStats[(int)KillModifiers.MixedAndTotal];
            if (all == null || all.Count == 0) return null;

            string best = null;
            var bestCount = 0f;
            foreach (var pair in all)
            {
                if (pair.Value > bestCount)
                {
                    bestCount = pair.Value;
                    best = pair.Key;
                }
            }
            return best;
        }

        public KillModifiers DominantKillStyle(float minimumShare = 0.6f)
        {
            var styles = new[]
            {
                KillModifiers.Unarmed, KillModifiers.Magic,
                KillModifiers.Ranged, KillModifiers.Melee,
            };

            var totals = styles.ToDictionary(s => s, s => Sum(_stats.m_enemyStats[(int)s]));
            var overall = totals.Values.Sum();
            if (overall <= 0f) return KillModifiers.MixedAndTotal;

            var leader = totals.OrderByDescending(p => p.Value).First();
            return leader.Value / overall >= minimumShare ? leader.Key : KillModifiers.MixedAndTotal;
        }

        public float GetDeathsBy(string creatureToken) => _deaths.By(creatureToken);

        public float GetDeathsIn(string biome) => _deaths.In(biome);

        private static float Sum(Dictionary<string, float> d)
        {
            if (d == null) return 0f;
            var total = 0f;
            foreach (var v in d.Values) total += v;
            return total;
        }
    }

    /// <summary>
    /// Only what this character has done since first joining the current world.
    ///
    /// Valheim keeps no per-world stat tally, so this is a difference against a baseline
    /// snapshot taken the first time the character entered this world. Consequence worth
    /// stating plainly: deeds done before the mod was installed are invisible here, and
    /// the very first login on an existing character sets the baseline at its current
    /// lifetime totals, which is exactly why this mode starts everyone unproven.
    /// </summary>
    internal sealed class WorldStatSource : IStatSource
    {
        private readonly LifetimeStatSource _lifetime;
        private readonly IReadOnlyDictionary<PlayerStatType, float> _baseline;
        private readonly DeathLedger _worldDeaths;

        internal WorldStatSource(
            LifetimeStatSource lifetime,
            IReadOnlyDictionary<PlayerStatType, float> baseline,
            DeathLedger worldDeaths)
        {
            _lifetime = lifetime;
            _baseline = baseline;
            _worldDeaths = worldDeaths ?? DeathLedger.Empty;
        }

        public float Get(PlayerStatType stat)
        {
            var now = _lifetime.Get(stat);
            var then = _baseline.TryGetValue(stat, out var b) ? b : 0f;

            // Clamped because a baseline can outlive the value it was taken from: deleting
            // and remaking a character reuses the name, and stats restart at zero.
            var delta = now - then;
            return delta > 0f ? delta : 0f;
        }

        // Per-creature tallies are not baselined. They would multiply the snapshot by the
        // number of creature prefabs a player has met, and every fragment that uses them
        // asks "what do you kill most", which is a ratio and stays honest either way.
        public float GetEnemyKills(string prefabName, KillModifiers modifier = KillModifiers.MixedAndTotal)
            => _lifetime.GetEnemyKills(prefabName, modifier);

        public string TopEnemy() => _lifetime.TopEnemy();

        public KillModifiers DominantKillStyle(float minimumShare = 0.6f)
            => _lifetime.DominantKillStyle(minimumShare);

        // The ledger keeps a per-world tally of its own, so no baseline is needed here.
        public float GetDeathsBy(string creatureToken) => _worldDeaths.By(creatureToken);

        public float GetDeathsIn(string biome) => _worldDeaths.In(biome);
    }
}
