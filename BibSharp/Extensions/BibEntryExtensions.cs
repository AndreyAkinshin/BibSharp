using System;
using System.Collections.Generic;
using System.Linq;
using BibSharp.Utilities;

namespace BibSharp.Extensions;

/// <summary>
/// Extension methods for <see cref="BibEntry"/> collections to improve discoverability and provide convenient access to utility methods.
/// </summary>
/// <remarks>
/// <para><b>Null Handling (Design Decision):</b></para>
/// <para>These extension methods explicitly check for null arguments and throw <see cref="ArgumentNullException"/> 
/// when called with null values. This differs from typical .NET extension method behavior (which would throw 
/// <see cref="NullReferenceException"/>) but provides several benefits:</para>
/// <list type="bullet">
/// <item><description><b>Clearer diagnostics:</b> The exception message includes the parameter name, making debugging easier.</description></item>
/// <item><description><b>Consistent with instance methods:</b> Matches the null-checking behavior of regular instance methods.</description></item>
/// <item><description><b>Better IDE support:</b> Allows static analysis tools to detect null-reference issues more reliably.</description></item>
/// </list>
/// <para>Example: <c>entries.FindDuplicates()</c> where <c>entries</c> is null will throw 
/// <see cref="ArgumentNullException"/> with the parameter name "entries" rather than a generic <see cref="NullReferenceException"/>.</para>
/// </remarks>
public static class BibEntryExtensions
{
    /// <summary>
    /// Finds duplicate entries within a collection based on DOI, title/author/year matching, and other criteria.
    /// </summary>
    /// <param name="entries">The collection of entries to search for duplicates.</param>
    /// <returns>
    /// A collection of groups where each group contains entries that are considered duplicates of each other.
    /// If no duplicates are found, returns an empty collection.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when entries is null.</exception>
    /// <example>
    /// <code>
    /// var entries = parser.ParseFile("references.bib");
    /// var duplicates = entries.FindDuplicates();
    /// foreach (var group in duplicates)
    /// {
    ///     Console.WriteLine($"Found {group.Count()} duplicates:");
    ///     foreach (var entry in group)
    ///     {
    ///         Console.WriteLine($"  - {entry.Key}");
    ///     }
    /// }
    /// </code>
    /// </example>
    public static IEnumerable<IEnumerable<BibEntry>> FindDuplicates(this IEnumerable<BibEntry> entries)
    {
        if (entries == null)
        {
            throw new ArgumentNullException(nameof(entries));
        }

        return BibEntrySearch.FindDuplicates(entries);
    }

    /// <summary>
    /// Finds a matching entry in a collection based on DOI, title/author/year similarity, and other criteria.
    /// </summary>
    /// <param name="targetEntry">The entry to find matches for.</param>
    /// <param name="candidates">The collection of candidate entries to search within.</param>
    /// <returns>The first matching entry found, or null if no match is found.</returns>
    /// <exception cref="ArgumentNullException">Thrown when targetEntry or candidates is null.</exception>
    /// <example>
    /// <code>
    /// var entry = parser.ParseFile("new.bib").First();
    /// var existingEntries = parser.ParseFile("library.bib");
    /// var match = entry.FindMatch(existingEntries);
    /// if (match != null)
    /// {
    ///     Console.WriteLine($"Found duplicate: {match.Key}");
    /// }
    /// </code>
    /// </example>
    public static BibEntry? FindMatch(this BibEntry targetEntry, IEnumerable<BibEntry> candidates)
    {
        if (targetEntry == null)
        {
            throw new ArgumentNullException(nameof(targetEntry));
        }

        if (candidates == null)
        {
            throw new ArgumentNullException(nameof(candidates));
        }

        var matcher = new BibEntryMatcher();
        return matcher.FindMatch(targetEntry, candidates);
    }

    /// <summary>
    /// Sorts entries by author (first author's last name) in ascending order.
    /// </summary>
    /// <param name="entries">The collection of entries to sort.</param>
    /// <param name="descending">If true, sorts in descending order; otherwise, ascending order.</param>
    /// <returns>A sorted collection of entries.</returns>
    /// <exception cref="ArgumentNullException">Thrown when entries is null.</exception>
    /// <example>
    /// <code>
    /// var entries = parser.ParseFile("references.bib");
    /// var sorted = entries.SortByAuthor();
    /// </code>
    /// </example>
    public static IEnumerable<BibEntry> SortByAuthor(this IEnumerable<BibEntry> entries, bool descending = false)
    {
        if (entries == null)
        {
            throw new ArgumentNullException(nameof(entries));
        }

        return descending
            ? entries.OrderByDescending(e => e, BibComparer.ByAuthor())
            : entries.OrderBy(e => e, BibComparer.ByAuthor());
    }

    /// <summary>
    /// Sorts entries by publication year.
    /// </summary>
    /// <param name="entries">The collection of entries to sort.</param>
    /// <param name="descending">If true, sorts in descending order (newest first); otherwise, ascending order (oldest first).</param>
    /// <returns>A sorted collection of entries.</returns>
    /// <exception cref="ArgumentNullException">Thrown when entries is null.</exception>
    /// <example>
    /// <code>
    /// var entries = parser.ParseFile("references.bib");
    /// var sorted = entries.SortByYear(descending: true); // Newest first
    /// </code>
    /// </example>
    public static IEnumerable<BibEntry> SortByYear(this IEnumerable<BibEntry> entries, bool descending = false)
    {
        if (entries == null)
        {
            throw new ArgumentNullException(nameof(entries));
        }

        return entries.OrderBy(e => e, BibComparer.ByYear(descending));
    }

    /// <summary>
    /// Sorts entries by title in ascending order.
    /// </summary>
    /// <param name="entries">The collection of entries to sort.</param>
    /// <param name="descending">If true, sorts in descending order; otherwise, ascending order.</param>
    /// <returns>A sorted collection of entries.</returns>
    /// <exception cref="ArgumentNullException">Thrown when entries is null.</exception>
    /// <example>
    /// <code>
    /// var entries = parser.ParseFile("references.bib");
    /// var sorted = entries.SortByTitle();
    /// </code>
    /// </example>
    public static IEnumerable<BibEntry> SortByTitle(this IEnumerable<BibEntry> entries, bool descending = false)
    {
        if (entries == null)
        {
            throw new ArgumentNullException(nameof(entries));
        }

        return descending
            ? entries.OrderByDescending(e => e, BibComparer.ByTitle())
            : entries.OrderBy(e => e, BibComparer.ByTitle());
    }

    /// <summary>
    /// Regenerates citation keys for all entries in the collection.
    /// </summary>
    /// <param name="entries">The collection of entries to regenerate keys for.</param>
    /// <param name="format">The format to use for key generation.</param>
    /// <param name="preserveExistingKeys">If true, preserves keys that are already set; otherwise, regenerates all keys.</param>
    /// <returns>A dictionary mapping entries to their generated keys.</returns>
    /// <exception cref="ArgumentNullException">Thrown when entries is null.</exception>
    /// <example>
    /// <code>
    /// var entries = parser.ParseFile("references.bib");
    /// var keyMap = entries.RegenerateKeys(BibKeyGenerator.KeyFormat.AuthorYear);
    /// </code>
    /// </example>
    public static Dictionary<BibEntry, string> RegenerateKeys(
        this IEnumerable<BibEntry> entries,
        BibKeyGenerator.KeyFormat format = BibKeyGenerator.KeyFormat.AuthorYear,
        bool preserveExistingKeys = true)
    {
        if (entries == null)
        {
            throw new ArgumentNullException(nameof(entries));
        }

        return BibEntry.RegenerateKeys(entries, format, preserveExistingKeys);
    }
}

