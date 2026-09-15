namespace Byname.Stats
{
    /// <summary>
    /// Read-only view of what a character has done, so fragment predicates do not care
    /// whether they are being judged on a lifetime or on one world's worth of deeds.
    /// </summary>
    internal interface IStatSource
    {
        float Get(PlayerStatType stat);

        /// <summary>Kills of one creature prefab, optionally narrowed to a weapon class.</summary>
        float GetEnemyKills(string prefabName, KillModifiers modifier = KillModifiers.MixedAndTotal);

        /// <summary>The creature this character has killed most, or null if it has killed nothing.</summary>
        string TopEnemy();

        /// <summary>
        /// The weapon class this character kills with most. Returns
        /// <see cref="KillModifiers.MixedAndTotal"/> when nothing dominates, which reads
        /// as "no particular style" rather than as a false claim of specialism.
        /// </summary>
        KillModifiers DominantKillStyle(float minimumShare = 0.6f);
    }
}
