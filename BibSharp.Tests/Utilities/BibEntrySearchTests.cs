using BibSharp.Models;
using BibSharp.Utilities;

namespace BibSharp.Tests.Utilities;

public class BibEntrySearchTests
{
    [Fact]
    public void FindMatch_Extension_ReturnsCorrectMatch()
    {
        // Arrange
        var entry = new BibEntry { Key = "smith2023", Doi = "10.1234/abcd.5678" };
            
        var candidates = new List<BibEntry>
        {
            new BibEntry { Key = "other1", Doi = "10.9999/xxxx.9999" },
            new BibEntry { Key = "match1", Doi = "10.1234/abcd.5678" },
            new BibEntry { Key = "other2", Doi = "10.8888/yyyy.8888" }
        };
            
        // Act
        var result = entry.FindMatch(candidates);
            
        // Assert
        Assert.NotNull(result);
        Assert.Equal("match1", result.Key);
    }
        
    [Fact]
    public void FindMatches_Extension_ReturnsCorrectMatches()
    {
        // Arrange
        var entries = new List<BibEntry>
        {
            new BibEntry { Key = "entry1", Doi = "10.1234/abcd.5678" },
            new BibEntry { Key = "entry2", Doi = "10.5678/efgh.1234" },
            new BibEntry { Key = "entry3", Doi = "10.9999/ijkl.5678" }
        };
            
        var candidates = new List<BibEntry>
        {
            new BibEntry { Key = "match1", Doi = "10.1234/abcd.5678" },
            new BibEntry { Key = "match2", Doi = "10.5678/efgh.1234" },
            new BibEntry { Key = "other", Doi = "10.8888/yyyy.8888" }
        };
            
        // Act
        var results = entries.FindMatches(candidates);
            
        // Assert
        Assert.Equal(3, results.Count);
        Assert.Equal("match1", results[entries[0]]?.Key);
        Assert.Equal("match2", results[entries[1]]?.Key);
        Assert.Null(results[entries[2]]);
    }
        
    [Fact]
    public void FindDuplicates_Extension_ReturnsCorrectGroups()
    {
        // Arrange
        var entries = new List<BibEntry>
        {
            new BibEntry { Key = "paper1", Doi = "10.1234/abcd.5678" },
            new BibEntry { Key = "paper2", Doi = "10.1234/abcd.5678" },  // Duplicate of paper1
            new BibEntry { Key = "paper3", Title = "Machine Learning", Year = 2023 },
            new BibEntry { Key = "paper4", Title = "Machine Learning Applications", Year = 2023 }, // Similar to paper3
            new BibEntry { Key = "paper5", Title = "Deep Learning", Year = 2023 },
            new BibEntry { Key = "paper6", Doi = "10.9999/zzzz.9999" }
        };
            
        // Add authors
        entries[2].AddAuthor(new Author("Smith", "John"));
        entries[3].AddAuthor(new Author("Smith", "John"));
        entries[4].AddAuthor(new Author("Brown", "Robert"));
            
        // Act
        var duplicateGroups = entries.FindDuplicates().ToList();
            
        // Assert
        Assert.Equal(2, duplicateGroups.Count);
            
        // First duplicate group should be paper1 and paper2 (DOI match)
        var doiGroup = duplicateGroups.FirstOrDefault(g => g.Key.Doi == "10.1234/abcd.5678");
        Assert.NotNull(doiGroup);
        Assert.Equal(2, doiGroup.Count());
        Assert.Contains(doiGroup, e => e.Key == "paper1");
        Assert.Contains(doiGroup, e => e.Key == "paper2");
            
        // Second duplicate group should be paper3 and paper4 (Author+Title+Year match)
        var authorTitleGroup = duplicateGroups.FirstOrDefault(g => g.Key.Title != null && g.Key.Title.Contains("Machine Learning"));
        Assert.NotNull(authorTitleGroup);
        Assert.Equal(2, authorTitleGroup.Count());
        Assert.Contains(authorTitleGroup, e => e.Key == "paper3");
        Assert.Contains(authorTitleGroup, e => e.Key == "paper4");
    }
}