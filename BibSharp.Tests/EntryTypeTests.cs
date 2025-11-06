namespace BibSharp.Tests;

public class EntryTypeTests
{
    [Fact]
    public void Constructor_CreatesEntryType()
    {
        var entryType = new EntryType("article");
        Assert.Equal("article", entryType.Value);
    }

    [Fact]
    public void Constructor_ConvertsToLowerCase()
    {
        var entryType = new EntryType("ARTICLE");
        Assert.Equal("article", entryType.Value);
    }

    [Fact]
    public void Constructor_ThrowsOnNull()
    {
        Assert.Throws<ArgumentNullException>(() => new EntryType(null!));
    }

    [Fact]
    public void ToString_ReturnsValue()
    {
        var entryType = new EntryType("book");
        Assert.Equal("book", entryType.ToString());
    }

    [Fact]
    public void ImplicitConversion_FromString()
    {
        EntryType entryType = "conference";
        Assert.Equal("conference", entryType.Value);
    }

    [Fact]
    public void ImplicitConversion_ToString()
    {
        var entryType = new EntryType("inproceedings");
        string value = entryType;
        Assert.Equal("inproceedings", value);
    }

    [Fact]
    public void Equals_SameValue_ReturnsTrue()
    {
        var type1 = new EntryType("article");
        var type2 = new EntryType("article");
        Assert.True(type1.Equals(type2));
        Assert.True(type1 == type2);
        Assert.False(type1 != type2);
    }

    [Fact]
    public void Equals_DifferentValue_ReturnsFalse()
    {
        var type1 = new EntryType("article");
        var type2 = new EntryType("book");
        Assert.False(type1.Equals(type2));
        Assert.False(type1 == type2);
        Assert.True(type1 != type2);
    }

    [Fact]
    public void Equals_CaseInsensitive()
    {
        var type1 = new EntryType("Article");
        var type2 = new EntryType("ARTICLE");
        Assert.True(type1.Equals(type2));
        Assert.True(type1 == type2);
    }

    [Fact]
    public void Equals_WithObject_ReturnsFalse()
    {
        var type1 = new EntryType("article");
        object obj = "article";
        Assert.False(type1.Equals(obj));
    }

    [Fact]
    public void GetHashCode_SameValue_SameHash()
    {
        var type1 = new EntryType("article");
        var type2 = new EntryType("ARTICLE");
        Assert.Equal(type1.GetHashCode(), type2.GetHashCode());
    }

    [Fact]
    public void PredefinedTypes_Article()
    {
        Assert.Equal("article", EntryType.Article.Value);
    }

    [Fact]
    public void PredefinedTypes_Book()
    {
        Assert.Equal("book", EntryType.Book.Value);
    }

    [Fact]
    public void PredefinedTypes_InProceedings()
    {
        Assert.Equal("inproceedings", EntryType.InProceedings.Value);
    }

    [Fact]
    public void PredefinedTypes_Conference()
    {
        Assert.Equal("conference", EntryType.Conference.Value);
    }

    [Fact]
    public void PredefinedTypes_InCollection()
    {
        Assert.Equal("incollection", EntryType.InCollection.Value);
    }

    [Fact]
    public void PredefinedTypes_InBook()
    {
        Assert.Equal("inbook", EntryType.InBook.Value);
    }

    [Fact]
    public void PredefinedTypes_Booklet()
    {
        Assert.Equal("booklet", EntryType.Booklet.Value);
    }

    [Fact]
    public void PredefinedTypes_PhdThesis()
    {
        Assert.Equal("phdthesis", EntryType.PhdThesis.Value);
    }

    [Fact]
    public void PredefinedTypes_MastersThesis()
    {
        Assert.Equal("mastersthesis", EntryType.MastersThesis.Value);
    }

    [Fact]
    public void PredefinedTypes_TechReport()
    {
        Assert.Equal("techreport", EntryType.TechReport.Value);
    }

    [Fact]
    public void PredefinedTypes_Manual()
    {
        Assert.Equal("manual", EntryType.Manual.Value);
    }

    [Fact]
    public void PredefinedTypes_Proceedings()
    {
        Assert.Equal("proceedings", EntryType.Proceedings.Value);
    }

    [Fact]
    public void PredefinedTypes_Unpublished()
    {
        Assert.Equal("unpublished", EntryType.Unpublished.Value);
    }

    [Fact]
    public void PredefinedTypes_Misc()
    {
        Assert.Equal("misc", EntryType.Misc.Value);
    }

    [Fact]
    public void Constructor_NullValue_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => new EntryType(null!));
    }
}

