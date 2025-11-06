using System;
using System.Text.RegularExpressions;

namespace BibSharp;

/// <summary>
/// Represents a Digital Object Identifier (DOI) with support for different formats.
/// Provides both the identifier (short form) and full URL representation.
/// </summary>
public readonly struct Doi : IEquatable<Doi>
{
    // DOI format validation regex: must start with "10." followed by 4+ digits, then "/" and any characters
    private static readonly Regex doiFormatRegex = new Regex(@"^10\.\d{4,}/\S+$", RegexOptions.Compiled, TimeSpan.FromMilliseconds(1000));
    
    private readonly string identifier;

    /// <summary>
    /// Gets the DOI identifier without any prefix (e.g., "10.1234/abcd.5678").
    /// </summary>
    public string Identifier => identifier ?? string.Empty;

    /// <summary>
    /// Gets the full DOI URL (e.g., "https://doi.org/10.1234/abcd.5678").
    /// </summary>
    public string Url => string.IsNullOrEmpty(identifier) ? string.Empty : $"https://doi.org/{identifier}";

    /// <summary>
    /// Initializes a new instance of the <see cref="Doi"/> struct.
    /// Accepts DOI in various formats: plain identifier, URL, or with doi: prefix.
    /// </summary>
    /// <param name="value">The DOI value in any supported format.</param>
    /// <exception cref="ArgumentNullException">Thrown when value is null or whitespace.</exception>
    /// <exception cref="ArgumentException">Thrown when the DOI format is invalid.</exception>
    public Doi(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentNullException(nameof(value), "DOI cannot be null or empty");
        }

        // Clean and normalize the DOI
        identifier = NormalizeDoi(value);
        
        // Validate the DOI format
        if (!doiFormatRegex.IsMatch(identifier))
        {
            throw new ArgumentException(
                $"Invalid DOI format: '{value}'. DOI must match the pattern '10.XXXX/...' where XXXX is at least 4 digits.",
                nameof(value));
        }
    }

    /// <summary>
    /// Tries to parse a DOI from a string.
    /// </summary>
    /// <param name="value">The string value to parse.</param>
    /// <param name="doi">The parsed DOI, if successful.</param>
    /// <returns>true if parsing succeeded; otherwise, false.</returns>
    public static bool TryParse(string? value, out Doi doi)
    {
        doi = default;
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        try
        {
            doi = new Doi(value);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Implicitly converts a Doi to its identifier string.
    /// </summary>
    public static implicit operator string(Doi doi) => doi.Identifier;

    /// <summary>
    /// Implicitly converts a string to a Doi.
    /// </summary>
    public static implicit operator Doi(string value) => new(value);

    /// <inheritdoc/>
    public bool Equals(Doi other) =>
        string.Equals(Identifier, other.Identifier, StringComparison.OrdinalIgnoreCase);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is Doi other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(Identifier);

    /// <summary>
    /// Returns the DOI identifier (without prefix).
    /// </summary>
    public override string ToString() => Identifier;

    /// <summary>Determines whether two DOIs are equal.</summary>
    public static bool operator ==(Doi left, Doi right) => left.Equals(right);
    /// <summary>Determines whether two DOIs are not equal.</summary>
    public static bool operator !=(Doi left, Doi right) => !left.Equals(right);

    /// <summary>
    /// Normalizes a DOI string by removing common prefixes (https://doi.org/, http://doi.org/, doi:).
    /// </summary>
    /// <param name="doi">The DOI string to normalize.</param>
    /// <returns>The normalized DOI identifier.</returns>
    /// <remarks>This is an internal helper method used by both Doi and DoiResolver classes.</remarks>
    internal static string NormalizeDoi(string doi)
    {
        doi = doi.Trim();

        // Remove URL prefixes (using safer string operations)
        if (doi.StartsWith("https://doi.org/", StringComparison.OrdinalIgnoreCase))
        {
            return doi.Substring("https://doi.org/".Length);
        }
        
        if (doi.StartsWith("http://doi.org/", StringComparison.OrdinalIgnoreCase))
        {
            return doi.Substring("http://doi.org/".Length);
        }

        // Remove doi: prefix
        if (doi.StartsWith("doi:", StringComparison.OrdinalIgnoreCase))
        {
            return doi.Substring("doi:".Length).TrimStart();
        }

        return doi;
    }
}

