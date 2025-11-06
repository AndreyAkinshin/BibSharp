using BibSharp.Models;
using BibSharp.Utilities;

namespace BibSharp.Tests.Utilities;

public class BibComparerTests
{
    [Fact]
    public void ByAuthor_SortsCorrectly()
    {
        var entries = new[]
        {
            CreateEntry("key1", authors: new[] { new Author("Smith", "John") }),
            CreateEntry("key2", authors: new[] { new Author("Brown", "Alice") }),
            CreateEntry("key3", authors: new[] { new Author("Doe", "Jane") })
        };
        
        var sorted = entries.OrderBy(e => e, BibComparer.ByAuthor()).ToArray();
        
        Assert.Equal("key2", sorted[0].Key); // Brown
        Assert.Equal("key3", sorted[1].Key); // Doe
        Assert.Equal("key1", sorted[2].Key); // Smith
    }
    
    [Fact]
    public void ByYear_Ascending_SortsCorrectly()
    {
        var entries = new[]
        {
            CreateEntry("key1", year: 2025),
            CreateEntry("key2", year: 2023),
            CreateEntry("key3", year: 2024)
        };
        
        var sorted = entries.OrderBy(e => e, BibComparer.ByYear(false)).ToArray();
        
        Assert.Equal("key2", sorted[0].Key); // 2023
        Assert.Equal("key3", sorted[1].Key); // 2024
        Assert.Equal("key1", sorted[2].Key); // 2025
    }
    
    [Fact]
    public void ByYear_Descending_SortsCorrectly()
    {
        var entries = new[]
        {
            CreateEntry("key1", year: 2025),
            CreateEntry("key2", year: 2023),
            CreateEntry("key3", year: 2024)
        };
        
        var sorted = entries.OrderBy(e => e, BibComparer.ByYear()).ToArray();
        
        Assert.Equal("key1", sorted[0].Key); // 2025
        Assert.Equal("key3", sorted[1].Key); // 2024
        Assert.Equal("key2", sorted[2].Key); // 2023
    }
    
    [Fact]
    public void ByTitle_SortsCorrectly()
    {
        var entries = new[]
        {
            CreateEntry("key1", title: "Zebra Studies"),
            CreateEntry("key2", title: "Apple Research"),
            CreateEntry("key3", title: "Machine Learning")
        };
        
        var sorted = entries.OrderBy(e => e, BibComparer.ByTitle()).ToArray();
        
        Assert.Equal("key2", sorted[0].Key); // Apple
        Assert.Equal("key3", sorted[1].Key); // Machine
        Assert.Equal("key1", sorted[2].Key); // Zebra
    }
    
    [Fact]
    public void ByKey_SortsCorrectly()
    {
        var entries = new[]
        {
            CreateEntry("keyC"),
            CreateEntry("keyA"),
            CreateEntry("keyB")
        };
        
        var sorted = entries.OrderBy(e => e, BibComparer.ByKey()).ToArray();
        
        Assert.Equal("keyA", sorted[0].Key);
        Assert.Equal("keyB", sorted[1].Key);
        Assert.Equal("keyC", sorted[2].Key);
    }
    
    [Fact]
    public void ByField_SortsCorrectly()
    {
        var entry1 = CreateEntry("key1");
        entry1["publisher"] = "Zebra Books";
        
        var entry2 = CreateEntry("key2");
        entry2["publisher"] = "Apple Books";
        
        var entry3 = CreateEntry("key3");
        entry3["publisher"] = "Middle Books";
        
        var entries = new[] { entry1, entry2, entry3 };
        var sorted = entries.OrderBy(e => e, BibComparer.ByField("publisher")).ToArray();
        
        Assert.Equal("key2", sorted[0].Key); // Apple
        Assert.Equal("key3", sorted[1].Key); // Middle
        Assert.Equal("key1", sorted[2].Key); // Zebra
    }
    
    [Fact]
    public void Chain_SortsCorrectly()
    {
        var entry1 = CreateEntry("key1", year: 2024, title: "A Study");
        var entry2 = CreateEntry("key2", year: 2025, title: "B Study");
        var entry3 = CreateEntry("key3", year: 2025, title: "A Study");
        
        var entries = new[] { entry1, entry2, entry3 };
        var sorted = entries.OrderBy(e => e, BibComparer.Chain(
            BibComparer.ByYear(),
            BibComparer.ByTitle()
        )).ToArray();
        
        // First by year (descending), then by title
        Assert.Equal("key3", sorted[0].Key); // 2025, A Study
        Assert.Equal("key2", sorted[1].Key); // 2025, B Study
        Assert.Equal("key1", sorted[2].Key); // 2024, A Study
    }
    
    [Fact]
    public void ByAuthor_HandlesEmptyAuthors()
    {
        var entry1 = CreateEntry("key1", authors: new[] { new Author("Smith", "John") });
        var entry2 = CreateEntry("key2"); // No authors
        
        var entries = new[] { entry1, entry2 };
        var sorted = entries.OrderBy(e => e, BibComparer.ByAuthor()).ToArray();
        
        Assert.Equal("key2", sorted[0].Key); // No authors (comes first)
        Assert.Equal("key1", sorted[1].Key); // Smith
    }
    
    private static BibEntry CreateEntry(string key, string? title = null, int? year = null, Author[]? authors = null)
    {
        var entry = new BibEntry("article") { Key = key };
        
        if (title != null)
        {
            entry.Title = title;
        }
        
        if (year.HasValue)
        {
            entry.Year = year.Value;
        }
        
        if (authors != null)
        {
            foreach (var author in authors)
            {
                entry.AddAuthor(author);
            }
        }
        
        return entry;
    }
}