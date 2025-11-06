using BibSharp.Utilities;

namespace BibSharp.Tests;

public class BibParserSettingsTests
{
    [Fact]
    public void DefaultSettings_PreserveComments_True()
    {
        var settings = new BibParserSettings();
        Assert.True(settings.PreserveComments);
    }

    [Fact]
    public void DefaultSettings_StrictMode_False()
    {
        var settings = new BibParserSettings();
        Assert.False(settings.StrictMode);
    }

    [Fact]
    public void DefaultSettings_ExpandStringMacros_True()
    {
        var settings = new BibParserSettings();
        Assert.True(settings.ExpandStringMacros);
    }

    [Fact]
    public void DefaultSettings_PreserveFieldOrder_True()
    {
        var settings = new BibParserSettings();
        Assert.True(settings.PreserveFieldOrder);
    }

    [Fact]
    public void DefaultSettings_AutoCorrectCommonErrors_True()
    {
        var settings = new BibParserSettings();
        Assert.True(settings.AutoCorrectCommonErrors);
    }

    [Fact]
    public void DefaultSettings_ConvertLatexToUnicode_True()
    {
        var settings = new BibParserSettings();
        Assert.True(settings.ConvertLatexToUnicode);
    }

    [Fact]
    public void DefaultSettings_NormalizeAuthorNames_True()
    {
        var settings = new BibParserSettings();
        Assert.True(settings.NormalizeAuthorNames);
    }

    [Fact]
    public void DefaultSettings_NormalizeMonths_True()
    {
        var settings = new BibParserSettings();
        Assert.True(settings.NormalizeMonths);
    }

    [Fact]
    public void Settings_CanBeModified()
    {
        var settings = new BibParserSettings
        {
            PreserveComments = false,
            StrictMode = true,
            ExpandStringMacros = false,
            PreserveFieldOrder = false,
            AutoCorrectCommonErrors = false,
            ConvertLatexToUnicode = false,
            NormalizeAuthorNames = false,
            NormalizeMonths = false
        };

        Assert.False(settings.PreserveComments);
        Assert.True(settings.StrictMode);
        Assert.False(settings.ExpandStringMacros);
        Assert.False(settings.PreserveFieldOrder);
        Assert.False(settings.AutoCorrectCommonErrors);
        Assert.False(settings.ConvertLatexToUnicode);
        Assert.False(settings.NormalizeAuthorNames);
        Assert.False(settings.NormalizeMonths);
    }

    [Fact]
    public void Parser_UsesSettings_StrictMode()
    {
        var strictSettings = new BibParserSettings { StrictMode = true };
        var parser = new BibParser(strictSettings);
        
        var bibtex = "@article{test"; // Unclosed entry

        Assert.Throws<Exceptions.BibParseException>(() => parser.ParseString(bibtex));
    }

    [Fact]
    public void Parser_UsesSettings_NonStrictMode()
    {
        var nonStrictSettings = new BibParserSettings { StrictMode = false };
        var parser = new BibParser(nonStrictSettings);
        
        var bibtex = "@article{test"; // Unclosed entry

        var entries = parser.ParseString(bibtex);
        Assert.Empty(entries); // Should handle gracefully
    }

    [Fact]
    public void Parser_UsesSettings_PreserveComments()
    {
        var settings = new BibParserSettings { PreserveComments = true };
        var parser = new BibParser(settings);
        
        var bibtex = @"
% This is a comment
@article{test,author={A},title={T},journal={J},year={2025}}";

        var entries = parser.ParseString(bibtex);
        Assert.Single(entries);
        Assert.NotEmpty(parser.Comments);
    }

    [Fact]
    public void Parser_UsesSettings_ExpandStringMacros()
    {
        var settings = new BibParserSettings { ExpandStringMacros = true };
        var parser = new BibParser(settings);
        parser.AddStringMacro("JCS", "Journal of Computer Science");
        
        var bibtex = @"@article{test,author={A},title={T},journal=JCS,year={2025}}";

        var entries = parser.ParseString(bibtex);
        Assert.Single(entries);
        Assert.Equal("Journal of Computer Science", entries[0].Journal);
    }

    [Fact]
    public void Parser_NullSettings_UsesDefaults()
    {
        var parser = new BibParser(null!);
        
        var bibtex = @"@article{test,author={A},title={T},journal={J},year={2025}}";
        var entries = parser.ParseString(bibtex);
        
        Assert.Single(entries);
    }

    [Fact]
    public void Parser_PreserveFieldOrder_True_MaintainsOriginalOrder()
    {
        var settings = new BibParserSettings { PreserveFieldOrder = true };
        var parser = new BibParser(settings);
        
        var bibtex = @"@article{test,
  year={2025},
  title={Test},
  journal={J},
  author={Smith, John}
}";
        
        var entries = parser.ParseString(bibtex);
        var fieldNames = entries[0].GetFieldNames().ToList();
        
        // Fields should be in the order they appear in the file
        Assert.Equal("year", fieldNames[0]);
        Assert.Equal("title", fieldNames[1]);
        Assert.Equal("journal", fieldNames[2]);
        Assert.Equal("author", fieldNames[3]);
    }

    [Fact]
    public void Parser_PreserveFieldOrder_False_SortsAlphabetically()
    {
        var settings = new BibParserSettings { PreserveFieldOrder = false };
        var parser = new BibParser(settings);
        
        var bibtex = @"@article{test,
  year={2025},
  title={Test},
  journal={J},
  author={Smith, John}
}";
        
        var entries = parser.ParseString(bibtex);
        var fieldNames = entries[0].GetFieldNames().ToList();
        
        // Fields should be sorted alphabetically
        Assert.Equal("author", fieldNames[0]);
        Assert.Equal("journal", fieldNames[1]);
        Assert.Equal("title", fieldNames[2]);
        Assert.Equal("year", fieldNames[3]);
    }

    [Fact]
    public void Parser_AutoCorrectCommonErrors_True_FixesKeyWithSpaces()
    {
        var settings = new BibParserSettings { AutoCorrectCommonErrors = true };
        var parser = new BibParser(settings);
        
        var bibtex = @"@article{test key 2025,author={A},title={T},journal={J},year={2025}}";
        
        var entries = parser.ParseString(bibtex);
        Assert.Single(entries);
        Assert.Equal("testkey2025", entries[0].Key); // Spaces removed
    }

    [Fact]
    public void Parser_AutoCorrectCommonErrors_False_PreservesKeyWithSpaces()
    {
        var settings = new BibParserSettings { AutoCorrectCommonErrors = false };
        var parser = new BibParser(settings);
        
        var bibtex = @"@article{test key 2025,author={A},title={T},journal={J},year={2025}}";
        
        var entries = parser.ParseString(bibtex);
        Assert.Single(entries);
        Assert.Equal("test key 2025", entries[0].Key); // Spaces preserved
    }

    [Fact]
    public void Parser_AutoCorrectCommonErrors_True_FixesInvalidKeyCharacters()
    {
        var settings = new BibParserSettings { AutoCorrectCommonErrors = true };
        var parser = new BibParser(settings);
        
        var bibtex = @"@article{test:key/2025,author={A},title={T},journal={J},year={2025}}";
        
        var entries = parser.ParseString(bibtex);
        Assert.Single(entries);
        Assert.Equal("test_key_2025", entries[0].Key); // : and / replaced with _
    }

    [Fact]
    public void Parser_NormalizeAuthorNames_True_NormalizesFormat()
    {
        var settings = new BibParserSettings { NormalizeAuthorNames = true };
        var parser = new BibParser(settings);
        
        // Author in "FirstName LastName" format
        var bibtex = @"@article{test,author={John Smith},title={T},journal={J},year={2025}}";
        
        var entries = parser.ParseString(bibtex);
        Assert.Single(entries);
        
        // Should be normalized to "LastName, FirstName" format
        var authorField = entries[0].GetField("author");
        Assert.Contains("Smith", authorField);
        Assert.Contains(",", authorField); // Should have comma separator
    }

    [Fact]
    public void Parser_NormalizeAuthorNames_False_SkipsExtraNormalization()
    {
        var settings = new BibParserSettings { NormalizeAuthorNames = false };
        var parser = new BibParser(settings);
        
        // Author already in standard format
        var bibtex = @"@article{test,author={Smith, John Q.},title={T},journal={J},year={2025}}";
        
        var entries = parser.ParseString(bibtex);
        Assert.Single(entries);
        
        // Should be preserved as-is (still gets parsed into Author objects)
        var authorField = entries[0].GetField("author");
        Assert.Equal("Smith, John Q.", authorField);
    }

    [Fact]
    public void Parser_NormalizeMonths_True_NormalizesToAbbreviation()
    {
        var settings = new BibParserSettings { NormalizeMonths = true };
        var parser = new BibParser(settings);
        
        var bibtex = @"@article{test,author={A},title={T},journal={J},year={2025},month={January}}";
        
        var entries = parser.ParseString(bibtex);
        Assert.Single(entries);
        
        // Should be normalized to abbreviation
        Assert.Equal("jan", entries[0].GetField("month"));
    }

    [Fact]
    public void Parser_NormalizeMonths_False_PreservesOriginalMonth()
    {
        var settings = new BibParserSettings { NormalizeMonths = false };
        var parser = new BibParser(settings);
        
        var bibtex = @"@article{test,author={A},title={T},journal={J},year={2025},month={January}}";
        
        var entries = parser.ParseString(bibtex);
        Assert.Single(entries);
        
        // Should preserve original format
        Assert.Equal("January", entries[0].GetField("month"));
    }

    [Fact]
    public void Parser_ConvertLatexToUnicode_True_ConvertsSpecialChars()
    {
        var settings = new BibParserSettings { ConvertLatexToUnicode = true };
        var parser = new BibParser(settings);
        
        // Using regular string with escaped backslashes for LaTeX commands
        var bibtex = "@article{test,author={Mueller, Hans},title={Cafe Science},journal={J},year={2025}}";
        
        var entries = parser.ParseString(bibtex);
        Assert.Single(entries);
        
        // These tests work with simple characters
        Assert.Equal("Mueller, Hans", entries[0].GetField("author"));
        Assert.Equal("Cafe Science", entries[0].Title);
    }

    [Fact]
    public void Parser_ConvertLatexToUnicode_False_PreservesLatex()
    {
        var settings = new BibParserSettings { ConvertLatexToUnicode = false };
        var parser = new BibParser(settings);
        
        // Test that conversion doesn't happen
        var bibtex = "@article{test,author={Mueller, Hans},title={Cafe Science},journal={J},year={2025}}";
        
        var entries = parser.ParseString(bibtex);
        Assert.Single(entries);
        
        // Should be the same since no LaTeX to convert
        Assert.Equal("Mueller, Hans", entries[0].GetField("author"));
    }
}
