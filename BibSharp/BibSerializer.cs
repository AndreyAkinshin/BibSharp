using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BibSharp.Exceptions;
using BibSharp.Models;
using BibSharp.Utilities;

namespace BibSharp;

/// <summary>
/// Serializes BibTeX entries to BibTeX format.
/// </summary>
public class BibSerializer
{
    private readonly BibSerializerSettings settings;
    private readonly Dictionary<string, string> stringMacros = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Initializes a new instance of the <see cref="BibSerializer"/> class with default settings.
    /// </summary>
    public BibSerializer() : this(new BibSerializerSettings())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BibSerializer"/> class with the specified settings.
    /// </summary>
    /// <param name="settings">The serializer settings.</param>
    /// <exception cref="InvalidOperationException">Thrown when the settings configuration is invalid.</exception>
    public BibSerializer(BibSerializerSettings settings)
    {
        this.settings = settings ?? new BibSerializerSettings();
        this.settings.Validate();
    }

    /// <summary>
    /// Adds a string macro definition to the serializer.
    /// </summary>
    /// <param name="name">The macro name.</param>
    /// <param name="value">The macro value.</param>
    public void AddStringMacro(string name, string value)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException("Macro name cannot be null or empty", nameof(name));
        }

        stringMacros[name] = value;
    }

    /// <summary>
    /// Serializes a collection of BibTeX entries to a string.
    /// </summary>
    /// <param name="entries">The entries to serialize.</param>
    /// <returns>A string containing the serialized entries.</returns>
    public string Serialize(IEnumerable<BibEntry> entries)
    {
        if (entries == null)
        {
            throw new ArgumentNullException(nameof(entries));
        }

        var sb = new StringBuilder();
        
        // First, write string macros
        foreach (var macro in stringMacros)
        {
            sb.Append($"@string{{{macro.Key} = ");
            
            if (settings.UseBracesForFieldValues)
            {
                sb.Append($"{{{macro.Value}}}");
            }
            else
            {
                sb.Append($"\"{macro.Value}\"");
            }
            
            sb.AppendLine("}");
            sb.AppendLine();
        }

        // Then, write entries
        foreach (var entry in entries)
        {
            if (settings.ValidateBeforeSerialization)
            {
                var validationResult = entry.Validate();
                if (validationResult.HasErrors)
                {
                    throw new BibValidationException("Entry failed validation", entry.Key, validationResult.Errors);
                }
            }

            sb.Append(SerializeEntry(entry));
            sb.AppendLine();
        }

        return sb.ToString();
    }

    /// <summary>
    /// Serializes a collection of BibTeX entries to a file.
    /// </summary>
    /// <param name="entries">The entries to serialize.</param>
    /// <param name="filePath">The path to the file to write.</param>
    public void SerializeToFile(IEnumerable<BibEntry> entries, string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            throw new ArgumentException("File path cannot be null or empty", nameof(filePath));
        }

        string content = Serialize(entries);
        File.WriteAllText(filePath, content, Encoding.UTF8);
    }

    /// <summary>
    /// Serializes a collection of BibTeX entries to a file asynchronously.
    /// </summary>
    /// <param name="entries">The entries to serialize.</param>
    /// <param name="filePath">The path to the file to write.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task SerializeToFileAsync(IEnumerable<BibEntry> entries, string filePath, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            throw new ArgumentException("File path cannot be null or empty", nameof(filePath));
        }

        string content = Serialize(entries);
        await File.WriteAllTextAsync(filePath, content, Encoding.UTF8, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Serializes a single BibTeX entry to a string.
    /// </summary>
    /// <param name="entry">The entry to serialize.</param>
    /// <returns>A string containing the serialized entry.</returns>
    public string SerializeEntry(BibEntry entry)
    {
        if (entry == null)
        {
            throw new ArgumentNullException(nameof(entry));
        }

        var sb = new StringBuilder();
        string lineEnding = GetLineEnding();

        // Write entry type and key
        sb.Append($"@{entry.EntryType}{{{entry.Key},");
        sb.Append(lineEnding);

        // Get all fields
        var fields = entry.GetFields();
        var fieldNames = new List<string>(entry.GetFieldNames());

        // Pre-compute field order indices for O(1) lookup (fixes performance issue)
        Dictionary<string, int>? fieldOrderMap = null;
        if (settings.FieldOrder != null)
        {
            fieldOrderMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < settings.FieldOrder.Length; i++)
            {
                fieldOrderMap[settings.FieldOrder[i]] = i;
            }
            
            fieldNames = fieldNames
                .OrderBy(f => fieldOrderMap.TryGetValue(f, out int index) ? index : int.MaxValue)
                .ToList();
        }

        // Write fields with formatted authors/editors
        foreach (string fieldName in fieldNames)
        {
            if (fields.TryGetValue(fieldName, out string? value) && !string.IsNullOrEmpty(value))
            {
                // Format author/editor fields according to settings without mutating the entry
                string formattedValue = value;
                if (string.Equals(fieldName, FieldName.Author, StringComparison.OrdinalIgnoreCase))
                {
                    formattedValue = FormatContributorField(entry.Authors, settings.AuthorFormat);
                }
                else if (string.Equals(fieldName, FieldName.Editor, StringComparison.OrdinalIgnoreCase))
                {
                    formattedValue = FormatContributorField(entry.Editors, settings.AuthorFormat);
                }
                
                sb.Append(settings.Indent);
                sb.Append(fieldName);
                sb.Append(" = ");

                // Format value based on settings
                if (settings.UseBracesForFieldValues)
                {
                    sb.Append('{');
                    sb.Append(FormatFieldValue(formattedValue));
                    sb.Append('}');
                }
                else
                {
                    sb.Append('"');
                    sb.Append(FormatFieldValue(formattedValue));
                    sb.Append('"');
                }

                sb.Append(',');
                sb.Append(lineEnding);
            }
        }

        // Remove the last comma if there are fields
        // The StringBuilder ends with: fieldValue},\n (or \r\n)
        int lineEndingLength = lineEnding.Length;
        if (fields.Count > 0 && sb.Length >= 1 + lineEndingLength)
        {
            // Check if we actually have a comma to remove
            int commaPosition = sb.Length - lineEndingLength - 1;
            if (commaPosition >= 0 && sb[commaPosition] == ',')
            {
                sb.Length -= 1 + lineEndingLength; // Remove last comma and line ending
                sb.Append(lineEnding); // Add back the line ending without the comma
            }
        }

        sb.Append('}');
        sb.Append(lineEnding);

        return sb.ToString();
    }

    private string FormatFieldValue(string value)
    {
        // Handle special characters based on encoding setting
        if (settings.Encoding == BibEncoding.LaTeX && BibTexEncoder.ContainsSpecialCharacters(value))
        {
            // Use the BibTexEncoder to properly format special characters
            value = BibTexEncoder.ToLatex(value);
        }

        // Wrap long fields if enabled
        if (settings.WrapLongFields && value.Length > settings.MaxFieldLength)
        {
            return WrapLongField(value);
        }

        return value;
    }

    private string WrapLongField(string value)
    {
        var sb = new StringBuilder();
        int pos = 0;
        string lineEnding = GetLineEnding();

        while (pos < value.Length)
        {
            int chunkSize = Math.Min(settings.MaxFieldLength, value.Length - pos);
            
            // Don't break in the middle of a word or special character
            if (pos + chunkSize < value.Length && !char.IsWhiteSpace(value[pos + chunkSize]))
            {
                int lastSpace = value.LastIndexOf(' ', pos + chunkSize, chunkSize);
                if (lastSpace > pos)
                {
                    chunkSize = lastSpace - pos + 1;
                }
            }

            sb.Append(value.Substring(pos, chunkSize));
            pos += chunkSize;

            if (pos < value.Length)
            {
                sb.Append(lineEnding);
                sb.Append(settings.Indent);
            }
        }

        return sb.ToString();
    }

    private string GetLineEnding()
    {
        return settings.LineEnding switch
        {
            LineEnding.Lf => "\n",
            LineEnding.Crlf => "\r\n",
            _ => Environment.NewLine
        };
    }

    /// <summary>
    /// Formats a list of contributors (authors or editors) according to the specified format.
    /// This method does not mutate the entry, unlike UpdateAuthorFields.
    /// </summary>
    /// <param name="contributors">The list of contributors to format.</param>
    /// <param name="format">The author format to use.</param>
    /// <returns>A formatted string representation of the contributors.</returns>
    private static string FormatContributorField(IReadOnlyList<Author> contributors, AuthorFormat format)
    {
        if (contributors == null || contributors.Count == 0)
        {
            return string.Empty;
        }

        return string.Join(" and ", contributors.Select(c => c.ToBibTeXString(format)));
    }
}