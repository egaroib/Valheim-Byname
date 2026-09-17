using System.Collections.Generic;

namespace Byname.Titles.Catalog
{
    /// <summary>
    /// Everything inside the walls: building, farming, foraging, cooking and crafting.
    ///
    /// Valheim breaks placed pieces into twenty <c>BuildCluster*</c> categories, which is
    /// what lets a title tell a roof-and-wall architect apart from someone who has covered
    /// the map in torches. That granularity is the reason a builder does not simply get
    /// "Builder" and stop.
    ///
    /// Careful with those thresholds. Piece.cs:548 writes them as
    /// <c>if (num > stat) SetStat(stat, num)</c> over a freshly counted cluster, so a
    /// BuildCluster value is the LARGEST SINGLE CONNECTED CLUSTER of that tag the player
    /// has ever built — not a lifetime total.
    ///
    /// 0.3.0 raised the wall, floor and roof thresholds again: on a shared server one
    /// ordinary longhouse cleared 120 roof pieces and 150 floors, so every builder was a
    /// Rare "Hallwright" and it crowded out everything more personal. The plain-hall tier
    /// is now Uncommon, and Rare needs a genuinely large build.
    /// </summary>
    internal static class HearthFragments
    {
        internal static IEnumerable<TitleFragment> All => new[]
        {
            // --- Building: volume and shape ----------------------------------
            TitleFragment.Threshold("hall_raising", TitleSlot.Epithet, TitleCategory.Building,
                Rarity.Uncommon, "Hall-Raising|Beam-Lifting|Ever-Building", PlayerStatType.BuiltPieces, 3000),
            TitleFragment.Threshold("stone_setting", TitleSlot.Epithet, TitleCategory.Building,
                Rarity.Uncommon, "Wall-Raising|Stone-Setting|Plank-Nailing", PlayerStatType.BuildClusterWall, 350),
            TitleFragment.Threshold("rampart_raising", TitleSlot.Epithet, TitleCategory.Building,
                Rarity.Rare, "Rampart-Raising|Fortress-Minded", PlayerStatType.BuildClusterWall, 900),
            TitleFragment.Threshold("high_building", TitleSlot.Epithet, TitleCategory.Building,
                Rarity.Epic, "High-Building|Tower-Raising|Sky-Reaching", PlayerStatType.MaxBuildingHeight, 40),
            TitleFragment.Threshold("well_lit", TitleSlot.Epithet, TitleCategory.Building,
                Rarity.Uncommon, "Well-Lit|Torch-Lining|Lamp-Hanging", PlayerStatType.BuildClusterLighting, 60),
            TitleFragment.Threshold("well_housed", TitleSlot.Epithet, TitleCategory.Building,
                Rarity.Rare, "Well-Housed|Comfort-Wise|Cushion-Proud", PlayerStatType.MaxComfort, 17),
            TitleFragment.Threshold("ever_mending", TitleSlot.Epithet, TitleCategory.Building,
                Rarity.Rare, "Ever-Mending|Never-Satisfied|Oft-Rebuilding", PlayerStatType.BuildPiecesRemoved, 2000),
            TitleFragment.Threshold("well_adorned", TitleSlot.Epithet, TitleCategory.Building,
                Rarity.Rare, "Well-Adorned|Rug-Proud|Banner-Hanging", PlayerStatType.BuildClusterDecor, 60),

            TitleFragment.Threshold("housecarl", TitleSlot.Noun, TitleCategory.Building,
                Rarity.Uncommon, "Housecarl|Joiner|Builder", PlayerStatType.BuiltPieces, 1500),
            TitleFragment.Threshold("thatcher", TitleSlot.Noun, TitleCategory.Building,
                Rarity.Uncommon, "Thatcher|Roofer|Shingler", PlayerStatType.BuildClusterRoof, 200),
            TitleFragment.Threshold("hallwright", TitleSlot.Noun, TitleCategory.Building,
                Rarity.Rare, "Hallwright|Roof-Raiser|Rafter-Master", PlayerStatType.BuildClusterRoof, 450),
            TitleFragment.Threshold("stonewright", TitleSlot.Noun, TitleCategory.Building,
                Rarity.Rare, "Stonewright|Mason|Pillar-Setter", PlayerStatType.BuildClusterArchitecture, 200),
            TitleFragment.Threshold("hearthkeeper", TitleSlot.Noun, TitleCategory.Building,
                Rarity.Uncommon, "Hearthkeeper|Homemaker|Bench-Carver", PlayerStatType.BuildClusterFurniture, 60),
            TitleFragment.Threshold("wallwarden", TitleSlot.Noun, TitleCategory.Building,
                Rarity.Rare, "Wallwarden|Palisader|Stake-Setter", PlayerStatType.BuildClusterDefense, 150),
            TitleFragment.Threshold("stairwright", TitleSlot.Noun, TitleCategory.Building,
                Rarity.Epic, "Stairwright|Step-Cutter", PlayerStatType.BuildClusterStairs, 80),
            TitleFragment.Threshold("jarl", TitleSlot.Noun, TitleCategory.Building,
                Rarity.Legendary, "Jarl|Chieftain|Hall-King", PlayerStatType.BuiltPieces, 20000),

            TitleFragment.Threshold("quiet_hearth", TitleSlot.Domain, TitleCategory.Building,
                Rarity.Uncommon, "Quiet Hearth|Warm Hall|Long Bench", PlayerStatType.BuildClusterFurniture, 40),
            TitleFragment.Threshold("high_hall", TitleSlot.Domain, TitleCategory.Building,
                Rarity.Rare, "High Hall|Tall Tower", PlayerStatType.MaxBuildingHeight, 28),
            TitleFragment.Threshold("long_wall", TitleSlot.Domain, TitleCategory.Building,
                Rarity.Rare, "Long Wall|Great Wall|High Ramparts", PlayerStatType.BuildClusterWall, 700),

            // --- Harvest -----------------------------------------------------
            TitleFragment.Threshold("bee_tending", TitleSlot.Epithet, TitleCategory.Harvest,
                Rarity.Uncommon, "Bee-Tending|Honey-Handed|Hive-Minded", PlayerStatType.BeesHarvested, 100),
            TitleFragment.Threshold("sap_drawing", TitleSlot.Epithet, TitleCategory.Harvest,
                Rarity.Rare, "Sap-Drawing|Root-Tapping", PlayerStatType.SapHarvested, 200),
            TitleFragment.Threshold("green_thumbed", TitleSlot.Epithet, TitleCategory.Harvest,
                Rarity.Uncommon, "Green-Thumbed|Soil-Blessed|Seed-Sowing", PlayerStatType.HarvestCrop, 500),
            TitleFragment.Threshold("berry_stained", TitleSlot.Epithet, TitleCategory.Harvest,
                Rarity.Uncommon, "Berry-Stained|Bramble-Scratched", PlayerStatType.HarvestBerry, 500),
            TitleFragment.Threshold("wood_wise", TitleSlot.Epithet, TitleCategory.Harvest,
                Rarity.Rare, "Wood-Wise|Mushroom-Mad|Toadstool-Wise", PlayerStatType.HarvestMushroom, 400),

            TitleFragment.Threshold("beekeeper", TitleSlot.Noun, TitleCategory.Harvest,
                Rarity.Uncommon, "Beekeeper|Hive-Warden|Honey-Taker", PlayerStatType.BeesHarvested, 50),
            TitleFragment.Threshold("tiller", TitleSlot.Noun, TitleCategory.Harvest,
                Rarity.Uncommon, "Tiller|Farmer|Ploughman", PlayerStatType.HarvestCrop, 200),
            TitleFragment.Threshold("forager", TitleSlot.Noun, TitleCategory.Harvest,
                Rarity.Common, "Forager|Berry-Picker", PlayerStatType.HarvestBerry, 150),
            TitleFragment.Threshold("sapwright", TitleSlot.Noun, TitleCategory.Harvest,
                Rarity.Rare, "Sapwright|Sap-Tapper", PlayerStatType.SapHarvested, 100),
            TitleFragment.Threshold("limbhewer", TitleSlot.Noun, TitleCategory.Harvest,
                Rarity.Uncommon, "Limbhewer|Axeman|Lumberjack", PlayerStatType.TreeChops, 5000),
            TitleFragment.Threshold("stonebreaker", TitleSlot.Noun, TitleCategory.Harvest,
                Rarity.Uncommon, "Stonebreaker|Rockhewer|Quarryman", PlayerStatType.Mines, 1500),
            TitleFragment.Threshold("oakfeller", TitleSlot.Noun, TitleCategory.Harvest,
                Rarity.Epic, "Oakfeller|Oak-Toppler", PlayerStatType.TreeOak, 100),

            TitleFragment.Threshold("green_hall", TitleSlot.Domain, TitleCategory.Harvest,
                Rarity.Rare, "Green Hall|Tilled Field|Turnip Rows", PlayerStatType.HarvestCrop, 400),
            TitleFragment.Threshold("deep_wood", TitleSlot.Domain, TitleCategory.Harvest,
                Rarity.Uncommon, "Deep Wood|Tall Timber|Stump Field", PlayerStatType.Tree, 800),
            TitleFragment.Threshold("black_vein", TitleSlot.Domain, TitleCategory.Harvest,
                Rarity.Rare, "Black Vein|Deep Seam", PlayerStatType.Mines, 2500),

            // --- Cooking -----------------------------------------------------
            TitleFragment.Threshold("well_fed", TitleSlot.Epithet, TitleCategory.Cooking,
                Rarity.Uncommon, "Well-Fed|Stout-Bellied|Hearty", PlayerStatType.FoodEaten, 800),
            TitleFragment.Threshold("ale_warmed", TitleSlot.Epithet, TitleCategory.Cooking,
                Rarity.Rare, "Ale-Warmed|Mead-Soaked", PlayerStatType.BuildClusterMeads, 20),
            TitleFragment.Threshold("smoke_touched", TitleSlot.Epithet, TitleCategory.Cooking,
                Rarity.Uncommon, "Smoke-Touched|Spit-Turning|Grease-Fingered", PlayerStatType.CraftGrill, 300),

            // Valheim counts the ones you burned. Of course it does.
            TitleFragment.Threshold("ash_tongued", TitleSlot.Epithet, TitleCategory.Cooking,
                Rarity.Rare, "Ash-Tongued|Char-Loving|Burnt-Offering", PlayerStatType.CraftGrillBurnt, 100),

            TitleFragment.Threshold("cook", TitleSlot.Noun, TitleCategory.Cooking,
                Rarity.Common, "Cook|Pot-Stirrer|Broth-Maker", PlayerStatType.CraftFood, 200),
            TitleFragment.Threshold("firetender", TitleSlot.Noun, TitleCategory.Cooking,
                Rarity.Uncommon, "Firetender|Grillmaster|Spit-Turner", PlayerStatType.CraftGrill, 150),
            TitleFragment.Threshold("meadwright", TitleSlot.Noun, TitleCategory.Cooking,
                Rarity.Rare, "Meadwright|Brewer|Mead-Brewer", PlayerStatType.BuildClusterMeads, 10),
            TitleFragment.Threshold("feastgiver", TitleSlot.Noun, TitleCategory.Cooking,
                Rarity.Epic, "Feastgiver|Feast-Host", PlayerStatType.BuildClusterFeasts, 8),

            TitleFragment.Threshold("long_table", TitleSlot.Domain, TitleCategory.Cooking,
                Rarity.Rare, "Long Table|Mead Bench", PlayerStatType.BuildClusterFeasts, 5),
            TitleFragment.Threshold("full_board", TitleSlot.Domain, TitleCategory.Cooking,
                Rarity.Uncommon, "Full Board|Groaning Board|Stewpot", PlayerStatType.FoodEaten, 500),

            // --- Crafting ----------------------------------------------------
            TitleFragment.Threshold("iron_handed", TitleSlot.Epithet, TitleCategory.Crafting,
                Rarity.Uncommon, "Iron-Handed|Anvil-Ringing", PlayerStatType.CraftWeapon, 100),
            TitleFragment.Threshold("ever_bettering", TitleSlot.Epithet, TitleCategory.Crafting,
                Rarity.Rare, "Ever-Bettering|Whetstone-Keen", PlayerStatType.Upgrades, 300),
            TitleFragment.Threshold("well_girded", TitleSlot.Epithet, TitleCategory.Crafting,
                Rarity.Uncommon, "Well-Girded|Mail-Stitching", PlayerStatType.CraftArmor, 60),
            TitleFragment.Threshold("torch_bearing", TitleSlot.Epithet, TitleCategory.Crafting,
                Rarity.Rare, "Torch-Bearing|Pitch-Stained", PlayerStatType.CraftTorch, 200),

            TitleFragment.Threshold("smith", TitleSlot.Noun, TitleCategory.Crafting,
                Rarity.Common, "Smith|Hammerer|Forgehand", PlayerStatType.Crafts, 300),
            TitleFragment.Threshold("weaponwright", TitleSlot.Noun, TitleCategory.Crafting,
                Rarity.Rare, "Weaponwright|Bladesmith|Edge-Maker", PlayerStatType.CraftWeapon, 200),
            TitleFragment.Threshold("fletcher", TitleSlot.Noun, TitleCategory.Crafting,
                Rarity.Uncommon, "Fletcher|Arrowsmith", PlayerStatType.CraftAmmo, 300),
            TitleFragment.Threshold("charmwright", TitleSlot.Noun, TitleCategory.Crafting,
                Rarity.Epic, "Charmwright|Trinket-Maker", PlayerStatType.CraftTrinket, 25),
            TitleFragment.Threshold("toolwright", TitleSlot.Noun, TitleCategory.Crafting,
                Rarity.Uncommon, "Toolwright|Tool-Maker", PlayerStatType.CraftTool, 40),
            TitleFragment.Threshold("refiner", TitleSlot.Noun, TitleCategory.Crafting,
                Rarity.Uncommon, "Refiner|Alloy-Maker", PlayerStatType.CraftMaterial, 300),
            TitleFragment.Threshold("masterwright", TitleSlot.Noun, TitleCategory.Crafting,
                Rarity.Legendary, "Masterwright|Master-Smith", PlayerStatType.CraftsOrUpgrades, 2500),

            TitleFragment.Threshold("forge", TitleSlot.Domain, TitleCategory.Crafting,
                Rarity.Uncommon, "Forge|Anvil|Smithy", PlayerStatType.Crafts, 500),
            TitleFragment.Threshold("keen_edge", TitleSlot.Domain, TitleCategory.Crafting,
                Rarity.Rare, "Keen Edge|Bright Blade", PlayerStatType.CraftWeapon, 150),
        };
    }
}
