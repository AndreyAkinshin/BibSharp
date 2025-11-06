using System;

namespace BibSharp;

/// <summary>
/// Represents a calendar month with its number, abbreviation, and full name.
/// Supports both predefined standard months and custom month representations.
/// </summary>
public readonly struct Month : IEquatable<Month>
{
    /// <summary>
    /// Gets the month number (1-12). Returns 0 for an uninitialized/default Month struct.
    /// </summary>
    public int Number { get; }

    /// <summary>
    /// Gets the abbreviated month name (e.g., "jan", "feb"). Returns empty string for an uninitialized/default Month struct.
    /// </summary>
    public string Abbreviation => abbreviation ?? string.Empty;

    /// <summary>
    /// Gets the full month name (e.g., "january", "february"). Returns empty string for an uninitialized/default Month struct.
    /// </summary>
    public string FullName => fullName ?? string.Empty;

    private readonly string? abbreviation;
    private readonly string? fullName;

    /// <summary>
    /// Initializes a new instance of the <see cref="Month"/> struct.
    /// </summary>
    /// <param name="number">The month number (1-12).</param>
    /// <param name="abbreviation">The abbreviated month name.</param>
    /// <param name="fullName">The full month name.</param>
    public Month(int number, string abbreviation, string fullName)
    {
        if (number < 1 || number > 12)
        {
            throw new ArgumentOutOfRangeException(nameof(number), "Month number must be between 1 and 12");
        }

        Number = number;
        this.abbreviation = abbreviation?.ToLowerInvariant() ?? throw new ArgumentNullException(nameof(abbreviation));
        this.fullName = fullName?.ToLowerInvariant() ?? throw new ArgumentNullException(nameof(fullName));
    }

    // Standard months
    /// <summary>Gets the month of January.</summary>
    public static readonly Month January = new(1, "jan", "january");
    /// <summary>Gets the month of February.</summary>
    public static readonly Month February = new(2, "feb", "february");
    /// <summary>Gets the month of March.</summary>
    public static readonly Month March = new(3, "mar", "march");
    /// <summary>Gets the month of April.</summary>
    public static readonly Month April = new(4, "apr", "april");
    /// <summary>Gets the month of May.</summary>
    public static readonly Month May = new(5, "may", "may");
    /// <summary>Gets the month of June.</summary>
    public static readonly Month June = new(6, "jun", "june");
    /// <summary>Gets the month of July.</summary>
    public static readonly Month July = new(7, "jul", "july");
    /// <summary>Gets the month of August.</summary>
    public static readonly Month August = new(8, "aug", "august");
    /// <summary>Gets the month of September.</summary>
    public static readonly Month September = new(9, "sep", "september");
    /// <summary>Gets the month of October.</summary>
    public static readonly Month October = new(10, "oct", "october");
    /// <summary>Gets the month of November.</summary>
    public static readonly Month November = new(11, "nov", "november");
    /// <summary>Gets the month of December.</summary>
    public static readonly Month December = new(12, "dec", "december");

    /// <summary>
    /// Gets a month by its number (1-12).
    /// </summary>
    /// <param name="monthNumber">The month number.</param>
    /// <returns>The corresponding Month.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when monthNumber is not between 1 and 12.</exception>
    public static Month FromNumber(int monthNumber)
    {
        return monthNumber switch
        {
            1 => January,
            2 => February,
            3 => March,
            4 => April,
            5 => May,
            6 => June,
            7 => July,
            8 => August,
            9 => September,
            10 => October,
            11 => November,
            12 => December,
            _ => throw new ArgumentOutOfRangeException(nameof(monthNumber), "Month number must be between 1 and 12")
        };
    }

    /// <summary>
    /// Tries to parse a month from a string (supports abbreviations and full names).
    /// </summary>
    /// <param name="value">The string value to parse.</param>
    /// <param name="month">The parsed month, if successful.</param>
    /// <returns>true if parsing succeeded; otherwise, false.</returns>
    public static bool TryParse(string value, out Month month)
    {
        month = default;
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var normalized = value.Trim().ToLowerInvariant();

        // Try to parse as number first
        if (int.TryParse(normalized, out int number) && number >= 1 && number <= 12)
        {
            month = FromNumber(number);
            return true;
        }

        // Try to match abbreviation or full name
        month = normalized switch
        {
            "jan" or "january" => January,
            "feb" or "february" => February,
            "mar" or "march" => March,
            "apr" or "april" => April,
            "may" => May,
            "jun" or "june" => June,
            "jul" or "july" => July,
            "aug" or "august" => August,
            "sep" or "september" => September,
            "oct" or "october" => October,
            "nov" or "november" => November,
            "dec" or "december" => December,
            _ => default
        };

        return month.Number > 0;
    }

    /// <summary>
    /// Implicitly converts a Month to its abbreviation string.
    /// </summary>
    public static implicit operator string(Month month) => month.Abbreviation;

    /// <summary>
    /// Implicitly converts a Month to its number.
    /// </summary>
    public static implicit operator int(Month month) => month.Number;

    /// <inheritdoc/>
    public bool Equals(Month other) => Number == other.Number;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is Month other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => Number.GetHashCode();

    /// <summary>
    /// Returns the abbreviated month name.
    /// </summary>
    public override string ToString() => Abbreviation;

    /// <summary>Determines whether two months are equal.</summary>
    public static bool operator ==(Month left, Month right) => left.Equals(right);
    /// <summary>Determines whether two months are not equal.</summary>
    public static bool operator !=(Month left, Month right) => !left.Equals(right);
}

