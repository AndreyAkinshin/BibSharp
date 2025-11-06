# BibSharp

[![NuGet](https://img.shields.io/nuget/v/BibSharp.svg)](https://www.nuget.org/packages/BibSharp)

A comprehensive, modern .NET library for BibTeX processing with a clean and intuitive API. BibSharp makes it easy to parse, manipulate, validate, and serialize BibTeX bibliography entries in your .NET applications.

## Features

✨ **Comprehensive BibTeX Support**
- Parse BibTeX files and strings with full support for all standard entry types
- Support for `@string` macros and `@preamble` directives
- Handle special characters and LaTeX encoding
- Preserve or expand string macros

📝 **Intuitive API**
- Fluent, object-oriented design
- Type-safe access to common fields (Title, Author, Year, etc.)
- Flexible field access via properties or indexers
- Method chaining for convenient entry construction

🔍 **Advanced Features**
- **DOI Resolution**: Automatically fetch BibTeX entries from DOIs using CrossRef and DOI.org APIs
- **Citation Formatting**: Format entries in APA, MLA, Chicago, Harvard, and IEEE styles
- **Key Generation**: Automatically generate citation keys with multiple formats (AuthorYear, AuthorTitleYear, etc.)
- **Validation**: Validate entries against BibTeX standards with detailed error and warning reporting
- **Entry Matching & Deduplication**: Find matching entries and duplicates using DOI, author+title+year matching
- **Sorting & Comparison**: Built-in comparers for sorting by author, year, title, or custom fields
- **Author Handling**: Parse and format author names with support for suffixes (Jr., Sr., III) and multiple formats
- **Internationalization**: Full support for Unicode and LaTeX-encoded special characters with bidirectional conversion
- **Custom Types**: Register custom entry types with required and optional fields
- **Field Aliases**: Map alternative field names for compatibility with different BibTeX variants

⚡ **Performance**
- Async/await support for I/O operations
- Streaming parsing for large files
- Efficient memory usage

## Installation

Install BibSharp via NuGet:

```bash
dotnet add package BibSharp
```

Or via the Package Manager Console:

```powershell
Install-Package BibSharp
```

> **💡 Looking for comprehensive examples?** Check out the [BibSharp.Demo](BibSharp.Demo) project! It includes working demonstrations of all major features including DOI resolution, citation formatting (APA, MLA, Chicago, Harvard, IEEE), internationalization, entry matching, sorting, custom fields, and more.

## Quick Start

### Parsing BibTeX Files

```csharp
using BibSharp;

// Parse a BibTeX file
var parser = new BibParser();
var entries = parser.ParseFile("references.bib");

// Access entry fields
foreach (var entry in entries)
{
    Console.WriteLine($"{entry.Key}: {entry.Title} ({entry.Year})");
    
    foreach (var author in entry.Authors)
    {
        Console.WriteLine($"  - {author.ToDisplayString()}");
    }
}
```

### Creating Entries Programmatically

```csharp
using BibSharp;
using BibSharp.Models;

// Create a new article entry
var entry = new BibEntry("article")
{
    Key = "smith2025",
    Title = "A Comprehensive Study of BibTeX Processing",
    Journal = "Journal of Computer Science",
    Year = 2025,
    Volume = 42,
    Number = "1",
    Pages = new PageRange(123, 145)
};

// Add authors
entry.AddAuthor(new Author("Smith", "John", "Q."));
entry.AddAuthor(new Author("Doe", "Jane"));

// Convert to BibTeX
string bibtex = entry.ToBibTeX();
Console.WriteLine(bibtex);
```

Output:
```bibtex
@article{smith2025,
  author = {Smith, John Q. and Doe, Jane},
  title = {A Comprehensive Study of BibTeX Processing},
  journal = {Journal of Computer Science},
  year = {2025},
  volume = {42},
  number = {1},
  pages = {123--145}
}
```

### Parsing BibTeX Strings

```csharp
using BibSharp;

// Parse a single entry
var entry = BibEntry.Parse(@"
@article{smith2025,
  author = {Smith, John},
  title = {My Article},
  year = {2025}
}");

// Try parse without exceptions
if (BibEntry.TryParse(bibTexString, out var parsedEntry))
{
    Console.WriteLine($"Parsed: {parsedEntry.Title}");
}

// Parse multiple entries
var entries = BibEntry.ParseAll(multipleBibTexString);
```

### Resolving DOIs

```csharp
using BibSharp;
using BibSharp.Utilities;

// Fetch a BibTeX entry from a DOI (with cancellation support)
var entry = await BibEntry.FromDoiAsync("10.1145/3377811.3380330");

if (entry != null)
{
    Console.WriteLine($"Title: {entry.Title}");
    Console.WriteLine($"Authors: {string.Join(", ", entry.Authors.Select(a => a.ToDisplayString()))}");
    Console.WriteLine($"Year: {entry.Year}");
    Console.WriteLine($"DOI: {entry.Doi}");
    
    // Generate BibTeX
    Console.WriteLine(entry.ToBibTeX());
}

// With cancellation token
using var cts = new CancellationTokenSource();
cts.CancelAfter(TimeSpan.FromSeconds(10));
var entryWithCancel = await BibEntry.FromDoiAsync("10.1145/3377811.3380330", cts.Token);

// For custom configuration (timeout, rate limiting), create a DoiResolver instance
var resolver = new DoiResolver(timeoutSeconds: 60, minRequestDelayMs: 200);
var customEntry = await resolver.ResolveDoiAsync("10.1145/3377811.3380330", cts.Token);

// DOI resolver accepts various formats:
// - "10.1145/3377811.3380330"
// - "https://doi.org/10.1145/3377811.3380330"
// - "doi:10.1145/3377811.3380330"

// Setting DOI on an entry (works automatically with implicit conversion)
var manualEntry = new BibEntry("article")
{
    Doi = "10.1145/3377811.3380330"  // Implicitly converts string to Doi struct
};

// Or explicitly
manualEntry.Doi = new Doi("10.1145/3377811.3380330");

// Access DOI as URL
Console.WriteLine(manualEntry.Doi?.Url);  // "https://doi.org/10.1145/3377811.3380330"
```

### Formatting Citations

```csharp
using BibSharp;
using BibSharp.Utilities;

var entry = new BibEntry("article")
{
    // ... set fields ...
};

// Format in different citation styles
string apa = entry.ToFormattedString(CitationStyle.APA);
string mla = entry.ToFormattedString(CitationStyle.MLA);
string chicago = entry.ToFormattedString(CitationStyle.Chicago);
string harvard = entry.ToFormattedString(CitationStyle.Harvard);
string ieee = entry.ToFormattedString(CitationStyle.IEEE);
```

### Validating Entries

```csharp
using BibSharp;

var entry = new BibEntry("article")
{
    Title = "My Article",
    Year = 2025
};

var result = entry.Validate();

if (!result.IsValid)
{
    Console.WriteLine("Validation errors:");
    foreach (var error in result.Errors)
    {
        Console.WriteLine($"  - {error}");
    }
}

if (result.HasWarnings)
{
    Console.WriteLine("Warnings:");
    foreach (var warning in result.Warnings)
    {
        Console.WriteLine($"  - {warning}");
    }
}
```

### Generating Citation Keys

```csharp
using BibSharp;
using BibSharp.Models;
using BibSharp.Utilities;

var entry = new BibEntry("article")
{
    Title = "Study of Modern Software",
    Year = 2025
};
entry.AddAuthor(new Author("Smith", "John"));

// Generate a citation key
string key = entry.GenerateKey(
    BibKeyGenerator.KeyFormat.AuthorTitleYear,
    setKeyAutomatically: true
);

Console.WriteLine($"Generated key: {entry.Key}"); // "smithstudy2025"

// Batch generate keys for multiple entries
var entries = new List<BibEntry> { entry, /* ... */ };
var keyMap = BibEntry.RegenerateKeys(
    entries, 
    format: BibKeyGenerator.KeyFormat.AuthorYear,
    preserveExistingKeys: true
);

// Available key formats:
// - AuthorYear: "smith2025"
// - AuthorTitleYear: "smithstudy2025"
// - AuthorTitleYearShort: "smistud2025"
// - AuthorJournalYear: "smithjcs2025"
```

### Serializing to Files

```csharp
using BibSharp;
using BibSharp.Models;
using BibSharp.Utilities;

var entries = new List<BibEntry> { /* your entries */ };

var serializer = new BibSerializer(new BibSerializerSettings
{
    FieldOrder = new[] { "author", "title", "journal", "year" },
    Indent = "  ",
    AuthorFormat = AuthorFormat.LastNameFirst,
    UseBracesForFieldValues = true,
    Encoding = BibEncoding.Unicode,
    LineEnding = LineEnding.Lf
});

// Serialize to file
serializer.SerializeToFile(entries, "output.bib");

// Or to string
string bibtex = serializer.Serialize(entries);
```

### Searching, Filtering, and Matching

```csharp
using BibSharp;
using BibSharp.Extensions; // For extension methods
using BibSharp.Utilities;

var parser = new BibParser();
var entries = parser.ParseFile("references.bib");

// Extension methods for improved discoverability
var duplicates = entries.FindDuplicates(); // Extension method
var sortedByAuthor = entries.SortByAuthor(); // Extension method
var sortedByYear = entries.SortByYear(descending: true); // Extension method
var sortedByTitle = entries.SortByTitle(); // Extension method

// Find matching entries across collections
var entry = entries.First();
var otherEntries = parser.ParseFile("other.bib");
var match = entry.FindMatch(otherEntries); // Extension method - uses DOI, authors+title+year, etc.

// Traditional API (still available for advanced scenarios)
var sortedByAuthorOld = entries.OrderBy(e => e, BibComparer.ByAuthor()).ToList();
var sortedByYearOld = entries.OrderBy(e => e, BibComparer.ByYear(descending: true)).ToList();

// Chain multiple sort criteria
var sorted = entries.OrderBy(e => e, BibComparer.Chain(
    BibComparer.ByAuthor(),
    BibComparer.ByYear(descending: true)
)).ToList();

// Filter using LINQ (System.Linq)
var recentPapers = entries.Where(e => e.Year >= 2020).ToList();
var smithPapers = entries.Where(e => 
    e.Authors.Any(a => a.LastName.Contains("Smith", System.StringComparison.OrdinalIgnoreCase))
).ToList();
```

### Working with Special Characters

```csharp
using BibSharp;
using BibSharp.Utilities;

// Parse entries with special characters
var parser = new BibParser(new BibParserSettings
{
    ConvertLatexToUnicode = true  // Convert LaTeX to Unicode
});

var entries = parser.ParseString(@"
@article{test2025,
  author = {M{\"u}ller, Hans},
  title = {Caf{\'e} Science}
}");

Console.WriteLine(entries[0].Authors[0].LastName); // "Müller"
Console.WriteLine(entries[0].Title); // "Café Science"

// Serialize with LaTeX encoding
var serializer = new BibSerializer(new BibSerializerSettings
{
    Encoding = BibEncoding.LaTeX  // Use LaTeX encoding
});

string latex = serializer.Serialize(entries);
// Output: author = {M{\"u}ller, Hans}, title = {Caf{\'e} Science}
```

### Working with Keywords and Months

```csharp
using BibSharp;

var entry = new BibEntry("article");

// Working with keywords
entry.Keywords = "machine learning, neural networks";
var keywords = entry.GetKeywordList(); // Returns ["machine learning", "neural networks"]
entry.AddKeyword("deep learning");
entry.SetKeywordList(new[] { "AI", "ML", "DL" });

// Working with months
entry.SetMonthByNumber(3, useAbbreviation: true); // Sets to "mar"
int? monthNumber = entry.GetMonthAsNumber(); // Returns 3

// Clone entries
var clonedEntry = entry.Clone();

// Access and manipulate fields
entry["custom-field"] = "custom value";
var allFields = entry.GetFields();
var fieldNames = entry.GetFieldNames();
entry.RemoveField("abstract");

// Check field existence
if (entry.HasField("doi"))
{
    Console.WriteLine($"DOI: {entry.Doi}");
}
```

### Async Operations

```csharp
using BibSharp;

var parser = new BibParser();

// Parse asynchronously
var entries = await parser.ParseFileAsync("large_bibliography.bib");

// Stream large files
await foreach (var entry in parser.ParseFileStreamAsync("huge_bibliography.bib"))
{
    // Process entries one at a time without loading all into memory
    Console.WriteLine($"Processing: {entry.Key}");
}

// Serialize asynchronously
var serializer = new BibSerializer();
await serializer.SerializeToFileAsync(entries, "output.bib");
```

## Supported Entry Types

BibSharp supports all standard BibTeX entry types:

- `@article` - Journal or magazine article
- `@book` - Published book
- `@inproceedings` / `@conference` - Conference paper
- `@incollection` - Part of a book with its own title
- `@inbook` - Part of a book (chapter, section, pages)
- `@booklet` - Printed work without a named publisher
- `@phdthesis` - Ph.D. thesis
- `@mastersthesis` - Master's thesis
- `@techreport` - Technical report
- `@manual` - Technical manual
- `@proceedings` - Conference proceedings
- `@unpublished` - Unpublished work
- `@misc` - Miscellaneous

### Standard Fields

BibSharp provides type-safe properties for all standard BibTeX fields:

```csharp
using BibSharp;
using BibSharp.Models;

var entry = new BibEntry("article")
{
    // Bibliographic information
    Title = "Article Title",
    Journal = "Journal Name",
    BookTitle = "Book Title", // For inproceedings, incollection
    
    // Publication details
    Year = 2025,
    Volume = 42,
    Number = "3",
    Pages = new PageRange(10, 25),
    Month = "mar",
    
    // Publisher information
    Publisher = "Publisher Name",
    Address = "City, Country",
    Edition = "2nd",
    Series = "Series Name",
    
    // Institutional
    Institution = "Institution Name",
    School = "University Name",
    Organization = "Organization Name",
    
    // Identifiers
    Doi = "10.1234/example",
    Url = "https://example.com",
    Isbn = "978-0-123456-78-9",
    Issn = "1234-5678",
    Arxiv = "2101.12345",
    
    // Additional information
    Note = "Additional notes",
    Keywords = "keyword1, keyword2",
    Abstract = "Abstract text",
    Language = "english",
    Copyright = "© 2025",
    Annote = "Annotations",
    
    // Entry-specific
    Chapter = "3",
    Type = "PhD thesis",
    HowPublished = "Online"
};
```

### Custom Entry Types and Field Aliases

BibSharp supports custom entry types and field aliases:

```csharp
using BibSharp;

// Register a custom entry type
BibEntry.RegisterCustomType(
    entryType: "dataset",
    requiredFields: new[] { "author", "title", "year", "url" },
    optionalFields: new[] { "version", "doi", "publisher" }
);

// Create an entry with the custom type
var dataset = new BibEntry("dataset")
{
    Key = "mydataset2025",
    Title = "Research Dataset",
    Year = 2025,
    Url = "https://example.com/dataset"
};

// Register field aliases for compatibility (e.g., for biblatex compatibility)
BibEntry.RegisterFieldAlias("journaltitle", "journal");
BibEntry.RegisterFieldAlias("location", "address");
```

## Advanced Configuration

### Parser Settings

```csharp
using BibSharp;
using BibSharp.Utilities;

var settings = new BibParserSettings
{
    PreserveComments = true,            // Keep comments from the file
    StrictMode = false,                 // Gracefully handle malformed entries
    ExpandStringMacros = true,          // Expand @string macros
    ConvertLatexToUnicode = true,       // Convert LaTeX to Unicode
    AutoCorrectCommonErrors = true,     // Fix common BibTeX errors
    PreserveFieldOrder = true,          // Preserve original field order
    NormalizeAuthorNames = true,        // Normalize author name formats
    NormalizeMonths = true              // Normalize month values
};

var parser = new BibParser(settings);
```

### Serializer Settings

```csharp
using BibSharp;
using BibSharp.Models;
using BibSharp.Utilities;

var settings = new BibSerializerSettings
{
    Indent = "    ",                              // 4 spaces
    FieldOrder = new[] { "author", "title" },    // Order fields
    Encoding = BibEncoding.LaTeX,                // LaTeX or Unicode
    LineEnding = LineEnding.Lf,                  // Lf, Crlf, or SystemDefault
    UseBracesForFieldValues = true,              // Use {} instead of ""
    ValidateBeforeSerialization = true,          // Validate before writing
    AuthorFormat = AuthorFormat.LastNameFirst,   // Author name format
    WrapLongFields = true,                       // Wrap long values
    MaxFieldLength = 80                          // Max line length
};

var serializer = new BibSerializer(settings);
```

## API Documentation

For detailed API documentation, use IntelliSense in your IDE. All public types and methods include comprehensive XML documentation with examples and usage notes.

## Examples

Check out the [BibSharp.Demo](BibSharp.Demo) project for more comprehensive examples including:

- DOI resolution and metadata fetching
- Citation formatting in multiple styles (APA, MLA, Chicago, Harvard, IEEE)
- International character handling and LaTeX encoding
- Entry matching and duplicate detection
- Sorting and comparison
- Custom field handling
- Batch key generation
- Keyword and month manipulation

## Error Handling

BibSharp uses specific exceptions for different error scenarios:

```csharp
using BibSharp;
using BibSharp.Exceptions;
using BibSharp.Utilities;

try
{
    var parser = new BibParser(new BibParserSettings { StrictMode = true });
    var entries = parser.ParseFile("references.bib");
}
catch (BibParseException ex)
{
    Console.WriteLine($"Parse error at line {ex.LineNumber}, column {ex.ColumnNumber}: {ex.Message}");
}

try
{
    var entry = new BibEntry("article");
    var result = entry.Validate();
    
    if (!result.IsValid)
    {
        // Handle validation errors
        foreach (var error in result.Errors)
        {
            Console.WriteLine($"Validation error: {error}");
        }
    }
    
    var serializer = new BibSerializer(new BibSerializerSettings 
    { 
        ValidateBeforeSerialization = true 
    });
    serializer.Serialize(new[] { entry });
}
catch (BibValidationException ex)
{
    Console.WriteLine($"Validation failed for entry '{ex.EntryKey}'");
    foreach (var error in ex.ValidationErrors)
    {
        Console.WriteLine($"  - {error}");
    }
}
```

## Thread Safety

BibSharp is designed with thread safety in mind:

- **BibEntry**: Thread-safe for read operations after construction. The static methods `RegisterCustomType()` and `RegisterFieldAlias()` use concurrent collections and are safe to call from multiple threads. Modification operations require external synchronization.
- **BibParser**: Not thread-safe. Create separate parser instances for concurrent parsing operations.
- **BibSerializer**: Not thread-safe. Create separate serializer instances for concurrent serialization operations.
- **DoiResolver**: Thread-safe for instance methods with per-instance rate limiting.

**Best Practices**: 
- For concurrent operations, create separate instances of parsers and serializers per thread
- Use instance-based `DoiResolver` with dependency injection for better testability and thread safety
- For concurrent read access to BibEntry, construct and populate entries before sharing across threads

## Requirements

- .NET Standard 2.1 or higher
- Compatible with .NET 5.0+, .NET Core 3.1+, and .NET Framework 4.8+

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

### Development Setup

**Prerequisites**: .NET 9.0 SDK or later

**Build and test**:
```bash
dotnet build                    # Build solution
dotnet test                     # Run all tests
dotnet pack -c Release          # Create NuGet package
```

**Project structure**:
- `BibSharp/` - Main library (target: .NET Standard 2.1)
- `BibSharp.Tests/` - Unit tests (xUnit)
- `BibSharp.Demo/` - Usage examples

**Before submitting a PR**:
1. Add tests for new functionality
2. Ensure all tests pass (`dotnet test`)
3. Add XML documentation for public APIs
4. Update README with examples if adding user-facing features

## Test Coverage

BibSharp has a comprehensive test suite with **1000+ passing tests** achieving **80+% line coverage**.

### Coverage Highlights
- **100% coverage**: Core structs (EntryType, FieldName, Month), Models (PageRange, ValidationResult), BibTexEncoder, Settings, Exception classes
- **Excellent coverage (>95%)**: BibEntry, BibSerializer, BibComparer
- **Strong coverage (>90%)**: Author, BibEntryMatcher, BibKeyGenerator
- **Good coverage (>80%)**: BibParser, BibSerializerSettings

### What's Tested
✅ All 14 standard BibTeX entry types  
✅ All 32 standard fields and field aliases  
✅ Complex author name parsing (suffixes, middle names, multiple authors)  
✅ String macros, preambles, and comments  
✅ Citation formatting (Chicago, MLA, APA, Harvard, IEEE)  
✅ Unicode and LaTeX character encoding (100+ special characters)  
✅ Async operations and streaming  
✅ Entry validation, matching, and deduplication  
✅ Key generation (4 formats)  
✅ Round-trip parsing and serialization  

Run tests with:
```bash
dotnet test
dotnet test --collect:"XPlat Code Coverage"  # With coverage report
```

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
