using System;
using System.Collections.Generic;
using System.Linq;
using Byname.Config;
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

        /// <summary>
        /// Every wording of this deed. Written in the catalog as "Hallwright|Roof-Raiser", so
        /// three friends who built the same hall are not all called the same thing.
        /// </summary>
        internal IReadOnlyList<string> Texts { get; }

        /// <summary>The first wording. Stable, so logs and the blocklist have one name to use.</summary>
        internal string Text => Texts[0];

        /// <summary>Stats this fragment consults, so World scope knows what to baseline.</summary>
        internal IReadOnlyList<PlayerStatType> Reads { get; }

        /// <summary>
        /// True for the unconditional words that exist only so a fresh character is never
        /// blank. They are dropped from any slot that has a real candidate, so a dedicated
        /// builder is never called a Stranger just because the constraint solver preferred
        /// a different category.
        /// </summary>
        internal bool IsFallback { get; }

        /// <summary>
        /// What this fragment draws on, as opaque keys. Two fragments sharing one may not
        /// appear in the same title: "Sodden Drowned of the Green Hall" cited death by
        /// drowning twice, which reads as padding and looks obviously silly the moment the
        /// title has to explain itself. Stronger than the category rule, which cannot see
        /// that two Death fragments read the very same counter.
        /// </summary>
        internal IReadOnlyCollection<string> SourceKeys { get; }

        private readonly Func<IStatSource, bool> _qualifies;

        /// <summary>
        /// How close the player is to earning this, 0 to 1, for near-miss diagnostics.
        /// Null when the fragment's condition has no meaningful notion of progress.
        /// </summary>
        private readonly Func<IStatSource, float> _progress;

        /// <summary>
        /// A human clause saying what earned this, with the numbers behind it, for the
        /// /byname command. Null falls back to the fragment's own name.
        /// </summary>
        private readonly Func<IStatSource, string> _describe;

        internal TitleFragment(
            string id,
            TitleSlot slot,
            TitleCategory category,
            Rarity rarity,
            string text,
            Func<IStatSource, bool> qualifies,
            IReadOnlyList<PlayerStatType> reads,
            Func<IStatSource, float> progress = null,
            bool isFallback = false,
            Func<IStatSource, string> describe = null,
            IReadOnlyCollection<string> sourceKeys = null)
        {
            Id = id;
            Slot = slot;
            Category = category;
            Rarity = rarity;
            Texts = text.Split('|').Select(t => t.Trim()).Where(t => t.Length > 0).ToArray();
            _qualifies = qualifies;
            Reads = reads ?? Array.Empty<PlayerStatType>();
            _progress = progress;
            IsFallback = isFallback;
            _describe = describe;
            SourceKeys = sourceKeys ?? Reads.Select(r => r.ToString()).ToArray();
        }

        /// <summary>
        /// The wording this player gets.
        ///
        /// Keyed on the player alone, never the epoch. A staleness reroll that only swapped
        /// "Hallwright" for "Roof-Raiser" would announce a change that says nothing new, and
        /// a player's wording of a deed is part of what makes the title theirs.
        /// </summary>
        internal string TextFor(long playerId)
        {
            if (Texts.Count == 1) return Texts[0];
            var seed = unchecked((int)(playerId ^ (playerId >> 32)));
            return Texts[StableHash(Id, seed) % Texts.Count];
        }

        /// <summary>
        /// What this fragment is worth when competing for a slot: its rarity, scaled by the
        /// admin's weight for its category. Rarity alone still decides the colour, so a
        /// down-weighted Rare hall is still drawn in Rare blue when it does win.
        /// </summary>
        internal float Score => (int)Rarity * BynameConfig.Weight(Category);

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
        /// What earned this, in a sentence a player can read. Never throws: this is called
        /// from a chat command and a broken clause must not eat the whole answer.
        /// </summary>
        internal string Describe(IStatSource stats)
        {
            if (_describe == null) return Text;
            try { return _describe(stats); }
            catch { return Text; }
        }

        /// <summary>
        /// Turns a PlayerStatType name into readable words: DeathByDrowning becomes
        /// "death by drowning". Mechanical rather than hand-written, so every fragment
        /// explains itself without anyone maintaining a parallel list of prose.
        /// </summary>
        internal static string Humanize(PlayerStatType stat)
        {
            var name = stat.ToString();

            // These are maxima over one connected cluster, not running totals, and the
            // literal split ("build cluster roof") actively misleads about that.
            if (name.StartsWith("BuildCluster"))
            {
                return "largest " + name.Substring("BuildCluster".Length).ToLowerInvariant() +
                       " cluster";
            }

            var sb = new System.Text.StringBuilder(name.Length + 8);
            for (var i = 0; i < name.Length; i++)
            {
                var c = name[i];
                if (i > 0 && char.IsUpper(c) && !char.IsUpper(name[i - 1])) sb.Append(' ');
                sb.Append(char.ToLowerInvariant(c));
            }
            return sb.ToString();
        }

        /// <summary>
        /// A threshold after the admin's per-category multiplier. Rounded up so a count of
        /// deaths never asks for 1.5 of them; zero stays zero, since "never died" must not
        /// turn into "died at most once".
        /// </summary>
        internal static float Scaled(TitleCategory category, float threshold)
        {
            if (threshold <= 0f) return threshold;
            return (float)Math.Ceiling(threshold * BynameConfig.ThresholdScale(category));
        }

        /// <summary>
        /// FNV-1a over the id, seeded. Hand-rolled because string.GetHashCode is not
        /// guaranteed stable between runs, and a title that reshuffled itself on restart
        /// would look exactly like a bug.
        /// </summary>
        internal static int StableHash(string id, int seed)
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

        internal static string Number(float v) =>
            v >= 1000f ? v.ToString("N0") : v.ToString("0.##");

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
                s => s.Get(stat) >= Scaled(category, threshold),
                reads,
                s => Progress(s.Get(stat), Scaled(category, threshold)),
                describe: s => $"{Humanize(stat)}: {Number(s.Get(stat))} " +
                               $"(needed {Number(Scaled(category, threshold))})");
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
                s => s.GetEnemyKills(enemyToken) >= Scaled(TitleCategory.Combat, threshold),
                Array.Empty<PlayerStatType>(),
                s => Progress(s.GetEnemyKills(enemyToken), Scaled(TitleCategory.Combat, threshold)),
                describe: s => $"{CreatureName(enemyToken)} kills: {Number(s.GetEnemyKills(enemyToken))} " +
                               $"(needed {Number(Scaled(TitleCategory.Combat, threshold))})",
                sourceKeys: new[] { enemyToken });

        /// <summary>
        /// Deaths to one kind of creature, summed over every token given — so a draugr and
        /// a draugr elite both count toward being killed by draugr.
        ///
        /// The game does not record this. Byname does, from the moment it is installed (see
        /// DeathLedger), which means deaths from before then are invisible here.
        /// </summary>
        internal static TitleFragment KilledBy(
            string id, TitleSlot slot, Rarity rarity, string text,
            float threshold, params string[] creatureTokens)
        {
            float Count(IStatSource s) => creatureTokens.Sum(t => s.GetDeathsBy(t));
            return new TitleFragment(
                id, slot, TitleCategory.Death, rarity, text,
                s => Count(s) >= Scaled(TitleCategory.Death, threshold),
                Array.Empty<PlayerStatType>(),
                s => Progress(Count(s), Scaled(TitleCategory.Death, threshold)),
                describe: s => $"killed by {CreatureName(creatureTokens[0])}: {Number(Count(s))} " +
                               $"(needed {Number(Scaled(TitleCategory.Death, threshold))})",
                sourceKeys: creatureTokens.Select(t => "killedby:" + t).ToArray());
        }

        /// <summary>
        /// Deaths in one biome, whatever the cause: drowning in the swamp counts as dying to
        /// the swamp. Recorded by Byname, so like <see cref="KilledBy"/> it only counts
        /// deaths since installation. <paramref name="biome"/> is a Heightmap.Biome name.
        /// </summary>
        internal static TitleFragment DiedIn(
            string id, TitleSlot slot, Rarity rarity, string text, string biome, float threshold)
            => new TitleFragment(
                id, slot, TitleCategory.Death, rarity, text,
                s => s.GetDeathsIn(biome) >= Scaled(TitleCategory.Death, threshold),
                Array.Empty<PlayerStatType>(),
                s => Progress(s.GetDeathsIn(biome), Scaled(TitleCategory.Death, threshold)),
                describe: s => $"deaths in the {BiomeName(biome)}: {Number(s.GetDeathsIn(biome))} " +
                               $"(needed {Number(Scaled(TitleCategory.Death, threshold))})",
                sourceKeys: new[] { "diedin:" + biome });

        private static float Progress(float value, float threshold) =>
            threshold <= 0f ? 1f : value / threshold;

        private static string CreatureName(string token) =>
            token.StartsWith("$enemy_")
                ? token.Substring("$enemy_".Length).Replace('_', ' ')
                : token;

        private static string BiomeName(string biome)
        {
            switch (biome)
            {
                case "BlackForest": return "black forest";
                case "AshLands": return "ashlands";
                case "DeepNorth": return "deep north";
                default: return biome.ToLowerInvariant();
            }
        }

        /// <summary>A fragment with no condition, used to guarantee a fresh character is titled.</summary>
        internal static TitleFragment Always(
            string id, TitleSlot slot, TitleCategory category, string text)
            => new TitleFragment(
                id, slot, category, Rarity.Common, text,
                _ => true, Array.Empty<PlayerStatType>(), _ => 1f, isFallback: true,
                describe: _ => "you have yet to earn anything else");
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

        /// <summary>Renders with each fragment's first wording, for tooling that has no player.</summary>
        internal string Render(IReadOnlyDictionary<TitleSlot, TitleFragment> chosen) =>
            Render(chosen, f => f.Text);

        internal string Render(IReadOnlyDictionary<TitleSlot, TitleFragment> chosen, long playerId) =>
            Render(chosen, f => f.TextFor(playerId));

        private string Render(
            IReadOnlyDictionary<TitleSlot, TitleFragment> chosen, Func<TitleFragment, string> word)
        {
            var text = Pattern;
            foreach (var pair in chosen)
            {
                text = text.Replace(Token(pair.Key), word(pair.Value));
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

        /// <summary>The wording each part rendered as, in the same order as <see cref="Parts"/>.</summary>
        internal IReadOnlyList<string> Words { get; }

        /// <summary>What the title scored in selection. Logged, so tuning weights has a number to watch.</summary>
        internal float Score { get; }

        internal ComposedTitle(
            string text, Rarity rarity, IReadOnlyList<TitleFragment> parts,
            IReadOnlyList<string> words, float score)
        {
            Text = text;
            Rarity = rarity;
            Parts = parts;
            Words = words;
            Score = score;
        }

        internal string Describe() =>
            $"\"{Text}\" [{Rarity}, score {Score:0.##}] from " +
            string.Join(" + ", Parts.Select(p => p.Id).ToArray());
    }

    /// <summary>
    /// Default per-category tuning, shared by the real config and the title-preview tool
    /// so the two cannot disagree about what "default settings" means.
    /// </summary>
    internal static class CategoryDefaults
    {
        /// <summary>
        /// Everyone on a shared server builds, chops and travels, so those deeds say the
        /// least about any one player. Deaths say the most: nobody chooses how they die.
        /// </summary>
        internal static float Weight(TitleCategory category)
        {
            switch (category)
            {
                case TitleCategory.Building: return 0.85f;
                case TitleCategory.Travel: return 0.85f;
                case TitleCategory.Harvest: return 0.9f;
                case TitleCategory.Death: return 1.2f;
                default: return 1f;
            }
        }
    }
}
