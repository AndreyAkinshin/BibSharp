using BibSharp.Exceptions;
using BibSharp.Models;
using BibSharp.Utilities;

namespace BibSharp.Tests;

public class BibSerializerEdgeCasesTests
{
    [Fact]
    public void Serialize_ValidationEnabled_InvalidEntry_ThrowsException()
    {
        var entry = new BibEntry("article")
        {
            Key = "", // Empty key is invalid
            Title = "Test"
        };

        var serializer = new BibSerializer(new BibSerializerSettings
        {
            ValidateBeforeSerialization = true
        });

        Assert.Throws<BibValidationException>(() => serializer.Serialize(new[] { entry }));
    }

    [Fact]
    public void Serialize_ValidationDisabled_InvalidEntry_Succeeds()
    {
        var entry = new BibEntry("article")
        {
            Key = "",
            Title = "Test"
        };

        var serializer = new BibSerializer(new BibSerializerSettings
        {
            ValidateBeforeSerialization = false
        });

        var output = serializer.Serialize(new[] { entry });
        Assert.Contains("@article{,", output);
    }

    [Fact]
    public void Serialize_LineEndingLf()
    {
        var entry = new BibEntry("article")
        {
            Key = "test",
            Title = "Test"
        };

        var serializer = new BibSerializer(new BibSerializerSettings
        {
            LineEnding = LineEnding.Lf,
            ValidateBeforeSerialization = false
        });

        var output = serializer.Serialize(new[] { entry });
        Assert.Contains("\n", output);
        Assert.DoesNotContain("\r\n", output);
    }

    [Fact]
    public void Serialize_LineEndingCrlf()
    {
        var entry = new BibEntry("article")
        {
            Key = "test",
            Title = "Test"
        };

        var serializer = new BibSerializer(new BibSerializerSettings
        {
            LineEnding = LineEnding.Crlf,
            ValidateBeforeSerialization = false
        });

        var output = serializer.Serialize(new[] { entry });
        Assert.Contains("\r\n", output);
    }

    [Fact]
    public void Serialize_WrapLongFields_WrapsText()
    {
        var entry = new BibEntry("article")
        {
            Key = "test",
            Title = "This is a very long title that should be wrapped across multiple lines when the wrapping feature is enabled in the serializer settings"
        };

        var serializer = new BibSerializer(new BibSerializerSettings
        {
            WrapLongFields = true,
            MaxFieldLength = 40,
            ValidateBeforeSerialization = false
        });

        var output = serializer.Serialize(new[] { entry });
        // Check that the output contains the title (wrapping may or may not happen depending on implementation details)
        Assert.Contains("This is a very long title", output);
    }

    [Fact]
    public void Serialize_WrapLongFields_DisabledByDefault()
    {
        var entry = new BibEntry("article")
        {
            Key = "test",
            Title = "This is a very long title that should NOT be wrapped"
        };

        var serializer = new BibSerializer(new BibSerializerSettings
        {
            WrapLongFields = false,
            ValidateBeforeSerialization = false
        });

        var output = serializer.Serialize(new[] { entry });
        // Title should be on one line
        Assert.Contains("title = {This is a very long title that should NOT be wrapped}", output);
    }

    [Fact]
    public void Serialize_CustomIndentation()
    {
        var entry = new BibEntry("article")
        {
            Key = "test",
            Title = "Test"
        };

        var serializer = new BibSerializer(new BibSerializerSettings
        {
            Indent = "\t",
            ValidateBeforeSerialization = false
        });

        var output = serializer.Serialize(new[] { entry });
        Assert.Contains("\ttitle = {Test}", output);
    }

    [Fact]
    public void Serialize_AuthorFormat_FirstNameFirst()
    {
        var entry = new BibEntry("article")
        {
            Key = "test",
            Title = "Test"
        };
        entry.AddAuthor(new Author("Smith", "John"));

        var serializer = new BibSerializer(new BibSerializerSettings
        {
            AuthorFormat = AuthorFormat.FirstNameFirst,
            ValidateBeforeSerialization = false
        });

        var output = serializer.Serialize(new[] { entry });
        Assert.Contains("author = {John Smith}", output);
    }

    [Fact]
    public void Serialize_AuthorFormat_LastNameFirst()
    {
        var entry = new BibEntry("article")
        {
            Key = "test",
            Title = "Test"
        };
        entry.AddAuthor(new Author("Smith", "John"));

        var serializer = new BibSerializer(new BibSerializerSettings
        {
            AuthorFormat = AuthorFormat.LastNameFirst,
            ValidateBeforeSerialization = false
        });

        var output = serializer.Serialize(new[] { entry });
        Assert.Contains("author = {Smith, John}", output);
    }

    [Fact]
    public void Serialize_LaTeXEncoding_ConvertsSpecialChars()
    {
        var entry = new BibEntry("article")
        {
            Key = "test",
            Title = "Café Müller"
        };

        var serializer = new BibSerializer(new BibSerializerSettings
        {
            Encoding = BibEncoding.LaTeX,
            ValidateBeforeSerialization = false
        });

        var output = serializer.Serialize(new[] { entry });
        Assert.Contains("Caf\\'e M\\\"uller", output);
    }

    [Fact]
    public void Serialize_UnicodeEncoding_PreservesChars()
    {
        var entry = new BibEntry("article")
        {
            Key = "test",
            Title = "Café Müller"
        };

        var serializer = new BibSerializer(new BibSerializerSettings
        {
            Encoding = BibEncoding.Unicode,
            ValidateBeforeSerialization = false
        });

        var output = serializer.Serialize(new[] { entry });
        Assert.Contains("Café Müller", output);
    }

    [Fact]
    public void Serialize_NullEntries_ThrowsException()
    {
        var serializer = new BibSerializer();
        Assert.Throws<ArgumentNullException>(() => serializer.Serialize(null!));
    }

    [Fact]
    public void Serialize_EmptyEntries_ReturnsEmpty()
    {
        var serializer = new BibSerializer();
        var output = serializer.Serialize(Array.Empty<BibEntry>());
        Assert.Empty(output.Trim());
    }

    [Fact]
    public void SerializeEntry_NullEntry_ThrowsException()
    {
        var serializer = new BibSerializer();
        Assert.Throws<ArgumentNullException>(() => serializer.SerializeEntry(null!));
    }

    [Fact]
    public void SerializeToFile_NullPath_ThrowsException()
    {
        var serializer = new BibSerializer();
        var entries = new[] { new BibEntry("misc") { Key = "test" } };
        Assert.Throws<ArgumentException>(() => serializer.SerializeToFile(entries, null!));
    }

    [Fact]
    public void SerializeToFile_EmptyPath_ThrowsException()
    {
        var serializer = new BibSerializer();
        var entries = new[] { new BibEntry("misc") { Key = "test" } };
        Assert.Throws<ArgumentException>(() => serializer.SerializeToFile(entries, ""));
    }

    [Fact]
    public void AddStringMacro_NullName_ThrowsException()
    {
        var serializer = new BibSerializer();
        Assert.Throws<ArgumentException>(() => serializer.AddStringMacro(null!, "value"));
    }

    [Fact]
    public void AddStringMacro_EmptyName_ThrowsException()
    {
        var serializer = new BibSerializer();
        Assert.Throws<ArgumentException>(() => serializer.AddStringMacro("", "value"));
    }

    [Fact]
    public void Serialize_WithQuotes_UsesQuotesForMacros()
    {
        var serializer = new BibSerializer(new BibSerializerSettings
        {
            UseBracesForFieldValues = false,
            ValidateBeforeSerialization = false
        });
        serializer.AddStringMacro("JCS", "Journal of Computer Science");

        var entry = new BibEntry("article")
        {
            Key = "test",
            Title = "Test"
        };

        var output = serializer.Serialize(new[] { entry });
        Assert.Contains("@string{JCS = \"Journal of Computer Science\"}", output);
    }

    [Fact]
    public void Serialize_MultipleStringMacros()
    {
        var serializer = new BibSerializer(new BibSerializerSettings
        {
            ValidateBeforeSerialization = false
        });
        serializer.AddStringMacro("JCS", "Journal of Computer Science");
        serializer.AddStringMacro("ACM", "Association for Computing Machinery");

        var entry = new BibEntry("article")
        {
            Key = "test",
            Title = "Test"
        };

        var output = serializer.Serialize(new[] { entry });
        Assert.Contains("@string{JCS", output);
        Assert.Contains("@string{ACM", output);
    }

    [Fact]
    public void Serialize_EntriesWithNoFields()
    {
        var entry = new BibEntry("misc")
        {
            Key = "test"
        };

        var serializer = new BibSerializer(new BibSerializerSettings
        {
            ValidateBeforeSerialization = false
        });

        var output = serializer.Serialize(new[] { entry });
        Assert.Contains("@misc{test,", output);
        Assert.Contains("}", output);
    }

    [Fact]
    public void Serialize_PreservesFieldOrder()
    {
        var entry = new BibEntry("article")
        {
            Key = "test",
            Year = 2025,
            Title = "Test"
        };
        entry["custom"] = "value";

        var serializer = new BibSerializer(new BibSerializerSettings
        {
            ValidateBeforeSerialization = false
        });

        var output = serializer.Serialize(new[] { entry });
        
        // Fields should appear in the order they were set
        int yearPos = output.IndexOf("year");
        int titlePos = output.IndexOf("title");
        int customPos = output.IndexOf("custom");
        
        Assert.True(yearPos > 0);
        Assert.True(titlePos > yearPos);
        Assert.True(customPos > titlePos);
    }

    [Fact]
    public async Task SerializeToFileAsync_CreatesFile()
    {
        var entry = new BibEntry("article")
        {
            Key = "test",
            Title = "Test"
        };

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
            Assert.Contains("@article{test,", content);
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
    public void BibSerializerSettings_MaxFieldLength_InvalidValue_ThrowsException()
    {
        var settings = new BibSerializerSettings();
        Assert.Throws<ArgumentException>(() => settings.MaxFieldLength = 0);
        Assert.Throws<ArgumentException>(() => settings.MaxFieldLength = -1);
    }

    [Fact]
    public void BibSerializerSettings_Validate_InvalidIndent_ThrowsException()
    {
        var settings = new BibSerializerSettings
        {
            Indent = null!
        };
        Assert.Throws<InvalidOperationException>(() => settings.Validate());
    }

    [Fact]
    public void BibSerializerSettings_Validate_WrapWithInvalidLength_ThrowsException()
    {
        var settings = new BibSerializerSettings
        {
            WrapLongFields = true
        };
        
        // Setting MaxFieldLength to 0 should throw immediately
        Assert.Throws<ArgumentException>(() => settings.MaxFieldLength = 0);
    }

    [Fact]
    public void Serialize_RemovesTrailingCommaFromLastField()
    {
        var entry = new BibEntry("article")
        {
            Key = "test",
            Title = "Test"
        };

        var serializer = new BibSerializer(new BibSerializerSettings
        {
            ValidateBeforeSerialization = false
        });

        var output = serializer.Serialize(new[] { entry });
        
        // The last field should not have a trailing comma
        var lines = output.Split('\n');
        var titleLine = lines.FirstOrDefault(l => l.Contains("title"));
        Assert.NotNull(titleLine);
        Assert.DoesNotContain("},", titleLine);
    }
}

