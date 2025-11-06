using BibSharp.Models;
using BibSharp.Utilities;

namespace BibSharp.Tests.Utilities;

public class BibFormatterComprehensiveTests
{
    [Fact]
    public void Format_NullEntry_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => 
            BibFormatter.Format(null!, CitationStyle.Chicago));
    }

    [Fact]
    public void FormatBibliography_NullEntries_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => 
            BibFormatter.FormatBibliography(null!, CitationStyle.Chicago));
    }

    [Fact]
    public void Format_UnsupportedStyle_ThrowsException()
    {
        var entry = new BibEntry(EntryType.Misc) { Key = "test", Title = "Test" };
        Assert.Throws<ArgumentException>(() => 
            BibFormatter.Format(entry, (CitationStyle)999));
    }

    [Fact]
    public void Format_Chicago_SingleAuthor()
    {
        var entry = new BibEntry(EntryType.Article)
        {
            Key = "test",
            Title = "Test Article",
            Journal = "Test Journal",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John", "A."));

        var formatted = BibFormatter.Format(entry, CitationStyle.Chicago);

        Assert.Contains("Smith, John A.", formatted);
        Assert.Contains("2025", formatted);
        Assert.Contains("Test Article", formatted);
    }

    [Fact]
    public void Format_Chicago_MultipleAuthors()
    {
        var entry = new BibEntry(EntryType.Article)
        {
            Key = "test",
            Title = "Test Article",
            Journal = "Test Journal",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John"));
        entry.AddAuthor(new Author("Doe", "Jane"));

        var formatted = BibFormatter.Format(entry, CitationStyle.Chicago);

        Assert.Contains("Smith", formatted);
        Assert.Contains("Doe", formatted);
        Assert.Contains("and", formatted);
    }

    [Fact]
    public void Format_Chicago_FourPlusAuthors_UsesEtAl()
    {
        var entry = new BibEntry(EntryType.Article)
        {
            Key = "test",
            Title = "Test Article",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John"));
        entry.AddAuthor(new Author("Doe", "Jane"));
        entry.AddAuthor(new Author("Johnson", "Bob"));
        entry.AddAuthor(new Author("Williams", "Alice"));

        var formatted = BibFormatter.Format(entry, CitationStyle.Chicago);

        Assert.Contains("Smith", formatted);
        Assert.Contains("et al", formatted);
        Assert.DoesNotContain("Williams", formatted);
    }

    [Fact]
    public void Format_Mla_TwoAuthors()
    {
        var entry = new BibEntry(EntryType.Article)
        {
            Key = "test",
            Title = "Test Article",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John"));
        entry.AddAuthor(new Author("Doe", "Jane"));

        var formatted = BibFormatter.Format(entry, CitationStyle.Mla);

        Assert.Contains("Smith", formatted);
        Assert.Contains("Doe", formatted);
    }

    [Fact]
    public void Format_Mla_ThreePlusAuthors_UsesEtAl()
    {
        var entry = new BibEntry(EntryType.Article)
        {
            Key = "test",
            Title = "Test Article",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John"));
        entry.AddAuthor(new Author("Doe", "Jane"));
        entry.AddAuthor(new Author("Johnson", "Bob"));

        var formatted = BibFormatter.Format(entry, CitationStyle.Mla);

        Assert.Contains("Smith", formatted);
        Assert.Contains("et al", formatted);
    }

    [Fact]
    public void Format_Apa_IncludesYear()
    {
        var entry = new BibEntry(EntryType.Article)
        {
            Key = "test",
            Title = "Test Article",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John"));

        var formatted = BibFormatter.Format(entry, CitationStyle.Apa);

        Assert.Contains("(2025)", formatted);
    }

    [Fact]
    public void Format_Apa_NoYear_UsesNd()
    {
        var entry = new BibEntry(EntryType.Article)
        {
            Key = "test",
            Title = "Test Article"
        };
        entry.AddAuthor(new Author("Smith", "John"));

        var formatted = BibFormatter.Format(entry, CitationStyle.Apa);

        Assert.Contains("(n.d.)", formatted);
    }

    [Fact]
    public void Format_Apa_TwentyPlusAuthors_UsesEllipsis()
    {
        var entry = new BibEntry(EntryType.Article)
        {
            Key = "test",
            Title = "Test Article",
            Year = 2025
        };
        
        for (int i = 1; i <= 25; i++)
        {
            entry.AddAuthor(new Author($"Author{i}", "First"));
        }

        var formatted = BibFormatter.Format(entry, CitationStyle.Apa);

        Assert.Contains(". . .", formatted);
        Assert.Contains("Author1", formatted);
        Assert.Contains("Author25", formatted); // Last author should be included
    }

    [Fact]
    public void Format_Harvard_WithEditors()
    {
        var entry = new BibEntry(EntryType.Book)
        {
            Key = "test",
            Title = "Test Book",
            Year = 2025
        };
        entry.AddEditor(new Author("Smith", "John"));

        var formatted = BibFormatter.Format(entry, CitationStyle.Harvard);

        Assert.Contains("Smith", formatted);
        Assert.Contains("(ed.)", formatted);
    }

    [Fact]
    public void Format_Harvard_MultipleEditors()
    {
        var entry = new BibEntry(EntryType.Book)
        {
            Key = "test",
            Title = "Test Book",
            Year = 2025
        };
        entry.AddEditor(new Author("Smith", "John"));
        entry.AddEditor(new Author("Doe", "Jane"));

        var formatted = BibFormatter.Format(entry, CitationStyle.Harvard);

        Assert.Contains("(eds.)", formatted);
    }

    [Fact]
    public void Format_Ieee_AuthorsWithInitials()
    {
        var entry = new BibEntry(EntryType.Article)
        {
            Key = "test",
            Title = "Test Article",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John", "A."));

        var formatted = BibFormatter.Format(entry, CitationStyle.Ieee);

        Assert.Contains("J. A. Smith", formatted);
    }

    [Fact]
    public void Format_Ieee_SevenPlusAuthors_UsesEtAl()
    {
        var entry = new BibEntry(EntryType.Article)
        {
            Key = "test",
            Title = "Test Article",
            Year = 2025
        };
        
        for (int i = 1; i <= 10; i++)
        {
            entry.AddAuthor(new Author($"Author{i}", "First"));
        }

        var formatted = BibFormatter.Format(entry, CitationStyle.Ieee);

        Assert.Contains("et al", formatted);
    }

    [Fact]
    public void Format_NoAuthorsOrEditors_UsesTitleAsLeadingElement()
    {
        var entry = new BibEntry(EntryType.Misc)
        {
            Key = "test",
            Title = "Untitled Work",
            Year = 2025
        };

        var formatted = BibFormatter.Format(entry, CitationStyle.Chicago);

        Assert.Contains("Untitled Work", formatted);
    }

    [Fact]
    public void Format_InProceedings_WithBookTitle()
    {
        var entry = new BibEntry(EntryType.InProceedings)
        {
            Key = "test",
            Title = "Paper Title",
            BookTitle = "Conference Proceedings",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John"));

        var formatted = BibFormatter.Format(entry, CitationStyle.Chicago);

        Assert.Contains("Paper Title", formatted);
        Assert.Contains("Conference Proceedings", formatted);
    }

    [Fact]
    public void Format_Book_WithEdition()
    {
        var entry = new BibEntry(EntryType.Book)
        {
            Key = "test",
            Title = "Test Book",
            Publisher = "Test Publisher",
            Edition = "2nd",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John"));

        var formatted = BibFormatter.Format(entry, CitationStyle.Chicago);

        Assert.Contains("2nd ed.", formatted);
    }

    [Fact]
    public void Format_WithDoi()
    {
        var entry = new BibEntry(EntryType.Article)
        {
            Key = "test",
            Title = "Test Article",
            Doi = "10.1234/test",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John"));

        var formatted = BibFormatter.Format(entry, CitationStyle.Chicago);

        Assert.Contains("10.1234/test", formatted);
        Assert.Contains("DOI", formatted);
    }

    [Fact]
    public void Format_WithUrl()
    {
        var entry = new BibEntry(EntryType.Misc)
        {
            Key = "test",
            Title = "Test",
            Url = "https://example.com",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John"));

        var formatted = BibFormatter.Format(entry, CitationStyle.Chicago);

        Assert.Contains("https://example.com", formatted);
    }

    [Fact]
    public void Format_Article_WithVolumeAndNumber()
    {
        var entry = new BibEntry(EntryType.Article)
        {
            Key = "test",
            Title = "Test Article",
            Journal = "Test Journal",
            Volume = 10,
            Number = "5",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John"));

        var formatted = BibFormatter.Format(entry, CitationStyle.Chicago);

        Assert.Contains("10", formatted);
        Assert.Contains("5", formatted);
    }

    [Fact]
    public void Format_Article_WithPages()
    {
        var entry = new BibEntry(EntryType.Article)
        {
            Key = "test",
            Title = "Test Article",
            Journal = "Test Journal",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John"));
        entry.Pages = new PageRange(100, 110);

        var formatted = BibFormatter.Format(entry, CitationStyle.Chicago);

        Assert.Contains("100", formatted);
        Assert.Contains("110", formatted);
    }

    [Fact]
    public void FormatBibliography_MultipleEntries()
    {
        var entries = new[]
        {
            new BibEntry(EntryType.Article) { Key = "test1", Title = "Article 1", Year = 2025 },
            new BibEntry(EntryType.Book) { Key = "test2", Title = "Book 1", Year = 2024 }
        };
        entries[0].AddAuthor(new Author("Smith", "John"));
        entries[1].AddAuthor(new Author("Doe", "Jane"));

        var bibliography = BibFormatter.FormatBibliography(entries, CitationStyle.Chicago);

        Assert.Contains("Article 1", bibliography);
        Assert.Contains("Book 1", bibliography);
        Assert.Contains("Smith", bibliography);
        Assert.Contains("Doe", bibliography);
    }

    [Fact]
    public void Format_InCollection_WithEditorsAndAuthors()
    {
        var entry = new BibEntry(EntryType.InCollection)
        {
            Key = "test",
            Title = "Chapter Title",
            BookTitle = "Book Title",
            Publisher = "Publisher",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John"));
        entry.AddEditor(new Author("Doe", "Jane"));

        var formatted = BibFormatter.Format(entry, CitationStyle.Chicago);

        Assert.Contains("Smith", formatted);
        Assert.Contains("Chapter Title", formatted);
        Assert.Contains("Book Title", formatted);
    }

    [Fact]
    public void Format_Apa_InCollection_WithPages()
    {
        var entry = new BibEntry(EntryType.InCollection)
        {
            Key = "test",
            Title = "Chapter Title",
            BookTitle = "Book Title",
            Publisher = "Publisher",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John"));
        entry.AddEditor(new Author("Doe", "Jane"));
        entry.Pages = new PageRange(50, 75);

        var formatted = BibFormatter.Format(entry, CitationStyle.Apa);

        Assert.Contains("pp. 50", formatted);
        Assert.Contains("75", formatted);
    }

    [Fact]
    public void Format_WithPublisherAndAddress()
    {
        var entry = new BibEntry(EntryType.Book)
        {
            Key = "test",
            Title = "Test Book",
            Publisher = "Test Publisher",
            Address = "New York",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John"));

        var formatted = BibFormatter.Format(entry, CitationStyle.Chicago);

        Assert.Contains("New York", formatted);
        Assert.Contains("Test Publisher", formatted);
    }

    [Fact]
    public void Format_Apa_Article_WithVolumeAndIssue()
    {
        var entry = new BibEntry(EntryType.Article)
        {
            Key = "test",
            Title = "Test Article",
            Journal = "Test Journal",
            Volume = 10,
            Number = "5",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John"));

        var formatted = BibFormatter.Format(entry, CitationStyle.Apa);

        Assert.Contains("10", formatted);
        Assert.Contains("(5)", formatted);
    }

    [Fact]
    public void Format_Ieee_WithEditors()
    {
        var entry = new BibEntry(EntryType.Book)
        {
            Key = "test",
            Title = "Test Book",
            Publisher = "Publisher",
            Year = 2025
        };
        entry.AddEditor(new Author("Smith", "John"));

        var formatted = BibFormatter.Format(entry, CitationStyle.Ieee);

        Assert.Contains("J. Smith", formatted);
        Assert.Contains("Ed.", formatted);
    }

    [Fact]
    public void Format_Ieee_MultipleEditors()
    {
        var entry = new BibEntry(EntryType.Book)
        {
            Key = "test",
            Title = "Test Book",
            Publisher = "Publisher",
            Year = 2025
        };
        entry.AddEditor(new Author("Smith", "John"));
        entry.AddEditor(new Author("Doe", "Jane"));

        var formatted = BibFormatter.Format(entry, CitationStyle.Ieee);

        Assert.Contains("Eds.", formatted);
    }
}

