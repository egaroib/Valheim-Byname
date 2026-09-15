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
    /// has ever built — not a lifetime total. 120 roof pieces means one enormous hall,
    /// not 120 roof pieces laid over a year.
    /// </summary>
    internal static class HearthFragments
    {
        internal static IEnumerable<TitleFragment> All => new[]
        {
            // --- Building: volume and shape ----------------------------------
            TitleFragment.Threshold("hall_raising", TitleSlot.Epithet, TitleCategory.Building,
                Rarity.Uncommon, "Hall-Raising", PlayerStatType.BuiltPieces, 1000),
            TitleFragment.Threshold("stone_setting", TitleSlot.Epithet, TitleCategory.Building,
                Rarity.Rare, "Stone-Setting", PlayerStatType.BuildClusterWall, 150),
            TitleFragment.Threshold("high_building", TitleSlot.Epithet, TitleCategory.Building,
                Rarity.Epic, "High-Building", PlayerStatType.MaxBuildingHeight, 30),
            TitleFragment.Threshold("well_lit", TitleSlot.Epithet, TitleCategory.Building,
                Rarity.Uncommon, "Well-Lit", PlayerStatType.BuildClusterLighting, 60),
            TitleFragment.Threshold("well_housed", TitleSlot.Epithet, TitleCategory.Building,
                Rarity.Rare, "Well-Housed", PlayerStatType.MaxComfort, 17),
            TitleFragment.Threshold("ever_mending", TitleSlot.Epithet, TitleCategory.Building,
                Rarity.Rare, "Ever-Mending", PlayerStatType.BuildPiecesRemoved, 2000),

            TitleFragment.Threshold("housecarl", TitleSlot.Noun, TitleCategory.Building,
                Rarity.Uncommon, "Housecarl", PlayerStatType.BuiltPieces, 500),
            TitleFragment.Threshold("hallwright", TitleSlot.Noun, TitleCategory.Building,
                Rarity.Rare, "Hallwright", PlayerStatType.BuildClusterRoof, 120),
            TitleFragment.Threshold("stonewright", TitleSlot.Noun, TitleCategory.Building,
                Rarity.Rare, "Stonewright", PlayerStatType.BuildClusterArchitecture, 80),
            TitleFragment.Threshold("hearthkeeper", TitleSlot.Noun, TitleCategory.Building,
                Rarity.Uncommon, "Hearthkeeper", PlayerStatType.BuildClusterFurniture, 40),
            TitleFragment.Threshold("wallwarden", TitleSlot.Noun, TitleCategory.Building,
                Rarity.Rare, "Wallwarden", PlayerStatType.BuildClusterDefense, 80),
            TitleFragment.Threshold("stairwright", TitleSlot.Noun, TitleCategory.Building,
                Rarity.Epic, "Stairwright", PlayerStatType.BuildClusterStairs, 60),
            TitleFragment.Threshold("jarl", TitleSlot.Noun, TitleCategory.Building,
                Rarity.Legendary, "Jarl", PlayerStatType.BuiltPieces, 8000),

            TitleFragment.Threshold("quiet_hearth", TitleSlot.Domain, TitleCategory.Building,
                Rarity.Uncommon, "Quiet Hearth", PlayerStatType.BuildClusterFurniture, 25),
            TitleFragment.Threshold("high_hall", TitleSlot.Domain, TitleCategory.Building,
                Rarity.Rare, "High Hall", PlayerStatType.MaxBuildingHeight, 20),
            TitleFragment.Threshold("long_wall", TitleSlot.Domain, TitleCategory.Building,
                Rarity.Rare, "Long Wall", PlayerStatType.BuildClusterWall, 100),

            // --- Harvest -----------------------------------------------------
            TitleFragment.Threshold("bee_tending", TitleSlot.Epithet, TitleCategory.Harvest,
                Rarity.Uncommon, "Bee-Tending", PlayerStatType.BeesHarvested, 100),
            TitleFragment.Threshold("sap_drawing", TitleSlot.Epithet, TitleCategory.Harvest,
                Rarity.Rare, "Sap-Drawing", PlayerStatType.SapHarvested, 200),
            TitleFragment.Threshold("green_thumbed", TitleSlot.Epithet, TitleCategory.Harvest,
                Rarity.Uncommon, "Green-Thumbed", PlayerStatType.HarvestCrop, 500),
            TitleFragment.Threshold("berry_stained", TitleSlot.Epithet, TitleCategory.Harvest,
                Rarity.Uncommon, "Berry-Stained", PlayerStatType.HarvestBerry, 500),
            TitleFragment.Threshold("wood_wise", TitleSlot.Epithet, TitleCategory.Harvest,
                Rarity.Rare, "Wood-Wise", PlayerStatType.HarvestMushroom, 400),

            TitleFragment.Threshold("beekeeper", TitleSlot.Noun, TitleCategory.Harvest,
                Rarity.Uncommon, "Beekeeper", PlayerStatType.BeesHarvested, 50),
            TitleFragment.Threshold("tiller", TitleSlot.Noun, TitleCategory.Harvest,
                Rarity.Uncommon, "Tiller", PlayerStatType.HarvestCrop, 200),
            TitleFragment.Threshold("forager", TitleSlot.Noun, TitleCategory.Harvest,
                Rarity.Common, "Forager", PlayerStatType.HarvestBerry, 150),
            TitleFragment.Threshold("sapwright", TitleSlot.Noun, TitleCategory.Harvest,
                Rarity.Rare, "Sapwright", PlayerStatType.SapHarvested, 100),
            TitleFragment.Threshold("limbhewer", TitleSlot.Noun, TitleCategory.Harvest,
                Rarity.Uncommon, "Limbhewer", PlayerStatType.TreeChops, 3000),
            TitleFragment.Threshold("stonebreaker", TitleSlot.Noun, TitleCategory.Harvest,
                Rarity.Uncommon, "Stonebreaker", PlayerStatType.Mines, 1000),
            TitleFragment.Threshold("oakfeller", TitleSlot.Noun, TitleCategory.Harvest,
                Rarity.Epic, "Oakfeller", PlayerStatType.TreeOak, 100),

            TitleFragment.Threshold("green_hall", TitleSlot.Domain, TitleCategory.Harvest,
                Rarity.Rare, "Green Hall", PlayerStatType.HarvestCrop, 400),
            TitleFragment.Threshold("deep_wood", TitleSlot.Domain, TitleCategory.Harvest,
                Rarity.Uncommon, "Deep Wood", PlayerStatType.Tree, 500),
            TitleFragment.Threshold("black_vein", TitleSlot.Domain, TitleCategory.Harvest,
                Rarity.Rare, "Black Vein", PlayerStatType.Mines, 2500),

            // --- Cooking -----------------------------------------------------
            TitleFragment.Threshold("well_fed", TitleSlot.Epithet, TitleCategory.Cooking,
                Rarity.Uncommon, "Well-Fed", PlayerStatType.FoodEaten, 800),
            TitleFragment.Threshold("ale_warmed", TitleSlot.Epithet, TitleCategory.Cooking,
                Rarity.Rare, "Ale-Warmed", PlayerStatType.BuildClusterMeads, 20),
            TitleFragment.Threshold("smoke_touched", TitleSlot.Epithet, TitleCategory.Cooking,
                Rarity.Uncommon, "Smoke-Touched", PlayerStatType.CraftGrill, 300),

            // Valheim counts the ones you burned. Of course it does.
            TitleFragment.Threshold("ash_tongued", TitleSlot.Epithet, TitleCategory.Cooking,
                Rarity.Rare, "Ash-Tongued", PlayerStatType.CraftGrillBurnt, 100),

            TitleFragment.Threshold("cook", TitleSlot.Noun, TitleCategory.Cooking,
                Rarity.Common, "Cook", PlayerStatType.CraftFood, 200),
            TitleFragment.Threshold("firetender", TitleSlot.Noun, TitleCategory.Cooking,
                Rarity.Uncommon, "Firetender", PlayerStatType.CraftGrill, 150),
            TitleFragment.Threshold("meadwright", TitleSlot.Noun, TitleCategory.Cooking,
                Rarity.Rare, "Meadwright", PlayerStatType.BuildClusterMeads, 10),
            TitleFragment.Threshold("feastgiver", TitleSlot.Noun, TitleCategory.Cooking,
                Rarity.Epic, "Feastgiver", PlayerStatType.BuildClusterFeasts, 8),

            TitleFragment.Threshold("long_table", TitleSlot.Domain, TitleCategory.Cooking,
                Rarity.Rare, "Long Table", PlayerStatType.BuildClusterFeasts, 5),
            TitleFragment.Threshold("full_board", TitleSlot.Domain, TitleCategory.Cooking,
                Rarity.Uncommon, "Full Board", PlayerStatType.FoodEaten, 500),

            // --- Crafting ----------------------------------------------------
            TitleFragment.Threshold("iron_handed", TitleSlot.Epithet, TitleCategory.Crafting,
                Rarity.Uncommon, "Iron-Handed", PlayerStatType.CraftWeapon, 100),
            TitleFragment.Threshold("ever_bettering", TitleSlot.Epithet, TitleCategory.Crafting,
                Rarity.Rare, "Ever-Bettering", PlayerStatType.Upgrades, 300),
            TitleFragment.Threshold("well_girded", TitleSlot.Epithet, TitleCategory.Crafting,
                Rarity.Uncommon, "Well-Girded", PlayerStatType.CraftArmor, 60),
            TitleFragment.Threshold("torch_bearing", TitleSlot.Epithet, TitleCategory.Crafting,
                Rarity.Rare, "Torch-Bearing", PlayerStatType.CraftTorch, 200),

            TitleFragment.Threshold("smith", TitleSlot.Noun, TitleCategory.Crafting,
                Rarity.Common, "Smith", PlayerStatType.Crafts, 300),
            TitleFragment.Threshold("weaponwright", TitleSlot.Noun, TitleCategory.Crafting,
                Rarity.Rare, "Weaponwright", PlayerStatType.CraftWeapon, 200),
            TitleFragment.Threshold("fletcher", TitleSlot.Noun, TitleCategory.Crafting,
                Rarity.Uncommon, "Fletcher", PlayerStatType.CraftAmmo, 300),
            TitleFragment.Threshold("charmwright", TitleSlot.Noun, TitleCategory.Crafting,
                Rarity.Epic, "Charmwright", PlayerStatType.CraftTrinket, 25),
            TitleFragment.Threshold("masterwright", TitleSlot.Noun, TitleCategory.Crafting,
                Rarity.Legendary, "Masterwright", PlayerStatType.CraftsOrUpgrades, 2500),

            TitleFragment.Threshold("forge", TitleSlot.Domain, TitleCategory.Crafting,
                Rarity.Uncommon, "Forge", PlayerStatType.Crafts, 500),
            TitleFragment.Threshold("keen_edge", TitleSlot.Domain, TitleCategory.Crafting,
                Rarity.Rare, "Keen Edge", PlayerStatType.CraftWeapon, 150),
        };
    }
}
