using System.Collections.Generic;

namespace Byname.Titles.Catalog
{
    /// <summary>
    /// Everything outside the walls: travel, exploration, the sea, tames, and fish.
    ///
    /// Distances are in metres, so the thresholds look large. They are the numbers most
    /// likely to need tuning after real play — turn on VerboseLogging and the near-miss
    /// lines will say whether they are set sensibly.
    /// </summary>
    internal static class WildFragments
    {
        internal static IEnumerable<TitleFragment> All => new[]
        {
            // --- Travel ------------------------------------------------------
            TitleFragment.Threshold("far_walking", TitleSlot.Epithet, TitleCategory.Travel,
                Rarity.Uncommon, "Far-Walking", PlayerStatType.DistanceWalk, 100000),
            TitleFragment.Threshold("swift_footed", TitleSlot.Epithet, TitleCategory.Travel,
                Rarity.Uncommon, "Swift-Footed", PlayerStatType.DistanceRun, 75000),
            TitleFragment.Threshold("salt_worn", TitleSlot.Epithet, TitleCategory.Travel,
                Rarity.Rare, "Salt-Worn", PlayerStatType.DistanceSail, 50000),
            TitleFragment.Threshold("wind_borne", TitleSlot.Epithet, TitleCategory.Travel,
                Rarity.Epic, "Wind-Borne", PlayerStatType.DistanceAir, 5000),
            TitleFragment.Threshold("road_weary", TitleSlot.Epithet, TitleCategory.Travel,
                Rarity.Rare, "Road-Weary", PlayerStatType.DistanceTraveled, 500000),

            TitleFragment.Threshold("wanderer", TitleSlot.Noun, TitleCategory.Travel,
                Rarity.Common, "Wanderer", PlayerStatType.DistanceTraveled, 50000),
            TitleFragment.Threshold("wayfarer", TitleSlot.Noun, TitleCategory.Travel,
                Rarity.Uncommon, "Wayfarer", PlayerStatType.DistanceTraveled, 200000),
            TitleFragment.Threshold("helmsman", TitleSlot.Noun, TitleCategory.Travel,
                Rarity.Rare, "Helmsman", PlayerStatType.DistanceSailHelm, 30000),
            TitleFragment.Threshold("ferryman", TitleSlot.Noun, TitleCategory.Travel,
                Rarity.Uncommon, "Ferryman", PlayerStatType.DistanceSail, 20000),

            TitleFragment.Threshold("long_road", TitleSlot.Domain, TitleCategory.Travel,
                Rarity.Uncommon, "Long Road", PlayerStatType.DistanceRun, 50000),
            TitleFragment.Threshold("nine_waves", TitleSlot.Domain, TitleCategory.Travel,
                Rarity.Rare, "Nine Waves", PlayerStatType.DistanceSail, 40000),
            TitleFragment.Threshold("open_sky", TitleSlot.Domain, TitleCategory.Travel,
                Rarity.Epic, "Open Sky", PlayerStatType.DistanceAir, 3000),

            // --- Exploration. Valheim tracks how far out you push per compass
            // --- direction, which is a genuinely unusual axis to be titled on.
            TitleFragment.Threshold("north_going", TitleSlot.Epithet, TitleCategory.Exploration,
                Rarity.Rare, "North-Going", PlayerStatType.ExploreNorth, 8000),
            TitleFragment.Threshold("south_going", TitleSlot.Epithet, TitleCategory.Exploration,
                Rarity.Rare, "South-Going", PlayerStatType.ExploreSouth, 8000),
            TitleFragment.Threshold("east_going", TitleSlot.Epithet, TitleCategory.Exploration,
                Rarity.Rare, "East-Going", PlayerStatType.ExploreEast, 8000),
            TitleFragment.Threshold("west_going", TitleSlot.Epithet, TitleCategory.Exploration,
                Rarity.Rare, "West-Going", PlayerStatType.ExploreWest, 8000),

            // Pushing out with no map at all is a deliberate, difficult way to play.
            TitleFragment.Threshold("chartless", TitleSlot.Epithet, TitleCategory.Exploration,
                Rarity.Legendary, "Chartless", PlayerStatType.ExploreNorthNoMap, 5000),

            TitleFragment.Threshold("deep_delving", TitleSlot.Epithet, TitleCategory.Exploration,
                Rarity.Rare, "Deep-Delving", PlayerStatType.PortalDungeonIn, 50),

            TitleFragment.Threshold("pathfinder", TitleSlot.Noun, TitleCategory.Exploration,
                Rarity.Uncommon, "Pathfinder", PlayerStatType.TreasureLocationFound, 15),
            TitleFragment.Threshold("delver", TitleSlot.Noun, TitleCategory.Exploration,
                Rarity.Rare, "Delver", PlayerStatType.TreasureDungeonFound, 20),
            TitleFragment.Threshold("cairnbreaker", TitleSlot.Noun, TitleCategory.Exploration,
                Rarity.Rare, "Cairnbreaker", PlayerStatType.TreasureBuriedFound, 25),
            TitleFragment.Threshold("leviathan_waker", TitleSlot.Noun, TitleCategory.Exploration,
                Rarity.Epic, "Leviathan-Waker", PlayerStatType.LeviathanSink, 3),

            TitleFragment.Threshold("hollow_hills", TitleSlot.Domain, TitleCategory.Exploration,
                Rarity.Rare, "Hollow Hills", PlayerStatType.PortalDungeonIn, 30),
            TitleFragment.Threshold("far_shore", TitleSlot.Domain, TitleCategory.Exploration,
                Rarity.Rare, "Far Shore", PlayerStatType.ExploreWest, 6000),

            // --- Taming ------------------------------------------------------
            TitleFragment.Threshold("kindly", TitleSlot.Epithet, TitleCategory.Taming,
                Rarity.Uncommon, "Kindly", PlayerStatType.TamedPetting, 100),
            TitleFragment.Threshold("beast_gentling", TitleSlot.Epithet, TitleCategory.Taming,
                Rarity.Rare, "Beast-Gentling", PlayerStatType.CreatureTamed, 25),

            TitleFragment.Threshold("tamer", TitleSlot.Noun, TitleCategory.Taming,
                Rarity.Uncommon, "Tamer", PlayerStatType.CreatureTamed, 10),
            TitleFragment.Threshold("beastfriend", TitleSlot.Noun, TitleCategory.Taming,
                Rarity.Rare, "Beastfriend", PlayerStatType.TamedPetting, 400),
            TitleFragment.Threshold("herdmaster", TitleSlot.Noun, TitleCategory.Taming,
                Rarity.Epic, "Herdmaster", PlayerStatType.CreatureTamed, 60),

            TitleFragment.Threshold("quiet_herd", TitleSlot.Domain, TitleCategory.Taming,
                Rarity.Uncommon, "Quiet Herd", PlayerStatType.CreatureTamed, 15),

            // --- Fishing -----------------------------------------------------
            TitleFragment.Threshold("line_wise", TitleSlot.Epithet, TitleCategory.Fishing,
                Rarity.Uncommon, "Line-Wise", PlayerStatType.FishCaught, 50),
            TitleFragment.Threshold("patient", TitleSlot.Epithet, TitleCategory.Fishing,
                Rarity.Rare, "Patient", PlayerStatType.FishHooked, 300),

            // Everyone has a story about the one that got away. Valheim counts them.
            TitleFragment.Threshold("oft_cheated", TitleSlot.Epithet, TitleCategory.Fishing,
                Rarity.Rare, "Oft-Cheated", PlayerStatType.FishLost, 100),

            TitleFragment.Threshold("angler", TitleSlot.Noun, TitleCategory.Fishing,
                Rarity.Uncommon, "Angler", PlayerStatType.FishCaught, 25),
            TitleFragment.Threshold("fishwright", TitleSlot.Noun, TitleCategory.Fishing,
                Rarity.Rare, "Fishwright", PlayerStatType.FishCaught, 200),
            TitleFragment.Threshold("netmaster", TitleSlot.Noun, TitleCategory.Fishing,
                Rarity.Epic, "Netmaster", PlayerStatType.FishCaught, 500),

            TitleFragment.Threshold("still_water", TitleSlot.Domain, TitleCategory.Fishing,
                Rarity.Uncommon, "Still Water", PlayerStatType.FishCaught, 60),
        };
    }
}
