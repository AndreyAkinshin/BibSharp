using BibSharp.Utilities;

namespace BibSharp.Tests.Utilities;

public class CitationStyleTests
{
    [Fact]
    public void CitationStyle_Chicago()
    {
        var style = CitationStyle.Chicago;
        Assert.Equal(CitationStyle.Chicago, style);
    }

    [Fact]
    public void CitationStyle_Mla()
    {
        var style = CitationStyle.Mla;
        Assert.Equal(CitationStyle.Mla, style);
    }

    [Fact]
    public void CitationStyle_Apa()
    {
        var style = CitationStyle.Apa;
        Assert.Equal(CitationStyle.Apa, style);
    }

    [Fact]
    public void CitationStyle_Harvard()
    {
        var style = CitationStyle.Harvard;
        Assert.Equal(CitationStyle.Harvard, style);
    }

    [Fact]
    public void CitationStyle_Ieee()
    {
        var style = CitationStyle.Ieee;
        Assert.Equal(CitationStyle.Ieee, style);
    }
}

