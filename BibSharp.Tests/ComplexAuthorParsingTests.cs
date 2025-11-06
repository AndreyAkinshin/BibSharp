using BibSharp.Models;

namespace BibSharp.Tests;

public class ComplexAuthorParsingTests
{
    [Fact]
    public void Parse_MultipleAuthors_WithAnd()
    {
        var parser = new BibParser();
        var bibtex = @"
@article{test2025,
  author = {Smith, John and Doe, Jane and Johnson, Bob},
  title = {Test},
  journal = {J},
  year = {2025}
}";

        var entries = parser.ParseString(bibtex);
        Assert.Single(entries);
        Assert.Equal(3, entries[0].Authors.Count);
        Assert.Equal("Smith", entries[0].Authors[0].LastName);
        Assert.Equal("Doe", entries[0].Authors[1].LastName);
        Assert.Equal("Johnson", entries[0].Authors[2].LastName);
    }

    [Fact]
    public void Parse_MultipleAuthors_WithAND()
    {
        var parser = new BibParser();
        var bibtex = @"
@article{test2025,
  author = {Smith, John AND Doe, Jane},
  title = {Test},
  journal = {J},
  year = {2025}
}";

        var entries = parser.ParseString(bibtex);
        Assert.Single(entries);
        Assert.Equal(2, entries[0].Authors.Count);
    }

    [Fact]
    public void Parse_AuthorWithMiddleName()
    {
        var parser = new BibParser();
        var bibtex = @"
@article{test2025,
  author = {Smith, John Andrew},
  title = {Test},
  journal = {J},
  year = {2025}
}";

        var entries = parser.ParseString(bibtex);
        Assert.Single(entries);
        Assert.Equal("Smith", entries[0].Authors[0].LastName);
        Assert.Equal("John", entries[0].Authors[0].FirstName);
        Assert.Equal("Andrew", entries[0].Authors[0].MiddleName);
    }

    [Fact]
    public void Parse_AuthorWithMultipleMiddleNames()
    {
        var parser = new BibParser();
        var bibtex = @"
@article{test2025,
  author = {Smith, John Andrew Benjamin},
  title = {Test},
  journal = {J},
  year = {2025}
}";

        var entries = parser.ParseString(bibtex);
        Assert.Single(entries);
        Assert.Equal("John", entries[0].Authors[0].FirstName);
        Assert.Equal("Andrew Benjamin", entries[0].Authors[0].MiddleName);
    }

    [Fact]
    public void Parse_AuthorWithSuffix()
    {
        var parser = new BibParser();
        var bibtex = @"
@article{test2025,
  author = {Smith, John Jr.},
  title = {Test},
  journal = {J},
  year = {2025}
}";

        var entries = parser.ParseString(bibtex);
        Assert.Single(entries);
        Assert.Equal("Jr.", entries[0].Authors[0].Suffix);
    }

    [Fact]
    public void Parse_MultipleEditors()
    {
        var parser = new BibParser();
        var bibtex = @"
@book{test2025,
  editor = {Smith, John and Doe, Jane},
  title = {Test Book},
  publisher = {Publisher},
  year = {2025}
}";

        var entries = parser.ParseString(bibtex);
        Assert.Single(entries);
        Assert.Equal(2, entries[0].Editors.Count);
        Assert.Equal("Smith", entries[0].Editors[0].LastName);
        Assert.Equal("Doe", entries[0].Editors[1].LastName);
    }

    [Fact]
    public void Parse_AuthorFirstNameFirst()
    {
        var parser = new BibParser();
        var bibtex = @"
@article{test2025,
  author = {John Andrew Smith},
  title = {Test},
  journal = {J},
  year = {2025}
}";

        var entries = parser.ParseString(bibtex);
        Assert.Single(entries);
        // When no comma, assumes FirstName MiddleName LastName
        Assert.Equal("Smith", entries[0].Authors[0].LastName);
        Assert.Equal("John", entries[0].Authors[0].FirstName);
        Assert.Equal("Andrew", entries[0].Authors[0].MiddleName);
    }

    [Fact]
    public void Parse_ComplexAuthorNames_WithSpecialCharacters()
    {
        var parser = new BibParser();
        var bibtex = @"
@article{test2025,
  author = {O'Brien, Se\'{a}n},
  title = {Test},
  journal = {J},
  year = {2025}
}";

        var entries = parser.ParseString(bibtex);
        Assert.Single(entries);
        Assert.Equal("O'Brien", entries[0].Authors[0].LastName);
    }

    [Fact]
    public void Parse_EmptyAuthor_IgnoresEmpty()
    {
        var entry = new BibEntry();
        entry.SetField("author", "");
        
        Assert.Empty(entry.Authors);
    }

    [Fact]
    public void Parse_SingleAuthorNoFirstName()
    {
        var parser = new BibParser();
        var bibtex = @"
@article{test2025,
  author = {Corporation},
  title = {Test},
  journal = {J},
  year = {2025}
}";

        var entries = parser.ParseString(bibtex);
        Assert.Single(entries);
        Assert.Equal("Corporation", entries[0].Authors[0].LastName);
        Assert.Null(entries[0].Authors[0].FirstName);
    }
}

