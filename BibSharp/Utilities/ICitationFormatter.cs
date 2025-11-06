namespace BibSharp.Utilities;

/// <summary>
/// Interface for citation formatters that convert BibEntry objects to formatted citation strings.
/// </summary>
public interface ICitationFormatter
{
    /// <summary>
    /// Formats a BibEntry into a citation string according to the specific citation style.
    /// </summary>
    /// <param name="entry">The BibEntry to format.</param>
    /// <returns>A formatted citation string.</returns>
    string Format(BibEntry entry);
}

