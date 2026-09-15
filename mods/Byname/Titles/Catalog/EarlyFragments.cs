using System.Collections.Generic;

namespace Byname.Titles.Catalog
{
    /// <summary>
    /// Low thresholds, low rarity — the first ten hours.
    ///
    /// The catalog skewed badly toward Rare and above, which meant a new character had a
    /// pool of two or three words and every staleness reroll cycled through the same
    /// ones. These exist so early titles vary, and so the rotation has somewhere to go
    /// before a player has felled anything impressive.
    ///
    /// Also fills out Taming, Fishing and the BuildCluster tags that had no fragments at
    /// all, so specialising in them is no longer a dead end.
    /// </summary>
    internal static class EarlyFragments
    {
        internal static IEnumerable<TitleFragment> All => new[]
        {
            // --- Earliest possible deeds -------------------------------------
            TitleFragment.Threshold("new_landed", TitleSlot.Epithet, TitleCategory.Misc,
                Rarity.Common, "New-Landed", PlayerStatType.PlayerSpawn, 1),
            TitleFragment.Threshold("hearth_lit", TitleSlot.Epithet, TitleCategory.Building,
                Rarity.Common, "Hearth-Lit", PlayerStatType.BuiltPieces, 50),
            TitleFragment.Threshold("first_footed", TitleSlot.Epithet, TitleCategory.Travel,
                Rarity.Common, "First-Footed", PlayerStatType.DistanceWalk, 5000),
            TitleFragment.Threshold("hungry", TitleSlot.Epithet, TitleCategory.Cooking,
                Rarity.Common, "Well-Meaning", PlayerStatType.FoodEaten, 50),
            TitleFragment.Threshold("hand_worn", TitleSlot.Epithet, TitleCategory.Crafting,
                Rarity.Common, "Hand-Worn", PlayerStatType.Crafts, 40),
            TitleFragment.Threshold("rough_handed", TitleSlot.Epithet, TitleCategory.Harvest,
                Rarity.Common, "Rough-Handed", PlayerStatType.TreeChops, 300),
            TitleFragment.Threshold("wave_shy", TitleSlot.Epithet, TitleCategory.Travel,
                Rarity.Common, "Wave-Shy", PlayerStatType.DistanceSail, 2000),
            TitleFragment.Threshold("night_worn", TitleSlot.Epithet, TitleCategory.Misc,
                Rarity.Common, "Night-Worn", PlayerStatType.Sleep, 5),
            TitleFragment.Threshold("twice_fallen", TitleSlot.Epithet, TitleCategory.Death,
                Rarity.Common, "Twice-Fallen", PlayerStatType.Deaths, 2),

            TitleFragment.Threshold("settler", TitleSlot.Noun, TitleCategory.Building,
                Rarity.Common, "Settler", PlayerStatType.BuiltPieces, 100),
            TitleFragment.Threshold("gatherer", TitleSlot.Noun, TitleCategory.Harvest,
                Rarity.Common, "Gatherer", PlayerStatType.ItemsPickedUp, 500),
            TitleFragment.Threshold("walker", TitleSlot.Noun, TitleCategory.Travel,
                Rarity.Common, "Walker", PlayerStatType.DistanceWalk, 15000),
            TitleFragment.Threshold("digger", TitleSlot.Noun, TitleCategory.Harvest,
                Rarity.Common, "Digger", PlayerStatType.MineHits, 500),
            TitleFragment.Threshold("apprentice", TitleSlot.Noun, TitleCategory.Crafting,
                Rarity.Common, "Apprentice", PlayerStatType.CraftsOrUpgrades, 100),
            TitleFragment.Threshold("thrall", TitleSlot.Noun, TitleCategory.Misc,
                Rarity.Common, "Thrall", PlayerStatType.Jumps, 300),

            TitleFragment.Threshold("first_camp", TitleSlot.Domain, TitleCategory.Building,
                Rarity.Common, "First Camp", PlayerStatType.BuiltPieces, 80),
            TitleFragment.Threshold("near_shore", TitleSlot.Domain, TitleCategory.Travel,
                Rarity.Common, "Near Shore", PlayerStatType.DistanceSail, 4000),
            TitleFragment.Threshold("open_road", TitleSlot.Domain, TitleCategory.Travel,
                Rarity.Common, "Open Road", PlayerStatType.DistanceWalk, 20000),

            // --- Taming, which had six fragments total -----------------------
            TitleFragment.Threshold("beast_calling", TitleSlot.Epithet, TitleCategory.Taming,
                Rarity.Common, "Beast-Calling", PlayerStatType.CreatureTamed, 2),
            TitleFragment.Threshold("well_heeded", TitleSlot.Epithet, TitleCategory.Taming,
                Rarity.Uncommon, "Well-Heeded", PlayerStatType.TamedCommand, 200),
            TitleFragment.Threshold("soft_spoken", TitleSlot.Epithet, TitleCategory.Taming,
                Rarity.Epic, "Soft-Spoken", PlayerStatType.TamedPetting, 800),
            TitleFragment.Threshold("herder", TitleSlot.Noun, TitleCategory.Taming,
                Rarity.Common, "Herder", PlayerStatType.CreatureTamed, 4),
            TitleFragment.Threshold("houndmaster", TitleSlot.Noun, TitleCategory.Taming,
                Rarity.Rare, "Houndmaster", PlayerStatType.TamedCommand, 500),
            TitleFragment.Threshold("warm_stable", TitleSlot.Domain, TitleCategory.Taming,
                Rarity.Rare, "Warm Stable", PlayerStatType.TamedPetting, 250),

            // --- Fishing, likewise -------------------------------------------
            TitleFragment.Threshold("float_watcher", TitleSlot.Noun, TitleCategory.Fishing,
                Rarity.Common, "Float-Watcher", PlayerStatType.FishHooked, 30),
            TitleFragment.Threshold("baited", TitleSlot.Epithet, TitleCategory.Fishing,
                Rarity.Uncommon, "Bait-Handed", PlayerStatType.CraftBait, 100),
            TitleFragment.Threshold("slack_lined", TitleSlot.Epithet, TitleCategory.Fishing,
                Rarity.Common, "Slack-Lined", PlayerStatType.FishLost, 25),

            // --- BuildCluster tags that had nothing at all --------------------
            // Remember these are largest-single-cluster, not lifetime totals.
            TitleFragment.Threshold("hoard_keeping", TitleSlot.Epithet, TitleCategory.Building,
                Rarity.Rare, "Hoard-Keeping", PlayerStatType.BuildClusterStorage, 40),
            TitleFragment.Threshold("floorwright", TitleSlot.Noun, TitleCategory.Building,
                Rarity.Rare, "Floorwright", PlayerStatType.BuildClusterFloor, 150),
            TitleFragment.Threshold("gatewarden", TitleSlot.Noun, TitleCategory.Building,
                Rarity.Uncommon, "Gatewarden", PlayerStatType.BuildClusterDoors, 20),
            TitleFragment.Threshold("cartwright", TitleSlot.Noun, TitleCategory.Building,
                Rarity.Rare, "Cartwright", PlayerStatType.BuildClusterTransport, 15),
            TitleFragment.Threshold("well_stocked", TitleSlot.Epithet, TitleCategory.Building,
                Rarity.Uncommon, "Well-Stocked", PlayerStatType.BuildClusterFood, 25),
            TitleFragment.Threshold("wreath_hanger", TitleSlot.Epithet, TitleCategory.Building,
                Rarity.Epic, "Wreath-Hanging", PlayerStatType.BuildClusterSeasonal, 20),
            TitleFragment.Threshold("workshop", TitleSlot.Domain, TitleCategory.Building,
                Rarity.Uncommon, "Workshop", PlayerStatType.BuildClusterCrafting, 20),

            // --- Home versus away, an axis nothing else covered ---------------
            TitleFragment.Threshold("homebound", TitleSlot.Epithet, TitleCategory.Misc,
                Rarity.Uncommon, "Homebound", PlayerStatType.TimeInBase, 200000),
            TitleFragment.Threshold("seldom_home", TitleSlot.Epithet, TitleCategory.Misc,
                Rarity.Rare, "Seldom-Home", PlayerStatType.TimeOutOfBase, 500000),
        };
    }
}
