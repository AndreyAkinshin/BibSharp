using BibSharp.Models;
using BibSharp.Utilities;

namespace BibSharp.Tests;

public class AdditionalEdgeCaseTests
{
    [Fact]
    public void BibEntry_Year_NullString_ReturnsNull()
    {
        var entry = new BibEntry { Key = "test" };
        entry.SetField("year", null);
        
        Assert.Null(entry.Year);
    }

    [Fact]
    public void BibEntry_Volume_NullString_ReturnsNull()
    {
        var entry = new BibEntry { Key = "test" };
        entry.SetField("volume", null);
        
        Assert.Null(entry.Volume);
    }

    [Fact]
    public void BibEntry_Pages_NullString_ReturnsNull()
    {
        var entry = new BibEntry { Key = "test" };
        entry.SetField("pages", null);
        
        Assert.Null(entry.Pages);
    }

    [Fact]
    public void BibEntry_Pages_EmptyString_ReturnsNull()
    {
        var entry = new BibEntry { Key = "test" };
        entry.SetField("pages", "");
        
        Assert.Null(entry.Pages);
    }

    [Fact]
    public void BibEntry_GetField_NonExistentField_ReturnsNull()
    {
        var entry = new BibEntry { Key = "test" };
        
        Assert.Null(entry.GetField("nonexistent"));
    }

    [Fact]
    public void BibEntry_HasField_NonExistentField_ReturnsFalse()
    {
        var entry = new BibEntry { Key = "test" };
        
        Assert.False(entry.HasField("nonexistent"));
    }

    [Fact]
    public void BibEntry_HasField_ExistingField_ReturnsTrue()
    {
        var entry = new BibEntry { Key = "test", Title = "Test" };
        
        Assert.True(entry.HasField("title"));
    }

    [Fact]
    public void BibEntry_GetFields_ReturnsAllFields()
    {
        var entry = new BibEntry { Key = "test", Title = "Test", Year = 2025 };
        
        var fields = entry.GetFields();
        
        Assert.Contains("title", fields.Keys);
        Assert.Contains("year", fields.Keys);
    }

    [Fact]
    public void BibEntry_GetFieldNames_PreservesOrder()
    {
        var entry = new BibEntry { Key = "test" };
        entry.Year = 2025;
        entry.Title = "Test";
        entry.Journal = "J";
        
        var fieldNames = entry.GetFieldNames().ToList();
        
        Assert.Equal("year", fieldNames[0]);
        Assert.Equal("title", fieldNames[1]);
        Assert.Equal("journal", fieldNames[2]);
    }

    [Fact]
    public void BibEntry_SetField_UpdatesExistingField()
    {
        var entry = new BibEntry { Key = "test", Title = "Original" };
        entry.SetField("title", "Updated");
        
        Assert.Equal("Updated", entry.Title);
    }

    [Fact]
    public void BibEntry_Indexer_SetNull_RemovesField()
    {
        var entry = new BibEntry { Key = "test" };
        entry["custom"] = "value";
        entry["custom"] = null;
        
        Assert.Null(entry["custom"]);
    }

    [Fact]
    public void BibEntry_AddAuthor_UpdatesField()
    {
        var entry = new BibEntry { Key = "test" };
        entry.AddAuthor(new Author("Smith", "John"));
        
        Assert.NotNull(entry["author"]);
        Assert.Contains("Smith", entry["author"]);
    }

    [Fact]
    public void BibEntry_AddEditor_UpdatesField()
    {
        var entry = new BibEntry { Key = "test" };
        entry.AddEditor(new Author("Doe", "Jane"));
        
        Assert.NotNull(entry["editor"]);
        Assert.Contains("Doe", entry["editor"]);
    }

    [Fact]
    public void BibEntry_RemoveAuthor_UpdatesField()
    {
        var entry = new BibEntry { Key = "test" };
        var author = new Author("Smith", "John");
        entry.AddAuthor(author);
        entry.RemoveAuthor(author);
        
        Assert.Null(entry["author"]);
    }

    [Fact]
    public void BibEntry_RemoveEditor_UpdatesField()
    {
        var entry = new BibEntry { Key = "test" };
        var editor = new Author("Doe", "Jane");
        entry.AddEditor(editor);
        entry.RemoveEditor(editor);
        
        Assert.Null(entry["editor"]);
    }

    [Fact]
    public void BibEntry_SetAuthors_ClearsList()
    {
        var entry = new BibEntry { Key = "test" };
        entry.AddAuthor(new Author("Smith", "John"));
        entry.SetField("author", null);
        
        Assert.Empty(entry.Authors);
    }

    [Fact]
    public void BibEntry_SetEditors_ClearsList()
    {
        var entry = new BibEntry { Key = "test" };
        entry.AddEditor(new Author("Doe", "Jane"));
        entry.SetField("editor", null);
        
        Assert.Empty(entry.Editors);
    }

    [Fact]
    public void BibEntry_SetAuthors_ParsesString()
    {
        var entry = new BibEntry { Key = "test" };
        entry.SetField("author", "Smith, John and Doe, Jane");
        
        Assert.Equal(2, entry.Authors.Count);
        Assert.Equal("Smith", entry.Authors[0].LastName);
        Assert.Equal("Doe", entry.Authors[1].LastName);
    }

    [Fact]
    public void BibEntry_SetEditors_ParsesString()
    {
        var entry = new BibEntry { Key = "test" };
        entry.SetField("editor", "Smith, John and Doe, Jane");
        
        Assert.Equal(2, entry.Editors.Count);
    }

    [Fact]
    public void Author_FromFormattedString_WithWhitespace()
    {
        var author = Author.FromFormattedString("  Smith  ,  John  ");
        
        Assert.Equal("Smith", author.LastName);
        Assert.Equal("John", author.FirstName);
    }

    [Fact]
    public void Author_FromFormattedString_CommaNoSpace()
    {
        var author = Author.FromFormattedString("Smith,John");
        
        Assert.Equal("Smith", author.LastName);
        Assert.Equal("John", author.FirstName);
    }

    [Fact]
    public void Author_FromFormattedString_MultipleSpaces()
    {
        var author = Author.FromFormattedString("Smith,  John   Andrew");
        
        Assert.Equal("Smith", author.LastName);
        Assert.Equal("John", author.FirstName);
        Assert.Equal("Andrew", author.MiddleName);
    }
}

