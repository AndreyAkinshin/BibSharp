using BibSharp.Models;

namespace BibSharp.Tests.Models;

public class AuthorComprehensiveTests
{
    [Fact]
    public void Constructor_LastNameOnly()
    {
        var author = new Author("Smith");
        
        Assert.Equal("Smith", author.LastName);
        Assert.Null(author.FirstName);
        Assert.Null(author.MiddleName);
        Assert.Null(author.Suffix);
    }

    [Fact]
    public void Constructor_WithFirstName()
    {
        var author = new Author("Smith", "John");
        
        Assert.Equal("Smith", author.LastName);
        Assert.Equal("John", author.FirstName);
        Assert.Null(author.MiddleName);
        Assert.Null(author.Suffix);
    }

    [Fact]
    public void Constructor_WithMiddleName()
    {
        var author = new Author("Smith", "John", "A.");
        
        Assert.Equal("Smith", author.LastName);
        Assert.Equal("John", author.FirstName);
        Assert.Equal("A.", author.MiddleName);
        Assert.Null(author.Suffix);
    }

    [Fact]
    public void Constructor_WithSuffix()
    {
        var author = new Author("Smith", "John", "A.", "Jr.");
        
        Assert.Equal("Smith", author.LastName);
        Assert.Equal("John", author.FirstName);
        Assert.Equal("A.", author.MiddleName);
        Assert.Equal("Jr.", author.Suffix);
    }

    [Fact]
    public void FromFormattedString_NullOrEmpty_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => Author.FromFormattedString(null!));
        Assert.Throws<ArgumentException>(() => Author.FromFormattedString(""));
        Assert.Throws<ArgumentException>(() => Author.FromFormattedString("   "));
    }

    [Fact]
    public void FromFormattedString_LastNameFirstFormat()
    {
        var author = Author.FromFormattedString("Smith, John");
        
        Assert.Equal("Smith", author.LastName);
        Assert.Equal("John", author.FirstName);
    }

    [Fact]
    public void FromFormattedString_LastNameFirstWithMiddle()
    {
        var author = Author.FromFormattedString("Smith, John A.");
        
        Assert.Equal("Smith", author.LastName);
        Assert.Equal("John", author.FirstName);
        Assert.Equal("A.", author.MiddleName);
    }

    [Fact]
    public void FromFormattedString_LastNameOnly()
    {
        var author = Author.FromFormattedString("Smith");
        
        Assert.Equal("Smith", author.LastName);
        Assert.Null(author.FirstName);
    }

    [Fact]
    public void FromFormattedString_FirstNameFirstFormat()
    {
        var author = Author.FromFormattedString("John Smith");
        
        Assert.Equal("Smith", author.LastName);
        Assert.Equal("John", author.FirstName);
    }

    [Fact]
    public void FromFormattedString_FirstNameFirstWithMiddle()
    {
        var author = Author.FromFormattedString("John A. Smith");
        
        Assert.Equal("Smith", author.LastName);
        Assert.Equal("John", author.FirstName);
        Assert.Equal("A.", author.MiddleName);
    }

    [Fact]
    public void FromFormattedString_WithSuffix_Jr()
    {
        var author = Author.FromFormattedString("Smith, John Jr.");
        
        Assert.Equal("Smith", author.LastName);
        Assert.Equal("John", author.FirstName);
        Assert.Equal("Jr.", author.Suffix);
    }

    [Fact]
    public void FromFormattedString_WithSuffix_Sr()
    {
        var author = Author.FromFormattedString("Smith, John Sr.");
        
        Assert.Equal("Smith", author.LastName);
        Assert.Equal("John", author.FirstName);
        Assert.Equal("Sr.", author.Suffix);
    }

    [Fact]
    public void FromFormattedString_WithSuffix_III()
    {
        var author = Author.FromFormattedString("Smith, John III");
        
        Assert.Equal("Smith", author.LastName);
        Assert.Equal("John", author.FirstName);
        Assert.Equal("III", author.Suffix);
    }

    [Fact]
    public void FromFormattedString_WithSuffix_IV()
    {
        var author = Author.FromFormattedString("Smith, John IV");
        
        Assert.Equal("Smith", author.LastName);
        Assert.Equal("John", author.FirstName);
        Assert.Equal("IV", author.Suffix);
    }

    [Fact]
    public void FromFormattedString_WithSuffix_CaseInsensitive()
    {
        var author = Author.FromFormattedString("Smith, John jr");
        
        Assert.Equal("Jr.", author.Suffix);
    }

    [Fact]
    public void FromFormattedString_FirstNameFirst_WithSuffix()
    {
        var author = Author.FromFormattedString("John Smith Jr.");
        
        Assert.Equal("Smith", author.LastName);
        Assert.Equal("John", author.FirstName);
        Assert.Equal("Jr.", author.Suffix);
    }

    [Fact]
    public void FromFormattedString_OnlyComma_LastNameOnly()
    {
        var author = Author.FromFormattedString("Smith,");
        
        Assert.Equal("Smith", author.LastName);
        Assert.Null(author.FirstName);
    }

    [Fact]
    public void ToString_LastNameOnly()
    {
        var author = new Author("Smith");
        Assert.Equal("Smith", author.ToString());
    }

    [Fact]
    public void ToString_WithFirstName()
    {
        var author = new Author("Smith", "John");
        Assert.Equal("Smith, John", author.ToString());
    }

    [Fact]
    public void ToString_WithMiddleName()
    {
        var author = new Author("Smith", "John", "A.");
        Assert.Equal("Smith, John A.", author.ToString());
    }

    [Fact]
    public void ToString_WithSuffix()
    {
        var author = new Author("Smith", "John", "A.", "Jr.");
        Assert.Equal("Smith, John A. Jr.", author.ToString());
    }

    [Fact]
    public void ToDisplayString_LastNameOnly()
    {
        var author = new Author("Smith");
        Assert.Equal("Smith", author.ToDisplayString());
    }

    [Fact]
    public void ToDisplayString_WithFirstName()
    {
        var author = new Author("Smith", "John");
        Assert.Equal("John Smith", author.ToDisplayString());
    }

    [Fact]
    public void ToDisplayString_WithMiddleName()
    {
        var author = new Author("Smith", "John", "A.");
        Assert.Equal("John A. Smith", author.ToDisplayString());
    }

    [Fact]
    public void ToDisplayString_WithSuffix()
    {
        var author = new Author("Smith", "John", null, "Jr.");
        Assert.Equal("John Smith Jr.", author.ToDisplayString());
    }

    [Fact]
    public void ToBibTeXString_LastNameFirst()
    {
        var author = new Author("Smith", "John", "A.");
        var result = author.ToBibTeXString(AuthorFormat.LastNameFirst);
        
        Assert.Equal("Smith, John A.", result);
    }

    [Fact]
    public void ToBibTeXString_FirstNameFirst()
    {
        var author = new Author("Smith", "John", "A.");
        var result = author.ToBibTeXString(AuthorFormat.FirstNameFirst);
        
        Assert.Equal("John A. Smith", result);
    }

    [Fact]
    public void ToBibTeXString_DefaultFormat_LastNameFirst()
    {
        var author = new Author("Smith", "John");
        var result = author.ToBibTeXString();
        
        Assert.Equal("Smith, John", result);
    }

    [Fact]
    public void Equals_SameAuthor_ReturnsTrue()
    {
        var author1 = new Author("Smith", "John", "A.", "Jr.");
        var author2 = new Author("Smith", "John", "A.", "Jr.");
        
        Assert.True(author1.Equals(author2));
    }

    [Fact]
    public void Equals_DifferentLastName_ReturnsFalse()
    {
        var author1 = new Author("Smith", "John");
        var author2 = new Author("Doe", "John");
        
        Assert.False(author1.Equals(author2));
    }

    [Fact]
    public void Equals_CaseInsensitive()
    {
        var author1 = new Author("smith", "john");
        var author2 = new Author("SMITH", "JOHN");
        
        Assert.True(author1.Equals(author2));
    }

    [Fact]
    public void Equals_WithNull_ReturnsFalse()
    {
        var author = new Author("Smith", "John");
        Assert.False(author.Equals(null));
    }

    [Fact]
    public void Equals_WithDifferentType_ReturnsFalse()
    {
        var author = new Author("Smith", "John");
        object obj = "Smith, John";
        
        Assert.False(author.Equals(obj));
    }

    [Fact]
    public void GetHashCode_SameAuthor_SameHash()
    {
        var author1 = new Author("Smith", "John", "A.");
        var author2 = new Author("SMITH", "JOHN", "A.");
        
        Assert.Equal(author1.GetHashCode(), author2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_DifferentAuthor_DifferentHash()
    {
        var author1 = new Author("Smith", "John");
        var author2 = new Author("Doe", "Jane");
        
        Assert.NotEqual(author1.GetHashCode(), author2.GetHashCode());
    }
}

