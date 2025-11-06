using BibSharp.Models;
using BibSharp.Utilities;

namespace BibSharp.Tests;

public class FileOperationTests
{
    [Fact]
    public void ParseFile_RealFile_Succeeds()
    {
        var parser = new BibParser();
        var entries = parser.ParseFile("TestData/sample.bib");
        
        Assert.NotEmpty(entries);
    }

    [Fact]
    public void ParseFile_NonExistentFile_ThrowsException()
    {
        var parser = new BibParser();
        
        Assert.Throws<FileNotFoundException>(() => 
            parser.ParseFile("NonExistent.bib"));
    }

    [Fact]
    public async Task ParseFileAsync_RealFile_Succeeds()
    {
        var parser = new BibParser();
        var entries = await parser.ParseFileAsync("TestData/sample.bib");
        
        Assert.NotEmpty(entries);
    }

    [Fact]
    public async Task ParseFileAsync_NonExistentFile_ThrowsException()
    {
        var parser = new BibParser();
        
        await Assert.ThrowsAsync<FileNotFoundException>(async () => 
            await parser.ParseFileAsync("NonExistent.bib"));
    }

    [Fact]
    public void SerializeToFile_CreatesFile()
    {
        var entry = new BibEntry(EntryType.Misc) { Key = "test", Title = "Test" };
        var serializer = new BibSerializer(new BibSerializerSettings
        {
            ValidateBeforeSerialization = false
        });

        var tempFile = Path.GetTempFileName();
        try
        {
            serializer.SerializeToFile(new[] { entry }, tempFile);
            
            Assert.True(File.Exists(tempFile));
            var content = File.ReadAllText(tempFile);
            Assert.Contains("@misc{test,", content);
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }

    [Fact]
    public async Task SerializeToFileAsync_CreatesFile()
    {
        var entry = new BibEntry(EntryType.Misc) { Key = "test", Title = "Test" };
        var serializer = new BibSerializer(new BibSerializerSettings
        {
            ValidateBeforeSerialization = false
        });

        var tempFile = Path.GetTempFileName();
        try
        {
            await serializer.SerializeToFileAsync(new[] { entry }, tempFile);
            
            Assert.True(File.Exists(tempFile));
            var content = await File.ReadAllTextAsync(tempFile);
            Assert.Contains("@misc{test,", content);
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }

    [Fact]
    public void RoundTrip_FileToFileWithUTF8()
    {
        var original = new BibEntry(EntryType.Article)
        {
            Key = "test",
            Title = "Test with ñ and ü and é"
        };
        original.AddAuthor(new Author("Müller", "Hans"));

        var serializer = new BibSerializer(new BibSerializerSettings
        {
            ValidateBeforeSerialization = false,
            Encoding = BibEncoding.Unicode
        });

        var tempFile = Path.GetTempFileName();
        try
        {
            serializer.SerializeToFile(new[] { original }, tempFile);
            
            var parser = new BibParser();
            var entries = parser.ParseFile(tempFile);
            
            Assert.Single(entries);
            Assert.Contains("ü", entries[0].Authors[0].LastName);
            Assert.Contains("ñ", entries[0].Title);
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }

    [Fact]
    public void Parse_FromMemoryStream()
    {
        var bibtex = "@article{test,author={A},title={T},journal={J},year={2025}}";
        var bytes = System.Text.Encoding.UTF8.GetBytes(bibtex);
        
        using var stream = new MemoryStream(bytes);
        var parser = new BibParser();
        var entries = parser.Parse(stream);
        
        Assert.Single(entries);
        Assert.Equal("test", entries[0].Key);
    }

    [Fact]
    public async Task ParseAsync_FromMemoryStream()
    {
        var bibtex = "@article{test,author={A},title={T},journal={J},year={2025}}";
        var bytes = System.Text.Encoding.UTF8.GetBytes(bibtex);
        
        using var stream = new MemoryStream(bytes);
        var parser = new BibParser();
        var entries = await parser.ParseAsync(stream);
        
        Assert.Single(entries);
        Assert.Equal("test", entries[0].Key);
    }

    [Fact]
    public async Task ParseFileStreamAsync_LargeFile()
    {
        var parser = new BibParser();
        var count = 0;
        
        await foreach (var entry in parser.ParseFileStreamAsync("TestData/sample.bib"))
        {
            Assert.NotNull(entry);
            count++;
        }
        
        Assert.True(count > 0);
    }

    [Fact]
    public void Parse_WithTextReader_FromString()
    {
        var bibtex = "@article{test,author={A},title={T},journal={J},year={2025}}";
        
        using var reader = new StringReader(bibtex);
        var parser = new BibParser();
        var entries = parser.Parse(reader);
        
        Assert.Single(entries);
    }

    [Fact]
    public async Task ParseAsync_WithTextReader_FromString()
    {
        var bibtex = "@article{test,author={A},title={T},journal={J},year={2025}}";
        
        using var reader = new StringReader(bibtex);
        var parser = new BibParser();
        var entries = await parser.ParseAsync(reader);
        
        Assert.Single(entries);
    }
}

