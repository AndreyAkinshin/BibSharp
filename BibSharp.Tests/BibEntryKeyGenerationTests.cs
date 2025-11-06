using BibSharp.Models;
using BibSharp.Utilities;

namespace BibSharp.Tests;

public class BibEntryKeyGenerationTests
{
    [Fact]
    public void GenerateKey_SetsKeyProperty_WhenAutoSetIsTrue()
    {
        // Arrange
        var entry = new BibEntry("article")
        {
            Title = "Test Article",
            Year = 2023
        };
        entry.AddAuthor(new Author("Smith", "John"));
            
        // Act
        string key = entry.GenerateKey(BibKeyGenerator.KeyFormat.AuthorYear, null, true);
            
        // Assert
        Assert.Equal("smith2023", key);
        Assert.Equal("smith2023", entry.Key);
    }
        
    [Fact]
    public void GenerateKey_DoesNotSetKeyProperty_WhenAutoSetIsFalse()
    {
        // Arrange
        var entry = new BibEntry("article")
        {
            Key = "original-key",
            Title = "Test Article",
            Year = 2023
        };
        entry.AddAuthor(new Author("Smith", "John"));
            
        // Act
        string key = entry.GenerateKey(BibKeyGenerator.KeyFormat.AuthorYear, null, false);
            
        // Assert
        Assert.Equal("smith2023", key);
        Assert.Equal("original-key", entry.Key);
    }
        
    [Fact]
    public void GenerateKey_HandlesExistingKeys()
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
        string key = entry.GenerateKey(BibKeyGenerator.KeyFormat.AuthorYear, existingKeys, true);
            
        // Assert
        Assert.Equal("smith2023b", key);
        Assert.Equal("smith2023b", entry.Key);
    }
        
    [Fact]
    public void RegenerateKeys_SetsKeys_ForAllEntries()
    {
        // Arrange
        var entries = new List<BibEntry>
        {
            new BibEntry("article") { Title = "First Article", Year = 2023 },
            new BibEntry("article") { Title = "Second Article", Year = 2023 }
        };
        entries[0].AddAuthor(new Author("Smith", "John"));
        entries[1].AddAuthor(new Author("Jones", "David"));
            
        // Act
        var keyMap = BibEntry.RegenerateKeys(entries, BibKeyGenerator.KeyFormat.AuthorYear);
            
        // Assert
        Assert.Equal(2, keyMap.Count);
        Assert.Equal("smith2023", entries[0].Key);
        Assert.Equal("jones2023", entries[1].Key);
    }
        
    [Fact]
    public void RegenerateKeys_PreservesExistingKeys_WhenSpecified()
    {
        // Arrange
        var entries = new List<BibEntry>
        {
            new BibEntry("article") 
            { 
                Key = "existing-key", 
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
        var keyMap = BibEntry.RegenerateKeys(entries, BibKeyGenerator.KeyFormat.AuthorYear, true);
            
        // Assert
        Assert.Equal(2, keyMap.Count);
        Assert.Equal("existing-key", entries[0].Key);
        Assert.Equal("jones2023", entries[1].Key);
    }
        
    [Fact]
    public void RegenerateKeys_OverwritesExistingKeys_WhenSpecified()
    {
        // Arrange
        var entries = new List<BibEntry>
        {
            new BibEntry("article") 
            { 
                Key = "existing-key", 
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
        var keyMap = BibEntry.RegenerateKeys(entries, BibKeyGenerator.KeyFormat.AuthorYear, false);
            
        // Assert
        Assert.Equal(2, keyMap.Count);
        Assert.Equal("smith2023", entries[0].Key);
        Assert.Equal("jones2023", entries[1].Key);
    }
        
    [Fact]
    public void RegenerateKeys_HandlesKeyCollisions()
    {
        // Arrange
        var entries = new List<BibEntry>
        {
            new BibEntry("article") { Title = "First Article", Year = 2023 },
            new BibEntry("article") { Title = "Second Article", Year = 2023 }
        };
        entries[0].AddAuthor(new Author("Smith", "John"));
        entries[1].AddAuthor(new Author("Smith", "Jane"));
            
        // Act
        var keyMap = BibEntry.RegenerateKeys(entries, BibKeyGenerator.KeyFormat.AuthorYear);
            
        // Assert
        Assert.Equal(2, keyMap.Count);
        Assert.Equal("smith2023", entries[0].Key);
        Assert.Equal("smith2023a", entries[1].Key);
    }
        
    [Fact] 
    public void RegenerateKeys_HandlesMixOfExistingAndNewKeys()
    {
        // Arrange
        var entries = new List<BibEntry>
        {
            new BibEntry("article") { Key = "smith2023", Title = "First Article", Year = 2023 },
            new BibEntry("article") { Title = "Second Article", Year = 2023 }
        };
        entries[0].AddAuthor(new Author("Smith", "John"));
        entries[1].AddAuthor(new Author("Smith", "Jane"));
            
        // Act
        var keyMap = BibEntry.RegenerateKeys(entries, BibKeyGenerator.KeyFormat.AuthorYear);
            
        // Assert
        Assert.Equal(2, keyMap.Count);
        Assert.Equal("smith2023", entries[0].Key);
        Assert.Equal("smith2023a", entries[1].Key);
    }
}