using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BibSharp.Utilities;

/// <summary>
/// Utility class for generating citation keys for BibTeX entries.
/// </summary>
public class BibKeyGenerator
{
    private static readonly Regex invalidCharsRegex = new Regex(@"[^\w\-]", RegexOptions.Compiled, TimeSpan.FromMilliseconds(100));
    private static readonly Regex latexCommandsRegex = new Regex(@"\\[a-zA-Z]+", RegexOptions.Compiled, TimeSpan.FromMilliseconds(100));
    private static readonly Regex bracesRegex = new Regex(@"[{}]", RegexOptions.Compiled, TimeSpan.FromMilliseconds(100));
    private static readonly Regex multipleDashesRegex = new Regex("-+", RegexOptions.Compiled, TimeSpan.FromMilliseconds(100));

    // Constants for key generation
    private const int defaultShortLength = 3;
    private const int defaultJournalAbbreviationLength = 3;

    /// <summary>
    /// Formats for generating citation keys.
    /// </summary>
    public enum KeyFormat
    {
        /// <summary>
        /// Format: [author][year], e.g., "smith2023"
        /// </summary>
        AuthorYear,

        /// <summary>
        /// Format: [author][title-first-word][year], e.g., "smithstudy2023"
        /// </summary>
        AuthorTitleYear,

        /// <summary>
        /// Format: [3-letter-author][3-letter-title][year], e.g., "smistud2023"
        /// </summary>
        AuthorTitleYearShort,

        /// <summary>
        /// Format: [author][journal-abbrev][year], e.g., "smithjcs2023"
        /// </summary>
        AuthorJournalYear
    }

    /// <summary>
    /// Generates a citation key for a BibEntry using the specified format.
    /// </summary>
    /// <param name="entry">The BibEntry to generate a key for.</param>
    /// <param name="format">The format to use for generating the key.</param>
    /// <param name="existingKeys">A collection of existing keys to avoid duplicates.</param>
    /// <returns>A generated citation key.</returns>
    public static string GenerateKey(BibEntry entry, KeyFormat format = KeyFormat.AuthorYear, IEnumerable<string>? existingKeys = null)
    {
        if (entry == null)
        {
            throw new ArgumentNullException(nameof(entry));
        }

        string baseKey;

        switch (format)
        {
            case KeyFormat.AuthorYear:
                baseKey = GenerateAuthorYearKey(entry);
                break;
            case KeyFormat.AuthorTitleYear:
                baseKey = GenerateAuthorTitleYearKey(entry);
                break;
            case KeyFormat.AuthorTitleYearShort:
                baseKey = GenerateAuthorTitleYearShortKey(entry);
                break;
            case KeyFormat.AuthorJournalYear:
                baseKey = GenerateAuthorJournalYearKey(entry);
                break;
            default:
                baseKey = GenerateAuthorYearKey(entry);
                break;
        }

        // If no existing keys provided, return the base key
        if (existingKeys == null)
        {
            return baseKey;
        }

        // Check for conflicts with existing keys
        var keys = existingKeys.ToHashSet(StringComparer.OrdinalIgnoreCase);

        if (!keys.Contains(baseKey))
        {
            return baseKey;
        }

        // If there's a conflict, add a suffix
        char suffix = 'a';
        string newKey = baseKey + suffix;

        while (keys.Contains(newKey) && suffix <= 'z')
        {
            suffix++;
            newKey = baseKey + suffix;
        }

        if (suffix > 'z')
        {
            // If we've gone through all letters, use a numeric suffix
            int numericSuffix = 1;
            newKey = baseKey + numericSuffix;

            while (keys.Contains(newKey))
            {
                numericSuffix++;
                newKey = baseKey + numericSuffix;
            }
        }

        return newKey;
    }

    /// <summary>
    /// Batch generates citation keys for multiple entries.
    /// </summary>
    /// <param name="entries">The collection of BibEntry objects to generate keys for.</param>
    /// <param name="format">The format to use for generating the keys.</param>
    /// <param name="preserveExistingKeys">Whether to preserve keys that are already set.</param>
    /// <returns>A dictionary mapping entries to their generated keys.</returns>
    public static Dictionary<BibEntry, string> GenerateKeys(IEnumerable<BibEntry> entries, KeyFormat format = KeyFormat.AuthorYear, bool preserveExistingKeys = true)
    {
        if (entries == null)
        {
            throw new ArgumentNullException(nameof(entries));
        }

        var results = new Dictionary<BibEntry, string>();
        var existingKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        // First pass: collect existing keys if we're preserving them
        if (preserveExistingKeys)
        {
            foreach (var entry in entries)
            {
                if (!string.IsNullOrEmpty(entry.Key))
                {
                    existingKeys.Add(entry.Key);
                    results[entry] = entry.Key;
                }
            }
        }

        // Second pass: generate keys for entries that need them
        foreach (var entry in entries)
        {
            if (!results.ContainsKey(entry))
            {
                string key = GenerateKey(entry, format, existingKeys);
                results[entry] = key;
                existingKeys.Add(key);
            }
        }

        return results;
    }

    private static string GenerateAuthorYearKey(BibEntry entry)
    {
        string authorPart = GetAuthorPart(entry);
        string yearPart = GetYearPart(entry);

        return CleanKey(authorPart + yearPart);
    }

    private static string GenerateAuthorTitleYearKey(BibEntry entry)
    {
        string authorPart = GetAuthorPart(entry);
        string titlePart = GetTitleFirstWordPart(entry);
        string yearPart = GetYearPart(entry);

        return CleanKey(authorPart + titlePart + yearPart);
    }

    private static string GenerateAuthorTitleYearShortKey(BibEntry entry)
    {
        string authorPart = GetShortAuthorPart(entry, defaultShortLength);
        string titlePart = GetShortTitlePart(entry, defaultShortLength);
        string yearPart = GetYearPart(entry);

        return CleanKey(authorPart + titlePart + yearPart);
    }

    private static string GenerateAuthorJournalYearKey(BibEntry entry)
    {
        string authorPart = GetAuthorPart(entry);
        string journalPart = GetJournalPart(entry);
        string yearPart = GetYearPart(entry);

        return CleanKey(authorPart + journalPart + yearPart);
    }

    private static string GetAuthorPart(BibEntry entry)
    {
        if (entry.Authors.Count == 0)
        {
            return "noauthor";
        }

        string lastName = entry.Authors[0].LastName ?? "noauthor";
        return lastName.ToLowerInvariant();
    }

    private static string GetShortAuthorPart(BibEntry entry, int length)
    {
        string authorPart = GetAuthorPart(entry);

        if (authorPart.Length <= length)
        {
            return authorPart;
        }

        return authorPart.Substring(0, length);
    }

    private static string GetYearPart(BibEntry entry)
    {
        return entry.Year?.ToString() ?? "nodate";
    }

    private static string GetTitleFirstWordPart(BibEntry entry)
    {
        if (string.IsNullOrEmpty(entry.Title))
        {
            return "notitle";
        }

        string title = entry.Title;

        // Remove any LaTeX commands and braces
        title = latexCommandsRegex.Replace(title, "");
        title = bracesRegex.Replace(title, "");

        // Split by spaces and take the first significant word
        var words = title.Split(new[] { ' ', '\t', '\n', '\r', '-', ':', '.', ',', ';', '!', '?' },
            StringSplitOptions.RemoveEmptyEntries);

        foreach (var word in words)
        {
            // Skip stop words
            if (IsStopWord(word))
            {
                continue;
            }

            return word.ToLowerInvariant();
        }

        // If we couldn't find a non-stop word, just use the first word
        return words.Length > 0 ? words[0].ToLowerInvariant() : "notitle";
    }

    private static string GetShortTitlePart(BibEntry entry, int length)
    {
        string titlePart = GetTitleFirstWordPart(entry);

        if (titlePart.Length <= length)
        {
            return titlePart;
        }

        return titlePart.Substring(0, length);
    }

    private static string GetJournalPart(BibEntry entry)
    {
        if (string.IsNullOrEmpty(entry.Journal))
        {
            // Try to use booktitle for conference papers
            if (!string.IsNullOrEmpty(entry.BookTitle) &&
                (entry.EntryType == EntryType.InProceedings || entry.EntryType == EntryType.Conference))
            {
                return GetAbbreviation(entry.BookTitle);
            }

            return entry.EntryType;
        }

        return GetAbbreviation(entry.Journal);
    }

    private static string GetAbbreviation(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return "";
        }

        // Remove any LaTeX commands and braces
        text = latexCommandsRegex.Replace(text, "");
        text = bracesRegex.Replace(text, "");

        var words = text.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        var abbreviation = new StringBuilder();

        foreach (var word in words)
        {
            // Skip stop words and punctuation
            if (IsStopWord(word) || string.IsNullOrWhiteSpace(word) || word.Length == 0)
            {
                continue;
            }

            // Take the first letter of each significant word
            char firstChar = word[0];
            if (char.IsLetter(firstChar))
            {
                abbreviation.Append(char.ToLowerInvariant(firstChar));
            }
        }

        // If no abbreviation could be formed, use the first few letters
        if (abbreviation.Length == 0 && text.Length > 0)
        {
            int takeLength = Math.Min(text.Length, defaultJournalAbbreviationLength);
            return text.Substring(0, takeLength).ToLowerInvariant();
        }

        return abbreviation.ToString();
    }

    private static bool IsStopWord(string word)
    {
        if (string.IsNullOrEmpty(word) || word.Length <= 1)
        {
            return true;
        }

        // Axon.Util English stop words
        var stopWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "a", "an", "the", "of", "and", "or", "to", "in", "for", "with", "on", "by", "at", "as",
            "from", "about", "into", "like", "through", "after", "over", "between", "out", "against",
            "during", "without", "before", "under", "around", "among"
        };

        return stopWords.Contains(word);
    }

    private static string CleanKey(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            return "unknownkey";
        }

        // Replace invalid characters with hyphens
        string cleanedKey = invalidCharsRegex.Replace(key, "-");

        // Remove multiple consecutive hyphens
        cleanedKey = multipleDashesRegex.Replace(cleanedKey, "-");

        // Remove leading and trailing hyphens
        cleanedKey = cleanedKey.Trim('-');

        // Ensure the key is not empty after cleaning
        return string.IsNullOrEmpty(cleanedKey) ? "unknownkey" : cleanedKey;
    }
}
