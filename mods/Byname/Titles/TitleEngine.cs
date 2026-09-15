using System;
using System.Collections.Generic;
using System.Linq;
using Byname.Config;
using Byname.Stats;

namespace Byname.Titles
{
    /// <summary>
    /// Turns a character's deeds into a title.
    ///
    /// The selection is deterministic in (stats, playerId, epoch). That is the whole
    /// mechanism behind "don't retitle on every login": an ordinary trigger recomputes
    /// and compares, and when nothing notable has happened the result is identical and
    /// nothing fires. Only a staleness reroll advances the epoch.
    /// </summary>
    internal static class TitleEngine
    {
        /// <summary>
        /// How far below the best rarity a fragment may be and still be in the running.
        ///
        /// Selecting the single rarest fragment made both the epoch and the player seed
        /// inert: one clear winner per slot meant the tie-break never ran, so a staleness
        /// reroll returned the same title and two players with similar deeds got identical
        /// names. A pool one rarity tier deep restores the variety without letting a
        /// Common word beat a Legendary one.
        /// </summary>
        private const int PoolDepth = 1;

        internal static ComposedTitle Compose(IStatSource stats, long playerId, int epoch)
        {
            var seed = Seed(playerId, epoch);

            var qualifying = Qualifying(stats);

            if (BynameConfig.VerboseLogging.Value) LogEvaluation(stats, qualifying, seed);

            var ranked = BuildPools(qualifying, seed, epoch);
            var maxLength = BynameConfig.MaxTitleLength.Value;

            var templates = FragmentCatalog.Templates
                .OrderByDescending(t => Potential(t, ranked))
                .ThenByDescending(t => t.Flourish)
                .ThenBy(t => TieBreak(t.Pattern, seed))
                .ToList();

            // Strict insists every slot come from a different category, which is what
            // stops "Slaying Slayer of the Slain". But a single-category specialist — a
            // cook whose every fragment is Cooking — can only ever fill a one-slot
            // template strictly, and "the Ale-Warmed" is a worse title than the relaxed
            // "Feastgiver of the Long Table". So both are computed and the fuller one
            // wins, with strict taking ties.
            var strict = TryTemplates(templates, ranked, maxLength, strict: true);
            var relaxed = TryTemplates(templates, ranked, maxLength, strict: false);
            return Better(strict, relaxed);
        }

        /// <summary>
        /// Prefers the title that says more about the player: more filled slots first,
        /// then higher rarity. Strict wins ties, so the category constraint still shapes
        /// every title it can actually shape.
        /// </summary>
        private static ComposedTitle Better(ComposedTitle strict, ComposedTitle relaxed)
        {
            if (strict == null) return relaxed;
            if (relaxed == null) return strict;

            if (relaxed.Parts.Count != strict.Parts.Count)
                return relaxed.Parts.Count > strict.Parts.Count ? relaxed : strict;

            return (int)relaxed.Rarity > (int)strict.Rarity ? relaxed : strict;
        }

        private static ComposedTitle TryTemplates(
            IEnumerable<TitleTemplate> templates,
            IReadOnlyDictionary<TitleSlot, List<TitleFragment>> ranked,
            int maxLength,
            bool strict)
        {
            foreach (var template in templates)
            {
                var chosen = TryFill(template, ranked, strict);
                if (chosen == null) continue;

                var text = template.Render(chosen);
                if (text.Length > maxLength) continue;

                var rarity = (Rarity)chosen.Values.Max(f => (int)f.Rarity);
                return new ComposedTitle(text, rarity, chosen.Values.ToList());
            }

            return null;
        }

        /// <summary>
        /// Builds the candidate pool for each slot: rarest first, fallbacks suppressed
        /// whenever a real candidate exists, then rotated by the epoch so a staleness
        /// reroll genuinely lands somewhere else.
        /// </summary>
        private static Dictionary<TitleSlot, List<TitleFragment>> BuildPools(
            IEnumerable<TitleFragment> qualifying, int seed, int epoch)
        {
            var pools = new Dictionary<TitleSlot, List<TitleFragment>>();

            foreach (var group in qualifying.GroupBy(f => f.Slot))
            {
                // "Unproven" and "Stranger" exist only so a new character is never blank.
                // The moment the player has earned anything for this slot, they are out.
                var candidates = group.Any(f => !f.IsFallback)
                    ? group.Where(f => !f.IsFallback).ToList()
                    : group.ToList();

                var ordered = candidates
                    .OrderByDescending(f => (int)f.Rarity)
                    .ThenBy(f => TieBreak(f.Id, seed))
                    .ToList();

                var best = (int)ordered[0].Rarity;
                var pool = ordered.Where(f => (int)f.Rarity >= best - PoolDepth).ToList();
                var tail = ordered.Skip(pool.Count).ToList();

                // Rotation is what makes the epoch mean something. The near-best pool
                // rotates; anything below it stays as a last resort in its original order.
                var shift = pool.Count > 0 ? ((epoch % pool.Count) + pool.Count) % pool.Count : 0;
                var rotated = pool.Skip(shift).Concat(pool.Take(shift)).ToList();

                pools[group.Key] = rotated.Concat(tail).ToList();
            }

            return pools;
        }

        private static Dictionary<TitleSlot, TitleFragment> TryFill(
            TitleTemplate template,
            IReadOnlyDictionary<TitleSlot, List<TitleFragment>> ranked,
            bool strict)
        {
            var chosen = new Dictionary<TitleSlot, TitleFragment>();
            var usedCategories = new HashSet<TitleCategory>();
            var usedSources = new HashSet<string>();

            foreach (var slot in template.Slots)
            {
                if (!ranked.TryGetValue(slot, out var candidates)) return null;

                // The source rule holds even when relaxed: the category rule may be waived
                // for a specialist, but citing one counter twice is never acceptable.
                var pick = candidates.FirstOrDefault(f =>
                    !f.SourceKeys.Any(usedSources.Contains) &&
                    (!strict || !usedCategories.Contains(f.Category)));

                if (pick == null) return null;

                chosen[slot] = pick;
                usedCategories.Add(pick.Category);
                foreach (var key in pick.SourceKeys) usedSources.Add(key);
            }

            // Even relaxed, refuse to repeat the same word twice in one title.
            return chosen.Values.Select(f => f.Text).Distinct().Count() == chosen.Count
                ? chosen
                : null;
        }

        /// <summary>
        /// Upper bound on what a template could score, used only to order the attempts.
        /// Ignores the category constraint, so it can overestimate — harmless, because a
        /// template that then fails to fill is simply skipped.
        /// </summary>
        private static int Potential(
            TitleTemplate template, IReadOnlyDictionary<TitleSlot, List<TitleFragment>> ranked)
        {
            var total = 0;
            foreach (var slot in template.Slots)
            {
                if (!ranked.TryGetValue(slot, out var candidates) || candidates.Count == 0) return -1;
                total += (int)candidates.Max(f => (int)f.Rarity);
            }
            return total;
        }

        /// <summary>Every fragment this character currently earns, after admin filtering.</summary>
        internal static List<TitleFragment> Qualifying(IStatSource stats) =>
            FragmentCatalog.Fragments
                .Where(f => BynameConfig.CategoryEnabled(f.Category))
                .Where(f => !BynameConfig.IsBlocked(f.Id))
                .Where(f => f.Qualifies(stats))
                .ToList();

        /// <summary>
        /// What the player is closest to earning but has not, most nearly earned first.
        ///
        /// Diagnostic only, and deliberately not surfaced to players: /byname explains the
        /// title someone already has rather than dangling the next one. This exists so a
        /// badly calibrated threshold shows up in the log as "80% of the way there".
        /// </summary>
        internal static List<KeyValuePair<TitleFragment, float>> NearMisses(
            IStatSource stats, int count, float floor = 0.25f)
        {
            var earned = new HashSet<string>(Qualifying(stats).Select(f => f.Id));

            return FragmentCatalog.Fragments
                .Where(f => !earned.Contains(f.Id) && !f.IsFallback)
                .Where(f => BynameConfig.CategoryEnabled(f.Category) && !BynameConfig.IsBlocked(f.Id))
                .Select(f => new KeyValuePair<TitleFragment, float>(f, f.Progress(stats)))
                .Where(x => x.Value > floor && x.Value < 1f)
                .OrderByDescending(x => x.Value)
                .Take(count)
                .ToList();
        }

        private static void LogEvaluation(IStatSource stats, List<TitleFragment> qualifying, int seed)
        {
            BynamePlugin.LogVerbose(
                $"evaluating with seed {seed}: {qualifying.Count} qualifying fragment(s) " +
                $"[{string.Join(", ", qualifying.Select(f => f.Id).ToArray())}]");

            foreach (var miss in NearMisses(stats, 5, 0.5f))
            {
                BynamePlugin.LogVerbose($"  near miss: {miss.Key.Id} at {miss.Value:P0}");
            }
        }

        private static int Seed(long playerId, int epoch) =>
            unchecked((int)(playerId ^ (playerId >> 32)) * 397 + epoch);

        /// <summary>
        /// Hand-rolled so it does not depend on string.GetHashCode, which .NET does not
        /// guarantee to be stable between runs. A title that silently reshuffled itself on
        /// restart would look exactly like a bug.
        /// </summary>
        private static int TieBreak(string id, int seed)
        {
            unchecked
            {
                var hash = (uint)seed * 2166136261u;
                foreach (var c in id)
                {
                    hash ^= c;
                    hash *= 16777619u;
                }
                return (int)(hash & 0x7FFFFFFF);
            }
        }
    }
}
