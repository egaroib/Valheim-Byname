using System.Collections.Generic;

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
    /// </summary>
    internal static class BeastFragments
    {
        internal static IEnumerable<TitleFragment> All => new[]
        {
            // --- The trash mobs. Common on purpose: these are the first titles a new
            // --- character can earn, and the early game needs a populated pool.
            TitleFragment.Enemy("greyling_bane", TitleSlot.Noun,
                Rarity.Common, "Greyling-Bane", "$enemy_greyling", 100),
            TitleFragment.Enemy("boar_hunter", TitleSlot.Noun,
                Rarity.Common, "Boar-Hunter", "$enemy_boar", 60),
            TitleFragment.Enemy("deer_stalker", TitleSlot.Noun,
                Rarity.Common, "Deer-Stalker", "$enemy_deer", 40),
            TitleFragment.Enemy("neck_catcher", TitleSlot.Noun,
                Rarity.Common, "Neck-Catcher", "$enemy_neck", 80),
            TitleFragment.Enemy("greydwarf_bane", TitleSlot.Noun,
                Rarity.Uncommon, "Greydwarf-Bane", "$enemy_greydwarf", 400),
            TitleFragment.Enemy("skeleton_breaker", TitleSlot.Noun,
                Rarity.Uncommon, "Bone-Breaker", "$enemy_skeleton", 200),

            // --- Black Forest and Swamp -------------------------------------
            TitleFragment.Enemy("troll_slayer", TitleSlot.Noun,
                Rarity.Rare, "Troll-Slayer", "$enemy_troll", 50),
            TitleFragment.Enemy("draugr_bane", TitleSlot.Noun,
                Rarity.Uncommon, "Draugr-Bane", "$enemy_draugr", 250),
            TitleFragment.Enemy("blob_render", TitleSlot.Noun,
                Rarity.Uncommon, "Blob-Render", "$enemy_blob", 150),
            TitleFragment.Enemy("leech_puller", TitleSlot.Noun,
                Rarity.Rare, "Leech-Puller", "$enemy_leech", 120),
            TitleFragment.Enemy("wraith_banisher", TitleSlot.Noun,
                Rarity.Rare, "Wraith-Banisher", "$enemy_wraith", 40),

            // --- Mountain ----------------------------------------------------
            TitleFragment.Enemy("wolf_breaker", TitleSlot.Noun,
                Rarity.Rare, "Wolf-Breaker", "$enemy_wolf", 150),
            TitleFragment.Enemy("drake_downer", TitleSlot.Noun,
                Rarity.Rare, "Drake-Downer", "$enemy_drake", 80),
            TitleFragment.Enemy("fenring_hunter", TitleSlot.Noun,
                Rarity.Epic, "Fenring-Hunter", "$enemy_fenring", 60),
            TitleFragment.Enemy("stone_golem_breaker", TitleSlot.Noun,
                Rarity.Epic, "Golem-Breaker", "$enemy_stonegolem", 25),

            // --- Plains ------------------------------------------------------
            TitleFragment.Enemy("deathsquito_swatter", TitleSlot.Noun,
                Rarity.Rare, "Squito-Swatter", "$enemy_deathsquito", 100),
            TitleFragment.Enemy("fuling_bane", TitleSlot.Noun,
                Rarity.Rare, "Fuling-Bane", "$enemy_goblin", 200),
            TitleFragment.Enemy("lox_feller", TitleSlot.Noun,
                Rarity.Epic, "Lox-Feller", "$enemy_lox", 50),

            // --- Ocean -------------------------------------------------------
            TitleFragment.Enemy("serpent_slayer", TitleSlot.Noun,
                Rarity.Epic, "Serpent-Slayer", "$enemy_serpent", 15),

            // --- Mistlands ---------------------------------------------------
            TitleFragment.Enemy("seeker_render", TitleSlot.Noun,
                Rarity.Epic, "Seeker-Render", "$enemy_seeker", 100),
            TitleFragment.Enemy("gjall_downer", TitleSlot.Noun,
                Rarity.Legendary, "Gjall-Downer", "$enemy_gjall", 25),
            TitleFragment.Enemy("dvergr_breaker", TitleSlot.Noun,
                Rarity.Epic, "Dvergr-Breaker", "$enemy_dvergr", 40),

            // --- Ashlands ----------------------------------------------------
            TitleFragment.Enemy("charred_bane", TitleSlot.Noun,
                Rarity.Epic, "Charred-Bane", "$enemy_charred", 150),
            TitleFragment.Enemy("asksvin_hunter", TitleSlot.Noun,
                Rarity.Epic, "Asksvin-Hunter", "$enemy_asksvin", 50),
            TitleFragment.Enemy("morgen_breaker", TitleSlot.Noun,
                Rarity.Legendary, "Morgen-Breaker", "$enemy_morgen", 30),
            TitleFragment.Enemy("valkyrie_feller", TitleSlot.Noun,
                Rarity.Legendary, "Valkyrie-Feller", "$enemy_fallenvalkyrie", 15),

            // --- A few as epithets, so these can pair with a non-combat noun --
            TitleFragment.Enemy("troll_worn", TitleSlot.Epithet,
                Rarity.Rare, "Troll-Worn", "$enemy_troll", 30),
            TitleFragment.Enemy("wolf_bitten", TitleSlot.Epithet,
                Rarity.Rare, "Wolf-Bitten", "$enemy_wolf", 100),
            TitleFragment.Enemy("serpent_touched", TitleSlot.Epithet,
                Rarity.Epic, "Serpent-Touched", "$enemy_serpent", 10),
            TitleFragment.Enemy("greydwarf_weary", TitleSlot.Epithet,
                Rarity.Common, "Greydwarf-Weary", "$enemy_greydwarf", 150),
        };
    }
}
