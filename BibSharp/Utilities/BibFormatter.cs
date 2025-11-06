using System;
using System.Collections.Generic;
using System.Text;
using BibSharp.Models;

namespace BibSharp.Utilities;

/// <summary>
/// Defines available citation styles for BibFormatter.
/// </summary>
public enum CitationStyle
{
    /// <summary>
    /// Chicago style (author-date format).
    /// </summary>
    Chicago,

    /// <summary>
    /// MLA (Modern Language Association) style.
    /// </summary>
    Mla,

    /// <summary>
    /// APA (American Psychological Association) style.
    /// </summary>
    Apa,

    /// <summary>
    /// Harvard style.
    /// </summary>
    Harvard,

    /// <summary>
    /// IEEE (Institute of Electrical and Electronics Engineers) style.
    /// </summary>
    Ieee
}

/// <summary>
/// Provides methods for formatting BibEntry objects into various citation styles.
/// </summary>
/// <remarks>
/// <para>This class uses a strategy pattern internally. For advanced scenarios, you can implement
/// <see cref="ICitationFormatter"/> to create custom citation styles.</para>
/// <para><b>Security Note:</b> The formatted output includes HTML-like tags (e.g., &lt;i&gt; for italics)
/// but does NOT encode special HTML characters in field values. If you need to display formatted citations
/// in HTML contexts where user-provided content might contain special characters, you should HTML-encode
/// the output before rendering it in a web page to prevent potential XSS issues.</para>
/// <example>
/// Example of safe HTML rendering:
/// <code>
/// string citation = BibFormatter.Format(entry, CitationStyle.APA);
/// string safeCitation = System.Net.WebUtility.HtmlEncode(citation);
/// // Now safe to render in HTML
/// </code>
/// </example>
/// </remarks>
public static class BibFormatter
{
    // Citation style constants - these control author truncation behavior
    // Based on citation style standards (APA 7th, MLA 9th, Chicago 17th, etc.)
    private const int apaMaxAuthorsBeforeEllipsis = 20;  // APA 7th edition standard
    private const int apaPositionBeforeEllipsis = 19; // 0-based index
    private const int ieeeMaxAuthorsBeforeEtAl = 6;  // IEEE citation standard
    private const int ieeePositionBeforeEtAl = 5; // 0-based index
    private const int chicagoMaxAuthorsBeforeEtAl = 3;  // Chicago author-date style
    private const int mlaMaxAuthorsBeforeEtAl = 2;  // MLA 9th edition
    private const int harvardMaxAuthorsBeforeEtAl = 3;  // Harvard referencing standard

    /// <summary>
    /// Formats a BibEntry into the specified citation style.
    /// </summary>
    /// <param name="entry">The BibEntry to format.</param>
    /// <param name="style">The citation style to use.</param>
    /// <returns>A formatted citation string.</returns>
    public static string Format(BibEntry entry, CitationStyle style)
    {
        if (entry == null)
        {
            throw new ArgumentNullException(nameof(entry));
        }

        return style switch
        {
            CitationStyle.Chicago => FormatChicago(entry),
            CitationStyle.Mla => FormatMla(entry),
            CitationStyle.Apa => FormatApa(entry),
            CitationStyle.Harvard => FormatHarvard(entry),
            CitationStyle.Ieee => FormatIeee(entry),
            _ => throw new ArgumentException($"Unsupported citation style: {style}", nameof(style))
        };
    }

    /// <summary>
    /// Formats a BibEntry using a custom citation formatter.
    /// </summary>
    /// <param name="entry">The BibEntry to format.</param>
    /// <param name="formatter">The custom formatter to use.</param>
    /// <returns>A formatted citation string.</returns>
    public static string Format(BibEntry entry, ICitationFormatter formatter)
    {
        if (entry == null)
        {
            throw new ArgumentNullException(nameof(entry));
        }

        if (formatter == null)
        {
            throw new ArgumentNullException(nameof(formatter));
        }

        return formatter.Format(entry);
    }

    /// <summary>
    /// Formats a BibEntry collection into the specified citation style.
    /// </summary>
    /// <param name="entries">The BibEntry collection to format.</param>
    /// <param name="style">The citation style to use.</param>
    /// <returns>A formatted bibliography string.</returns>
    public static string FormatBibliography(IEnumerable<BibEntry> entries, CitationStyle style)
    {
        if (entries == null)
        {
            throw new ArgumentNullException(nameof(entries));
        }

        var sb = new StringBuilder();

        foreach (var entry in entries)
        {
            sb.AppendLine(Format(entry, style));
            sb.AppendLine();
        }

        return sb.ToString();
    }

    // Helper methods to reduce code duplication
    
    /// <summary>
    /// Formats author name with initials for first and middle names.
    /// </summary>
    private static string FormatAuthorWithInitials(Author author, bool firstNameFirst = false)
    {
        var sb = new StringBuilder();
        
        if (firstNameFirst)
        {
            if (!string.IsNullOrEmpty(author.FirstName))
            {
                sb.Append(author.FirstName[0]);
                sb.Append(". ");
            }

            if (!string.IsNullOrEmpty(author.MiddleName))
            {
                sb.Append(author.MiddleName[0]);
                sb.Append(". ");
            }

            sb.Append(author.LastName);
        }
        else
        {
            sb.Append(author.LastName);
            sb.Append(", ");

            if (!string.IsNullOrEmpty(author.FirstName))
            {
                sb.Append(author.FirstName[0]);
                sb.Append(".");
            }

            if (!string.IsNullOrEmpty(author.MiddleName))
            {
                sb.Append(" ");
                sb.Append(author.MiddleName[0]);
                sb.Append(".");
            }
        }
        
        return sb.ToString();
    }
    
    /// <summary>
    /// Formats author name in full "LastName, FirstName MiddleName" format.
    /// </summary>
    private static string FormatAuthorFull(Author author, bool includeMiddle = true)
    {
        var sb = new StringBuilder();
        sb.Append(author.LastName);
        
        if (!string.IsNullOrEmpty(author.FirstName))
        {
            sb.Append(", ");
            sb.Append(author.FirstName);
            
            if (includeMiddle && !string.IsNullOrEmpty(author.MiddleName))
            {
                sb.Append(" ");
                sb.Append(author.MiddleName);
            }
        }
        
        return sb.ToString();
    }
    
    /// <summary>
    /// Formats author name in display order "FirstName MiddleName LastName".
    /// </summary>
    private static string FormatAuthorDisplay(Author author, bool includeMiddle = true)
    {
        var sb = new StringBuilder();
        
        if (!string.IsNullOrEmpty(author.FirstName))
        {
            sb.Append(author.FirstName);
            
            if (includeMiddle && !string.IsNullOrEmpty(author.MiddleName))
            {
                sb.Append(" ");
                sb.Append(author.MiddleName);
            }
            
            sb.Append(" ");
        }
        
        sb.Append(author.LastName);
        return sb.ToString();
    }
    
    /// <summary>
    /// Appends "et al" or similar notation based on remaining author count.
    /// </summary>
    private static void AppendEtAl(StringBuilder sb, string notation = "et al")
    {
        sb.Append(" ");
        sb.Append(notation);
    }
    
    /// <summary>
    /// Ensures a string ends with a period if it doesn't end with punctuation.
    /// </summary>
    private static void EnsureEndsPeriod(StringBuilder sb)
    {
        if (sb.Length > 0 && sb[sb.Length - 1] != '.')
        {
            sb.Append('.');
        }
    }

    /// <summary>
    /// Formats a BibEntry in Chicago style (author-date format).
    /// </summary>
    /// <param name="entry">The BibEntry to format.</param>
    /// <returns>A Chicago-style formatted citation string.</returns>
    private static string FormatChicago(BibEntry entry)
    {
        var sb = new StringBuilder();

        // Format authors
        if (entry.Authors.Count > 0)
        {
            if (entry.Authors.Count == 1)
            {
                // Single author
                var author = entry.Authors[0];
                sb.Append($"{author.LastName}, {author.FirstName}");
                if (!string.IsNullOrEmpty(author.MiddleName))
                {
                    sb.Append($" {author.MiddleName}");
                }
            }
            else if (entry.Authors.Count <= chicagoMaxAuthorsBeforeEtAl)
            {
                // 2-3 authors: list all
                for (int i = 0; i < entry.Authors.Count; i++)
                {
                    var author = entry.Authors[i];
                    if (i == 0)
                    {
                        sb.Append($"{author.LastName}, {author.FirstName}");
                        if (!string.IsNullOrEmpty(author.MiddleName))
                        {
                            sb.Append($" {author.MiddleName}");
                        }
                    }
                    else if (i == entry.Authors.Count - 1)
                    {
                        sb.Append($", and {author.FirstName}");
                        if (!string.IsNullOrEmpty(author.MiddleName))
                        {
                            sb.Append($" {author.MiddleName}");
                        }
                        sb.Append($" {author.LastName}");
                    }
                    else
                    {
                        sb.Append($", {author.FirstName}");
                        if (!string.IsNullOrEmpty(author.MiddleName))
                        {
                            sb.Append($" {author.MiddleName}");
                        }
                        sb.Append($" {author.LastName}");
                    }
                }
            }
            else
            {
                // 4+ authors: first author + et al.
                var author = entry.Authors[0];
                sb.Append($"{author.LastName}, {author.FirstName}");
                if (!string.IsNullOrEmpty(author.MiddleName))
                {
                    sb.Append($" {author.MiddleName}");
                }
                sb.Append(" et al");
            }
        }
        else if (entry.Editors.Count > 0)
        {
            // Use editors if no authors
            if (entry.Editors.Count == 1)
            {
                // Single editor
                var editor = entry.Editors[0];
                sb.Append($"{editor.LastName}, {editor.FirstName}");
                if (!string.IsNullOrEmpty(editor.MiddleName))
                {
                    sb.Append($" {editor.MiddleName}");
                }
                sb.Append(", ed");
            }
            else if (entry.Editors.Count <= chicagoMaxAuthorsBeforeEtAl)
            {
                // 2-3 editors: list all
                for (int i = 0; i < entry.Editors.Count; i++)
                {
                    var editor = entry.Editors[i];
                    if (i == 0)
                    {
                        sb.Append($"{editor.LastName}, {editor.FirstName}");
                        if (!string.IsNullOrEmpty(editor.MiddleName))
                        {
                            sb.Append($" {editor.MiddleName}");
                        }
                    }
                    else if (i == entry.Editors.Count - 1)
                    {
                        sb.Append($", and {editor.FirstName}");
                        if (!string.IsNullOrEmpty(editor.MiddleName))
                        {
                            sb.Append($" {editor.MiddleName}");
                        }
                        sb.Append($" {editor.LastName}");
                    }
                    else
                    {
                        sb.Append($", {editor.FirstName}");
                        if (!string.IsNullOrEmpty(editor.MiddleName))
                        {
                            sb.Append($" {editor.MiddleName}");
                        }
                        sb.Append($" {editor.LastName}");
                    }
                }
                sb.Append(", eds");
            }
            else
            {
                // 4+ editors: first editor + et al.
                var editor = entry.Editors[0];
                sb.Append($"{editor.LastName}, {editor.FirstName}");
                if (!string.IsNullOrEmpty(editor.MiddleName))
                {
                    sb.Append($" {editor.MiddleName}");
                }
                sb.Append(" et al., eds");
            }
        }
        else
        {
            // No authors or editors
            sb.Append(entry.Title);
        }

        // Year
        if (entry.Year.HasValue)
        {
            sb.Append($". {entry.Year}");
        }

        // Title (different formatting based on entry type)
        if (!string.IsNullOrEmpty(entry.Title))
        {
            if (entry.EntryType == EntryType.Article)
            {
                // Article title in quotes
                sb.Append($". \"{entry.Title}.\"");
            }
            else if (entry.EntryType == EntryType.Book ||
                     entry.EntryType == EntryType.Proceedings)
            {
                // Book/proceedings title in italics
                sb.Append($". <i>{entry.Title}</i>");
            }
            else if (entry.EntryType == EntryType.InProceedings ||
                     entry.EntryType == EntryType.InCollection)
            {
                // Chapter title in quotes
                sb.Append($". \"{entry.Title}.\"");

                // Book title in italics
                if (!string.IsNullOrEmpty(entry.BookTitle))
                {
                    sb.Append($" In <i>{entry.BookTitle}</i>");

                    // Editors (if not already used as primary authors)
                    if (entry.Authors.Count > 0 && entry.Editors.Count > 0)
                    {
                        sb.Append(", edited by ");
                        for (int i = 0; i < entry.Editors.Count; i++)
                        {
                            var editor = entry.Editors[i];
                            if (i == 0)
                            {
                                sb.Append($"{editor.FirstName}");
                                if (!string.IsNullOrEmpty(editor.MiddleName))
                                {
                                    sb.Append($" {editor.MiddleName}");
                                }
                                sb.Append($" {editor.LastName}");
                            }
                            else if (i == entry.Editors.Count - 1)
                            {
                                sb.Append($" and {editor.FirstName}");
                                if (!string.IsNullOrEmpty(editor.MiddleName))
                                {
                                    sb.Append($" {editor.MiddleName}");
                                }
                                sb.Append($" {editor.LastName}");
                            }
                            else
                            {
                                sb.Append($", {editor.FirstName}");
                                if (!string.IsNullOrEmpty(editor.MiddleName))
                                {
                                    sb.Append($" {editor.MiddleName}");
                                }
                                sb.Append($" {editor.LastName}");
                            }
                        }
                    }
                }
            }
            else
            {
                // Default: title in quotes
                sb.Append($". \"{entry.Title}.\"");
            }
        }

        // Journal name for articles
        if (entry.EntryType == EntryType.Article &&
            !string.IsNullOrEmpty(entry.Journal))
        {
            sb.Append($" <i>{entry.Journal}</i>");

            // Volume and issue
            if (entry.Volume.HasValue)
            {
                sb.Append($" {entry.Volume}");
                if (!string.IsNullOrEmpty(entry.Number))
                {
                    sb.Append($", no. {entry.Number}");
                }
            }
        }

        // Publisher info for books
        if ((entry.EntryType == EntryType.Book ||
             entry.EntryType == EntryType.InProceedings ||
             entry.EntryType == EntryType.InCollection) &&
            !string.IsNullOrEmpty(entry.Publisher))
        {
            sb.Append(".");

            // Edition
            if (!string.IsNullOrEmpty(entry.Edition))
            {
                sb.Append($" {entry.Edition} ed.");
            }

            // Place of publication
            if (!string.IsNullOrEmpty(entry.Address))
            {
                sb.Append($" {entry.Address}:");
            }

            // Publisher
            sb.Append($" {entry.Publisher}");
        }

        // Pages for articles and book chapters
        if (entry.Pages != null)
        {
            if (entry.EntryType == EntryType.Article)
            {
                sb.Append($": {entry.Pages}");
            }
            else if (entry.EntryType == EntryType.InProceedings ||
                     entry.EntryType == EntryType.InCollection)
            {
                sb.Append($", {entry.Pages}");
            }
        }

        // DOI
        if (!string.IsNullOrEmpty(entry.Doi))
        {
            sb.Append($". DOI: {entry.Doi}");
        }

        // URL
        if (!string.IsNullOrEmpty(entry.Url))
        {
            sb.Append($". {entry.Url}");
        }

        // End with period
        if (sb.Length > 0 && sb[sb.Length - 1] != '.')
        {
            sb.Append('.');
        }

        return sb.ToString();
    }

    /// <summary>
    /// Formats a BibEntry in MLA style.
    /// </summary>
    /// <param name="entry">The BibEntry to format.</param>
    /// <returns>An MLA-style formatted citation string.</returns>
    private static string FormatMla(BibEntry entry)
    {
        var sb = new StringBuilder();

        // Format authors
        if (entry.Authors.Count > 0)
        {
            if (entry.Authors.Count == 1)
            {
                // Single author
                var author = entry.Authors[0];
                sb.Append($"{author.LastName}, {author.FirstName}");
                if (!string.IsNullOrEmpty(author.MiddleName))
                {
                    sb.Append($" {author.MiddleName}");
                }
            }
            else if (entry.Authors.Count == 2)
            {
                // 2 authors
                var author1 = entry.Authors[0];
                var author2 = entry.Authors[1];
                sb.Append($"{author1.LastName}, {author1.FirstName}");
                if (!string.IsNullOrEmpty(author1.MiddleName))
                {
                    sb.Append($" {author1.MiddleName}");
                }
                sb.Append($", and {author2.FirstName}");
                if (!string.IsNullOrEmpty(author2.MiddleName))
                {
                    sb.Append($" {author2.MiddleName}");
                }
                sb.Append($" {author2.LastName}");
            }
            else
            {
                // 3+ authors: first author + et al. (MLA)
                var author = entry.Authors[0];
                sb.Append($"{author.LastName}, {author.FirstName}");
                if (!string.IsNullOrEmpty(author.MiddleName))
                {
                    sb.Append($" {author.MiddleName}");
                }
                sb.Append(", et al");
            }
        }
        else if (entry.Editors.Count > 0)
        {
            // Use editors if no authors
            if (entry.Editors.Count == 1)
            {
                // Single editor
                var editor = entry.Editors[0];
                sb.Append($"{editor.LastName}, {editor.FirstName}");
                if (!string.IsNullOrEmpty(editor.MiddleName))
                {
                    sb.Append($" {editor.MiddleName}");
                }
                sb.Append(", editor");
            }
            else if (entry.Editors.Count == 2)
            {
                // 2 editors
                var editor1 = entry.Editors[0];
                var editor2 = entry.Editors[1];
                sb.Append($"{editor1.LastName}, {editor1.FirstName}");
                if (!string.IsNullOrEmpty(editor1.MiddleName))
                {
                    sb.Append($" {editor1.MiddleName}");
                }
                sb.Append($", and {editor2.FirstName}");
                if (!string.IsNullOrEmpty(editor2.MiddleName))
                {
                    sb.Append($" {editor2.MiddleName}");
                }
                sb.Append($" {editor2.LastName}, editors");
            }
            else
            {
                // 3+ editors: first editor + et al.
                var editor = entry.Editors[0];
                sb.Append($"{editor.LastName}, {editor.FirstName}");
                if (!string.IsNullOrEmpty(editor.MiddleName))
                {
                    sb.Append($" {editor.MiddleName}");
                }
                sb.Append(", et al., editors");
            }
        }
        else
        {
            // No authors or editors, use title
            sb.Append(entry.Title);
        }

        // Title (different formatting based on entry type)
        if (!string.IsNullOrEmpty(entry.Title))
        {
            if (entry.EntryType == EntryType.Article)
            {
                // Article title in quotes
                sb.Append($". \"{entry.Title}.\"");
            }
            else if (entry.EntryType == EntryType.Book ||
                     entry.EntryType == EntryType.Proceedings)
            {
                // Book/proceedings title in italics with a period
                sb.Append($". <i>{entry.Title}</i>");
            }
            else if (entry.EntryType == EntryType.InProceedings ||
                     entry.EntryType == EntryType.InCollection)
            {
                // Chapter title in quotes
                sb.Append($". \"{entry.Title}.\"");

                // Book title in italics
                if (!string.IsNullOrEmpty(entry.BookTitle))
                {
                    sb.Append($" <i>{entry.BookTitle}</i>");

                    // Editors (if not already used as primary authors)
                    if (entry.Authors.Count > 0 && entry.Editors.Count > 0)
                    {
                        sb.Append(", edited by ");
                        for (int i = 0; i < entry.Editors.Count; i++)
                        {
                            var editor = entry.Editors[i];
                            if (i == 0)
                            {
                                sb.Append($"{editor.FirstName}");
                                if (!string.IsNullOrEmpty(editor.MiddleName))
                                {
                                    sb.Append($" {editor.MiddleName}");
                                }
                                sb.Append($" {editor.LastName}");
                            }
                            else if (i == entry.Editors.Count - 1)
                            {
                                sb.Append($" and {editor.FirstName}");
                                if (!string.IsNullOrEmpty(editor.MiddleName))
                                {
                                    sb.Append($" {editor.MiddleName}");
                                }
                                sb.Append($" {editor.LastName}");
                            }
                            else
                            {
                                sb.Append($", {editor.FirstName}");
                                if (!string.IsNullOrEmpty(editor.MiddleName))
                                {
                                    sb.Append($" {editor.MiddleName}");
                                }
                                sb.Append($" {editor.LastName}");
                            }
                        }
                    }
                }
            }
            else
            {
                // Default: title in quotes
                sb.Append($". \"{entry.Title}.\"");
            }
        }

        // Journal name for articles
        if (entry.EntryType == EntryType.Article &&
            !string.IsNullOrEmpty(entry.Journal))
        {
            sb.Append($" <i>{entry.Journal}</i>");

            // Volume and issue
            if (entry.Volume.HasValue)
            {
                sb.Append($", vol. {entry.Volume}");
                if (!string.IsNullOrEmpty(entry.Number))
                {
                    sb.Append($", no. {entry.Number}");
                }
            }
        }

        // Publisher info for books
        if ((entry.EntryType == EntryType.Book ||
             entry.EntryType == EntryType.InProceedings ||
             entry.EntryType == EntryType.InCollection) &&
            !string.IsNullOrEmpty(entry.Publisher))
        {
            // Edition
            if (!string.IsNullOrEmpty(entry.Edition))
            {
                sb.Append($", {entry.Edition} ed.");
            }

            sb.Append(",");

            // Publisher
            sb.Append($" {entry.Publisher}");

            // Place of publication
            if (!string.IsNullOrEmpty(entry.Address))
            {
                sb.Append($", {entry.Address}");
            }
        }

        // Year
        if (entry.Year.HasValue)
        {
            sb.Append($", {entry.Year}");
        }

        // Pages for articles and book chapters
        if (entry.Pages != null)
        {
            if (entry.EntryType == EntryType.Article)
            {
                sb.Append($", pp. {entry.Pages}");
            }
            else if (entry.EntryType == EntryType.InProceedings ||
                     entry.EntryType == EntryType.InCollection)
            {
                sb.Append($", pp. {entry.Pages}");
            }
        }

        // DOI
        if (!string.IsNullOrEmpty(entry.Doi))
        {
            sb.Append($", DOI: {entry.Doi}");
        }

        // URL
        if (!string.IsNullOrEmpty(entry.Url))
        {
            sb.Append($", {entry.Url}");
        }

        // End with period
        if (sb.Length > 0 && sb[sb.Length - 1] != '.')
        {
            sb.Append('.');
        }

        return sb.ToString();
    }

    /// <summary>
    /// Formats a BibEntry in APA style.
    /// </summary>
    /// <param name="entry">The BibEntry to format.</param>
    /// <returns>An APA-style formatted citation string.</returns>
    private static string FormatApa(BibEntry entry)
    {
        var sb = new StringBuilder();

        // Format authors
        if (entry.Authors.Count > 0)
        {
            for (int i = 0; i < entry.Authors.Count; i++)
            {
                var author = entry.Authors[i];

                if (i > 0)
                {
                    // Handle the last author differently
                    if (i == entry.Authors.Count - 1)
                    {
                        // If there are more than 2 authors, use a comma before the &
                        if (entry.Authors.Count > 2)
                        {
                            sb.Append(",");
                        }
                        sb.Append(" & ");
                    }
                    else
                    {
                        sb.Append(", ");
                    }
                }

                // Format each author as "LastName, F. M."
                sb.Append(author.LastName);
                sb.Append(", ");

                if (!string.IsNullOrEmpty(author.FirstName) && author.FirstName.Length > 0)
                {
                    sb.Append(author.FirstName[0]);
                    sb.Append(".");
                }

                if (!string.IsNullOrEmpty(author.MiddleName) && author.MiddleName.Length > 0)
                {
                    sb.Append(" ");
                    sb.Append(author.MiddleName[0]);
                    sb.Append(".");
                }

                // APA style now includes up to 20 authors before using ellipsis
                if (i == apaPositionBeforeEllipsis && entry.Authors.Count > apaMaxAuthorsBeforeEllipsis)
                {
                    sb.Append(", . . .");

                    // Add the last author
                    var lastAuthor = entry.Authors[entry.Authors.Count - 1];
                    sb.Append(", & ");
                    sb.Append(lastAuthor.LastName);
                    sb.Append(", ");

                    if (!string.IsNullOrEmpty(lastAuthor.FirstName) && lastAuthor.FirstName.Length > 0)
                    {
                        sb.Append(lastAuthor.FirstName[0]);
                        sb.Append(".");
                    }

                    if (!string.IsNullOrEmpty(lastAuthor.MiddleName) && lastAuthor.MiddleName.Length > 0)
                    {
                        sb.Append(" ");
                        sb.Append(lastAuthor.MiddleName[0]);
                        sb.Append(".");
                    }

                    // Break the loop as we've added the last author
                    break;
                }
            }
        }
        else if (entry.Editors.Count > 0)
        {
            // Use editors if no authors
            for (int i = 0; i < entry.Editors.Count; i++)
            {
                var editor = entry.Editors[i];

                if (i > 0)
                {
                    // Handle the last editor differently
                    if (i == entry.Editors.Count - 1)
                    {
                        // If there are more than 2 editors, use a comma before the &
                        if (entry.Editors.Count > 2)
                        {
                            sb.Append(",");
                        }
                        sb.Append(" & ");
                    }
                    else
                    {
                        sb.Append(", ");
                    }
                }

                // Format each editor as "LastName, F. M."
                sb.Append(editor.LastName);
                sb.Append(", ");

                if (!string.IsNullOrEmpty(editor.FirstName) && editor.FirstName.Length > 0)
                {
                    sb.Append(editor.FirstName[0]);
                    sb.Append(".");
                }

                if (!string.IsNullOrEmpty(editor.MiddleName) && editor.MiddleName.Length > 0)
                {
                    sb.Append(" ");
                    sb.Append(editor.MiddleName[0]);
                    sb.Append(".");
                }
            }

            // Add (Ed.) or (Eds.) as appropriate
            if (entry.Editors.Count == 1)
            {
                sb.Append(" (Ed.)");
            }
            else
            {
                sb.Append(" (Eds.)");
            }
        }
        else
        {
            // No authors or editors, use title
            sb.Append(entry.Title);
        }

        // Year
        if (entry.Year.HasValue)
        {
            sb.Append($" ({entry.Year})");
        }
        else
        {
            sb.Append(" (n.d.)");
        }

        // Title (different formatting based on entry type)
        if (!string.IsNullOrEmpty(entry.Title))
        {
            sb.Append(". ");

            if (entry.EntryType == EntryType.Article)
            {
                // Article title in sentence case, no quotes or italics
                sb.Append(entry.Title);

                // Make sure it ends with a period
                if (!entry.Title.EndsWith(".", StringComparison.Ordinal) &&
                    !entry.Title.EndsWith("?", StringComparison.Ordinal) &&
                    !entry.Title.EndsWith("!", StringComparison.Ordinal))
                {
                    sb.Append(".");
                }
            }
            else if (entry.EntryType == EntryType.Book ||
                     entry.EntryType == EntryType.Proceedings)
            {
                // Book/proceedings title in italics, sentence case
                sb.Append("<i>");
                sb.Append(entry.Title);

                // Make sure it ends with a period
                if (!entry.Title.EndsWith(".", StringComparison.Ordinal) &&
                    !entry.Title.EndsWith("?", StringComparison.Ordinal) &&
                    !entry.Title.EndsWith("!", StringComparison.Ordinal))
                {
                    sb.Append(".");
                }

                sb.Append("</i>");

                // Edition if available
                if (!string.IsNullOrEmpty(entry.Edition))
                {
                    sb.Append($" ({entry.Edition} ed.)");
                }
            }
            else if (entry.EntryType == EntryType.InProceedings ||
                     entry.EntryType == EntryType.InCollection)
            {
                // Chapter title in sentence case, no formatting
                sb.Append(entry.Title);

                // Make sure it ends with a period
                if (!entry.Title.EndsWith(".", StringComparison.Ordinal) &&
                    !entry.Title.EndsWith("?", StringComparison.Ordinal) &&
                    !entry.Title.EndsWith("!", StringComparison.Ordinal))
                {
                    sb.Append(".");
                }

                // Book title in italics
                if (!string.IsNullOrEmpty(entry.BookTitle))
                {
                    sb.Append(" In ");

                    // Add editors here for APA style
                    if (entry.Authors.Count > 0 && entry.Editors.Count > 0)
                    {
                        for (int i = 0; i < entry.Editors.Count; i++)
                        {
                            var editor = entry.Editors[i];

                            if (i > 0)
                            {
                                // Handle the last editor differently
                                if (i == entry.Editors.Count - 1)
                                {
                                    // If there are more than 2 editors, use a comma before the &
                                    if (entry.Editors.Count > 2)
                                    {
                                        sb.Append(",");
                                    }
                                    sb.Append(" & ");
                                }
                                else
                                {
                                    sb.Append(", ");
                                }
                            }

                            if (!string.IsNullOrEmpty(editor.FirstName) && editor.FirstName.Length > 0)
                            {
                                sb.Append(editor.FirstName[0]);
                                sb.Append(".");
                            }

                            if (!string.IsNullOrEmpty(editor.MiddleName) && editor.MiddleName.Length > 0)
                            {
                                sb.Append(" ");
                                sb.Append(editor.MiddleName[0]);
                                sb.Append(".");
                            }

                            sb.Append(" ");
                            sb.Append(editor.LastName);
                        }

                        // Add (Ed.) or (Eds.) as appropriate
                        if (entry.Editors.Count == 1)
                        {
                            sb.Append(" (Ed.), ");
                        }
                        else
                        {
                            sb.Append(" (Eds.), ");
                        }
                    }

                    sb.Append("<i>");
                    sb.Append(entry.BookTitle);
                    sb.Append("</i>");

                    // Add page range specific for chapters in APA style
                    if (entry.Pages != null)
                    {
                        sb.Append($" (pp. {entry.Pages})");
                    }
                }
            }
            else
            {
                // Default: title in no special formatting
                sb.Append(entry.Title);

                // Make sure it ends with a period
                if (!entry.Title.EndsWith(".", StringComparison.Ordinal) &&
                    !entry.Title.EndsWith("?", StringComparison.Ordinal) &&
                    !entry.Title.EndsWith("!", StringComparison.Ordinal))
                {
                    sb.Append(".");
                }
            }
        }

        // Journal name for articles
        if (entry.EntryType == EntryType.Article &&
            !string.IsNullOrEmpty(entry.Journal))
        {
            sb.Append(" ");
            sb.Append("<i>");
            sb.Append(entry.Journal);

            // In APA, volume is italicized, issue is in parentheses
            if (entry.Volume.HasValue)
            {
                sb.Append(", ");
                sb.Append(entry.Volume);
                sb.Append("</i>");

                if (!string.IsNullOrEmpty(entry.Number))
                {
                    sb.Append($"({entry.Number})");
                }
                // No else needed - italics already closed above
            }
            else
            {
                sb.Append("</i>");
            }

            // Pages for articles (without pp.)
            if (entry.Pages != null)
            {
                sb.Append(", ");
                sb.Append(entry.Pages);
            }
        }

        // Publisher info for books
        if ((entry.EntryType == EntryType.Book ||
             entry.EntryType == EntryType.InProceedings ||
             entry.EntryType == EntryType.InCollection) &&
            !string.IsNullOrEmpty(entry.Publisher))
        {
            // APA style: Publisher location: Publisher
            if (!string.IsNullOrEmpty(entry.Address))
            {
                sb.Append(" ");
                sb.Append(entry.Address);
            }

            // APA 7th no longer requires location
            sb.Append(": ");
            sb.Append(entry.Publisher);
        }

        // DOI
        if (!string.IsNullOrEmpty(entry.Doi))
        {
            sb.Append(". https://doi.org/");
            sb.Append(entry.Doi);
        }
        // URL if no DOI
        else if (!string.IsNullOrEmpty(entry.Url))
        {
            sb.Append(". ");
            sb.Append(entry.Url);
        }

        return sb.ToString();
    }

    /// <summary>
    /// Formats a BibEntry in Harvard style.
    /// </summary>
    /// <param name="entry">The BibEntry to format.</param>
    /// <returns>A Harvard-style formatted citation string.</returns>
    private static string FormatHarvard(BibEntry entry)
    {
        var sb = new StringBuilder();

        // Format authors
        if (entry.Authors.Count > 0)
        {
            if (entry.Authors.Count == 1)
            {
                // Single author
                var author = entry.Authors[0];
                sb.Append(author.LastName);
                if (!string.IsNullOrEmpty(author.FirstName) && author.FirstName.Length > 0)
                {
                    sb.Append($", {author.FirstName[0]}.");
                    if (!string.IsNullOrEmpty(author.MiddleName) && author.MiddleName.Length > 0)
                    {
                        sb.Append($"{author.MiddleName[0]}.");
                    }
                }
            }
            else if (entry.Authors.Count <= harvardMaxAuthorsBeforeEtAl)
            {
                // 2-3 authors: list all
                for (int i = 0; i < entry.Authors.Count; i++)
                {
                    var author = entry.Authors[i];
                    if (i == 0)
                    {
                        sb.Append(author.LastName);
                        if (!string.IsNullOrEmpty(author.FirstName) && author.FirstName.Length > 0)
                        {
                            sb.Append($", {author.FirstName[0]}.");
                            if (!string.IsNullOrEmpty(author.MiddleName) && author.MiddleName.Length > 0)
                            {
                                sb.Append($"{author.MiddleName[0]}.");
                            }
                        }
                    }
                    else if (i == entry.Authors.Count - 1)
                    {
                        sb.Append($" and {author.LastName}");
                        if (!string.IsNullOrEmpty(author.FirstName) && author.FirstName.Length > 0)
                        {
                            sb.Append($", {author.FirstName[0]}.");
                            if (!string.IsNullOrEmpty(author.MiddleName) && author.MiddleName.Length > 0)
                            {
                                sb.Append($"{author.MiddleName[0]}.");
                            }
                        }
                    }
                    else
                    {
                        sb.Append($", {author.LastName}");
                        if (!string.IsNullOrEmpty(author.FirstName) && author.FirstName.Length > 0)
                        {
                            sb.Append($", {author.FirstName[0]}.");
                            if (!string.IsNullOrEmpty(author.MiddleName) && author.MiddleName.Length > 0)
                            {
                                sb.Append($"{author.MiddleName[0]}.");
                            }
                        }
                    }
                }
            }
            else
            {
                // 4+ authors: first author + et al.
                var author = entry.Authors[0];
                sb.Append(author.LastName);
                if (!string.IsNullOrEmpty(author.FirstName) && author.FirstName.Length > 0)
                {
                    sb.Append($", {author.FirstName[0]}.");
                    if (!string.IsNullOrEmpty(author.MiddleName) && author.MiddleName.Length > 0)
                    {
                        sb.Append($"{author.MiddleName[0]}.");
                    }
                }
                sb.Append(" et al");
            }
        }
        else if (entry.Editors.Count > 0)
        {
            // Use editors if no authors
            if (entry.Editors.Count == 1)
            {
                // Single editor
                var editor = entry.Editors[0];
                sb.Append(editor.LastName);
                if (!string.IsNullOrEmpty(editor.FirstName) && editor.FirstName.Length > 0)
                {
                    sb.Append($", {editor.FirstName[0]}.");
                    if (!string.IsNullOrEmpty(editor.MiddleName) && editor.MiddleName.Length > 0)
                    {
                        sb.Append($"{editor.MiddleName[0]}.");
                    }
                }
                sb.Append(" (ed.)");
            }
            else if (entry.Editors.Count <= harvardMaxAuthorsBeforeEtAl)
            {
                // 2-3 editors: list all
                for (int i = 0; i < entry.Editors.Count; i++)
                {
                    var editor = entry.Editors[i];
                    if (i == 0)
                    {
                        sb.Append(editor.LastName);
                        if (!string.IsNullOrEmpty(editor.FirstName) && editor.FirstName.Length > 0)
                        {
                            sb.Append($", {editor.FirstName[0]}.");
                            if (!string.IsNullOrEmpty(editor.MiddleName) && editor.MiddleName.Length > 0)
                            {
                                sb.Append($"{editor.MiddleName[0]}.");
                            }
                        }
                    }
                    else if (i == entry.Editors.Count - 1)
                    {
                        sb.Append($" and {editor.LastName}");
                        if (!string.IsNullOrEmpty(editor.FirstName) && editor.FirstName.Length > 0)
                        {
                            sb.Append($", {editor.FirstName[0]}.");
                            if (!string.IsNullOrEmpty(editor.MiddleName) && editor.MiddleName.Length > 0)
                            {
                                sb.Append($"{editor.MiddleName[0]}.");
                            }
                        }
                    }
                    else
                    {
                        sb.Append($", {editor.LastName}");
                        if (!string.IsNullOrEmpty(editor.FirstName) && editor.FirstName.Length > 0)
                        {
                            sb.Append($", {editor.FirstName[0]}.");
                            if (!string.IsNullOrEmpty(editor.MiddleName) && editor.MiddleName.Length > 0)
                            {
                                sb.Append($"{editor.MiddleName[0]}.");
                            }
                        }
                    }
                }
                sb.Append(" (eds.)");
            }
            else
            {
                // 4+ editors: first editor + et al.
                var editor = entry.Editors[0];
                sb.Append(editor.LastName);
                if (!string.IsNullOrEmpty(editor.FirstName) && editor.FirstName.Length > 0)
                {
                    sb.Append($", {editor.FirstName[0]}.");
                    if (!string.IsNullOrEmpty(editor.MiddleName) && editor.MiddleName.Length > 0)
                    {
                        sb.Append($"{editor.MiddleName[0]}.");
                    }
                }
                sb.Append(" et al. (eds.)");
            }
        }
        else
        {
            // No authors or editors, use title
            sb.Append(entry.Title);
        }

        // Year
        if (entry.Year.HasValue)
        {
            sb.Append($" ({entry.Year})");
        }

        // Title (different formatting based on entry type)
        if (!string.IsNullOrEmpty(entry.Title))
        {
            sb.Append($" {entry.Title}");

            if (entry.EntryType == EntryType.Article)
            {
                // No special formatting for article titles
                sb.Append(",");
            }
            else if (entry.EntryType == EntryType.Book ||
                     entry.EntryType == EntryType.Proceedings)
            {
                // Book/proceedings title in italics
                sb.Append(",");

                // Edition
                if (!string.IsNullOrEmpty(entry.Edition))
                {
                    sb.Append($" {entry.Edition} edn.,");
                }
            }
            else if (entry.EntryType == EntryType.InProceedings ||
                     entry.EntryType == EntryType.InCollection)
            {
                // Chapter title
                sb.Append(", in");

                // Book title
                if (!string.IsNullOrEmpty(entry.BookTitle))
                {
                    // Editors (if not already used as primary authors)
                    if (entry.Authors.Count > 0 && entry.Editors.Count > 0)
                    {
                        sb.Append(" ");
                        for (int i = 0; i < entry.Editors.Count; i++)
                        {
                            var editor = entry.Editors[i];
                            if (i == 0)
                            {
                                if (!string.IsNullOrEmpty(editor.FirstName) && editor.FirstName.Length > 0)
                                {
                                    sb.Append($"{editor.FirstName[0]}.");
                                    if (!string.IsNullOrEmpty(editor.MiddleName) && editor.MiddleName.Length > 0)
                                    {
                                        sb.Append($"{editor.MiddleName[0]}.");
                                    }
                                    sb.Append($" {editor.LastName}");
                                }
                                else
                                {
                                    sb.Append(editor.LastName);
                                }
                            }
                            else if (i == entry.Editors.Count - 1)
                            {
                                if (!string.IsNullOrEmpty(editor.FirstName) && editor.FirstName.Length > 0)
                                {
                                    sb.Append($" and {editor.FirstName[0]}.");
                                    if (!string.IsNullOrEmpty(editor.MiddleName) && editor.MiddleName.Length > 0)
                                    {
                                        sb.Append($"{editor.MiddleName[0]}.");
                                    }
                                    sb.Append($" {editor.LastName}");
                                }
                                else
                                {
                                    sb.Append($" and {editor.LastName}");
                                }
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(editor.FirstName) && editor.FirstName.Length > 0)
                                {
                                    sb.Append($", {editor.FirstName[0]}.");
                                    if (!string.IsNullOrEmpty(editor.MiddleName) && editor.MiddleName.Length > 0)
                                    {
                                        sb.Append($"{editor.MiddleName[0]}.");
                                    }
                                    sb.Append($" {editor.LastName}");
                                }
                                else
                                {
                                    sb.Append($", {editor.LastName}");
                                }
                            }
                        }

                        if (entry.Editors.Count == 1)
                        {
                            sb.Append(" (ed.)");
                        }
                        else
                        {
                            sb.Append(" (eds.)");
                        }
                        sb.Append(",");
                    }

                    sb.Append($" {entry.BookTitle}");

                    // Edition
                    if (!string.IsNullOrEmpty(entry.Edition))
                    {
                        sb.Append($", {entry.Edition} edn.,");
                    }
                    else
                    {
                        sb.Append(",");
                    }
                }
            }
            else
            {
                // Default: no special formatting
                sb.Append(",");
            }
        }

        // Journal name for articles
        if (entry.EntryType == EntryType.Article &&
            !string.IsNullOrEmpty(entry.Journal))
        {
            sb.Append($" <i>{entry.Journal}</i>");

            // Volume and issue
            if (entry.Volume.HasValue)
            {
                sb.Append($", vol. {entry.Volume}");
                if (!string.IsNullOrEmpty(entry.Number))
                {
                    sb.Append($", no. {entry.Number}");
                }
            }
        }

        // Publisher info for books
        if ((entry.EntryType == EntryType.Book ||
             entry.EntryType == EntryType.InProceedings ||
             entry.EntryType == EntryType.InCollection) &&
            !string.IsNullOrEmpty(entry.Publisher))
        {
            // Publisher
            sb.Append($" {entry.Publisher}");

            // Place of publication
            if (!string.IsNullOrEmpty(entry.Address))
            {
                sb.Append($", {entry.Address}");
            }
        }

        // Pages for articles and book chapters
        if (entry.Pages != null)
        {
            if (entry.EntryType == EntryType.Article)
            {
                sb.Append($", pp. {entry.Pages}");
            }
            else if (entry.EntryType == EntryType.InProceedings ||
                     entry.EntryType == EntryType.InCollection)
            {
                sb.Append($", pp. {entry.Pages}");
            }
        }

        // DOI
        if (!string.IsNullOrEmpty(entry.Doi))
        {
            sb.Append($", DOI: {entry.Doi}");
        }

        // URL (note: access date would typically be tracked separately in a real application)
        if (!string.IsNullOrEmpty(entry.Url))
        {
            sb.Append($", Available at: {entry.Url}");
        }

        // End with period
        if (sb.Length > 0 && sb[sb.Length - 1] != '.')
        {
            sb.Append('.');
        }

        return sb.ToString();
    }

    /// <summary>
    /// Formats a BibEntry in IEEE style.
    /// </summary>
    /// <param name="entry">The BibEntry to format.</param>
    /// <returns>An IEEE-style formatted citation string.</returns>
    private static string FormatIeee(BibEntry entry)
    {
        var sb = new StringBuilder();

        // Format authors - IEEE uses initials first, then last name
        if (entry.Authors.Count > 0)
        {
            for (int i = 0; i < entry.Authors.Count; i++)
            {
                if (i > 0)
                {
                    sb.Append(", ");
                }

                var author = entry.Authors[i];

                if (!string.IsNullOrEmpty(author.FirstName))
                {
                    sb.Append(author.FirstName[0] + ". ");
                }

                if (!string.IsNullOrEmpty(author.MiddleName))
                {
                    sb.Append(author.MiddleName[0] + ". ");
                }

                sb.Append(author.LastName);

                // IEEE uses et al. after 6 authors
                if (i == ieeePositionBeforeEtAl && entry.Authors.Count > ieeeMaxAuthorsBeforeEtAl)
                {
                    sb.Append(" et al");
                    break;
                }
            }
        }
        else if (entry.Editors.Count > 0)
        {
            // Use editors if no authors
            for (int i = 0; i < entry.Editors.Count; i++)
            {
                if (i > 0)
                {
                    sb.Append(", ");
                }

                var editor = entry.Editors[i];

                if (!string.IsNullOrEmpty(editor.FirstName))
                {
                    sb.Append(editor.FirstName[0] + ". ");
                }

                if (!string.IsNullOrEmpty(editor.MiddleName))
                {
                    sb.Append(editor.MiddleName[0] + ". ");
                }

                sb.Append(editor.LastName);

                // IEEE uses et al. after 6 editors
                if (i == ieeePositionBeforeEtAl && entry.Editors.Count > ieeeMaxAuthorsBeforeEtAl)
                {
                    sb.Append(" et al");
                    break;
                }
            }

            // Add (Ed.) or (Eds.) for IEEE
            if (entry.Editors.Count == 1)
            {
                sb.Append(", Ed.");
            }
            else
            {
                sb.Append(", Eds.");
            }
        }
        else
        {
            // No authors or editors, use title
            sb.Append(entry.Title);
        }

        sb.Append(", ");

        // Title in quotation marks for articles and chapters, italics for books
        if (!string.IsNullOrEmpty(entry.Title))
        {
            if (entry.EntryType == EntryType.Article ||
                entry.EntryType == EntryType.InProceedings ||
                entry.EntryType == EntryType.InCollection)
            {
                sb.Append($"\"{entry.Title},\" ");
            }
            else
            {
                sb.Append($"<i>{entry.Title}</i>, ");
            }
        }

        // Container (journal, book, proceedings)
        if (entry.EntryType == EntryType.Article &&
            !string.IsNullOrEmpty(entry.Journal))
        {
            sb.Append($"<i>{entry.Journal}</i>, ");
        }
        else if ((entry.EntryType == EntryType.InProceedings ||
                  entry.EntryType == EntryType.InCollection) &&
                 !string.IsNullOrEmpty(entry.BookTitle))
        {
            sb.Append($"in <i>{entry.BookTitle}</i>, ");

            // Editors for book chapters (if not already used as primary authors)
            if (entry.Authors.Count > 0 && entry.Editors.Count > 0)
            {
                sb.Append("Ed. ");
                if (entry.Editors.Count > 1)
                {
                    sb.Append("Eds. ");
                }

                for (int i = 0; i < entry.Editors.Count; i++)
                {
                    if (i > 0)
                    {
                        sb.Append(", ");
                    }

                    var editor = entry.Editors[i];

                    if (!string.IsNullOrEmpty(editor.FirstName))
                    {
                        sb.Append(editor.FirstName[0] + ". ");
                    }

                    if (!string.IsNullOrEmpty(editor.MiddleName))
                    {
                        sb.Append(editor.MiddleName[0] + ". ");
                    }

                    sb.Append(editor.LastName);

                    // IEEE uses et al. after 6 editors
                    if (i == ieeePositionBeforeEtAl && entry.Editors.Count > ieeeMaxAuthorsBeforeEtAl)
                    {
                        sb.Append(" et al");
                        break;
                    }
                }

                sb.Append(", ");
            }
        }

        // Edition for books
        if (!string.IsNullOrEmpty(entry.Edition) &&
            (entry.EntryType == EntryType.Book ||
             entry.EntryType == EntryType.InProceedings ||
             entry.EntryType == EntryType.InCollection))
        {
            sb.Append($"{entry.Edition} ed., ");
        }

        // Publisher and address
        if (!string.IsNullOrEmpty(entry.Publisher))
        {
            sb.Append($"{entry.Publisher}");

            if (!string.IsNullOrEmpty(entry.Address))
            {
                sb.Append($", {entry.Address}");
            }

            sb.Append(", ");
        }

        // Volume, number for articles
        if (entry.EntryType == EntryType.Article)
        {
            if (entry.Volume.HasValue)
            {
                sb.Append($"vol. {entry.Volume}");

                if (!string.IsNullOrEmpty(entry.Number))
                {
                    sb.Append($", no. {entry.Number}");
                }

                sb.Append(", ");
            }
        }

        // Pages
        if (entry.Pages != null)
        {
            sb.Append($"pp. {entry.Pages}, ");
        }

        // Year
        if (entry.Year.HasValue)
        {
            sb.Append(entry.Year);
        }

        // DOI
        if (!string.IsNullOrEmpty(entry.Doi))
        {
            sb.Append($", doi: {entry.Doi}");
        }

        // URL
        if (!string.IsNullOrEmpty(entry.Url))
        {
            sb.Append($", {entry.Url}");
        }

        // End with period
        if (sb.Length > 0 && sb[sb.Length - 1] != '.')
        {
            sb.Append('.');
        }

        return sb.ToString();
    }
}
