using BibSharp.Models;
using BibSharp.Utilities;

namespace BibSharp.Tests;

public class IntegrationTests
{
    [Fact]
    public void RoundTrip_ParseAndSerialize_PreservesData()
    {
        var originalBibtex = @"
@article{smith2025,
  author = {Smith, John and Doe, Jane},
  title = {A Comprehensive Study},
  journal = {Journal of Testing},
  year = {2025},
  volume = {10},
  number = {5},
  pages = {100--110}
}";

        var parser = new BibParser();
        var entries = parser.ParseString(originalBibtex);
        
        var serializer = new BibSerializer(new BibSerializerSettings
        {
            ValidateBeforeSerialization = false
        });
        var serialized = serializer.Serialize(entries);

        // Parse again
        var reparsed = parser.ParseString(serialized);

        Assert.Equal(entries.Count, reparsed.Count);
        Assert.Equal(entries[0].Key, reparsed[0].Key);
        Assert.Equal(entries[0].Title, reparsed[0].Title);
        Assert.Equal(entries[0].Authors.Count, reparsed[0].Authors.Count);
    }

    [Fact]
    public void ComplexWorkflow_CreateModifyValidateSerialize()
    {
        // Create entry
        var entry = new BibEntry(EntryType.Article)
        {
            Key = "test2025",
            Title = "Original Title",
            Journal = "Test Journal",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John"));

        // Modify
        entry.Title = "Modified Title";
        entry.Volume = 10;
        entry.Pages = new PageRange(50, 75);

        // Add keywords
        entry.AddKeyword("machine learning");
        entry.AddKeyword("neural networks");

        // Validate
        var validation = entry.Validate();
        Assert.True(validation.IsValid);

        // Generate key
        var key = entry.GenerateKey();
        Assert.NotEmpty(key);

        // Serialize
        var serializer = new BibSerializer(new BibSerializerSettings
        {
            ValidateBeforeSerialization = false
        });
        var bibtex = serializer.Serialize(new[] { entry });

        Assert.Contains("Modified Title", bibtex);
        Assert.Contains("keywords", bibtex); // Keywords field should exist
    }

    [Fact]
    public void MultipleEntriesWorkflow_SortAndFilter()
    {
        var entries = new List<BibEntry>
        {
            new BibEntry(EntryType.Article) { Key = "z", Title = "Z Article", Year = 2020 },
            new BibEntry(EntryType.Book) { Key = "a", Title = "A Book", Year = 2025 },
            new BibEntry(EntryType.Article) { Key = "m", Title = "M Article", Year = 2022 }
        };
        entries[0].AddAuthor(new Author("Zebra", "John"));
        entries[1].AddAuthor(new Author("Aardvark", "Jane"));
        entries[2].AddAuthor(new Author("Middle", "Bob"));

        // Sort by year descending
        var sorted = entries.OrderBy(e => e, BibComparer.ByYear(descending: true)).ToList();
        
        Assert.Equal(2025, sorted[0].Year);
        Assert.Equal(2022, sorted[1].Year);
        Assert.Equal(2020, sorted[2].Year);

        // Filter articles only
        var articles = entries.Where(e => e.EntryType == EntryType.Article).ToList();
        Assert.Equal(2, articles.Count);

        // Generate keys for all
        BibEntry.RegenerateKeys(entries, preserveExistingKeys: false);
        
        Assert.All(entries, e => Assert.NotEmpty(e.Key));
    }

    [Fact]
    public void CloneAndModify_OriginalUnchanged()
    {
        var original = new BibEntry(EntryType.Article)
        {
            Key = "original",
            Title = "Original Title",
            Year = 2025
        };
        original.AddAuthor(new Author("Smith", "John"));
        original.AddKeyword("original");

        var clone = original.Clone();
        clone.Key = "modified";
        clone.Title = "Modified Title";
        clone.Year = 2026;
        clone.AddAuthor(new Author("Doe", "Jane"));
        clone.AddKeyword("modified");

        // Original should be unchanged
        Assert.Equal("original", original.Key);
        Assert.Equal("Original Title", original.Title);
        Assert.Equal(2025, original.Year);
        Assert.Single(original.Authors);
        Assert.Contains("original", original.GetKeywordList());
        Assert.DoesNotContain("modified", original.GetKeywordList());
    }

    [Fact]
    public void DuplicateDetection_FindsDuplicates()
    {
        var entry1 = new BibEntry { Key = "e1", Doi = "10.1234/test" };
        var entry2 = new BibEntry { Key = "e2", Doi = "10.1234/test" }; // Duplicate
        var entry3 = new BibEntry { Key = "e3", Doi = "10.5678/other" };

        var entries = new[] { entry1, entry2, entry3 };
        var duplicates = entries.FindDuplicates().ToList();

        Assert.Single(duplicates);
        Assert.Equal(2, duplicates[0].Count());
    }

    [Fact]
    public void FormatCitations_AllStyles()
    {
        var entry = new BibEntry(EntryType.Article)
        {
            Key = "test2025",
            Title = "Test Article",
            Journal = "Test Journal",
            Year = 2025,
            Volume = 10
        };
        entry.AddAuthor(new Author("Smith", "John"));

        // Test all citation styles
        var chicago = entry.ToFormattedString(CitationStyle.Chicago);
        var mla = entry.ToFormattedString(CitationStyle.Mla);
        var apa = entry.ToFormattedString(CitationStyle.Apa);
        var harvard = entry.ToFormattedString(CitationStyle.Harvard);
        var ieee = entry.ToFormattedString(CitationStyle.Ieee);

        Assert.All(new[] { chicago, mla, apa, harvard, ieee }, 
            formatted => Assert.Contains("Smith", formatted));
        Assert.All(new[] { chicago, mla, apa, harvard, ieee }, 
            formatted => Assert.Contains("2025", formatted));
    }

    [Fact]
    public void ComplexBibTeXParsing_WithComments()
    {
        var bibtex = @"
% This is a header comment
@string{JCS = {Journal of Computer Science}}

% Article comment
@article{test1,
  author = {Smith, John},
  title = {Test 1},
  journal = JCS,
  year = {2025}
}

% Another comment
@comment{This is a comment entry}

@article{test2,
  author = {Doe, Jane},
  title = {Test 2},
  journal = JCS,
  year = {2024}
}
";

        var parser = new BibParser(new BibParserSettings { PreserveComments = true });
        var entries = parser.ParseString(bibtex);

        Assert.Equal(2, entries.Count);
        Assert.NotEmpty(parser.Comments);
        Assert.Contains(parser.StringMacros.Keys, k => k == "JCS");
    }

    [Fact]
    public void ComplexBibTeX_AllEntryTypes()
    {
        var bibtex = @"
@article{art1,author={A},title={T},journal={J},year={2025}}
@book{book1,title={T},publisher={P},year={2025}}
@inproceedings{conf1,author={A},title={T},booktitle={B},year={2025}}
@conference{conf2,author={A},title={T},booktitle={B},year={2025}}
@incollection{coll1,author={A},title={T},booktitle={B},publisher={P},year={2025}}
@inbook{inb1,author={A},title={T},publisher={P},year={2025}}
@booklet{bklt1,title={T}}
@phdthesis{phd1,author={A},title={T},school={S},year={2025}}
@mastersthesis{mast1,author={A},title={T},school={S},year={2025}}
@techreport{tech1,author={A},title={T},institution={I},year={2025}}
@manual{man1,title={T}}
@proceedings{proc1,title={T},year={2025}}
@unpublished{unpub1,author={A},title={T},note={N}}
@misc{misc1,title={T}}
";

        var parser = new BibParser();
        var entries = parser.ParseString(bibtex);

        Assert.Equal(14, entries.Count);
        Assert.Contains(entries, e => e.EntryType == EntryType.Article);
        Assert.Contains(entries, e => e.EntryType == EntryType.Book);
        Assert.Contains(entries, e => e.EntryType == EntryType.InProceedings);
        Assert.Contains(entries, e => e.EntryType == EntryType.Conference);
        Assert.Contains(entries, e => e.EntryType == EntryType.InCollection);
        Assert.Contains(entries, e => e.EntryType == EntryType.InBook);
        Assert.Contains(entries, e => e.EntryType == EntryType.Booklet);
        Assert.Contains(entries, e => e.EntryType == EntryType.PhdThesis);
        Assert.Contains(entries, e => e.EntryType == EntryType.MastersThesis);
        Assert.Contains(entries, e => e.EntryType == EntryType.TechReport);
        Assert.Contains(entries, e => e.EntryType == EntryType.Manual);
        Assert.Contains(entries, e => e.EntryType == EntryType.Proceedings);
        Assert.Contains(entries, e => e.EntryType == EntryType.Unpublished);
        Assert.Contains(entries, e => e.EntryType == EntryType.Misc);
    }

    [Fact]
    public void BibTeXWithUnicodeCharacters()
    {
        var entry = new BibEntry(EntryType.Article)
        {
            Key = "test",
            Title = "Café Résumé Über Naïve Señor"
        };
        entry.AddAuthor(new Author("Müller", "Hans"));

        var serializer = new BibSerializer(new BibSerializerSettings
        {
            Encoding = BibEncoding.Unicode,
            ValidateBeforeSerialization = false
        });

        var bibtex = serializer.Serialize(new[] { entry });

        // Should preserve Unicode
        Assert.Contains("Café", bibtex);
        Assert.Contains("Müller", bibtex);
        Assert.Contains("Señor", bibtex);
    }

    [Fact]
    public void BibTeXWithLaTeXEncoding()
    {
        var entry = new BibEntry(EntryType.Article)
        {
            Key = "test",
            Title = "Café Müller"
        };

        var serializer = new BibSerializer(new BibSerializerSettings
        {
            Encoding = BibEncoding.LaTeX,
            ValidateBeforeSerialization = false
        });

        var bibtex = serializer.Serialize(new[] { entry });

        // Should convert to LaTeX
        Assert.Contains("\\'e", bibtex);
        Assert.Contains("\\\"u", bibtex);
    }

    [Fact]
    public void KeywordWorkflow_AddAndReplace()
    {
        var entry = new BibEntry(EntryType.Misc) { Key = "test" };
        
        // Add keywords one by one
        entry.AddKeyword("machinelearning");
        entry.AddKeyword("AI");
        entry.AddKeyword("networks");

        var keywords = entry.GetKeywordList();
        Assert.True(keywords.Count >= 3); // Should have at least 3
        Assert.Contains("machinelearning", keywords);
        Assert.Contains("AI", keywords);
        Assert.Contains("networks", keywords);

        // Set completely new list
        entry.SetKeywordList(new[] { "new", "keywords" });
        keywords = entry.GetKeywordList();
        Assert.Equal(2, keywords.Count);
        Assert.Contains("new", keywords);
        Assert.Contains("keywords", keywords);
        Assert.DoesNotContain("machinelearning", keywords);
    }
}

