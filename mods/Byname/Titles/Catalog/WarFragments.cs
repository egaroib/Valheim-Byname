using System.Collections.Generic;

namespace Byname.Titles.Catalog
{
    /// <summary>
    /// Killing, and how you go about it.
    ///
    /// The weapon-class fragments lean on <c>m_enemyStats</c> being split by
    /// <see cref="KillModifiers"/>, so "Bare-Fisted" means the player genuinely kills with
    /// their hands rather than merely having punched something once. Boss fragments
    /// separate solo from group kills, which Valheim records distinctly.
    ///
    /// Boss words carry the most variants in the catalog: on a shared server everyone is
    /// credited with the same boss kills on the same night, so without them a whole group
    /// wakes up as "Bane" together.
    /// </summary>
    internal static class WarFragments
    {
        private static TitleFragment Style(
            string id, Rarity rarity, string text, KillModifiers style, string styleName, float kills)
            => new TitleFragment(id, TitleSlot.Epithet, TitleCategory.Combat, rarity, text,
                s => s.DominantKillStyle() == style &&
                     s.Get(PlayerStatType.EnemyKills) >= TitleFragment.Scaled(TitleCategory.Combat, kills),
                new[] { PlayerStatType.EnemyKills },
                describe: s => $"most of your {TitleFragment.Number(s.Get(PlayerStatType.EnemyKills))} " +
                               $"kills are {styleName} (needed " +
                               $"{TitleFragment.Number(TitleFragment.Scaled(TitleCategory.Combat, kills))}+, " +
                               "and a clear majority)");

        internal static IEnumerable<TitleFragment> All => new[]
        {
            // --- Kill style. Requires a real majority, not a single instance. --
            Style("bare_fisted", Rarity.Legendary, "Bare-Fisted|Knuckle-Bloodied",
                KillModifiers.Unarmed, "unarmed", 100),
            Style("far_shooting", Rarity.Rare, "Far-Shooting|Keen-Eyed|Long-Aiming",
                KillModifiers.Ranged, "ranged", 200),
            Style("rune_handed", Rarity.Epic, "Rune-Handed|Staff-Wielding|Galdr-Wise",
                KillModifiers.Magic, "magic", 200),
            Style("close_fighting", Rarity.Uncommon, "Close-Fighting|Shield-Pressing|Toe-to-Toe",
                KillModifiers.Melee, "melee", 300),

            // --- Volume ------------------------------------------------------
            TitleFragment.Threshold("blood_worn", TitleSlot.Epithet, TitleCategory.Combat,
                Rarity.Uncommon, "Blood-Worn|Blade-Weary|Gore-Spattered", PlayerStatType.EnemyKills, 500),
            TitleFragment.Threshold("much_feared", TitleSlot.Epithet, TitleCategory.Combat,
                Rarity.Rare, "Much-Feared|Dread-Named|Grim-Famed", PlayerStatType.EnemyKills, 2500),
            TitleFragment.Threshold("red_handed", TitleSlot.Epithet, TitleCategory.Combat,
                Rarity.Epic, "Red-Handed|Slaughter-Glad", PlayerStatType.EnemyKills, 7500),
            TitleFragment.Threshold("shield_worn", TitleSlot.Epithet, TitleCategory.Combat,
                Rarity.Uncommon, "Shield-Worn|Dent-Helmed|Bruise-Hided", PlayerStatType.HitsTakenEnemies, 2000),
            TitleFragment.Threshold("arrow_glad", TitleSlot.Epithet, TitleCategory.Combat,
                Rarity.Uncommon, "Arrow-Glad|Quiver-Light", PlayerStatType.ArrowsShot, 2000),
            TitleFragment.Threshold("bone_calling", TitleSlot.Epithet, TitleCategory.Combat,
                Rarity.Rare, "Bone-Calling|Grave-Stirring", PlayerStatType.SkeletonSummons, 50),
            TitleFragment.Threshold("trap_setting", TitleSlot.Epithet, TitleCategory.Combat,
                Rarity.Uncommon, "Trap-Setting|Snare-Minded", PlayerStatType.TrapArmed, 20),

            TitleFragment.Threshold("slayer", TitleSlot.Noun, TitleCategory.Combat,
                Rarity.Common, "Slayer|Reaver|Brawler", PlayerStatType.EnemyKills, 200),
            TitleFragment.Threshold("hunter", TitleSlot.Noun, TitleCategory.Combat,
                Rarity.Uncommon, "Hunter|Stalker|Tracker", PlayerStatType.EnemyKillsLastHits, 500),
            TitleFragment.Threshold("bowman", TitleSlot.Noun, TitleCategory.Combat,
                Rarity.Uncommon, "Bowman|Archer|Bowyer", PlayerStatType.ArrowsShot, 1000),
            TitleFragment.Threshold("shieldbearer", TitleSlot.Noun, TitleCategory.Combat,
                Rarity.Rare, "Shieldbearer|Shieldwall|Bulwark", PlayerStatType.HitsTakenEnemies, 5000),
            TitleFragment.Threshold("bonecaller", TitleSlot.Noun, TitleCategory.Combat,
                Rarity.Epic, "Bonecaller|Necromancer", PlayerStatType.SkeletonSummons, 150),
            TitleFragment.Threshold("trapper", TitleSlot.Noun, TitleCategory.Combat,
                Rarity.Rare, "Trapper|Snarer", PlayerStatType.TrapTriggered, 25),
            TitleFragment.Threshold("ballista_keeper", TitleSlot.Noun, TitleCategory.Combat,
                Rarity.Uncommon, "Ballista-Keeper|Bolt-Loader", PlayerStatType.TurretAmmoAdded, 200),
            TitleFragment.Threshold("kinslayer", TitleSlot.Noun, TitleCategory.Combat,
                Rarity.Rare, "Kinslayer|Oathbreaker|Backstabber", PlayerStatType.PlayerKills, 3),

            TitleFragment.Threshold("red_field", TitleSlot.Domain, TitleCategory.Combat,
                Rarity.Uncommon, "Red Field|Blood Moor|Battlefield", PlayerStatType.EnemyKills, 1000),
            TitleFragment.Threshold("long_war", TitleSlot.Domain, TitleCategory.Combat,
                Rarity.Rare, "Long War|Endless Fray", PlayerStatType.EnemyHits, 20000),

            // --- Bosses ------------------------------------------------------
            TitleFragment.Threshold("crown_breaking", TitleSlot.Epithet, TitleCategory.Bosses,
                Rarity.Rare, "Crown-Breaking|King-Toppling|Throne-Breaking", PlayerStatType.BossKills, 5),
            TitleFragment.Threshold("god_touched", TitleSlot.Epithet, TitleCategory.Bosses,
                Rarity.Epic, "God-Touched|Power-Wielding|Altar-Blessed", PlayerStatType.UseGuardianPower, 200),
            TitleFragment.Threshold("alone_going", TitleSlot.Epithet, TitleCategory.Bosses,
                Rarity.Epic, "Alone-Going|Lone-Standing|Unaided", PlayerStatType.BossKillSolo, 3),
            TitleFragment.Threshold("oath_bound", TitleSlot.Epithet, TitleCategory.Bosses,
                Rarity.Uncommon, "Oath-Bound|Kin-Fighting|War-Banded", PlayerStatType.BossKillMultiplayer, 4),

            TitleFragment.Threshold("bane", TitleSlot.Noun, TitleCategory.Bosses,
                Rarity.Uncommon, "Bane|Horn-Taker|Crown-Taker|Beast-Queller", PlayerStatType.BossKills, 3),
            TitleFragment.Threshold("lone_slayer", TitleSlot.Noun, TitleCategory.Bosses,
                Rarity.Legendary, "Lone-Slayer|Lone-Wolf", PlayerStatType.BossKillSolo, 5),
            TitleFragment.Threshold("kingsbane", TitleSlot.Noun, TitleCategory.Bosses,
                Rarity.Legendary, "Kingsbane|Godsbane|Throne-Ender", PlayerStatType.BossKills, 8),
            TitleFragment.Threshold("deathbringer", TitleSlot.Noun, TitleCategory.Bosses,
                Rarity.Epic, "Deathbringer|Finisher|Last-Striker", PlayerStatType.BossLastHits, 3),

            TitleFragment.Threshold("nine_crowns", TitleSlot.Domain, TitleCategory.Bosses,
                Rarity.Legendary, "Nine Crowns|Fallen Gods", PlayerStatType.BossKills, 8),
            TitleFragment.Threshold("stag_horn", TitleSlot.Domain, TitleCategory.Bosses,
                Rarity.Uncommon, "Stag's Horn|Storm Stag", PlayerStatType.UsePowerEikthyr, 25),
            TitleFragment.Threshold("elder_wood", TitleSlot.Domain, TitleCategory.Bosses,
                Rarity.Rare, "Elder Wood|Old Root", PlayerStatType.UsePowerElder, 25),
            TitleFragment.Threshold("frozen_peak", TitleSlot.Domain, TitleCategory.Bosses,
                Rarity.Rare, "Frozen Peak|Dragon's Breath", PlayerStatType.UsePowerModer, 25),
            TitleFragment.Threshold("black_mire", TitleSlot.Domain, TitleCategory.Bosses,
                Rarity.Rare, "Black Mire|Bone Heap", PlayerStatType.UsePowerBonemass, 25),
            TitleFragment.Threshold("ashen_crown", TitleSlot.Domain, TitleCategory.Bosses,
                Rarity.Epic, "Ashen Crown|Fallen King", PlayerStatType.UsePowerYagluth, 25),
            TitleFragment.Threshold("hive_queen", TitleSlot.Domain, TitleCategory.Bosses,
                Rarity.Epic, "Hive Queen|Seeker Hive", PlayerStatType.UsePowerQueen, 25),
            TitleFragment.Threshold("last_fire", TitleSlot.Domain, TitleCategory.Bosses,
                Rarity.Epic, "Last Fire|Burning Ring", PlayerStatType.UsePowerAshlands, 25),
        };
    }
}
