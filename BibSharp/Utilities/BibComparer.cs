using System;
using System.Collections.Generic;
using System.Linq;

namespace BibSharp.Utilities;

/// <summary>
/// Provides comparison functionality for BibTeX entries.
/// </summary>
public static class BibComparer
{
    /// <summary>
    /// Creates a comparer that sorts entries by author last name.
    /// </summary>
    /// <returns>A comparer that can be used with OrderBy.</returns>
    public static IComparer<BibEntry> ByAuthor()
    {
        return Comparer<BibEntry>.Create((x, y) =>
        {
            var xAuthor = x.Authors.FirstOrDefault();
            var yAuthor = y.Authors.FirstOrDefault();

            if (xAuthor == null && yAuthor == null)
            {
                return 0;
            }

            if (xAuthor == null)
            {
                return -1;
            }

            if (yAuthor == null)
            {
                return 1;
            }

            return string.Compare(xAuthor.LastName, yAuthor.LastName, StringComparison.OrdinalIgnoreCase);
        });
    }

    /// <summary>
    /// Creates a comparer that sorts entries by year.
    /// </summary>
    /// <param name="descending">Whether to sort in descending order (newest first).</param>
    /// <returns>A comparer that can be used with OrderBy.</returns>
    public static IComparer<BibEntry> ByYear(bool descending = true)
    {
        return Comparer<BibEntry>.Create((x, y) =>
        {
            int xYear = x.Year ?? 0;
            int yYear = y.Year ?? 0;

            return descending ? yYear.CompareTo(xYear) : xYear.CompareTo(yYear);
        });
    }

    /// <summary>
    /// Creates a comparer that sorts entries by title.
    /// </summary>
    /// <returns>A comparer that can be used with OrderBy.</returns>
    public static IComparer<BibEntry> ByTitle()
    {
        return Comparer<BibEntry>.Create((x, y) =>
        {
            string xTitle = x.Title ?? string.Empty;
            string yTitle = y.Title ?? string.Empty;

            return string.Compare(xTitle, yTitle, StringComparison.OrdinalIgnoreCase);
        });
    }

    /// <summary>
    /// Creates a comparer that sorts entries by key.
    /// </summary>
    /// <returns>A comparer that can be used with OrderBy.</returns>
    public static IComparer<BibEntry> ByKey()
    {
        return Comparer<BibEntry>.Create((x, y) =>
        {
            return string.Compare(x.Key, y.Key, StringComparison.OrdinalIgnoreCase);
        });
    }

    /// <summary>
    /// Creates a comparer that sorts entries by a custom field.
    /// </summary>
    /// <param name="fieldName">The name of the field to sort by.</param>
    /// <returns>A comparer that can be used with OrderBy.</returns>
    public static IComparer<BibEntry> ByField(string fieldName)
    {
        return Comparer<BibEntry>.Create((x, y) =>
        {
            string xValue = x[fieldName] ?? string.Empty;
            string yValue = y[fieldName] ?? string.Empty;

            return string.Compare(xValue, yValue, StringComparison.OrdinalIgnoreCase);
        });
    }

    /// <summary>
    /// Creates a comparer that combines multiple comparers in sequence.
    /// </summary>
    /// <param name="comparers">The sequence of comparers to use.</param>
    /// <returns>A comparer that can be used with OrderBy.</returns>
    public static IComparer<BibEntry> Chain(params IComparer<BibEntry>[] comparers)
    {
        return Comparer<BibEntry>.Create((x, y) =>
        {
            foreach (var comparer in comparers)
            {
                int result = comparer.Compare(x, y);
                if (result != 0)
                {
                    return result;
                }
            }

            return 0;
        });
    }
}
