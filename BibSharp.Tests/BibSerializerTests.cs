using BibSharp.Models;
using BibSharp.Utilities;

namespace BibSharp.Tests;

public class BibSerializerTests
{
    [Fact]
    public void Serialize_FormatsCorrectly()
    {
        var entry = new BibEntry("article")
        {
            Key = "test2025",
            Title = "Test Title",
            Year = 2025,
            Journal = "Test Journal"
        };
        entry.AddAuthor(new Author("Smith", "John"));
        
        var serializer = new BibSerializer(new BibSerializerSettings
        {
            ValidateBeforeSerialization = false
        });
        string output = serializer.Serialize(new[] { entry });
        
        Assert.Contains("@article{test2025,", output);
        Assert.Contains("  title = {Test Title},", output);
        Assert.Contains("  year = {2025},", output);
        Assert.Contains("  journal = {Test Journal},", output);
        Assert.Contains("  author = {Smith, John}", output);
    }
    
    [Fact]
    public void Serialize_IncludesStringMacros()
    {
        var entry = new BibEntry("article")
        {
            Key = "test2025",
            Title = "Test Title",
            Year = 2025,
            Journal = "Test Journal"
        };
        
        var serializer = new BibSerializer(new BibSerializerSettings { ValidateBeforeSerialization = false });
        serializer.AddStringMacro("TJ", "Test Journal");
        string output = serializer.Serialize(new[] { entry });
        
        Assert.Contains("@string{TJ = {Test Journal}}", output);
    }
    
    [Fact]
    public void Serialize_WithQuotesOption_UsesQuotes()
    {
        var entry = new BibEntry("article")
        {
            Key = "test2025",
            Title = "Test Title"
        };
        
        var serializer = new BibSerializer(new BibSerializerSettings
        {
            UseBracesForFieldValues = false,
            ValidateBeforeSerialization = false
        });
        string output = serializer.Serialize(new[] { entry });
        
        Assert.Contains("  title = \"Test Title\"", output);
    }
    
    [Fact]
    public void Serialize_WithOrderedFields_OrdersCorrectly()
    {
        var entry = new BibEntry("article")
        {
            Key = "test2025",
            Title = "Test Title",
            Year = 2025,
            Journal = "Test Journal"
        };
        entry.AddAuthor(new Author("Smith", "John"));
        
        var serializer = new BibSerializer(new BibSerializerSettings
        {
            FieldOrder = new[] { "author", "title", "year", "journal" },
            ValidateBeforeSerialization = false
        });
        string output = serializer.Serialize(new[] { entry });
        
        int authorPos = output.IndexOf("author");
        int titlePos = output.IndexOf("title");
        int yearPos = output.IndexOf("year");
        int journalPos = output.IndexOf("journal");
        
        Assert.True(authorPos < titlePos);
        Assert.True(titlePos < yearPos);
        Assert.True(yearPos < journalPos);
    }
    
    [Fact]
    public void Serialize_WithLaTeXEncoding_EscapesSpecialChars()
    {
        var entry = new BibEntry("article")
        {
            Key = "special2025",
            Title = "Test with äöü characters"
        };
        
        var serializer = new BibSerializer(new BibSerializerSettings
        {
            Encoding = BibEncoding.LaTeX,
            ValidateBeforeSerialization = false
        });
        string output = serializer.Serialize(new[] { entry });
        
        Assert.Contains(@"Test with \""a\""o\""u characters", output);
    }
    
    [Fact]
    public void Serialize_WithMultipleEntries_FormatsProperly()
    {
        var entry1 = new BibEntry("article") { Key = "one2025", Title = "First Article" };
        var entry2 = new BibEntry("book") { Key = "two2025", Title = "Second Book" };
        
        var serializer = new BibSerializer(new BibSerializerSettings { ValidateBeforeSerialization = false });
        string output = serializer.Serialize(new[] { entry1, entry2 });
        
        Assert.Contains("@article{one2025,", output);
        Assert.Contains("@book{two2025,", output);
    }
    
    [Fact]
    public void SerializeToFile_WritesToFile()
    {
        var entry = new BibEntry("article")
        {
            Key = "test2025",
            Title = "Test Title"
        };
        
        string tempFile = Path.GetTempFileName();
        try
        {
            var serializer = new BibSerializer(new BibSerializerSettings { ValidateBeforeSerialization = false });
            serializer.SerializeToFile(new[] { entry }, tempFile);
            
            string content = File.ReadAllText(tempFile);
            Assert.Contains("@article{test2025,", content);
            Assert.Contains("  title = {Test Title}", content);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }
    
    [Fact]
    public async Task SerializeToFileAsync_WritesToFile()
    {
        var entry = new BibEntry("article")
        {
            Key = "test2025",
            Title = "Test Title"
        };
        
        string tempFile = Path.GetTempFileName();
        try
        {
            var serializer = new BibSerializer(new BibSerializerSettings { ValidateBeforeSerialization = false });
            await serializer.SerializeToFileAsync(new[] { entry }, tempFile);
            
            string content = File.ReadAllText(tempFile);
            Assert.Contains("@article{test2025,", content);
            Assert.Contains("  title = {Test Title}", content);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }
}