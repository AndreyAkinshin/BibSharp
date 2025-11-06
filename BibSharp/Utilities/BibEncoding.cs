namespace BibSharp.Utilities;

/// <summary>
/// Defines the encoding options for BibTeX special characters.
/// </summary>
public enum BibEncoding
{
    /// <summary>
    /// Use Unicode characters directly in the output.
    /// </summary>
    Unicode,

    /// <summary>
    /// Use LaTeX escape sequences for special characters (e.g., \"{o} for ö).
    /// </summary>
    LaTeX
}

/// <summary>
/// Defines the line ending options for serialized BibTeX output.
/// </summary>
public enum LineEnding
{
    /// <summary>
    /// Use the system's default line ending.
    /// </summary>
    SystemDefault,

    /// <summary>
    /// Use LF (Unix-style) line endings.
    /// </summary>
    Lf,

    /// <summary>
    /// Use CRLF (Windows-style) line endings.
    /// </summary>
    Crlf
}
