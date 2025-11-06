using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using BibSharp.Exceptions;
using BibSharp.Models;
using BibSharp.Utilities;

namespace BibSharp;

/// <summary>
/// Parser for BibTeX (.bib) files.
/// </summary>
/// <remarks>
/// This class is not thread-safe. Do not reuse the same BibParser instance across multiple concurrent operations.
/// For concurrent parsing, create separate BibParser instances for each operation.
/// </remarks>
public class BibParser : IDisposable
{
    // Regex timeout set to 1000ms for better reliability with large/complex files
    private const int RegexTimeoutMilliseconds = 1000;
    
    private static readonly Regex entryTypeRegex = new Regex(@"@(\w+)\s*{", RegexOptions.Compiled, TimeSpan.FromMilliseconds(RegexTimeoutMilliseconds));
    private static readonly Regex stringMacroBracesRegex = new Regex(@"@string\s*{\s*(\w+)\s*=\s*(?:{([^}]*)})?\s*}", RegexOptions.Compiled, TimeSpan.FromMilliseconds(RegexTimeoutMilliseconds));
    private static readonly Regex stringMacroQuotesRegex = new Regex(@"@string\s*{\s*(\w+)\s*=\s*""([^""]*)""\s*}", RegexOptions.Compiled, TimeSpan.FromMilliseconds(RegexTimeoutMilliseconds));
    private static readonly Regex preambleRegex = new Regex(@"@preamble\s*{([^}]*)}", RegexOptions.Compiled, TimeSpan.FromMilliseconds(RegexTimeoutMilliseconds));
    
    private readonly BibParserSettings settings;
    private readonly Dictionary<string, string> stringMacros = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<string> comments = new();
    private readonly List<string> preambles = new();
    private bool expandMacros = true;
    private TextReader? reader;
    private bool ownsReader;
    private int currentLine;
    private int currentColumn;
    private bool disposed;

    /// <summary>
    /// Gets the collection of comments found during parsing.
    /// </summary>
    public IReadOnlyList<string> Comments => comments.AsReadOnly();

    /// <summary>
    /// Gets the collection of preambles found during parsing.
    /// </summary>
    public IReadOnlyList<string> Preambles => preambles.AsReadOnly();

    /// <summary>
    /// Gets the collection of string macros defined in the BibTeX file.
    /// </summary>
    public IReadOnlyDictionary<string, string> StringMacros => stringMacros;

    /// <summary>
    /// Initializes a new instance of the <see cref="BibParser"/> class with default settings.
    /// </summary>
    public BibParser() : this(new BibParserSettings())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BibParser"/> class with the specified settings.
    /// </summary>
    /// <param name="settings">The parser settings.</param>
    public BibParser(BibParserSettings settings)
    {
        this.settings = settings ?? new BibParserSettings();
        this.settings.Validate();
        expandMacros = this.settings.ExpandStringMacros;
    }

    /// <summary>
    /// Adds a string macro definition.
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
    /// Creates a BibParseException with line/column information.
    /// </summary>
    private BibParseException CreateParseException(string message)
    {
        return new BibParseException(message, currentLine, currentColumn);
    }
    
    /// <summary>
    /// Auto-corrects common errors in entry keys.
    /// </summary>
    private string AutoCorrectKey(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            return key;
        }
        
        // Remove invalid characters that commonly appear in keys
        key = key.Replace(" ", "").Replace("\t", "").Replace("\n", "").Replace("\r", "");
        
        // Replace common problematic characters
        key = key.Replace(":", "_").Replace("/", "_").Replace("\\", "_");
        
        return key;
    }
    
    /// <summary>
    /// Auto-corrects common errors in field values.
    /// </summary>
    private string AutoCorrectFieldValue(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }
        
        // Trim excessive whitespace
        value = value.Trim();
        
        // Fix common quote/brace mismatches (simple cases)
        if (value.StartsWith("{", StringComparison.Ordinal) && !value.EndsWith("}", StringComparison.Ordinal) && !value.Contains("}", StringComparison.Ordinal))
        {
            value += "}";
        }
        
        if (value.StartsWith("\"", StringComparison.Ordinal) && !value.EndsWith("\"", StringComparison.Ordinal) && !value.Contains("\"", StringComparison.Ordinal))
        {
            value += "\"";
        }
        
        return value;
    }
    
    /// <summary>
    /// Normalizes author names to a consistent format (LastName, FirstName).
    /// </summary>
    private string NormalizeAuthorField(string authorString)
    {
        if (string.IsNullOrEmpty(authorString))
        {
            return authorString;
        }
        
        try
        {
            // Parse and re-format author names to ensure consistent format
            string[] authorParts = authorString.Split(new[] { " and ", " AND " }, StringSplitOptions.None);
            var normalizedAuthors = new List<string>();
            
            foreach (string authorPart in authorParts)
            {
                var author = Author.FromFormattedString(authorPart.Trim());
                // Re-format to standard "LastName, FirstName MiddleName" format
                normalizedAuthors.Add(author.ToString());
            }
            
            return string.Join(" and ", normalizedAuthors);
        }
        catch
        {
            // If parsing fails, return original value
            return authorString;
        }
    }
    
    /// <summary>
    /// Normalizes month values to standard abbreviations (e.g., "January" -> "jan").
    /// </summary>
    private string NormalizeMonthField(string monthString)
    {
        if (string.IsNullOrEmpty(monthString))
        {
            return monthString;
        }
        
        // Try to parse the month and convert to abbreviation
        if (Month.TryParse(monthString, out var month))
        {
            return month.Abbreviation;
        }
        
        // If parsing fails, return original value
        return monthString;
    }

    /// <summary>
    /// Parses a BibTeX file.
    /// </summary>
    /// <param name="filePath">The path to the BibTeX file.</param>
    /// <returns>A collection of BibTeX entries.</returns>
    /// <exception cref="ArgumentException">Thrown when filePath is null or empty.</exception>
    /// <exception cref="FileNotFoundException">Thrown when the file does not exist.</exception>
    /// <exception cref="BibParseException">Thrown when the file contains invalid BibTeX and StrictMode is enabled.</exception>
    public IList<BibEntry> ParseFile(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            throw new ArgumentException("File path cannot be null or empty", nameof(filePath));
        }

        ownsReader = true;
        using FileStream fileStream = File.OpenRead(filePath);
        return Parse(fileStream);
    }

    /// <summary>
    /// Parses a BibTeX file asynchronously.
    /// </summary>
    /// <param name="filePath">The path to the BibTeX file.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A collection of BibTeX entries.</returns>
    public async Task<IList<BibEntry>> ParseFileAsync(string filePath, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            throw new ArgumentException("File path cannot be null or empty", nameof(filePath));
        }

        ownsReader = true;
        using FileStream fileStream = File.OpenRead(filePath);
        return await ParseAsync(fileStream, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Parses BibTeX entries from a stream.
    /// </summary>
    /// <param name="stream">The stream containing BibTeX content.</param>
    /// <returns>A collection of BibTeX entries.</returns>
    public IList<BibEntry> Parse(Stream stream)
    {
        if (stream == null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        ownsReader = true;
        using (reader = new StreamReader(stream, Encoding.UTF8, true, 4096, true))
        {
            return ParseInternal();
        }
    }

    /// <summary>
    /// Parses BibTeX entries from a stream asynchronously.
    /// </summary>
    /// <param name="stream">The stream containing BibTeX content.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A collection of BibTeX entries.</returns>
    public async Task<IList<BibEntry>> ParseAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        if (stream == null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        ownsReader = true;
        using (reader = new StreamReader(stream, Encoding.UTF8, true, 4096, true))
        {
            return await ParseInternalAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Parses BibTeX entries from a string.
    /// </summary>
    /// <param name="bibContent">The string containing BibTeX content.</param>
    /// <returns>A collection of BibTeX entries.</returns>
    /// <exception cref="ArgumentNullException">Thrown when bibContent is null.</exception>
    /// <exception cref="BibParseException">Thrown when the content contains invalid BibTeX and StrictMode is enabled.</exception>
    public IList<BibEntry> ParseString(string bibContent)
    {
        if (bibContent == null)
        {
            throw new ArgumentNullException(nameof(bibContent));
        }

        ownsReader = true;
        using (reader = new StringReader(bibContent))
        {
            return ParseInternal();
        }
    }

    /// <summary>
    /// Parses BibTeX entries from a TextReader.
    /// </summary>
    /// <param name="reader">The TextReader containing BibTeX content.</param>
    /// <returns>A collection of BibTeX entries.</returns>
    /// <remarks>
    /// Note: This method does not dispose the provided reader. The caller is responsible for disposing it.
    /// </remarks>
    public IList<BibEntry> Parse(TextReader reader)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        this.reader = reader;
        ownsReader = false;
        try
        {
            return ParseInternal();
        }
        finally
        {
            this.reader = null;
        }
    }

    /// <summary>
    /// Parses BibTeX entries from a TextReader asynchronously.
    /// </summary>
    /// <param name="reader">The TextReader containing BibTeX content.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A collection of BibTeX entries.</returns>
    /// <remarks>
    /// Note: This method does not dispose the provided reader. The caller is responsible for disposing it.
    /// </remarks>
    public async Task<IList<BibEntry>> ParseAsync(TextReader reader, CancellationToken cancellationToken = default)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        this.reader = reader;
        ownsReader = false;
        try
        {
            return await ParseInternalAsync(cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            this.reader = null;
        }
    }

    /// <summary>
    /// Asynchronously enumerates BibTeX entries from a file as a stream.
    /// </summary>
    /// <param name="filePath">The path to the BibTeX file.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>An asynchronous enumerable of BibTeX entries.</returns>
    /// <remarks>
    /// This method uses streaming to process large files without loading the entire file into memory.
    /// The reader is automatically disposed when enumeration completes or is cancelled.
    /// </remarks>
    public async IAsyncEnumerable<BibEntry> ParseFileStreamAsync(string filePath, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            throw new ArgumentException("File path cannot be null or empty", nameof(filePath));
        }

        using FileStream fileStream = File.OpenRead(filePath);
        using StreamReader reader = new(fileStream, Encoding.UTF8, true, 4096, true);

        this.reader = reader;
        this.ownsReader = false; // Don't dispose in Dispose() as using statement handles it
        currentLine = 1;
        currentColumn = 1;
        comments.Clear();
        preambles.Clear();

        try
        {
            await foreach (var entry in ParseInternalStreamingAsync(cancellationToken).ConfigureAwait(false))
            {
                yield return entry;
            }
        }
        finally
        {
            this.reader = null;
        }
    }

    /// <summary>
    /// Processes a single line of BibTeX content, updating parsing state.
    /// </summary>
    /// <param name="line">The line to process.</param>
    /// <param name="inEntry">Reference to whether we're currently inside an entry.</param>
    /// <param name="braceCount">Reference to the current brace nesting level.</param>
    /// <param name="entryBuilder">StringBuilder accumulating the current entry text.</param>
    /// <param name="entries">List to add completed entries to.</param>
    private void ProcessLine(string line, ref bool inEntry, ref int braceCount, StringBuilder entryBuilder, List<BibEntry> entries)
    {
        // Handle comments that start with %
        if (!inEntry && line.TrimStart().StartsWith("%", StringComparison.Ordinal))
        {
            if (settings.PreserveComments)
            {
                comments.Add(line);
            }
            return;
        }

        // Process each character in the line
        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];
            currentColumn = i + 1;

            if (!inEntry && c == '@')
            {
                // Start of a new entry
                inEntry = true;
                braceCount = 0;
                entryBuilder.Clear();
                entryBuilder.Append(c);
            }
            else if (inEntry)
            {
                entryBuilder.Append(c);

                if (c == '{')
                {
                    braceCount++;
                }
                else if (c == '}')
                {
                    braceCount--;

                    if (braceCount == 0)
                    {
                        // End of the entry, parse it
                        inEntry = false;
                        string entryText = entryBuilder.ToString();

                        try
                        {
                            ParseEntry(entryText, entries);
                        }
                        catch (BibParseException ex)
                        {
                            // In non-strict mode, continue parsing
                            if (settings.StrictMode)
                            {
                                throw;
                            }

                            // Add to comments if preserving
                            if (settings.PreserveComments)
                            {
                                comments.Add($"% Error parsing entry: {ex.Message}");
                                comments.Add(entryText);
                            }
                        }
                    }
                }
            }
        }
    }

    private IList<BibEntry> ParseInternal()
    {
        if (reader == null)
        {
            throw new InvalidOperationException("Reader is not initialized");
        }

        currentLine = 1;
        currentColumn = 1;
        comments.Clear();
        preambles.Clear();

        var entries = new List<BibEntry>();
        string? line;
        StringBuilder entryBuilder = new();
        bool inEntry = false;
        int braceCount = 0;

        while ((line = reader.ReadLine()) != null)
        {
            ProcessLine(line, ref inEntry, ref braceCount, entryBuilder, entries);
            currentLine++;
            currentColumn = 1;
        }

        // Handle unclosed entry
        if (inEntry && settings.StrictMode)
        {
            throw CreateParseException("Unclosed entry at end of file");
        }

        return entries;
    }

    private async Task<IList<BibEntry>> ParseInternalAsync(CancellationToken cancellationToken)
    {
        if (reader == null)
        {
            throw new InvalidOperationException("Reader is not initialized");
        }

        currentLine = 1;
        currentColumn = 1;
        comments.Clear();
        preambles.Clear();

        var entries = new List<BibEntry>();
        string? line;
        StringBuilder entryBuilder = new();
        bool inEntry = false;
        int braceCount = 0;

        while ((line = await reader.ReadLineAsync().ConfigureAwait(false)) != null)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ProcessLine(line, ref inEntry, ref braceCount, entryBuilder, entries);
            currentLine++;
            currentColumn = 1;
        }

        // Handle unclosed entry
        if (inEntry && settings.StrictMode)
        {
            throw CreateParseException("Unclosed entry at end of file");
        }

        return entries;
    }

    private async IAsyncEnumerable<BibEntry> ParseInternalStreamingAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        if (reader == null)
        {
            throw new InvalidOperationException("Reader is not initialized");
        }

        string? line;
        StringBuilder entryBuilder = new();
        bool inEntry = false;
        int braceCount = 0;

        while ((line = await reader.ReadLineAsync().ConfigureAwait(false)) != null)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Handle comments that start with %
            if (!inEntry && line.TrimStart().StartsWith("%", StringComparison.Ordinal))
            {
                if (settings.PreserveComments)
                {
                    comments.Add(line);
                }
                currentLine++;
                continue;
            }

            // Process the line
            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                currentColumn = i + 1;

                if (!inEntry && c == '@')
                {
                    // Start of a new entry
                    inEntry = true;
                    braceCount = 0;
                    entryBuilder.Clear();
                    entryBuilder.Append(c);
                }
                else if (inEntry)
                {
                    entryBuilder.Append(c);

                    if (c == '{')
                    {
                        braceCount++;
                    }
                    else if (c == '}')
                    {
                        braceCount--;

                        if (braceCount == 0)
                        {
                            // End of the entry, parse it
                            inEntry = false;
                            string entryText = entryBuilder.ToString();

                            BibEntry? entry = null;
                            try
                            {
                                entry = ParseSingleEntry(entryText);
                            }
                            catch (BibParseException ex)
                            {
                                // In non-strict mode, continue parsing
                                if (settings.StrictMode)
                                {
                                    throw;
                                }

                                // Add to comments if preserving
                                if (settings.PreserveComments)
                                {
                                    comments.Add($"% Error parsing entry: {ex.Message}");
                                    comments.Add(entryText);
                                }
                            }

                            if (entry != null)
                            {
                                yield return entry;
                            }
                        }
                    }
                }
            }

            currentLine++;
            currentColumn = 1;
        }

        // Handle unclosed entry
        if (inEntry && settings.StrictMode)
        {
            throw CreateParseException("Unclosed entry at end of file");
        }
    }

    private void ParseEntry(string entryText, List<BibEntry> entries)
    {
        var entry = ParseSingleEntry(entryText);
        if (entry != null)
        {
            entries.Add(entry);
        }
    }

    private BibEntry? ParseSingleEntry(string entryText)
    {
        // Extract entry type and content
        var entryTypeMatch = entryTypeRegex.Match(entryText);
        if (!entryTypeMatch.Success)
        {
            if (settings.StrictMode)
            {
                string preview = string.IsNullOrEmpty(entryText) ? "(empty)" 
                    : entryText.Length <= 20 ? entryText 
                    : entryText.Substring(0, 20) + "...";
                throw CreateParseException($"Invalid entry format: {preview}");
            }
            return null;
        }

        string entryType = entryTypeMatch.Groups[1].Value.ToLowerInvariant();

        // Handle special entry types
        if (entryType == "string")
        {
            ParseStringMacro(entryText);
            return null;
        }
        else if (entryType == "preamble")
        {
            ParsePreamble(entryText);
            return null;
        }
        else if (entryType == "comment")
        {
            if (settings.PreserveComments)
            {
                comments.Add(entryText);
            }
            return null;
        }

        // Extract the entry key and fields
        int keyStart = entryText.IndexOf('{') + 1;
        int keyEnd = entryText.IndexOf(',', keyStart);
        if (keyEnd == -1)
        {
            if (settings.StrictMode)
            {
                throw CreateParseException("Entry key not found");
            }
            return null;
        }

        string key = entryText.Substring(keyStart, keyEnd - keyStart).Trim();
        
        // Auto-correct common key errors if enabled
        if (settings.AutoCorrectCommonErrors)
        {
            key = AutoCorrectKey(key);
        }

        // Create the entry
        var entry = new BibEntry(entryType)
        {
            Key = key
        };

        // Parse fields
        string fieldsText = entryText.Substring(keyEnd + 1, entryText.Length - keyEnd - 2).Trim();
        
        // If PreserveFieldOrder is false, we'll sort fields alphabetically after parsing
        ParseFields(fieldsText, entry);
        
        if (!settings.PreserveFieldOrder)
        {
            // Get all current fields (including author and editor) and sort alphabetically
            var currentFields = entry.GetFields().OrderBy(f => f.Key, StringComparer.OrdinalIgnoreCase).ToList();
            var currentAuthors = entry.Authors.ToList();
            var currentEditors = entry.Editors.ToList();
            
            // Clear all fields
            foreach (var field in entry.GetFieldNames().ToList())
            {
                entry.RemoveField(field);
            }
            
            // Clear authors and editors (they'll be restored via field values)
            entry.ClearAuthors();
            entry.ClearEditors();
            
            // Re-add all fields in alphabetical order
            // This will automatically restore authors/editors when "author" and "editor" fields are set
            foreach (var field in currentFields)
            {
                entry[field.Key] = field.Value;
            }
        }

        return entry;
    }

    private void ParseFields(string fieldsText, BibEntry entry)
    {
        int pos = 0;
        while (pos < fieldsText.Length)
        {
            // Skip whitespace
            while (pos < fieldsText.Length && char.IsWhiteSpace(fieldsText[pos]))
            {
                pos++;
            }

            if (pos >= fieldsText.Length)
            {
                break;
            }

            // Find field name
            int fieldNameStart = pos;
            while (pos < fieldsText.Length && fieldsText[pos] != '=')
            {
                pos++;
            }

            if (pos >= fieldsText.Length)
            {
                // End of fields text without an equals sign
                break;
            }

            string fieldName = fieldsText.Substring(fieldNameStart, pos - fieldNameStart).Trim();
            pos++; // Skip '='

            // Skip whitespace
            while (pos < fieldsText.Length && char.IsWhiteSpace(fieldsText[pos]))
            {
                pos++;
            }

            if (pos >= fieldsText.Length)
            {
                // End of fields text without a value
                break;
            }

            // Parse the field value
            char delimiter = fieldsText[pos];
            string fieldValue;

            if (delimiter == '{')
            {
                // Value is in braces
                int braceCount = 1;
                int valueStart = pos + 1;
                pos++;

                while (pos < fieldsText.Length && braceCount > 0)
                {
                    // Handle escape sequences - skip escaped characters
                    if (fieldsText[pos] == '\\' && pos + 1 < fieldsText.Length)
                    {
                        // Skip the backslash and the next character
                        pos += 2;
                        continue;
                    }
                    
                    if (fieldsText[pos] == '{')
                    {
                        braceCount++;
                    }
                    else if (fieldsText[pos] == '}')
                    {
                        braceCount--;
                    }
                    pos++;
                }

                if (braceCount > 0 && settings.StrictMode)
                {
                    throw CreateParseException("Unclosed brace in field value");
                }

                fieldValue = fieldsText.Substring(valueStart, pos - valueStart - 1);
            }
            else if (delimiter == '"')
            {
                // Value is in quotes
                int valueStart = pos + 1;
                pos++;

                while (pos < fieldsText.Length && fieldsText[pos] != '"')
                {
                    // Handle escaped quotes
                    if (fieldsText[pos] == '\\' && pos + 1 < fieldsText.Length && fieldsText[pos + 1] == '"')
                    {
                        pos++; // Skip the escape character
                    }
                    pos++;
                }

                if (pos >= fieldsText.Length && settings.StrictMode)
                {
                    throw CreateParseException("Unclosed quote in field value");
                }

                fieldValue = fieldsText.Substring(valueStart, pos - valueStart);
                pos++; // Skip closing quote
            }
            else if (delimiter == '#')
            {
                // Value is a macro or concatenation
                StringBuilder sb = new();

                // Handle simple case where field value is just a macro (like "journal = JCS")
                int macroStart = pos;
                while (pos < fieldsText.Length && char.IsLetterOrDigit(fieldsText[pos]))
                {
                    pos++;
                }

                if (pos > macroStart)
                {
                    string macroName = fieldsText.Substring(macroStart, pos - macroStart).Trim();

                    // Check if we found a macro without # signs
                    if (macroName.Length > 0 && stringMacros.TryGetValue(macroName, out string? macroValue))
                    {
                        fieldValue = expandMacros ? macroValue : macroName;
                    }
                    else
                    {
                        fieldValue = macroName;
                    }
                }
                else
                {
                    // More complex macro usage with # signs
                    bool inMacro = true;

                    while (pos < fieldsText.Length && fieldsText[pos] != ',' && fieldsText[pos] != '}')
                    {
                        if (fieldsText[pos] == '#')
                        {
                            inMacro = true;
                        }
                        else if (inMacro && !char.IsWhiteSpace(fieldsText[pos]))
                        {
                            // Start of a macro name
                            macroStart = pos;
                            while (pos < fieldsText.Length && char.IsLetterOrDigit(fieldsText[pos]))
                            {
                                pos++;
                            }
                            string macroName = fieldsText.Substring(macroStart, pos - macroStart);

                            if (expandMacros && stringMacros.TryGetValue(macroName, out string? macroValue))
                            {
                                sb.Append(macroValue);
                            }
                            else
                            {
                                sb.Append(macroName);
                            }

                            inMacro = false;
                            continue;
                        }

                        sb.Append(fieldsText[pos]);
                        pos++;
                    }

                    fieldValue = sb.ToString().Trim();
                }
            }
            else
            {
                // Value is unquoted (e.g., a number or a macro)
                int valueStart = pos;
                while (pos < fieldsText.Length && fieldsText[pos] != ',' && fieldsText[pos] != '}')
                {
                    pos++;
                }

                fieldValue = fieldsText.Substring(valueStart, pos - valueStart).Trim();

                // Handle string macros
                if (fieldValue.StartsWith("#", StringComparison.Ordinal) && fieldValue.EndsWith("#", StringComparison.Ordinal))
                {
                    string macroName = fieldValue.Trim('#').Trim();
                    if (stringMacros.TryGetValue(macroName, out string? macroValue))
                    {
                        fieldValue = expandMacros ? macroValue : macroName;
                    }
                }
                // Check if value itself is a macro without # signs
                else if (stringMacros.TryGetValue(fieldValue, out string? macroValue))
                {
                    fieldValue = expandMacros ? macroValue : fieldValue;
                }
            }

            // Auto-correct common field value errors if enabled
            if (settings.AutoCorrectCommonErrors)
            {
                fieldValue = AutoCorrectFieldValue(fieldValue);
            }
            
            // Process the field value to handle LaTeX encoded characters if needed
            if (settings.ConvertLatexToUnicode)
            {
                // Convert any LaTeX encoded special characters to Unicode
                fieldValue = BibTexEncoder.ToUnicode(fieldValue);
            }

            // Normalize author names if enabled
            if (settings.NormalizeAuthorNames && 
                (string.Equals(fieldName, FieldName.Author, StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(fieldName, FieldName.Editor, StringComparison.OrdinalIgnoreCase)))
            {
                fieldValue = NormalizeAuthorField(fieldValue);
            }
            
            // Normalize month values if enabled
            if (settings.NormalizeMonths && string.Equals(fieldName, FieldName.Month, StringComparison.OrdinalIgnoreCase))
            {
                fieldValue = NormalizeMonthField(fieldValue);
            }
            
            // Add the field to the entry
            entry[fieldName] = fieldValue;

            // Skip to next field
            while (pos < fieldsText.Length && fieldsText[pos] != ',')
            {
                pos++;
            }

            if (pos < fieldsText.Length)
            {
                pos++; // Skip comma
            }
        }
    }

    private void ParseStringMacro(string entryText)
    {
        var match = stringMacroBracesRegex.Match(entryText);
        if (!match.Success)
        {
            match = stringMacroQuotesRegex.Match(entryText);
            if (!match.Success && settings.StrictMode)
            {
                throw CreateParseException("Invalid string macro format");
            }
        }

        if (match.Success)
        {
            string name = match.Groups[1].Value;
            string value = match.Groups[2].Value;
            stringMacros[name] = value;
        }
    }

    private void ParsePreamble(string entryText)
    {
        var match = preambleRegex.Match(entryText);
        if (!match.Success && settings.StrictMode)
        {
            throw CreateParseException("Invalid preamble format");
        }

        if (match.Success)
        {
            string preamble = match.Groups[1].Value;
            preambles.Add(preamble);
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases the unmanaged resources used by the <see cref="BibParser"/> and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                // Capture the reader reference and ownership flag in a thread-safe manner
                var readerToDispose = reader;
                var shouldDispose = ownsReader;
                
                if (readerToDispose != null && shouldDispose)
                {
                    readerToDispose.Dispose();
                }
                
                reader = null;
            }

            disposed = true;
        }
    }
}
