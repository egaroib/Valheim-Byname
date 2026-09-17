using System;
using System.Collections.Generic;
using System.Linq;
using Byname.Stats;
using Byname.Titles;

namespace TitlePreview
{
    internal sealed class FakeStats : IStatSource
    {
        private readonly Dictionary<PlayerStatType, float> _v;
        private readonly KillModifiers _style;
        private readonly Dictionary<string, float> _kills;
        private readonly Dictionary<string, float> _deathsBy;
        private readonly Dictionary<string, float> _deathsIn;

        internal FakeStats(Dictionary<PlayerStatType, float> v,
                           KillModifiers style = KillModifiers.MixedAndTotal,
                           Dictionary<string, float> kills = null,
                           Dictionary<string, float> deathsBy = null,
                           Dictionary<string, float> deathsIn = null)
        {
            _v = v;
            _style = style;
            _kills = kills ?? new Dictionary<string, float>();
            _deathsBy = deathsBy ?? new Dictionary<string, float>();
            _deathsIn = deathsIn ?? new Dictionary<string, float>();
        }

        public float Get(PlayerStatType s) => _v.TryGetValue(s, out var x) ? x : 0f;
        public float GetEnemyKills(string n, KillModifiers m = KillModifiers.MixedAndTotal)
            => _kills.TryGetValue(n, out var x) ? x : 0f;
        public string TopEnemy()
            => _kills.Count == 0 ? null : _kills.OrderByDescending(p => p.Value).First().Key;
        public KillModifiers DominantKillStyle(float minimumShare = 0.6f) => _style;
        public float GetDeathsBy(string t) => _deathsBy.TryGetValue(t, out var x) ? x : 0f;
        public float GetDeathsIn(string b) => _deathsIn.TryGetValue(b, out var x) ? x : 0f;
    }

    internal static class Program
    {
        private static Dictionary<PlayerStatType, float> S(params (PlayerStatType, float)[] pairs)
            => pairs.ToDictionary(p => p.Item1, p => p.Item2);

        private static void Main(string[] args)
        {
            if (args.Any(a => a.Equals("coverage", StringComparison.OrdinalIgnoreCase)))
            {
                Coverage();
                return;
            }

            if (args.Any(a => a.Equals("combinations", StringComparison.OrdinalIgnoreCase)))
            {
                Combinations();
                return;
            }

            if (args.Any(a => a.Equals("explain", StringComparison.OrdinalIgnoreCase)))
            {
                Explain();
                return;
            }

            if (args.Any(a => a.Equals("lint", StringComparison.OrdinalIgnoreCase)))
            {
                Lint();
                return;
            }

            if (args.Any(a => a.Equals("ladders", StringComparison.OrdinalIgnoreCase)))
            {
                Ladders();
                return;
            }

            if (args.Any(a => a.Equals("server", StringComparison.OrdinalIgnoreCase)))
            {
                Server();
                return;
            }

            var archetypes = new (string Name, FakeStats Stats)[]
            {
                ("Fresh character", new FakeStats(S())),

                ("Drowns constantly, keeps bees, farms", new FakeStats(S(
                    (PlayerStatType.DeathByDrowning, 12), (PlayerStatType.Deaths, 24),
                    (PlayerStatType.BeesHarvested, 130), (PlayerStatType.HarvestCrop, 520),
                    (PlayerStatType.DistanceTraveled, 90000)))),

                ("Solo'd every boss, has never died", new FakeStats(S(
                    (PlayerStatType.Deaths, 0), (PlayerStatType.EnemyKills, 900),
                    (PlayerStatType.BossKills, 8), (PlayerStatType.BossKillSolo, 8),
                    (PlayerStatType.BossLastHits, 6), (PlayerStatType.ConsecutiveDaysSurvivedMax, 160),
                    (PlayerStatType.DistanceTraveled, 240000)), KillModifiers.Melee)),

                ("Never leaves base, builds enormous halls", new FakeStats(S(
                    (PlayerStatType.BuiltPieces, 4200), (PlayerStatType.BuildClusterFurniture, 260),
                    (PlayerStatType.BuildClusterRoof, 640), (PlayerStatType.BuildClusterLighting, 410),
                    (PlayerStatType.MaxComfort, 18), (PlayerStatType.MaxBuildingHeight, 34),
                    (PlayerStatType.EnemyKills, 150), (PlayerStatType.Deaths, 12),
                    (PlayerStatType.Sleep, 130)))),

                ("Lives on a longship, flattened by a birch once", new FakeStats(S(
                    (PlayerStatType.DistanceSail, 62000), (PlayerStatType.DistanceSailHelm, 41000),
                    (PlayerStatType.DeathByTree, 2), (PlayerStatType.Deaths, 18),
                    (PlayerStatType.ExploreWest, 9200), (PlayerStatType.DistanceTraveled, 310000)))),

                ("Punches everything to death", new FakeStats(S(
                    (PlayerStatType.EnemyKills, 1400), (PlayerStatType.Deaths, 62),
                    (PlayerStatType.HitsTakenEnemies, 5400), (PlayerStatType.DistanceRun, 80000)),
                    KillModifiers.Unarmed)),

                ("Forages, sleeps a lot, avoids trouble", new FakeStats(S(
                    (PlayerStatType.HarvestBerry, 720), (PlayerStatType.HarvestMushroom, 460),
                    (PlayerStatType.BeesHarvested, 60), (PlayerStatType.Sleep, 210),
                    (PlayerStatType.Deaths, 4), (PlayerStatType.ConsecutiveDaysSurvivedMax, 60)))),

                ("Mines and chops without pause", new FakeStats(S(
                    (PlayerStatType.Mines, 3100), (PlayerStatType.TreeChops, 4200),
                    (PlayerStatType.Tree, 900), (PlayerStatType.TreeOak, 150),
                    (PlayerStatType.DeathByStructural, 3), (PlayerStatType.Deaths, 21)))),

                ("Fishes, loses most of them", new FakeStats(S(
                    (PlayerStatType.FishCaught, 260), (PlayerStatType.FishLost, 140),
                    (PlayerStatType.FishHooked, 420), (PlayerStatType.DistanceSail, 26000),
                    (PlayerStatType.Deaths, 9)))),

                ("Cook, burns half of it", new FakeStats(S(
                    (PlayerStatType.CraftFood, 430), (PlayerStatType.CraftGrill, 380),
                    (PlayerStatType.CraftGrillBurnt, 145), (PlayerStatType.BuildClusterMeads, 74),
                    (PlayerStatType.BuildClusterFeasts, 28), (PlayerStatType.FoodEaten, 1250)))),

                ("Explorer, maps nothing", new FakeStats(S(
                    (PlayerStatType.ExploreNorth, 12000), (PlayerStatType.ExploreNorthNoMap, 6400),
                    (PlayerStatType.TreasureBuriedFound, 44), (PlayerStatType.PortalDungeonIn, 62),
                    (PlayerStatType.DistanceWalk, 180000), (PlayerStatType.Deaths, 33)))),

                ("Troll-hunter who lives in the Black Forest", new FakeStats(S(
                    (PlayerStatType.EnemyKills, 900), (PlayerStatType.Deaths, 26),
                    (PlayerStatType.TreeChops, 2000), (PlayerStatType.DistanceWalk, 60000)),
                    KillModifiers.Melee,
                    new Dictionary<string, float> {
                        { "$enemy_troll", 80 }, { "$enemy_greydwarf", 900 },
                        { "$enemy_greyling", 400 }, { "$enemy_boar", 120 } })),

                ("Sea-hunter, serpents and fish", new FakeStats(S(
                    (PlayerStatType.DistanceSail, 55000), (PlayerStatType.FishCaught, 180),
                    (PlayerStatType.FishCaughtTier3, 20), (PlayerStatType.Deaths, 14),
                    (PlayerStatType.DeathByDrowning, 6)),
                    KillModifiers.Ranged,
                    new Dictionary<string, float> {
                        { "$enemy_serpent", 22 }, { "$enemy_neck", 300 } })),

                ("Thousand-hour veteran, dies constantly", new FakeStats(S(
                    (PlayerStatType.Deaths, 180), (PlayerStatType.EnemyKills, 8200),
                    (PlayerStatType.BuiltPieces, 9000), (PlayerStatType.BossKills, 8),
                    (PlayerStatType.DistanceTraveled, 900000), (PlayerStatType.Crafts, 1400),
                    (PlayerStatType.CraftsOrUpgrades, 2900), (PlayerStatType.DeathByFall, 22),
                    (PlayerStatType.ItemsPickedUp, 24000), (PlayerStatType.Sleep, 260)),
                    KillModifiers.Melee)),
            };

            Console.WriteLine($"catalog: {FragmentCatalog.Fragments.Count} fragments, " +
                              $"{FragmentCatalog.Templates.Count} templates, " +
                              $"{FragmentCatalog.TrackedStats.Count} stats referenced\n");

            Console.WriteLine("=== One player each, three different characters (playerId varies) ===\n");
            long[] ids = { 11111L, 585858L, 9090909L };
            foreach (var a in archetypes)
            {
                var titles = ids.Select(id => TitleEngine.Compose(a.Stats, id, 0))
                                .Select(t => t == null ? "<none>" : t.Text).ToArray();
                Console.WriteLine($"{a.Name}");
                Console.WriteLine($"    {string.Join("  |  ", titles)}");
            }

            Console.WriteLine("\n=== Staleness rerolls: same player, same stats, epoch advancing ===\n");
            foreach (var a in archetypes.Skip(1).Take(5))
            {
                var seq = Enumerable.Range(0, 4)
                    .Select(e => TitleEngine.Compose(a.Stats, 585858L, e))
                    .Select(t => t == null ? "<none>" : $"{t.Text} [{t.Rarity}]");
                Console.WriteLine($"{a.Name}");
                foreach (var s in seq) Console.WriteLine($"    {s}");
            }
        }

        /// <summary>
        /// What the catalog covers and what it does not. Answers "is this finished" with
        /// numbers instead of a feeling, and stays useful as the catalog grows.
        /// </summary>
        private static void Coverage()
        {
            var frags = FragmentCatalog.Fragments;

            Console.WriteLine($"{frags.Count} fragments across {FragmentCatalog.Templates.Count} templates\n");

            Console.WriteLine($"{"CATEGORY",-14}{"EPITHET",9}{"NOUN",7}{"DOMAIN",8}{"TOTAL",7}");
            foreach (TitleCategory cat in Enum.GetValues(typeof(TitleCategory)))
            {
                var inCat = frags.Where(f => f.Category == cat).ToList();
                int N(TitleSlot sl) => inCat.Count(f => f.Slot == sl);
                Console.WriteLine($"{cat,-14}{N(TitleSlot.Epithet),9}{N(TitleSlot.Noun),7}{N(TitleSlot.Domain),8}{inCat.Count,7}");
            }
            Console.WriteLine($"{"ALL",-14}{frags.Count(f => f.Slot == TitleSlot.Epithet),9}" +
                              $"{frags.Count(f => f.Slot == TitleSlot.Noun),7}" +
                              $"{frags.Count(f => f.Slot == TitleSlot.Domain),8}{frags.Count,7}");

            Console.WriteLine("\nBy rarity:");
            foreach (Rarity r in Enum.GetValues(typeof(Rarity)))
                Console.WriteLine($"  {r,-10} {frags.Count(f => f.Rarity == r)}");

            var used = new HashSet<PlayerStatType>(FragmentCatalog.TrackedStats);
            var all = Enum.GetValues(typeof(PlayerStatType)).Cast<PlayerStatType>()
                .Where(s2 => s2 != PlayerStatType.Count && s2 != PlayerStatType.None)
                .ToList();
            var unused = all.Where(s2 => !used.Contains(s2)).ToList();

            Console.WriteLine($"\nStats referenced: {used.Count} of {all.Count} " +
                              $"({100.0 * used.Count / all.Count:F0}%)");
            Console.WriteLine($"Untouched stats ({unused.Count}):");
            for (var i = 0; i < unused.Count; i += 4)
                Console.WriteLine("  " + string.Join("  ", unused.Skip(i).Take(4).Select(x => $"{x,-28}")).TrimEnd());
        }


        /// <summary>
        /// Every distinct title the grammar can render: each template crossed with every
        /// legal fill, subject to the length cap and the same-category rule. Counts what
        /// the engine CAN emit, not what any one player will see.
        /// </summary>
        private static void Combinations()
        {
            var frags = FragmentCatalog.Fragments;
            var bySlot = Enum.GetValues(typeof(TitleSlot)).Cast<TitleSlot>()
                .ToDictionary(sl => sl, sl => frags.Where(f => f.Slot == sl).ToList());

            var max = Byname.Config.BynameConfig.MaxTitleLength.Value;
            Console.WriteLine($"{frags.Count} fragments " +
                              $"({bySlot[TitleSlot.Epithet].Count} epithet, " +
                              $"{bySlot[TitleSlot.Noun].Count} noun, " +
                              $"{bySlot[TitleSlot.Domain].Count} domain), " +
                              $"MaxTitleLength {max}\n");

            long grandDistinct = 0, grandAny = 0;
            var seen = new HashSet<string>();

            Console.WriteLine($"{"TEMPLATE",-36}{"DISTINCT-CAT",14}{"ANY-CAT",11}{"TOO LONG",10}");
            foreach (var t in FragmentCatalog.Templates)
            {
                long distinct = 0, any = 0, tooLong = 0;

                foreach (var combo in Fills(t.Slots.ToList(), bySlot))
                {
                    var map = combo.ToDictionary(f => f.Slot, f => f);
                    var text = t.Render(map);
                    if (text.Length > max) { tooLong++; continue; }

                    any++;
                    seen.Add(text);
                    if (combo.Select(f => f.Category).Distinct().Count() == combo.Count) distinct++;
                }

                grandDistinct += distinct;
                grandAny += any;
                Console.WriteLine($"{t.Pattern,-36}{distinct,14:N0}{any,11:N0}{tooLong,10:N0}");
            }

            Console.WriteLine($"{"TOTAL",-36}{grandDistinct,14:N0}{grandAny,11:N0}");
            Console.WriteLine($"\ndistinct rendered strings: {seen.Count:N0}");
        }

        private static IEnumerable<List<TitleFragment>> Fills(
            List<TitleSlot> slots, Dictionary<TitleSlot, List<TitleFragment>> bySlot)
        {
            if (slots.Count == 0) { yield return new List<TitleFragment>(); yield break; }
            var head = slots[0];
            var rest = slots.Skip(1).ToList();
            foreach (var f in bySlot[head])
                foreach (var tail in Fills(rest, bySlot))
                {
                    tail.Insert(0, f);
                    yield return tail;
                }
        }


        /// <summary>
        /// Reproduces what /byname prints, minus the game-dependent lines (standing-since
        /// day, rarity colour). Exists to check that the auto-generated clauses actually
        /// read like English before they reach a player.
        /// </summary>
        private static void Explain()
        {
            var samples = new (string Name, FakeStats Stats)[]
            {
                ("Drowns constantly, keeps bees, farms", new FakeStats(S(
                    (PlayerStatType.DeathByDrowning, 12), (PlayerStatType.Deaths, 24),
                    (PlayerStatType.BeesHarvested, 130), (PlayerStatType.HarvestCrop, 520),
                    (PlayerStatType.DistanceTraveled, 90000), (PlayerStatType.DistanceWalk, 61204),
                    (PlayerStatType.BuildClusterRoof, 90)))),

                ("Punches everything to death", new FakeStats(S(
                    (PlayerStatType.EnemyKills, 1400), (PlayerStatType.Deaths, 62),
                    (PlayerStatType.HitsTakenEnemies, 5400), (PlayerStatType.DistanceRun, 80000)),
                    KillModifiers.Unarmed,
                    new Dictionary<string, float> { { "$enemy_troll", 27 }, { "$enemy_greydwarf", 800 } })),
            };

            foreach (var (name, stats) in samples)
            {
                var composed = TitleEngine.Compose(stats, 585858L, 0);
                Console.WriteLine($"--- {name} ---");
                Console.WriteLine($"You are {composed.Text}  ({composed.Rarity})");
                Console.WriteLine();
                for (var i = 0; i < composed.Parts.Count; i++)
                    Console.WriteLine($"  {composed.Words[i],-18} {composed.Parts[i].Describe(stats)}");

                // No near-miss list: /byname does not show one, and this exists to match
                // what players actually see. NearMisses stays a log-only diagnostic.
                Console.WriteLine();
            }
        }


        /// <summary>
        /// Catalog mistakes that compile fine and fail quietly: a duplicated id (the
        /// blocklist then removes two fragments), one word shared by two fragments (two
        /// deeds that read as the same thing), and a word so long it can never fit.
        /// </summary>
        private static void Lint()
        {
            var frags = FragmentCatalog.Fragments;
            var problems = 0;

            foreach (var dup in frags.GroupBy(f => f.Id).Where(g => g.Count() > 1))
            {
                Console.WriteLine($"duplicate id: {dup.Key}");
                problems++;
            }

            var words = frags.SelectMany(f => f.Texts.Select(t => (Word: t, f.Id)))
                .GroupBy(x => x.Word, StringComparer.OrdinalIgnoreCase)
                .Where(g => g.Select(x => x.Id).Distinct().Count() > 1);
            foreach (var w in words)
            {
                Console.WriteLine($"word used by several fragments: {w.Key} ({string.Join(", ", w.Select(x => x.Id))})");
                problems++;
            }

            var max = Byname.Config.BynameConfig.MaxTitleLength.Value;
            foreach (var f in frags)
            foreach (var t in f.Texts)
            {
                // "the {epithet}" is the shortest pattern any slot can appear in.
                var shortest = f.Slot == TitleSlot.Domain ? $"x of the {t}" : $"The {t}";
                if (shortest.Length > max)
                {
                    Console.WriteLine($"never fits in {max} chars: {f.Id} '{t}'");
                    problems++;
                }
            }

            var variants = frags.Sum(f => f.Texts.Count);
            Console.WriteLine($"{frags.Count} fragments, {variants} wordings, {problems} problem(s)");
        }

        /// <summary>
        /// Three friends on one server: the same boss kills, the same hall, similar
        /// distances. The case that made everyone "of the Open Sky" in 0.2.0.
        /// </summary>
        private static void Server()
        {
            var shared = new (PlayerStatType, float)[]
            {
                (PlayerStatType.BossKills, 4), (PlayerStatType.BossKillMultiplayer, 4),
                (PlayerStatType.PlayerSpawn, 40), (PlayerStatType.Sleep, 70),
            };

            FakeStats Make(Dictionary<string, float> deathsBy, Dictionary<string, float> deathsIn,
                           params (PlayerStatType, float)[] own)
                => new FakeStats(S(shared.Concat(own).ToArray()), KillModifiers.Melee, null, deathsBy, deathsIn);

            var players = new (string Name, long Id, FakeStats Stats)[]
            {
                ("Builder, dies in the swamp", 11111L, Make(
                    new Dictionary<string, float> { { "$enemy_draugr", 2 }, { "$enemy_blob", 2 } },
                    new Dictionary<string, float> { { "Swamp", 5 } },
                    (PlayerStatType.BuiltPieces, 4200), (PlayerStatType.BuildClusterFloor, 310),
                    (PlayerStatType.BuildClusterRoof, 260), (PlayerStatType.BuildClusterWall, 420),
                    (PlayerStatType.DistanceTraveled, 180000), (PlayerStatType.DistanceAir, 16000),
                    (PlayerStatType.DistanceWalk, 70000), (PlayerStatType.EnemyKills, 700),
                    (PlayerStatType.Deaths, 11), (PlayerStatType.DeathByEnemyHit, 7),
                    (PlayerStatType.DeathByPoisoned, 2), (PlayerStatType.TombstonesOpenedOwn, 10))),

                ("Builder, farms", 585858L, Make(null, null,
                    (PlayerStatType.BuiltPieces, 2600), (PlayerStatType.BuildClusterFloor, 310),
                    (PlayerStatType.BuildClusterRoof, 260), (PlayerStatType.BuildClusterWall, 420),
                    (PlayerStatType.HarvestCrop, 330), (PlayerStatType.DistanceTraveled, 140000),
                    (PlayerStatType.DistanceAir, 9000), (PlayerStatType.EnemyKills, 350),
                    (PlayerStatType.Deaths, 6), (PlayerStatType.DeathByEnemyHit, 4))),

                ("Explorer, sails", 9090909L, Make(null, null,
                    (PlayerStatType.BuiltPieces, 900), (PlayerStatType.DistanceTraveled, 320000),
                    (PlayerStatType.DistanceSail, 70000), (PlayerStatType.DistanceSailHelm, 52000),
                    (PlayerStatType.DistanceAir, 21000), (PlayerStatType.EnemyKills, 1100),
                    (PlayerStatType.TreasureBuriedFound, 12), (PlayerStatType.PortalDungeonIn, 22),
                    (PlayerStatType.Deaths, 14), (PlayerStatType.DeathByEnemyHit, 9),
                    (PlayerStatType.DeathByDrowning, 3))),
            };

            foreach (var p in players)
            {
                Console.WriteLine($"--- {p.Name} ---");
                for (var epoch = 0; epoch < 4; epoch++)
                {
                    var t = TitleEngine.Compose(p.Stats, p.Id, epoch);
                    Console.WriteLine($"  epoch {epoch}: {t.Describe()}");
                }
            }
        }

        /// <summary>
        /// Every creature's kill words as tiers, lowest first, and a player climbing one
        /// ladder — so a gap or an out-of-order rarity shows up before a player hits it.
        /// </summary>
        private static void Ladders()
        {
            var enemy = FragmentCatalog.Fragments
                .Where(f => f.Slot == TitleSlot.Noun && f.Category == TitleCategory.Combat &&
                            f.SourceKeys.Count == 1 && f.SourceKeys.First().StartsWith("$enemy_"))
                .GroupBy(f => f.SourceKeys.First());

            foreach (var g in enemy)
            {
                var rungs = g.OrderBy(f => f.Rarity).ToList();
                var ordered = rungs.Select((f, i) => i == 0 || f.Rarity > rungs[i - 1].Rarity).All(x => x);
                Console.WriteLine($"{g.Key,-24} {(ordered ? "" : "OUT OF ORDER  ")}" +
                    string.Join("  <  ", rungs.Select(f => $"{f.Text} [{f.Rarity}]")));
            }

            Console.WriteLine("\n--- one boar hunter, climbing ---");
            foreach (var kills in new[] { 59f, 60f, 400f, 600f, 1500f })
            {
                var stats = new FakeStats(S((PlayerStatType.EnemyKills, kills)), KillModifiers.MixedAndTotal,
                    new Dictionary<string, float> { { "$enemy_boar", kills } });
                var nouns = Enumerable.Range(0, 4).Select(e => TitleEngine.Compose(stats, 585858L, e).Text);
                Console.WriteLine($"{kills,6} boars: {string.Join("  |  ", nouns)}");
            }
        }
    }
}
