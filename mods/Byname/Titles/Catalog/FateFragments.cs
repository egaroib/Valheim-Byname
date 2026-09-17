using System.Collections.Generic;

namespace Byname.Titles.Catalog
{
    /// <summary>
    /// How you die, and how long you manage not to.
    ///
    /// The richest seam in the whole catalog, because Valheim tracks death by cause in
    /// absurd detail — drowning, burning, freezing, carts, boats, drawbridges, and trees
    /// broken out by tier. A player is far more identifiable by what keeps killing them
    /// than by what they kill. Who and where killed you lives in DoomFragments, since the
    /// game does not record it and Byname has to.
    /// </summary>
    internal static class FateFragments
    {
        internal static IEnumerable<TitleFragment> All => new[]
        {
            // --- Unconditional. Without these a fresh character renders blank, which
            // --- reads as a broken mod rather than as a story not yet told.
            TitleFragment.Always("unproven", TitleSlot.Epithet, TitleCategory.Misc, "Unproven|Untested|Untried"),
            TitleFragment.Always("stranger", TitleSlot.Noun, TitleCategory.Misc, "Stranger|Newcomer|Outlander"),

            // --- Death: epithets ---------------------------------------------
            TitleFragment.Threshold("sodden", TitleSlot.Epithet, TitleCategory.Death,
                Rarity.Uncommon, "Sodden|Waterlogged|Brine-Lunged", PlayerStatType.DeathByDrowning, 3),
            TitleFragment.Threshold("deep_drowned", TitleSlot.Epithet, TitleCategory.Death,
                Rarity.Epic, "Deep-Drowned|Sea-Claimed", PlayerStatType.DeathByDrowning, 15),
            TitleFragment.Threshold("timber_struck", TitleSlot.Epithet, TitleCategory.Death,
                Rarity.Rare, "Timber-Struck|Tree-Flattened", PlayerStatType.DeathByTree, 1),
            TitleFragment.Threshold("oft_felled", TitleSlot.Epithet, TitleCategory.Death,
                Rarity.Epic, "Oft-Felled|Forest-Hated", PlayerStatType.DeathByTree, 4),
            TitleFragment.Threshold("ember_scarred", TitleSlot.Epithet, TitleCategory.Death,
                Rarity.Uncommon, "Ember-Scarred|Smoke-Blackened|Twice-Singed", PlayerStatType.DeathByBurning, 3),
            TitleFragment.Threshold("frost_bitten", TitleSlot.Epithet, TitleCategory.Death,
                Rarity.Uncommon, "Frost-Bitten|Rime-Touched|Blue-Lipped", PlayerStatType.DeathByFreezing, 3),
            TitleFragment.Threshold("venom_touched", TitleSlot.Epithet, TitleCategory.Death,
                Rarity.Uncommon, "Venom-Touched|Poison-Veined|Green-Gilled", PlayerStatType.DeathByPoisoned, 4),
            TitleFragment.Threshold("smoke_choked", TitleSlot.Epithet, TitleCategory.Death,
                Rarity.Rare, "Smoke-Choked|Soot-Lunged", PlayerStatType.DeathBySmoke, 3),
            TitleFragment.Threshold("cliff_fallen", TitleSlot.Epithet, TitleCategory.Death,
                Rarity.Uncommon, "Cliff-Fallen|Ledge-Leaping|Gravity-Scorned", PlayerStatType.DeathByFall, 5),
            TitleFragment.Threshold("cart_crushed", TitleSlot.Epithet, TitleCategory.Death,
                Rarity.Rare, "Cart-Crushed|Wheel-Pressed", PlayerStatType.DeathByCart, 1),
            TitleFragment.Threshold("keel_hauled", TitleSlot.Epithet, TitleCategory.Death,
                Rarity.Rare, "Keel-Hauled|Hull-Struck", PlayerStatType.DeathByBoat, 1),
            TitleFragment.Threshold("self_undone", TitleSlot.Epithet, TitleCategory.Death,
                Rarity.Rare, "Self-Undone|Self-Slain", PlayerStatType.DeathBySelf, 3),
            TitleFragment.Threshold("spike_struck", TitleSlot.Epithet, TitleCategory.Death,
                Rarity.Epic, "Spike-Struck|Ceiling-Speared", PlayerStatType.DeathByStalagtite, 1),
            TitleFragment.Threshold("lava_kissed", TitleSlot.Epithet, TitleCategory.Death,
                Rarity.Epic, "Lava-Kissed|Magma-Bathed", PlayerStatType.DeathByAshlandsLava, 1),
            TitleFragment.Threshold("hollow_eyed", TitleSlot.Epithet, TitleCategory.Death,
                Rarity.Rare, "Hollow-Eyed|Oft-Buried|Grave-Familiar", PlayerStatType.Deaths, 50),
            TitleFragment.Threshold("much_mourned", TitleSlot.Epithet, TitleCategory.Death,
                Rarity.Epic, "Much-Mourned|Hel-Weary", PlayerStatType.Deaths, 150),
            TitleFragment.Threshold("stone_crushed", TitleSlot.Epithet, TitleCategory.Death,
                Rarity.Rare, "Stone-Crushed|Roof-Buried", PlayerStatType.DeathByStructural, 2),
            TitleFragment.Threshold("cinder_burnt", TitleSlot.Epithet, TitleCategory.Death,
                Rarity.Rare, "Cinder-Burnt|Ash-Rained", PlayerStatType.DeathByCinderFire, 1),
            TitleFragment.Threshold("sea_boiled", TitleSlot.Epithet, TitleCategory.Death,
                Rarity.Rare, "Sea-Boiled|Boil-Drowned", PlayerStatType.DeathByAshlandsOcean, 1),
            TitleFragment.Threshold("bolt_pinned", TitleSlot.Epithet, TitleCategory.Death,
                Rarity.Epic, "Bolt-Pinned|Ballista-Struck", PlayerStatType.DeathByTurret, 1),
            TitleFragment.Threshold("catapult_struck", TitleSlot.Epithet, TitleCategory.Death,
                Rarity.Epic, "Catapult-Struck|Siege-Flattened", PlayerStatType.DeathByCatapult, 1),
            TitleFragment.Threshold("friend_slain", TitleSlot.Epithet, TitleCategory.Death,
                Rarity.Rare, "Friend-Slain|Kin-Struck|Back-Stabbed", PlayerStatType.DeathByPlayerHit, 1),

            // The funniest deaths in the game, and all are single-event rarities.
            TitleFragment.Threshold("worlds_edge", TitleSlot.Epithet, TitleCategory.Death,
                Rarity.Legendary, "World's-Edge|Edge-Fallen", PlayerStatType.DeathByEdgeOfWorld, 1),
            TitleFragment.Threshold("gate_crushed", TitleSlot.Epithet, TitleCategory.Death,
                Rarity.Legendary, "Gate-Crushed|Drawbridge-Flat", PlayerStatType.DeathByDrawBridge, 1),
            TitleFragment.Threshold("obliterated", TitleSlot.Epithet, TitleCategory.Death,
                Rarity.Legendary, "Obliterated|Incinerated", PlayerStatType.DeathByIncinerator, 1),

            // --- Death: nouns ------------------------------------------------
            TitleFragment.Threshold("gravewalker", TitleSlot.Noun, TitleCategory.Death,
                Rarity.Uncommon, "Gravewalker|Corpse-Walker|Grave-Returner", PlayerStatType.Deaths, 30),
            TitleFragment.Threshold("revenant", TitleSlot.Noun, TitleCategory.Death,
                Rarity.Epic, "Revenant|Wight|Hel-Walker", PlayerStatType.Deaths, 120),
            TitleFragment.Threshold("drowned", TitleSlot.Noun, TitleCategory.Death,
                Rarity.Rare, "Drowned|Kelp-Crown", PlayerStatType.DeathByDrowning, 8),
            TitleFragment.Threshold("graverobber", TitleSlot.Noun, TitleCategory.Death,
                Rarity.Rare, "Graverobber|Grave-Picker", PlayerStatType.TombstonesOpenedOther, 1),
            TitleFragment.Threshold("corpse_runner", TitleSlot.Noun, TitleCategory.Death,
                Rarity.Uncommon, "Corpse-Runner|Grave-Fetcher|Kit-Chaser", PlayerStatType.TombstonesOpenedOwn, 15),

            // --- Death: domains ----------------------------------------------
            TitleFragment.Threshold("the_deep", TitleSlot.Domain, TitleCategory.Death,
                Rarity.Uncommon, "Deep|Depths|Undertow", PlayerStatType.DeathByDrowning, 1),
            TitleFragment.Threshold("cold_ground", TitleSlot.Domain, TitleCategory.Death,
                Rarity.Uncommon, "Cold Ground|Shallow Grave|Barrow", PlayerStatType.Deaths, 20),
            TitleFragment.Threshold("long_fall", TitleSlot.Domain, TitleCategory.Death,
                Rarity.Uncommon, "Long Fall|Sudden Drop", PlayerStatType.DeathByFall, 3),
            TitleFragment.Threshold("pyre", TitleSlot.Domain, TitleCategory.Death,
                Rarity.Rare, "Pyre|Bonfire", PlayerStatType.DeathByBurning, 5),

            // --- Survival. The inverse of the above, and deliberately hard. ---
            new TitleFragment("undying", TitleSlot.Epithet, TitleCategory.Death,
                Rarity.Legendary, "Undying|Hel-Denied",
                s => s.Get(PlayerStatType.Deaths) == 0 &&
                     s.Get(PlayerStatType.EnemyKills) >= TitleFragment.Scaled(TitleCategory.Death, 200),
                new[] { PlayerStatType.Deaths, PlayerStatType.EnemyKills },
                s => s.Get(PlayerStatType.EnemyKills) / TitleFragment.Scaled(TitleCategory.Death, 200),
                describe: s => $"{TitleFragment.Number(s.Get(PlayerStatType.EnemyKills))} kills " +
                               $"and not one death (needed {TitleFragment.Number(TitleFragment.Scaled(TitleCategory.Death, 200))} kills, 0 deaths)"),

            new TitleFragment("scarce_mourned", TitleSlot.Epithet, TitleCategory.Death,
                Rarity.Epic, "Scarce-Mourned|Death-Dodging",
                s => s.Get(PlayerStatType.Deaths) <= 2 &&
                     s.Get(PlayerStatType.ConsecutiveDaysSurvivedMax) >= TitleFragment.Scaled(TitleCategory.Death, 50),
                new[] { PlayerStatType.Deaths, PlayerStatType.ConsecutiveDaysSurvivedMax },
                s => s.Get(PlayerStatType.ConsecutiveDaysSurvivedMax) / TitleFragment.Scaled(TitleCategory.Death, 50),
                describe: s => $"{TitleFragment.Number(s.Get(PlayerStatType.ConsecutiveDaysSurvivedMax))} " +
                               $"days survived unbroken, only {TitleFragment.Number(s.Get(PlayerStatType.Deaths))} " +
                               $"deaths (needed {TitleFragment.Number(TitleFragment.Scaled(TitleCategory.Death, 50))} days, at most 2)"),

            TitleFragment.Threshold("long_lived", TitleSlot.Epithet, TitleCategory.Misc,
                Rarity.Epic, "Long-Lived|Grey-Bearded", PlayerStatType.ConsecutiveDaysSurvivedMax, 100),

            // --- Misc: the odds and ends that make a title feel personal ------
            TitleFragment.Threshold("well_rested", TitleSlot.Epithet, TitleCategory.Misc,
                Rarity.Uncommon, "Well-Rested|Sleep-Glad|Bed-Loving", PlayerStatType.Sleep, 250),
            TitleFragment.Threshold("raven_spoken", TitleSlot.Epithet, TitleCategory.Misc,
                Rarity.Rare, "Raven-Spoken|Munin-Heeding", PlayerStatType.RavenTalk, 30),
            TitleFragment.Threshold("leap_glad", TitleSlot.Epithet, TitleCategory.Misc,
                Rarity.Uncommon, "Leap-Glad|Spring-Heeled", PlayerStatType.Jumps, 5000),
            TitleFragment.Threshold("portal_worn", TitleSlot.Epithet, TitleCategory.Misc,
                Rarity.Uncommon, "Portal-Worn|Rift-Stepping|Gate-Hopping", PlayerStatType.PortalsUsed, 500),

            // Tracked because Valheim tracks it. Nobody sets out to earn this one.
            TitleFragment.Threshold("door_minded", TitleSlot.Epithet, TitleCategory.Misc,
                Rarity.Rare, "Door-Minded|Draught-Hating", PlayerStatType.DoorsClosed, 500),

            // Hitting Munin. He does not appreciate it.
            TitleFragment.Threshold("raven_striking", TitleSlot.Epithet, TitleCategory.Misc,
                Rarity.Epic, "Raven-Striking|Munin-Swatting", PlayerStatType.RavenHits, 1),

            TitleFragment.Threshold("trophy_proud", TitleSlot.Epithet, TitleCategory.Misc,
                Rarity.Uncommon, "Trophy-Proud|Head-Hanging", PlayerStatType.ItemStandUses, 30),
            TitleFragment.Threshold("armor_proud", TitleSlot.Epithet, TitleCategory.Misc,
                Rarity.Uncommon, "Armor-Proud|Mail-Displaying", PlayerStatType.ArmorStandUses, 20),
            TitleFragment.Threshold("tidy_handed", TitleSlot.Epithet, TitleCategory.Misc,
                Rarity.Uncommon, "Tidy-Handed|Chest-Sorting", PlayerStatType.PlaceStacks, 300),

            TitleFragment.Threshold("skald", TitleSlot.Noun, TitleCategory.Misc,
                Rarity.Uncommon, "Skald|Saga-Teller|Rune-Reader", PlayerStatType.RavenTalk, 10),
            TitleFragment.Threshold("hoarder", TitleSlot.Noun, TitleCategory.Misc,
                Rarity.Rare, "Hoarder|Packrat|Magpie", PlayerStatType.ItemsPickedUp, 10000),
            TitleFragment.Threshold("sleeper", TitleSlot.Noun, TitleCategory.Misc,
                Rarity.Rare, "Sleeper|Dreamer|Slugabed", PlayerStatType.Sleep, 500),
            TitleFragment.Threshold("elder", TitleSlot.Noun, TitleCategory.Misc,
                Rarity.Epic, "Elder|Greybeard|Old-Timer", PlayerStatType.ConsecutiveDaysSurvivedMax, 150),
            TitleFragment.Threshold("trophy_keeper", TitleSlot.Noun, TitleCategory.Misc,
                Rarity.Rare, "Trophy-Keeper|Head-Collector", PlayerStatType.ItemStandUses, 80),

            TitleFragment.Threshold("long_night", TitleSlot.Domain, TitleCategory.Misc,
                Rarity.Uncommon, "Long Night|Dark Hours", PlayerStatType.Sleep, 150),
            TitleFragment.Threshold("old_ways", TitleSlot.Domain, TitleCategory.Misc,
                Rarity.Rare, "Old Ways|Old Gods", PlayerStatType.ConsecutiveDaysSurvivedMax, 50),
            TitleFragment.Threshold("trophy_wall", TitleSlot.Domain, TitleCategory.Misc,
                Rarity.Rare, "Trophy Wall|Mounted Heads", PlayerStatType.ItemStandUses, 50),
        };
    }
}
