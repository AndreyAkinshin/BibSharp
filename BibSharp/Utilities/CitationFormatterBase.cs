using System.Text;
using BibSharp.Models;

namespace BibSharp.Utilities;

/// <summary>
/// Base class for citation formatters providing common helper methods.
/// </summary>
public abstract class CitationFormatterBase : ICitationFormatter
{
    /// <summary>
    /// Formats a BibEntry into a citation string according to the specific citation style.
    /// </summary>
    /// <param name="entry">The BibEntry to format.</param>
    /// <returns>A formatted citation string.</returns>
    public abstract string Format(BibEntry entry);

    /// <summary>
    /// Formats author name with initials for first and middle names.
    /// </summary>
    /// <param name="author">The author to format.</param>
    /// <param name="firstNameFirst">Whether to format as "F. M. LastName" (true) or "LastName, F. M." (false).</param>
    /// <returns>The formatted author name.</returns>
    protected static string FormatAuthorWithInitials(Author author, bool firstNameFirst = false)
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
    /// <param name="author">The author to format.</param>
    /// <param name="includeMiddle">Whether to include the middle name.</param>
    /// <returns>The formatted author name.</returns>
    protected static string FormatAuthorFull(Author author, bool includeMiddle = true)
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
    /// <param name="author">The author to format.</param>
    /// <param name="includeMiddle">Whether to include the middle name.</param>
    /// <returns>The formatted author name.</returns>
    protected static string FormatAuthorDisplay(Author author, bool includeMiddle = true)
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
    /// Ensures a StringBuilder ends with a period if it doesn't end with punctuation.
    /// </summary>
    /// <param name="sb">The StringBuilder to check.</param>
    protected static void EnsureEndsPeriod(StringBuilder sb)
    {
        if (sb.Length > 0 && sb[sb.Length - 1] != '.')
        {
            sb.Append('.');
        }
    }
}

