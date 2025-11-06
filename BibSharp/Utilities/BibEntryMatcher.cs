using System;
using System.Collections.Generic;
using System.Linq;
using BibSharp.Models;

namespace BibSharp.Utilities;

/// <summary>
/// Utility class for matching BibTeX entries
/// </summary>
public class BibEntryMatcher
{
    /// <summary>
    /// Finds a matching BibEntry in a collection based on multiple matching criteria
    /// </summary>
    /// <param name="entry">The entry to find matches for</param>
    /// <param name="candidates">The candidate entries to search within</param>
    /// <returns>The matching entry or null if no match is found</returns>
    public BibEntry? FindMatch(BibEntry entry, IEnumerable<BibEntry> candidates)
    {
        // First try to match by DOI (highest confidence)
        if (!string.IsNullOrEmpty(entry.Doi))
        {
            var doiMatch = candidates.FirstOrDefault(c =>
                !string.IsNullOrEmpty(c.Doi) &&
                NormalizeDoi(c.Doi) == NormalizeDoi(entry.Doi));

            if (doiMatch != null)
                return doiMatch;
        }

        // Then try URL matching
        if (!string.IsNullOrEmpty(entry["url"]))
        {
            string entryUrl = entry["url"]!; // Already checked above
            var urlMatch = candidates.FirstOrDefault(c =>
                !string.IsNullOrEmpty(c["url"]) &&
                NormalizeUrl(c["url"]!) == NormalizeUrl(entryUrl));

            if (urlMatch != null)
                return urlMatch;
        }

        // Then try Author + Title + Year combination
        if (entry.Authors.Count > 0 && !string.IsNullOrEmpty(entry.Title) && entry.Year.HasValue)
        {
            var combinedMatch = candidates.FirstOrDefault(c =>
                c.Authors.Count > 0 &&
                !string.IsNullOrEmpty(c.Title) &&
                c.Year.HasValue &&
                AuthorsMatch(c.Authors, entry.Authors) &&
                TitleSimilarity(c.Title, entry.Title) > 0.6 &&
                c.Year == entry.Year);

            if (combinedMatch != null)
                return combinedMatch;
        }

        // Finally, try matching by citation key (if not auto-generated)
        if (!string.IsNullOrEmpty(entry.Key) && entry.Key.Length > 3) // Ensure key is meaningful
        {
            var keyMatch = candidates.FirstOrDefault(c =>
                !string.IsNullOrEmpty(c.Key) &&
                c.Key == entry.Key);

            if (keyMatch != null)
                return keyMatch;
        }

        return null; // No match found
    }

    private string NormalizeDoi(string doi)
    {
        if (string.IsNullOrEmpty(doi))
            return string.Empty;

        // Use the shared Doi normalization method for consistency
        return Doi.NormalizeDoi(doi).ToLowerInvariant();
    }

    private string NormalizeUrl(string url)
    {
        if (string.IsNullOrEmpty(url))
            return string.Empty;

        // Normalize URLs (remove trailing slashes, etc.)
        return url.ToLowerInvariant().TrimEnd('/');
    }

    private bool AuthorsMatch(IReadOnlyList<Author> authors1, IReadOnlyList<Author> authors2)
    {
        // Simple matching: first author match
        if (authors1.Count == 0 || authors2.Count == 0)
            return false;

        // Match first author
        bool firstAuthorMatch = NormalizeAuthorName(authors1[0].LastName) ==
                                NormalizeAuthorName(authors2[0].LastName);

        // If single author for both, first author match is sufficient
        if (authors1.Count == 1 && authors2.Count == 1)
            return firstAuthorMatch;

        // If multiple authors, check if at least 60% of authors match (allowing for different ordering)
        if (authors1.Count > 1 && authors2.Count > 1 && firstAuthorMatch)
        {
            int matchCount = 0;
            foreach (var author1 in authors1)
            {
                if (authors2.Any(a2 => NormalizeAuthorName(a2.LastName) == NormalizeAuthorName(author1.LastName)))
                {
                    matchCount++;
                }
            }

            // Calculate match percentage based on the smaller author list
            double matchPercentage = (double)matchCount / Math.Min(authors1.Count, authors2.Count);
            return matchPercentage >= 0.6; // At least 60% of authors should match
        }

        return firstAuthorMatch;
    }

    private string NormalizeAuthorName(string name)
    {
        if (string.IsNullOrEmpty(name))
            return string.Empty;

        // Remove accents, spaces, and convert to lowercase
        return name.ToLowerInvariant()
            .Replace(" ", "")
            .Replace("-", "");
    }

    private double TitleSimilarity(string title1, string title2)
    {
        if (string.IsNullOrEmpty(title1) || string.IsNullOrEmpty(title2))
            return 0;

        // Normalize both titles (lowercase, remove punctuation)
        var normalized1 = NormalizeTitle(title1);
        var normalized2 = NormalizeTitle(title2);

        // If one title is contained within the other, consider them very similar
        if (normalized1.Contains(normalized2, StringComparison.Ordinal) || normalized2.Contains(normalized1, StringComparison.Ordinal))
        {
            return 0.9;
        }

        double similarity = ComputeSimilarity(normalized1, normalized2);

        // For long titles, give extra boost to the similarity if they share many words
        if (normalized1.Split(' ').Length > 3 && normalized2.Split(' ').Length > 3 && similarity > 0.5)
        {
            similarity = Math.Min(1.0, similarity + 0.1);
        }

        return similarity;
    }

    private string NormalizeTitle(string title)
    {
        if (string.IsNullOrEmpty(title))
            return string.Empty;

        return title.ToLowerInvariant()
            .Replace(":", "")
            .Replace(",", "")
            .Replace(".", "")
            .Replace("-", " ")
            .Replace("  ", " ")
            .Trim();
    }

    private double ComputeSimilarity(string s1, string s2)
    {
        if (string.IsNullOrEmpty(s1) || string.IsNullOrEmpty(s2))
            return 0;

        // Word overlap similarity measure
        var words1 = new HashSet<string>(s1.Split(' ', StringSplitOptions.RemoveEmptyEntries));
        var words2 = new HashSet<string>(s2.Split(' ', StringSplitOptions.RemoveEmptyEntries));

        if (words1.Count == 0 || words2.Count == 0)
            return 0;

        // Count intersection more efficiently without creating intermediate collection
        int intersection = 0;
        foreach (var word in words1)
        {
            if (words2.Contains(word))
            {
                intersection++;
            }
        }
        
        return (double)intersection / Math.Max(words1.Count, words2.Count);
    }
}
