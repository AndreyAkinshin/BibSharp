namespace BibSharp.Utilities;

/// <summary>
/// Settings to control the behavior of the BibTeX parser.
/// </summary>
/// <example>
/// <code>
/// var settings = new BibParserSettings
/// {
///     StrictMode = true,
///     ConvertLatexToUnicode = true,
///     ExpandStringMacros = false
/// };
/// var parser = new BibParser(settings);
/// var entries = parser.ParseFile("references.bib");
/// </code>
/// </example>
public class BibParserSettings
{
    /// <summary>
    /// Gets or sets a value indicating whether to preserve comments found in the BibTeX file.
    /// </summary>
    public bool PreserveComments { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether the parser should operate in strict mode,
    /// where malformed entries will cause exceptions rather than being handled gracefully.
    /// </summary>
    public bool StrictMode { get; set; } = false;
    
    /// <summary>
    /// Gets or sets a value indicating whether to expand string macros in field values.
    /// </summary>
    public bool ExpandStringMacros { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to preserve the original field ordering from the BibTeX file.
    /// When false, fields will be sorted alphabetically.
    /// </summary>
    public bool PreserveFieldOrder { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to automatically detect and handle common errors in BibTeX files.
    /// When enabled, the parser will automatically fix common issues like:
    /// - Whitespace in entry keys
    /// - Invalid characters in keys (replaced with underscores)
    /// - Unclosed braces or quotes in field values
    /// </summary>
    public bool AutoCorrectCommonErrors { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to automatically convert LaTeX encoded special characters to Unicode.
    /// </summary>
    public bool ConvertLatexToUnicode { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to normalize author names during parsing.
    /// When true, all author names are reformatted to the standard "LastName, FirstName MiddleName" format
    /// regardless of how they appear in the source BibTeX file.
    /// </summary>
    public bool NormalizeAuthorNames { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to normalize month values during parsing.
    /// When true, month values are converted to their standard three-letter abbreviations
    /// (e.g., "January" -> "jan", "12" -> "dec").
    /// </summary>
    public bool NormalizeMonths { get; set; } = true;

    /// <summary>
    /// Validates the current settings configuration.
    /// </summary>
    /// <remarks>
    /// Currently, all settings have valid defaults and no constraints that would make them invalid.
    /// This method is provided for consistency with BibSerializerSettings and for future extensibility.
    /// </remarks>
    public void Validate()
    {
        // No validation constraints at this time
        // This method is reserved for future use if constraints are added
    }
}