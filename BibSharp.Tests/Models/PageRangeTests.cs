using BibSharp.Models;

namespace BibSharp.Tests.Models;

public class PageRangeTests
{
    [Fact]
    public void Constructor_WithSinglePage_SetsStartPageOnly()
    {
        var range = new PageRange(42);
        
        Assert.Equal(42, range.StartPage);
        Assert.Null(range.EndPage);
    }
    
    [Fact]
    public void Constructor_WithPageRange_SetsBothPages()
    {
        var range = new PageRange(42, 50);
        
        Assert.Equal(42, range.StartPage);
        Assert.Equal(50, range.EndPage);
    }
    
    [Fact]
    public void Constructor_WithStringSinglePage_ParsesCorrectly()
    {
        var range = new PageRange("42");
        
        Assert.Equal(42, range.StartPage);
        Assert.Null(range.EndPage);
    }
    
    [Fact]
    public void Constructor_WithStringRange_ParsesCorrectly()
    {
        var range = new PageRange("42-50");
        
        Assert.Equal(42, range.StartPage);
        Assert.Equal(50, range.EndPage);
    }
    
    [Fact]
    public void Constructor_WithBibTeXStyleRange_ParsesCorrectly()
    {
        var range = new PageRange("42--50");
        
        Assert.Equal(42, range.StartPage);
        Assert.Equal(50, range.EndPage);
    }
    
    [Fact]
    public void Constructor_WithInvalidString_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => new PageRange("not-a-number"));
    }
    
    [Fact]
    public void Constructor_WithEmptyString_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => new PageRange(""));
    }
    
    [Fact]
    public void ToString_WithSinglePage_FormatsCorrectly()
    {
        var range = new PageRange(42);
        
        Assert.Equal("42", range.ToString());
    }
    
    [Fact]
    public void ToString_WithPageRange_FormatsWithDoubleDash()
    {
        var range = new PageRange(42, 50);
        
        Assert.Equal("42--50", range.ToString());
    }
    
    [Fact]
    public void GetPageCount_WithSinglePage_ReturnsOne()
    {
        var range = new PageRange(42);
        
        Assert.Equal(1, range.GetPageCount());
    }
    
    [Fact]
    public void GetPageCount_WithPageRange_ReturnsCorrectCount()
    {
        var range = new PageRange(42, 50);
        
        Assert.Equal(9, range.GetPageCount());
    }
    
    [Fact]
    public void Equals_WithIdenticalRanges_ReturnsTrue()
    {
        var range1 = new PageRange(42, 50);
        var range2 = new PageRange(42, 50);
        
        Assert.Equal(range1, range2);
        Assert.True(range1.Equals(range2));
    }
    
    [Fact]
    public void Equals_WithDifferentRanges_ReturnsFalse()
    {
        var range1 = new PageRange(42, 50);
        var range2 = new PageRange(42, 51);
        
        Assert.NotEqual(range1, range2);
        Assert.False(range1.Equals(range2));
    }
    
    [Fact]
    public void Equals_WithNonPageRangeObject_ReturnsFalse()
    {
        var range = new PageRange(42, 50);
        
        Assert.False(range.Equals("42-50"));
    }
    
    [Fact]
    public void GetHashCode_WithEqualRanges_ReturnsSameHash()
    {
        var range1 = new PageRange(42, 50);
        var range2 = new PageRange(42, 50);
        
        Assert.Equal(range1.GetHashCode(), range2.GetHashCode());
    }
    
    [Fact]
    public void Constructor_WithSpacedRange_ParsesCorrectly()
    {
        var range = new PageRange("42 - 50");
        
        Assert.Equal(42, range.StartPage);
        Assert.Equal(50, range.EndPage);
    }

    [Fact]
    public void Constructor_WithNegativeSinglePage_ThrowsArgumentException()
    {
        var exception = Assert.Throws<ArgumentException>(() => new PageRange(-1));
        Assert.Contains("cannot be negative", exception.Message);
        Assert.Equal("startPage", exception.ParamName);
    }
}