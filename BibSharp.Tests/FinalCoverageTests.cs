using BibSharp.Models;
using BibSharp.Utilities;

namespace BibSharp.Tests;

public class FinalCoverageTests
{
    [Fact]
    public void KeyGenerator_CleanKey_LeadingHyphens()
    {
        var entry = new BibEntry(EntryType.Article)
        {
            Key = "---test",
            Title = "Test",
            Year = 2025
        };
        entry.AddAuthor(new Author("---Smith---", "John"));

        var key = BibKeyGenerator.GenerateKey(entry);
        
        // Should remove leading/trailing hyphens
        Assert.False(key.StartsWith("-"));
        Assert.False(key.EndsWith("-"));
    }

    [Fact]
    public void KeyGenerator_TitleWithAllStopWords()
    {
        var entry = new BibEntry(EntryType.Article)
        {
            Key = "old",
            Title = "The Of And In For With On By At",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John"));

        var key = BibKeyGenerator.GenerateKey(entry, BibKeyGenerator.KeyFormat.AuthorTitleYear);
        
        // Should still generate a key even if all words are stop words
        Assert.NotEmpty(key);
    }

    [Fact]
    public void KeyGenerator_JournalAbbreviation_AllStopWords()
    {
        var entry = new BibEntry(EntryType.Article)
        {
            Key = "old",
            Title = "Test",
            Journal = "The of and",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John"));

        var key = BibKeyGenerator.GenerateKey(entry, BibKeyGenerator.KeyFormat.AuthorJournalYear);
        
        Assert.NotEmpty(key);
    }

    [Fact]
    public void KeyGenerator_EmptyKeyAfterCleaning()
    {
        var entry = new BibEntry(EntryType.Article)
        {
            Key = "old",
            Title = "@@@###$$$",
            Year = 2025
        };
        entry.AddAuthor(new Author("@@@", "###"));

        var key = BibKeyGenerator.GenerateKey(entry, BibKeyGenerator.KeyFormat.AuthorTitleYear);
        
        // Should return "unknownkey" or similar default
        Assert.NotEmpty(key);
    }

    [Fact]
    public void KeyGenerator_JournalWithOnlyStopWords()
    {
        var entry = new BibEntry(EntryType.Article)
        {
            Key = "old",
            Title = "Test",
            Journal = "a",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John"));

        var key = BibKeyGenerator.GenerateKey(entry, BibKeyGenerator.KeyFormat.AuthorJournalYear);
        
        Assert.Contains("smith", key);
    }

    [Fact]
    public void KeyGenerator_TitleWithLatexCommands()
    {
        var entry = new BibEntry(EntryType.Article)
        {
            Key = "old",
            Title = "\\textbf{Bold} \\emph{Italic} Study",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John"));

        var key = BibKeyGenerator.GenerateKey(entry, BibKeyGenerator.KeyFormat.AuthorTitleYear);
        
        // LaTeX commands should be removed, backslash should not appear
        Assert.DoesNotContain("\\", key);
        // Should contain some part of the title
        Assert.NotEmpty(key);
    }

    [Fact]
    public void Matcher_AuthorMatch_SingleAuthorBoth()
    {
        var matcher = new BibEntryMatcher();
        var entry = new BibEntry { Key = "e1", Title = "Similar Title", Year = 2025 };
        entry.AddAuthor(new Author("Smith", "John"));
        
        var candidate = new BibEntry { Key = "c1", Title = "Similar Title", Year = 2025 };
        candidate.AddAuthor(new Author("Smith", "Jane")); // Same last name

        var match = matcher.FindMatch(entry, new[] { candidate });
        
        Assert.NotNull(match);
    }

    [Fact]
    public void Matcher_MultipleAuthors_PartialMatch()
    {
        var matcher = new BibEntryMatcher();
        var entry = new BibEntry { Key = "e1", Title = "Test Article", Year = 2025 };
        entry.AddAuthor(new Author("Smith", "John"));
        entry.AddAuthor(new Author("Doe", "Jane"));
        entry.AddAuthor(new Author("Johnson", "Bob"));
        entry.AddAuthor(new Author("Williams", "Alice"));
        entry.AddAuthor(new Author("Brown", "Charlie"));
        
        var candidate = new BibEntry { Key = "c1", Title = "Test Article", Year = 2025 };
        candidate.AddAuthor(new Author("Smith", "John"));
        candidate.AddAuthor(new Author("Doe", "Jane"));
        candidate.AddAuthor(new Author("Johnson", "Bob"));
        candidate.AddAuthor(new Author("Different", "Author"));
        candidate.AddAuthor(new Author("Other", "Person"));

        // At least 60% should match (3 out of 5)
        var match = matcher.FindMatch(entry, new[] { candidate });
        
        Assert.NotNull(match);
    }

    [Fact]
    public void Matcher_TitleContainment_Forward()
    {
        var matcher = new BibEntryMatcher();
        var entry = new BibEntry { Key = "e1", Title = "Short", Year = 2025 };
        entry.AddAuthor(new Author("Smith", "John"));
        
        var candidate = new BibEntry { Key = "c1", Title = "Short Title Extended", Year = 2025 };
        candidate.AddAuthor(new Author("Smith", "John"));

        var match = matcher.FindMatch(entry, new[] { candidate });
        
        Assert.NotNull(match);
    }

    [Fact]
    public void Matcher_TitleContainment_Reverse()
    {
        var matcher = new BibEntryMatcher();
        var entry = new BibEntry { Key = "e1", Title = "Long Title Extended Version", Year = 2025 };
        entry.AddAuthor(new Author("Smith", "John"));
        
        var candidate = new BibEntry { Key = "c1", Title = "Long", Year = 2025 };
        candidate.AddAuthor(new Author("Smith", "John"));

        var match = matcher.FindMatch(entry, new[] { candidate });
        
        Assert.NotNull(match);
    }

    [Fact]
    public void Matcher_TitleSimilarity_LongTitles()
    {
        var matcher = new BibEntryMatcher();
        var entry = new BibEntry { Key = "e1", Title = "A Comprehensive Study of Machine Learning Algorithms", Year = 2025 };
        entry.AddAuthor(new Author("Smith", "John"));
        
        var candidate = new BibEntry { Key = "c1", Title = "A Comprehensive Study of Deep Learning Methods", Year = 2025 };
        candidate.AddAuthor(new Author("Smith", "John"));

        var match = matcher.FindMatch(entry, new[] { candidate });
        
        // High word overlap should match
        Assert.NotNull(match);
    }
}

