using BibSharp.Utilities;

namespace BibSharp.Tests;

public class BibParserTests
{
    [Fact]
    public void ParseFile_LoadsAllEntries()
    {
        var parser = new BibParser();
        var entries = parser.ParseFile("TestData/sample.bib");
        
        Assert.Equal(8, entries.Count);
    }
    
    [Fact]
    public void Parse_HandlesMacros()
    {
        var parser = new BibParser();
        var entries = parser.ParseFile("TestData/sample.bib");
        
        var article = entries.FirstOrDefault(e => e.Key == "smith2025");
        Assert.NotNull(article);
        Assert.Equal("Journal of Computer Science", article?.Journal);
    }
    
    [Fact]
    public void Parse_HandlesSpecialCharacters()
    {
        var parser = new BibParser();
        var entries = parser.ParseFile("TestData/sample.bib");
        
        var article = entries.FirstOrDefault(e => e.Key == "müller2024");
        Assert.NotNull(article);
        Assert.Equal("Müller", article?.Authors[0].LastName);
        Assert.Equal("Hans", article?.Authors[0].FirstName);
        Assert.Equal("García", article?.Authors[1].LastName);
        Assert.Equal("María", article?.Authors[1].FirstName);
        Assert.Equal("Café Science: A Study of Internationalization", article?.Title);
    }
    
    [Fact]
    public void ParseString_HandlesBasicEntry()
    {
        var parser = new BibParser();
        var entries = parser.ParseString(@"
@article{test2025,
  author = {Smith, John},
  title = {Test Article},
  journal = {Test Journal},
  year = {2025}
}");
        
        Assert.Single(entries);
        Assert.Equal("test2025", entries[0].Key);
        Assert.Equal("article", entries[0].EntryType);
        Assert.Equal("Smith, John", entries[0]["author"]);
        Assert.Equal("Test Article", entries[0].Title);
        Assert.Equal("Test Journal", entries[0].Journal);
        Assert.Equal(2025, entries[0].Year);
    }
    
    [Fact]
    public void ParseString_HandlesQuotedValues()
    {
        var parser = new BibParser();
        var entries = parser.ParseString(@"
@article{test2025,
  author = ""Smith, John"",
  title = ""Test Article"",
  journal = ""Test Journal"",
  year = ""2025""
}");
        
        Assert.Single(entries);
        Assert.Equal("test2025", entries[0].Key);
        Assert.Equal("Test Article", entries[0].Title);
    }
    
    [Fact]
    public void Parse_PreservesComments()
    {
        var parser = new BibParser(new BibParserSettings { PreserveComments = true });
        var entries = parser.ParseFile("TestData/sample.bib");
        
        Assert.NotEmpty(parser.Comments);
        Assert.Contains(parser.Comments, c => c.Contains("sample BibTeX file"));
    }
    
    [Fact]
    public async Task ParseAsync_LoadsAllEntries()
    {
        var parser = new BibParser();
        var entries = await parser.ParseFileAsync("TestData/sample.bib");
        
        Assert.Equal(8, entries.Count);
    }
    
    [Fact]
    public async Task ParseFileStreamAsync_EnumeratesAllEntries()
    {
        var parser = new BibParser();
        int count = 0;
        
        await foreach (var entry in parser.ParseFileStreamAsync("TestData/sample.bib"))
        {
            count++;
            Assert.NotNull(entry);
        }
        
        Assert.Equal(8, count);
    }
    
    [Fact]
    public void Parse_HandlesComplexAuthors()
    {
        var parser = new BibParser();
        var entries = parser.ParseFile("TestData/sample.bib");
        
        var entry = entries.FirstOrDefault(e => e.Key == "odd2023");
        Assert.NotNull(entry);
        Assert.NotNull(entry!.Authors);
        Assert.Single(entry.Authors);
        Assert.Equal("Lee", entry.Authors[0].LastName);
        Assert.Equal("David", entry.Authors[0].FirstName);
        Assert.Equal("J.", entry.Authors[0].MiddleName);
    }
    
    [Fact]
    public void AddStringMacro_ExpandsMacros()
    {
        var parser = new BibParser();
        parser.AddStringMacro("TEST", "Test Journal");
        
        var entries = parser.ParseString(@"
@article{test2025,
  author = {Smith, John},
  title = {Test Article},
  journal = TEST,
  year = {2025}
}");
        
        Assert.Single(entries);
        Assert.Equal("Test Journal", entries[0].Journal);
    }
    
    [Fact]
    public void Parse_IgnoresCommentEntries()
    {
        var parser = new BibParser();
        var entries = parser.ParseString(@"
@comment{This is a comment entry that should be ignored}

@article{test2025,
  author = {Smith, John},
  title = {Test Article},
  journal = {Test Journal},
  year = {2025}
}");
        
        Assert.Single(entries);
        Assert.Equal("test2025", entries[0].Key);
    }
}