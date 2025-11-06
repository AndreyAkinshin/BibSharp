using System;

namespace BibSharp;

/// <summary>
/// Represents a BibTeX field name (e.g., author, title, year).
/// Supports both predefined standard fields and custom fields.
/// </summary>
public readonly struct FieldName : IEquatable<FieldName>
{
    private readonly string value;

    /// <summary>
    /// Gets the string value of the field name.
    /// </summary>
    public string Value => value ?? string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="FieldName"/> struct.
    /// </summary>
    /// <param name="value">The field name value.</param>
    public FieldName(string value)
    {
        this.value = value?.ToLowerInvariant() ?? throw new ArgumentNullException(nameof(value));
    }

    // Standard BibTeX fields
    /// <summary>Gets the field name for author(s).</summary>
    public static readonly FieldName Author = new("author");
    /// <summary>Gets the field name for the work's title.</summary>
    public static readonly FieldName Title = new("title");
    /// <summary>Gets the field name for the journal name.</summary>
    public static readonly FieldName Journal = new("journal");
    /// <summary>Gets the field name for the publication year.</summary>
    public static readonly FieldName Year = new("year");
    /// <summary>Gets the field name for the volume number.</summary>
    public static readonly FieldName Volume = new("volume");
    /// <summary>Gets the field name for the issue number.</summary>
    public static readonly FieldName Number = new("number");
    /// <summary>Gets the field name for page numbers.</summary>
    public static readonly FieldName Pages = new("pages");
    /// <summary>Gets the field name for the publication month.</summary>
    public static readonly FieldName Month = new("month");
    /// <summary>Gets the field name for the Digital Object Identifier.</summary>
    public static readonly FieldName Doi = new("doi");
    /// <summary>Gets the field name for additional notes.</summary>
    public static readonly FieldName Note = new("note");
    /// <summary>Gets the field name for the URL.</summary>
    public static readonly FieldName Url = new("url");
    /// <summary>Gets the field name for the International Standard Serial Number.</summary>
    public static readonly FieldName Issn = new("issn");
    /// <summary>Gets the field name for keywords.</summary>
    public static readonly FieldName Keywords = new("keywords");
    /// <summary>Gets the field name for the abstract.</summary>
    public static readonly FieldName Abstract = new("abstract");
    /// <summary>Gets the field name for the language.</summary>
    public static readonly FieldName Language = new("language");
    /// <summary>Gets the field name for copyright information.</summary>
    public static readonly FieldName Copyright = new("copyright");
    /// <summary>Gets the field name for the publisher.</summary>
    public static readonly FieldName Publisher = new("publisher");
    /// <summary>Gets the field name for editor(s).</summary>
    public static readonly FieldName Editor = new("editor");
    /// <summary>Gets the field name for the series name.</summary>
    public static readonly FieldName Series = new("series");
    /// <summary>Gets the field name for the publisher's address.</summary>
    public static readonly FieldName Address = new("address");
    /// <summary>Gets the field name for the edition.</summary>
    public static readonly FieldName Edition = new("edition");
    /// <summary>Gets the field name for the International Standard Book Number.</summary>
    public static readonly FieldName Isbn = new("isbn");
    /// <summary>Gets the field name for the book title (for chapters/sections).</summary>
    public static readonly FieldName BookTitle = new("booktitle");
    /// <summary>Gets the field name for the organizing institution.</summary>
    public static readonly FieldName Organization = new("organization");
    /// <summary>Gets the field name for the chapter number.</summary>
    public static readonly FieldName Chapter = new("chapter");
    /// <summary>Gets the field name for the publication type.</summary>
    public static readonly FieldName Type = new("type");
    /// <summary>Gets the field name for how the work was published.</summary>
    public static readonly FieldName HowPublished = new("howpublished");
    /// <summary>Gets the field name for the educational institution.</summary>
    public static readonly FieldName School = new("school");
    /// <summary>Gets the field name for the institution.</summary>
    public static readonly FieldName Institution = new("institution");
    /// <summary>Gets the field name for annotations.</summary>
    public static readonly FieldName Annote = new("annote");
    /// <summary>Gets the field name for arXiv identifier.</summary>
    public static readonly FieldName Arxiv = new("arxiv");

    /// <summary>
    /// Implicitly converts a string to a FieldName.
    /// </summary>
    public static implicit operator FieldName(string value) => new(value);

    /// <summary>
    /// Implicitly converts a FieldName to a string.
    /// </summary>
    public static implicit operator string(FieldName fieldName) => fieldName.Value;

    /// <inheritdoc/>
    public bool Equals(FieldName other) =>
        string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is FieldName other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(Value);

    /// <inheritdoc/>
    public override string ToString() => Value;

    /// <summary>Determines whether two field names are equal.</summary>
    public static bool operator ==(FieldName left, FieldName right) => left.Equals(right);
    /// <summary>Determines whether two field names are not equal.</summary>
    public static bool operator !=(FieldName left, FieldName right) => !left.Equals(right);
}

