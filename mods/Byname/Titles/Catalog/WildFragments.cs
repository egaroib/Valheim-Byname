using System.Collections.Generic;

namespace Byname.Titles.Catalog
{
    /// <summary>
    /// Everything outside the walls: travel, exploration, the sea, tames, and fish.
    ///
    /// Distances are in metres, so the thresholds look large. They are the numbers most
    /// likely to need tuning after real play — turn on VerboseLogging and the near-miss
    /// lines will say whether they are set sensibly, or scale the whole Travel category
    /// from the Thresholds config section.
    ///
    /// <c>DistanceAir</c> is not flight. Player.UpdateStats adds to it for every 2.5s
    /// sample where the player is neither on the ground nor aboard a ship — so jumping,
    /// falling, swimming and riding all count. 0.2.0 treated 3km of it as Epic, and every
    /// player on a shared server became "of the Open Sky" without trying.
    /// </summary>
    internal static class WildFragments
    {
        internal static IEnumerable<TitleFragment> All => new[]
        {
            // --- Travel ------------------------------------------------------
            TitleFragment.Threshold("far_walking", TitleSlot.Epithet, TitleCategory.Travel,
                Rarity.Uncommon, "Far-Walking|Long-Striding|Sore-Footed", PlayerStatType.DistanceWalk, 150000),
            TitleFragment.Threshold("swift_footed", TitleSlot.Epithet, TitleCategory.Travel,
                Rarity.Uncommon, "Swift-Footed|Fleet-Footed|Hare-Quick", PlayerStatType.DistanceRun, 120000),
            TitleFragment.Threshold("salt_worn", TitleSlot.Epithet, TitleCategory.Travel,
                Rarity.Rare, "Salt-Worn|Sea-Weathered|Brine-Crusted", PlayerStatType.DistanceSail, 120000),
            TitleFragment.Threshold("wind_borne", TitleSlot.Epithet, TitleCategory.Travel,
                Rarity.Rare, "Wind-Borne|Seldom-Grounded", PlayerStatType.DistanceAir, 60000),
            TitleFragment.Threshold("road_weary", TitleSlot.Epithet, TitleCategory.Travel,
                Rarity.Rare, "Road-Weary|World-Worn|Mile-Eating", PlayerStatType.DistanceTraveled, 800000),

            TitleFragment.Threshold("wanderer", TitleSlot.Noun, TitleCategory.Travel,
                Rarity.Common, "Wanderer|Roamer|Drifter", PlayerStatType.DistanceTraveled, 50000),
            TitleFragment.Threshold("wayfarer", TitleSlot.Noun, TitleCategory.Travel,
                Rarity.Uncommon, "Wayfarer|Journeyer|Strider", PlayerStatType.DistanceTraveled, 350000),
            TitleFragment.Threshold("helmsman", TitleSlot.Noun, TitleCategory.Travel,
                Rarity.Rare, "Helmsman|Steersman|Skipper", PlayerStatType.DistanceSailHelm, 80000),
            TitleFragment.Threshold("ferryman", TitleSlot.Noun, TitleCategory.Travel,
                Rarity.Uncommon, "Ferryman|Oarsman|Boatman", PlayerStatType.DistanceSail, 40000),

            TitleFragment.Threshold("long_road", TitleSlot.Domain, TitleCategory.Travel,
                Rarity.Uncommon, "Long Road|Winding Path", PlayerStatType.DistanceRun, 80000),
            TitleFragment.Threshold("nine_waves", TitleSlot.Domain, TitleCategory.Travel,
                Rarity.Rare, "Nine Waves|Whale Road|Swan Road", PlayerStatType.DistanceSail, 100000),
            TitleFragment.Threshold("open_sky", TitleSlot.Domain, TitleCategory.Travel,
                Rarity.Uncommon, "Open Sky|Empty Air|High Leap", PlayerStatType.DistanceAir, 25000),

            // --- Exploration. Valheim tracks how long you spend out past the edge of
            // --- the map per compass direction, which is a genuinely unusual axis.
            TitleFragment.Threshold("north_going", TitleSlot.Epithet, TitleCategory.Exploration,
                Rarity.Rare, "North-Going|North-Bound|Pole-Seeking", PlayerStatType.ExploreNorth, 8000),
            TitleFragment.Threshold("south_going", TitleSlot.Epithet, TitleCategory.Exploration,
                Rarity.Rare, "South-Going|South-Bound", PlayerStatType.ExploreSouth, 8000),
            TitleFragment.Threshold("east_going", TitleSlot.Epithet, TitleCategory.Exploration,
                Rarity.Rare, "East-Going|Dawn-Chasing", PlayerStatType.ExploreEast, 8000),
            TitleFragment.Threshold("west_going", TitleSlot.Epithet, TitleCategory.Exploration,
                Rarity.Rare, "West-Going|Sunset-Chasing", PlayerStatType.ExploreWest, 8000),

            // Pushing out with no map at all is a deliberate, difficult way to play. Any
            // direction counts; the furthest one decides.
            new TitleFragment("chartless", TitleSlot.Epithet, TitleCategory.Exploration,
                Rarity.Legendary, "Chartless|Map-Scorning",
                s => MaxNoMap(s) >= TitleFragment.Scaled(TitleCategory.Exploration, 5000),
                new[]
                {
                    PlayerStatType.ExploreNorthNoMap, PlayerStatType.ExploreSouthNoMap,
                    PlayerStatType.ExploreEastNoMap, PlayerStatType.ExploreWestNoMap,
                },
                s => MaxNoMap(s) / TitleFragment.Scaled(TitleCategory.Exploration, 5000),
                describe: s => $"time at the world's edge with no map: {TitleFragment.Number(MaxNoMap(s))} " +
                               $"(needed {TitleFragment.Number(TitleFragment.Scaled(TitleCategory.Exploration, 5000))})"),

            TitleFragment.Threshold("deep_delving", TitleSlot.Epithet, TitleCategory.Exploration,
                Rarity.Rare, "Deep-Delving|Crypt-Crawling|Tomb-Haunting", PlayerStatType.PortalDungeonIn, 50),

            TitleFragment.Threshold("pathfinder", TitleSlot.Noun, TitleCategory.Exploration,
                Rarity.Uncommon, "Pathfinder|Trailblazer|Scout", PlayerStatType.TreasureLocationFound, 15),
            TitleFragment.Threshold("delver", TitleSlot.Noun, TitleCategory.Exploration,
                Rarity.Rare, "Delver|Tomb-Robber|Crypt-Looter", PlayerStatType.TreasureDungeonFound, 20),
            TitleFragment.Threshold("cairnbreaker", TitleSlot.Noun, TitleCategory.Exploration,
                Rarity.Rare, "Cairnbreaker|Barrow-Digger|Hoard-Finder", PlayerStatType.TreasureBuriedFound, 25),
            TitleFragment.Threshold("leviathan_waker", TitleSlot.Noun, TitleCategory.Exploration,
                Rarity.Epic, "Leviathan-Waker|Leviathan-Rider", PlayerStatType.LeviathanSink, 3),

            TitleFragment.Threshold("hollow_hills", TitleSlot.Domain, TitleCategory.Exploration,
                Rarity.Rare, "Hollow Hills|Dark Crypts|Burial Mounds", PlayerStatType.PortalDungeonIn, 30),
            TitleFragment.Threshold("far_shore", TitleSlot.Domain, TitleCategory.Exploration,
                Rarity.Rare, "Far Shore|World's Rim", PlayerStatType.ExploreWest, 6000),

            // --- Taming ------------------------------------------------------
            TitleFragment.Threshold("kindly", TitleSlot.Epithet, TitleCategory.Taming,
                Rarity.Uncommon, "Kindly|Gentle-Handed|Ear-Scratching", PlayerStatType.TamedPetting, 100),
            TitleFragment.Threshold("beast_gentling", TitleSlot.Epithet, TitleCategory.Taming,
                Rarity.Rare, "Beast-Gentling|Wild-Taming", PlayerStatType.CreatureTamed, 25),

            TitleFragment.Threshold("tamer", TitleSlot.Noun, TitleCategory.Taming,
                Rarity.Uncommon, "Tamer|Wrangler|Beast-Breaker", PlayerStatType.CreatureTamed, 10),
            TitleFragment.Threshold("beastfriend", TitleSlot.Noun, TitleCategory.Taming,
                Rarity.Rare, "Beastfriend|Wolf-Friend|Lox-Friend", PlayerStatType.TamedPetting, 400),
            TitleFragment.Threshold("herdmaster", TitleSlot.Noun, TitleCategory.Taming,
                Rarity.Epic, "Herdmaster|Stockmaster", PlayerStatType.CreatureTamed, 60),

            TitleFragment.Threshold("quiet_herd", TitleSlot.Domain, TitleCategory.Taming,
                Rarity.Uncommon, "Quiet Herd|Wolf Pack", PlayerStatType.CreatureTamed, 15),

            // --- Fishing -----------------------------------------------------
            TitleFragment.Threshold("line_wise", TitleSlot.Epithet, TitleCategory.Fishing,
                Rarity.Uncommon, "Line-Wise|Hook-Clever", PlayerStatType.FishCaught, 50),
            TitleFragment.Threshold("patient", TitleSlot.Epithet, TitleCategory.Fishing,
                Rarity.Rare, "Patient|Still-Waiting|Tide-Watching", PlayerStatType.FishHooked, 300),

            // Everyone has a story about the one that got away. Valheim counts them.
            TitleFragment.Threshold("oft_cheated", TitleSlot.Epithet, TitleCategory.Fishing,
                Rarity.Rare, "Oft-Cheated|Fish-Mocked", PlayerStatType.FishLost, 100),

            TitleFragment.Threshold("angler", TitleSlot.Noun, TitleCategory.Fishing,
                Rarity.Uncommon, "Angler|Fisher|Rod-Holder", PlayerStatType.FishCaught, 25),
            TitleFragment.Threshold("fishwright", TitleSlot.Noun, TitleCategory.Fishing,
                Rarity.Rare, "Fishwright|Fishmonger", PlayerStatType.FishCaught, 200),
            TitleFragment.Threshold("netmaster", TitleSlot.Noun, TitleCategory.Fishing,
                Rarity.Epic, "Netmaster|Sea-Harvester", PlayerStatType.FishCaught, 500),

            TitleFragment.Threshold("still_water", TitleSlot.Domain, TitleCategory.Fishing,
                Rarity.Uncommon, "Still Water|Quiet Pool|Deep Pool", PlayerStatType.FishCaught, 60),
        };

        private static float MaxNoMap(Stats.IStatSource s) =>
            System.Math.Max(
                System.Math.Max(s.Get(PlayerStatType.ExploreNorthNoMap), s.Get(PlayerStatType.ExploreSouthNoMap)),
                System.Math.Max(s.Get(PlayerStatType.ExploreEastNoMap), s.Get(PlayerStatType.ExploreWestNoMap)));
    }
}
