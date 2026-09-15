using System;
using System.Collections.Generic;
using System.Linq;
using Byname.Stats;

namespace Byname.Titles
{
    /// <summary>Which position in a template a fragment can fill.</summary>
    internal enum TitleSlot
    {
        /// <summary>A describing word: "Sodden", "Unflinching".</summary>
        Epithet,

        /// <summary>A role noun: "Beekeeper", "Bane", "Wanderer".</summary>
        Noun,

        /// <summary>The object of a connector: "of the <i>Deep</i>".</summary>
        Domain,
    }

    /// <summary>
    /// The admin-facing grouping. One switch per value in the Categories config section,
    /// and no two slots of a single title may come from the same category, which is what
    /// stops "Slaying Slayer" from ever being generated.
    /// </summary>
    internal enum TitleCategory
    {
        Combat,
        Bosses,
        Building,
        Death,
        Harvest,
        Travel,
        Exploration,
        Taming,
        Fishing,
        Cooking,
        Crafting,
        Misc,
    }

    /// <summary>
    /// How hard a fragment is to earn. Drives both selection (the rarest qualifying
    /// fragment wins its slot) and, optionally, the colour it renders in.
    /// </summary>
    internal enum Rarity
    {
        /// <summary>Earned by existing at all. Exists so a new character is never blank.</summary>
        Common = 1,
        Uncommon = 2,
        Rare = 3,
        Epic = 4,

        /// <summary>Most players will never see this.</summary>
        Legendary = 5,
    }

    /// <summary>
    /// One word, and the condition that earns it.
    ///
    /// <see cref="Reads"/> is not decoration: in World scope the baseline snapshot only
    /// stores stats some fragment actually reads, which is what keeps it small enough to
    /// live in the character save rather than a side file.
    /// </summary>
    internal sealed class TitleFragment
    {
        internal string Id { get; }
        internal TitleSlot Slot { get; }
        internal TitleCategory Category { get; }
        internal Rarity Rarity { get; }
        internal string Text { get; }

        /// <summary>Stats this fragment consults, so World scope knows what to baseline.</summary>
        internal IReadOnlyList<PlayerStatType> Reads { get; }

        /// <summary>
        /// True for the unconditional words that exist only so a fresh character is never
        /// blank. They are dropped from any slot that has a real candidate, so a dedicated
        /// builder is never called a Stranger just because the constraint solver preferred
        /// a different category.
        /// </summary>
        internal bool IsFallback { get; }

        private readonly Func<IStatSource, bool> _qualifies;

        /// <summary>
        /// How close the player is to earning this, 0 to 1, for near-miss diagnostics.
        /// Null when the fragment's condition has no meaningful notion of progress.
        /// </summary>
        private readonly Func<IStatSource, float> _progress;

        internal TitleFragment(
            string id,
            TitleSlot slot,
            TitleCategory category,
            Rarity rarity,
            string text,
            Func<IStatSource, bool> qualifies,
            IReadOnlyList<PlayerStatType> reads,
            Func<IStatSource, float> progress = null,
            bool isFallback = false)
        {
            Id = id;
            Slot = slot;
            Category = category;
            Rarity = rarity;
            Text = text;
            _qualifies = qualifies;
            Reads = reads ?? Array.Empty<PlayerStatType>();
            _progress = progress;
            IsFallback = isFallback;
        }

        internal bool Qualifies(IStatSource stats)
        {
            try
            {
                return _qualifies(stats);
            }
            catch (Exception e)
            {
                // A single malformed predicate must not take the whole title down.
                BynamePlugin.LogError($"fragment '{Id}' threw while evaluating: {e.Message}");
                return false;
            }
        }

        internal float Progress(IStatSource stats)
        {
            if (_progress == null) return 0f;
            try
            {
                return Math.Max(0f, Math.Min(1f, _progress(stats)));
            }
            catch
            {
                return 0f;
            }
        }

        /// <summary>
        /// The common shape: one stat at or above a threshold. Progress falls out of the
        /// same two numbers, so near-miss reporting costs nothing at the call site.
        /// </summary>
        internal static TitleFragment Threshold(
            string id, TitleSlot slot, TitleCategory category, Rarity rarity, string text,
            PlayerStatType stat, float threshold)
        {
            var reads = new[] { stat };
            return new TitleFragment(
                id, slot, category, rarity, text,
                s => s.Get(stat) >= threshold,
                reads,
                s => threshold <= 0f ? 1f : s.Get(stat) / threshold);
        }

        /// <summary>
        /// Kills of one named creature.
        ///
        /// Reads is deliberately empty: per-creature tallies are not part of the World
        /// scope baseline (see WorldStatSource), so there is nothing here to snapshot.
        /// The token is a localisation key taken from the game's own localisation data —
        /// never invented, because a wrong key is a fragment that never fires and says
        /// nothing about it in the log.
        /// </summary>
        internal static TitleFragment Enemy(
            string id, TitleSlot slot, Rarity rarity, string text,
            string enemyToken, float threshold)
            => new TitleFragment(
                id, slot, TitleCategory.Combat, rarity, text,
                s => s.GetEnemyKills(enemyToken) >= threshold,
                Array.Empty<PlayerStatType>(),
                s => threshold <= 0f ? 1f : s.GetEnemyKills(enemyToken) / threshold);

        /// <summary>A fragment with no condition, used to guarantee a fresh character is titled.</summary>
        internal static TitleFragment Always(
            string id, TitleSlot slot, TitleCategory category, string text)
            => new TitleFragment(
                id, slot, category, Rarity.Common, text,
                _ => true, Array.Empty<PlayerStatType>(), _ => 1f, isFallback: true);
    }

    /// <summary>
    /// A pattern with slots to fill. Connector words live in the pattern rather than in
    /// the fragments, so "of the" is written once instead of being baked into every noun.
    /// </summary>
    internal sealed class TitleTemplate
    {
        internal string Pattern { get; }

        /// <summary>Distinct slots this pattern needs filled, in first-appearance order.</summary>
        internal IReadOnlyList<TitleSlot> Slots { get; }

        /// <summary>
        /// Nudges selection between templates that are otherwise tied. Slightly favours
        /// the more elaborate patterns so they are not permanently crowded out by
        /// two-word ones, which score the same but read as plainer.
        /// </summary>
        internal int Flourish { get; }

        internal TitleTemplate(string pattern, int flourish = 0)
        {
            Pattern = pattern;
            Flourish = flourish;
            Slots = ParseSlots(pattern);
        }

        internal string Render(IReadOnlyDictionary<TitleSlot, TitleFragment> chosen)
        {
            var text = Pattern;
            foreach (var pair in chosen)
            {
                text = text.Replace(Token(pair.Key), pair.Value.Text);
            }
            return Capitalize(text);
        }

        /// <summary>
        /// Uppercases the first letter only.
        ///
        /// Fragment text is already capitalised, but several patterns open with a
        /// lowercase connector — "the {epithet}" would otherwise render as "the Unproven"
        /// where it should read "The Unproven". Applied here rather than at a call site so
        /// every surface that renders a title gets it: nameplate, toast, chat and the
        /// character panel alike. Deliberately only the first character, so the "the" in
        /// "Drowned of the Green Hall" stays lowercase where it belongs.
        /// </summary>
        private static string Capitalize(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            if (!char.IsLetter(text[0]) || char.IsUpper(text[0])) return text;
            return char.ToUpperInvariant(text[0]) + text.Substring(1);
        }

        private static string Token(TitleSlot slot) => "{" + slot.ToString().ToLowerInvariant() + "}";

        private static IReadOnlyList<TitleSlot> ParseSlots(string pattern)
        {
            var found = new List<TitleSlot>();
            foreach (TitleSlot slot in Enum.GetValues(typeof(TitleSlot)))
            {
                if (pattern.Contains(Token(slot))) found.Add(slot);
            }
            return found;
        }
    }

    /// <summary>The outcome of one evaluation.</summary>
    internal sealed class ComposedTitle
    {
        internal string Text { get; }
        internal Rarity Rarity { get; }
        internal IReadOnlyList<TitleFragment> Parts { get; }

        internal ComposedTitle(string text, Rarity rarity, IReadOnlyList<TitleFragment> parts)
        {
            Text = text;
            Rarity = rarity;
            Parts = parts;
        }

        internal string Describe() =>
            $"\"{Text}\" [{Rarity}] from {string.Join(" + ", Parts.Select(p => p.Id).ToArray())}";
    }
}
