using System.Collections.Generic;
using System.Linq;

namespace BibSharp.Utilities;

/// <summary>
/// Extension methods for searching and matching BibTeX entries
/// </summary>
public static class BibEntrySearch
{
    private static readonly BibEntryMatcher matcher = new BibEntryMatcher();

    /// <summary>
    /// Finds a matching BibEntry in a collection based on multiple matching criteria
    /// </summary>
    /// <param name="entry">The entry to find matches for</param>
    /// <param name="candidates">The candidate entries to search within</param>
    /// <returns>The matching entry or null if no match is found</returns>
    public static BibEntry? FindMatch(this BibEntry entry, IEnumerable<BibEntry> candidates)
    {
        return matcher.FindMatch(entry, candidates);
    }

    /// <summary>
    /// Finds matches for multiple entries at once
    /// </summary>
    /// <param name="entries">The entries to find matches for</param>
    /// <param name="candidates">The candidate entries to search within</param>
    /// <returns>A dictionary mapping each entry to its match (or null if no match found)</returns>
    public static Dictionary<BibEntry, BibEntry?> FindMatches(this IEnumerable<BibEntry> entries, IEnumerable<BibEntry> candidates)
    {
        var result = new Dictionary<BibEntry, BibEntry?>();
        var candidateList = candidates.ToList(); // Avoid multiple enumeration

        foreach (var entry in entries)
        {
            result[entry] = entry.FindMatch(candidateList);
        }

        return result;
    }

    /// <summary>
    /// Finds duplicate entries within a collection
    /// </summary>
    /// <param name="entries">The collection to search for duplicates</param>
    /// <returns>Groups of duplicate entries</returns>
    public static IEnumerable<IGrouping<BibEntry, BibEntry>> FindDuplicates(this IEnumerable<BibEntry> entries)
    {
        var entriesList = entries.ToList();
        var groups = new List<List<BibEntry>>();

        // Process each entry
        foreach (var entry in entriesList)
        {
            bool foundGroup = false;

            // Check if this entry belongs to an existing group
            foreach (var group in groups)
            {
                if (group.Any(g => matcher.FindMatch(entry, new[] { g }) != null))
                {
                    group.Add(entry);
                    foundGroup = true;
                    break;
                }
            }

            // If no existing group, start a new one
            if (!foundGroup)
            {
                groups.Add(new List<BibEntry> { entry });
            }
        }

        // Convert to groupings and return only groups with more than one entry
        return groups.Where(g => g.Count > 1)
            .Select(g => new EntryGrouping(g[0], g));
    }

    /// <summary>
    /// Implementation of IGrouping for BibEntry duplicate groups
    /// </summary>
    private class EntryGrouping : IGrouping<BibEntry, BibEntry>
    {
        private readonly List<BibEntry> entries;

        public EntryGrouping(BibEntry key, IEnumerable<BibEntry> entries)
        {
            Key = key;
            this.entries = entries.ToList();
        }

        public BibEntry Key { get; }

        public IEnumerator<BibEntry> GetEnumerator() => entries.GetEnumerator();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
