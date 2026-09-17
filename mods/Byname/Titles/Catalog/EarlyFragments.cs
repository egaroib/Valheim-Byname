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
                Rarity.Common, "New-Landed|Raven-Brought|Fresh-Landed", PlayerStatType.PlayerSpawn, 1),
            TitleFragment.Threshold("hearth_lit", TitleSlot.Epithet, TitleCategory.Building,
                Rarity.Common, "Hearth-Lit|Turf-Roofed|Smoke-Rising", PlayerStatType.BuiltPieces, 50),
            TitleFragment.Threshold("first_footed", TitleSlot.Epithet, TitleCategory.Travel,
                Rarity.Common, "First-Footed|Trail-Dusted|Mud-Booted", PlayerStatType.DistanceWalk, 5000),
            TitleFragment.Threshold("hungry", TitleSlot.Epithet, TitleCategory.Cooking,
                Rarity.Common, "Well-Meaning|Broth-Warmed|Crumb-Bearded", PlayerStatType.FoodEaten, 50),
            TitleFragment.Threshold("hand_worn", TitleSlot.Epithet, TitleCategory.Crafting,
                Rarity.Common, "Hand-Worn|Blister-Palmed|Callus-Handed", PlayerStatType.Crafts, 40),
            TitleFragment.Threshold("rough_handed", TitleSlot.Epithet, TitleCategory.Harvest,
                Rarity.Common, "Rough-Handed|Bark-Scuffed|Sap-Sticky", PlayerStatType.TreeChops, 300),
            TitleFragment.Threshold("wave_shy", TitleSlot.Epithet, TitleCategory.Travel,
                Rarity.Common, "Wave-Shy|Raft-Faring|Shore-Hugging", PlayerStatType.DistanceSail, 2000),
            TitleFragment.Threshold("night_worn", TitleSlot.Epithet, TitleCategory.Misc,
                Rarity.Common, "Night-Worn|Bed-Warmed|Dream-Fed", PlayerStatType.Sleep, 5),
            TitleFragment.Threshold("twice_fallen", TitleSlot.Epithet, TitleCategory.Death,
                Rarity.Common, "Twice-Fallen|Still-Rising|Ill-Starred", PlayerStatType.Deaths, 2),

            TitleFragment.Threshold("settler", TitleSlot.Noun, TitleCategory.Building,
                Rarity.Common, "Settler|Homesteader|Crofter", PlayerStatType.BuiltPieces, 100),
            TitleFragment.Threshold("gatherer", TitleSlot.Noun, TitleCategory.Harvest,
                Rarity.Common, "Gatherer|Scrounger|Gleaner", PlayerStatType.ItemsPickedUp, 500),
            TitleFragment.Threshold("walker", TitleSlot.Noun, TitleCategory.Travel,
                Rarity.Common, "Walker|Rambler|Footslogger", PlayerStatType.DistanceWalk, 15000),
            TitleFragment.Threshold("digger", TitleSlot.Noun, TitleCategory.Harvest,
                Rarity.Common, "Digger|Mudlark|Ditch-Digger", PlayerStatType.MineHits, 500),
            TitleFragment.Threshold("apprentice", TitleSlot.Noun, TitleCategory.Crafting,
                Rarity.Common, "Apprentice|Tinkerer|Journeyman", PlayerStatType.CraftsOrUpgrades, 100),
            TitleFragment.Threshold("thrall", TitleSlot.Noun, TitleCategory.Misc,
                Rarity.Common, "Thrall|Bounder|Hopper", PlayerStatType.Jumps, 300),

            TitleFragment.Threshold("first_camp", TitleSlot.Domain, TitleCategory.Building,
                Rarity.Common, "First Camp|Lean-To|Rough Camp", PlayerStatType.BuiltPieces, 80),
            TitleFragment.Threshold("near_shore", TitleSlot.Domain, TitleCategory.Travel,
                Rarity.Common, "Near Shore|Shallows|Home Waters", PlayerStatType.DistanceSail, 4000),
            TitleFragment.Threshold("open_road", TitleSlot.Domain, TitleCategory.Travel,
                Rarity.Common, "Open Road|Beaten Path|Muddy Track", PlayerStatType.DistanceWalk, 20000),

            // --- Taming, which had six fragments total -----------------------
            TitleFragment.Threshold("beast_calling", TitleSlot.Epithet, TitleCategory.Taming,
                Rarity.Common, "Beast-Calling|Feed-Tossing", PlayerStatType.CreatureTamed, 2),
            TitleFragment.Threshold("well_heeded", TitleSlot.Epithet, TitleCategory.Taming,
                Rarity.Uncommon, "Well-Heeded|Well-Obeyed", PlayerStatType.TamedCommand, 200),
            TitleFragment.Threshold("soft_spoken", TitleSlot.Epithet, TitleCategory.Taming,
                Rarity.Epic, "Soft-Spoken|Beast-Beloved", PlayerStatType.TamedPetting, 800),
            TitleFragment.Threshold("herder", TitleSlot.Noun, TitleCategory.Taming,
                Rarity.Common, "Herder|Swineherd|Drover", PlayerStatType.CreatureTamed, 4),
            TitleFragment.Threshold("houndmaster", TitleSlot.Noun, TitleCategory.Taming,
                Rarity.Rare, "Houndmaster|Packleader", PlayerStatType.TamedCommand, 500),
            TitleFragment.Threshold("warm_stable", TitleSlot.Domain, TitleCategory.Taming,
                Rarity.Rare, "Warm Stable|Full Pen|Warm Byre", PlayerStatType.TamedPetting, 250),

            // --- Fishing, likewise -------------------------------------------
            TitleFragment.Threshold("float_watcher", TitleSlot.Noun, TitleCategory.Fishing,
                Rarity.Common, "Float-Watcher|Bobber-Gazer|Line-Dangler", PlayerStatType.FishHooked, 30),
            TitleFragment.Threshold("baited", TitleSlot.Epithet, TitleCategory.Fishing,
                Rarity.Uncommon, "Bait-Handed|Worm-Fingered", PlayerStatType.CraftBait, 100),
            TitleFragment.Threshold("slack_lined", TitleSlot.Epithet, TitleCategory.Fishing,
                Rarity.Common, "Slack-Lined|Line-Snapped", PlayerStatType.FishLost, 25),

            // --- BuildCluster tags that had nothing at all --------------------
            // Remember these are largest-single-cluster, not lifetime totals.
            TitleFragment.Threshold("hoard_keeping", TitleSlot.Epithet, TitleCategory.Building,
                Rarity.Rare, "Hoard-Keeping|Chest-Proud|Dragon-Hoarding", PlayerStatType.BuildClusterStorage, 80),
            TitleFragment.Threshold("floor_layer", TitleSlot.Noun, TitleCategory.Building,
                Rarity.Uncommon, "Floor-Layer|Plank-Setter|Boardwright", PlayerStatType.BuildClusterFloor, 350),
            TitleFragment.Threshold("floorwright", TitleSlot.Noun, TitleCategory.Building,
                Rarity.Rare, "Floorwright|Hall-Floorer|Deckwright", PlayerStatType.BuildClusterFloor, 800),
            TitleFragment.Threshold("gatewarden", TitleSlot.Noun, TitleCategory.Building,
                Rarity.Uncommon, "Gatewarden|Doorward|Porter", PlayerStatType.BuildClusterDoors, 20),
            TitleFragment.Threshold("cartwright", TitleSlot.Noun, TitleCategory.Building,
                Rarity.Rare, "Cartwright|Wainwright", PlayerStatType.BuildClusterTransport, 15),
            TitleFragment.Threshold("well_stocked", TitleSlot.Epithet, TitleCategory.Building,
                Rarity.Uncommon, "Well-Stocked|Larder-Proud|Full-Pantried", PlayerStatType.BuildClusterFood, 25),
            TitleFragment.Threshold("wreath_hanger", TitleSlot.Epithet, TitleCategory.Building,
                Rarity.Epic, "Wreath-Hanging|Yule-Merry", PlayerStatType.BuildClusterSeasonal, 20),
            TitleFragment.Threshold("workshop", TitleSlot.Domain, TitleCategory.Building,
                Rarity.Uncommon, "Workshop|Workbench|Tool Loft", PlayerStatType.BuildClusterCrafting, 20),

            // --- Home versus away, an axis nothing else covered ---------------
            TitleFragment.Threshold("homebound", TitleSlot.Epithet, TitleCategory.Misc,
                Rarity.Uncommon, "Homebound|Hearth-Bound|Door-Hugging", PlayerStatType.TimeInBase, 200000),
            TitleFragment.Threshold("seldom_home", TitleSlot.Epithet, TitleCategory.Misc,
                Rarity.Rare, "Seldom-Home|Long-Absent|Hearthless", PlayerStatType.TimeOutOfBase, 500000),
        };
    }
}
