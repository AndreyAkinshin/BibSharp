using System;

namespace BibSharp;

/// <summary>
/// Represents a BibTeX entry type (e.g., article, book, inproceedings).
/// Supports both predefined standard types and custom types.
/// </summary>
public readonly struct EntryType : IEquatable<EntryType>
{
    private readonly string value;

    /// <summary>
    /// Gets the string value of the entry type.
    /// </summary>
    public string Value => value ?? Misc.value;

    /// <summary>
    /// Initializes a new instance of the <see cref="EntryType"/> struct.
    /// </summary>
    /// <param name="value">The entry type value.</param>
    public EntryType(string value)
    {
        this.value = value?.ToLowerInvariant() ?? throw new ArgumentNullException(nameof(value));
    }

    // Standard BibTeX entry types
    /// <summary>Gets the entry type for journal articles.</summary>
    public static readonly EntryType Article = new("article");
    /// <summary>Gets the entry type for books.</summary>
    public static readonly EntryType Book = new("book");
    /// <summary>Gets the entry type for conference papers.</summary>
    public static readonly EntryType InProceedings = new("inproceedings");
    /// <summary>Gets the entry type for conference papers (alias for InProceedings).</summary>
    public static readonly EntryType Conference = new("conference");
    /// <summary>Gets the entry type for book chapters or sections.</summary>
    public static readonly EntryType InCollection = new("incollection");
    /// <summary>Gets the entry type for parts of a book.</summary>
    public static readonly EntryType InBook = new("inbook");
    /// <summary>Gets the entry type for printed works without a publisher.</summary>
    public static readonly EntryType Booklet = new("booklet");
    /// <summary>Gets the entry type for PhD dissertations.</summary>
    public static readonly EntryType PhdThesis = new("phdthesis");
    /// <summary>Gets the entry type for Master's theses.</summary>
    public static readonly EntryType MastersThesis = new("mastersthesis");
    /// <summary>Gets the entry type for technical reports.</summary>
    public static readonly EntryType TechReport = new("techreport");
    /// <summary>Gets the entry type for technical manuals.</summary>
    public static readonly EntryType Manual = new("manual");
    /// <summary>Gets the entry type for conference proceedings.</summary>
    public static readonly EntryType Proceedings = new("proceedings");
    /// <summary>Gets the entry type for unpublished works.</summary>
    public static readonly EntryType Unpublished = new("unpublished");
    /// <summary>Gets the entry type for miscellaneous entries.</summary>
    public static readonly EntryType Misc = new("misc");

    /// <summary>
    /// Implicitly converts a string to an EntryType.
    /// </summary>
    public static implicit operator EntryType(string value) => new(value);

    /// <summary>
    /// Implicitly converts an EntryType to a string.
    /// </summary>
    public static implicit operator string(EntryType type) => type.Value;

    /// <inheritdoc/>
    public bool Equals(EntryType other) =>
        string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is EntryType other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(Value);

    /// <inheritdoc/>
    public override string ToString() => Value;

    /// <summary>Determines whether two entry types are equal.</summary>
    public static bool operator ==(EntryType left, EntryType right) => left.Equals(right);
    /// <summary>Determines whether two entry types are not equal.</summary>
    public static bool operator !=(EntryType left, EntryType right) => !left.Equals(right);
}

