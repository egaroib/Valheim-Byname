using System.Collections.Generic;

namespace Byname.Titles.Catalog
{
    /// <summary>
    /// Killing, and how you go about it.
    ///
    /// The weapon-class fragments lean on <c>m_enemyStats</c> being split by
    /// <see cref="KillModifiers"/>, so "Bare-Fisted" means the player genuinely kills with
    /// their hands rather than merely having punched something once. Boss fragments
    /// separate solo from group kills, which Valheim records distinctly.
    /// </summary>
    internal static class WarFragments
    {
        internal static IEnumerable<TitleFragment> All => new[]
        {
            // --- Kill style. Requires a real majority, not a single instance. --
            new TitleFragment("bare_fisted", TitleSlot.Epithet, TitleCategory.Combat,
                Rarity.Legendary, "Bare-Fisted",
                s => s.DominantKillStyle() == KillModifiers.Unarmed &&
                     s.Get(PlayerStatType.EnemyKills) >= 100,
                new[] { PlayerStatType.EnemyKills }),

            new TitleFragment("far_shooting", TitleSlot.Epithet, TitleCategory.Combat,
                Rarity.Rare, "Far-Shooting",
                s => s.DominantKillStyle() == KillModifiers.Ranged &&
                     s.Get(PlayerStatType.EnemyKills) >= 200,
                new[] { PlayerStatType.EnemyKills }),

            new TitleFragment("rune_handed", TitleSlot.Epithet, TitleCategory.Combat,
                Rarity.Epic, "Rune-Handed",
                s => s.DominantKillStyle() == KillModifiers.Magic &&
                     s.Get(PlayerStatType.EnemyKills) >= 200,
                new[] { PlayerStatType.EnemyKills }),

            new TitleFragment("close_fighting", TitleSlot.Epithet, TitleCategory.Combat,
                Rarity.Uncommon, "Close-Fighting",
                s => s.DominantKillStyle() == KillModifiers.Melee &&
                     s.Get(PlayerStatType.EnemyKills) >= 300,
                new[] { PlayerStatType.EnemyKills }),

            // --- Volume ------------------------------------------------------
            TitleFragment.Threshold("blood_worn", TitleSlot.Epithet, TitleCategory.Combat,
                Rarity.Uncommon, "Blood-Worn", PlayerStatType.EnemyKills, 500),
            TitleFragment.Threshold("much_feared", TitleSlot.Epithet, TitleCategory.Combat,
                Rarity.Rare, "Much-Feared", PlayerStatType.EnemyKills, 2500),
            TitleFragment.Threshold("red_handed", TitleSlot.Epithet, TitleCategory.Combat,
                Rarity.Epic, "Red-Handed", PlayerStatType.EnemyKills, 7500),
            TitleFragment.Threshold("shield_worn", TitleSlot.Epithet, TitleCategory.Combat,
                Rarity.Uncommon, "Shield-Worn", PlayerStatType.HitsTakenEnemies, 2000),
            TitleFragment.Threshold("arrow_glad", TitleSlot.Epithet, TitleCategory.Combat,
                Rarity.Uncommon, "Arrow-Glad", PlayerStatType.ArrowsShot, 2000),
            TitleFragment.Threshold("bone_calling", TitleSlot.Epithet, TitleCategory.Combat,
                Rarity.Rare, "Bone-Calling", PlayerStatType.SkeletonSummons, 50),

            TitleFragment.Threshold("slayer", TitleSlot.Noun, TitleCategory.Combat,
                Rarity.Common, "Slayer", PlayerStatType.EnemyKills, 200),
            TitleFragment.Threshold("hunter", TitleSlot.Noun, TitleCategory.Combat,
                Rarity.Uncommon, "Hunter", PlayerStatType.EnemyKillsLastHits, 500),
            TitleFragment.Threshold("bowman", TitleSlot.Noun, TitleCategory.Combat,
                Rarity.Uncommon, "Bowman", PlayerStatType.ArrowsShot, 1000),
            TitleFragment.Threshold("shieldbearer", TitleSlot.Noun, TitleCategory.Combat,
                Rarity.Rare, "Shieldbearer", PlayerStatType.HitsTakenEnemies, 5000),
            TitleFragment.Threshold("bonecaller", TitleSlot.Noun, TitleCategory.Combat,
                Rarity.Epic, "Bonecaller", PlayerStatType.SkeletonSummons, 150),
            TitleFragment.Threshold("trapper", TitleSlot.Noun, TitleCategory.Combat,
                Rarity.Rare, "Trapper", PlayerStatType.TrapTriggered, 25),

            TitleFragment.Threshold("red_field", TitleSlot.Domain, TitleCategory.Combat,
                Rarity.Uncommon, "Red Field", PlayerStatType.EnemyKills, 1000),
            TitleFragment.Threshold("long_war", TitleSlot.Domain, TitleCategory.Combat,
                Rarity.Rare, "Long War", PlayerStatType.EnemyHits, 20000),

            // --- Bosses ------------------------------------------------------
            TitleFragment.Threshold("crown_breaking", TitleSlot.Epithet, TitleCategory.Bosses,
                Rarity.Rare, "Crown-Breaking", PlayerStatType.BossKills, 5),
            TitleFragment.Threshold("god_touched", TitleSlot.Epithet, TitleCategory.Bosses,
                Rarity.Epic, "God-Touched", PlayerStatType.UseGuardianPower, 200),
            TitleFragment.Threshold("alone_going", TitleSlot.Epithet, TitleCategory.Bosses,
                Rarity.Epic, "Alone-Going", PlayerStatType.BossKillSolo, 3),

            TitleFragment.Threshold("bane", TitleSlot.Noun, TitleCategory.Bosses,
                Rarity.Rare, "Bane", PlayerStatType.BossKills, 3),
            TitleFragment.Threshold("lone_slayer", TitleSlot.Noun, TitleCategory.Bosses,
                Rarity.Legendary, "Lone-Slayer", PlayerStatType.BossKillSolo, 5),
            TitleFragment.Threshold("kingsbane", TitleSlot.Noun, TitleCategory.Bosses,
                Rarity.Legendary, "Kingsbane", PlayerStatType.BossKills, 8),
            TitleFragment.Threshold("deathbringer", TitleSlot.Noun, TitleCategory.Bosses,
                Rarity.Epic, "Deathbringer", PlayerStatType.BossLastHits, 3),

            TitleFragment.Threshold("nine_crowns", TitleSlot.Domain, TitleCategory.Bosses,
                Rarity.Legendary, "Nine Crowns", PlayerStatType.BossKills, 8),
            TitleFragment.Threshold("elder_wood", TitleSlot.Domain, TitleCategory.Bosses,
                Rarity.Rare, "Elder Wood", PlayerStatType.UsePowerElder, 25),
            TitleFragment.Threshold("frozen_peak", TitleSlot.Domain, TitleCategory.Bosses,
                Rarity.Rare, "Frozen Peak", PlayerStatType.UsePowerModer, 25),
            TitleFragment.Threshold("black_mire", TitleSlot.Domain, TitleCategory.Bosses,
                Rarity.Rare, "Black Mire", PlayerStatType.UsePowerBonemass, 25),
        };
    }
}
