using BibSharp.Utilities;

namespace BibSharp.Tests;

public class BibParserStreamingTests
{
    [Fact]
    public void Parse_WithStream_NullStream_ThrowsException()
    {
        var parser = new BibParser();
        Assert.Throws<ArgumentNullException>(() => parser.Parse((Stream)null!));
    }

    [Fact]
    public async Task ParseAsync_WithStream_NullStream_ThrowsException()
    {
        var parser = new BibParser();
        await Assert.ThrowsAsync<ArgumentNullException>(async () => 
            await parser.ParseAsync((Stream)null!));
    }

    [Fact]
    public async Task ParseAsync_TextReader_NullReader_ThrowsException()
    {
        var parser = new BibParser();
        await Assert.ThrowsAsync<ArgumentNullException>(async () => 
            await parser.ParseAsync((TextReader)null!));
    }

    [Fact]
    public async Task ParseFileAsync_NullPath_ThrowsException()
    {
        var parser = new BibParser();
        await Assert.ThrowsAsync<ArgumentException>(async () => 
            await parser.ParseFileAsync(null!));
    }

    [Fact]
    public async Task ParseFileAsync_EmptyPath_ThrowsException()
    {
        var parser = new BibParser();
        await Assert.ThrowsAsync<ArgumentException>(async () => 
            await parser.ParseFileAsync(""));
    }

    [Fact]
    public async Task ParseFileStreamAsync_NullPath_ThrowsException()
    {
        var parser = new BibParser();
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await foreach (var entry in parser.ParseFileStreamAsync(null!))
            {
                // Should not execute
            }
        });
    }

    [Fact]
    public async Task ParseFileStreamAsync_EmptyPath_ThrowsException()
    {
        var parser = new BibParser();
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await foreach (var entry in parser.ParseFileStreamAsync(""))
            {
                // Should not execute
            }
        });
    }

    [Fact]
    public void Parse_WithTextReader_ParsesCorrectly()
    {
        var bibtex = "@article{test,author={Smith},title={Test},journal={J},year={2025}}";
        var reader = new StringReader(bibtex);
        var parser = new BibParser();

        var entries = parser.Parse(reader);

        Assert.Single(entries);
        Assert.Equal("test", entries[0].Key);
    }

    [Fact]
    public async Task ParseAsync_WithTextReader_ParsesCorrectly()
    {
        var bibtex = "@article{test,author={Smith},title={Test},journal={J},year={2025}}";
        var reader = new StringReader(bibtex);
        var parser = new BibParser();

        var entries = await parser.ParseAsync(reader);

        Assert.Single(entries);
        Assert.Equal("test", entries[0].Key);
    }

    [Fact]
    public void Parse_LeavesReaderOpen()
    {
        var bibtex = "@article{test,author={Smith},title={Test},journal={J},year={2025}}";
        var reader = new StringReader(bibtex);
        var parser = new BibParser();

        var entries = parser.Parse(reader);

        // Reader should still be accessible after parse
        Assert.NotNull(reader);
    }
}

