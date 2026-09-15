using System.Collections.Generic;

namespace Byname.Titles.Catalog
{
    /// <summary>
    /// The graded axes: what tier of tree you fell, what tier of rock you break, what
    /// quality of fish you land, and which tier of tree eventually flattens you.
    ///
    /// None of these are written by name in the game's source, which is why they look
    /// missing until you find the mechanism:
    ///
    ///   TreeTier0-5        TreeBase.cs:171   switch on m_minToolTier (axe needed)
    ///   MineTier0-5        MineRock5.cs:424  same idea for the pickaxe
    ///   FishCaughtTier0-6  FishingFloat:290  (PlayerStatType)(161 + m_itemData.m_quality)
    ///   DeathByTreeTier0-5 Player.cs:3381    (PlayerStatType)(152 + m_lastHit.m_toolTier)
    ///   TreeFir / TreeOak  TreeBase.m_treeStatType, set per prefab in asset data
    ///
    /// So tier is a proxy for progression, not for species: TreeTier0 is anything a stone
    /// axe fells, TreeTier5 needs the best axe in the game.
    /// </summary>
    internal static class TierFragments
    {
        internal static IEnumerable<TitleFragment> All => new[]
        {
            // --- Felling by required axe tier --------------------------------
            TitleFragment.Threshold("softwood_hewer", TitleSlot.Noun, TitleCategory.Harvest,
                Rarity.Common, "Woodcutter", PlayerStatType.TreeTier0, 200),
            TitleFragment.Threshold("hardwood_hewer", TitleSlot.Noun, TitleCategory.Harvest,
                Rarity.Uncommon, "Hardwood-Hewer", PlayerStatType.TreeTier2, 150),
            TitleFragment.Threshold("ironwood_hewer", TitleSlot.Noun, TitleCategory.Harvest,
                Rarity.Rare, "Ironwood-Hewer", PlayerStatType.TreeTier3, 100),
            TitleFragment.Threshold("ancient_hewer", TitleSlot.Noun, TitleCategory.Harvest,
                Rarity.Epic, "Ancient-Hewer", PlayerStatType.TreeTier4, 60),
            TitleFragment.Threshold("world_hewer", TitleSlot.Epithet, TitleCategory.Harvest,
                Rarity.Legendary, "World-Hewing", PlayerStatType.TreeTier5, 40),

            // --- Tree species, driven by TreeBase.m_treeStatType -------------
            TitleFragment.Threshold("fir_feller", TitleSlot.Noun, TitleCategory.Harvest,
                Rarity.Common, "Fir-Feller", PlayerStatType.TreeFir, 150),
            TitleFragment.Threshold("birch_breaker", TitleSlot.Noun, TitleCategory.Harvest,
                Rarity.Uncommon, "Birch-Breaker", PlayerStatType.TreeBirch, 120),
            TitleFragment.Threshold("mire_hewer", TitleSlot.Noun, TitleCategory.Harvest,
                Rarity.Rare, "Mire-Hewer", PlayerStatType.TreeSwamp, 150),
            TitleFragment.Threshold("shoot_hewer", TitleSlot.Noun, TitleCategory.Harvest,
                Rarity.Legendary, "Shoot-Hewer", PlayerStatType.TreeYggdrasilShoot, 25),
            TitleFragment.Threshold("ash_hewer", TitleSlot.Noun, TitleCategory.Harvest,
                Rarity.Epic, "Ash-Hewer", PlayerStatType.TreeAshlands, 80),
            TitleFragment.Threshold("snowline", TitleSlot.Domain, TitleCategory.Harvest,
                Rarity.Rare, "Snowline", PlayerStatType.TreeSnowFir, 100),

            // --- Mining by required pickaxe tier -----------------------------
            TitleFragment.Threshold("stone_knapper", TitleSlot.Noun, TitleCategory.Harvest,
                Rarity.Common, "Stoneknapper", PlayerStatType.MineTier0, 300),
            TitleFragment.Threshold("iron_delver", TitleSlot.Noun, TitleCategory.Harvest,
                Rarity.Uncommon, "Iron-Delver", PlayerStatType.MineTier2, 200),
            TitleFragment.Threshold("deep_hewer", TitleSlot.Epithet, TitleCategory.Harvest,
                Rarity.Rare, "Deep-Hewing", PlayerStatType.MineTier3, 150),
            TitleFragment.Threshold("blackmetal_delver", TitleSlot.Noun, TitleCategory.Harvest,
                Rarity.Epic, "Black-Delver", PlayerStatType.MineTier4, 100),
            TitleFragment.Threshold("flametal_delver", TitleSlot.Noun, TitleCategory.Harvest,
                Rarity.Legendary, "Flame-Delver", PlayerStatType.MineTier5, 60),

            // --- Fish by quality ---------------------------------------------
            TitleFragment.Threshold("small_catch", TitleSlot.Noun, TitleCategory.Fishing,
                Rarity.Common, "Minnow-Taker", PlayerStatType.FishCaughtTier0, 40),
            TitleFragment.Threshold("fair_catch", TitleSlot.Epithet, TitleCategory.Fishing,
                Rarity.Uncommon, "Sure-Handed", PlayerStatType.FishCaughtTier1, 30),
            TitleFragment.Threshold("fine_catch", TitleSlot.Noun, TitleCategory.Fishing,
                Rarity.Rare, "Deep-Catcher", PlayerStatType.FishCaughtTier2, 25),
            TitleFragment.Threshold("great_catch", TitleSlot.Noun, TitleCategory.Fishing,
                Rarity.Epic, "Great-Catcher", PlayerStatType.FishCaughtTier3, 15),
            TitleFragment.Threshold("legend_catch", TitleSlot.Epithet, TitleCategory.Fishing,
                Rarity.Legendary, "Deep-Favoured", PlayerStatType.FishCaughtTier4, 10),
            TitleFragment.Threshold("cold_current", TitleSlot.Domain, TitleCategory.Fishing,
                Rarity.Rare, "Cold Current", PlayerStatType.FishCaughtTier2, 12),

            // --- Killed by a tree, graded by what kind of tree ---------------
            // The joke improves with the tier: a sapling is humiliating, an ancient is fate.
            TitleFragment.Threshold("sapling_struck", TitleSlot.Epithet, TitleCategory.Death,
                Rarity.Rare, "Sapling-Struck", PlayerStatType.DeathByTreeTier0, 1),
            TitleFragment.Threshold("hardwood_struck", TitleSlot.Epithet, TitleCategory.Death,
                Rarity.Rare, "Hardwood-Struck", PlayerStatType.DeathByTreeTier2, 1),
            TitleFragment.Threshold("ironwood_struck", TitleSlot.Epithet, TitleCategory.Death,
                Rarity.Epic, "Ironwood-Struck", PlayerStatType.DeathByTreeTier3, 1),
            TitleFragment.Threshold("ancient_struck", TitleSlot.Epithet, TitleCategory.Death,
                Rarity.Legendary, "Ancient-Struck", PlayerStatType.DeathByTreeTier5, 1),
            TitleFragment.Threshold("fallen_timber", TitleSlot.Domain, TitleCategory.Death,
                Rarity.Rare, "Fallen Timber", PlayerStatType.DeathByTree, 2),
        };
    }
}
