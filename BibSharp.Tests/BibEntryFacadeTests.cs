using BibSharp.Exceptions;
using BibSharp.Models;
using BibSharp.Utilities;

namespace BibSharp.Tests;

public class BibEntryFacadeTests
{
    [Fact]
    public void Parse_ValidBibTeXString_ReturnsBibEntry()
    {
        // Arrange
        string bibTexString = @"@article{smith2023,
  author = {Smith, John and Doe, Jane},
  title = {Test Article},
  journal = {Test Journal},
  year = {2023},
  volume = {10},
  pages = {1--10}
}";
            
        // Act
        var entry = BibEntry.Parse(bibTexString);
            
        // Assert
        Assert.NotNull(entry);
        Assert.Equal("article", entry.EntryType);
        Assert.Equal("smith2023", entry.Key);
        Assert.Equal("Test Article", entry.Title);
        Assert.Equal("Test Journal", entry.Journal);
        Assert.Equal(2023, entry.Year);
        Assert.Equal(10, entry.Volume);
        Assert.Equal("1--10", entry.Pages?.ToString());
        Assert.Equal(2, entry.Authors.Count);
        Assert.Equal("Smith", entry.Authors[0].LastName);
        Assert.Equal("John", entry.Authors[0].FirstName);
        Assert.Equal("Doe", entry.Authors[1].LastName);
        Assert.Equal("Jane", entry.Authors[1].FirstName);
    }
        
    [Fact]
    public void Parse_EmptyString_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => BibEntry.Parse(""));
    }
        
    [Fact]
    public void Parse_InvalidBibTeXString_ThrowsBibParseException()
    {
        // Arrange
        string invalidBibTexString = "This is not a valid BibTeX string";
            
        // Act & Assert
        Assert.Throws<BibParseException>(() => BibEntry.Parse(invalidBibTexString));
    }
        
    [Fact]
    public void Parse_MultipleBibTeXEntries_ThrowsBibParseException()
    {
        // Arrange
        string multipleBibTexString = @"@article{smith2023,
  title = {First Article},
  author = {Smith, John},
  journal = {Test Journal},
  year = {2023}
}

@book{jones2022,
  title = {Test Book},
  author = {Jones, Mary},
  publisher = {Test Publisher},
  year = {2022}
}";
            
        // Act & Assert
        Assert.Throws<BibParseException>(() => BibEntry.Parse(multipleBibTexString));
    }
        
    [Fact]
    public void TryParse_ValidBibTeXString_ReturnsTrue()
    {
        // Arrange
        string bibTexString = @"@article{smith2023,
  author = {Smith, John},
  title = {Test Article},
  journal = {Test Journal},
  year = {2023}
}";
            
        // Act
        bool result = BibEntry.TryParse(bibTexString, out var entry);
            
        // Assert
        Assert.True(result);
        Assert.NotNull(entry);
        Assert.Equal("smith2023", entry!.Key);
        Assert.Equal("Test Article", entry.Title);
    }
        
    [Fact]
    public void TryParse_InvalidBibTeXString_ReturnsFalse()
    {
        // Arrange
        string invalidBibTexString = "This is not a valid BibTeX string";
            
        // Act
        bool result = BibEntry.TryParse(invalidBibTexString, out var entry);
            
        // Assert
        Assert.False(result);
        Assert.Null(entry);
    }
        
    [Fact]
    public void TryParse_EmptyString_ReturnsFalse()
    {
        // Act
        bool result = BibEntry.TryParse("", out var entry);
            
        // Assert
        Assert.False(result);
        Assert.Null(entry);
    }
        
    [Fact]
    public void ParseAll_ValidBibTeXString_ReturnsAllEntries()
    {
        // Arrange
        string bibTexString = @"@article{smith2023,
  author = {Smith, John},
  title = {First Article},
  journal = {Test Journal},
  year = {2023}
}

@book{jones2022,
  author = {Jones, Mary},
  title = {Test Book},
  publisher = {Test Publisher},
  year = {2022}
}";
            
        // Act
        var entries = BibEntry.ParseAll(bibTexString);
            
        // Assert
        Assert.NotNull(entries);
        Assert.Equal(2, entries.Count);
            
        var article = entries.FirstOrDefault(e => e.EntryType == "article");
        var book = entries.FirstOrDefault(e => e.EntryType == "book");
            
        Assert.NotNull(article);
        Assert.NotNull(book);
            
        Assert.Equal("smith2023", article!.Key);
        Assert.Equal("First Article", article.Title);
            
        Assert.Equal("jones2022", book!.Key);
        Assert.Equal("Test Book", book.Title);
    }
        
    [Fact]
    public void ParseAll_EmptyString_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => BibEntry.ParseAll(""));
    }
        
    [Fact]
    public void ToFormattedString_CallsBibFormatterFormat()
    {
        // Arrange
        var entry = new BibEntry("article")
        {
            Key = "test2023",
            Title = "Test Article",
            Year = 2023
        };
        entry.AddAuthor(new Author("Smith", "John"));
            
        // Act
        string formattedChicago = entry.ToFormattedString(CitationStyle.Chicago);
        string formattedMla = entry.ToFormattedString(CitationStyle.Mla);
            
        // Assert
        string directChicago = BibFormatter.Format(entry, CitationStyle.Chicago);
        string directMla = BibFormatter.Format(entry, CitationStyle.Mla);
            
        Assert.Equal(directChicago, formattedChicago);
        Assert.Equal(directMla, formattedMla);
    }
}