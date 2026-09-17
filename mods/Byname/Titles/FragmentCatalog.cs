using System.Collections.Generic;
using System.Linq;
using Byname.Titles.Catalog;

namespace Byname.Titles
{
    /// <summary>
    /// Every word Byname can use, and every pattern it can arrange them in.
    ///
    /// The fragments live in themed files under Catalog/ so the list stays reviewable;
    /// nothing here needs to change to grow them. Connector words ("of the") belong to the
    /// templates rather than to the fragments, so "of the" is written once instead of
    /// being baked into every noun that might want it.
    /// </summary>
    internal static class FragmentCatalog
    {
        private static readonly List<TitleFragment> AllFragments =
            FateFragments.All
                .Concat(WarFragments.All)
                .Concat(WildFragments.All)
                .Concat(HearthFragments.All)
                .Concat(BeastFragments.All)
                .Concat(TierFragments.All)
                .Concat(EarlyFragments.All)
                .Concat(DoomFragments.All)
                .ToList();

        internal static IReadOnlyList<TitleFragment> Fragments => AllFragments;
        internal static IReadOnlyList<TitleTemplate> Templates => Patterns;

        /// <summary>
        /// Every stat any fragment consults. World scope baselines exactly this set, so
        /// the snapshot grows with the catalog without anyone having to maintain a second
        /// list by hand.
        /// </summary>
        internal static IReadOnlyList<PlayerStatType> TrackedStats =>
            AllFragments.SelectMany(f => f.Reads).Distinct().ToList();

        /// <summary>
        /// Ordered loosely from plain to ornate. Flourish breaks ties in favour of the
        /// more elaborate pattern, so connector forms are not permanently crowded out by
        /// the two-word one — but MaxTitleLength still has the final say, which is what
        /// keeps the long patterns rare rather than sprawling.
        ///
        /// There is deliberately no "of {domain}" form without the article: every domain
        /// in the catalog is a common noun, so "Hearthkeeper of Long Night" reads wrong
        /// where "of the Long Night" reads right. Adding article-less patterns would mean
        /// tagging each domain with whether it takes one.
        /// </summary>
        private static readonly List<TitleTemplate> Patterns = new List<TitleTemplate>
        {
            new TitleTemplate("{epithet} {noun}"),
            new TitleTemplate("the {epithet}"),
            new TitleTemplate("the {epithet} {noun}", flourish: 1),
            new TitleTemplate("{noun} of the {domain}", flourish: 2),
            new TitleTemplate("{epithet} {noun} of the {domain}", flourish: 4),
        };
    }
}
