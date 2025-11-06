using BibSharp.Models;
using BibSharp.Utilities;

namespace BibSharp.Tests.Utilities;

public class BibEntrySearchComprehensiveTests
{
    [Fact]
    public void FindMatch_ExtensionMethod_WorksCorrectly()
    {
        var entry = new BibEntry { Key = "test1", Doi = "10.1234/test" };
        var candidates = new[]
        {
            new BibEntry { Key = "candidate1", Doi = "10.1234/test" },
            new BibEntry { Key = "candidate2", Doi = "10.5678/other" }
        };

        var match = entry.FindMatch(candidates);

        Assert.NotNull(match);
        Assert.Equal("candidate1", match.Key);
    }

    [Fact]
    public void FindMatches_MultipleEntries_ReturnsCorrectMatches()
    {
        var entries = new[]
        {
            new BibEntry { Key = "entry1", Doi = "10.1234/test1" },
            new BibEntry { Key = "entry2", Doi = "10.1234/test2" }
        };
        
        var candidates = new[]
        {
            new BibEntry { Key = "candidate1", Doi = "10.1234/test1" },
            new BibEntry { Key = "candidate2", Doi = "10.5678/other" }
        };

        var matches = entries.FindMatches(candidates);

        Assert.Equal(2, matches.Count);
        Assert.NotNull(matches[entries[0]]);
        Assert.Equal("candidate1", matches[entries[0]]?.Key);
        Assert.Null(matches[entries[1]]);
    }

    [Fact]
    public void FindDuplicates_NoDuplicates_ReturnsEmpty()
    {
        var entries = new[]
        {
            new BibEntry { Key = "entry1", Doi = "10.1234/test1" },
            new BibEntry { Key = "entry2", Doi = "10.1234/test2" }
        };

        var duplicates = entries.FindDuplicates().ToList();

        Assert.Empty(duplicates);
    }

    [Fact]
    public void FindDuplicates_WithDuplicates_ReturnsGroups()
    {
        var entries = new[]
        {
            new BibEntry { Key = "entry1", Doi = "10.1234/test" },
            new BibEntry { Key = "entry2", Doi = "10.1234/test" }, // Duplicate by DOI
            new BibEntry { Key = "entry3", Doi = "10.5678/other" }
        };

        var duplicates = entries.FindDuplicates().ToList();

        Assert.Single(duplicates);
        Assert.Equal(2, duplicates[0].Count());
    }

    [Fact]
    public void FindDuplicates_ByAuthorTitleYear_ReturnsGroups()
    {
        var entry1 = new BibEntry { Key = "entry1", Title = "Test Article", Year = 2025 };
        entry1.AddAuthor(new Author("Smith", "John"));
        
        var entry2 = new BibEntry { Key = "entry2", Title = "Test Article", Year = 2025 };
        entry2.AddAuthor(new Author("Smith", "John"));
        
        var entry3 = new BibEntry { Key = "entry3", Title = "Different Article", Year = 2025 };
        entry3.AddAuthor(new Author("Doe", "Jane"));

        var entries = new[] { entry1, entry2, entry3 };
        var duplicates = entries.FindDuplicates().ToList();

        Assert.Single(duplicates);
        Assert.Equal(2, duplicates[0].Count());
    }

    [Fact]
    public void FindDuplicates_ThreeEntriesSameGroup_ReturnsOneGroup()
    {
        var entry1 = new BibEntry { Key = "entry1", Doi = "10.1234/test" };
        var entry2 = new BibEntry { Key = "entry2", Doi = "10.1234/test" };
        var entry3 = new BibEntry { Key = "entry3", Doi = "10.1234/test" };

        var entries = new[] { entry1, entry2, entry3 };
        var duplicates = entries.FindDuplicates().ToList();

        Assert.Single(duplicates);
        Assert.Equal(3, duplicates[0].Count());
    }

    [Fact]
    public void FindDuplicates_MultipleGroups()
    {
        var entry1 = new BibEntry { Key = "entry1", Doi = "10.1234/test1" };
        var entry2 = new BibEntry { Key = "entry2", Doi = "10.1234/test1" };
        var entry3 = new BibEntry { Key = "entry3", Doi = "10.5678/test2" };
        var entry4 = new BibEntry { Key = "entry4", Doi = "10.5678/test2" };

        var entries = new[] { entry1, entry2, entry3, entry4 };
        var duplicates = entries.FindDuplicates().ToList();

        Assert.Equal(2, duplicates.Count);
    }

    [Fact]
    public void FindDuplicates_EmptyCollection_ReturnsEmpty()
    {
        var entries = Array.Empty<BibEntry>();
        var duplicates = entries.FindDuplicates().ToList();

        Assert.Empty(duplicates);
    }

    [Fact]
    public void FindDuplicates_SingleEntry_ReturnsEmpty()
    {
        var entries = new[]
        {
            new BibEntry { Key = "entry1", Doi = "10.1234/test" }
        };

        var duplicates = entries.FindDuplicates().ToList();

        Assert.Empty(duplicates);
    }

    [Fact]
    public void FindMatch_NoDoi_NoUrl_NoAuthors_ReturnsNull()
    {
        var matcher = new BibEntryMatcher();
        var entry = new BibEntry { Key = "test1", Title = "Test" };
        var candidates = new[]
        {
            new BibEntry { Key = "candidate1", Title = "Test" }
        };

        var match = matcher.FindMatch(entry, candidates);

        Assert.Null(match);
    }

    [Fact]
    public void FindMatches_WithNoCandidates_AllNull()
    {
        var entries = new[]
        {
            new BibEntry { Key = "entry1", Doi = "10.1234/test" },
            new BibEntry { Key = "entry2", Doi = "10.5678/test" }
        };

        var matches = entries.FindMatches(Array.Empty<BibEntry>());

        Assert.Equal(2, matches.Count);
        Assert.Null(matches[entries[0]]);
        Assert.Null(matches[entries[1]]);
    }
}

