using BibSharp.Models;
using BibSharp.Utilities;

namespace BibSharp.Tests.Utilities;

public class BibFormatterTests
{
    [Fact]
    public void Format_WithArticle_ProducesChicagoStyle()
    {
        // Arrange
        var entry = CreateArticleEntry();
        
        // Act
        var result = BibFormatter.Format(entry, CitationStyle.Chicago);
        
        // Assert
        Assert.Contains("Smith, John, and Jane Doe.", result);
        Assert.Contains("\"Sample Article.\"", result);
        Assert.Contains("<i>Test Journal</i>", result);
        Assert.Contains("123--145", result);
    }
    
    [Fact]
    public void Format_WithBook_ProducesChicagoStyle()
    {
        // Arrange
        var entry = CreateBookEntry();
        
        // Act
        var result = BibFormatter.Format(entry, CitationStyle.Chicago);
        
        // Assert
        Assert.Contains("Brown, Alice.", result);
        Assert.Contains("<i>Sample Book</i>", result);
        Assert.Contains("Test Publisher", result);
    }
    
    [Fact]
    public void Format_WithArticle_ProducesMLAStyle()
    {
        // Arrange
        var entry = CreateArticleEntry();
        
        // Act
        var result = BibFormatter.Format(entry, CitationStyle.Mla);
        
        // Assert
        Assert.Contains("Smith, John, and Jane Doe.", result);
        Assert.Contains("\"Sample Article.\"", result);
        Assert.Contains("<i>Test Journal</i>", result);
        Assert.Contains("pp. 123--145", result);
    }
    
    [Fact]
    public void Format_WithArticle_ProducesAPAStyle()
    {
        // Arrange
        var entry = CreateArticleEntry();
        
        // Act
        var result = BibFormatter.Format(entry, CitationStyle.Apa);
        
        // Assert
        Assert.Contains("Smith, J. & Doe, J.", result);
        Assert.Contains("(2023)", result);
        Assert.Contains("<i>Test Journal", result);
    }
    
    [Fact]
    public void Format_WithArticle_ProducesHarvardStyle()
    {
        // Arrange
        var entry = CreateArticleEntry();
        
        // Act
        var result = BibFormatter.Format(entry, CitationStyle.Harvard);
        
        // Assert
        Assert.Contains("Smith, J. and Doe, J.", result);
        Assert.Contains("(2023)", result);
        Assert.Contains("<i>Test Journal</i>", result);
        Assert.Contains("pp. 123--145", result);
    }
    
    [Fact]
    public void Format_WithArticle_ProducesIEEEStyle()
    {
        // Arrange
        var entry = CreateArticleEntry();
        
        // Act
        var result = BibFormatter.Format(entry, CitationStyle.Ieee);
        
        // Assert
        Assert.Contains("J. Smith, J. Doe", result);
        Assert.Contains("\"Sample Article,\"", result);
        Assert.Contains("<i>Test Journal</i>", result);
        Assert.Contains("pp. 123--145", result);
    }
    
    [Fact]
    public void FormatBibliography_WithMultipleEntries_ProducesFormattedBibliography()
    {
        // Arrange
        var entries = new List<BibEntry>
        {
            CreateArticleEntry(),
            CreateBookEntry()
        };
        
        // Act
        var result = BibFormatter.FormatBibliography(entries, CitationStyle.Chicago);
        
        // Assert
        Assert.Contains("Smith, John, and Jane Doe.", result);
        Assert.Contains("Brown, Alice.", result);
    }
    
    [Fact]
    public void Format_WithNullEntry_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => BibFormatter.Format(null!, CitationStyle.Chicago));
    }
    
    [Fact]
    public void FormatBibliography_WithNullEntries_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => BibFormatter.FormatBibliography(null!, CitationStyle.Chicago));
    }
    
    [Fact]
    public void Format_WithInvalidCitationStyle_ThrowsArgumentException()
    {
        // Arrange
        var entry = CreateArticleEntry();
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => BibFormatter.Format(entry, (CitationStyle)99));
    }
    
    private static BibEntry CreateArticleEntry()
    {
        var entry = new BibEntry("article")
        {
            Key = "smith2023",
            Title = "Sample Article",
            Year = 2023,
            Journal = "Test Journal",
            Volume = 10,
            Number = "2",
            Pages = new PageRange("123--145"),
            Doi = "10.1000/test123",
            Url = "https://example.com/article"
        };
        
        entry.AddAuthor(new Author("Smith", "John"));
        entry.AddAuthor(new Author("Doe", "Jane"));
        
        return entry;
    }
    
    private static BibEntry CreateBookEntry()
    {
        var entry = new BibEntry("book")
        {
            Key = "brown2023",
            Title = "Sample Book",
            Year = 2023,
            Publisher = "Test Publisher",
            Address = "New York",
            Edition = "2nd",
            Isbn = "123-456-789",
            Url = "https://example.com/book"
        };
        
        entry.AddAuthor(new Author("Brown", "Alice"));
        
        return entry;
    }
}