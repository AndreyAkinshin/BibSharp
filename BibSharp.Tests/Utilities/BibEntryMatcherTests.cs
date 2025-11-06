using BibSharp.Models;
using BibSharp.Utilities;

namespace BibSharp.Tests.Utilities;

public class BibEntryMatcherTests
{
    [Fact]
    public void FindMatch_MatchesByDoi_ReturnsCorrectEntry()
    {
        // Arrange
        var entry = new BibEntry
        {
            Key = "smith2023",
            Doi = "10.1234/abcd.5678"
        };
            
        var candidates = new List<BibEntry>
        {
            new BibEntry { Key = "other1", Doi = "10.9999/xxxx.9999" },
            new BibEntry { Key = "match1", Doi = "10.1234/abcd.5678" },
            new BibEntry { Key = "other2", Doi = "10.8888/yyyy.8888" }
        };
            
        // Act
        var matcher = new BibEntryMatcher();
        var result = matcher.FindMatch(entry, candidates);
            
        // Assert
        Assert.NotNull(result);
        Assert.Equal("match1", result.Key);
    }
        
    [Fact]
    public void FindMatch_MatchesByDoi_WithDifferentFormatting_ReturnsCorrectEntry()
    {
        // Arrange
        var entry = new BibEntry
        {
            Key = "smith2023",
            Doi = "https://doi.org/10.1234/abcd.5678"
        };
            
        var candidates = new List<BibEntry>
        {
            new BibEntry { Key = "other1", Doi = "10.9999/xxxx.9999" },
            new BibEntry { Key = "match1", Doi = "doi:10.1234/abcd.5678" },
            new BibEntry { Key = "other2", Doi = "10.8888/yyyy.8888" }
        };
            
        // Act
        var matcher = new BibEntryMatcher();
        var result = matcher.FindMatch(entry, candidates);
            
        // Assert
        Assert.NotNull(result);
        Assert.Equal("match1", result.Key);
    }
        
    [Fact]
    public void FindMatch_MatchesByUrl_ReturnsCorrectEntry()
    {
        // Arrange
        var entry = new BibEntry { Key = "smith2023" };
        entry["url"] = "https://example.com/paper";
            
        var candidates = new List<BibEntry>
        {
            new BibEntry { Key = "other1" },
            new BibEntry { Key = "match1" },
            new BibEntry { Key = "other2" }
        };
            
        candidates[0]["url"] = "https://example.com/other";
        candidates[1]["url"] = "https://example.com/paper";
        candidates[2]["url"] = "https://othersite.com/paper";
            
        // Act
        var matcher = new BibEntryMatcher();
        var result = matcher.FindMatch(entry, candidates);
            
        // Assert
        Assert.NotNull(result);
        Assert.Equal("match1", result.Key);
    }
        
    [Fact]
    public void FindMatch_MatchesByAuthorTitleYear_ReturnsCorrectEntry()
    {
        // Arrange
        var entry = new BibEntry
        {
            Key = "smith2023",
            Title = "Machine Learning Applications",
            Year = 2023
        };
        entry.AddAuthor(new Author("Smith", "John"));
        entry.AddAuthor(new Author("Johnson", "Alice"));
            
        var candidates = new List<BibEntry>
        {
            new BibEntry { Key = "other1", Title = "Different Title", Year = 2023 },
            new BibEntry { Key = "match1", Title = "Machine Learning Applications in Science", Year = 2023 },
            new BibEntry { Key = "other2", Title = "Machine Learning Applications", Year = 2022 }
        };
            
        candidates[0].AddAuthor(new Author("Smith", "John"));
        candidates[1].AddAuthor(new Author("Smith", "John"));
        candidates[1].AddAuthor(new Author("Johnson", "Alice"));
        candidates[2].AddAuthor(new Author("Brown", "Robert"));
            
        // Act
        var matcher = new BibEntryMatcher();
        var result = matcher.FindMatch(entry, candidates);
            
        // Assert
        Assert.NotNull(result);
        Assert.Equal("match1", result.Key);
    }
        
    [Fact]
    public void FindMatch_NoMatch_ReturnsNull()
    {
        // Arrange
        var entry = new BibEntry
        {
            Key = "smith2023",
            Title = "Machine Learning Applications",
            Year = 2023
        };
        entry.AddAuthor(new Author("Smith", "John"));
            
        var candidates = new List<BibEntry>
        {
            new BibEntry { Key = "other1", Title = "Different Title", Year = 2023 },
            new BibEntry { Key = "other2", Title = "Machine Learning Applications", Year = 2021 }
        };
            
        candidates[0].AddAuthor(new Author("Brown", "Robert"));
        candidates[1].AddAuthor(new Author("Jones", "Mark"));
            
        // Act
        var matcher = new BibEntryMatcher();
        var result = matcher.FindMatch(entry, candidates);
            
        // Assert
        Assert.Null(result);
    }
}