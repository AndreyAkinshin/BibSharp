using BibSharp.Models;

namespace BibSharp.Tests.Models;

public class AuthorTests
{
    [Fact]
    public void Constructor_WithLastNameOnly_SetsCorrectProperties()
    {
        var author = new Author("Smith");
        
        Assert.Equal("Smith", author.LastName);
        Assert.Null(author.FirstName);
        Assert.Null(author.MiddleName);
        Assert.Null(author.Suffix);
    }
    
    [Fact]
    public void Constructor_WithFullName_SetsCorrectProperties()
    {
        var author = new Author("Smith", "John", "Robert", "Jr.");
        
        Assert.Equal("Smith", author.LastName);
        Assert.Equal("John", author.FirstName);
        Assert.Equal("Robert", author.MiddleName);
        Assert.Equal("Jr.", author.Suffix);
    }
    
    [Fact]
    public void FromFormattedString_WithLastnameFirstname_ParsesCorrectly()
    {
        var author = Author.FromFormattedString("Smith, John");
        
        Assert.Equal("Smith", author.LastName);
        Assert.Equal("John", author.FirstName);
        Assert.Null(author.MiddleName);
    }
    
    [Fact]
    public void FromFormattedString_WithLastnameOnly_ParsesCorrectly()
    {
        var author = Author.FromFormattedString("Smith");
        
        Assert.Equal("Smith", author.LastName);
        Assert.Null(author.FirstName);
        Assert.Null(author.MiddleName);
    }
    
    [Fact]
    public void FromFormattedString_WithMiddleName_ParsesCorrectly()
    {
        var author = Author.FromFormattedString("Smith, John Robert");
        
        Assert.Equal("Smith", author.LastName);
        Assert.Equal("John", author.FirstName);
        Assert.Equal("Robert", author.MiddleName);
    }
    
    [Fact]
    public void FromFormattedString_WithSuffix_ParsesCorrectly()
    {
        var author = Author.FromFormattedString("Smith, John Robert Jr.");
        
        Assert.Equal("Smith", author.LastName);
        Assert.Equal("John", author.FirstName);
        Assert.Equal("Robert", author.MiddleName);
        Assert.Equal("Jr.", author.Suffix);
    }
    
    [Fact]
    public void ToString_WithFullName_FormatsCorrectly()
    {
        var author = new Author("Smith", "John", "Robert", "Jr.");
        
        Assert.Equal("Smith, John Robert Jr.", author.ToString());
    }
    
    [Fact]
    public void ToString_WithLastNameOnly_FormatsCorrectly()
    {
        var author = new Author("Smith");
        
        Assert.Equal("Smith", author.ToString());
    }
    
    [Fact]
    public void ToDisplayString_WithFullName_FormatsCorrectly()
    {
        var author = new Author("Smith", "John", "Robert", "Jr.");
        
        Assert.Equal("John Robert Smith Jr.", author.ToDisplayString());
    }
    
    [Fact]
    public void ToDisplayString_WithLastNameOnly_FormatsCorrectly()
    {
        var author = new Author("Smith");
        
        Assert.Equal("Smith", author.ToDisplayString());
    }
    
    [Fact]
    public void Equals_WithIdenticalAuthors_ReturnsTrue()
    {
        var author1 = new Author("Smith", "John");
        var author2 = new Author("Smith", "John");
        
        Assert.Equal(author1, author2);
        Assert.True(author1.Equals(author2));
    }
    
    [Fact]
    public void Equals_WithDifferentAuthors_ReturnsFalse()
    {
        var author1 = new Author("Smith", "John");
        var author2 = new Author("Smith", "Jane");
        
        Assert.NotEqual(author1, author2);
        Assert.False(author1.Equals(author2));
    }
    
    [Fact]
    public void Equals_WithNonAuthorObject_ReturnsFalse()
    {
        var author = new Author("Smith", "John");
        
        Assert.False(author.Equals("Smith, John"));
    }
    
    [Fact]
    public void GetHashCode_WithEqualAuthors_ReturnsSameHash()
    {
        var author1 = new Author("Smith", "John");
        var author2 = new Author("Smith", "John");
        
        Assert.Equal(author1.GetHashCode(), author2.GetHashCode());
    }
    
    [Fact]
    public void FromFormattedString_IgnoresCase_ForSuffix()
    {
        var authorJr = Author.FromFormattedString("Smith, John jr.");
        var authorSr = Author.FromFormattedString("Smith, John sr.");
        
        Assert.Equal("Jr.", authorJr.Suffix);
        Assert.Equal("Sr.", authorSr.Suffix);
    }
}