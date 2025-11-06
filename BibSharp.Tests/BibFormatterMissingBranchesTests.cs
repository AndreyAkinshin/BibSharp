using BibSharp.Models;
using BibSharp.Utilities;

namespace BibSharp.Tests;

public class BibFormatterMissingBranchesTests
{
    [Fact]
    public void Format_Mla_InProceedings_WithEditorsNoAuthors()
    {
        var entry = new BibEntry(EntryType.InProceedings)
        {
            Key = "test",
            Title = "Paper",
            BookTitle = "Book",
            Year = 2025
        };
        entry.AddEditor(new Author("Smith", "John"));

        var formatted = BibFormatter.Format(entry, CitationStyle.Mla);
        Assert.Contains("Smith", formatted);
        Assert.Contains("editor", formatted);
    }

    [Fact]
    public void Format_Mla_InCollection_WithBookTitle()
    {
        var entry = new BibEntry(EntryType.InCollection)
        {
            Key = "test",
            Title = "Chapter",
            BookTitle = "Book",
            Publisher = "P",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John"));

        var formatted = BibFormatter.Format(entry, CitationStyle.Mla);
        Assert.Contains("Chapter", formatted);
        Assert.Contains("Book", formatted);
    }

    [Fact]
    public void Format_Mla_Book_WithEditionAndPublisher()
    {
        var entry = new BibEntry(EntryType.Book)
        {
            Key = "test",
            Title = "Test Book",
            Publisher = "Publisher",
            Edition = "3rd",
            Address = "Boston",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John"));

        var formatted = BibFormatter.Format(entry, CitationStyle.Mla);
        Assert.Contains("3rd ed.", formatted);
        Assert.Contains("Publisher", formatted);
        Assert.Contains("Boston", formatted);
    }

    [Fact]
    public void Format_Apa_InProceedings_NoEditors()
    {
        var entry = new BibEntry(EntryType.InProceedings)
        {
            Key = "test",
            Title = "Paper",
            BookTitle = "Proceedings",
            Publisher = "P",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John"));

        var formatted = BibFormatter.Format(entry, CitationStyle.Apa);
        Assert.Contains("Paper", formatted);
        Assert.Contains("Proceedings", formatted);
    }

    [Fact]
    public void Format_Apa_InCollection_NoPages()
    {
        var entry = new BibEntry(EntryType.InCollection)
        {
            Key = "test",
            Title = "Chapter",
            BookTitle = "Book",
            Publisher = "P",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John"));
        entry.AddEditor(new Author("Doe", "Jane"));

        var formatted = BibFormatter.Format(entry, CitationStyle.Apa);
        Assert.Contains("Chapter", formatted);
        Assert.Contains("Book", formatted);
    }

    [Fact]
    public void Format_Apa_InProceedings_SingleEditor()
    {
        var entry = new BibEntry(EntryType.InProceedings)
        {
            Key = "test",
            Title = "Paper",
            BookTitle = "Proceedings",
            Publisher = "P",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John"));
        entry.AddEditor(new Author("Doe", "Jane", "A."));

        var formatted = BibFormatter.Format(entry, CitationStyle.Apa);
        Assert.Contains("(Ed.)", formatted);
    }

    [Fact]
    public void Format_Apa_Article_NoVolume()
    {
        var entry = new BibEntry(EntryType.Article)
        {
            Key = "test",
            Title = "Test",
            Journal = "Journal",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John"));

        var formatted = BibFormatter.Format(entry, CitationStyle.Apa);
        Assert.Contains("Journal", formatted);
    }

    [Fact]
    public void Format_Apa_Book_NoAddress()
    {
        var entry = new BibEntry(EntryType.Book)
        {
            Key = "test",
            Title = "Test",
            Publisher = "Publisher",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John"));

        var formatted = BibFormatter.Format(entry, CitationStyle.Apa);
        Assert.Contains("Publisher", formatted);
    }

    [Fact]
    public void Format_Harvard_InCollection_WithEditors()
    {
        var entry = new BibEntry(EntryType.InCollection)
        {
            Key = "test",
            Title = "Chapter",
            BookTitle = "Book",
            Publisher = "P",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John"));
        entry.AddEditor(new Author("Doe", "Jane", "A."));

        var formatted = BibFormatter.Format(entry, CitationStyle.Harvard);
        Assert.Contains("Chapter", formatted);
    }

    [Fact]
    public void Format_Harvard_InCollection_NoEdition()
    {
        var entry = new BibEntry(EntryType.InCollection)
        {
            Key = "test",
            Title = "Chapter",
            BookTitle = "Book",
            Publisher = "P",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John"));
        entry.AddEditor(new Author("Doe", "Jane"));
        entry.AddEditor(new Author("Johnson", "Bob"));

        var formatted = BibFormatter.Format(entry, CitationStyle.Harvard);
        Assert.Contains("Chapter", formatted);
    }

    [Fact]
    public void Format_Harvard_TwoAuthors()
    {
        var entry = new BibEntry(EntryType.Article)
        {
            Key = "test",
            Title = "Test",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John", "A."));
        entry.AddAuthor(new Author("Doe", "Jane", "B."));

        var formatted = BibFormatter.Format(entry, CitationStyle.Harvard);
        Assert.Contains("Smith", formatted);
        Assert.Contains("Doe", formatted);
        Assert.Contains("and", formatted);
    }

    [Fact]
    public void Format_Harvard_ThreeAuthors()
    {
        var entry = new BibEntry(EntryType.Article)
        {
            Key = "test",
            Title = "Test",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John", "A."));
        entry.AddAuthor(new Author("Doe", "Jane"));
        entry.AddAuthor(new Author("Johnson", "Bob", "C."));

        var formatted = BibFormatter.Format(entry, CitationStyle.Harvard);
        Assert.Contains("Smith", formatted);
        Assert.Contains("Johnson", formatted);
    }

    [Fact]
    public void Format_Harvard_Article_WithVolume()
    {
        var entry = new BibEntry(EntryType.Article)
        {
            Key = "test",
            Title = "Test",
            Journal = "Journal",
            Volume = 15,
            Number = "3",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John"));

        var formatted = BibFormatter.Format(entry, CitationStyle.Harvard);
        Assert.Contains("vol. 15", formatted);
        Assert.Contains("no. 3", formatted);
    }

    [Fact]
    public void Format_Ieee_Article_WithVolumeAndNumber()
    {
        var entry = new BibEntry(EntryType.Article)
        {
            Key = "test",
            Title = "Test",
            Journal = "Journal",
            Volume = 20,
            Number = "7",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John"));

        var formatted = BibFormatter.Format(entry, CitationStyle.Ieee);
        Assert.Contains("vol. 20", formatted);
        Assert.Contains("no. 7", formatted);
    }

    [Fact]
    public void Format_Ieee_InProceedings_SevenPlusEditorsWithAuthors()
    {
        var entry = new BibEntry(EntryType.InProceedings)
        {
            Key = "test",
            Title = "Paper",
            BookTitle = "Proceedings",
            Publisher = "P",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John"));
        
        for (int i = 0; i < 10; i++)
        {
            entry.AddEditor(new Author($"Editor{i}", "First"));
        }

        var formatted = BibFormatter.Format(entry, CitationStyle.Ieee);
        Assert.Contains("et al", formatted);
    }

    [Fact]
    public void Format_Book_WithAddress()
    {
        var entry = new BibEntry(EntryType.Book)
        {
            Key = "test",
            Title = "Test",
            Publisher = "Publisher",
            Address = "London",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John"));

        var formatted = BibFormatter.Format(entry, CitationStyle.Chicago);
        Assert.Contains("London", formatted);
    }

    [Fact]
    public void Format_InProceedings_WithPublisher()
    {
        var entry = new BibEntry(EntryType.InProceedings)
        {
            Key = "test",
            Title = "Paper",
            BookTitle = "Conference",
            Publisher = "IEEE",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John"));

        var formatted = BibFormatter.Format(entry, CitationStyle.Chicago);
        Assert.Contains("IEEE", formatted);
    }

    [Fact]
    public void Format_InCollection_WithPages()
    {
        var entry = new BibEntry(EntryType.InCollection)
        {
            Key = "test",
            Title = "Chapter",
            BookTitle = "Book",
            Publisher = "P",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John"));
        entry.Pages = new PageRange(25, 50);

        var formatted = BibFormatter.Format(entry, CitationStyle.Chicago);
        Assert.Contains("25", formatted);
        Assert.Contains("50", formatted);
    }
}

