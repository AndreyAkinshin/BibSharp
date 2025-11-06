using BibSharp.Models;

namespace BibSharp.Tests;

public class BibEntryTests
{
    [Fact]
    public void Constructor_WithNoParameters_CreatesMiscEntry()
    {
        var entry = new BibEntry();
        
        Assert.Equal("misc", entry.EntryType);
        Assert.Empty(entry.Key);
    }
    
    [Fact]
    public void Constructor_WithEntryType_CreatesCorrectEntry()
    {
        var entry = new BibEntry("article");
        
        Assert.Equal("article", entry.EntryType);
        Assert.Empty(entry.Key);
    }
    
    [Fact]
    public void Key_CanBeSet()
    {
        var entry = new BibEntry { Key = "test123" };
        
        Assert.Equal("test123", entry.Key);
    }
    
    [Fact]
    public void Fields_CanBeAccessedViaProperties()
    {
        var entry = new BibEntry
        {
            Title = "Test Title",
            Year = 2025,
            Journal = "Test Journal",
            Volume = 42,
            Pages = new PageRange(10, 20)
        };
        
        Assert.Equal("Test Title", entry.Title);
        Assert.Equal(2025, entry.Year);
        Assert.Equal("Test Journal", entry.Journal);
        Assert.Equal(42, entry.Volume);
        Assert.Equal(10, entry.Pages?.StartPage);
        Assert.Equal(20, entry.Pages?.EndPage);
    }
    
    [Fact]
    public void Fields_CanBeAccessedViaIndexer()
    {
        var entry = new BibEntry();
        entry["title"] = "Test Title";
        entry["year"] = "2025";
        entry["custom-field"] = "Custom Value";
        
        Assert.Equal("Test Title", entry["title"]);
        Assert.Equal("2025", entry["year"]);
        Assert.Equal("Custom Value", entry["custom-field"]);
    }
    
    [Fact]
    public void Fields_AreCaseInsensitive()
    {
        var entry = new BibEntry();
        entry["TITLE"] = "Test Title";
        
        Assert.Equal("Test Title", entry["title"]);
        Assert.Equal("Test Title", entry["Title"]);
    }
    
    [Fact]
    public void Authors_CanBeAddedAndRemoved()
    {
        var entry = new BibEntry();
        var author1 = new Author("Smith", "John");
        var author2 = new Author("Doe", "Jane");
        
        entry.AddAuthor(author1);
        entry.AddAuthor(author2);
        
        Assert.Equal(2, entry.Authors.Count);
        Assert.Contains(author1, entry.Authors);
        Assert.Contains(author2, entry.Authors);
        
        entry.RemoveAuthor(author1);
        
        Assert.Single(entry.Authors);
        Assert.Contains(author2, entry.Authors);
    }
    
    [Fact]
    public void Authors_UpdatesAuthorField()
    {
        var entry = new BibEntry();
        entry.AddAuthor(new Author("Smith", "John"));
        entry.AddAuthor(new Author("Doe", "Jane"));
        
        Assert.Equal("Smith, John and Doe, Jane", entry["author"]);
    }
    
    [Fact]
    public void RemoveField_RemovesField()
    {
        var entry = new BibEntry();
        entry["title"] = "Test Title";
        entry["year"] = "2025";
        
        Assert.True(entry.HasField("title"));
        Assert.True(entry.RemoveField("title"));
        Assert.False(entry.HasField("title"));
    }
    
    [Fact]
    public void ToBibTeX_FormatsCorrectly()
    {
        var entry = new BibEntry("article")
        {
            Key = "test2025",
            Title = "Test Title",
            Year = 2025,
            Journal = "Test Journal"
        };
        entry.AddAuthor(new Author("Smith", "John"));
        
        string bibtex = entry.ToBibTeX();
        
        Assert.Contains("@article{test2025,", bibtex);
        Assert.Contains("title = {Test Title}", bibtex);
        Assert.Contains("year = {2025}", bibtex);
        Assert.Contains("journal = {Test Journal}", bibtex);
        Assert.Contains("author = {Smith, John}", bibtex);
    }
    
    [Fact]
    public void Clone_CreatesCopy()
    {
        var original = new BibEntry("article")
        {
            Key = "test2025",
            Title = "Test Title",
            Year = 2025,
            Journal = "Test Journal"
        };
        original.AddAuthor(new Author("Smith", "John"));
        
        var clone = original.Clone();
        
        Assert.Equal(original.Key, clone.Key);
        Assert.Equal(original.EntryType, clone.EntryType);
        Assert.Equal(original.Title, clone.Title);
        Assert.Equal(original.Year, clone.Year);
        Assert.Equal(original.Journal, clone.Journal);
        Assert.Single(clone.Authors);
        Assert.Equal("Smith", clone.Authors[0].LastName);
        Assert.Equal("John", clone.Authors[0].FirstName);
    }
    
    [Fact]
    public void Validate_DetectsErrors()
    {
        var entry = new BibEntry("article");  // Article requires author, title, journal, year
        
        var result = entry.Validate();
        
        Assert.True(result.HasErrors);
        Assert.Equal(5, result.Errors.Count);  // Missing key + 4 required fields
    }
    
    [Fact]
    public void Validate_PassesWithValidEntry()
    {
        var entry = new BibEntry("article")
        {
            Key = "test2025",
            Title = "Test Title",
            Year = 2025,
            Journal = "Test Journal"
        };
        entry.AddAuthor(new Author("Smith", "John"));
        
        var result = entry.Validate();
        
        Assert.False(result.HasErrors);
    }
    
    [Fact]
    public void RegisterCustomType_AddsType()
    {
        BibEntry.RegisterCustomType("software", 
            new[] { "title", "version" }, 
            new[] { "author", "year", "url" });
        
        var entry = new BibEntry("software")
        {
            Key = "app2025",
            Title = "Test App",
            ["version"] = "1.0.0"
        };
        
        var result = entry.Validate();
        
        Assert.False(result.HasErrors);
    }
}