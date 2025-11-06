using BibSharp.Models;

namespace BibSharp.Tests;

public class BibEntryComprehensiveTests
{
    [Fact]
    public void Clone_CreatesDeepCopy()
    {
        var original = new BibEntry(EntryType.Article)
        {
            Key = "test2025",
            Title = "Original Title",
            Year = 2025,
            Volume = 10
        };
        original.AddAuthor(new Author("Smith", "John"));
        original.AddEditor(new Author("Doe", "Jane"));

        var clone = original.Clone();

        Assert.NotSame(original, clone);
        Assert.Equal(original.Key, clone.Key);
        Assert.Equal(original.Title, clone.Title);
        Assert.Equal(original.Year, clone.Year);
        Assert.Equal(original.Volume, clone.Volume);
        Assert.Equal(original.Authors.Count, clone.Authors.Count);
        Assert.Equal(original.Editors.Count, clone.Editors.Count);
    }

    [Fact]
    public void Clone_ModifyingClone_DoesNotAffectOriginal()
    {
        var original = new BibEntry(EntryType.Article)
        {
            Key = "test2025",
            Title = "Original Title"
        };
        original.AddAuthor(new Author("Smith", "John"));

        var clone = original.Clone();
        clone.Title = "Modified Title";
        clone.Key = "modified2025";
        clone.AddAuthor(new Author("Doe", "Jane"));

        Assert.Equal("Original Title", original.Title);
        Assert.Equal("test2025", original.Key);
        Assert.Single(original.Authors);
    }

    [Fact]
    public void GetKeywordList_Empty_ReturnsEmpty()
    {
        var entry = new BibEntry(EntryType.Misc) { Key = "test" };
        var keywords = entry.GetKeywordList();
        Assert.Empty(keywords);
    }

    [Fact]
    public void GetKeywordList_CommaSeparated()
    {
        var entry = new BibEntry(EntryType.Misc) { Key = "test" };
        entry.Keywords = "machine learning, neural networks, AI";

        var keywords = entry.GetKeywordList();
        Assert.Equal(3, keywords.Count);
        Assert.Contains("machine learning", keywords);
        Assert.Contains("neural networks", keywords);
        Assert.Contains("AI", keywords);
    }

    [Fact]
    public void GetKeywordList_SemicolonSeparated()
    {
        var entry = new BibEntry(EntryType.Misc) { Key = "test" };
        entry.Keywords = "keyword1; keyword2; keyword3";

        var keywords = entry.GetKeywordList();
        Assert.Equal(3, keywords.Count);
        Assert.Contains("keyword1", keywords);
        Assert.Contains("keyword2", keywords);
        Assert.Contains("keyword3", keywords);
    }

    [Fact]
    public void GetKeywordList_WhitespaceSeparated()
    {
        var entry = new BibEntry(EntryType.Misc) { Key = "test" };
        entry.Keywords = "keyword1 keyword2 keyword3";

        var keywords = entry.GetKeywordList();
        Assert.Equal(3, keywords.Count);
        Assert.Contains("keyword1", keywords);
        Assert.Contains("keyword2", keywords);
        Assert.Contains("keyword3", keywords);
    }

    [Fact]
    public void GetKeywordList_BracedPhrase()
    {
        var entry = new BibEntry(EntryType.Misc) { Key = "test" };
        entry.Keywords = "{machine learning algorithms}";

        var keywords = entry.GetKeywordList();
        Assert.Single(keywords);
        Assert.Equal("machine learning algorithms", keywords.First());
    }

    [Fact]
    public void SetKeywordList_WithCollection()
    {
        var entry = new BibEntry(EntryType.Misc) { Key = "test" };
        var keywords = new[] { "keyword1", "keyword2", "keyword3" };

        entry.SetKeywordList(keywords);

        Assert.Equal("keyword1, keyword2, keyword3", entry.Keywords);
    }

    [Fact]
    public void SetKeywordList_WithCustomSeparator()
    {
        var entry = new BibEntry(EntryType.Misc) { Key = "test" };
        var keywords = new[] { "keyword1", "keyword2", "keyword3" };

        entry.SetKeywordList(keywords, "; ");

        Assert.Equal("keyword1; keyword2; keyword3", entry.Keywords);
    }

    [Fact]
    public void SetKeywordList_Null_ClearsKeywords()
    {
        var entry = new BibEntry(EntryType.Misc) { Key = "test" };
        entry.Keywords = "existing keywords";

        entry.SetKeywordList(null);

        Assert.Null(entry.Keywords);
    }

    [Fact]
    public void SetKeywordList_FiltersEmptyKeywords()
    {
        var entry = new BibEntry(EntryType.Misc) { Key = "test" };
        var keywords = new[] { "keyword1", "", "keyword2", null!, "keyword3" };

        entry.SetKeywordList(keywords);

        var result = entry.GetKeywordList();
        Assert.Equal(3, result.Count);
        Assert.DoesNotContain("", result);
    }

    [Fact]
    public void AddKeyword_AddsToList()
    {
        var entry = new BibEntry(EntryType.Misc) { Key = "test" };
        entry.Keywords = "keyword1, keyword2";

        entry.AddKeyword("keyword3");

        var keywords = entry.GetKeywordList();
        Assert.Equal(3, keywords.Count);
        Assert.Contains("keyword3", keywords);
    }

    [Fact]
    public void AddKeyword_ToEmptyList()
    {
        var entry = new BibEntry(EntryType.Misc) { Key = "test" };

        entry.AddKeyword("keyword1");

        Assert.Equal("keyword1", entry.Keywords);
    }

    [Fact]
    public void AddKeyword_DuplicateIgnored()
    {
        var entry = new BibEntry(EntryType.Misc) { Key = "test" };
        entry.Keywords = "keyword1, keyword2";

        entry.AddKeyword("keyword1");

        var keywords = entry.GetKeywordList();
        Assert.Equal(2, keywords.Count);
    }

    [Fact]
    public void AddKeyword_CaseInsensitiveDuplicate()
    {
        var entry = new BibEntry(EntryType.Misc) { Key = "test" };
        entry.Keywords = "Keyword1";

        entry.AddKeyword("keyword1");

        var keywords = entry.GetKeywordList();
        Assert.Single(keywords);
    }

    [Fact]
    public void AddKeyword_NullOrEmpty_Ignored()
    {
        var entry = new BibEntry(EntryType.Misc) { Key = "test" };
        entry.Keywords = "keyword1";

        entry.AddKeyword(null!);
        entry.AddKeyword("");
        entry.AddKeyword("   ");

        var keywords = entry.GetKeywordList();
        Assert.Single(keywords);
    }

    [Fact]
    public void UpdateAuthorFields_UpdatesAuthorsAndEditors()
    {
        var entry = new BibEntry(EntryType.Book) { Key = "test" };
        entry.AddAuthor(new Author("Smith", "John", "A."));
        entry.AddEditor(new Author("Doe", "Jane", "B."));

        entry.UpdateAuthorFields(AuthorFormat.FirstNameFirst);

        Assert.Contains("John A. Smith", entry["author"]);
        Assert.Contains("Jane B. Doe", entry["editor"]);
    }

    [Fact]
    public void Parse_SingleEntry_ReturnsEntry()
    {
        var bibtex = @"
@article{test2025,
  author = {Smith, John},
  title = {Test Article},
  year = {2025}
}";

        var entry = BibEntry.Parse(bibtex);

        Assert.Equal("test2025", entry.Key);
        Assert.Equal("Test Article", entry.Title);
    }

    [Fact]
    public void Parse_NullOrEmpty_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => BibEntry.Parse(null!));
        Assert.Throws<ArgumentException>(() => BibEntry.Parse(""));
    }

    [Fact]
    public void Parse_NoEntries_ThrowsException()
    {
        Assert.Throws<Exceptions.BibParseException>(() => BibEntry.Parse("% Just a comment"));
    }

    [Fact]
    public void Parse_MultipleEntries_ThrowsException()
    {
        var bibtex = @"
@article{test1,author={A},title={T},year={2025}}
@article{test2,author={B},title={T2},year={2025}}";

        Assert.Throws<Exceptions.BibParseException>(() => BibEntry.Parse(bibtex));
    }

    [Fact]
    public void TryParse_ValidEntry_ReturnsTrue()
    {
        var bibtex = @"@article{test,author={Smith},title={Test},year={2025}}";

        var result = BibEntry.TryParse(bibtex, out var entry);

        Assert.True(result);
        Assert.NotNull(entry);
        Assert.Equal("test", entry.Key);
    }

    [Fact]
    public void TryParse_InvalidEntry_ReturnsFalse()
    {
        var bibtex = "invalid bibtex";

        var result = BibEntry.TryParse(bibtex, out var entry);

        Assert.False(result);
        Assert.Null(entry);
    }

    [Fact]
    public void TryParse_MultipleEntries_ReturnsFalse()
    {
        var bibtex = @"
@article{test1,author={A},title={T},year={2025}}
@article{test2,author={B},title={T2},year={2025}}";

        var result = BibEntry.TryParse(bibtex, out var entry);

        Assert.False(result);
        Assert.Null(entry);
    }

    [Fact]
    public void ParseAll_MultipleEntries_ReturnsAll()
    {
        var bibtex = @"
@article{test1,author={A},title={T1},year={2025}}
@article{test2,author={B},title={T2},year={2025}}
@book{test3,author={C},title={T3},publisher={P},year={2025}}";

        var entries = BibEntry.ParseAll(bibtex);

        Assert.Equal(3, entries.Count);
        Assert.Equal("test1", entries[0].Key);
        Assert.Equal("test2", entries[1].Key);
        Assert.Equal("test3", entries[2].Key);
    }

    [Fact]
    public void ParseAll_NullOrEmpty_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => BibEntry.ParseAll(null!));
        Assert.Throws<ArgumentException>(() => BibEntry.ParseAll(""));
    }

    [Fact]
    public void RegenerateKeys_GeneratesUniqueKeys()
    {
        var entries = new[]
        {
            new BibEntry(EntryType.Article) { Key = "old1", Title = "Test 1", Year = 2025 },
            new BibEntry(EntryType.Article) { Key = "old2", Title = "Test 2", Year = 2025 }
        };
        entries[0].AddAuthor(new Author("Smith", "John"));
        entries[1].AddAuthor(new Author("Smith", "John"));

        var keyMap = BibEntry.RegenerateKeys(entries, preserveExistingKeys: false);

        Assert.Equal(2, keyMap.Count);
        Assert.NotEqual("old1", entries[0].Key);
        Assert.NotEqual("old2", entries[1].Key);
        Assert.NotEqual(entries[0].Key, entries[1].Key);
    }

    [Fact]
    public void RegenerateKeys_PreservesExistingKeys()
    {
        var entries = new[]
        {
            new BibEntry(EntryType.Article) { Key = "existing1", Title = "Test 1", Year = 2025 },
            new BibEntry(EntryType.Article) { Key = "", Title = "Test 2", Year = 2025 }
        };
        entries[0].AddAuthor(new Author("Smith", "John"));
        entries[1].AddAuthor(new Author("Doe", "Jane"));

        var keyMap = BibEntry.RegenerateKeys(entries, preserveExistingKeys: true);

        Assert.Equal("existing1", entries[0].Key);
        Assert.NotEmpty(entries[1].Key);
    }

    [Fact]
    public void RegenerateKeys_NullEntries_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => BibEntry.RegenerateKeys(null!));
    }

    [Fact]
    public void ToBibTeX_GeneratesValidBibTeX()
    {
        var entry = new BibEntry(EntryType.Article)
        {
            Key = "test2025",
            Title = "Test Article",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John"));

        var bibtex = entry.ToBibTeX();

        Assert.Contains("@article{test2025,", bibtex);
        Assert.Contains("Smith, John", bibtex); // Just check for the author, format may vary
        Assert.Contains("Test Article", bibtex);
        Assert.Contains("2025", bibtex);
    }

    [Fact]
    public void ToBibTeX_WithAuthorFormat()
    {
        var entry = new BibEntry(EntryType.Article)
        {
            Key = "test2025"
        };
        entry.AddAuthor(new Author("Smith", "John"));

        var bibtex = entry.ToBibTeX(AuthorFormat.FirstNameFirst);

        Assert.Contains("John Smith", bibtex);
        Assert.DoesNotContain("Smith, John", bibtex);
    }

    [Fact]
    public void GetFields_ReturnsReadOnlyDictionary()
    {
        var entry = new BibEntry(EntryType.Article)
        {
            Key = "test",
            Title = "Test",
            Year = 2025
        };

        var fields = entry.GetFields();

        Assert.NotNull(fields);
        Assert.Contains("title", fields.Keys);
        Assert.Contains("year", fields.Keys);
    }

    [Fact]
    public void GetFieldNames_ReturnsAllFieldNames()
    {
        var entry = new BibEntry(EntryType.Article)
        {
            Key = "test",
            Title = "Test",
            Year = 2025
        };

        var fieldNames = entry.GetFieldNames().ToList();

        Assert.Contains("title", fieldNames);
        Assert.Contains("year", fieldNames);
    }

    [Fact]
    public void ToFormattedString_Chicago()
    {
        var entry = new BibEntry(EntryType.Article)
        {
            Key = "test2025",
            Title = "Test Article",
            Journal = "Test Journal",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John"));

        var formatted = entry.ToFormattedString(BibSharp.Utilities.CitationStyle.Chicago);

        Assert.Contains("Smith", formatted);
        Assert.Contains("Test Article", formatted);
    }
}

