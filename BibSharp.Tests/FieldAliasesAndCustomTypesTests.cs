using BibSharp.Models;

namespace BibSharp.Tests;

public class FieldAliasesAndCustomTypesTests
{
    [Fact]
    public void FieldAlias_JournalTitle_MapsToJournal()
    {
        var entry = new BibEntry(EntryType.Article)
        {
            Key = "test"
        };
        entry["journaltitle"] = "Test Journal";

        Assert.Equal("Test Journal", entry.Journal);
        Assert.Equal("Test Journal", entry["journal"]);
    }

    [Fact]
    public void FieldAlias_JournalTitleHyphen_MapsToJournal()
    {
        var entry = new BibEntry(EntryType.Article)
        {
            Key = "test"
        };
        entry["journal-title"] = "Test Journal";

        Assert.Equal("Test Journal", entry.Journal);
    }

    [Fact]
    public void FieldAlias_Book_MapsToBookTitle()
    {
        var entry = new BibEntry(EntryType.InProceedings)
        {
            Key = "test"
        };
        entry["book"] = "Proceedings Title";

        Assert.Equal("Proceedings Title", entry.BookTitle);
    }

    [Fact]
    public void FieldAlias_School_IsIndependent()
    {
        var entry = new BibEntry(EntryType.PhdThesis)
        {
            Key = "test"
        };
        entry["school"] = "University";

        // School should be its own field, not mapped to Institution
        Assert.Equal("University", entry.School);
        Assert.Null(entry.Institution);
    }

    [Fact]
    public void FieldAlias_University_MapsToInstitution()
    {
        var entry = new BibEntry(EntryType.TechReport)
        {
            Key = "test"
        };
        entry["university"] = "MIT";

        Assert.Equal("MIT", entry.Institution);
    }

    [Fact]
    public void FieldAlias_Issue_MapsToNumber()
    {
        var entry = new BibEntry(EntryType.Article)
        {
            Key = "test"
        };
        entry["issue"] = "5";

        Assert.Equal("5", entry.Number);
    }

    [Fact]
    public void FieldAlias_Location_MapsToAddress()
    {
        var entry = new BibEntry(EntryType.Book)
        {
            Key = "test"
        };
        entry["location"] = "New York";

        Assert.Equal("New York", entry.Address);
    }

    [Fact]
    public void FieldAlias_Venue_MapsToAddress()
    {
        var entry = new BibEntry(EntryType.InProceedings)
        {
            Key = "test"
        };
        entry["venue"] = "Conference Center";

        Assert.Equal("Conference Center", entry.Address);
    }

    [Fact]
    public void FieldAlias_Isbn10_MapsToIsbn()
    {
        var entry = new BibEntry(EntryType.Book)
        {
            Key = "test"
        };
        entry["isbn-10"] = "1234567890";

        Assert.Equal("1234567890", entry.Isbn);
    }

    [Fact]
    public void FieldAlias_Isbn13_MapsToIsbn()
    {
        var entry = new BibEntry(EntryType.Book)
        {
            Key = "test"
        };
        entry["isbn-13"] = "978-1234567890";

        Assert.Equal("978-1234567890", entry.Isbn);
    }

    [Fact]
    public void FieldAlias_ElectronicIssn_MapsToIssn()
    {
        var entry = new BibEntry(EntryType.Article)
        {
            Key = "test"
        };
        entry["electronic-issn"] = "1234-5678";

        Assert.Equal("1234-5678", entry.Issn);
    }

    [Fact]
    public void FieldAlias_Link_MapsToUrl()
    {
        var entry = new BibEntry(EntryType.Misc)
        {
            Key = "test"
        };
        entry["link"] = "https://example.com";

        Assert.Equal("https://example.com", entry.Url);
    }

    [Fact]
    public void FieldAlias_Web_MapsToUrl()
    {
        var entry = new BibEntry(EntryType.Misc)
        {
            Key = "test"
        };
        entry["web"] = "https://example.com";

        Assert.Equal("https://example.com", entry.Url);
    }

    [Fact]
    public void FieldAlias_DoiUrl_MapsToDoi()
    {
        var entry = new BibEntry(EntryType.Article)
        {
            Key = "test"
        };
        entry["doi-url"] = "10.1234/test";

        Assert.Equal("10.1234/test", entry.Doi);
    }

    [Fact]
    public void FieldAlias_Keyword_MapsToKeywords()
    {
        var entry = new BibEntry(EntryType.Misc)
        {
            Key = "test"
        };
        entry["keyword"] = "test, example";

        Assert.Equal("test, example", entry.Keywords);
    }

    [Fact]
    public void FieldAlias_Tags_MapsToKeywords()
    {
        var entry = new BibEntry(EntryType.Misc)
        {
            Key = "test"
        };
        entry["tags"] = "test, example";

        Assert.Equal("test, example", entry.Keywords);
    }

    [Fact]
    public void FieldAlias_Date_MapsToYear()
    {
        var entry = new BibEntry(EntryType.Misc)
        {
            Key = "test"
        };
        entry["date"] = "2025";

        Assert.Equal(2025, entry.Year);
    }

    [Fact]
    public void FieldAlias_Published_MapsToYear()
    {
        var entry = new BibEntry(EntryType.Misc)
        {
            Key = "test"
        };
        entry["published"] = "2025";

        Assert.Equal(2025, entry.Year);
    }

    [Fact]
    public void FieldAlias_Day_DoesNotMapToMonth()
    {
        // "day" should NOT map to "month" as they are semantically different
        // (day = day of month, month = the month itself)
        var entry = new BibEntry(EntryType.Misc)
        {
            Key = "test"
        };
        entry["day"] = "15";

        // "day" is stored as a custom field, not mapped to month
        Assert.Null(entry.Month);
        Assert.Equal("15", entry["day"]);
    }

    [Fact]
    public void FieldAlias_Urldate_MapsToNote()
    {
        var entry = new BibEntry(EntryType.Misc)
        {
            Key = "test"
        };
        entry["urldate"] = "2025-01-01";

        Assert.Equal("2025-01-01", entry.Note);
    }

    [Fact]
    public void FieldAlias_Pubstate_MapsToNote()
    {
        var entry = new BibEntry(EntryType.Misc)
        {
            Key = "test"
        };
        entry["pubstate"] = "in press";

        Assert.Equal("in press", entry.Note);
    }

    [Fact]
    public void RegisterFieldAlias_CustomAlias()
    {
        BibEntry.RegisterFieldAlias("customfield", "title");
        
        var entry = new BibEntry(EntryType.Misc)
        {
            Key = "test"
        };
        entry["customfield"] = "Custom Title";

        Assert.Equal("Custom Title", entry.Title);
    }

    [Fact]
    public void RegisterFieldAlias_NullAlias_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => 
            BibEntry.RegisterFieldAlias(null!, "title"));
    }

    [Fact]
    public void RegisterFieldAlias_EmptyAlias_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => 
            BibEntry.RegisterFieldAlias("", "title"));
    }

    [Fact]
    public void RegisterFieldAlias_NullTarget_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => 
            BibEntry.RegisterFieldAlias("alias", null!));
    }

    [Fact]
    public void RegisterFieldAlias_EmptyTarget_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => 
            BibEntry.RegisterFieldAlias("alias", ""));
    }

    [Fact]
    public void HasField_WithAlias_ReturnsTrue()
    {
        var entry = new BibEntry(EntryType.Article)
        {
            Key = "test"
        };
        entry["journaltitle"] = "Test Journal";

        Assert.True(entry.HasField("journal"));
        Assert.True(entry.HasField("journaltitle"));
    }

    [Fact]
    public void GetField_WithAlias_ReturnsValue()
    {
        var entry = new BibEntry(EntryType.Article)
        {
            Key = "test"
        };
        entry.SetField("journaltitle", "Test Journal");

        Assert.Equal("Test Journal", entry.GetField("journal"));
        Assert.Equal("Test Journal", entry.GetField("journaltitle"));
    }

    [Fact]
    public void RemoveField_WithAlias_RemovesField()
    {
        var entry = new BibEntry(EntryType.Article)
        {
            Key = "test"
        };
        entry["journaltitle"] = "Test Journal";

        Assert.True(entry.RemoveField("journaltitle"));
        Assert.Null(entry.Journal);
        Assert.Null(entry["journaltitle"]);
    }

    [Fact]
    public void RegisterCustomType_OverridesExistingType()
    {
        // Register a custom type name that doesn't conflict with standard types
        BibEntry.RegisterCustomType("customarticle", new[] { "title", "custom" }, new[] { "year" });
        
        var entry = new BibEntry("customarticle")
        {
            Key = "test",
            Title = "Test"
        };
        entry["custom"] = "Custom Value";

        var result = entry.Validate();
        // Should validate based on new requirements
        Assert.True(result.IsValid);
    }

    [Fact]
    public void RegisterCustomType_WithNullRequiredFields()
    {
        BibEntry.RegisterCustomType("nulltest", null!, new[] { "year" });
        
        var entry = new BibEntry("nulltest")
        {
            Key = "test"
        };

        // Should validate successfully with no required fields
        var result = entry.Validate();
        Assert.True(result.IsValid);
    }

    [Fact]
    public void RegisterCustomType_WithNullOptionalFields()
    {
        BibEntry.RegisterCustomType("nulltest2", new[] { "title" }, null!);
        
        var entry = new BibEntry("nulltest2")
        {
            Key = "test",
            Title = "Test"
        };

        var result = entry.Validate();
        Assert.True(result.IsValid);
    }

    [Fact]
    public void FieldAlias_CaseInsensitive()
    {
        var entry = new BibEntry(EntryType.Article)
        {
            Key = "test"
        };
        entry["JOURNALTITLE"] = "Test Journal";

        Assert.Equal("Test Journal", entry.Journal);
        Assert.Equal("Test Journal", entry["JournalTitle"]);
    }

    [Fact]
    public void CustomType_CaseInsensitive()
    {
        BibEntry.RegisterCustomType("CustomType", new[] { "title" }, new[] { "year" });
        
        var entry = new BibEntry("customtype")
        {
            Key = "test",
            Title = "Test"
        };

        var result = entry.Validate();
        Assert.True(result.IsValid);
    }
}

