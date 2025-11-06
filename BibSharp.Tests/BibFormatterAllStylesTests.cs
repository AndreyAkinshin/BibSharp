using BibSharp.Models;
using BibSharp.Utilities;

namespace BibSharp.Tests;

public class BibFormatterAllStylesTests
{
    private BibEntry CreateSampleArticle()
    {
        var entry = new BibEntry(EntryType.Article)
        {
            Key = "test2025",
            Title = "Sample Article Title",
            Journal = "Sample Journal",
            Year = 2025,
            Volume = 10,
            Number = "5"
        };
        entry.AddAuthor(new Author("Smith", "John", "A."));
        entry.Pages = new PageRange(100, 110);
        return entry;
    }

    private BibEntry CreateSampleBook()
    {
        var entry = new BibEntry(EntryType.Book)
        {
            Key = "test2024",
            Title = "Sample Book Title",
            Publisher = "Sample Publisher",
            Address = "New York",
            Edition = "2nd",
            Year = 2024
        };
        entry.AddAuthor(new Author("Doe", "Jane", "B."));
        return entry;
    }

    [Fact]
    public void Chicago_Article_FormatsCorrectly()
    {
        var entry = CreateSampleArticle();
        var formatted = BibFormatter.Format(entry, CitationStyle.Chicago);

        Assert.Contains("Smith", formatted);
        Assert.Contains("2025", formatted);
        Assert.Contains("Sample Article Title", formatted);
        Assert.Contains("Sample Journal", formatted);
        Assert.Contains("10", formatted);
        Assert.Contains("5", formatted);
        Assert.Contains("100", formatted);
        Assert.Contains("110", formatted);
    }

    [Fact]
    public void Chicago_Book_FormatsCorrectly()
    {
        var entry = CreateSampleBook();
        var formatted = BibFormatter.Format(entry, CitationStyle.Chicago);

        Assert.Contains("Doe", formatted);
        Assert.Contains("2024", formatted);
        Assert.Contains("Sample Book Title", formatted);
        Assert.Contains("Sample Publisher", formatted);
        Assert.Contains("2nd", formatted);
    }

    [Fact]
    public void Mla_Article_FormatsCorrectly()
    {
        var entry = CreateSampleArticle();
        var formatted = BibFormatter.Format(entry, CitationStyle.Mla);

        Assert.Contains("Smith", formatted);
        Assert.Contains("Sample Article Title", formatted);
        Assert.Contains("Sample Journal", formatted);
    }

    [Fact]
    public void Mla_Book_FormatsCorrectly()
    {
        var entry = CreateSampleBook();
        var formatted = BibFormatter.Format(entry, CitationStyle.Mla);

        Assert.Contains("Doe", formatted);
        Assert.Contains("Sample Book Title", formatted);
        Assert.Contains("Sample Publisher", formatted);
    }

    [Fact]
    public void Apa_Article_FormatsCorrectly()
    {
        var entry = CreateSampleArticle();
        var formatted = BibFormatter.Format(entry, CitationStyle.Apa);

        Assert.Contains("Smith, J. A.", formatted);
        Assert.Contains("(2025)", formatted);
        Assert.Contains("Sample Article Title", formatted);
    }

    [Fact]
    public void Apa_Book_FormatsCorrectly()
    {
        var entry = CreateSampleBook();
        var formatted = BibFormatter.Format(entry, CitationStyle.Apa);

        Assert.Contains("Doe, J. B.", formatted);
        Assert.Contains("(2024)", formatted);
        Assert.Contains("Sample Book Title", formatted);
    }

    [Fact]
    public void Harvard_Article_FormatsCorrectly()
    {
        var entry = CreateSampleArticle();
        var formatted = BibFormatter.Format(entry, CitationStyle.Harvard);

        Assert.Contains("Smith", formatted);
        Assert.Contains("(2025)", formatted);
        Assert.Contains("Sample Journal", formatted);
    }

    [Fact]
    public void Harvard_Book_FormatsCorrectly()
    {
        var entry = CreateSampleBook();
        var formatted = BibFormatter.Format(entry, CitationStyle.Harvard);

        Assert.Contains("Doe", formatted);
        Assert.Contains("(2024)", formatted);
        Assert.Contains("Sample Book Title", formatted);
    }

    [Fact]
    public void Ieee_Article_FormatsCorrectly()
    {
        var entry = CreateSampleArticle();
        var formatted = BibFormatter.Format(entry, CitationStyle.Ieee);

        Assert.Contains("J. A. Smith", formatted);
        Assert.Contains("2025", formatted);
        Assert.Contains("Sample Article Title", formatted);
    }

    [Fact]
    public void Ieee_Book_FormatsCorrectly()
    {
        var entry = CreateSampleBook();
        var formatted = BibFormatter.Format(entry, CitationStyle.Ieee);

        Assert.Contains("J. B. Doe", formatted);
        Assert.Contains("2024", formatted);
        Assert.Contains("Sample Book Title", formatted);
    }

    [Fact]
    public void Format_Article_WithoutVolumeOrNumber()
    {
        var entry = new BibEntry(EntryType.Article)
        {
            Key = "test",
            Title = "Test",
            Journal = "J",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John"));

        var formatted = BibFormatter.Format(entry, CitationStyle.Chicago);
        Assert.Contains("Smith", formatted);
    }

    [Fact]
    public void Format_InProceedings_WithoutPages()
    {
        var entry = new BibEntry(EntryType.InProceedings)
        {
            Key = "test",
            Title = "Test",
            BookTitle = "Proceedings",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John"));

        var formatted = BibFormatter.Format(entry, CitationStyle.Chicago);
        Assert.Contains("Proceedings", formatted);
    }

    [Fact]
    public void Format_Book_WithoutEdition()
    {
        var entry = new BibEntry(EntryType.Book)
        {
            Key = "test",
            Title = "Test Book",
            Publisher = "Publisher",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John"));

        var formatted = BibFormatter.Format(entry, CitationStyle.Chicago);
        Assert.Contains("Test Book", formatted);
    }

    [Fact]
    public void Format_WithOnlyDoi_NoDuplicateUrl()
    {
        var entry = new BibEntry(EntryType.Article)
        {
            Key = "test",
            Title = "Test",
            Doi = "10.1234/test",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John"));

        var formatted = BibFormatter.Format(entry, CitationStyle.Apa);
        Assert.Contains("doi.org", formatted);
    }

    [Fact]
    public void Format_WithBothDoiAndUrl_PrefersDoiInApa()
    {
        var entry = new BibEntry(EntryType.Article)
        {
            Key = "test",
            Title = "Test",
            Doi = "10.1234/test",
            Url = "https://example.com",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John"));

        var formatted = BibFormatter.Format(entry, CitationStyle.Apa);
        Assert.Contains("doi.org", formatted);
        Assert.DoesNotContain("https://example.com", formatted);
    }

    [Fact]
    public void Format_AuthorWithNoFirstName()
    {
        var entry = new BibEntry(EntryType.Article)
        {
            Key = "test",
            Title = "Test",
            Year = 2025
        };
        entry.AddAuthor(new Author("Corporation"));

        var formatted = BibFormatter.Format(entry, CitationStyle.Chicago);
        Assert.Contains("Corporation", formatted);
    }

    [Fact]
    public void Format_EditorWithNoFirstName()
    {
        var entry = new BibEntry(EntryType.Book)
        {
            Key = "test",
            Title = "Test",
            Publisher = "P",
            Year = 2025
        };
        entry.AddEditor(new Author("Organization"));

        var formatted = BibFormatter.Format(entry, CitationStyle.Chicago);
        Assert.Contains("Organization", formatted);
    }
}

