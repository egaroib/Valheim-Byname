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
        /// How far below the best score a fragment may be and still be in the running.
        ///
        /// Selecting the single best fragment made both the epoch and the player seed
        /// inert: one clear winner per slot meant the tie-break never ran, so a staleness
        /// reroll returned the same title and two players with similar deeds got identical
        /// names. A pool one rarity tier deep restores the variety without letting a
        /// Common word beat a Legendary one.
        /// </summary>
        private const float PoolDepth = 1f;

        /// <summary>
        /// What each filled slot must beat to be worth adding to a title.
        ///
        /// Without it, the three-slot template won whenever all three slots could be filled
        /// at all, so every title padded itself out with whatever basic word was lying
        /// around: "Hallwright of the Open Sky" when "Hallwright" was the whole story. Set
        /// between Common and Uncommon, so a Common word only ever fills a slot when there
        /// is nothing better to say.
        /// </summary>
        private const float SlotCost = 1.5f;

        /// <summary>
        /// Charged for each slot that repeats a category already in the title. Two Building
        /// words can still win for a builder who has done nothing else, but a title that
        /// covers two sides of a player beats one that says the same thing twice.
        /// </summary>
        private const float RepeatedCategoryCost = 1f;

        /// <summary>Large enough that no real title, however weak, loses to a placeholder one.</summary>
        private const float OnlyFallbacksCost = 100f;

        internal static ComposedTitle Compose(IStatSource stats, long playerId, int epoch)
        {
            var seed = Seed(playerId, epoch);

            var qualifying = Qualifying(stats);

            if (BynameConfig.VerboseLogging.Value) LogEvaluation(stats, qualifying, seed);

            var ranked = BuildPools(qualifying, seed, epoch);
            var maxLength = BynameConfig.MaxTitleLength.Value;

            // Every template is filled twice: strictly, where every slot must come from a
            // different category, and relaxed, where only the source rule holds. The
            // relaxed fill exists for specialists — a cook whose every fragment is Cooking
            // can only fill one slot strictly. Scoring decides between them, and strict
            // wins a tie.
            ComposedTitle best = null;
            var bestStrict = false;
            var bestFlourish = 0;
            var bestTie = 0;

            foreach (var template in FragmentCatalog.Templates)
            {
                foreach (var strict in new[] { true, false })
                {
                    var chosen = TryFill(template, ranked, strict, playerId);
                    if (chosen == null) continue;

                    var text = template.Render(chosen, playerId);
                    if (text.Length > maxLength) continue;

                    var parts = template.Slots.Select(sl => chosen[sl]).ToList();
                    var words = parts.Select(f => f.TextFor(playerId)).ToList();
                    var rarity = (Rarity)parts.Max(f => (int)f.Rarity);
                    var candidate = new ComposedTitle(text, rarity, parts, words, Score(parts));
                    var tie = TieBreak(template.Pattern, seed);

                    if (best == null || Beats(candidate, strict, template.Flourish, tie,
                                              best, bestStrict, bestFlourish, bestTie))
                    {
                        best = candidate;
                        bestStrict = strict;
                        bestFlourish = template.Flourish;
                        bestTie = tie;
                    }
                }
            }

            return best;
        }

        /// <summary>
        /// A title's worth: what its words score, less what each slot costs and what each
        /// repeated category costs.
        /// </summary>
        private static float Score(IReadOnlyList<TitleFragment> parts)
        {
            var repeats = parts.Count - parts.Select(f => f.Category).Distinct().Count();
            var score = parts.Sum(f => f.Score) - SlotCost * parts.Count - RepeatedCategoryCost * repeats;

            // A title of nothing but placeholders scores like a title of Common words, so
            // "The Untried" tied with — and beat — "Untried Boar-Hunter". Anything earned
            // must outrank a title that says nothing at all.
            return parts.All(f => f.IsFallback) ? score - OnlyFallbacksCost : score;
        }

        /// <summary>
        /// Higher score first. Near-equal scores fall through to strict, then the more
        /// ornate pattern, then the seed, so the choice is deterministic and a reroll can
        /// still land somewhere different.
        /// </summary>
        private static bool Beats(
            ComposedTitle a, bool aStrict, int aFlourish, int aTie,
            ComposedTitle b, bool bStrict, int bFlourish, int bTie)
        {
            const float epsilon = 0.001f;
            if (a.Score > b.Score + epsilon) return true;
            if (a.Score < b.Score - epsilon) return false;
            if (aStrict != bStrict) return aStrict;
            if (aFlourish != bFlourish) return aFlourish > bFlourish;
            return aTie < bTie;
        }

        /// <summary>
        /// Builds the candidate pool for each slot: best score first, fallbacks suppressed
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
                var earned = group.Any(f => !f.IsFallback)
                    ? group.Where(f => !f.IsFallback).ToList()
                    : group.ToList();

                // A deed shows only at its highest earned tier. Someone with 1,500 boar
                // kills still qualifies for the 60-kill word, and without this a reroll
                // could hand them "Boar-Hunter" back instead of "Boar-Reaper".
                var candidates = earned
                    .Where(f => !earned.Any(g => g.Rarity > f.Rarity && SameDeed(f, g)))
                    .ToList();

                var ordered = candidates
                    .OrderByDescending(f => f.Score)
                    .ThenBy(f => TieBreak(f.Id, seed))
                    .ToList();

                var best = ordered[0].Score;
                var pool = ordered.Where(f => f.Score >= best - PoolDepth).ToList();
                var tail = ordered.Skip(pool.Count).ToList();

                // Rotation is what makes the epoch mean something. The near-best pool
                // rotates; anything below it stays as a last resort in its original order.
                var shift = pool.Count > 0 ? ((epoch % pool.Count) + pool.Count) % pool.Count : 0;
                var rotated = pool.Skip(shift).Concat(pool.Take(shift)).ToList();

                pools[group.Key] = rotated.Concat(tail).ToList();
            }

            return pools;
        }

        /// <summary>
        /// Two fragments are tiers of one deed when they read exactly the same counters.
        /// Merely overlapping is not enough: "Undying" reads EnemyKills too, and it is not a
        /// higher tier of "Blood-Worn".
        /// </summary>
        private static bool SameDeed(TitleFragment a, TitleFragment b) =>
            a.SourceKeys.Count == b.SourceKeys.Count && !a.SourceKeys.Except(b.SourceKeys).Any();

        private static Dictionary<TitleSlot, TitleFragment> TryFill(
            TitleTemplate template,
            IReadOnlyDictionary<TitleSlot, List<TitleFragment>> ranked,
            bool strict,
            long playerId)
        {
            var chosen = new Dictionary<TitleSlot, TitleFragment>();
            var usedCategories = new HashSet<TitleCategory>();
            var usedSources = new HashSet<string>();
            var usedWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var slot in template.Slots)
            {
                if (!ranked.TryGetValue(slot, out var candidates)) return null;

                // The source rule holds even when relaxed: the category rule may be waived
                // for a specialist, but citing one counter twice is never acceptable. Nor is
                // the same word twice, which two fragments can now share as variants.
                var pick = candidates.FirstOrDefault(f =>
                    !f.SourceKeys.Any(usedSources.Contains) &&
                    !usedWords.Contains(f.TextFor(playerId)) &&
                    (!strict || !usedCategories.Contains(f.Category)));

                if (pick == null) return null;

                chosen[slot] = pick;
                usedCategories.Add(pick.Category);
                usedWords.Add(pick.TextFor(playerId));
                foreach (var key in pick.SourceKeys) usedSources.Add(key);
            }

            return chosen;
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

        private static int TieBreak(string id, int seed) => TitleFragment.StableHash(id, seed);
    }
}
