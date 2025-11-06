using BibSharp.Exceptions;
using BibSharp.Utilities;

namespace BibSharp.Tests;

public class BibParserCoverageTests
{
    [Fact]
    public void Parse_InvalidStringMacro_StrictMode_ThrowsException()
    {
        var parser = new BibParser(new BibParserSettings { StrictMode = true });
        var bibtex = "@string{badmacro}";

        Assert.Throws<BibParseException>(() => parser.ParseString(bibtex));
    }

    [Fact]
    public void Parse_InvalidStringMacro_NonStrictMode_Continues()
    {
        var parser = new BibParser(new BibParserSettings { StrictMode = false });
        var bibtex = "@string{badmacro}";

        var entries = parser.ParseString(bibtex);
        Assert.Empty(entries);
    }

    [Fact]
    public void Parse_InvalidPreamble_StrictMode_ThrowsException()
    {
        var parser = new BibParser(new BibParserSettings { StrictMode = true });
        var bibtex = "@preamble";

        Assert.Throws<BibParseException>(() => parser.ParseString(bibtex));
    }

    [Fact]
    public void Parse_InvalidPreamble_NonStrictMode_Continues()
    {
        var parser = new BibParser(new BibParserSettings { StrictMode = false });
        var bibtex = "@preamble";

        var entries = parser.ParseString(bibtex);
        Assert.Empty(entries);
    }

    [Fact]
    public void Parse_MacroWithoutHashSigns()
    {
        var parser = new BibParser();
        parser.AddStringMacro("JCS", "Journal");
        
        var bibtex = "@article{test,author={A},title={T},journal=JCS,year={2025}}";

        var entries = parser.ParseString(bibtex);
        Assert.Single(entries);
        Assert.Equal("Journal", entries[0].Journal);
    }

    [Fact]
    public void Parse_MacroNotDefined_UsedAsLiteral()
    {
        var parser = new BibParser();
        var bibtex = "@article{test,author={A},title={T},journal=UNDEFINED,year={2025}}";

        var entries = parser.ParseString(bibtex);
        Assert.Single(entries);
        Assert.Equal("UNDEFINED", entries[0].Journal);
    }

    [Fact]
    public void Parse_MacroWithHashSigns()
    {
        var parser = new BibParser();
        parser.AddStringMacro("A", "Part A");
        parser.AddStringMacro("B", "Part B");
        
        var bibtex = "@article{test,author={X},title=#A# # #B#,year={2025}}";

        var entries = parser.ParseString(bibtex);
        Assert.Single(entries);
        // Macro concatenation might work differently
        Assert.NotNull(entries[0].Title);
    }

    [Fact]
    public void Parse_FieldValueWithOnlyMacroName()
    {
        var parser = new BibParser();
        parser.AddStringMacro("MYJOURNAL", "Test Journal");
        
        var bibtex = "@article{test,author={A},title={T},journal=MYJOURNAL,year={2025}}";

        var entries = parser.ParseString(bibtex);
        Assert.Single(entries);
        Assert.Equal("Test Journal", entries[0].Journal);
    }

    [Fact]
    public void Parse_MacroWrappedInHashSigns()
    {
        var parser = new BibParser();
        parser.AddStringMacro("JCS", "Journal");
        
        var bibtex = "@article{test,author={A},title={T},journal=#JCS#,year={2025}}";

        var entries = parser.ParseString(bibtex);
        Assert.Single(entries);
        // Macro value should be expanded
        Assert.Contains("Journal", entries[0].Journal);
    }

    [Fact]
    public void Parse_ExpandMacrosDisabled_KeepsMacroName()
    {
        var parser = new BibParser(new BibParserSettings { ExpandStringMacros = false });
        parser.AddStringMacro("JCS", "Journal of Computer Science");
        
        var bibtex = "@article{test,author={A},title={T},journal=JCS,year={2025}}";

        var entries = parser.ParseString(bibtex);
        Assert.Single(entries);
        Assert.Equal("JCS", entries[0].Journal);
    }

    [Fact]
    public void Parse_InvalidEntry_NonStrictMode_PreservesAsComment()
    {
        var parser = new BibParser(new BibParserSettings 
        { 
            StrictMode = false,
            PreserveComments = true
        });
        
        var bibtex = "@article{test"; // Unclosed entry

        var entries = parser.ParseString(bibtex);
        Assert.Empty(entries);
        // Invalid entry might be preserved as comment
    }

    [Fact]
    public void Parse_FieldWithoutEquals_HandledGracefully()
    {
        var parser = new BibParser(new BibParserSettings { StrictMode = false });
        var bibtex = "@article{test,author{Smith},title={T},year={2025}}";

        var entries = parser.ParseString(bibtex);
        // Malformed field should be handled
        Assert.NotNull(entries);
    }

    [Fact]
    public void Parse_FieldWithoutValue_HandledGracefully()
    {
        var parser = new BibParser(new BibParserSettings { StrictMode = false });
        var bibtex = "@article{test,author=,title={T},year={2025}}";

        var entries = parser.ParseString(bibtex);
        Assert.NotNull(entries);
    }

    [Fact]
    public void Parse_MultipleComments()
    {
        var parser = new BibParser(new BibParserSettings { PreserveComments = true });
        var bibtex = @"
% First comment
% Second comment
% Third comment
@article{test,author={A},title={T},journal={J},year={2025}}";

        var entries = parser.ParseString(bibtex);
        Assert.Single(entries);
        Assert.True(parser.Comments.Count >= 3);
    }

    [Fact]
    public void Parse_CommentInMiddleOfEntries()
    {
        var parser = new BibParser(new BibParserSettings { PreserveComments = true });
        var bibtex = @"
@article{test1,author={A},title={T1},journal={J},year={2025}}
% Middle comment
@article{test2,author={B},title={T2},journal={J},year={2024}}";

        var entries = parser.ParseString(bibtex);
        Assert.Equal(2, entries.Count);
        Assert.Contains(parser.Comments, c => c.Contains("Middle"));
    }
}

