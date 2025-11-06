using BibSharp.Models;

namespace BibSharp.Tests.Models;

public class AuthorFullCoverageTests
{
    [Fact]
    public void FromFormattedString_LastNameFirst_OnlyLastName()
    {
        var author = Author.FromFormattedString("Smith,");
        
        Assert.Equal("Smith", author.LastName);
        Assert.Null(author.FirstName);
    }

    [Fact]
    public void FromFormattedString_LastNameFirst_WithWhitespaceAfterComma()
    {
        var author = Author.FromFormattedString("Smith,   ");
        
        Assert.Equal("Smith", author.LastName);
        Assert.Null(author.FirstName);
    }

    [Fact]
    public void FromFormattedString_FirstNameFirst_OnePart()
    {
        var author = Author.FromFormattedString("Smith");
        
        Assert.Equal("Smith", author.LastName);
        Assert.Null(author.FirstName);
    }

    [Fact]
    public void FromFormattedString_FirstNameFirst_TwoParts()
    {
        var author = Author.FromFormattedString("John Smith");
        
        Assert.Equal("Smith", author.LastName);
        Assert.Equal("John", author.FirstName);
        Assert.Null(author.MiddleName);
    }

    [Fact]
    public void FromFormattedString_FirstNameFirst_ThreeParts()
    {
        var author = Author.FromFormattedString("John Andrew Smith");
        
        Assert.Equal("Smith", author.LastName);
        Assert.Equal("John", author.FirstName);
        Assert.Equal("Andrew", author.MiddleName);
    }

    [Fact]
    public void FromFormattedString_FirstNameFirst_WithSuffixJr()
    {
        var author = Author.FromFormattedString("John Smith Jr.");
        
        Assert.Equal("Smith", author.LastName);
        Assert.Equal("John", author.FirstName);
        Assert.Equal("Jr.", author.Suffix);
    }

    [Fact]
    public void FromFormattedString_FirstNameFirst_WithSuffixSr()
    {
        var author = Author.FromFormattedString("John Smith Sr.");
        
        Assert.Equal("Smith", author.LastName);
        Assert.Equal("John", author.FirstName);
        Assert.Equal("Sr.", author.Suffix);
    }

    [Fact]
    public void FromFormattedString_FirstNameFirst_WithSuffixIII()
    {
        var author = Author.FromFormattedString("John Andrew Smith III");
        
        Assert.Equal("Smith", author.LastName);
        Assert.Equal("John", author.FirstName);
        Assert.Equal("Andrew", author.MiddleName);
        Assert.Equal("III", author.Suffix);
    }

    [Fact]
    public void FromFormattedString_FirstNameFirst_WithSuffixIV()
    {
        var author = Author.FromFormattedString("John Smith IV");
        
        Assert.Equal("Smith", author.LastName);
        Assert.Equal("John", author.FirstName);
        Assert.Equal("IV", author.Suffix);
    }

    [Fact]
    public void FromFormattedString_FirstNameFirst_TwoPartsWithSuffix()
    {
        var author = Author.FromFormattedString("Smith Jr.");
        
        // With two parts where second is a suffix, the suffix is properly extracted
        // and first part becomes the last name with no first name
        Assert.Equal("Smith", author.LastName);
        Assert.Null(author.FirstName);
        // With enhanced suffix handling, the suffix is now properly recognized and assigned
        Assert.Equal("Jr.", author.Suffix);
    }

    [Fact]
    public void FromFormattedString_LastNameFirst_MultipleParts_NoSuffix()
    {
        var author = Author.FromFormattedString("Smith, John Andrew Benjamin");
        
        Assert.Equal("Smith", author.LastName);
        Assert.Equal("John", author.FirstName);
        Assert.Equal("Andrew Benjamin", author.MiddleName);
        Assert.Null(author.Suffix);
    }

    [Fact]
    public void FromFormattedString_LastNameFirst_WithMiddleAndSuffix()
    {
        var author = Author.FromFormattedString("Smith, John Andrew Jr.");
        
        Assert.Equal("Smith", author.LastName);
        Assert.Equal("John", author.FirstName);
        Assert.Equal("Andrew", author.MiddleName);
        Assert.Equal("Jr.", author.Suffix);
    }

    [Fact]
    public void FromFormattedString_FirstNameFirst_WithMiddleAndSuffix()
    {
        var author = Author.FromFormattedString("John Andrew Smith Jr.");
        
        Assert.Equal("Smith", author.LastName);
        Assert.Equal("John", author.FirstName);
        Assert.Equal("Andrew", author.MiddleName);
        Assert.Equal("Jr.", author.Suffix);
    }

    [Fact]
    public void FromFormattedString_SuffixNormalization_Jr()
    {
        var authorLower = Author.FromFormattedString("Smith, John jr");
        var authorUpper = Author.FromFormattedString("Smith, John JR");
        var authorDot = Author.FromFormattedString("Smith, John Jr.");
        
        Assert.Equal("Jr.", authorLower.Suffix);
        Assert.Equal("Jr.", authorUpper.Suffix);
        Assert.Equal("Jr.", authorDot.Suffix);
    }

    [Fact]
    public void FromFormattedString_SuffixNormalization_Sr()
    {
        var authorLower = Author.FromFormattedString("Smith, John sr");
        var authorUpper = Author.FromFormattedString("Smith, John SR");
        
        Assert.Equal("Sr.", authorLower.Suffix);
        Assert.Equal("Sr.", authorUpper.Suffix);
    }

    [Fact]
    public void FromFormattedString_SuffixNormalization_III()
    {
        var authorLower = Author.FromFormattedString("Smith, John iii");
        var authorUpper = Author.FromFormattedString("Smith, John III");
        
        Assert.Equal("III", authorLower.Suffix);
        Assert.Equal("III", authorUpper.Suffix);
    }

    [Fact]
    public void FromFormattedString_SuffixNormalization_IV()
    {
        var authorLower = Author.FromFormattedString("Smith, John iv");
        
        Assert.Equal("IV", authorLower.Suffix);
    }

    [Fact]
    public void ToString_WithoutFirstName_OnlyLastName()
    {
        var author = new Author("Smith");
        
        Assert.Equal("Smith", author.ToString());
    }

    [Fact]
    public void ToString_WithFirstNameNoMiddle()
    {
        var author = new Author("Smith", "John");
        
        Assert.Equal("Smith, John", author.ToString());
    }

    [Fact]
    public void ToString_WithMiddleNoSuffix()
    {
        var author = new Author("Smith", "John", "A.");
        
        Assert.Equal("Smith, John A.", author.ToString());
    }

    [Fact]
    public void ToString_AllComponents()
    {
        var author = new Author("Smith", "John", "A.", "Jr.");
        
        Assert.Equal("Smith, John A. Jr.", author.ToString());
    }

    [Fact]
    public void ToDisplayString_OnlyLastName()
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
        var author = new Author("Smith", "John", "Andrew");
        
        Assert.Equal("John Andrew Smith", author.ToDisplayString());
    }

    [Fact]
    public void ToDisplayString_WithSuffixNoMiddle()
    {
        var author = new Author("Smith", "John", null, "Jr.");
        
        Assert.Equal("John Smith Jr.", author.ToDisplayString());
    }

    [Fact]
    public void ToDisplayString_AllComponents()
    {
        var author = new Author("Smith", "John", "A.", "Jr.");
        
        Assert.Equal("John A. Smith Jr.", author.ToDisplayString());
    }

    [Fact]
    public void Equals_DifferentFirstName_ReturnsFalse()
    {
        var author1 = new Author("Smith", "John");
        var author2 = new Author("Smith", "Jane");
        
        Assert.False(author1.Equals(author2));
    }

    [Fact]
    public void Equals_DifferentMiddleName_ReturnsFalse()
    {
        var author1 = new Author("Smith", "John", "A.");
        var author2 = new Author("Smith", "John", "B.");
        
        Assert.False(author1.Equals(author2));
    }

    [Fact]
    public void Equals_DifferentSuffix_ReturnsFalse()
    {
        var author1 = new Author("Smith", "John", null, "Jr.");
        var author2 = new Author("Smith", "John", null, "Sr.");
        
        Assert.False(author1.Equals(author2));
    }

    [Fact]
    public void GetHashCode_WithNullComponents()
    {
        var author = new Author("Smith");
        var hash = author.GetHashCode();
        
        Assert.NotEqual(0, hash);
    }
}

