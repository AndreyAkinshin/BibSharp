using BibSharp.Models;
using BibSharp.Utilities;

namespace BibSharp.Tests.Utilities;

public class BibKeyGeneratorTests
{
    [Fact]
    public void GenerateKey_AuthorYearFormat_ReturnsCorrectKey()
    {
        // Arrange
        var entry = new BibEntry("article")
        {
            Title = "Test Article",
            Year = 2023
        };
        entry.AddAuthor(new Author("Smith", "John"));
            
        // Act
        string key = BibKeyGenerator.GenerateKey(entry, BibKeyGenerator.KeyFormat.AuthorYear);
            
        // Assert
        Assert.Equal("smith2023", key);
    }
        
    [Fact]
    public void GenerateKey_AuthorYearFormat_WithNoAuthor_UsesNoauthorPrefix()
    {
        // Arrange
        var entry = new BibEntry("article")
        {
            Title = "Test Article",
            Year = 2023
        };
            
        // Act
        string key = BibKeyGenerator.GenerateKey(entry, BibKeyGenerator.KeyFormat.AuthorYear);
            
        // Assert
        Assert.Equal("noauthor2023", key);
    }
        
    [Fact]
    public void GenerateKey_AuthorYearFormat_WithNoYear_UsesNodatePrefix()
    {
        // Arrange
        var entry = new BibEntry("article")
        {
            Title = "Test Article"
        };
        entry.AddAuthor(new Author("Smith", "John"));
            
        // Act
        string key = BibKeyGenerator.GenerateKey(entry, BibKeyGenerator.KeyFormat.AuthorYear);
            
        // Assert
        Assert.Equal("smithnodate", key);
    }
        
    [Fact]
    public void GenerateKey_AuthorTitleYearFormat_ReturnsCorrectKey()
    {
        // Arrange
        var entry = new BibEntry("article")
        {
            Title = "A Study of BibTeX Keys",
            Year = 2023
        };
        entry.AddAuthor(new Author("Smith", "John"));
            
        // Act
        string key = BibKeyGenerator.GenerateKey(entry, BibKeyGenerator.KeyFormat.AuthorTitleYear);
            
        // Assert
        Assert.Equal("smithstudy2023", key);
    }
        
    [Fact]
    public void GenerateKey_AuthorTitleYearShortFormat_ReturnsCorrectKey()
    {
        // Arrange
        var entry = new BibEntry("article")
        {
            Title = "A Study of BibTeX Keys",
            Year = 2023
        };
        entry.AddAuthor(new Author("Smith", "John"));
            
        // Act
        string key = BibKeyGenerator.GenerateKey(entry, BibKeyGenerator.KeyFormat.AuthorTitleYearShort);
            
        // Assert
        Assert.Equal("smistu2023", key);
    }
        
    [Fact]
    public void GenerateKey_AuthorJournalYearFormat_ReturnsCorrectKey()
    {
        // Arrange
        var entry = new BibEntry("article")
        {
            Title = "Test Article",
            Journal = "Journal of Computer Science",
            Year = 2023
        };
        entry.AddAuthor(new Author("Smith", "John"));
            
        // Act
        string key = BibKeyGenerator.GenerateKey(entry, BibKeyGenerator.KeyFormat.AuthorJournalYear);
            
        // Assert
        Assert.Equal("smithjcs2023", key);
    }
        
    [Fact]
    public void GenerateKey_WithExistingKeys_AddsLetterSuffix()
    {
        // Arrange
        var entry = new BibEntry("article")
        {
            Title = "Test Article",
            Year = 2023
        };
        entry.AddAuthor(new Author("Smith", "John"));
            
        var existingKeys = new List<string> { "smith2023" };
            
        // Act
        string key = BibKeyGenerator.GenerateKey(entry, BibKeyGenerator.KeyFormat.AuthorYear, existingKeys);
            
        // Assert
        Assert.Equal("smith2023a", key);
    }
        
    [Fact]
    public void GenerateKey_WithMultipleExistingKeys_IncrementsSuffix()
    {
        // Arrange
        var entry = new BibEntry("article")
        {
            Title = "Test Article",
            Year = 2023
        };
        entry.AddAuthor(new Author("Smith", "John"));
            
        var existingKeys = new List<string> { "smith2023", "smith2023a" };
            
        // Act
        string key = BibKeyGenerator.GenerateKey(entry, BibKeyGenerator.KeyFormat.AuthorYear, existingKeys);
            
        // Assert
        Assert.Equal("smith2023b", key);
    }
        
    [Fact]
    public void GenerateKeys_PreservesExistingKeys_WhenSpecified()
    {
        // Arrange
        var entries = new List<BibEntry>
        {
            new BibEntry("article") 
            { 
                Key = "existing-key-1", 
                Title = "First Article",
                Year = 2023
            },
            new BibEntry("article") 
            { 
                Title = "Second Article",
                Year = 2023
            }
        };
        entries[0].AddAuthor(new Author("Smith", "John"));
        entries[1].AddAuthor(new Author("Smith", "John"));
            
        // Act
        var keyMap = BibKeyGenerator.GenerateKeys(entries, BibKeyGenerator.KeyFormat.AuthorYear, preserveExistingKeys: true);
            
        // Assert
        Assert.Equal(2, keyMap.Count);
        Assert.Equal("existing-key-1", keyMap[entries[0]]);
        Assert.Equal("smith2023", keyMap[entries[1]]);
    }
        
    [Fact]
    public void GenerateKeys_OverwritesExistingKeys_WhenSpecified()
    {
        // Arrange
        var entries = new List<BibEntry>
        {
            new BibEntry("article") 
            { 
                Key = "existing-key-1", 
                Title = "First Article",
                Year = 2023
            },
            new BibEntry("article") 
            { 
                Title = "Second Article",
                Year = 2023
            }
        };
        entries[0].AddAuthor(new Author("Smith", "John"));
        entries[1].AddAuthor(new Author("Jones", "David"));
            
        // Act
        var keyMap = BibKeyGenerator.GenerateKeys(entries, BibKeyGenerator.KeyFormat.AuthorYear, preserveExistingKeys: false);
            
        // Assert
        Assert.Equal(2, keyMap.Count);
        Assert.Equal("smith2023", keyMap[entries[0]]);
        Assert.Equal("jones2023", keyMap[entries[1]]);
    }
        
    [Fact]
    public void GenerateKeys_HandlesKeyCollisions_WithinGeneratedKeys()
    {
        // Arrange
        var entries = new List<BibEntry>
        {
            new BibEntry("article") 
            { 
                Title = "First Article", 
                Year = 2023
            },
            new BibEntry("article") 
            { 
                Title = "Second Article",
                Year = 2023
            }
        };
        entries[0].AddAuthor(new Author("Smith", "John"));
        entries[1].AddAuthor(new Author("Smith", "Jane"));
            
        // Act
        var keyMap = BibKeyGenerator.GenerateKeys(entries, BibKeyGenerator.KeyFormat.AuthorYear);
            
        // Assert
        Assert.Equal(2, keyMap.Count);
        Assert.Equal("smith2023", keyMap[entries[0]]);
        Assert.Equal("smith2023a", keyMap[entries[1]]);
    }
        
    [Fact]
    public void GenerateKeys_HandlesMultipleKeyCollisions()
    {
        // Arrange
        var entries = new List<BibEntry>
        {
            new BibEntry("article") { Title = "First Article", Year = 2023 },
            new BibEntry("article") { Title = "Second Article", Year = 2023 },
            new BibEntry("article") { Title = "Third Article", Year = 2023 }
        };
        entries[0].AddAuthor(new Author("Smith", "John"));
        entries[1].AddAuthor(new Author("Smith", "Jane"));
        entries[2].AddAuthor(new Author("Smith", "Robert"));
            
        // Act
        var keyMap = BibKeyGenerator.GenerateKeys(entries, BibKeyGenerator.KeyFormat.AuthorYear);
            
        // Assert
        Assert.Equal(3, keyMap.Count);
        Assert.Equal("smith2023", keyMap[entries[0]]);
        Assert.Equal("smith2023a", keyMap[entries[1]]);
        Assert.Equal("smith2023b", keyMap[entries[2]]);
    }
}