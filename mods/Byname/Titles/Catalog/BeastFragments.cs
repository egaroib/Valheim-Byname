using System.Collections.Generic;
using System.Linq;

namespace Byname.Titles.Catalog
{
    /// <summary>
    /// Titles earned against one particular creature.
    ///
    /// These read <c>m_enemyStats</c>, the per-creature dictionary Valheim keys by
    /// <c>Character.m_name</c>. Every token below was taken from the game's own
    /// localisation table in resources.assets, not invented — an invented key produces a
    /// fragment that never fires and never complains.
    ///
    /// All are Combat, so the category anti-collision stops a title pairing two of them:
    /// nobody becomes "Greydwarf-Bane of the Greydwarves".
    ///
    /// Text is "First|Second|Third": each player is given one wording and keeps it, so
    /// three friends who all hunted trolls together are not all "Troll-Slayer".
    ///
    /// Every creature noun climbs: the hand-written word is the first rung, and
    /// <see cref="Ladders"/> adds rarer ones at higher counts. The engine only ever offers
    /// the highest rung earned, so 1,500 boars never rerolls back down to "Boar-Hunter".
    /// Remember every player who damaged a creature is credited with its kill, so a group
    /// that hunts together climbs together.
    /// </summary>
    internal static class BeastFragments
    {
        internal static IEnumerable<TitleFragment> All => Firsts.Concat(Ladders);

        /// <summary>
        /// Rungs above a creature's first word, one per threshold, each a rarity higher
        /// than the last and capped at Legendary. Worded by rarity rather than by rung, so
        /// "-Doom" always means Legendary whichever creature it is attached to.
        /// </summary>
        private static IEnumerable<TitleFragment> Ladder(
            string stem, string creature, string token, Rarity first, params float[] thresholds)
        {
            var rarity = first;
            foreach (var threshold in thresholds)
            {
                if (rarity < Rarity.Legendary) rarity++;
                yield return TitleFragment.Enemy(
                    $"{stem}_{rarity.ToString().ToLowerInvariant()}", TitleSlot.Noun, rarity,
                    RungWords(creature, rarity), token, threshold);
            }
        }

        private static string RungWords(string creature, Rarity rarity)
        {
            switch (rarity)
            {
                case Rarity.Uncommon: return $"{creature}-Harrier|{creature}-Culler";
                case Rarity.Rare: return $"{creature}-Scourge|{creature}-Butcher";
                case Rarity.Epic: return $"{creature}-Reaper|{creature}-Ruin";
                default: return $"{creature}-Doom|{creature}-Ender";
            }
        }

        private static IEnumerable<TitleFragment> Ladders =>
            // Meadows and Black Forest: farmed constantly, so the rungs sit far apart.
            Ladder("greyling", "Greyling", "$enemy_greyling", Rarity.Common, 400, 1000, 2500)
            .Concat(Ladder("boar", "Boar", "$enemy_boar", Rarity.Common, 250, 600, 1500))
            .Concat(Ladder("deer", "Deer", "$enemy_deer", Rarity.Common, 200, 500, 1200))
            .Concat(Ladder("neck", "Neck", "$enemy_neck", Rarity.Common, 250, 600, 1500))
            .Concat(Ladder("greydwarf", "Greydwarf", "$enemy_greydwarf", Rarity.Uncommon, 1500, 4000, 10000))
            .Concat(Ladder("skeleton", "Skeleton", "$enemy_skeleton", Rarity.Uncommon, 800, 2000, 5000))
            .Concat(Ladder("troll", "Troll", "$enemy_troll", Rarity.Rare, 150, 400))

            // Swamp.
            .Concat(Ladder("draugr", "Draugr", "$enemy_draugr", Rarity.Uncommon, 800, 2000, 5000))
            .Concat(Ladder("blob", "Blob", "$enemy_blob", Rarity.Uncommon, 500, 1500, 4000))
            .Concat(Ladder("surtling", "Surtling", "$enemy_surtling", Rarity.Uncommon, 200, 600, 1500))
            .Concat(Ladder("leech", "Leech", "$enemy_leech", Rarity.Rare, 400, 1000))
            .Concat(Ladder("wraith", "Wraith", "$enemy_wraith", Rarity.Rare, 120, 300))
            .Concat(Ladder("abomination", "Abomination", "$enemy_abomination", Rarity.Epic, 50))

            // Mountain.
            .Concat(Ladder("wolf", "Wolf", "$enemy_wolf", Rarity.Rare, 500, 1200))
            .Concat(Ladder("drake", "Drake", "$enemy_drake", Rarity.Rare, 250, 700))
            .Concat(Ladder("fenring", "Fenring", "$enemy_fenring", Rarity.Epic, 200))
            .Concat(Ladder("stonegolem", "Golem", "$enemy_stonegolem", Rarity.Epic, 80))
            .Concat(Ladder("ulv", "Ulv", "$enemy_ulv", Rarity.Epic, 100))

            // Plains.
            .Concat(Ladder("deathsquito", "Squito", "$enemy_deathsquito", Rarity.Rare, 300, 800))
            .Concat(Ladder("fuling", "Fuling", "$enemy_goblin", Rarity.Rare, 700, 2000))
            .Concat(Ladder("lox", "Lox", "$enemy_lox", Rarity.Epic, 150))

            // Ocean.
            .Concat(Ladder("serpent", "Serpent", "$enemy_serpent", Rarity.Epic, 50))

            // Mistlands.
            .Concat(Ladder("seeker", "Seeker", "$enemy_seeker", Rarity.Epic, 400))
            .Concat(Ladder("seekerbrute", "Soldier", "$enemy_seekerbrute", Rarity.Epic, 80))
            .Concat(Ladder("tick", "Tick", "$enemy_tick", Rarity.Rare, 200, 500))
            .Concat(Ladder("gjall", "Gjall", "$enemy_gjall", Rarity.Epic, 75))
            .Concat(Ladder("dvergr", "Dvergr", "$enemy_dvergr", Rarity.Epic, 150))

            // Ashlands.
            .Concat(Ladder("charred", "Charred", "$enemy_charred", Rarity.Epic, 600))
            .Concat(Ladder("asksvin", "Asksvin", "$enemy_asksvin", Rarity.Epic, 200))
            .Concat(Ladder("morgen", "Morgen", "$enemy_morgen", Rarity.Epic, 100))
            .Concat(Ladder("valkyrie", "Valkyrie", "$enemy_fallenvalkyrie", Rarity.Epic, 50));

        private static IEnumerable<TitleFragment> Firsts => new[]
        {
            // --- The trash mobs. Common on purpose: these are the first titles a new
            // --- character can earn, and the early game needs a populated pool.
            TitleFragment.Enemy("greyling_bane", TitleSlot.Noun,
                Rarity.Common, "Greyling-Bane|Greyling-Swatter", "$enemy_greyling", 100),
            TitleFragment.Enemy("boar_hunter", TitleSlot.Noun,
                Rarity.Common, "Boar-Hunter|Boar-Sticker|Tusk-Taker", "$enemy_boar", 60),
            TitleFragment.Enemy("deer_stalker", TitleSlot.Noun,
                Rarity.Common, "Deer-Stalker|Stag-Hunter|Antler-Taker", "$enemy_deer", 40),
            TitleFragment.Enemy("neck_catcher", TitleSlot.Noun,
                Rarity.Common, "Neck-Catcher|Neck-Wringer", "$enemy_neck", 80),
            TitleFragment.Enemy("greydwarf_bane", TitleSlot.Noun,
                Rarity.Uncommon, "Greydwarf-Bane|Greydwarf-Breaker", "$enemy_greydwarf", 400),
            TitleFragment.Enemy("skeleton_breaker", TitleSlot.Noun,
                Rarity.Uncommon, "Bone-Breaker|Skull-Cracker", "$enemy_skeleton", 200),

            // --- Black Forest and Swamp -------------------------------------
            TitleFragment.Enemy("troll_slayer", TitleSlot.Noun,
                Rarity.Rare, "Troll-Slayer|Troll-Feller|Troll-Toppler", "$enemy_troll", 50),
            TitleFragment.Enemy("draugr_bane", TitleSlot.Noun,
                Rarity.Uncommon, "Draugr-Bane|Draugr-Unmaker|Barrow-Breaker", "$enemy_draugr", 250),
            TitleFragment.Enemy("blob_render", TitleSlot.Noun,
                Rarity.Uncommon, "Blob-Render|Ooze-Popper", "$enemy_blob", 150),
            TitleFragment.Enemy("leech_puller", TitleSlot.Noun,
                Rarity.Rare, "Leech-Puller|Leech-Burner", "$enemy_leech", 120),
            TitleFragment.Enemy("wraith_banisher", TitleSlot.Noun,
                Rarity.Rare, "Wraith-Banisher|Ghost-Cutter", "$enemy_wraith", 40),
            TitleFragment.Enemy("abomination_feller", TitleSlot.Noun,
                Rarity.Epic, "Root-Cutter|Bog-Feller", "$enemy_abomination", 15),
            TitleFragment.Enemy("surtling_snuffer", TitleSlot.Noun,
                Rarity.Uncommon, "Surtling-Snuffer|Ember-Quencher", "$enemy_surtling", 60),

            // --- Mountain ----------------------------------------------------
            TitleFragment.Enemy("wolf_breaker", TitleSlot.Noun,
                Rarity.Rare, "Wolf-Breaker|Wolf-Hunter|Pelt-Taker", "$enemy_wolf", 150),
            TitleFragment.Enemy("drake_downer", TitleSlot.Noun,
                Rarity.Rare, "Drake-Downer|Drake-Hunter", "$enemy_drake", 80),
            TitleFragment.Enemy("fenring_hunter", TitleSlot.Noun,
                Rarity.Epic, "Fenring-Hunter|Moon-Hunter", "$enemy_fenring", 60),
            TitleFragment.Enemy("stone_golem_breaker", TitleSlot.Noun,
                Rarity.Epic, "Golem-Breaker|Crystal-Cracker", "$enemy_stonegolem", 25),
            TitleFragment.Enemy("ulv_hunter", TitleSlot.Noun,
                Rarity.Epic, "Ulv-Hunter|Ulv-Bane", "$enemy_ulv", 30),

            // --- Plains ------------------------------------------------------
            TitleFragment.Enemy("deathsquito_swatter", TitleSlot.Noun,
                Rarity.Rare, "Squito-Swatter|Needle-Breaker", "$enemy_deathsquito", 100),
            TitleFragment.Enemy("fuling_bane", TitleSlot.Noun,
                Rarity.Rare, "Fuling-Bane|Fuling-Smasher|Goblin-Bane", "$enemy_goblin", 200),
            TitleFragment.Enemy("lox_feller", TitleSlot.Noun,
                Rarity.Epic, "Lox-Feller|Lox-Toppler", "$enemy_lox", 50),

            // --- Ocean -------------------------------------------------------
            TitleFragment.Enemy("serpent_slayer", TitleSlot.Noun,
                Rarity.Epic, "Serpent-Slayer|Wyrm-Gaffer", "$enemy_serpent", 15),

            // --- Mistlands ---------------------------------------------------
            TitleFragment.Enemy("seeker_render", TitleSlot.Noun,
                Rarity.Epic, "Seeker-Render|Bug-Crusher", "$enemy_seeker", 100),
            TitleFragment.Enemy("seeker_brute_breaker", TitleSlot.Noun,
                Rarity.Epic, "Soldier-Breaker|Carapace-Cracker", "$enemy_seekerbrute", 25),
            TitleFragment.Enemy("tick_squasher", TitleSlot.Noun,
                Rarity.Rare, "Tick-Squasher|Tick-Picker", "$enemy_tick", 50),
            TitleFragment.Enemy("gjall_downer", TitleSlot.Noun,
                Rarity.Epic, "Gjall-Downer|Sky-Gutter", "$enemy_gjall", 25),
            TitleFragment.Enemy("dvergr_breaker", TitleSlot.Noun,
                Rarity.Epic, "Dvergr-Breaker|Dwarf-Wronger", "$enemy_dvergr", 40),

            // --- Ashlands ----------------------------------------------------
            TitleFragment.Enemy("charred_bane", TitleSlot.Noun,
                Rarity.Epic, "Charred-Bane|Ash-Sweeper", "$enemy_charred", 150),
            TitleFragment.Enemy("asksvin_hunter", TitleSlot.Noun,
                Rarity.Epic, "Asksvin-Hunter|Scale-Taker", "$enemy_asksvin", 50),
            TitleFragment.Enemy("morgen_breaker", TitleSlot.Noun,
                Rarity.Epic, "Morgen-Breaker|Morgen-Bane", "$enemy_morgen", 30),
            TitleFragment.Enemy("valkyrie_feller", TitleSlot.Noun,
                Rarity.Epic, "Valkyrie-Feller|Wing-Clipper", "$enemy_fallenvalkyrie", 15),

            // --- A few as epithets, so these can pair with a non-combat noun --
            TitleFragment.Enemy("troll_worn", TitleSlot.Epithet,
                Rarity.Rare, "Troll-Worn|Troll-Tested", "$enemy_troll", 30),
            TitleFragment.Enemy("wolf_bitten", TitleSlot.Epithet,
                Rarity.Rare, "Wolf-Bitten|Wolf-Scarred", "$enemy_wolf", 100),
            TitleFragment.Enemy("serpent_touched", TitleSlot.Epithet,
                Rarity.Epic, "Serpent-Touched|Wyrm-Blooded", "$enemy_serpent", 10),
            TitleFragment.Enemy("greydwarf_weary", TitleSlot.Epithet,
                Rarity.Common, "Greydwarf-Weary|Pinecone-Pelted", "$enemy_greydwarf", 150),
            TitleFragment.Enemy("draugr_haunted", TitleSlot.Epithet,
                Rarity.Uncommon, "Draugr-Haunted|Crypt-Hardened", "$enemy_draugr", 120),
        };
    }
}
