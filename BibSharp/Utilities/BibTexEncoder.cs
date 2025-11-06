using System.Collections.Generic;
using System.Text;

namespace BibSharp.Utilities;

/// <summary>
/// Provides methods for encoding and decoding strings between Unicode and LaTeX formats.
/// </summary>
public static class BibTexEncoder
{
    // Dictionary mapping special Unicode characters to their LaTeX escape sequences
    private static readonly Dictionary<char, string> unicodeToLatex = new()
    {
        // German umlauts
        { 'ä', "\\\"a" },
        { 'ö', "\\\"o" },
        { 'ü', "\\\"u" },
        { 'Ä', "\\\"A" },
        { 'Ö', "\\\"O" },
        { 'Ü', "\\\"U" },
        { 'ß', "\\ss{}" },

        // French accents
        { 'é', "\\'e" },
        { 'è', "\\`e" },
        { 'ê', "\\^e" },
        { 'ë', "\\\"e" },
        { 'á', "\\'a" },
        { 'à', "\\`a" },
        { 'â', "\\^a" },
        { 'í', "\\'i" },
        { 'ì', "\\`i" },
        { 'î', "\\^i" },
        { 'ï', "\\\"i" },
        { 'ó', "\\'o" },
        { 'ò', "\\`o" },
        { 'ô', "\\^o" },
        { 'ú', "\\'u" },
        { 'ù', "\\`u" },
        { 'û', "\\^u" },

        // Spanish and Portuguese characters
        { 'ñ', "\\~n" },
        { 'Ñ', "\\~N" },
        { 'ç', "\\c{c}" },
        { 'Ç', "\\c{C}" },
        { '¿', "?`" },
        { '¡', "!`" },

        // Scandinavian characters
        { 'å', "\\aa{}" },
        { 'Å', "\\AA{}" },
        { 'ø', "\\o{}" },
        { 'Ø', "\\O{}" },
        { 'æ', "\\ae{}" },
        { 'Æ', "\\AE{}" },

        // Eastern European characters
        { 'ą', "\\k{a}" },
        { 'Ą', "\\k{A}" },
        { 'ć', "\\'c" },
        { 'Ć', "\\'C" },
        { 'ę', "\\k{e}" },
        { 'Ę', "\\k{E}" },
        { 'ł', "\\l{}" },
        { 'Ł', "\\L{}" },
        { 'ń', "\\'n" },
        { 'Ń', "\\'N" },
        { 'ś', "\\'s" },
        { 'Ś', "\\'S" },
        { 'ź', "\\'z" },
        { 'Ź', "\\'Z" },
        { 'ż', "\\.z" },
        { 'Ż', "\\.Z" },

        // Greek letters commonly used in scientific notation
        { 'α', "\\alpha" },
        { 'β', "\\beta" },
        { 'γ', "\\gamma" },
        { 'Γ', "\\Gamma" },
        { 'δ', "\\delta" },
        { 'Δ', "\\Delta" },
        { 'ε', "\\epsilon" },
        { 'ζ', "\\zeta" },
        { 'η', "\\eta" },
        { 'θ', "\\theta" },
        { 'Θ', "\\Theta" },
        { 'κ', "\\kappa" },
        { 'λ', "\\lambda" },
        { 'Λ', "\\Lambda" },
        { 'μ', "\\mu" },
        { 'ν', "\\nu" },
        { 'ξ', "\\xi" },
        { 'Ξ', "\\Xi" },
        { 'π', "\\pi" },
        { 'Π', "\\Pi" },
        { 'ρ', "\\rho" },
        { 'σ', "\\sigma" },
        { 'Σ', "\\Sigma" },
        { 'τ', "\\tau" },
        { 'υ', "\\upsilon" },
        { 'φ', "\\phi" },
        { 'Φ', "\\Phi" },
        { 'χ', "\\chi" },
        { 'ψ', "\\psi" },
        { 'Ψ', "\\Psi" },
        { 'ω', "\\omega" },
        { 'Ω', "\\Omega" },

        // Special symbols and punctuation
        { '°', "\\textdegree" },
        { '†', "\\dag" },
        { '‡', "\\ddag" },
        { '•', "\\bullet" },
        { '…', "\\ldots" },
        { '§', "\\S" },
        { '©', "\\copyright" },
        { '®', "\\textregistered" },
        { '™', "\\texttrademark" },
        { '£', "\\pounds" },
        { '€', "\\euro" },
        { '$', "\\$" },
        { '%', "\\%" },
        { '&', "\\&" },
        { '#', "\\#" },
        { '_', "\\_" },
        { '{', "\\{" },
        { '}', "\\}" },
        { '\\', "\\textbackslash{}" },
        { '~', "\\textasciitilde{}" },
        { '^', "\\textasciicircum{}" },

        // Quotes
        { '"', "\\textquotedbl{}" },
        { '\u2018', "\\textquoteleft{}" }, // Left single quote
        { '\u2019', "\\textquoteright{}" }, // Right single quote
        { '\u201C', "\\textquotedblleft{}" }, // Left double quote
        { '\u201D', "\\textquotedblright{}" }, // Right double quote

        // Mathematical symbols
        { '±', "\\pm" },
        { '×', "\\times" },
        { '÷', "\\div" },
        { '≤', "\\leq" },
        { '≥', "\\geq" },
        { '≠', "\\neq" },
        { '≈', "\\approx" },
        { '∞', "\\infty" },
    };

    // Dictionary for reverse lookup (LaTeX to Unicode)
    private static readonly Dictionary<string, char> latexToUnicode = new();

    // Regular expressions for more complex LaTeX command matching
    private static readonly Dictionary<string, string> latexCommands = new()
    {
        { "\\textbackslash", "\\" },
        { "\\textasciitilde", "~" },
        { "\\textasciicircum", "^" },
        { "\\ldots", "…" },
        { "\\textquotedblleft", "\u201C" },
        { "\\textquotedblright", "\u201D" },
        { "\\textquoteleft", "\u2018" },
        { "\\textquoteright", "\u2019" },
        { "\\textquotedbl", "\"" },
        { "\\textdegree", "°" },
    };

    // Static constructor to initialize the reverse lookup dictionary
    static BibTexEncoder()
    {
        foreach (var pair in unicodeToLatex)
        {
            if (!latexToUnicode.ContainsKey(pair.Value))
            {
                latexToUnicode[pair.Value] = pair.Key;
            }
        }
    }

    /// <summary>
    /// Converts a Unicode string to a LaTeX-encoded string.
    /// </summary>
    /// <param name="input">The Unicode string to convert.</param>
    /// <returns>A LaTeX-encoded string.</returns>
    public static string ToLatex(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        var sb = new StringBuilder(input.Length * 2);

        foreach (char c in input)
        {
            if (unicodeToLatex.TryGetValue(c, out string? latexCommand))
            {
                sb.Append(latexCommand);
            }
            else
            {
                sb.Append(c);
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// Attempts to convert LaTeX-encoded text to Unicode.
    /// Note: This is a best-effort conversion and may not handle all LaTeX commands.
    /// </summary>
    /// <param name="input">The LaTeX-encoded string to convert.</param>
    /// <returns>A Unicode string.</returns>
    public static string ToUnicode(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        // First replace the more complex commands
        foreach (var command in latexCommands)
        {
            input = input.Replace(command.Key, command.Value);
        }

        // Handle simpler accents
        input = HandleAccentCommands(input);

        return input;
    }

    /// <summary>
    /// Processes LaTeX accent commands like \"{a}, \'{e}, etc.
    /// </summary>
    /// <param name="input">The input string containing LaTeX accent commands.</param>
    /// <returns>The string with accent commands converted to Unicode.</returns>
    private static string HandleAccentCommands(string input)
    {
        var sb = new StringBuilder(input);

        // Process each known LaTeX command
        foreach (var command in latexToUnicode)
        {
            sb.Replace(command.Key, command.Value.ToString());
        }

        return sb.ToString();
    }

    /// <summary>
    /// Determines whether a string contains characters that would need LaTeX encoding.
    /// </summary>
    /// <param name="input">The string to check.</param>
    /// <returns>True if the string contains characters that would need LaTeX encoding; otherwise, false.</returns>
    public static bool ContainsSpecialCharacters(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return false;
        }

        foreach (char c in input)
        {
            if (unicodeToLatex.ContainsKey(c))
            {
                return true;
            }
        }

        return false;
    }
}

