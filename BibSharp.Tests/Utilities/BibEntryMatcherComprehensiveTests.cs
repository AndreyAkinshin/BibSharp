using BibSharp.Models;
using BibSharp.Utilities;

namespace BibSharp.Tests.Utilities;

public class BibEntryMatcherComprehensiveTests
{
    [Fact]
    public void FindMatch_ByDoi_ReturnsMatch()
    {
        var matcher = new BibEntryMatcher();
        var entry = new BibEntry { Key = "test1", Doi = "10.1234/test" };
        var candidates = new[]
        {
            new BibEntry { Key = "candidate1", Doi = "10.1234/test" },
            new BibEntry { Key = "candidate2", Doi = "10.5678/other" }
        };

        var match = matcher.FindMatch(entry, candidates);

        Assert.NotNull(match);
        Assert.Equal("candidate1", match.Key);
    }

    [Fact]
    public void FindMatch_ByDoi_WithPrefix_ReturnsMatch()
    {
        var matcher = new BibEntryMatcher();
        var entry = new BibEntry { Key = "test1", Doi = "https://doi.org/10.1234/test" };
        var candidates = new[]
        {
            new BibEntry { Key = "candidate1", Doi = "10.1234/test" }
        };

        var match = matcher.FindMatch(entry, candidates);

        Assert.NotNull(match);
        Assert.Equal("candidate1", match.Key);
    }

    [Fact]
    public void FindMatch_ByDoi_WithDoiPrefix_ReturnsMatch()
    {
        var matcher = new BibEntryMatcher();
        var entry = new BibEntry { Key = "test1", Doi = "doi:10.1234/test" };
        var candidates = new[]
        {
            new BibEntry { Key = "candidate1", Doi = "10.1234/test" }
        };

        var match = matcher.FindMatch(entry, candidates);

        Assert.NotNull(match);
        Assert.Equal("candidate1", match.Key);
    }

    [Fact]
    public void FindMatch_ByUrl_ReturnsMatch()
    {
        var matcher = new BibEntryMatcher();
        var entry = new BibEntry { Key = "test1", Url = "https://example.com/article" };
        var candidates = new[]
        {
            new BibEntry { Key = "candidate1", Url = "https://example.com/article" },
            new BibEntry { Key = "candidate2", Url = "https://other.com/article" }
        };

        var match = matcher.FindMatch(entry, candidates);

        Assert.NotNull(match);
        Assert.Equal("candidate1", match.Key);
    }

    [Fact]
    public void FindMatch_ByUrl_WithTrailingSlash_ReturnsMatch()
    {
        var matcher = new BibEntryMatcher();
        var entry = new BibEntry { Key = "test1", Url = "https://example.com/article/" };
        var candidates = new[]
        {
            new BibEntry { Key = "candidate1", Url = "https://example.com/article" }
        };

        var match = matcher.FindMatch(entry, candidates);

        Assert.NotNull(match);
        Assert.Equal("candidate1", match.Key);
    }

    [Fact]
    public void FindMatch_ByAuthorTitleYear_ReturnsMatch()
    {
        var matcher = new BibEntryMatcher();
        var entry = new BibEntry { Key = "test1", Title = "Test Article", Year = 2025 };
        entry.AddAuthor(new Author("Smith", "John"));
        
        var candidates = new[]
        {
            new BibEntry { Key = "candidate1", Title = "Test Article", Year = 2025 }
        };
        candidates[0].AddAuthor(new Author("Smith", "John"));

        var match = matcher.FindMatch(entry, candidates);

        Assert.NotNull(match);
        Assert.Equal("candidate1", match.Key);
    }

    [Fact]
    public void FindMatch_SimilarTitle_ReturnsMatch()
    {
        var matcher = new BibEntryMatcher();
        var entry = new BibEntry { Key = "test1", Title = "A Study of Machine Learning", Year = 2025 };
        entry.AddAuthor(new Author("Smith", "John"));
        
        var candidates = new[]
        {
            new BibEntry { Key = "candidate1", Title = "A Study of Machine Learning Algorithms", Year = 2025 }
        };
        candidates[0].AddAuthor(new Author("Smith", "John"));

        var match = matcher.FindMatch(entry, candidates);

        Assert.NotNull(match);
    }

    [Fact]
    public void FindMatch_ByKey_ShortKey_NoMatch()
    {
        var matcher = new BibEntryMatcher();
        var entry = new BibEntry { Key = "ab" }; // Too short (<=3)
        var candidates = new[]
        {
            new BibEntry { Key = "ab" }
        };

        var match = matcher.FindMatch(entry, candidates);

        // Short keys are not considered meaningful for matching
        Assert.Null(match);
    }

    [Fact]
    public void FindMatch_ByKey_LongKey_ReturnsMatch()
    {
        var matcher = new BibEntryMatcher();
        var entry = new BibEntry { Key = "smith2025" };
        var candidates = new[]
        {
            new BibEntry { Key = "smith2025" }
        };

        var match = matcher.FindMatch(entry, candidates);

        Assert.NotNull(match);
    }

    [Fact]
    public void FindMatch_NoMatchingCriteria_ReturnsNull()
    {
        var matcher = new BibEntryMatcher();
        var entry = new BibEntry { Key = "test1", Title = "Different Title" };
        var candidates = new[]
        {
            new BibEntry { Key = "candidate1", Title = "Other Title" }
        };

        var match = matcher.FindMatch(entry, candidates);

        Assert.Null(match);
    }

    [Fact]
    public void FindMatch_EmptyCandidates_ReturnsNull()
    {
        var matcher = new BibEntryMatcher();
        var entry = new BibEntry { Key = "test1", Doi = "10.1234/test" };

        var match = matcher.FindMatch(entry, Array.Empty<BibEntry>());

        Assert.Null(match);
    }

    [Fact]
    public void FindMatch_MultipleAuthors_MatchesCorrectly()
    {
        var matcher = new BibEntryMatcher();
        var entry = new BibEntry { Key = "test1", Title = "Test Article", Year = 2025 };
        entry.AddAuthor(new Author("Smith", "John"));
        entry.AddAuthor(new Author("Doe", "Jane"));
        entry.AddAuthor(new Author("Johnson", "Bob"));
        
        var candidates = new[]
        {
            new BibEntry { Key = "candidate1", Title = "Test Article", Year = 2025 }
        };
        candidates[0].AddAuthor(new Author("Smith", "John"));
        candidates[0].AddAuthor(new Author("Doe", "Jane"));
        candidates[0].AddAuthor(new Author("Williams", "Alice")); // Different third author

        var match = matcher.FindMatch(entry, candidates);

        Assert.NotNull(match); // Should match based on 60% author overlap
    }

    [Fact]
    public void FindMatch_DifferentFirstAuthor_NoMatch()
    {
        var matcher = new BibEntryMatcher();
        var entry = new BibEntry { Key = "test1", Title = "Test Article", Year = 2025 };
        entry.AddAuthor(new Author("Smith", "John"));
        
        var candidates = new[]
        {
            new BibEntry { Key = "candidate1", Title = "Test Article", Year = 2025 }
        };
        candidates[0].AddAuthor(new Author("Doe", "Jane"));

        var match = matcher.FindMatch(entry, candidates);

        Assert.Null(match); // First author must match
    }

    [Fact]
    public void FindMatch_NoAuthors_NoMatch()
    {
        var matcher = new BibEntryMatcher();
        var entry = new BibEntry { Key = "test1", Title = "Test Article", Year = 2025 };
        var candidates = new[]
        {
            new BibEntry { Key = "candidate1", Title = "Test Article", Year = 2025 }
        };

        var match = matcher.FindMatch(entry, candidates);

        Assert.Null(match); // Need authors for title/year matching
    }

    [Fact]
    public void FindMatch_LowTitleSimilarity_NoMatch()
    {
        var matcher = new BibEntryMatcher();
        var entry = new BibEntry { Key = "test1", Title = "Completely Different", Year = 2025 };
        entry.AddAuthor(new Author("Smith", "John"));
        
        var candidates = new[]
        {
            new BibEntry { Key = "candidate1", Title = "Nothing Similar", Year = 2025 }
        };
        candidates[0].AddAuthor(new Author("Smith", "John"));

        var match = matcher.FindMatch(entry, candidates);

        Assert.Null(match); // Title similarity too low
    }
}

