namespace BibSharp.Tests;

public class BibEntryExtendedFieldsTests
{
    [Fact]
    public void ExtendedFields_AreProperlyAccessible()
    {
        var entry = new BibEntry("book")
        {
            Key = "extendedTest",
            Title = "Book with Extended Fields",
            Year = 2025,
            Publisher = "Extended Publishing",
            Edition = "2nd",
            Series = "Extended Series",
            Chapter = "3",
            Address = "New York, NY",
            Isbn = "978-1-23456-789-0",
            Language = "English",
            Abstract = "This is a test abstract for a book with extended fields.",
            Copyright = "© 2025 Extended Publishing"
        };
        
        // Test that all fields are retrievable
        Assert.Equal("2nd", entry.Edition);
        Assert.Equal("Extended Series", entry.Series);
        Assert.Equal("3", entry.Chapter);
        Assert.Equal("New York, NY", entry.Address);
        Assert.Equal("978-1-23456-789-0", entry.Isbn);
        Assert.Equal("English", entry.Language);
        Assert.Equal("This is a test abstract for a book with extended fields.", entry.Abstract);
        Assert.Equal("© 2025 Extended Publishing", entry.Copyright);
        
        // Test indexer access
        Assert.Equal("2nd", entry["edition"]);
        Assert.Equal("Extended Series", entry["series"]);
        
        // Test case insensitivity
        Assert.Equal("2nd", entry["EDITION"]);
        Assert.Equal("Extended Series", entry["Series"]);
    }
    
    [Fact]
    public void Keywords_CanBeAddedAndRetrieved()
    {
        var entry = new BibEntry()
        {
            Key = "keywordTest",
            Title = "Test with Keywords"
        };
        
        // Add keywords as a comma-separated string
        entry.Keywords = "keyword1, keyword2, keyword3";
        
        // Retrieve as list
        var keywordList = entry.GetKeywordList();
        Assert.Equal(3, keywordList.Count);
        Assert.Contains("keyword1", keywordList);
        Assert.Contains("keyword2", keywordList);
        Assert.Contains("keyword3", keywordList);
        
        // Add a new keyword
        entry.AddKeyword("keyword4");
        keywordList = entry.GetKeywordList();
        Assert.Equal(4, keywordList.Count);
        Assert.Contains("keyword4", keywordList);
        
        // Set from list
        entry.SetKeywordList(new[] { "tag1", "tag2", "tag3" });
        Assert.Equal("tag1, tag2, tag3", entry.Keywords);
        
        // Test with semicolons
        entry.Keywords = "semi1; semi2; semi3";
        keywordList = entry.GetKeywordList();
        Assert.Equal(3, keywordList.Count);
        Assert.Contains("semi1", keywordList);
        Assert.Contains("semi2", keywordList);
        Assert.Contains("semi3", keywordList);
        
        // Test with braces
        entry.Keywords = "{keyword in braces}";
        keywordList = entry.GetKeywordList();
        Assert.Single(keywordList);
        Assert.Contains("keyword in braces", keywordList);
    }
    
    [Fact]
    public void Month_CanBeParsedProperly()
    {
        var entry = new BibEntry();
        
        // Test setting month struct
        entry.Month = BibSharp.Month.January;
        Assert.Equal(1, entry.Month?.Number);
        
        entry.Month = BibSharp.Month.February;
        Assert.Equal(2, entry.Month?.Number);
        
        entry.Month = BibSharp.Month.March;
        Assert.Equal(3, entry.Month?.Number);
        
        // Test parsing from raw field (parser would set this)
        entry.SetField("month", "apr");
        Assert.Equal(4, entry.Month?.Number);
        
        entry.SetField("month", "june");
        Assert.Equal(6, entry.Month?.Number);
    }
    
    [Fact]
    public void FieldAliases_WorkProperly()
    {
        var entry = new BibEntry();
        
        // Test journal aliases
        entry["journaltitle"] = "Test Journal";
        Assert.Equal("Test Journal", entry.Journal);
        Assert.Equal("Test Journal", entry["journal"]);
        Assert.Equal("Test Journal", entry["journaltitle"]);
        
        // Test ISBN aliases
        entry["isbn-13"] = "978-1-23456-789-0";
        Assert.Equal("978-1-23456-789-0", entry.Isbn);
        Assert.Equal("978-1-23456-789-0", entry["isbn"]);
        
        // Test URL aliases
        entry["link"] = "https://example.com";
        Assert.Equal("https://example.com", entry.Url);
        Assert.Equal("https://example.com", entry["url"]);
        
        // Test keyword aliases
        entry["tags"] = "tag1, tag2";
        Assert.Equal("tag1, tag2", entry.Keywords);
        Assert.Equal("tag1, tag2", entry["keywords"]);
        
        // Test date alias
        entry["date"] = "2025";
        Assert.Equal(2025, entry.Year);
    }
}