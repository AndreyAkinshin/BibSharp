using BibSharp.Models;

namespace BibSharp.Tests;

public class BibEntryAdditionalTests
{
    [Fact]
    public void Constructor_DefaultType_IsMisc()
    {
        var entry = new BibEntry();
        Assert.Equal("misc", entry.EntryType.Value);
    }

    [Fact]
    public void Constructor_WithType_SetsType()
    {
        var entry = new BibEntry(EntryType.Article);
        Assert.Equal("article", entry.EntryType.Value);
    }

    [Fact]
    public void SetField_NullOrEmptyName_ThrowsException()
    {
        var entry = new BibEntry();
        Assert.Throws<ArgumentException>(() => entry.SetField(null!, "value"));
        Assert.Throws<ArgumentException>(() => entry.SetField("", "value"));
    }

    [Fact]
    public void SetField_NullValue_RemovesField()
    {
        var entry = new BibEntry { Key = "test" };
        entry.Title = "Test";
        entry.Title = null;

        Assert.Null(entry.Title);
        Assert.False(entry.HasField("title"));
    }

    [Fact]
    public void SetField_EmptyValue_RemovesField()
    {
        var entry = new BibEntry { Key = "test" };
        entry.Title = "Test";
        entry.SetField("title", "");

        Assert.Null(entry.Title);
        Assert.False(entry.HasField("title"));
    }

    [Fact]
    public void RemoveField_NullOrEmpty_ReturnsFalse()
    {
        var entry = new BibEntry { Key = "test" };
        Assert.False(entry.RemoveField(null!));
        Assert.False(entry.RemoveField(""));
    }

    [Fact]
    public void RemoveField_NonExistentField_ReturnsFalse()
    {
        var entry = new BibEntry { Key = "test" };
        Assert.False(entry.RemoveField("nonexistent"));
    }

    [Fact]
    public void RemoveField_ExistingField_ReturnsTrue()
    {
        var entry = new BibEntry { Key = "test", Title = "Test" };
        Assert.True(entry.RemoveField("title"));
        Assert.Null(entry.Title);
    }

    [Fact]
    public void RemoveField_Author_ClearsAuthors()
    {
        var entry = new BibEntry { Key = "test" };
        entry.AddAuthor(new Author("Smith", "John"));

        Assert.True(entry.RemoveField("author"));
        Assert.Empty(entry.Authors);
    }

    [Fact]
    public void RemoveField_Editor_ClearsEditors()
    {
        var entry = new BibEntry { Key = "test" };
        entry.AddEditor(new Author("Doe", "Jane"));

        Assert.True(entry.RemoveField("editor"));
        Assert.Empty(entry.Editors);
    }

    [Fact]
    public void AddAuthor_NullAuthor_ThrowsException()
    {
        var entry = new BibEntry();
        Assert.Throws<ArgumentNullException>(() => entry.AddAuthor(null!));
    }

    [Fact]
    public void AddEditor_NullEditor_ThrowsException()
    {
        var entry = new BibEntry();
        Assert.Throws<ArgumentNullException>(() => entry.AddEditor(null!));
    }

    [Fact]
    public void RemoveAuthor_NullAuthor_ReturnsFalse()
    {
        var entry = new BibEntry();
        Assert.False(entry.RemoveAuthor(null));
    }

    [Fact]
    public void RemoveEditor_NullEditor_ReturnsFalse()
    {
        var entry = new BibEntry();
        Assert.False(entry.RemoveEditor(null));
    }

    [Fact]
    public void RemoveAuthor_ExistingAuthor_ReturnsTrue()
    {
        var entry = new BibEntry();
        var author = new Author("Smith", "John");
        entry.AddAuthor(author);

        Assert.True(entry.RemoveAuthor(author));
        Assert.Empty(entry.Authors);
    }

    [Fact]
    public void RemoveEditor_ExistingEditor_ReturnsTrue()
    {
        var entry = new BibEntry();
        var editor = new Author("Doe", "Jane");
        entry.AddEditor(editor);

        Assert.True(entry.RemoveEditor(editor));
        Assert.Empty(entry.Editors);
    }

    [Fact]
    public void AddAuthor_ReturnsThisForChaining()
    {
        var entry = new BibEntry();
        var result = entry.AddAuthor(new Author("Smith", "John"));

        Assert.Same(entry, result);
    }

    [Fact]
    public void AddEditor_ReturnsThisForChaining()
    {
        var entry = new BibEntry();
        var result = entry.AddEditor(new Author("Doe", "Jane"));

        Assert.Same(entry, result);
    }

    [Fact]
    public void Year_WithBraces_ParsesCorrectly()
    {
        var entry = new BibEntry();
        entry.SetField("year", "{2025}");

        Assert.Equal(2025, entry.Year);
    }

    [Fact]
    public void Year_NonNumeric_ReturnsNull()
    {
        var entry = new BibEntry();
        entry.SetField("year", "not a year");

        Assert.Null(entry.Year);
    }

    [Fact]
    public void Volume_WithBraces_ParsesCorrectly()
    {
        var entry = new BibEntry();
        entry.SetField("volume", "{10}");

        Assert.Equal(10, entry.Volume);
    }

    [Fact]
    public void Volume_NonNumeric_ReturnsNull()
    {
        var entry = new BibEntry();
        entry.SetField("volume", "not a number");

        Assert.Null(entry.Volume);
    }

    [Fact]
    public void Pages_InvalidFormat_ReturnsNull()
    {
        var entry = new BibEntry();
        entry.SetField("pages", "invalid");

        Assert.Null(entry.Pages);
    }

    [Fact]
    public void Pages_ValidRange_Parses()
    {
        var entry = new BibEntry();
        entry.SetField("pages", "100--110");

        Assert.NotNull(entry.Pages);
        Assert.Equal(100, entry.Pages.StartPage);
        Assert.Equal(110, entry.Pages.EndPage);
    }

    [Fact]
    public void Indexer_GetField()
    {
        var entry = new BibEntry { Key = "test", Title = "Test Title" };
        Assert.Equal("Test Title", entry["title"]);
    }

    [Fact]
    public void Indexer_SetField()
    {
        var entry = new BibEntry { Key = "test" };
        entry["custom"] = "Custom Value";

        Assert.Equal("Custom Value", entry["custom"]);
    }

    [Fact]
    public void AllStandardFields_CanBeSetAndGet()
    {
        var entry = new BibEntry(EntryType.Misc) { Key = "test" };

        entry.Title = "Title";
        entry.Journal = "Journal";
        entry.BookTitle = "BookTitle";
        entry.Publisher = "Publisher";
        entry.Year = 2025;
        entry.Volume = 10;
        entry.Number = "5";
        entry.Doi = "10.1234/test";
        entry.Url = "https://example.com";
        entry.Note = "Note";
        entry.Edition = "1st";
        entry.Series = "Series";
        entry.Chapter = "1";
        entry.Address = "Address";
        entry.HowPublished = "Online";
        entry.Isbn = "1234567890";
        entry.Issn = "1234-5678";
        entry.School = "School"; // School is now independent from Institution
        entry.Institution = "School"; // Set Institution separately
        entry.Organization = "Organization";
        entry.Type = "Type";
        entry.Language = "en";
        entry.Keywords = "keywords";
        entry.Abstract = "Abstract";
        entry.Annote = "Annotation";
        entry.Copyright = "Copyright";
        entry.Arxiv = "1234.5678";

        Assert.Equal("Title", entry.Title);
        Assert.Equal("Journal", entry.Journal);
        Assert.Equal("BookTitle", entry.BookTitle);
        Assert.Equal("Publisher", entry.Publisher);
        Assert.Equal(2025, entry.Year);
        Assert.Equal(10, entry.Volume);
        Assert.Equal("5", entry.Number);
        Assert.Equal("10.1234/test", entry.Doi);
        Assert.Equal("https://example.com", entry.Url);
        Assert.Equal("Note", entry.Note);
        Assert.Equal("1st", entry.Edition);
        Assert.Equal("Series", entry.Series);
        Assert.Equal("1", entry.Chapter);
        Assert.Equal("Address", entry.Address);
        Assert.Equal("Online", entry.HowPublished);
        Assert.Equal("1234567890", entry.Isbn);
        Assert.Equal("1234-5678", entry.Issn);
        Assert.Equal("School", entry.School); // School is now independent
        Assert.Equal("School", entry.Institution); // Institution was set separately
        Assert.Equal("Organization", entry.Organization);
        Assert.Equal("Type", entry.Type);
        Assert.Equal("en", entry.Language);
        Assert.Equal("keywords", entry.Keywords);
        Assert.Equal("Abstract", entry.Abstract);
        Assert.Equal("Annotation", entry.Annote);
        Assert.Equal("Copyright", entry.Copyright);
        Assert.Equal("1234.5678", entry.Arxiv);
    }

    [Fact]
    public void Month_SetNull_RemovesField()
    {
        var entry = new BibEntry { Key = "test" };
        entry.Month = BibSharp.Month.January;
        entry.Month = null;

        Assert.Null(entry.Month);
    }

    [Fact]
    public void Month_ParsesFromField()
    {
        var entry = new BibEntry { Key = "test" };
        entry.SetField("month", "dec");

        Assert.Equal(12, entry.Month?.Number);
    }

    [Fact]
    public void Pages_SetViaProperty()
    {
        var entry = new BibEntry { Key = "test" };
        entry.Pages = new PageRange(50, 60);

        Assert.Equal(50, entry.Pages.StartPage);
        Assert.Equal(60, entry.Pages.EndPage);
    }

    [Fact]
    public void Pages_SetNull_RemovesField()
    {
        var entry = new BibEntry { Key = "test" };
        entry.Pages = new PageRange(50, 60);
        entry.Pages = null;

        Assert.Null(entry.Pages);
    }

    [Fact]
    public void GenerateKey_WithoutExistingKeys()
    {
        var entry = new BibEntry(EntryType.Article)
        {
            Key = "oldkey",
            Title = "Test",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John"));

        var key = entry.GenerateKey();

        Assert.NotEmpty(key);
        Assert.Equal(key, entry.Key); // Should automatically set
    }

    [Fact]
    public void GenerateKey_WithoutAutomaticSet()
    {
        var entry = new BibEntry(EntryType.Article)
        {
            Key = "oldkey",
            Title = "Test",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John"));

        var key = entry.GenerateKey(setKeyAutomatically: false);

        Assert.NotEmpty(key);
        Assert.Equal("oldkey", entry.Key); // Should NOT automatically set
    }

    [Fact]
    public void GenerateKey_WithExistingKeys_AvoidsCollisions()
    {
        var entry = new BibEntry(EntryType.Article)
        {
            Title = "Test",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John"));

        var existingKeys = new[] { "smith2025" };
        var key = entry.GenerateKey(existingKeys: existingKeys);

        Assert.NotEqual("smith2025", key);
        Assert.Contains("smith2025", key); // Should have suffix like 'a' or number
    }
}

