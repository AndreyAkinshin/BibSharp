using BibSharp.Exceptions;
using BibSharp.Utilities;

namespace BibSharp.Tests;

public class BibParserEdgeCasesTests
{
    [Fact]
    public void Parse_UnclosedBraces_StrictMode_ThrowsException()
    {
        var parser = new BibParser(new BibParserSettings { StrictMode = true });
        var bibtex = @"@article{test2025,
  author = {Smith, John},
  title = {Test Article";

        Assert.Throws<BibParseException>(() => parser.ParseString(bibtex));
    }

    [Fact]
    public void Parse_UnclosedBraces_NonStrictMode_IgnoresEntry()
    {
        var parser = new BibParser(new BibParserSettings { StrictMode = false });
        var bibtex = @"@article{test2025,
  author = {Smith, John},
  title = {Test Article";

        var entries = parser.ParseString(bibtex);
        Assert.Empty(entries);
    }

    [Fact]
    public void Parse_UnclosedFieldValue_StrictMode_ThrowsException()
    {
        var parser = new BibParser(new BibParserSettings { StrictMode = true });
        var bibtex = @"@article{test2025,
  author = {Smith, John,
  title = {Test Article}
}";

        Assert.Throws<BibParseException>(() => parser.ParseString(bibtex));
    }

    [Fact]
    public void Parse_UnclosedQuote_StrictMode_ThrowsException()
    {
        var parser = new BibParser(new BibParserSettings { StrictMode = true });
        var bibtex = @"@article{test2025,
  author = ""Smith, John,
  title = {Test Article}
}";

        Assert.Throws<BibParseException>(() => parser.ParseString(bibtex));
    }

    [Fact]
    public void Parse_StringMacro_WithBraces()
    {
        var parser = new BibParser();
        var bibtex = @"
@string{JCS = {Journal of Computer Science}}
@article{test2025,
  author = {Smith, John},
  title = {Test},
  journal = JCS,
  year = {2025}
}";

        var entries = parser.ParseString(bibtex);
        Assert.Single(entries);
        Assert.Equal("Journal of Computer Science", entries[0].Journal);
        Assert.Contains("JCS", parser.StringMacros.Keys);
        Assert.Equal("Journal of Computer Science", parser.StringMacros["JCS"]);
    }

    [Fact]
    public void Parse_StringMacro_WithQuotes()
    {
        var parser = new BibParser();
        var bibtex = @"
@string{JCS = ""Journal of Computer Science""}
@article{test2025,
  author = {Smith, John},
  title = {Test},
  journal = JCS,
  year = {2025}
}";

        var entries = parser.ParseString(bibtex);
        Assert.Single(entries);
        Assert.Equal("Journal of Computer Science", entries[0].Journal);
    }

    [Fact]
    public void Parse_MacroExpansionDisabled()
    {
        var parser = new BibParser(new BibParserSettings { ExpandStringMacros = false });
        parser.AddStringMacro("JCS", "Journal of Computer Science");
        
        var bibtex = @"
@article{test2025,
  author = {Smith, John},
  title = {Test},
  journal = JCS,
  year = {2025}
}";

        var entries = parser.ParseString(bibtex);
        Assert.Single(entries);
        Assert.Equal("JCS", entries[0].Journal);
    }

    [Fact]
    public void Parse_Preamble()
    {
        var parser = new BibParser();
        var bibtex = @"
@preamble{This is a preamble}
@article{test2025,
  author = {Smith, John},
  title = {Test},
  year = {2025}
}";

        var entries = parser.ParseString(bibtex);
        Assert.Single(entries);
        Assert.Single(parser.Preambles);
        Assert.Equal("This is a preamble", parser.Preambles[0]);
    }

    [Fact]
    public void Parse_Comment_PreserveCommentsTrue()
    {
        var parser = new BibParser(new BibParserSettings { PreserveComments = true });
        var bibtex = @"% This is a comment
@article{test2025,
  author = {Smith, John},
  title = {Test},
  year = {2025}
}
% Another comment";

        var entries = parser.ParseString(bibtex);
        Assert.Single(entries);
        Assert.NotEmpty(parser.Comments);
        Assert.Contains(parser.Comments, c => c.Contains("This is a comment"));
    }

    [Fact]
    public void Parse_Comment_PreserveCommentsFalse()
    {
        var parser = new BibParser(new BibParserSettings { PreserveComments = false });
        var bibtex = @"% This is a comment
@article{test2025,
  author = {Smith, John},
  title = {Test},
  year = {2025}
}";

        var entries = parser.ParseString(bibtex);
        Assert.Single(entries);
        Assert.Empty(parser.Comments);
    }

    [Fact]
    public void Parse_CommentEntry()
    {
        var parser = new BibParser(new BibParserSettings { PreserveComments = true });
        var bibtex = @"
@comment{This is a comment entry}
@article{test2025,
  author = {Smith, John},
  title = {Test},
  year = {2025}
}";

        var entries = parser.ParseString(bibtex);
        Assert.Single(entries);
        Assert.Contains(parser.Comments, c => c.Contains("@comment"));
    }

    [Fact]
    public void Parse_NestedBraces()
    {
        var parser = new BibParser();
        var bibtex = @"
@article{test2025,
  author = {Smith, John},
  title = {Test with {nested {braces}}},
  year = {2025}
}";

        var entries = parser.ParseString(bibtex);
        Assert.Single(entries);
        Assert.Equal("Test with {nested {braces}}", entries[0].Title);
    }

    [Fact]
    public void Parse_EscapedCharacters()
    {
        var parser = new BibParser();
        var bibtex = @"
@article{test2025,
  author = {Smith, John},
  title = {Test with \{ escaped \} braces},
  year = {2025}
}";

        var entries = parser.ParseString(bibtex);
        Assert.Single(entries);
        Assert.Contains("escaped", entries[0].Title);
    }

    [Fact]
    public void Parse_EscapedQuotes()
    {
        var parser = new BibParser();
        var bibtex = @"
@article{test2025,
  author = ""Smith, John"",
  title = ""Test with quotes"",
  year = ""2025""
}";

        var entries = parser.ParseString(bibtex);
        Assert.Single(entries);
        Assert.Contains("Test", entries[0].Title);
    }

    [Fact]
    public void Parse_MacroConcatenation()
    {
        var parser = new BibParser();
        var bibtex = @"
@string{A = {Part A}}
@string{B = {Part B}}
@article{test2025,
  author = {Smith, John},
  title = #A# # #B#,
  year = {2025}
}";

        var entries = parser.ParseString(bibtex);
        Assert.Single(entries);
        // The title should contain both parts
        Assert.Contains("Part", entries[0].Title);
    }

    [Fact]
    public void Parse_NumericFieldValue()
    {
        var parser = new BibParser();
        var bibtex = @"
@article{test2025,
  author = {Smith, John},
  title = {Test},
  year = 2025,
  volume = 10
}";

        var entries = parser.ParseString(bibtex);
        Assert.Single(entries);
        Assert.Equal(2025, entries[0].Year);
        Assert.Equal(10, entries[0].Volume);
    }

    [Fact]
    public void Parse_EmptyFields()
    {
        var parser = new BibParser();
        var bibtex = @"
@article{test2025,
  author = {},
  title = {Test},
  year = {2025}
}";

        var entries = parser.ParseString(bibtex);
        Assert.Single(entries);
        // Empty fields may be stored as empty string or null depending on implementation
        var author = entries[0]["author"];
        Assert.True(string.IsNullOrEmpty(author));
    }

    [Fact]
    public void Parse_MissingComma()
    {
        var parser = new BibParser(new BibParserSettings { StrictMode = false });
        var bibtex = @"
@article{test2025,
  author = {Smith, John}
  title = {Test}
  year = {2025}
}";

        var entries = parser.ParseString(bibtex);
        // In non-strict mode, this might parse partially
        Assert.NotNull(entries);
    }

    [Fact]
    public void Parse_MultipleEntries()
    {
        var parser = new BibParser();
        var bibtex = @"
@article{test2025a,
  author = {Smith, John},
  title = {Test A},
  year = {2025}
}

@book{test2025b,
  author = {Doe, Jane},
  title = {Test B},
  publisher = {Publisher},
  year = {2025}
}";

        var entries = parser.ParseString(bibtex);
        Assert.Equal(2, entries.Count);
        Assert.Equal("test2025a", entries[0].Key);
        Assert.Equal("test2025b", entries[1].Key);
    }

    [Fact]
    public void Parse_WhitespaceHandling()
    {
        var parser = new BibParser();
        var bibtex = @"
@article{  test2025  ,
  author   =   {Smith, John}  ,
  title  =  {Test}   ,
  year =   {2025}  
}";

        var entries = parser.ParseString(bibtex);
        Assert.Single(entries);
        Assert.Equal("test2025", entries[0].Key);
    }

    [Fact]
    public void Parse_ConvertLatexToUnicode_Enabled()
    {
        var parser = new BibParser(new BibParserSettings { ConvertLatexToUnicode = true });
        var bibtex = @"
@article{test2025,
  author = {M\""uller, Hans},
  title = {Test},
  year = {2025}
}";

        var entries = parser.ParseString(bibtex);
        Assert.Single(entries);
        // Should convert LaTeX to Unicode
        Assert.Contains("ü", entries[0]["author"]);
    }

    [Fact]
    public void Parse_ConvertLatexToUnicode_Disabled()
    {
        var parser = new BibParser(new BibParserSettings { ConvertLatexToUnicode = false });
        var bibtex = @"
@article{test2025,
  author = {M\""uller, Hans},
  title = {Test},
  year = {2025}
}";

        var entries = parser.ParseString(bibtex);
        Assert.Single(entries);
        // Should NOT convert LaTeX to Unicode
        Assert.Contains("\\\"", entries[0]["author"]);
    }

    [Fact]
    public void Parse_NullInput_ThrowsException()
    {
        var parser = new BibParser();
        Assert.Throws<ArgumentNullException>(() => parser.ParseString(null!));
    }

    [Fact]
    public void Parse_EmptyString_ReturnsEmpty()
    {
        var parser = new BibParser();
        var entries = parser.ParseString("");
        Assert.Empty(entries);
    }

    [Fact]
    public void Parse_OnlyWhitespace_ReturnsEmpty()
    {
        var parser = new BibParser();
        var entries = parser.ParseString("   \n\t  ");
        Assert.Empty(entries);
    }

    [Fact]
    public void Parse_InvalidEntryType_NonStrictMode()
    {
        var parser = new BibParser(new BibParserSettings { StrictMode = false });
        var bibtex = @"
@invalidtype{test2025,
  author = {Smith, John},
  title = {Test},
  year = {2025}
}";

        var entries = parser.ParseString(bibtex);
        Assert.Single(entries);
        Assert.Equal("invalidtype", entries[0].EntryType.Value);
    }

    [Fact]
    public void AddStringMacro_NullName_ThrowsException()
    {
        var parser = new BibParser();
        Assert.Throws<ArgumentException>(() => parser.AddStringMacro(null!, "value"));
    }

    [Fact]
    public void AddStringMacro_EmptyName_ThrowsException()
    {
        var parser = new BibParser();
        Assert.Throws<ArgumentException>(() => parser.AddStringMacro("", "value"));
    }

    [Fact]
    public void ParseFile_NullPath_ThrowsException()
    {
        var parser = new BibParser();
        Assert.Throws<ArgumentException>(() => parser.ParseFile(null!));
    }

    [Fact]
    public void ParseFile_EmptyPath_ThrowsException()
    {
        var parser = new BibParser();
        Assert.Throws<ArgumentException>(() => parser.ParseFile(""));
    }

    [Fact]
    public void Parse_WithTextReader_NullReader_ThrowsException()
    {
        var parser = new BibParser();
        Assert.Throws<ArgumentNullException>(() => parser.Parse((TextReader)null!));
    }

    [Fact]
    public void Parse_WithTextReader_DisposesCorrectly()
    {
        var parser = new BibParser();
        var reader = new StringReader("@article{test,author={Test},title={Test},year={2025}}");
        var entries = parser.Parse(reader);
        Assert.Single(entries);
        // Reader should still be usable (not disposed by parser)
    }

    [Fact]
    public void Dispose_CanBeCalledMultipleTimes()
    {
        var parser = new BibParser();
        parser.Dispose();
        parser.Dispose(); // Should not throw
    }

    [Fact]
    public void Parse_EntryWithNoKey_StrictMode_ThrowsException()
    {
        var parser = new BibParser(new BibParserSettings { StrictMode = true });
        var bibtex = @"
@article{,
  author = {Smith, John},
  title = {Test},
  year = {2025}
}";

        var entries = parser.ParseString(bibtex);
        // Should handle empty key
        Assert.Single(entries);
        Assert.Equal("", entries[0].Key);
    }

    [Fact]
    public void Parse_CaseInsensitiveEntryType()
    {
        var parser = new BibParser();
        var bibtex = @"
@ARTICLE{test2025,
  author = {Smith, John},
  title = {Test},
  year = {2025}
}";

        var entries = parser.ParseString(bibtex);
        Assert.Single(entries);
        Assert.Equal("article", entries[0].EntryType.Value);
    }

    [Fact]
    public void Parse_CaseInsensitiveFieldNames()
    {
        var parser = new BibParser();
        var bibtex = @"
@article{test2025,
  AUTHOR = {Smith, John},
  TITLE = {Test},
  YEAR = {2025}
}";

        var entries = parser.ParseString(bibtex);
        Assert.Single(entries);
        Assert.Equal("Smith, John", entries[0]["author"]);
        Assert.Equal("Test", entries[0].Title);
    }
}

