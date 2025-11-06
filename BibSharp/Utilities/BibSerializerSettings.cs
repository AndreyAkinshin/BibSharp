using System;
using BibSharp.Models;

namespace BibSharp.Utilities;

/// <summary>
/// Settings to control the behavior of the BibTeX serializer.
/// </summary>
public class BibSerializerSettings
{
    /// <summary>
    /// Gets or sets the indentation string to use for fields within entries.
    /// </summary>
    public string Indent { get; set; } = "  ";
    
    /// <summary>
    /// Gets or sets the ordered list of field names to determine serialization order.
    /// Fields not in this list will be serialized in their original order after the ordered fields.
    /// </summary>
    public string[]? FieldOrder { get; set; }
    
    /// <summary>
    /// Gets or sets the encoding to use for special characters.
    /// </summary>
    public BibEncoding Encoding { get; set; } = BibEncoding.Unicode;
    
    /// <summary>
    /// Gets or sets the line ending to use in the serialized output.
    /// </summary>
    public LineEnding LineEnding { get; set; } = LineEnding.SystemDefault;
    
    /// <summary>
    /// Gets or sets a value indicating whether to validate entries before serialization.
    /// </summary>
    public bool ValidateBeforeSerialization { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to use braces {} instead of quotes "" for field values.
    /// </summary>
    public bool UseBracesForFieldValues { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to wrap long field values across multiple lines.
    /// </summary>
    public bool WrapLongFields { get; set; } = false;
    
    private int maxFieldLength = 80;
    
    /// <summary>
    /// Gets or sets the maximum length of a field value before wrapping (if WrapLongFields is true).
    /// Must be greater than 0.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when value is less than or equal to 0.</exception>
    public int MaxFieldLength
    {
        get => maxFieldLength;
        set
        {
            if (value <= 0)
            {
                throw new ArgumentException("MaxFieldLength must be greater than 0", nameof(MaxFieldLength));
            }
            maxFieldLength = value;
        }
    }
    
    /// <summary>
    /// Gets or sets the format to use for author names.
    /// </summary>
    public AuthorFormat AuthorFormat { get; set; } = AuthorFormat.LastNameFirst;

    /// <summary>
    /// Validates the current settings configuration.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the configuration is invalid.</exception>
    public void Validate()
    {
        if (WrapLongFields && MaxFieldLength <= 0)
        {
            throw new InvalidOperationException("MaxFieldLength must be greater than 0 when WrapLongFields is enabled");
        }

        if (Indent == null)
        {
            throw new InvalidOperationException("Indent cannot be null");
        }
    }
}