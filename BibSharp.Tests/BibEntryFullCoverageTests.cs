using BibSharp.Models;

namespace BibSharp.Tests;

public class BibEntryFullCoverageTests
{
    [Fact]
    public void Clone_ClonesAllFieldTypes()
    {
        var original = new BibEntry(EntryType.Article)
        {
            Key = "test",
            Title = "Title",
            Journal = "Journal",
            BookTitle = "BookTitle",
            Publisher = "Publisher",
            Year = 2025,
            Volume = 10,
            Number = "5",
            Doi = "10.1234/test",
            Url = "https://example.com",
            Note = "Note",
            Edition = "1st",
            Series = "Series",
            Chapter = "1",
            Address = "Address",
            HowPublished = "Online",
            Isbn = "1234567890",
            Issn = "1234-5678",
            Institution = "Institution",
            School = "School",
            Organization = "Org",
            Type = "Type",
            Language = "en",
            Keywords = "k1, k2",
            Abstract = "Abstract",
            Annote = "Annote",
            Copyright = "Copyright",
            Arxiv = "1234.5678"
        };
        original.AddAuthor(new Author("Smith", "John", "A.", "Jr."));
        original.AddEditor(new Author("Doe", "Jane"));
        original.Pages = new PageRange(10, 20);
        original.Month = BibSharp.Month.January;

        var clone = original.Clone();

        Assert.Equal(original.Title, clone.Title);
        Assert.Equal(original.Journal, clone.Journal);
        Assert.Equal(original.BookTitle, clone.BookTitle);
        Assert.Equal(original.Publisher, clone.Publisher);
        Assert.Equal(original.Year, clone.Year);
        Assert.Equal(original.Volume, clone.Volume);
        Assert.Equal(original.Number, clone.Number);
        Assert.Equal(original.Doi, clone.Doi);
        Assert.Equal(original.Url, clone.Url);
        Assert.Equal(original.Note, clone.Note);
        Assert.Equal(original.Edition, clone.Edition);
        Assert.Equal(original.Series, clone.Series);
        Assert.Equal(original.Chapter, clone.Chapter);
        Assert.Equal(original.Address, clone.Address);
        Assert.Equal(original.HowPublished, clone.HowPublished);
        Assert.Equal(original.Isbn, clone.Isbn);
        Assert.Equal(original.Issn, clone.Issn);
        Assert.Equal(original.School, clone.School);
        Assert.Equal(original.Organization, clone.Organization);
        Assert.Equal(original.Type, clone.Type);
        Assert.Equal(original.Language, clone.Language);
        Assert.Equal(original.Keywords, clone.Keywords);
        Assert.Equal(original.Abstract, clone.Abstract);
        Assert.Equal(original.Annote, clone.Annote);
        Assert.Equal(original.Copyright, clone.Copyright);
        Assert.Equal(original.Arxiv, clone.Arxiv);
    }

    [Fact]
    public void HasField_WithAlias_ChecksBoth()
    {
        var entry = new BibEntry { Key = "test" };
        entry.SetField("journaltitle", "Test Journal");
        
        // Should find via both the alias and the canonical name
        Assert.True(entry.HasField("journal"));
        Assert.True(entry.HasField("journaltitle"));
    }

    [Fact]
    public void GetField_WithAlias_ReturnsValue()
    {
        var entry = new BibEntry { Key = "test" };
        entry.SetField("location", "New York");
        
        // location is an alias for address
        Assert.Equal("New York", entry.GetField("address"));
        Assert.Equal("New York", entry.Address);
    }

    [Fact]
    public void Year_EmptyString_ReturnsNull()
    {
        var entry = new BibEntry { Key = "test" };
        entry.SetField("year", "");
        
        Assert.Null(entry.Year);
    }

    [Fact]
    public void Volume_EmptyString_ReturnsNull()
    {
        var entry = new BibEntry { Key = "test" };
        entry.SetField("volume", "");
        
        Assert.Null(entry.Volume);
    }

    [Fact]
    public void BibEntry_MultipleAuthors_AllParsed()
    {
        var entry = new BibEntry { Key = "test" };
        entry.SetField("author", "Smith, John and Doe, Jane and Johnson, Bob and Williams, Alice");
        
        Assert.Equal(4, entry.Authors.Count);
        Assert.Equal("Smith", entry.Authors[0].LastName);
        Assert.Equal("Doe", entry.Authors[1].LastName);
        Assert.Equal("Johnson", entry.Authors[2].LastName);
        Assert.Equal("Williams", entry.Authors[3].LastName);
    }

    [Fact]
    public void BibEntry_EmptyAuthorString_ClearsAuthors()
    {
        var entry = new BibEntry { Key = "test" };
        entry.AddAuthor(new Author("Smith", "John"));
        entry.SetField("author", "");
        
        Assert.Empty(entry.Authors);
    }
}

