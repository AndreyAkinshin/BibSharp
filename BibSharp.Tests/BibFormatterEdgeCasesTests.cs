using BibSharp.Models;
using BibSharp.Utilities;

namespace BibSharp.Tests;

public class BibFormatterEdgeCasesTests
{
    [Fact]
    public void Format_Chicago_TwoAuthors()
    {
        var entry = new BibEntry(EntryType.Article)
        {
            Key = "test",
            Title = "Test",
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
    public void Format_Chicago_ThreeAuthors()
    {
        var entry = new BibEntry(EntryType.Article)
        {
            Key = "test",
            Title = "Test",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John"));
        entry.AddAuthor(new Author("Doe", "Jane"));
        entry.AddAuthor(new Author("Johnson", "Bob"));

        var formatted = BibFormatter.Format(entry, CitationStyle.Chicago);
        Assert.Contains("Smith", formatted);
        Assert.Contains("Johnson", formatted);
    }

    [Fact]
    public void Format_Chicago_SingleEditor()
    {
        var entry = new BibEntry(EntryType.Book)
        {
            Key = "test",
            Title = "Test",
            Publisher = "P",
            Year = 2025
        };
        entry.AddEditor(new Author("Smith", "John", "A."));

        var formatted = BibFormatter.Format(entry, CitationStyle.Chicago);
        Assert.Contains("Smith", formatted);
        Assert.Contains("ed", formatted);
    }

    [Fact]
    public void Format_Chicago_TwoEditors()
    {
        var entry = new BibEntry(EntryType.Book)
        {
            Key = "test",
            Title = "Test",
            Publisher = "P",
            Year = 2025
        };
        entry.AddEditor(new Author("Smith", "John"));
        entry.AddEditor(new Author("Doe", "Jane"));

        var formatted = BibFormatter.Format(entry, CitationStyle.Chicago);
        Assert.Contains("eds", formatted);
    }

    [Fact]
    public void Format_Chicago_ThreeEditors()
    {
        var entry = new BibEntry(EntryType.Book)
        {
            Key = "test",
            Title = "Test",
            Publisher = "P",
            Year = 2025
        };
        entry.AddEditor(new Author("Smith", "John"));
        entry.AddEditor(new Author("Doe", "Jane"));
        entry.AddEditor(new Author("Johnson", "Bob"));

        var formatted = BibFormatter.Format(entry, CitationStyle.Chicago);
        Assert.Contains("eds", formatted);
    }

    [Fact]
    public void Format_Chicago_FourPlusEditors()
    {
        var entry = new BibEntry(EntryType.Book)
        {
            Key = "test",
            Title = "Test",
            Publisher = "P",
            Year = 2025
        };
        entry.AddEditor(new Author("Smith", "John"));
        entry.AddEditor(new Author("Doe", "Jane"));
        entry.AddEditor(new Author("Johnson", "Bob"));
        entry.AddEditor(new Author("Williams", "Alice"));

        var formatted = BibFormatter.Format(entry, CitationStyle.Chicago);
        Assert.Contains("et al", formatted);
        Assert.Contains("eds", formatted);
    }

    [Fact]
    public void Format_Mla_TwoEditors()
    {
        var entry = new BibEntry(EntryType.Book)
        {
            Key = "test",
            Title = "Test",
            Publisher = "P",
            Year = 2025
        };
        entry.AddEditor(new Author("Smith", "John", "A."));
        entry.AddEditor(new Author("Doe", "Jane", "B."));

        var formatted = BibFormatter.Format(entry, CitationStyle.Mla);
        Assert.Contains("Smith", formatted);
        Assert.Contains("Doe", formatted);
        Assert.Contains("editors", formatted);
    }

    [Fact]
    public void Format_Mla_ThreePlusEditors()
    {
        var entry = new BibEntry(EntryType.Book)
        {
            Key = "test",
            Title = "Test",
            Publisher = "P",
            Year = 2025
        };
        entry.AddEditor(new Author("Smith", "John"));
        entry.AddEditor(new Author("Doe", "Jane"));
        entry.AddEditor(new Author("Johnson", "Bob"));

        var formatted = BibFormatter.Format(entry, CitationStyle.Mla);
        Assert.Contains("et al", formatted);
        Assert.Contains("editors", formatted);
    }

    [Fact]
    public void Format_Apa_TwoEditors()
    {
        var entry = new BibEntry(EntryType.Book)
        {
            Key = "test",
            Title = "Test",
            Publisher = "P",
            Year = 2025
        };
        entry.AddEditor(new Author("Smith", "John"));
        entry.AddEditor(new Author("Doe", "Jane"));

        var formatted = BibFormatter.Format(entry, CitationStyle.Apa);
        Assert.Contains("Smith", formatted);
        Assert.Contains("&", formatted);
        Assert.Contains("Doe", formatted);
    }

    [Fact]
    public void Format_Apa_ThreePlusEditors()
    {
        var entry = new BibEntry(EntryType.Book)
        {
            Key = "test",
            Title = "Test",
            Publisher = "P",
            Year = 2025
        };
        entry.AddEditor(new Author("Smith", "John"));
        entry.AddEditor(new Author("Doe", "Jane"));
        entry.AddEditor(new Author("Johnson", "Bob"));

        var formatted = BibFormatter.Format(entry, CitationStyle.Apa);
        Assert.Contains("Smith", formatted);
        Assert.Contains("&", formatted);
    }

    [Fact]
    public void Format_Apa_SingleEditor()
    {
        var entry = new BibEntry(EntryType.Book)
        {
            Key = "test",
            Title = "Test",
            Publisher = "P",
            Year = 2025
        };
        entry.AddEditor(new Author("Smith", "John", "A."));

        var formatted = BibFormatter.Format(entry, CitationStyle.Apa);
        Assert.Contains("(Ed.)", formatted);
    }

    [Fact]
    public void Format_Apa_MultipleEditors_UsesEds()
    {
        var entry = new BibEntry(EntryType.Book)
        {
            Key = "test",
            Title = "Test",
            Publisher = "P",
            Year = 2025
        };
        entry.AddEditor(new Author("Smith", "John"));
        entry.AddEditor(new Author("Doe", "Jane"));

        var formatted = BibFormatter.Format(entry, CitationStyle.Apa);
        Assert.Contains("(Eds.)", formatted);
    }

    [Fact]
    public void Format_Apa_TitleEndsWithPunctuation()
    {
        var entry = new BibEntry(EntryType.Article)
        {
            Key = "test",
            Title = "Is this a test?",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John"));

        var formatted = BibFormatter.Format(entry, CitationStyle.Apa);
        Assert.Contains("Is this a test?", formatted);
        // Should not add extra period after question mark
        Assert.DoesNotContain("?.", formatted);
    }

    [Fact]
    public void Format_Apa_TitleEndsWithExclamation()
    {
        var entry = new BibEntry(EntryType.Book)
        {
            Key = "test",
            Title = "Amazing Discovery!",
            Publisher = "P",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John"));

        var formatted = BibFormatter.Format(entry, CitationStyle.Apa);
        Assert.Contains("Amazing Discovery!", formatted);
        Assert.DoesNotContain("!.", formatted);
    }

    [Fact]
    public void Format_Apa_Book_WithEdition()
    {
        var entry = new BibEntry(EntryType.Book)
        {
            Key = "test",
            Title = "Test Book",
            Publisher = "Publisher",
            Edition = "3rd",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John"));

        var formatted = BibFormatter.Format(entry, CitationStyle.Apa);
        Assert.Contains("3rd ed.", formatted);
    }

    [Fact]
    public void Format_Apa_InCollection_WithEditors()
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
        entry.AddEditor(new Author("Doe", "Jane", "A."));
        entry.AddEditor(new Author("Johnson", "Bob", "B."));
        entry.Pages = new PageRange(50, 75);

        var formatted = BibFormatter.Format(entry, CitationStyle.Apa);
        Assert.Contains("(Eds.)", formatted);
        Assert.Contains("pp. 50", formatted);
    }

    [Fact]
    public void Format_Apa_Article_VolumeOnly()
    {
        var entry = new BibEntry(EntryType.Article)
        {
            Key = "test",
            Title = "Test",
            Journal = "Journal",
            Volume = 10,
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John"));

        var formatted = BibFormatter.Format(entry, CitationStyle.Apa);
        Assert.Contains("10", formatted);
    }

    [Fact]
    public void Format_Ieee_InProceedings_WithEditors()
    {
        var entry = new BibEntry(EntryType.InProceedings)
        {
            Key = "test",
            Title = "Paper Title",
            BookTitle = "Conference Proceedings",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John"));
        entry.AddEditor(new Author("Doe", "Jane"));
        entry.AddEditor(new Author("Johnson", "Bob"));

        var formatted = BibFormatter.Format(entry, CitationStyle.Ieee);
        Assert.Contains("Paper Title", formatted);
    }

    [Fact]
    public void Format_Harvard_FourPlusAuthors()
    {
        var entry = new BibEntry(EntryType.Article)
        {
            Key = "test",
            Title = "Test",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John", "A."));
        entry.AddAuthor(new Author("Doe", "Jane"));
        entry.AddAuthor(new Author("Johnson", "Bob"));
        entry.AddAuthor(new Author("Williams", "Alice"));

        var formatted = BibFormatter.Format(entry, CitationStyle.Harvard);
        Assert.Contains("et al", formatted);
    }

    [Fact]
    public void Format_Harvard_TwoEditors()
    {
        var entry = new BibEntry(EntryType.Book)
        {
            Key = "test",
            Title = "Test",
            Publisher = "P",
            Year = 2025
        };
        entry.AddEditor(new Author("Smith", "John", "A."));
        entry.AddEditor(new Author("Doe", "Jane"));

        var formatted = BibFormatter.Format(entry, CitationStyle.Harvard);
        Assert.Contains("(eds.)", formatted);
    }

    [Fact]
    public void Format_Harvard_ThreeEditors()
    {
        var entry = new BibEntry(EntryType.Book)
        {
            Key = "test",
            Title = "Test",
            Publisher = "P",
            Year = 2025
        };
        entry.AddEditor(new Author("Smith", "John"));
        entry.AddEditor(new Author("Doe", "Jane"));
        entry.AddEditor(new Author("Johnson", "Bob"));

        var formatted = BibFormatter.Format(entry, CitationStyle.Harvard);
        Assert.Contains("(eds.)", formatted);
    }

    [Fact]
    public void Format_Harvard_FourPlusEditors()
    {
        var entry = new BibEntry(EntryType.Book)
        {
            Key = "test",
            Title = "Test",
            Publisher = "P",
            Year = 2025
        };
        for (int i = 0; i < 5; i++)
        {
            entry.AddEditor(new Author($"Editor{i}", "First"));
        }

        var formatted = BibFormatter.Format(entry, CitationStyle.Harvard);
        Assert.Contains("et al", formatted);
        Assert.Contains("(eds.)", formatted);
    }

    [Fact]
    public void Format_Harvard_InProceedings_WithEditorsAndAuthors()
    {
        var entry = new BibEntry(EntryType.InProceedings)
        {
            Key = "test",
            Title = "Paper Title",
            BookTitle = "Book Title",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John"));
        entry.AddEditor(new Author("Doe", "Jane", "A."));

        var formatted = BibFormatter.Format(entry, CitationStyle.Harvard);
        Assert.Contains("Paper Title", formatted);
        Assert.Contains("Book Title", formatted);
    }

    [Fact]
    public void Format_Harvard_InProceedings_WithEdition()
    {
        var entry = new BibEntry(EntryType.InProceedings)
        {
            Key = "test",
            Title = "Paper Title",
            BookTitle = "Book Title",
            Edition = "2nd",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John"));

        var formatted = BibFormatter.Format(entry, CitationStyle.Harvard);
        Assert.Contains("2nd edn", formatted);
    }

    [Fact]
    public void Format_Ieee_InProceedings_WithBookTitle()
    {
        var entry = new BibEntry(EntryType.InProceedings)
        {
            Key = "test",
            Title = "Paper Title",
            BookTitle = "Conference Proceedings",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John"));

        var formatted = BibFormatter.Format(entry, CitationStyle.Ieee);
        Assert.Contains("in", formatted);
        Assert.Contains("Conference Proceedings", formatted);
    }

    [Fact]
    public void Format_Ieee_Book_WithEdition()
    {
        var entry = new BibEntry(EntryType.Book)
        {
            Key = "test",
            Title = "Test Book",
            Publisher = "Publisher",
            Edition = "5th",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John"));

        var formatted = BibFormatter.Format(entry, CitationStyle.Ieee);
        Assert.Contains("5th ed.", formatted);
    }

    [Fact]
    public void Format_Ieee_InCollection_WithEdition()
    {
        var entry = new BibEntry(EntryType.InCollection)
        {
            Key = "test",
            Title = "Chapter",
            BookTitle = "Book",
            Publisher = "Publisher",
            Edition = "1st",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John"));

        var formatted = BibFormatter.Format(entry, CitationStyle.Ieee);
        Assert.Contains("1st ed.", formatted);
    }

    [Fact]
    public void Format_Apa_AuthorWithNoFirstOrMiddleName()
    {
        var entry = new BibEntry(EntryType.Article)
        {
            Key = "test",
            Title = "Test",
            Year = 2025
        };
        entry.AddAuthor(new Author("Corporation"));

        var formatted = BibFormatter.Format(entry, CitationStyle.Apa);
        Assert.Contains("Corporation", formatted);
    }

    [Fact]
    public void Format_Harvard_AuthorWithNoFirstName()
    {
        var entry = new BibEntry(EntryType.Article)
        {
            Key = "test",
            Title = "Test",
            Year = 2025
        };
        entry.AddAuthor(new Author("Organization"));

        var formatted = BibFormatter.Format(entry, CitationStyle.Harvard);
        Assert.Contains("Organization", formatted);
    }

    [Fact]
    public void Format_Harvard_EditorWithNoFirstName()
    {
        var entry = new BibEntry(EntryType.Book)
        {
            Key = "test",
            Title = "Test",
            Publisher = "P",
            Year = 2025
        };
        entry.AddEditor(new Author("Corporation"));

        var formatted = BibFormatter.Format(entry, CitationStyle.Harvard);
        Assert.Contains("Corporation", formatted);
    }

    [Fact]
    public void Format_Ieee_AuthorWithNoFirstOrMiddleName()
    {
        var entry = new BibEntry(EntryType.Article)
        {
            Key = "test",
            Title = "Test",
            Year = 2025
        };
        entry.AddAuthor(new Author("Corporation"));

        var formatted = BibFormatter.Format(entry, CitationStyle.Ieee);
        Assert.Contains("Corporation", formatted);
    }

    [Fact]
    public void Format_Proceedings_AsEntryType()
    {
        var entry = new BibEntry(EntryType.Proceedings)
        {
            Key = "test",
            Title = "Proceedings Title",
            Year = 2025
        };
        entry.AddEditor(new Author("Smith", "John"));

        var formatted = BibFormatter.Format(entry, CitationStyle.Chicago);
        Assert.Contains("Proceedings Title", formatted);
    }

    [Fact]
    public void Format_DefaultType_QuotesTitle()
    {
        var entry = new BibEntry("customtype")
        {
            Key = "test",
            Title = "Custom Type Title",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John"));

        var formatted = BibFormatter.Format(entry, CitationStyle.Chicago);
        Assert.Contains("Custom Type Title", formatted);
    }
}

