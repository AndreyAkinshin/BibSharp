using Xunit;

namespace BibSharp.Tests;

public class BibEntryDoiIntegrationTests
{
    [Fact]
    public void Doi_SetAsString_WorksWithImplicitConversion()
    {
        var entry = new BibEntry { Key = "test" };
        
        // Set DOI using string (implicit conversion)
        entry.Doi = "10.1234/abcd.5678";
        
        // Get DOI as struct
        Assert.NotNull(entry.Doi);
        Assert.Equal("10.1234/abcd.5678", entry.Doi.Value.Identifier);
        Assert.Equal("https://doi.org/10.1234/abcd.5678", entry.Doi.Value.Url);
    }

    [Fact]
    public void Doi_SetAsStruct_Works()
    {
        var entry = new BibEntry { Key = "test" };
        var doi = new Doi("10.1234/abcd.5678");
        
        entry.Doi = doi;
        
        Assert.NotNull(entry.Doi);
        Assert.Equal("10.1234/abcd.5678", entry.Doi.Value.Identifier);
    }

    [Fact]
    public void Doi_SetWithUrl_ExtractsIdentifier()
    {
        var entry = new BibEntry { Key = "test" };
        
        entry.Doi = "https://doi.org/10.1234/abcd.5678";
        
        Assert.NotNull(entry.Doi);
        Assert.Equal("10.1234/abcd.5678", entry.Doi.Value.Identifier);
        Assert.Equal("https://doi.org/10.1234/abcd.5678", entry.Doi.Value.Url);
    }

    [Fact]
    public void Doi_SetWithPrefix_ExtractsIdentifier()
    {
        var entry = new BibEntry { Key = "test" };
        
        entry.Doi = "doi:10.1234/abcd.5678";
        
        Assert.NotNull(entry.Doi);
        Assert.Equal("10.1234/abcd.5678", entry.Doi.Value.Identifier);
    }

    [Fact]
    public void Doi_ImplicitConversionToString_Works()
    {
        var entry = new BibEntry { Key = "test" };
        entry.Doi = "10.1234/abcd.5678";
        
        // Implicit conversion to string
        string? doiString = entry.Doi;
        
        Assert.Equal("10.1234/abcd.5678", doiString);
    }

    [Fact]
    public void GetDoiIdentifier_ReturnsCorrectValue()
    {
        var entry = new BibEntry { Key = "test" };
        entry.Doi = "https://doi.org/10.1234/abcd.5678";
        
        Assert.Equal("10.1234/abcd.5678", entry.GetDoiIdentifier());
    }

    [Fact]
    public void GetDoiUrl_ReturnsCorrectValue()
    {
        var entry = new BibEntry { Key = "test" };
        entry.Doi = "10.1234/abcd.5678";
        
        Assert.Equal("https://doi.org/10.1234/abcd.5678", entry.GetDoiUrl());
    }

    [Fact]
    public void GetDoiUrl_WithDifferentFormats_AlwaysReturnsHttps()
    {
        var entry1 = new BibEntry { Key = "test1", Doi = "10.1234/abcd.5678" };
        var entry2 = new BibEntry { Key = "test2", Doi = "http://doi.org/10.1234/abcd.5678" };
        var entry3 = new BibEntry { Key = "test3", Doi = "doi:10.1234/abcd.5678" };
        
        Assert.Equal("https://doi.org/10.1234/abcd.5678", entry1.GetDoiUrl());
        Assert.Equal("https://doi.org/10.1234/abcd.5678", entry2.GetDoiUrl());
        Assert.Equal("https://doi.org/10.1234/abcd.5678", entry3.GetDoiUrl());
    }

    [Fact]
    public void Doi_SetNull_RemovesField()
    {
        var entry = new BibEntry { Key = "test" };
        entry.Doi = "10.1234/abcd.5678";
        
        Assert.NotNull(entry.Doi);
        
        entry.Doi = null;
        
        Assert.Null(entry.Doi);
        Assert.Null(entry.GetDoiIdentifier());
        Assert.Null(entry.GetDoiUrl());
    }

    [Fact]
    public void Doi_ComparisonWithString_WorksViaImplicitConversion()
    {
        var entry = new BibEntry { Key = "test" };
        entry.Doi = "10.1234/abcd.5678";
        
        // This works because of implicit conversion from Doi to string
        string? doiValue = entry.Doi;
        Assert.Equal("10.1234/abcd.5678", doiValue);
    }

    [Fact]
    public void Doi_ToBibTeX_StoresIdentifierOnly()
    {
        var entry = new BibEntry(EntryType.Article)
        {
            Key = "test2023",
            Title = "Test Article",
            Doi = "https://doi.org/10.1234/abcd.5678"
        };
        
        entry.AddAuthor(new BibSharp.Models.Author("Doe", "John"));
        entry.Journal = "Test Journal";
        entry.Year = 2023;
        
        string bibtex = entry.ToBibTeX();
        
        // Should contain the identifier, not the full URL
        Assert.Contains("doi = {10.1234/abcd.5678}", bibtex);
        Assert.DoesNotContain("https://doi.org/", bibtex);
    }

    [Fact]
    public void Doi_ParsedFromBibTeX_PreservesIdentifier()
    {
        string bibtex = @"@article{test2023,
  author = {John Doe},
  title = {Test Article},
  journal = {Test Journal},
  year = {2023},
  doi = {10.1234/abcd.5678},
}";
        
        var entry = BibEntry.Parse(bibtex);
        
        Assert.NotNull(entry.Doi);
        Assert.Equal("10.1234/abcd.5678", entry.Doi.Value.Identifier);
        Assert.Equal("https://doi.org/10.1234/abcd.5678", entry.Doi.Value.Url);
    }
}

