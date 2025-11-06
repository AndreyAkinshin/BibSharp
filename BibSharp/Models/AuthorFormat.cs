namespace BibSharp.Models;

/// <summary>
/// Defines the formatting options for author names in BibTeX.
/// </summary>
public enum AuthorFormat
{
    /// <summary>
    /// Format authors as "LastName, FirstName MiddleName" (e.g., "Akinshin, Andrey").
    /// This is the traditional BibTeX format.
    /// </summary>
    LastNameFirst,
    
    /// <summary>
    /// Format authors as "FirstName MiddleName LastName" (e.g., "Andrey Akinshin").
    /// </summary>
    FirstNameFirst
}