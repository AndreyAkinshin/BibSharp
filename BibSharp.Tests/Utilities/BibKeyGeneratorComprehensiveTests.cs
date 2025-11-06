using BibSharp.Models;
using BibSharp.Utilities;

namespace BibSharp.Tests.Utilities;

public class BibKeyGeneratorComprehensiveTests
{
    [Fact]
    public void GenerateKey_NullEntry_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => 
            BibKeyGenerator.GenerateKey(null!));
    }

    [Fact]
    public void GenerateKeys_NullEntries_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => 
            BibKeyGenerator.GenerateKeys(null!));
    }

    [Fact]
    public void GenerateKey_NoAuthor_UsesNoauthor()
    {
        var entry = new BibEntry(EntryType.Article)
        {
            Key = "old",
            Title = "Test Article",
            Year = 2025
        };

        var key = BibKeyGenerator.GenerateKey(entry, BibKeyGenerator.KeyFormat.AuthorYear);

        Assert.Contains("noauthor", key.ToLower());
    }

    [Fact]
    public void GenerateKey_NoYear_UsesNodate()
    {
        var entry = new BibEntry(EntryType.Article)
        {
            Key = "old",
            Title = "Test Article"
        };
        entry.AddAuthor(new Author("Smith", "John"));

        var key = BibKeyGenerator.GenerateKey(entry, BibKeyGenerator.KeyFormat.AuthorYear);

        Assert.Contains("nodate", key.ToLower());
    }

    [Fact]
    public void GenerateKey_NoTitle_UsesNotitle()
    {
        var entry = new BibEntry(EntryType.Article)
        {
            Key = "old",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John"));

        var key = BibKeyGenerator.GenerateKey(entry, BibKeyGenerator.KeyFormat.AuthorTitleYear);

        Assert.Contains("notitle", key.ToLower());
    }

    [Fact]
    public void GenerateKey_AuthorYear_Format()
    {
        var entry = new BibEntry(EntryType.Article)
        {
            Key = "old",
            Title = "Test Article",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John"));

        var key = BibKeyGenerator.GenerateKey(entry, BibKeyGenerator.KeyFormat.AuthorYear);

        Assert.Equal("smith2025", key);
    }

    [Fact]
    public void GenerateKey_AuthorTitleYear_Format()
    {
        var entry = new BibEntry(EntryType.Article)
        {
            Key = "old",
            Title = "Analysis Study",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John"));

        var key = BibKeyGenerator.GenerateKey(entry, BibKeyGenerator.KeyFormat.AuthorTitleYear);

        Assert.Contains("smith", key);
        Assert.Contains("analysis", key);
        Assert.Contains("2025", key);
    }

    [Fact]
    public void GenerateKey_AuthorTitleYearShort_Format()
    {
        var entry = new BibEntry(EntryType.Article)
        {
            Key = "old",
            Title = "Analysis Study",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smithson", "John"));

        var key = BibKeyGenerator.GenerateKey(entry, BibKeyGenerator.KeyFormat.AuthorTitleYearShort);

        // Should take first 3 letters
        Assert.Contains("smi", key);
        Assert.Contains("ana", key);
        Assert.Contains("2025", key);
    }

    [Fact]
    public void GenerateKey_AuthorJournalYear_WithJournal()
    {
        var entry = new BibEntry(EntryType.Article)
        {
            Key = "old",
            Title = "Test Article",
            Journal = "Journal of Computer Science",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John"));

        var key = BibKeyGenerator.GenerateKey(entry, BibKeyGenerator.KeyFormat.AuthorJournalYear);

        Assert.Contains("smith", key);
        Assert.Contains("jcs", key); // Abbreviation of journal
        Assert.Contains("2025", key);
    }

    [Fact]
    public void GenerateKey_AuthorJournalYear_NoJournal_UsesBookTitle()
    {
        var entry = new BibEntry(EntryType.InProceedings)
        {
            Key = "old",
            Title = "Test Paper",
            BookTitle = "Proceedings of Test Conference",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John"));

        var key = BibKeyGenerator.GenerateKey(entry, BibKeyGenerator.KeyFormat.AuthorJournalYear);

        Assert.Contains("smith", key);
        Assert.Contains("ptc", key); // Abbreviation of proceedings
        Assert.Contains("2025", key);
    }

    [Fact]
    public void GenerateKey_AuthorJournalYear_NoJournalOrBookTitle_UsesEntryType()
    {
        var entry = new BibEntry(EntryType.Misc)
        {
            Key = "old",
            Title = "Test",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John"));

        var key = BibKeyGenerator.GenerateKey(entry, BibKeyGenerator.KeyFormat.AuthorJournalYear);

        Assert.Contains("smith", key);
        Assert.Contains("misc", key);
        Assert.Contains("2025", key);
    }

    [Fact]
    public void GenerateKey_SkipsStopWords()
    {
        var entry = new BibEntry(EntryType.Article)
        {
            Key = "old",
            Title = "The Study of Analysis",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John"));

        var key = BibKeyGenerator.GenerateKey(entry, BibKeyGenerator.KeyFormat.AuthorTitleYear);

        // Should skip "The" and "of" and use "Study"
        Assert.Contains("study", key.ToLower());
        Assert.DoesNotContain("the", key.ToLower());
    }

    [Fact]
    public void GenerateKey_WithSpecialCharacters_Cleaned()
    {
        var entry = new BibEntry(EntryType.Article)
        {
            Key = "old",
            Title = "Test@Article#2025",
            Year = 2025
        };
        entry.AddAuthor(new Author("O'Brien", "John"));

        var key = BibKeyGenerator.GenerateKey(entry, BibKeyGenerator.KeyFormat.AuthorYear);

        // Special characters should be cleaned
        Assert.DoesNotContain("@", key);
        Assert.DoesNotContain("#", key);
        Assert.DoesNotContain("'", key);
    }

    [Fact]
    public void GenerateKey_WithConflict_AddsSuffix()
    {
        var entry = new BibEntry(EntryType.Article)
        {
            Key = "old",
            Title = "Test",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John"));

        var existingKeys = new[] { "smith2025" };
        var key = BibKeyGenerator.GenerateKey(entry, BibKeyGenerator.KeyFormat.AuthorYear, existingKeys);

        Assert.Equal("smith2025a", key);
    }

    [Fact]
    public void GenerateKey_WithMultipleConflicts_IncrementsAlphabetically()
    {
        var entry = new BibEntry(EntryType.Article)
        {
            Key = "old",
            Title = "Test",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John"));

        var existingKeys = new[] { "smith2025", "smith2025a", "smith2025b" };
        var key = BibKeyGenerator.GenerateKey(entry, BibKeyGenerator.KeyFormat.AuthorYear, existingKeys);

        Assert.Equal("smith2025c", key);
    }

    [Fact]
    public void GenerateKey_AfterAllLetters_UsesNumeric()
    {
        var entry = new BibEntry(EntryType.Article)
        {
            Key = "old",
            Title = "Test",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John"));

        var existingKeys = new List<string> { "smith2025" };
        for (char c = 'a'; c <= 'z'; c++)
        {
            existingKeys.Add($"smith2025{c}");
        }

        var key = BibKeyGenerator.GenerateKey(entry, BibKeyGenerator.KeyFormat.AuthorYear, existingKeys);

        Assert.Equal("smith20251", key);
    }

    [Fact]
    public void GenerateKey_MultipleDashes_Collapsed()
    {
        var entry = new BibEntry(EntryType.Article)
        {
            Key = "old",
            Title = "Test -- Article --- 2025",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John"));

        var key = BibKeyGenerator.GenerateKey(entry, BibKeyGenerator.KeyFormat.AuthorTitleYear);

        // Multiple dashes should be collapsed
        Assert.DoesNotContain("--", key);
        Assert.DoesNotContain("---", key);
    }

    [Fact]
    public void GenerateKeys_BatchGeneration_AvoidsCollisions()
    {
        var entries = new[]
        {
            new BibEntry(EntryType.Article) { Key = "", Title = "Test 1", Year = 2025 },
            new BibEntry(EntryType.Article) { Key = "", Title = "Test 2", Year = 2025 },
            new BibEntry(EntryType.Article) { Key = "", Title = "Test 3", Year = 2025 }
        };
        entries[0].AddAuthor(new Author("Smith", "John"));
        entries[1].AddAuthor(new Author("Smith", "Jane"));
        entries[2].AddAuthor(new Author("Smith", "Bob"));

        var keys = BibKeyGenerator.GenerateKeys(entries, BibKeyGenerator.KeyFormat.AuthorYear, preserveExistingKeys: false);

        Assert.Equal(3, keys.Count);
        var keyValues = keys.Values.ToList();
        Assert.Equal(3, keyValues.Distinct().Count()); // All keys should be unique
    }

    [Fact]
    public void GenerateKeys_PreserveExisting_KeepsSetKeys()
    {
        var entries = new[]
        {
            new BibEntry(EntryType.Article) { Key = "existing1", Title = "Test 1", Year = 2025 },
            new BibEntry(EntryType.Article) { Key = "", Title = "Test 2", Year = 2025 }
        };
        entries[0].AddAuthor(new Author("Smith", "John"));
        entries[1].AddAuthor(new Author("Doe", "Jane"));

        var keys = BibKeyGenerator.GenerateKeys(entries, BibKeyGenerator.KeyFormat.AuthorYear, preserveExistingKeys: true);

        Assert.Equal("existing1", keys[entries[0]]);
        Assert.NotEmpty(keys[entries[1]]);
    }

    [Fact]
    public void GenerateKeys_NoPreserve_RegeneratesAll()
    {
        var entries = new[]
        {
            new BibEntry(EntryType.Article) { Key = "existing1", Title = "Test 1", Year = 2025 },
            new BibEntry(EntryType.Article) { Key = "existing2", Title = "Test 2", Year = 2025 }
        };
        entries[0].AddAuthor(new Author("Smith", "John"));
        entries[1].AddAuthor(new Author("Doe", "Jane"));

        var keys = BibKeyGenerator.GenerateKeys(entries, BibKeyGenerator.KeyFormat.AuthorYear, preserveExistingKeys: false);

        Assert.NotEqual("existing1", keys[entries[0]]);
        Assert.NotEqual("existing2", keys[entries[1]]);
    }
}

