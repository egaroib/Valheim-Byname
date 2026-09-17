using System.Collections.Generic;

namespace Byname.Titles.Catalog
{
    /// <summary>
    /// What killed you, and where.
    ///
    /// Valheim itself only knows "killed by an enemy" (<c>DeathByEnemyHit</c>), which is
    /// retroactive and covers the first block below. Everything after it reads Byname's
    /// own DeathLedger, written on each death from the killing creature and the biome —
    /// so those only start counting the day the mod is installed.
    ///
    /// Thresholds are low on purpose. Dying to one thing over and over is distinctive in
    /// a way that building one more wall is not; a handful of swamp deaths is a story.
    ///
    /// Creature tokens are from the game's localisation table, the same source
    /// BeastFragments uses. Several creatures share a fragment where players would not
    /// tell them apart in the moment: an Oozer is still a blob when it kills you.
    /// </summary>
    internal static class DoomFragments
    {
        internal static IEnumerable<TitleFragment> All => new[]
        {
            // --- Killed by monsters, any monster. Valheim's own count. --------
            TitleFragment.Threshold("tooth_marked", TitleSlot.Epithet, TitleCategory.Death,
                Rarity.Uncommon, "Tooth-Marked|Claw-Raked|Oft-Mauled", PlayerStatType.DeathByEnemyHit, 5),
            TitleFragment.Threshold("much_mauled", TitleSlot.Epithet, TitleCategory.Death,
                Rarity.Rare, "Much-Mauled|Beast-Bitten|Monster-Fed", PlayerStatType.DeathByEnemyHit, 25),
            TitleFragment.Threshold("fodder", TitleSlot.Noun, TitleCategory.Death,
                Rarity.Uncommon, "Fodder|Easy-Meat|Carrion", PlayerStatType.DeathByEnemyHit, 10),
            TitleFragment.Threshold("last_stand", TitleSlot.Domain, TitleCategory.Death,
                Rarity.Uncommon, "Last Stand|Losing Fight|Red Snow", PlayerStatType.DeathByEnemyHit, 8),

            // --- Where you keep dying ----------------------------------------
            // Any cause counts: drowning in the swamp is still the swamp's doing.
            TitleFragment.DiedIn("meadow_felled", TitleSlot.Epithet,
                Rarity.Uncommon, "Meadow-Felled|Grass-Stained", "Meadows", 4),
            TitleFragment.DiedIn("forest_taken", TitleSlot.Epithet,
                Rarity.Uncommon, "Forest-Taken|Pine-Shadowed|Moss-Covered", "BlackForest", 3),
            TitleFragment.DiedIn("dark_wood", TitleSlot.Domain,
                Rarity.Rare, "Dark Wood|Black Pines", "BlackForest", 6),

            TitleFragment.DiedIn("mire_taken", TitleSlot.Epithet,
                Rarity.Uncommon, "Mire-Taken|Bog-Claimed|Swamp-Sunk", "Swamp", 3),
            TitleFragment.DiedIn("bog_body", TitleSlot.Noun,
                Rarity.Rare, "Bog-Body|Mire-Wight|Swamp-Thing", "Swamp", 6),
            TitleFragment.DiedIn("the_mire", TitleSlot.Domain,
                Rarity.Uncommon, "Mire|Bog|Rotting Fen", "Swamp", 4),

            TitleFragment.DiedIn("peak_fallen", TitleSlot.Epithet,
                Rarity.Uncommon, "Peak-Fallen|Snow-Buried", "Mountain", 3),
            TitleFragment.DiedIn("high_snows", TitleSlot.Domain,
                Rarity.Rare, "High Snows|Cold Peaks", "Mountain", 6),

            TitleFragment.DiedIn("dust_taken", TitleSlot.Epithet,
                Rarity.Uncommon, "Dust-Taken|Field-Fallen", "Plains", 3),
            TitleFragment.DiedIn("yellow_grass", TitleSlot.Domain,
                Rarity.Rare, "Yellow Grass|Golden Fields", "Plains", 6),

            TitleFragment.DiedIn("sea_taken", TitleSlot.Epithet,
                Rarity.Uncommon, "Sea-Taken|Salt-Buried", "Ocean", 3),
            TitleFragment.DiedIn("whale_road", TitleSlot.Domain,
                Rarity.Rare, "Salt Grave|Cold Sea", "Ocean", 6),

            TitleFragment.DiedIn("mist_lost", TitleSlot.Epithet,
                Rarity.Uncommon, "Mist-Lost|Fog-Swallowed", "Mistlands", 3),
            TitleFragment.DiedIn("grey_mist", TitleSlot.Domain,
                Rarity.Rare, "Grey Mist|Hollow Mist", "Mistlands", 6),

            TitleFragment.DiedIn("ash_claimed", TitleSlot.Epithet,
                Rarity.Rare, "Ash-Claimed|Cinder-Taken", "AshLands", 3),
            TitleFragment.DiedIn("ash_wastes", TitleSlot.Domain,
                Rarity.Epic, "Ash Wastes|Burning Shore", "AshLands", 6),

            TitleFragment.DiedIn("ice_claimed", TitleSlot.Epithet,
                Rarity.Rare, "Ice-Claimed|Frost-Graved", "DeepNorth", 3),

            // --- What keeps killing you --------------------------------------
            // Meadows and Black Forest. Dying to a boar is its own kind of fame.
            TitleFragment.KilledBy("boar_gored", TitleSlot.Epithet,
                Rarity.Epic, "Boar-Gored|Pig-Bested", 1, "$enemy_boar"),
            TitleFragment.KilledBy("greyling_bested", TitleSlot.Epithet,
                Rarity.Epic, "Greyling-Bested|Twig-Beaten", 1, "$enemy_greyling"),
            TitleFragment.KilledBy("neck_nibbled", TitleSlot.Epithet,
                Rarity.Epic, "Neck-Nibbled|Lizard-Licked", 1, "$enemy_neck"),
            TitleFragment.KilledBy("greydwarf_pelted", TitleSlot.Epithet,
                Rarity.Uncommon, "Greydwarf-Pelted|Stone-Pelted", 3,
                "$enemy_greydwarf", "$enemy_greydwarfbrute", "$enemy_greydwarfshaman"),
            TitleFragment.KilledBy("troll_stomped", TitleSlot.Epithet,
                Rarity.Uncommon, "Troll-Stomped|Troll-Flattened|Club-Struck", 2,
                "$enemy_troll", "$enemy_trollfrost"),
            TitleFragment.KilledBy("bone_beaten", TitleSlot.Epithet,
                Rarity.Uncommon, "Bone-Beaten|Crypt-Taken", 3,
                "$enemy_skeleton", "$enemy_skeletonpoison", "$enemy_skeletonfire"),

            // Swamp. The poison deaths only land here because RecentAttackerTracker
            // remembers who poisoned you; the killing tick itself names nobody.
            TitleFragment.KilledBy("draugr_fodder", TitleSlot.Epithet,
                Rarity.Uncommon, "Draugr-Hewn|Draugr-Felled|Barrow-Taken", 3,
                "$enemy_draugr", "$enemy_draugrelite"),
            TitleFragment.KilledBy("draugr_meat", TitleSlot.Noun,
                Rarity.Rare, "Draugr-Fodder|Draugr-Meat", 6,
                "$enemy_draugr", "$enemy_draugrelite"),
            TitleFragment.KilledBy("ooze_choked", TitleSlot.Epithet,
                Rarity.Uncommon, "Ooze-Choked|Blob-Smothered|Slime-Drowned", 3,
                "$enemy_blob", "$enemy_blobelite"),
            TitleFragment.KilledBy("leech_drained", TitleSlot.Epithet,
                Rarity.Uncommon, "Leech-Drained|Leech-Supped", 2, "$enemy_leech"),
            TitleFragment.KilledBy("wraith_taken", TitleSlot.Epithet,
                Rarity.Rare, "Wraith-Taken|Ghost-Chilled", 2, "$enemy_wraith"),
            TitleFragment.KilledBy("root_crushed", TitleSlot.Epithet,
                Rarity.Rare, "Root-Crushed|Log-Walloped", 2, "$enemy_abomination"),

            // Mountain.
            TitleFragment.KilledBy("wolf_eaten", TitleSlot.Epithet,
                Rarity.Uncommon, "Wolf-Eaten|Wolf-Torn|Pack-Hunted", 3,
                "$enemy_wolf", "$enemy_ulv"),
            TitleFragment.KilledBy("drake_frozen", TitleSlot.Epithet,
                Rarity.Uncommon, "Drake-Frozen|Ice-Spat", 3, "$enemy_drake"),
            TitleFragment.KilledBy("fenring_torn", TitleSlot.Epithet,
                Rarity.Rare, "Fenring-Torn|Moon-Mauled", 2,
                "$enemy_fenring", "$enemy_fenringcultist"),
            TitleFragment.KilledBy("golem_pounded", TitleSlot.Epithet,
                Rarity.Rare, "Golem-Pounded|Crystal-Crushed", 2, "$enemy_stonegolem"),

            // Plains.
            TitleFragment.KilledBy("squito_stung", TitleSlot.Epithet,
                Rarity.Rare, "Squito-Stung|Needle-Struck", 2, "$enemy_deathsquito"),
            TitleFragment.KilledBy("fuling_clubbed", TitleSlot.Epithet,
                Rarity.Uncommon, "Fuling-Clubbed|Fuling-Felled", 3,
                "$enemy_goblin", "$enemy_goblinbrute", "$enemy_goblinshaman"),
            TitleFragment.KilledBy("lox_trampled", TitleSlot.Epithet,
                Rarity.Rare, "Lox-Trampled|Lox-Flattened", 2, "$enemy_lox"),

            // Ocean.
            TitleFragment.KilledBy("serpent_swallowed", TitleSlot.Epithet,
                Rarity.Rare, "Serpent-Eaten|Wyrm-Swallowed", 2, "$enemy_serpent"),

            // Mistlands.
            TitleFragment.KilledBy("seeker_stung", TitleSlot.Epithet,
                Rarity.Uncommon, "Seeker-Stung|Bug-Bitten", 3,
                "$enemy_seeker", "$enemy_seekerbrute"),
            TitleFragment.KilledBy("gjall_bombed", TitleSlot.Epithet,
                Rarity.Rare, "Gjall-Bombed|Sky-Spat", 2, "$enemy_gjall"),
            TitleFragment.KilledBy("tick_drained", TitleSlot.Epithet,
                Rarity.Rare, "Tick-Drained|Tick-Supped", 2, "$enemy_tick"),
            TitleFragment.KilledBy("dvergr_wronged", TitleSlot.Epithet,
                Rarity.Rare, "Dvergr-Wronged|Dwarf-Blasted", 2,
                "$enemy_dvergr", "$enemy_dvergr_mage"),

            // Ashlands.
            TitleFragment.KilledBy("charred_cut", TitleSlot.Epithet,
                Rarity.Uncommon, "Charred-Cut|Ash-Felled", 3,
                "$enemy_charred", "$enemy_charred_melee", "$enemy_charred_archer",
                "$enemy_charred_mage", "$enemy_charred_twitcher"),
            TitleFragment.KilledBy("morgen_mauled", TitleSlot.Epithet,
                Rarity.Rare, "Morgen-Mauled|Morgen-Torn", 2, "$enemy_morgen"),
            TitleFragment.KilledBy("valkyrie_taken", TitleSlot.Epithet,
                Rarity.Epic, "Valkyrie-Taken|Valkyrie-Chosen", 1, "$enemy_fallenvalkyrie"),

            // Bosses. One death to a boss is ordinary; a few says something.
            TitleFragment.KilledBy("stag_struck", TitleSlot.Epithet,
                Rarity.Rare, "Stag-Struck|Antler-Lit", 2, "$enemy_eikthyr"),
            TitleFragment.KilledBy("elder_crushed", TitleSlot.Epithet,
                Rarity.Rare, "Elder-Crushed|Root-Bound", 2, "$enemy_gdking"),
            TitleFragment.KilledBy("bonemass_taken", TitleSlot.Epithet,
                Rarity.Rare, "Bone-Mashed|Rot-Smothered", 2, "$enemy_bonemass"),
            TitleFragment.KilledBy("moder_frozen", TitleSlot.Epithet,
                Rarity.Rare, "Moder-Frozen|Wing-Chilled", 2, "$enemy_dragon"),
            TitleFragment.KilledBy("yagluth_burned", TitleSlot.Epithet,
                Rarity.Epic, "King-Burned|Yagluth-Scorched", 2, "$enemy_goblinking"),
            TitleFragment.KilledBy("queen_stung", TitleSlot.Epithet,
                Rarity.Epic, "Queen-Stung|Hive-Taken", 2, "$enemy_seekerqueen"),
            TitleFragment.KilledBy("fader_scorched", TitleSlot.Epithet,
                Rarity.Epic, "Fader-Scorched|Fire-Unmade", 2, "$enemy_fader"),
        };
    }
}
