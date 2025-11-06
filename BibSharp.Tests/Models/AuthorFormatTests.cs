using BibSharp.Models;
using BibSharp.Utilities;

namespace BibSharp.Tests.Models;

public class AuthorFormatTests
{
    [Fact]
    public void FromFormattedString_SupportsLastNameFirstFormat()
    {
        var author = Author.FromFormattedString("Akinshin, Andrey A.");
        
        Assert.Equal("Akinshin", author.LastName);
        Assert.Equal("Andrey", author.FirstName);
        Assert.Equal("A.", author.MiddleName);
        Assert.Null(author.Suffix);
    }
    
    [Fact]
    public void FromFormattedString_SupportsFirstNameLastFormat()
    {
        var author = Author.FromFormattedString("Andrey A. Akinshin");
        
        Assert.Equal("Akinshin", author.LastName);
        Assert.Equal("Andrey", author.FirstName);
        Assert.Equal("A.", author.MiddleName);
        Assert.Null(author.Suffix);
    }
    
    [Fact]
    public void FromFormattedString_BothFormatsParsedSame()
    {
        var author1 = Author.FromFormattedString("Akinshin, Andrey A.");
        var author2 = Author.FromFormattedString("Andrey A. Akinshin");
        
        Assert.Equal(author1.LastName, author2.LastName);
        Assert.Equal(author1.FirstName, author2.FirstName);
        Assert.Equal(author1.MiddleName, author2.MiddleName);
        Assert.Equal(author1.Suffix, author2.Suffix);
    }
    
    [Fact]
    public void FromFormattedString_HandlesJrSuffixInLastNameFirstFormat()
    {
        var author = Author.FromFormattedString("Smith, John Robert Jr.");
        
        Assert.Equal("Smith", author.LastName);
        Assert.Equal("John", author.FirstName);
        Assert.Equal("Robert", author.MiddleName);
        Assert.Equal("Jr.", author.Suffix);
    }
    
    [Fact]
    public void FromFormattedString_HandlesJrSuffixInFirstNameLastFormat()
    {
        var author = Author.FromFormattedString("John Robert Smith Jr.");
        
        Assert.Equal("Smith", author.LastName);
        Assert.Equal("John", author.FirstName);
        Assert.Equal("Robert", author.MiddleName);
        Assert.Equal("Jr.", author.Suffix);
    }
    
    [Fact]
    public void ToBibTeXString_UsesLastNameFirstFormatByDefault()
    {
        var author = new Author("Akinshin", "Andrey", "A.");
        
        string result = author.ToBibTeXString();
        
        Assert.Equal("Akinshin, Andrey A.", result);
    }
    
    [Fact]
    public void ToBibTeXString_SupportsExplicitLastNameFirstFormat()
    {
        var author = new Author("Akinshin", "Andrey", "A.");
        
        string result = author.ToBibTeXString(AuthorFormat.LastNameFirst);
        
        Assert.Equal("Akinshin, Andrey A.", result);
    }
    
    [Fact]
    public void ToBibTeXString_SupportsFirstNameLastFormat()
    {
        var author = new Author("Akinshin", "Andrey", "A.");
        
        string result = author.ToBibTeXString(AuthorFormat.FirstNameFirst);
        
        Assert.Equal("Andrey A. Akinshin", result);
    }
    
    [Fact]
    public void BibEntry_CanSpecifyAuthorFormatWhenSerializing()
    {
        var entry = new BibEntry("article")
        {
            Key = "test2025",
            Title = "Test Title"
        };
        
        entry.AddAuthor(new Author("Akinshin", "Andrey"));
        entry.AddAuthor(new Author("Smith", "John"));
        
        // Default format (LastNameFirst)
        string defaultFormat = entry.ToBibTeX();
        Assert.Contains("author = {Akinshin, Andrey and Smith, John}", defaultFormat);
        
        // Explicit LastNameFirst format
        string lastNameFirstFormat = entry.ToBibTeX(AuthorFormat.LastNameFirst);
        Assert.Contains("author = {Akinshin, Andrey and Smith, John}", lastNameFirstFormat);
        
        // FirstNameLast format
        string firstNameLastFormat = entry.ToBibTeX(AuthorFormat.FirstNameFirst);
        Assert.Contains("author = {Andrey Akinshin and John Smith}", firstNameLastFormat);
    }
    
    [Fact]
    public void BibSerializer_UsesAuthorFormatFromSettings()
    {
        var entry = new BibEntry("article")
        {
            Key = "test2025",
            Title = "Test Title",
            Year = 2025,
            Journal = "Test Journal"
        };
        
        entry.AddAuthor(new Author("Akinshin", "Andrey"));
        
        // Default settings (LastNameFirst)
        var defaultSerializer = new BibSerializer(new BibSerializerSettings
        {
            ValidateBeforeSerialization = false
        });
        string defaultResult = defaultSerializer.Serialize(new[] { entry });
        Assert.Contains("author = {Akinshin, Andrey}", defaultResult);
        
        // With FirstNameLast setting
        var customSerializer = new BibSerializer(new BibSerializerSettings
        {
            AuthorFormat = AuthorFormat.FirstNameFirst,
            ValidateBeforeSerialization = false
        });
        string customResult = customSerializer.Serialize(new[] { entry });
        Assert.Contains("author = {Andrey Akinshin}", customResult);
    }
}