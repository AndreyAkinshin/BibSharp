using BibSharp.Models;

namespace BibSharp.Tests.Models;

public class PageRangeComprehensiveTests
{
    [Fact]
    public void Constructor_SinglePage()
    {
        var range = new PageRange(100);
        
        Assert.Equal(100, range.StartPage);
        Assert.Null(range.EndPage);
    }

    [Fact]
    public void Constructor_PageRange()
    {
        var range = new PageRange(100, 110);
        
        Assert.Equal(100, range.StartPage);
        Assert.Equal(110, range.EndPage);
    }

    [Fact]
    public void Constructor_SamePage_Valid()
    {
        var range = new PageRange(100, 100);
        
        Assert.Equal(100, range.StartPage);
        Assert.Equal(100, range.EndPage);
    }

    [Fact]
    public void Constructor_EndBeforeStart_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => new PageRange(100, 99));
    }

    [Fact]
    public void Constructor_FromString_SinglePage()
    {
        var range = new PageRange("100");
        
        Assert.Equal(100, range.StartPage);
        Assert.Null(range.EndPage);
    }

    [Fact]
    public void Constructor_FromString_Range()
    {
        var range = new PageRange("100-110");
        
        Assert.Equal(100, range.StartPage);
        Assert.Equal(110, range.EndPage);
    }

    [Fact]
    public void Constructor_FromString_DoubleDash()
    {
        var range = new PageRange("100--110");
        
        Assert.Equal(100, range.StartPage);
        Assert.Equal(110, range.EndPage);
    }

    [Fact]
    public void Constructor_FromString_TripleDash()
    {
        var range = new PageRange("100---110");
        
        Assert.Equal(100, range.StartPage);
        Assert.Equal(110, range.EndPage);
    }

    [Fact]
    public void Constructor_FromString_WithWhitespace()
    {
        var range = new PageRange("  100 -- 110  ");
        
        Assert.Equal(100, range.StartPage);
        Assert.Equal(110, range.EndPage);
    }

    [Fact]
    public void Constructor_FromString_NullOrEmpty_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => new PageRange(null!));
        Assert.Throws<ArgumentException>(() => new PageRange(""));
        Assert.Throws<ArgumentException>(() => new PageRange("   "));
    }

    [Fact]
    public void Constructor_FromString_InvalidStartPage_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => new PageRange("abc"));
        Assert.Throws<ArgumentException>(() => new PageRange("abc-110"));
    }

    [Fact]
    public void Constructor_FromString_InvalidEndPage_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => new PageRange("100-xyz"));
    }

    [Fact]
    public void Constructor_FromString_EndBeforeStart_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => new PageRange("110-100"));
    }

    [Fact]
    public void ToString_SinglePage()
    {
        var range = new PageRange(100);
        Assert.Equal("100", range.ToString());
    }

    [Fact]
    public void ToString_Range_UsesDoubleDash()
    {
        var range = new PageRange(100, 110);
        Assert.Equal("100--110", range.ToString());
    }

    [Fact]
    public void GetPageCount_SinglePage_ReturnsOne()
    {
        var range = new PageRange(100);
        Assert.Equal(1, range.GetPageCount());
    }

    [Fact]
    public void GetPageCount_Range_ReturnsCount()
    {
        var range = new PageRange(100, 110);
        Assert.Equal(11, range.GetPageCount()); // 100-110 inclusive = 11 pages
    }

    [Fact]
    public void GetPageCount_SamePage_ReturnsOne()
    {
        var range = new PageRange(100, 100);
        Assert.Equal(1, range.GetPageCount());
    }

    [Fact]
    public void Equals_SameRange_ReturnsTrue()
    {
        var range1 = new PageRange(100, 110);
        var range2 = new PageRange(100, 110);
        
        Assert.True(range1.Equals(range2));
    }

    [Fact]
    public void Equals_DifferentRange_ReturnsFalse()
    {
        var range1 = new PageRange(100, 110);
        var range2 = new PageRange(100, 120);
        
        Assert.False(range1.Equals(range2));
    }

    [Fact]
    public void Equals_SinglePageVsRange_ReturnsFalse()
    {
        var range1 = new PageRange(100);
        var range2 = new PageRange(100, 110);
        
        Assert.False(range1.Equals(range2));
    }

    [Fact]
    public void Equals_WithNull_ReturnsFalse()
    {
        var range = new PageRange(100);
        Assert.False(range.Equals(null));
    }

    [Fact]
    public void Equals_WithDifferentType_ReturnsFalse()
    {
        var range = new PageRange(100);
        object obj = "100";
        
        Assert.False(range.Equals(obj));
    }

    [Fact]
    public void GetHashCode_SameRange_SameHash()
    {
        var range1 = new PageRange(100, 110);
        var range2 = new PageRange(100, 110);
        
        Assert.Equal(range1.GetHashCode(), range2.GetHashCode());
    }
}

