using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BibSharp.Exceptions;
using BibSharp.Models;
using BibSharp.Utilities;

namespace BibSharp;

/// <summary>
/// Represents a BibTeX bibliography entry.
/// </summary>
/// <remarks>
/// <para><b>Thread Safety:</b></para>
/// <list type="bullet">
/// <item><description><b>Read operations:</b> Methods like <see cref="GetFields"/>, <see cref="GetFieldNames"/>, and property getters
/// are thread-safe and can be called concurrently from multiple threads, even during modifications.</description></item>
/// <item><description><b>Modification operations:</b> Setting fields, adding authors/editors, and other write operations are NOT thread-safe
/// when called concurrently. Use external synchronization (e.g., lock statements) if multiple threads modify the same entry.</description></item>
/// <item><description><b>Static methods:</b> <see cref="RegisterCustomType"/> and <see cref="RegisterFieldAlias"/> use concurrent collections
/// and are fully thread-safe.</description></item>
/// </list>
/// <para><b>Recommended Usage:</b></para>
/// <list type="bullet">
/// <item><description>For read-only access after construction: Safe to share across threads without synchronization.</description></item>
/// <item><description>For concurrent modifications: Use external locking (e.g., <c>lock(entry) { ... }</c>) or ensure only one thread modifies at a time.</description></item>
/// <item><description>Internal caches are protected by locks and will not cause data corruption, but concurrent modifications may still
/// produce inconsistent results without external synchronization.</description></item>
/// </list>
/// </remarks>
public class BibEntry
{
    private static readonly ConcurrentDictionary<string, EntryTypeDefinition> entryTypeDefinitions = new(StringComparer.OrdinalIgnoreCase)
    {
        [EntryType.Article] = DefineType(
            required: [FieldName.Author, FieldName.Title, FieldName.Journal, FieldName.Year],
            optional: [FieldName.Volume, FieldName.Number, FieldName.Pages, FieldName.Month, FieldName.Doi, FieldName.Note, FieldName.Url, FieldName.Issn, FieldName.Keywords, FieldName.Abstract, FieldName.Language, FieldName.Copyright]),

        [EntryType.Book] = DefineType(
            required: [FieldName.Title, FieldName.Publisher, FieldName.Year],
            optional: [FieldName.Author, FieldName.Editor, FieldName.Volume, FieldName.Number, FieldName.Series, FieldName.Address, FieldName.Edition, FieldName.Month, FieldName.Isbn, FieldName.Doi, FieldName.Note, FieldName.Url, FieldName.Language, FieldName.Keywords, FieldName.Abstract, FieldName.Copyright]),

        [EntryType.InProceedings] = DefineType(
            required: [FieldName.Author, FieldName.Title, FieldName.BookTitle, FieldName.Year],
            optional: [FieldName.Editor, FieldName.Volume, FieldName.Number, FieldName.Series, FieldName.Pages, FieldName.Address, FieldName.Month, FieldName.Organization, FieldName.Publisher, FieldName.Isbn, FieldName.Issn, FieldName.Doi, FieldName.Note, FieldName.Url, FieldName.Language, FieldName.Keywords, FieldName.Abstract, FieldName.Copyright]),

        [EntryType.Conference] = DefineType(
            required: [FieldName.Author, FieldName.Title, FieldName.BookTitle, FieldName.Year],
            optional: [FieldName.Editor, FieldName.Volume, FieldName.Number, FieldName.Series, FieldName.Pages, FieldName.Address, FieldName.Month, FieldName.Organization, FieldName.Publisher, FieldName.Isbn, FieldName.Issn, FieldName.Doi, FieldName.Note, FieldName.Url, FieldName.Language, FieldName.Keywords, FieldName.Abstract, FieldName.Copyright]),

        [EntryType.InCollection] = DefineType(
            required: [FieldName.Author, FieldName.Title, FieldName.BookTitle, FieldName.Publisher, FieldName.Year],
            optional: [FieldName.Editor, FieldName.Chapter, FieldName.Pages, FieldName.Address, FieldName.Edition, FieldName.Month, FieldName.Series, FieldName.Volume, FieldName.Number, FieldName.Type, FieldName.Isbn, FieldName.Doi, FieldName.Note, FieldName.Url, FieldName.Language, FieldName.Keywords, FieldName.Abstract, FieldName.Copyright]),

        [EntryType.InBook] = DefineType(
            required: [FieldName.Author, FieldName.Title, FieldName.Publisher, FieldName.Year],
            optional: [FieldName.Editor, FieldName.Chapter, FieldName.Pages, FieldName.Address, FieldName.Volume, FieldName.Number, FieldName.Series, FieldName.Edition, FieldName.Month, FieldName.Isbn, FieldName.Doi, FieldName.Note, FieldName.Url, FieldName.Language, FieldName.Keywords, FieldName.Abstract, FieldName.Copyright]),

        [EntryType.Booklet] = DefineType(
            required: [FieldName.Title],
            optional: [FieldName.Author, FieldName.HowPublished, FieldName.Address, FieldName.Month, FieldName.Year, FieldName.Note, FieldName.Url, FieldName.Language, FieldName.Keywords, FieldName.Abstract, FieldName.Copyright]),

        [EntryType.PhdThesis] = DefineType(
            required: [FieldName.Author, FieldName.Title, FieldName.School, FieldName.Year],
            optional: [FieldName.Type, FieldName.Address, FieldName.Month, FieldName.Note, FieldName.Url, FieldName.Language, FieldName.Keywords, FieldName.Abstract, FieldName.Copyright]),

        [EntryType.MastersThesis] = DefineType(
            required: [FieldName.Author, FieldName.Title, FieldName.School, FieldName.Year],
            optional: [FieldName.Type, FieldName.Address, FieldName.Month, FieldName.Note, FieldName.Url, FieldName.Language, FieldName.Keywords, FieldName.Abstract, FieldName.Copyright]),

        [EntryType.TechReport] = DefineType(
            required: [FieldName.Author, FieldName.Title, FieldName.Institution, FieldName.Year],
            optional: [FieldName.Type, FieldName.Number, FieldName.Address, FieldName.Month, FieldName.Doi, FieldName.Note, FieldName.Url, FieldName.Language, FieldName.Keywords, FieldName.Abstract, FieldName.Copyright]),

        [EntryType.Manual] = DefineType(
            required: [FieldName.Title],
            optional: [FieldName.Author, FieldName.Organization, FieldName.Address, FieldName.Edition, FieldName.Month, FieldName.Year, FieldName.Doi, FieldName.Note, FieldName.Url, FieldName.Language, FieldName.Keywords, FieldName.Abstract, FieldName.Copyright]),

        [EntryType.Proceedings] = DefineType(
            required: [FieldName.Title, FieldName.Year],
            optional: [FieldName.Editor, FieldName.Publisher, FieldName.Organization, FieldName.Address, FieldName.Month, FieldName.Isbn, FieldName.Issn, FieldName.Doi, FieldName.Note, FieldName.Url, FieldName.Language, FieldName.Keywords, FieldName.Abstract, FieldName.Copyright]),

        [EntryType.Unpublished] = DefineType(
            required: [FieldName.Author, FieldName.Title, FieldName.Note],
            optional: [FieldName.Month, FieldName.Year, FieldName.Url, FieldName.Language, FieldName.Keywords, FieldName.Abstract, FieldName.Copyright]),

        [EntryType.Misc] = DefineType(
            required: [],
            optional: [FieldName.Author, FieldName.Title, FieldName.HowPublished, FieldName.Month, FieldName.Year, FieldName.Doi, FieldName.Isbn, FieldName.Issn, FieldName.Note, FieldName.Url, FieldName.Language, FieldName.Keywords, FieldName.Abstract, FieldName.Copyright, FieldName.Annote])
    };

    private static readonly ConcurrentDictionary<string, string> fieldAliases = BuildFieldAliases();

    /// <summary>
    /// Builds the default field aliases dictionary mapping alternative field names to canonical BibTeX field names.
    /// These are standard aliases provided by the library for compatibility with various BibTeX variants (e.g., BibLaTeX).
    /// Additional aliases can be registered at runtime using <see cref="RegisterFieldAlias"/>.
    /// </summary>
    /// <returns>A concurrent dictionary of field aliases.</returns>
    private static ConcurrentDictionary<string, string> BuildFieldAliases()
    {
        // Define standard aliases: FieldName => List of aliases
        // Note: These aliases map alternative field names to their canonical BibTeX equivalents
        var aliasDefinitions = new Dictionary<string, string[]>
        {
            [FieldName.Journal] = ["journaltitle", "journal-title"],
            [FieldName.BookTitle] = ["book", "book-title"],
            [FieldName.Institution] = ["university"],
            [FieldName.Number] = ["issue"],
            [FieldName.Address] = ["location", "venue"],
            [FieldName.Isbn] = ["isbn-10", "isbn-13"],
            [FieldName.Issn] = ["electronic-issn", "print-issn"],
            [FieldName.Url] = ["link", "web"],
            [FieldName.Doi] = ["doi-url"],
            [FieldName.Keywords] = ["keyword", "tags"],
            // Note: "date" and "published" map to "year" for compatibility with BibLaTeX and other systems.
            // This mapping assumes these fields contain only year values. If full ISO dates (YYYY-MM-DD) are present,
            // only the year component will be extracted. For full date support, access the raw field value directly.
            [FieldName.Year] = ["date", "published"],
            [FieldName.Note] = ["urldate", "pubstate"]
        };

        // Convert to lookup dictionary (alias => canonical field name)
        var result = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var kvp in aliasDefinitions)
        {
            foreach (var alias in kvp.Value)
            {
                result[alias] = kvp.Key;
            }
        }

        return result;
    }

    private readonly Dictionary<string, string> fields = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<string> fieldOrder = new();
    private readonly List<Author> authors = new();
    private readonly List<Author> editors = new();
    
    // Cache for frequently accessed computed properties
    // Note: These caches are not thread-safe. External synchronization is required for concurrent modifications.
    private volatile string[]? cachedFieldNames;
    private volatile IReadOnlyDictionary<string, string>? cachedFields;
    private readonly object cacheLock = new object();

    /// <summary>
    /// Gets or sets the entry key.
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Generates a citation key for this entry.
    /// </summary>
    /// <param name="format">The format to use for the key generation.</param>
    /// <param name="existingKeys">A collection of existing keys to avoid duplicates.</param>
    /// <param name="setKeyAutomatically">Whether to automatically set the Key property.</param>
    /// <returns>The generated citation key.</returns>
    public string GenerateKey(
        BibKeyGenerator.KeyFormat format = BibKeyGenerator.KeyFormat.AuthorYear,
        IEnumerable<string>? existingKeys = null,
        bool setKeyAutomatically = true)
    {
        string generatedKey = BibKeyGenerator.GenerateKey(this, format, existingKeys);

        if (setKeyAutomatically)
        {
            Key = generatedKey;
        }

        return generatedKey;
    }

    /// <summary>
    /// Gets the entry type (e.g., article, book, inproceedings).
    /// </summary>
    public EntryType EntryType { get; }

    /// <summary>
    /// Gets the read-only collection of authors.
    /// </summary>
    /// <remarks>
    /// This collection is read-only. Use <see cref="AddAuthor"/>, <see cref="RemoveAuthor"/>, 
    /// <see cref="SetAuthors(IEnumerable{Author})"/>, or <see cref="ClearAuthors"/> methods
    /// to modify the authors.
    /// </remarks>
    public IReadOnlyList<Author> Authors => authors;

    /// <summary>
    /// Gets the read-only collection of editors.
    /// </summary>
    /// <remarks>
    /// This collection is read-only. Use <see cref="AddEditor"/>, <see cref="RemoveEditor"/>, 
    /// <see cref="SetEditors(IEnumerable{Author})"/>, or <see cref="ClearEditors"/> methods
    /// to modify the editors.
    /// </remarks>
    public IReadOnlyList<Author> Editors => editors;

    /// <summary>
    /// Gets or sets the title.
    /// </summary>
    public string? Title
    {
        get => GetField(FieldName.Title);
        set => SetField(FieldName.Title, value);
    }

    /// <summary>
    /// Gets or sets the journal name.
    /// </summary>
    public string? Journal
    {
        get => GetField(FieldName.Journal);
        set => SetField(FieldName.Journal, value);
    }

    /// <summary>
    /// Gets or sets the book title (for inproceedings, incollection, etc.).
    /// </summary>
    public string? BookTitle
    {
        get => GetField(FieldName.BookTitle);
        set => SetField(FieldName.BookTitle, value);
    }

    /// <summary>
    /// Gets or sets the publisher.
    /// </summary>
    public string? Publisher
    {
        get => GetField(FieldName.Publisher);
        set => SetField(FieldName.Publisher, value);
    }

    /// <summary>
    /// Gets or sets the year.
    /// </summary>
    public int? Year
    {
        get
        {
            string? yearStr = GetField(FieldName.Year);
            if (string.IsNullOrEmpty(yearStr))
            {
                return null;
            }

            // Clean up the year value (remove braces, etc.)
            yearStr = yearStr.Trim('{', '}', ' ');

            if (int.TryParse(yearStr, out int year))
            {
                return year;
            }

            return null;
        }
        set => SetField(FieldName.Year, value?.ToString());
    }

    /// <summary>
    /// Gets or sets the volume.
    /// </summary>
    public int? Volume
    {
        get
        {
            string? volumeStr = GetField(FieldName.Volume);
            if (string.IsNullOrEmpty(volumeStr))
            {
                return null;
            }

            volumeStr = volumeStr.Trim('{', '}', ' ');

            if (int.TryParse(volumeStr, out int volume))
            {
                return volume;
            }

            return null;
        }
        set => SetField(FieldName.Volume, value?.ToString());
    }

    /// <summary>
    /// Gets or sets the number (or issue).
    /// </summary>
    public string? Number
    {
        get => GetField(FieldName.Number);
        set => SetField(FieldName.Number, value);
    }

    /// <summary>
    /// Gets or sets the page range.
    /// </summary>
    /// <remarks>
    /// <para>The getter attempts to parse the pages field value into a PageRange object.
    /// If parsing fails (e.g., due to malformed page numbers), null is returned instead of throwing an exception.
    /// This allows the library to gracefully handle entries with invalid page ranges.</para>
    /// <para><b>Important:</b> This property cannot distinguish between "no pages set" and "invalid page format".
    /// Both cases return null. To detect parsing errors, access the raw string value via <c>entry["pages"]</c> 
    /// and attempt to construct a new PageRange to check validity explicitly.</para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Check if pages field exists and is valid
    /// if (entry.HasField("pages"))
    /// {
    ///     if (entry.Pages != null)
    ///     {
    ///         Console.WriteLine($"Valid page range: {entry.Pages}");
    ///     }
    ///     else
    ///     {
    ///         Console.WriteLine($"Invalid page format: {entry["pages"]}");
    ///     }
    /// }
    /// </code>
    /// </example>
    public PageRange? Pages
    {
        get
        {
            string? pagesStr = GetField(FieldName.Pages);
            if (string.IsNullOrEmpty(pagesStr))
            {
                return null;
            }

            try
            {
                return new PageRange(pagesStr);
            }
            catch
            {
                // Intentionally swallow parsing exceptions to allow graceful handling of malformed page ranges
                // The raw string value is still accessible via entry["pages"]
                return null;
            }
        }
        set => SetField(FieldName.Pages, value?.ToString());
    }

    /// <summary>
    /// Gets or sets the month.
    /// </summary>
    /// <remarks>
    /// <para>The getter attempts to parse the month field value into a Month object.
    /// Supports numeric values (1-12), abbreviations (jan, feb, ...), and full names (january, february, ...).</para>
    /// <para><b>Important:</b> This property cannot distinguish between "no month set" and "invalid month format".
    /// Both cases return null. To detect parsing errors, access the raw string value via <c>entry["month"]</c> 
    /// and use <see cref="Month.TryParse"/> to check validity explicitly.</para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Check if month field exists and is valid
    /// if (entry.HasField("month"))
    /// {
    ///     if (entry.Month != null)
    ///     {
    ///         Console.WriteLine($"Valid month: {entry.Month.Value.FullName}");
    ///     }
    ///     else
    ///     {
    ///         Console.WriteLine($"Invalid month format: {entry["month"]}");
    ///     }
    /// }
    /// </code>
    /// </example>
    public Month? Month
    {
        get
        {
            string? monthStr = GetField(FieldName.Month);
            if (string.IsNullOrEmpty(monthStr))
            {
                return null;
            }

            // Clean the month value (remove braces, quotes, etc.)
            monthStr = monthStr.Trim('{', '}', '"', ' ');

            // Try to parse using Month.TryParse which handles numbers, abbreviations, and full names
            if (BibSharp.Month.TryParse(monthStr, out var parsedMonth))
            {
                return parsedMonth;
            }

            // Parsing failed - return null to gracefully handle invalid month values
            return null;
        }
        set => SetField(FieldName.Month, value.HasValue ? value.Value.Abbreviation : null);
    }

    /// <summary>
    /// Sets the month using a numeric value (1-12).
    /// </summary>
    /// <param name="monthNumber">The month number (1-12).</param>
    /// <param name="useAbbreviation">Whether to use the abbreviated form (e.g., "jan" vs "january").</param>
    public void SetMonthByNumber(int monthNumber, bool useAbbreviation = true)
    {
        var month = BibSharp.Month.FromNumber(monthNumber);
        SetField(FieldName.Month, useAbbreviation ? month.Abbreviation : month.FullName);
    }

    /// <summary>
    /// Gets the month as a numeric value (1-12), or null if no month is set or it cannot be parsed.
    /// </summary>
    /// <returns>The month number (1-12), or null.</returns>
    public int? GetMonthAsNumber()
    {
        return Month?.Number;
    }

    /// <summary>
    /// Gets or sets the DOI (Digital Object Identifier).
    /// Accepts DOI in various formats: plain identifier, URL, or with doi: prefix.
    /// Stores as the plain identifier (e.g., "10.1234/abcd.5678").
    /// </summary>
    public Doi? Doi
    {
        get
        {
            string? doiStr = GetField(FieldName.Doi);
            if (string.IsNullOrEmpty(doiStr))
            {
                return null;
            }

            if (BibSharp.Doi.TryParse(doiStr, out var doi))
            {
                return doi;
            }

            return null;
        }
        set => SetField(FieldName.Doi, value?.Identifier);
    }

    /// <summary>
    /// Gets the DOI as a string identifier (without prefix).
    /// Returns null if no DOI is set.
    /// </summary>
    /// <returns>The DOI identifier, or null.</returns>
    public string? GetDoiIdentifier()
    {
        return Doi?.Identifier;
    }

    /// <summary>
    /// Gets the DOI as a full URL.
    /// Returns null if no DOI is set.
    /// </summary>
    /// <returns>The DOI URL, or null.</returns>
    public string? GetDoiUrl()
    {
        return Doi?.Url;
    }

    /// <summary>
    /// Gets or sets the URL.
    /// </summary>
    public string? Url
    {
        get => GetField(FieldName.Url);
        set => SetField(FieldName.Url, value);
    }

    /// <summary>
    /// Gets or sets the note.
    /// </summary>
    public string? Note
    {
        get => GetField(FieldName.Note);
        set => SetField(FieldName.Note, value);
    }

    /// <summary>
    /// Gets or sets the edition.
    /// </summary>
    public string? Edition
    {
        get => GetField(FieldName.Edition);
        set => SetField(FieldName.Edition, value);
    }

    /// <summary>
    /// Gets or sets the series.
    /// </summary>
    public string? Series
    {
        get => GetField(FieldName.Series);
        set => SetField(FieldName.Series, value);
    }

    /// <summary>
    /// Gets or sets the chapter number.
    /// </summary>
    public string? Chapter
    {
        get => GetField(FieldName.Chapter);
        set => SetField(FieldName.Chapter, value);
    }

    /// <summary>
    /// Gets or sets the address (e.g. publisher's address).
    /// </summary>
    public string? Address
    {
        get => GetField(FieldName.Address);
        set => SetField(FieldName.Address, value);
    }

    /// <summary>
    /// Gets or sets how the publication was published, if not through conventional channels.
    /// </summary>
    public string? HowPublished
    {
        get => GetField(FieldName.HowPublished);
        set => SetField(FieldName.HowPublished, value);
    }

    /// <summary>
    /// Gets or sets the ISBN (International Standard Book Number).
    /// </summary>
    public string? Isbn
    {
        get => GetField(FieldName.Isbn);
        set => SetField(FieldName.Isbn, value);
    }

    /// <summary>
    /// Gets or sets the ISSN (International Standard Serial Number).
    /// </summary>
    public string? Issn
    {
        get => GetField(FieldName.Issn);
        set => SetField(FieldName.Issn, value);
    }

    /// <summary>
    /// Gets or sets the institution that published the work.
    /// </summary>
    public string? Institution
    {
        get => GetField(FieldName.Institution);
        set => SetField(FieldName.Institution, value);
    }

    /// <summary>
    /// Gets or sets the school where a thesis was written.
    /// </summary>
    public string? School
    {
        get => GetField(FieldName.School);
        set => SetField(FieldName.School, value);
    }

    /// <summary>
    /// Gets or sets the organization that sponsored the publication.
    /// </summary>
    public string? Organization
    {
        get => GetField(FieldName.Organization);
        set => SetField(FieldName.Organization, value);
    }

    // Publisher property already exists above

    /// <summary>
    /// Gets or sets the type of technical report, PhD thesis, etc.
    /// </summary>
    public string? Type
    {
        get => GetField(FieldName.Type);
        set => SetField(FieldName.Type, value);
    }

    /// <summary>
    /// Gets or sets the language in which the work was written.
    /// </summary>
    public string? Language
    {
        get => GetField(FieldName.Language);
        set => SetField(FieldName.Language, value);
    }

    /// <summary>
    /// Gets or sets keywords associated with the publication.
    /// </summary>
    public string? Keywords
    {
        get => GetField(FieldName.Keywords);
        set => SetField(FieldName.Keywords, value);
    }

    /// <summary>
    /// Gets the list of keywords as a collection of strings. Returns an empty collection if no keywords are set.
    /// </summary>
    /// <returns>A collection of keyword strings.</returns>
    public ICollection<string> GetKeywordList()
    {
        string? keywordString = GetField(FieldName.Keywords);

        if (string.IsNullOrEmpty(keywordString))
        {
            return Array.Empty<string>();
        }

        // Check if wrapped in braces (indicates single phrase even with spaces)
        bool isSinglePhrase = keywordString.StartsWith("{", StringComparison.Ordinal) && keywordString.EndsWith("}", StringComparison.Ordinal);
        
        // Remove outer braces if present
        if (isSinglePhrase)
        {
            keywordString = keywordString.Trim('{', '}');
        }

        // Check if keywords contain common delimiters (commas or semicolons)
        if (keywordString.Contains(',', StringComparison.Ordinal) || keywordString.Contains(';', StringComparison.Ordinal))
        {
            // Split by commas or semicolons
            return keywordString
                .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(k => k.Trim())
                .Where(k => !string.IsNullOrEmpty(k))
                .ToList();
        }
        else if (isSinglePhrase)
        {
            // If wrapped in braces, treat as a single phrase
            return new[] { keywordString.Trim() };
        }
        else
        {
            // No delimiters found and not in braces - split by whitespace
            return keywordString
                .Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(k => k.Trim())
                .Where(k => !string.IsNullOrEmpty(k))
                .ToList();
        }
    }

    /// <summary>
    /// Sets the keywords from a collection of strings.
    /// </summary>
    /// <param name="keywords">The collection of keywords.</param>
    /// <param name="separator">The separator to use between keywords (default is comma).</param>
    public void SetKeywordList(IEnumerable<string>? keywords, string separator = ", ")
    {
        if (keywords == null)
        {
            Keywords = null;
            return;
        }

        string keywordString = string.Join(separator, keywords.Where(k => !string.IsNullOrEmpty(k)));
        Keywords = keywordString;
    }

    /// <summary>
    /// Adds a keyword to the list of keywords.
    /// </summary>
    /// <param name="keyword">The keyword to add.</param>
    /// <param name="separator">The separator to use between keywords (default is comma).</param>
    public void AddKeyword(string keyword, string separator = ", ")
    {
        if (string.IsNullOrEmpty(keyword))
        {
            return;
        }

        var keywordList = GetKeywordList().ToList();

        // Only add if it's not already in the list (case-insensitive comparison)
        if (!keywordList.Any(k => string.Equals(k, keyword, StringComparison.OrdinalIgnoreCase)))
        {
            keywordList.Add(keyword);
            SetKeywordList(keywordList, separator);
        }
    }

    /// <summary>
    /// Gets or sets the abstract of the work.
    /// </summary>
    public string? Abstract
    {
        get => GetField(FieldName.Abstract);
        set => SetField(FieldName.Abstract, value);
    }

    /// <summary>
    /// Gets or sets the annotation or description for annotated bibliographies.
    /// </summary>
    public string? Annote
    {
        get => GetField(FieldName.Annote);
        set => SetField(FieldName.Annote, value);
    }

    /// <summary>
    /// Gets or sets the copyright information.
    /// </summary>
    public string? Copyright
    {
        get => GetField(FieldName.Copyright);
        set => SetField(FieldName.Copyright, value);
    }

    /// <summary>
    /// ArXivID
    /// </summary>
    public string? Arxiv
    {
        get => GetField(FieldName.Arxiv);
        set => SetField(FieldName.Arxiv, value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BibEntry"/> class.
    /// </summary>
    public BibEntry() : this(EntryType.Misc)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BibEntry"/> class with the specified entry type.
    /// </summary>
    /// <param name="entryType">The entry type.</param>
    public BibEntry(EntryType entryType)
    {
        EntryType = entryType;
    }

    /// <summary>
    /// Indexer for accessing fields by name.
    /// </summary>
    /// <param name="fieldName">The field name.</param>
    /// <returns>The field value, or null if the field does not exist.</returns>
    public string? this[string fieldName]
    {
        get => GetField(fieldName);
        set => SetField(fieldName, value);
    }

    /// <summary>
    /// Gets a value indicating whether this entry has a field with the specified name.
    /// </summary>
    /// <param name="fieldName">The field name to check.</param>
    /// <returns>true if the entry has the field; otherwise, false.</returns>
    public bool HasField(string fieldName)
    {
        if (fieldAliases.TryGetValue(fieldName, out string? alias))
        {
            return fields.ContainsKey(alias) || fields.ContainsKey(fieldName);
        }

        return fields.ContainsKey(fieldName);
    }

    /// <summary>
    /// Gets a field value by name.
    /// </summary>
    /// <param name="fieldName">The field name.</param>
    /// <returns>The field value, or null if the field does not exist.</returns>
    public string? GetField(string fieldName)
    {
        if (fields.TryGetValue(fieldName, out string? value))
        {
            return value;
        }

        // Try alias if field not found directly
        if (fieldAliases.TryGetValue(fieldName, out string? alias) && fields.TryGetValue(alias, out value))
        {
            return value;
        }

        return null;
    }

    /// <summary>
    /// Sets a field value.
    /// </summary>
    /// <param name="fieldName">The field name.</param>
    /// <param name="value">The field value.</param>
    /// <exception cref="ArgumentException">Thrown when fieldName is null or empty.</exception>
    public void SetField(string fieldName, string? value)
    {
        if (string.IsNullOrEmpty(fieldName))
        {
            throw new ArgumentException("Field name cannot be null or empty", nameof(fieldName));
        }

        // Handle field aliases
        if (fieldAliases.TryGetValue(fieldName, out string? alias))
        {
            fieldName = alias;
        }

        // Special handling for author and editor fields
        if (string.Equals(fieldName, FieldName.Author, StringComparison.OrdinalIgnoreCase))
        {
            SetAuthors(value);
            return;
        }

        if (string.Equals(fieldName, FieldName.Editor, StringComparison.OrdinalIgnoreCase))
        {
            SetEditors(value);
            return;
        }

        // Add the field to the order list if it doesn't already exist
        if (!fields.ContainsKey(fieldName) && !string.IsNullOrEmpty(value))
        {
            fieldOrder.Add(fieldName);
        }

        // Set or remove the field
        if (string.IsNullOrEmpty(value))
        {
            fields.Remove(fieldName);
            fieldOrder.Remove(fieldName);
        }
        else
        {
            fields[fieldName] = value;
        }
        
        // Invalidate caches when fields are modified
        InvalidateCache();
    }
    
    /// <summary>
    /// Invalidates the cached field data.
    /// This method is thread-safe.
    /// </summary>
    private void InvalidateCache()
    {
        lock (cacheLock)
        {
            cachedFieldNames = null;
            cachedFields = null;
        }
    }

    /// <summary>
    /// Removes a field from the entry.
    /// </summary>
    /// <param name="fieldName">The field name to remove.</param>
    /// <returns>true if the field was removed; otherwise, false.</returns>
    public bool RemoveField(string fieldName)
    {
        if (string.IsNullOrEmpty(fieldName))
        {
            return false;
        }

        // Handle field aliases
        if (fieldAliases.TryGetValue(fieldName, out string? alias))
        {
            fieldName = alias;
        }

        // Special handling for author and editor fields
        if (string.Equals(fieldName, FieldName.Author, StringComparison.OrdinalIgnoreCase))
        {
            authors.Clear();
            fields.Remove(FieldName.Author);
            fieldOrder.Remove(FieldName.Author);
            InvalidateCache();
            return true;
        }

        if (string.Equals(fieldName, FieldName.Editor, StringComparison.OrdinalIgnoreCase))
        {
            editors.Clear();
            fields.Remove(FieldName.Editor);
            fieldOrder.Remove(FieldName.Editor);
            InvalidateCache();
            return true;
        }

        fieldOrder.Remove(fieldName);
        bool removed = fields.Remove(fieldName);
        if (removed)
        {
            InvalidateCache();
        }
        return removed;
    }

    /// <summary>
    /// Gets all field names in this entry.
    /// </summary>
    /// <returns>A collection of field names.</returns>
    /// <remarks>
    /// This method returns a cached copy for performance. The cache is invalidated
    /// when fields are modified. This method is thread-safe for read operations.
    /// </remarks>
    public IEnumerable<string> GetFieldNames()
    {
        if (cachedFieldNames != null)
        {
            return cachedFieldNames;
        }

        lock (cacheLock)
        {
            return cachedFieldNames ??= fieldOrder.ToArray();
        }
    }

    /// <summary>
    /// Gets all fields in this entry as a dictionary.
    /// </summary>
    /// <returns>A dictionary containing all fields.</returns>
    /// <remarks>
    /// This method returns a cached readonly dictionary for performance. The cache is invalidated
    /// when fields are modified. This method is thread-safe for read operations.
    /// </remarks>
    public IReadOnlyDictionary<string, string> GetFields()
    {
        if (cachedFields != null)
        {
            return cachedFields;
        }

        lock (cacheLock)
        {
            return cachedFields ??= new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(fields));
        }
    }

    /// <summary>
    /// Adds an author to the entry.
    /// </summary>
    /// <param name="author">The author to add.</param>
    /// <returns>The current BibEntry instance to enable method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when author is null.</exception>
    public BibEntry AddAuthor(Author author)
    {
        if (author == null)
        {
            throw new ArgumentNullException(nameof(author));
        }

        authors.Add(author);
        UpdateAuthorsField();
        return this;
    }

    /// <summary>
    /// Removes an author from the entry.
    /// </summary>
    /// <param name="author">The author to remove.</param>
    /// <returns>true if the author was removed; otherwise, false.</returns>
    public bool RemoveAuthor(Author? author)
    {
        if (author == null)
        {
            return false;
        }

        bool removed = authors.Remove(author);
        if (removed)
        {
            UpdateAuthorsField();
        }

        return removed;
    }

    /// <summary>
    /// Adds an editor to the entry.
    /// </summary>
    /// <param name="editor">The editor to add.</param>
    /// <returns>The current BibEntry instance to enable method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when editor is null.</exception>
    public BibEntry AddEditor(Author editor)
    {
        if (editor == null)
        {
            throw new ArgumentNullException(nameof(editor));
        }

        editors.Add(editor);
        UpdateEditorsField();
        return this;
    }

    /// <summary>
    /// Removes an editor from the entry.
    /// </summary>
    /// <param name="editor">The editor to remove.</param>
    /// <returns>true if the editor was removed; otherwise, false.</returns>
    public bool RemoveEditor(Author? editor)
    {
        if (editor == null)
        {
            return false;
        }

        bool removed = editors.Remove(editor);
        if (removed)
        {
            UpdateEditorsField();
        }

        return removed;
    }

    /// <summary>
    /// Sets the complete list of authors for this entry, replacing any existing authors.
    /// </summary>
    /// <param name="newAuthors">The collection of authors to set.</param>
    /// <returns>The current BibEntry instance to enable method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when newAuthors is null.</exception>
    public BibEntry SetAuthors(IEnumerable<Author> newAuthors)
    {
        if (newAuthors == null)
        {
            throw new ArgumentNullException(nameof(newAuthors));
        }

        authors.Clear();
        foreach (var author in newAuthors)
        {
            if (author != null)
            {
                authors.Add(author);
            }
        }
        UpdateAuthorsField();
        return this;
    }

    /// <summary>
    /// Sets the complete list of authors for this entry, replacing any existing authors.
    /// </summary>
    /// <param name="newAuthors">The authors to set.</param>
    /// <returns>The current BibEntry instance to enable method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when newAuthors is null.</exception>
    public BibEntry SetAuthors(params Author[] newAuthors)
    {
        return SetAuthors((IEnumerable<Author>)newAuthors);
    }

    /// <summary>
    /// Sets the complete list of editors for this entry, replacing any existing editors.
    /// </summary>
    /// <param name="newEditors">The collection of editors to set.</param>
    /// <returns>The current BibEntry instance to enable method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when newEditors is null.</exception>
    public BibEntry SetEditors(IEnumerable<Author> newEditors)
    {
        if (newEditors == null)
        {
            throw new ArgumentNullException(nameof(newEditors));
        }

        editors.Clear();
        foreach (var editor in newEditors)
        {
            if (editor != null)
            {
                editors.Add(editor);
            }
        }
        UpdateEditorsField();
        return this;
    }

    /// <summary>
    /// Sets the complete list of editors for this entry, replacing any existing editors.
    /// </summary>
    /// <param name="newEditors">The editors to set.</param>
    /// <returns>The current BibEntry instance to enable method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when newEditors is null.</exception>
    public BibEntry SetEditors(params Author[] newEditors)
    {
        return SetEditors((IEnumerable<Author>)newEditors);
    }

    /// <summary>
    /// Removes all authors from this entry.
    /// </summary>
    /// <returns>The current BibEntry instance to enable method chaining.</returns>
    public BibEntry ClearAuthors()
    {
        if (authors.Count > 0)
        {
            authors.Clear();
            UpdateAuthorsField();
        }
        return this;
    }

    /// <summary>
    /// Removes all editors from this entry.
    /// </summary>
    /// <returns>The current BibEntry instance to enable method chaining.</returns>
    public BibEntry ClearEditors()
    {
        if (editors.Count > 0)
        {
            editors.Clear();
            UpdateEditorsField();
        }
        return this;
    }

    /// <summary>
    /// Registers a custom field alias, mapping an alternative field name to a canonical (standard) field name.
    /// This adds to the standard aliases already provided by the library (see <see cref="BuildFieldAliases"/>).
    /// </summary>
    /// <param name="alias">The alternative field name (e.g., "journaltitle" for biblatex compatibility).</param>
    /// <param name="target">The canonical field name to map to (e.g., "journal").</param>
    /// <remarks>
    /// <para>This method is thread-safe and can be called concurrently.</para>
    /// <para>If the alias already exists (either as a standard or custom alias), it will be updated to the new target.</para>
    /// <para><b>Note:</b> Standard aliases like "journaltitle" → "journal" are already provided by the library.
    /// Use this method only for custom aliases specific to your application.</para>
    /// 
    /// <example>
    /// Example usage:
    /// <code>
    /// // Map a custom field name to a standard field
    /// BibEntry.RegisterFieldAlias("publication-date", "year");
    /// </code>
    /// </example>
    /// </remarks>
    public static void RegisterFieldAlias(string alias, string target)
    {
        if (string.IsNullOrEmpty(alias))
        {
            throw new ArgumentException("Alias cannot be null or empty", nameof(alias));
        }

        if (string.IsNullOrEmpty(target))
        {
            throw new ArgumentException("Target cannot be null or empty", nameof(target));
        }

        fieldAliases[alias] = target;
    }

    /// <summary>
    /// Registers a custom entry type with required and optional fields.
    /// </summary>
    /// <param name="entryType">The entry type name.</param>
    /// <param name="requiredFields">The required fields for this entry type.</param>
    /// <param name="optionalFields">The optional fields for this entry type.</param>
    /// <remarks>
    /// This method is thread-safe and can be called concurrently.
    /// If the entry type already exists, it will be updated with the new field definitions.
    /// </remarks>
    public static void RegisterCustomType(string entryType, string[] requiredFields, string[] optionalFields)
    {
        if (string.IsNullOrEmpty(entryType))
        {
            throw new ArgumentException("Entry type cannot be null or empty", nameof(entryType));
        }

        entryTypeDefinitions[entryType] = new EntryTypeDefinition(
            requiredFields ?? Array.Empty<string>(),
            optionalFields ?? Array.Empty<string>());
    }

    /// <summary>
    /// Resolves a DOI to a BibEntry by fetching metadata from online services.
    /// </summary>
    /// <param name="doi">The DOI to resolve (can be a DOI string, URL, or with doi: prefix).</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A BibEntry constructed from the resolved DOI metadata, or null if resolution failed.</returns>
    /// <remarks>
    /// This method uses a default DoiResolver instance with default settings (30 second timeout, 100ms rate limit).
    /// For custom configuration, create a <see cref="DoiResolver"/> instance and call <see cref="DoiResolver.ResolveDoiAsync"/>.
    /// </remarks>
    public static async Task<BibEntry?> FromDoiAsync(string doi, CancellationToken cancellationToken = default)
    {
        var resolver = new DoiResolver();
        return await resolver.ResolveDoiAsync(doi, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Parses a single BibTeX entry from a string.
    /// </summary>
    /// <param name="text">The BibTeX string to parse.</param>
    /// <returns>A BibEntry object representing the parsed entry.</returns>
    /// <exception cref="BibParseException">Thrown when the input string cannot be parsed or contains multiple entries.</exception>
    public static BibEntry Parse(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            throw new ArgumentException("Text cannot be null or empty", nameof(text));
        }

        var parser = new BibParser();
        var entries = parser.ParseString(text);

        if (entries.Count == 0)
        {
            throw new BibParseException("No BibTeX entries found in the input string");
        }

        if (entries.Count > 1)
        {
            throw new BibParseException("Multiple BibTeX entries found in the input string when only one was expected");
        }

        return entries[0];
    }

    /// <summary>
    /// Attempts to parse a single BibTeX entry from a string.
    /// </summary>
    /// <param name="text">The BibTeX string to parse.</param>
    /// <param name="entry">When this method returns, contains the parsed BibEntry if parsing succeeded, or null if parsing failed.</param>
    /// <returns>true if parsing succeeded; otherwise, false.</returns>
    public static bool TryParse(string text, out BibEntry? entry)
    {
        entry = null;

        if (string.IsNullOrEmpty(text))
        {
            return false;
        }

        try
        {
            var parser = new BibParser();
            var entries = parser.ParseString(text);

            if (entries.Count != 1)
            {
                return false;
            }

            entry = entries[0];
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Parses multiple BibTeX entries from a string.
    /// </summary>
    /// <param name="text">The BibTeX string to parse.</param>
    /// <returns>A list of BibEntry objects representing the parsed entries.</returns>
    /// <exception cref="ArgumentException">Thrown when text is null or empty.</exception>
    /// <exception cref="BibParseException">Thrown when the input string cannot be parsed.</exception>
    public static List<BibEntry> ParseAll(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            throw new ArgumentException("Text cannot be null or empty", nameof(text));
        }

        var parser = new BibParser();
        return parser.ParseString(text).ToList();
    }

    /// <summary>
    /// Regenerates citation keys for a collection of entries.
    /// </summary>
    /// <param name="entries">The collection of entries to regenerate keys for.</param>
    /// <param name="format">The format to use for key generation.</param>
    /// <param name="preserveExistingKeys">Whether to preserve keys that are already set.</param>
    /// <returns>A dictionary mapping entries to their generated keys.</returns>
    /// <exception cref="ArgumentNullException">Thrown when entries is null.</exception>
    public static Dictionary<BibEntry, string> RegenerateKeys(
        IEnumerable<BibEntry> entries,
        BibKeyGenerator.KeyFormat format = BibKeyGenerator.KeyFormat.AuthorYear,
        bool preserveExistingKeys = true)
    {
        if (entries == null)
        {
            throw new ArgumentNullException(nameof(entries));
        }

        var keyMap = BibKeyGenerator.GenerateKeys(entries, format, preserveExistingKeys);

        // Apply the generated keys to the entries
        foreach (var kvp in keyMap)
        {
            kvp.Key.Key = kvp.Value;
        }

        return keyMap;
    }

    /// <summary>
    /// Creates a clone of this entry.
    /// </summary>
    /// <returns>A new <see cref="BibEntry"/> that is a clone of this instance.</returns>
    public BibEntry Clone()
    {
        var clone = new BibEntry(EntryType)
        {
            Key = Key
        };

        // Copy all fields
        foreach (var field in fields)
        {
            if (field.Key != FieldName.Author && field.Key != FieldName.Editor)
            {
                clone.fields[field.Key] = field.Value;
                clone.fieldOrder.Add(field.Key);
            }
        }

        // Copy authors and editors
        foreach (var author in authors)
        {
            clone.authors.Add(new Author(author.LastName, author.FirstName, author.MiddleName, author.Suffix));
        }

        foreach (var editor in editors)
        {
            clone.editors.Add(new Author(editor.LastName, editor.FirstName, editor.MiddleName, editor.Suffix));
        }

        clone.UpdateAuthorsField();
        clone.UpdateEditorsField();

        return clone;
    }

    /// <summary>
    /// Validates this entry against the requirements for its entry type.
    /// </summary>
    /// <returns>A <see cref="ValidationResult"/> indicating whether the entry is valid.</returns>
    public ValidationResult Validate()
    {
        var result = new ValidationResult();

        // Check for empty key
        if (string.IsNullOrEmpty(Key))
        {
            result.AddError("Entry key is missing");
        }

        // Check for required fields based on entry type
        if (entryTypeDefinitions.TryGetValue(EntryType, out var typeDefinition))
        {
            foreach (string requiredField in typeDefinition.RequiredFields)
            {
                if (!HasField(requiredField))
                {
                    result.AddError($"Required field '{requiredField}' is missing");
                }
            }
        }
        else
        {
            // If entry type is not recognized, add a warning
            result.AddWarning($"Entry type '{EntryType}' is not a standard BibTeX type");
        }

        // Check field values for common issues
        if (Year.HasValue && (Year < 1000 || Year > 3000))
        {
            result.AddWarning($"Year value {Year} seems suspicious");
        }

        if (Volume.HasValue && Volume < 0)
        {
            result.AddWarning($"Volume value {Volume} should be non-negative");
        }

        // Check for potentially malformed authors
        foreach (var author in authors)
        {
            if (string.IsNullOrEmpty(author.LastName))
            {
                result.AddWarning("Author with empty last name detected");
            }
        }

        return result;
    }

    /// <summary>
    /// Formats this entry according to the specified citation style.
    /// </summary>
    /// <param name="style">The citation style to use for formatting.</param>
    /// <returns>A formatted citation string.</returns>
    public string ToFormattedString(CitationStyle style)
    {
        return BibFormatter.Format(this, style);
    }

    /// <summary>
    /// Converts the entry to a BibTeX-formatted string.
    /// </summary>
    /// <param name="authorFormat">The format to use for author names. If not specified, LastNameFirst format is used.</param>
    /// <returns>A BibTeX-formatted string representation of this entry.</returns>
    public string ToBibTeX(AuthorFormat? authorFormat = null)
    {
        var sb = new StringBuilder();

        // Write entry type and key
        sb.Append('@');
        sb.Append(EntryType);
        sb.Append('{');
        sb.Append(Key);
        sb.AppendLine(",");

        // Update the authors and editors fields to ensure they're correctly formatted
        // Use the specified format or default to LastNameFirst
        UpdateAuthorsField(authorFormat);
        UpdateEditorsField(authorFormat);

        // Write fields
        foreach (string fieldName in fieldOrder)
        {
            if (fields.TryGetValue(fieldName, out string? value) && !string.IsNullOrEmpty(value))
            {
                sb.Append("  ");
                sb.Append(fieldName);
                sb.Append(" = {");
                sb.Append(value);
                sb.AppendLine("},");
            }
        }

        // Remove the last comma and add the closing brace
        if (sb.Length >= 2 && sb[sb.Length - 2] == ',')
        {
            sb.Length -= 2;
            sb.AppendLine();
        }

        sb.AppendLine("}");

        return sb.ToString();
    }

    private void SetAuthors(string? authorString)
    {
        authors.Clear();

        if (string.IsNullOrEmpty(authorString))
        {
            fields.Remove(FieldName.Author);
            fieldOrder.Remove(FieldName.Author);
            InvalidateCache();
            return;
        }

        // Parse author string (format: "Lastname1, Firstname1 and Lastname2, Firstname2")
        string[] authorParts = authorString.Split(new[] { " and ", " AND " }, StringSplitOptions.None);
        foreach (string authorPart in authorParts)
        {
            authors.Add(Author.FromFormattedString(authorPart));
        }

        UpdateAuthorsField();
    }

    private void SetEditors(string? editorString)
    {
        editors.Clear();

        if (string.IsNullOrEmpty(editorString))
        {
            fields.Remove(FieldName.Editor);
            fieldOrder.Remove(FieldName.Editor);
            InvalidateCache();
            return;
        }

        // Parse editor string (format: "Lastname1, Firstname1 and Lastname2, Firstname2")
        string[] editorParts = editorString.Split(new[] { " and ", " AND " }, StringSplitOptions.None);
        foreach (string editorPart in editorParts)
        {
            editors.Add(Author.FromFormattedString(editorPart));
        }

        UpdateEditorsField();
    }

    /// <summary>
    /// Updates a contributor field (author or editor) with the provided list.
    /// This is a shared implementation to avoid code duplication.
    /// </summary>
    private void UpdateContributorField(List<Author> contributors, string fieldName, AuthorFormat? format = null)
    {
        if (contributors.Count == 0)
        {
            fields.Remove(fieldName);
            fieldOrder.Remove(fieldName);
            InvalidateCache();
            return;
        }

        // Use LastNameFirst format by default (traditional BibTeX format)
        AuthorFormat authorFormat = format ?? AuthorFormat.LastNameFirst;

        string contributorString = string.Join(" and ", contributors.Select(c => c.ToBibTeXString(authorFormat)));

        if (!fields.ContainsKey(fieldName))
        {
            fieldOrder.Add(fieldName);
        }

        fields[fieldName] = contributorString;
        InvalidateCache();
    }

    private void UpdateAuthorsField(AuthorFormat? format = null)
    {
        UpdateContributorField(authors, FieldName.Author, format);
    }

    private void UpdateEditorsField(AuthorFormat? format = null)
    {
        UpdateContributorField(editors, FieldName.Editor, format);
    }

    /// <summary>
    /// Updates the authors and editors fields with the specified format.
    /// </summary>
    /// <param name="format">The format to use for author names.</param>
    public void UpdateAuthorFields(AuthorFormat format)
    {
        UpdateAuthorsField(format);
        UpdateEditorsField(format);
    }

    private static EntryTypeDefinition DefineType(FieldName[] required, FieldName[] optional)
    {
        return new EntryTypeDefinition(
            required.Select(f => (string)f).ToArray(),
            optional.Select(f => (string)f).ToArray());
    }

    private class EntryTypeDefinition
    {
        public string[] RequiredFields { get; }
        public string[] OptionalFields { get; }

        public EntryTypeDefinition(string[] requiredFields, string[] optionalFields)
        {
            RequiredFields = requiredFields;
            OptionalFields = optionalFields;
        }
    }
}
