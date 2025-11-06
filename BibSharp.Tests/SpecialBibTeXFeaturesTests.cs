using BibSharp.Models;
using BibSharp.Utilities;

namespace BibSharp.Tests;

public class SpecialBibTeXFeaturesTests
{
    [Fact]
    public void Parse_NestedBraces_PreservesStructure()
    {
        var parser = new BibParser();
        var bibtex = @"
@article{test,
  author = {Smith, John},
  title = {{A Study of {Nested {Deeply Nested} Braces}}},
  journal = {J},
  year = {2025}
}";

        var entries = parser.ParseString(bibtex);
        Assert.Single(entries);
        Assert.Contains("Nested", entries[0].Title);
        Assert.Contains("{", entries[0].Title);
    }

    [Fact]
    public void Parse_BracesInFieldValue()
    {
        var parser = new BibParser();
        var bibtex = @"
@article{test,
  author = {Smith, John},
  title = {The {DNA} Analysis},
  journal = {J},
  year = {2025}
}";

        var entries = parser.ParseString(bibtex);
        Assert.Single(entries);
        Assert.Contains("{DNA}", entries[0].Title);
    }

    [Fact]
    public void Parse_NumberField_AsLiteral()
    {
        var parser = new BibParser();
        var bibtex = @"
@article{test,
  author = {Smith, John},
  title = {Test},
  journal = {J},
  year = 2025,
  volume = 10
}";

        var entries = parser.ParseString(bibtex);
        Assert.Single(entries);
        Assert.Equal(2025, entries[0].Year);
        Assert.Equal(10, entries[0].Volume);
    }

    [Fact]
    public void Parse_CrossRef_Field()
    {
        var parser = new BibParser();
        var bibtex = @"
@inproceedings{paper1,
  author = {Smith, John},
  title = {Paper Title},
  crossref = {conf2025}
}

@proceedings{conf2025,
  title = {Conference Proceedings},
  year = {2025}
}";

        var entries = parser.ParseString(bibtex);
        Assert.Equal(2, entries.Count);
        // The crossref field should be preserved
        var paper = entries.FirstOrDefault(e => e.Key == "paper1");
        Assert.NotNull(paper);
        Assert.Equal("conf2025", paper["crossref"]);
    }

    [Fact]
    public void Parse_VeryLongFieldValue()
    {
        var longAbstract = new string('a', 5000);
        var parser = new BibParser();
        var bibtex = $@"
@article{{test,
  author = {{Smith, John}},
  title = {{Test}},
  journal = {{J}},
  year = {{2025}},
  abstract = {{{longAbstract}}}
}}";

        var entries = parser.ParseString(bibtex);
        Assert.Single(entries);
        Assert.Equal(longAbstract, entries[0].Abstract);
    }

    [Fact]
    public void Serialize_VeryLongFieldValue_WithWrapping()
    {
        var entry = new BibEntry(EntryType.Article)
        {
            Key = "test",
            Abstract = new string('a', 500)
        };

        var serializer = new BibSerializer(new BibSerializerSettings
        {
            WrapLongFields = true,
            MaxFieldLength = 80,
            ValidateBeforeSerialization = false
        });

        var bibtex = serializer.Serialize(new[] { entry });
        Assert.Contains("abstract", bibtex);
    }

    [Fact]
    public void Parse_MultilineBibTeX()
    {
        var parser = new BibParser();
        var bibtex = @"
@article{test,
  author = {Smith, John
            and Doe, Jane
            and Johnson, Bob},
  title = {A Very Long Title
           That Spans Multiple
           Lines},
  journal = {J},
  year = {2025}
}";

        var entries = parser.ParseString(bibtex);
        Assert.Single(entries);
        Assert.Equal(3, entries[0].Authors.Count);
        Assert.Contains("Long Title", entries[0].Title);
    }

    [Fact]
    public void Parse_SpecialCharactersInKey()
    {
        // Disable auto-correction to preserve special characters in keys
        var settings = new BibParserSettings { AutoCorrectCommonErrors = false };
        var parser = new BibParser(settings);
        var bibtex = @"
@article{smith:2025:test,
  author = {Smith, John},
  title = {Test},
  journal = {J},
  year = {2025}
}";

        var entries = parser.ParseString(bibtex);
        Assert.Single(entries);
        Assert.Equal("smith:2025:test", entries[0].Key);
    }

    [Fact]
    public void Parse_MinimalEntry()
    {
        var parser = new BibParser(new BibParserSettings { StrictMode = false });
        var bibtex = @"@misc{minimal,}";

        var entries = parser.ParseString(bibtex);
        // Entry with just a key and trailing comma
        Assert.Single(entries);
        Assert.Equal("minimal", entries[0].Key);
    }

    [Fact]
    public void Parse_TrailingCommaInLastField()
    {
        var parser = new BibParser();
        var bibtex = @"
@article{test,
  author = {Smith, John},
  title = {Test},
  year = {2025},
}";

        var entries = parser.ParseString(bibtex);
        Assert.Single(entries);
        Assert.Equal(2025, entries[0].Year);
    }

    [Fact]
    public void Parse_NoTrailingCommaInLastField()
    {
        var parser = new BibParser();
        var bibtex = @"
@article{test,
  author = {Smith, John},
  title = {Test},
  year = {2025}
}";

        var entries = parser.ParseString(bibtex);
        Assert.Single(entries);
        Assert.Equal(2025, entries[0].Year);
    }

    [Fact]
    public void Serialize_DifferentLineEndings()
    {
        var entry = new BibEntry(EntryType.Misc) { Key = "test", Title = "Test" };

        var serializerLf = new BibSerializer(new BibSerializerSettings
        {
            LineEnding = LineEnding.Lf,
            ValidateBeforeSerialization = false
        });
        var outputLf = serializerLf.Serialize(new[] { entry });
        Assert.Contains("\n", outputLf);
        Assert.DoesNotContain("\r", outputLf);

        var serializerCrlf = new BibSerializer(new BibSerializerSettings
        {
            LineEnding = LineEnding.Crlf,
            ValidateBeforeSerialization = false
        });
        var outputCrlf = serializerCrlf.Serialize(new[] { entry });
        Assert.Contains("\r\n", outputCrlf);
    }

    [Fact]
    public void Parse_ComplexStringMacros()
    {
        var parser = new BibParser();
        var bibtex = @"
@string{ACM = {Association for Computing Machinery}}
@string{PROC = {Proceedings of the }}
@article{test,
  author = {Smith, John},
  title = {Test},
  journal = ACM,
  year = {2025}
}";

        var entries = parser.ParseString(bibtex);
        Assert.Single(entries);
        Assert.Equal("Association for Computing Machinery", entries[0].Journal);
    }

    [Fact]
    public void Parse_MultiplePreambles()
    {
        var parser = new BibParser();
        var bibtex = @"
@preamble{First preamble}
@preamble{Second preamble}
@article{test,author={Smith},title={Test},journal={J},year={2025}}";

        var entries = parser.ParseString(bibtex);
        Assert.Single(entries);
        Assert.Equal(2, parser.Preambles.Count);
    }

    [Fact]
    public void FieldOrder_PreservedDuringRoundTrip()
    {
        var entry = new BibEntry(EntryType.Article) { Key = "test" };
        entry.Year = 2025;
        entry.Title = "Test";
        entry.Journal = "J";
        entry.AddAuthor(new Author("Smith", "John"));

        var fieldNames = entry.GetFieldNames().ToList();
        
        // Fields should be in the order they were set
        Assert.Equal("year", fieldNames[0]);
        Assert.Equal("title", fieldNames[1]);
        Assert.Equal("journal", fieldNames[2]);
        Assert.Equal("author", fieldNames[3]);
    }
}

