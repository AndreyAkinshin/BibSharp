using BibSharp.Models;
using BibSharp.Utilities;

namespace BibSharp.Tests;

public class AsyncOperationTests
{
    [Fact]
    public async Task ParseFileAsync_LoadsEntries()
    {
        var parser = new BibParser();
        var entries = await parser.ParseFileAsync("TestData/sample.bib");
        
        Assert.NotEmpty(entries);
    }

    [Fact]
    public async Task ParseFileAsync_WithCancellationToken_CanBeCancelled()
    {
        var parser = new BibParser();
        var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(async () =>
            await parser.ParseFileAsync("TestData/sample.bib", cts.Token));
    }

    [Fact]
    public async Task ParseAsync_FromStream_LoadsEntries()
    {
        var parser = new BibParser();
        var bibtex = @"
@article{test2025,
  author = {Smith, John},
  title = {Test Article},
  journal = {Test Journal},
  year = {2025}
}";

        using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(bibtex));
        var entries = await parser.ParseAsync(stream);
        
        Assert.Single(entries);
        Assert.Equal("test2025", entries[0].Key);
    }

    [Fact]
    public async Task ParseAsync_WithCancellationToken_CanBeCancelled()
    {
        var parser = new BibParser();
        var bibtex = "@article{test,author={Test},title={Test},year={2025}}";
        var cts = new CancellationTokenSource();
        cts.Cancel();

        using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(bibtex));
        
        await Assert.ThrowsAsync<OperationCanceledException>(async () =>
            await parser.ParseAsync(stream, cts.Token));
    }

    [Fact]
    public async Task ParseAsync_TextReader_LoadsEntries()
    {
        var parser = new BibParser();
        var bibtex = @"
@article{test2025,
  author = {Smith, John},
  title = {Test Article},
  journal = {Test Journal},
  year = {2025}
}";

        using var reader = new StringReader(bibtex);
        var entries = await parser.ParseAsync(reader);
        
        Assert.Single(entries);
        Assert.Equal("test2025", entries[0].Key);
    }

    [Fact]
    public async Task ParseFileStreamAsync_EnumeratesEntries()
    {
        var parser = new BibParser();
        var entries = new List<BibEntry>();

        await foreach (var entry in parser.ParseFileStreamAsync("TestData/sample.bib"))
        {
            entries.Add(entry);
        }

        Assert.NotEmpty(entries);
    }

    [Fact]
    public async Task ParseFileStreamAsync_WithCancellation_CanBeCancelled()
    {
        var parser = new BibParser();
        var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(async () =>
        {
            await foreach (var entry in parser.ParseFileStreamAsync("TestData/sample.bib", cts.Token))
            {
                // Should not execute
            }
        });
    }

    [Fact]
    public async Task SerializeToFileAsync_CreatesFile()
    {
        var entry = new BibEntry(EntryType.Article)
        {
            Key = "test2025",
            Title = "Test Article",
            Year = 2025
        };
        entry.AddAuthor(new Author("Smith", "John"));

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
            Assert.Contains("@article{test2025,", content);
            Assert.Contains("Smith, John", content);
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
    public async Task SerializeToFileAsync_WithCancellation_CanBeCancelled()
    {
        var entry = new BibEntry(EntryType.Misc)
        {
            Key = "test"
        };

        var serializer = new BibSerializer(new BibSerializerSettings
        {
            ValidateBeforeSerialization = false
        });

        var cts = new CancellationTokenSource();
        cts.Cancel();

        var tempFile = Path.GetTempFileName();
        try
        {
            // TaskCanceledException is a subclass of OperationCanceledException
            await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
                await serializer.SerializeToFileAsync(new[] { entry }, tempFile, cts.Token));
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
    public async Task ParseFileStreamAsync_ProcessesEntriesIndividually()
    {
        var parser = new BibParser();
        var count = 0;

        await foreach (var entry in parser.ParseFileStreamAsync("TestData/sample.bib"))
        {
            Assert.NotNull(entry);
            Assert.NotEmpty(entry.Key);
            count++;
        }

        Assert.True(count > 0);
    }

    [Fact]
    public async Task ParallelParsing_MultipleFiles()
    {
        var tasks = new List<Task<IList<BibEntry>>>();
        
        for (int i = 0; i < 3; i++)
        {
            var parser = new BibParser();
            tasks.Add(parser.ParseFileAsync("TestData/sample.bib"));
        }

        var results = await Task.WhenAll(tasks);
        
        Assert.Equal(3, results.Length);
        foreach (var result in results)
        {
            Assert.NotEmpty(result);
        }
    }

    [Fact]
    public async Task ParseAsync_LargeFile_StreamsEfficiently()
    {
        var parser = new BibParser();
        var largeFile = GenerateLargeBibTeX(100);

        using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(largeFile));
        var entries = await parser.ParseAsync(stream);
        
        Assert.Equal(100, entries.Count);
    }

    [Fact]
    public async Task ParseFileStreamAsync_LargeFile_StreamsEfficiently()
    {
        var parser = new BibParser();
        var largeFile = GenerateLargeBibTeX(100);
        
        var tempFile = Path.GetTempFileName();
        try
        {
            await File.WriteAllTextAsync(tempFile, largeFile);
            
            var count = 0;
            await foreach (var entry in parser.ParseFileStreamAsync(tempFile))
            {
                count++;
            }
            
            Assert.Equal(100, count);
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
    public async Task SerializeToFileAsync_MultipleEntries()
    {
        var entries = new List<BibEntry>();
        for (int i = 0; i < 10; i++)
        {
            var entry = new BibEntry(EntryType.Article)
            {
                Key = $"test{i}",
                Title = $"Article {i}",
                Year = 2025
            };
            entry.AddAuthor(new Author($"Author{i}"));
            entries.Add(entry);
        }

        var serializer = new BibSerializer(new BibSerializerSettings
        {
            ValidateBeforeSerialization = false
        });

        var tempFile = Path.GetTempFileName();
        try
        {
            await serializer.SerializeToFileAsync(entries, tempFile);
            
            var content = await File.ReadAllTextAsync(tempFile);
            for (int i = 0; i < 10; i++)
            {
                Assert.Contains($"test{i}", content);
            }
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
    public async Task RoundTrip_ParseAndSerializeAsync()
    {
        var parser = new BibParser();
        var originalEntries = await parser.ParseFileAsync("TestData/sample.bib");

        var serializer = new BibSerializer(new BibSerializerSettings
        {
            ValidateBeforeSerialization = false
        });

        var tempFile = Path.GetTempFileName();
        try
        {
            await serializer.SerializeToFileAsync(originalEntries, tempFile);
            var reparsedEntries = await parser.ParseFileAsync(tempFile);
            
            Assert.Equal(originalEntries.Count, reparsedEntries.Count);
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }

    private string GenerateLargeBibTeX(int count)
    {
        var sb = new System.Text.StringBuilder();
        for (int i = 0; i < count; i++)
        {
            sb.AppendLine($@"
@article{{test{i},
  author = {{Author{i}}},
  title = {{Article {i}}},
  journal = {{Journal}},
  year = {{2025}}
}}");
        }
        return sb.ToString();
    }
}

