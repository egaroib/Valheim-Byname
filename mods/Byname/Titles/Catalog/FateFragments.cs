using System.Collections.Generic;

namespace Byname.Titles.Catalog
{
    /// <summary>
    /// How you die, and how long you manage not to.
    ///
    /// The richest seam in the whole catalog, because Valheim tracks death by cause in
    /// absurd detail — drowning, burning, freezing, carts, boats, drawbridges, and trees
    /// broken out by tier. A player is far more identifiable by what keeps killing them
    /// than by what they kill.
    /// </summary>
    internal static class FateFragments
    {
        internal static IEnumerable<TitleFragment> All => new[]
        {
            // --- Unconditional. Without these a fresh character renders blank, which
            // --- reads as a broken mod rather than as a story not yet told.
            TitleFragment.Always("unproven", TitleSlot.Epithet, TitleCategory.Misc, "Unproven"),
            TitleFragment.Always("stranger", TitleSlot.Noun, TitleCategory.Misc, "Stranger"),

            // --- Death: epithets ---------------------------------------------
            TitleFragment.Threshold("sodden", TitleSlot.Epithet, TitleCategory.Death,
                Rarity.Uncommon, "Sodden", PlayerStatType.DeathByDrowning, 3),
            TitleFragment.Threshold("deep_drowned", TitleSlot.Epithet, TitleCategory.Death,
                Rarity.Epic, "Deep-Drowned", PlayerStatType.DeathByDrowning, 15),
            TitleFragment.Threshold("timber_struck", TitleSlot.Epithet, TitleCategory.Death,
                Rarity.Rare, "Timber-Struck", PlayerStatType.DeathByTree, 1),
            TitleFragment.Threshold("oft_felled", TitleSlot.Epithet, TitleCategory.Death,
                Rarity.Epic, "Oft-Felled", PlayerStatType.DeathByTree, 4),
            TitleFragment.Threshold("ember_scarred", TitleSlot.Epithet, TitleCategory.Death,
                Rarity.Uncommon, "Ember-Scarred", PlayerStatType.DeathByBurning, 3),
            TitleFragment.Threshold("frost_bitten", TitleSlot.Epithet, TitleCategory.Death,
                Rarity.Uncommon, "Frost-Bitten", PlayerStatType.DeathByFreezing, 3),
            TitleFragment.Threshold("venom_touched", TitleSlot.Epithet, TitleCategory.Death,
                Rarity.Uncommon, "Venom-Touched", PlayerStatType.DeathByPoisoned, 5),
            TitleFragment.Threshold("smoke_choked", TitleSlot.Epithet, TitleCategory.Death,
                Rarity.Rare, "Smoke-Choked", PlayerStatType.DeathBySmoke, 3),
            TitleFragment.Threshold("cliff_fallen", TitleSlot.Epithet, TitleCategory.Death,
                Rarity.Uncommon, "Cliff-Fallen", PlayerStatType.DeathByFall, 5),
            TitleFragment.Threshold("cart_crushed", TitleSlot.Epithet, TitleCategory.Death,
                Rarity.Rare, "Cart-Crushed", PlayerStatType.DeathByCart, 1),
            TitleFragment.Threshold("keel_hauled", TitleSlot.Epithet, TitleCategory.Death,
                Rarity.Rare, "Keel-Hauled", PlayerStatType.DeathByBoat, 1),
            TitleFragment.Threshold("self_undone", TitleSlot.Epithet, TitleCategory.Death,
                Rarity.Rare, "Self-Undone", PlayerStatType.DeathBySelf, 3),
            TitleFragment.Threshold("spike_struck", TitleSlot.Epithet, TitleCategory.Death,
                Rarity.Epic, "Spike-Struck", PlayerStatType.DeathByStalagtite, 1),
            TitleFragment.Threshold("lava_kissed", TitleSlot.Epithet, TitleCategory.Death,
                Rarity.Epic, "Lava-Kissed", PlayerStatType.DeathByAshlandsLava, 1),
            TitleFragment.Threshold("hollow_eyed", TitleSlot.Epithet, TitleCategory.Death,
                Rarity.Rare, "Hollow-Eyed", PlayerStatType.Deaths, 50),
            TitleFragment.Threshold("much_mourned", TitleSlot.Epithet, TitleCategory.Death,
                Rarity.Epic, "Much-Mourned", PlayerStatType.Deaths, 150),
            TitleFragment.Threshold("stone_crushed", TitleSlot.Epithet, TitleCategory.Death,
                Rarity.Rare, "Stone-Crushed", PlayerStatType.DeathByStructural, 2),

            // The two funniest deaths in the game, and both are single-event rarities.
            TitleFragment.Threshold("worlds_edge", TitleSlot.Epithet, TitleCategory.Death,
                Rarity.Legendary, "World's-Edge", PlayerStatType.DeathByEdgeOfWorld, 1),
            TitleFragment.Threshold("gate_crushed", TitleSlot.Epithet, TitleCategory.Death,
                Rarity.Legendary, "Gate-Crushed", PlayerStatType.DeathByDrawBridge, 1),

            // --- Death: nouns ------------------------------------------------
            TitleFragment.Threshold("gravewalker", TitleSlot.Noun, TitleCategory.Death,
                Rarity.Uncommon, "Gravewalker", PlayerStatType.Deaths, 30),
            TitleFragment.Threshold("revenant", TitleSlot.Noun, TitleCategory.Death,
                Rarity.Epic, "Revenant", PlayerStatType.Deaths, 120),
            TitleFragment.Threshold("drowned", TitleSlot.Noun, TitleCategory.Death,
                Rarity.Rare, "Drowned", PlayerStatType.DeathByDrowning, 8),
            TitleFragment.Threshold("graverobber", TitleSlot.Noun, TitleCategory.Death,
                Rarity.Rare, "Graverobber", PlayerStatType.TombstonesOpenedOther, 1),

            // --- Death: domains ----------------------------------------------
            TitleFragment.Threshold("the_deep", TitleSlot.Domain, TitleCategory.Death,
                Rarity.Uncommon, "Deep", PlayerStatType.DeathByDrowning, 1),
            TitleFragment.Threshold("cold_ground", TitleSlot.Domain, TitleCategory.Death,
                Rarity.Uncommon, "Cold Ground", PlayerStatType.Deaths, 20),
            TitleFragment.Threshold("long_fall", TitleSlot.Domain, TitleCategory.Death,
                Rarity.Uncommon, "Long Fall", PlayerStatType.DeathByFall, 3),
            TitleFragment.Threshold("pyre", TitleSlot.Domain, TitleCategory.Death,
                Rarity.Rare, "Pyre", PlayerStatType.DeathByBurning, 5),

            // --- Survival. The inverse of the above, and deliberately hard. ---
            new TitleFragment("undying", TitleSlot.Epithet, TitleCategory.Death,
                Rarity.Legendary, "Undying",
                s => s.Get(PlayerStatType.Deaths) == 0 && s.Get(PlayerStatType.EnemyKills) >= 200,
                new[] { PlayerStatType.Deaths, PlayerStatType.EnemyKills },
                s => s.Get(PlayerStatType.EnemyKills) / 200f,
                describe: s => $"{TitleFragment.Number(s.Get(PlayerStatType.EnemyKills))} kills " +
                               "and not one death (needed 200 kills, 0 deaths)"),

            new TitleFragment("scarce_mourned", TitleSlot.Epithet, TitleCategory.Death,
                Rarity.Epic, "Scarce-Mourned",
                s => s.Get(PlayerStatType.Deaths) <= 2 &&
                     s.Get(PlayerStatType.ConsecutiveDaysSurvivedMax) >= 50,
                new[] { PlayerStatType.Deaths, PlayerStatType.ConsecutiveDaysSurvivedMax },
                s => s.Get(PlayerStatType.ConsecutiveDaysSurvivedMax) / 50f,
                describe: s => $"{TitleFragment.Number(s.Get(PlayerStatType.ConsecutiveDaysSurvivedMax))} " +
                               $"days survived unbroken, only {TitleFragment.Number(s.Get(PlayerStatType.Deaths))} " +
                               "deaths (needed 50 days, at most 2)"),

            TitleFragment.Threshold("long_lived", TitleSlot.Epithet, TitleCategory.Misc,
                Rarity.Epic, "Long-Lived", PlayerStatType.ConsecutiveDaysSurvivedMax, 100),

            // --- Misc: the odds and ends that make a title feel personal ------
            TitleFragment.Threshold("well_rested", TitleSlot.Epithet, TitleCategory.Misc,
                Rarity.Uncommon, "Well-Rested", PlayerStatType.Sleep, 100),
            TitleFragment.Threshold("raven_spoken", TitleSlot.Epithet, TitleCategory.Misc,
                Rarity.Rare, "Raven-Spoken", PlayerStatType.RavenTalk, 30),
            TitleFragment.Threshold("leap_glad", TitleSlot.Epithet, TitleCategory.Misc,
                Rarity.Uncommon, "Leap-Glad", PlayerStatType.Jumps, 5000),
            TitleFragment.Threshold("portal_worn", TitleSlot.Epithet, TitleCategory.Misc,
                Rarity.Uncommon, "Portal-Worn", PlayerStatType.PortalsUsed, 500),

            // Tracked because Valheim tracks it. Nobody sets out to earn this one.
            TitleFragment.Threshold("door_minded", TitleSlot.Epithet, TitleCategory.Misc,
                Rarity.Rare, "Door-Minded", PlayerStatType.DoorsClosed, 500),

            TitleFragment.Threshold("skald", TitleSlot.Noun, TitleCategory.Misc,
                Rarity.Uncommon, "Skald", PlayerStatType.RavenTalk, 10),
            TitleFragment.Threshold("hoarder", TitleSlot.Noun, TitleCategory.Misc,
                Rarity.Rare, "Hoarder", PlayerStatType.ItemsPickedUp, 10000),
            TitleFragment.Threshold("sleeper", TitleSlot.Noun, TitleCategory.Misc,
                Rarity.Rare, "Sleeper", PlayerStatType.Sleep, 200),
            TitleFragment.Threshold("elder", TitleSlot.Noun, TitleCategory.Misc,
                Rarity.Epic, "Elder", PlayerStatType.ConsecutiveDaysSurvivedMax, 150),

            TitleFragment.Threshold("long_night", TitleSlot.Domain, TitleCategory.Misc,
                Rarity.Uncommon, "Long Night", PlayerStatType.Sleep, 50),
            TitleFragment.Threshold("old_ways", TitleSlot.Domain, TitleCategory.Misc,
                Rarity.Rare, "Old Ways", PlayerStatType.ConsecutiveDaysSurvivedMax, 50),
        };
    }
}
