using BibSharp.Models;

namespace BibSharp.Tests;

public class BibEntryTypesTests
{
    [Fact]
    public void Validate_Article_RequiredFields()
    {
        var entry = new BibEntry(EntryType.Article)
        {
            Key = "test2025"
        };
        entry.AddAuthor(new Author("Smith", "John"));
        entry.Title = "Test Article";
        entry.Journal = "Test Journal";
        entry.Year = 2025;

        var result = entry.Validate();
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_Article_MissingAuthor()
    {
        var entry = new BibEntry(EntryType.Article)
        {
            Key = "test2025",
            Title = "Test Article",
            Journal = "Test Journal",
            Year = 2025
        };

        var result = entry.Validate();
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("author"));
    }

    [Fact]
    public void Validate_Book_RequiredFields()
    {
        var entry = new BibEntry(EntryType.Book)
        {
            Key = "test2025",
            Title = "Test Book",
            Publisher = "Test Publisher",
            Year = 2025
        };

        var result = entry.Validate();
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_Book_WithAuthor()
    {
        var entry = new BibEntry(EntryType.Book)
        {
            Key = "test2025",
            Title = "Test Book",
            Publisher = "Test Publisher",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John"));

        var result = entry.Validate();
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_Book_WithEditor()
    {
        var entry = new BibEntry(EntryType.Book)
        {
            Key = "test2025",
            Title = "Test Book",
            Publisher = "Test Publisher",
            Year = 2025
        };
        entry.AddEditor(new Author("Smith", "John"));

        var result = entry.Validate();
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_InProceedings_RequiredFields()
    {
        var entry = new BibEntry(EntryType.InProceedings)
        {
            Key = "test2025",
            Title = "Test Paper",
            BookTitle = "Proceedings of Test Conference",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John"));

        var result = entry.Validate();
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_Conference_SameAsInProceedings()
    {
        var entry = new BibEntry(EntryType.Conference)
        {
            Key = "test2025",
            Title = "Test Paper",
            BookTitle = "Proceedings of Test Conference",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John"));

        var result = entry.Validate();
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_InCollection_RequiredFields()
    {
        var entry = new BibEntry(EntryType.InCollection)
        {
            Key = "test2025",
            Title = "Test Chapter",
            BookTitle = "Test Book",
            Publisher = "Test Publisher",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John"));

        var result = entry.Validate();
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_InBook_RequiredFields()
    {
        var entry = new BibEntry(EntryType.InBook)
        {
            Key = "test2025",
            Title = "Test Chapter",
            Publisher = "Test Publisher",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John"));

        var result = entry.Validate();
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_Booklet_RequiredFields()
    {
        var entry = new BibEntry(EntryType.Booklet)
        {
            Key = "test2025",
            Title = "Test Booklet"
        };

        var result = entry.Validate();
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_PhdThesis_RequiredFields()
    {
        var entry = new BibEntry(EntryType.PhdThesis)
        {
            Key = "test2025",
            Title = "Test Thesis",
            School = "Test University",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John"));

        var result = entry.Validate();
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_MastersThesis_RequiredFields()
    {
        var entry = new BibEntry(EntryType.MastersThesis)
        {
            Key = "test2025",
            Title = "Test Thesis",
            School = "Test University",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John"));

        var result = entry.Validate();
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_TechReport_RequiredFields()
    {
        var entry = new BibEntry(EntryType.TechReport)
        {
            Key = "test2025",
            Title = "Test Report",
            Institution = "Test Institution",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John"));

        var result = entry.Validate();
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_Manual_RequiredFields()
    {
        var entry = new BibEntry(EntryType.Manual)
        {
            Key = "test2025",
            Title = "Test Manual"
        };

        var result = entry.Validate();
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_Proceedings_RequiredFields()
    {
        var entry = new BibEntry(EntryType.Proceedings)
        {
            Key = "test2025",
            Title = "Test Proceedings",
            Year = 2025
        };

        var result = entry.Validate();
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_Unpublished_RequiredFields()
    {
        var entry = new BibEntry(EntryType.Unpublished)
        {
            Key = "test2025",
            Title = "Test Manuscript",
            Note = "Unpublished manuscript"
        };
        entry.AddAuthor(new Author("Smith", "John"));

        var result = entry.Validate();
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_Misc_NoRequiredFields()
    {
        var entry = new BibEntry(EntryType.Misc)
        {
            Key = "test2025"
        };

        var result = entry.Validate();
        Assert.True(result.IsValid);
    }

    [Fact]
    public void RegisterCustomType_WithRequiredFields()
    {
        BibEntry.RegisterCustomType("customtype", new[] { "author", "title" }, new[] { "year" });
        
        var entry = new BibEntry("customtype")
        {
            Key = "test2025",
            Title = "Test"
        };
        entry.AddAuthor(new Author("Smith", "John"));

        var result = entry.Validate();
        Assert.True(result.IsValid);
    }

    [Fact]
    public void RegisterCustomType_MissingRequiredField()
    {
        BibEntry.RegisterCustomType("customtype2", new[] { "author", "title", "custom" }, new[] { "year" });
        
        var entry = new BibEntry("customtype2")
        {
            Key = "test2025",
            Title = "Test"
        };
        entry.AddAuthor(new Author("Smith", "John"));

        var result = entry.Validate();
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("custom"));
    }

    [Fact]
    public void RegisterCustomType_NullEntryType_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => 
            BibEntry.RegisterCustomType(null!, new[] { "author" }, new[] { "year" }));
    }

    [Fact]
    public void RegisterCustomType_EmptyEntryType_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => 
            BibEntry.RegisterCustomType("", new[] { "author" }, new[] { "year" }));
    }

    [Fact]
    public void Validate_UnknownEntryType_GivesWarning()
    {
        var entry = new BibEntry("unknowntype")
        {
            Key = "test2025",
            Title = "Test"
        };

        var result = entry.Validate();
        Assert.False(result.HasErrors);
        Assert.True(result.HasWarnings);
        Assert.Contains(result.Warnings, w => w.Contains("unknowntype"));
    }

    [Fact]
    public void Validate_EmptyKey_GivesError()
    {
        var entry = new BibEntry(EntryType.Misc)
        {
            Key = ""
        };

        var result = entry.Validate();
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("key"));
    }

    [Fact]
    public void Validate_SuspiciousYear_GivesWarning()
    {
        var entry = new BibEntry(EntryType.Misc)
        {
            Key = "test",
            Year = 999
        };

        var result = entry.Validate();
        Assert.True(result.HasWarnings);
        Assert.Contains(result.Warnings, w => w.Contains("999"));
    }

    [Fact]
    public void Validate_FutureYear_GivesWarning()
    {
        var entry = new BibEntry(EntryType.Misc)
        {
            Key = "test",
            Year = 3001
        };

        var result = entry.Validate();
        Assert.True(result.HasWarnings);
        Assert.Contains(result.Warnings, w => w.Contains("3001"));
    }

    [Fact]
    public void Validate_NegativeVolume_GivesWarning()
    {
        var entry = new BibEntry(EntryType.Misc)
        {
            Key = "test",
            Volume = -1
        };

        var result = entry.Validate();
        Assert.True(result.HasWarnings);
        Assert.Contains(result.Warnings, w => w.Contains("Volume"));
    }

    [Fact]
    public void Author_WithEmptyLastName_ThrowsException()
    {
        // Empty last name should throw ArgumentException
        Assert.Throws<ArgumentException>(() => new Author(""));
    }
    
    [Fact]
    public void Author_WithWhitespaceLastName_ThrowsException()
    {
        // Whitespace last name should throw ArgumentException
        Assert.Throws<ArgumentException>(() => new Author("   "));
    }

    [Fact]
    public void Article_WithOptionalFields()
    {
        var entry = new BibEntry(EntryType.Article)
        {
            Key = "test2025",
            Title = "Test Article",
            Journal = "Test Journal",
            Year = 2025,
            Volume = 10,
            Number = "5",
            Doi = "10.1234/test",
            Note = "Test note",
            Url = "https://example.com",
            Issn = "1234-5678",
            Keywords = "test, example",
            Abstract = "This is a test abstract",
            Language = "en",
            Copyright = "© 2025"
        };
        entry.AddAuthor(new Author("Smith", "John"));
        entry.Pages = new PageRange(100, 110);

        var result = entry.Validate();
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Book_WithAllOptionalFields()
    {
        var entry = new BibEntry(EntryType.Book)
        {
            Key = "test2025",
            Title = "Test Book",
            Publisher = "Test Publisher",
            Year = 2025,
            Volume = 1,
            Number = "2",
            Series = "Test Series",
            Address = "New York",
            Edition = "1st",
            Isbn = "978-0-123456-78-9",
            Doi = "10.1234/book",
            Note = "Test note",
            Url = "https://example.com",
            Language = "en",
            Keywords = "book, test",
            Abstract = "Book abstract",
            Copyright = "© 2025"
        };
        entry.AddAuthor(new Author("Smith", "John"));
        entry.AddEditor(new Author("Doe", "Jane"));

        var result = entry.Validate();
        Assert.True(result.IsValid);
    }
}

