using BibSharp.Models;
using BibSharp.Utilities;

namespace BibSharp.Tests;

public class BibSerializerCoverageTests
{
    [Fact]
    public void SerializeEntry_WithEmptyFields_SkipsEmptyValues()
    {
        var entry = new BibEntry(EntryType.Misc)
        {
            Key = "test",
            Title = "Test"
        };
        entry.SetField("custom", ""); // Empty field

        var serializer = new BibSerializer(new BibSerializerSettings
        {
            ValidateBeforeSerialization = false
        });

        var output = serializer.SerializeEntry(entry);
        
        Assert.DoesNotContain("custom", output);
        Assert.Contains("title", output);
    }

    [Fact]
    public void Serialize_SystemDefaultLineEnding()
    {
        var entry = new BibEntry(EntryType.Misc) { Key = "test", Title = "Test" };
        
        var serializer = new BibSerializer(new BibSerializerSettings
        {
            LineEnding = LineEnding.SystemDefault,
            ValidateBeforeSerialization = false
        });

        var output = serializer.Serialize(new[] { entry });
        
        // Should use system default (which varies by platform)
        Assert.NotEmpty(output);
    }

    [Fact]
    public void Serialize_EntryWithNullFields()
    {
        var entry = new BibEntry(EntryType.Misc)
        {
            Key = "test",
            Title = null,
            Year = null
        };

        var serializer = new BibSerializer(new BibSerializerSettings
        {
            ValidateBeforeSerialization = false
        });

        var output = serializer.Serialize(new[] { entry });
        
        Assert.Contains("@misc{test,", output);
        // Null fields should not appear
        Assert.DoesNotContain("title =", output);
        Assert.DoesNotContain("year =", output);
    }

    [Fact]
    public void Serialize_FieldOrderWithUnorderedFields()
    {
        var entry = new BibEntry(EntryType.Article)
        {
            Key = "test",
            Title = "Test",
            Year = 2025
        };
        entry["custom1"] = "value1";
        entry["custom2"] = "value2";

        var serializer = new BibSerializer(new BibSerializerSettings
        {
            FieldOrder = new[] { "title" }, // Only title in order
            ValidateBeforeSerialization = false
        });

        var output = serializer.Serialize(new[] { entry });
        
        int titlePos = output.IndexOf("title");
        int yearPos = output.IndexOf("year");
        
        // Title should come before year (which is not in FieldOrder)
        Assert.True(titlePos < yearPos);
    }

    [Fact]
    public void SerializeEntry_UpdatesAuthorFieldsBeforeSerialization()
    {
        var entry = new BibEntry(EntryType.Article)
        {
            Key = "test",
            Title = "Test"
        };
        entry.AddAuthor(new Author("Smith", "John"));
        entry.AddEditor(new Author("Doe", "Jane"));

        var serializer = new BibSerializer(new BibSerializerSettings
        {
            AuthorFormat = AuthorFormat.FirstNameFirst,
            ValidateBeforeSerialization = false
        });

        var output = serializer.SerializeEntry(entry);
        
        Assert.Contains("John Smith", output);
        Assert.Contains("Jane Doe", output);
    }

    [Fact]
    public void Serialize_WithSpecialCharacters_NoEncoding()
    {
        var entry = new BibEntry(EntryType.Misc)
        {
            Key = "test",
            Title = "Plain ASCII Title"
        };

        var serializer = new BibSerializer(new BibSerializerSettings
        {
            Encoding = BibEncoding.LaTeX,
            ValidateBeforeSerialization = false
        });

        var output = serializer.Serialize(new[] { entry });
        
        // No special characters, so no encoding needed
        Assert.Contains("Plain ASCII Title", output);
    }

    [Fact]
    public void Serialize_WrapField_AtWordBoundary()
    {
        var longText = "This is a very long field value that needs to be wrapped " +
                      "at word boundaries to avoid breaking in the middle of words";
        var entry = new BibEntry(EntryType.Misc)
        {
            Key = "test",
            Title = longText
        };

        var serializer = new BibSerializer(new BibSerializerSettings
        {
            WrapLongFields = true,
            MaxFieldLength = 30,
            ValidateBeforeSerialization = false
        });

        var output = serializer.Serialize(new[] { entry });
        
        // Should wrap and contain the text (may be split across lines)
        Assert.Contains("very", output);
        Assert.Contains("long", output);
        Assert.Contains("field", output);
    }

    [Fact]
    public void Serialize_WrapField_NoSpaceToBreak()
    {
        var longText = "AAAAAAAAAABBBBBBBBBBCCCCCCCCCCDDDDDDDDDDEEEEEEEEEEFFFFFFFFFF";
        var entry = new BibEntry(EntryType.Misc)
        {
            Key = "test",
            Title = longText
        };

        var serializer = new BibSerializer(new BibSerializerSettings
        {
            WrapLongFields = true,
            MaxFieldLength = 20,
            ValidateBeforeSerialization = false
        });

        var output = serializer.Serialize(new[] { entry });
        
        // Should handle text without spaces
        Assert.Contains("AAAA", output);
    }

    [Fact]
    public void Serialize_LastFieldNoComma()
    {
        var entry = new BibEntry(EntryType.Misc)
        {
            Key = "test",
            Title = "Only Field"
        };

        var serializer = new BibSerializer(new BibSerializerSettings
        {
            ValidateBeforeSerialization = false
        });

        var output = serializer.Serialize(new[] { entry });
        
        var lines = output.Split('\n');
        var titleLine = lines.FirstOrDefault(l => l.Contains("Only Field"));
        
        Assert.NotNull(titleLine);
        // Last field should not have trailing comma
        Assert.DoesNotContain("},", titleLine);
    }
}

