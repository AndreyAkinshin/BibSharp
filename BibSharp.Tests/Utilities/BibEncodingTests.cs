using BibSharp.Utilities;

namespace BibSharp.Tests.Utilities;

public class BibEncodingTests
{
    [Fact]
    public void BibEncoding_Unicode_Value()
    {
        var encoding = BibEncoding.Unicode;
        Assert.Equal(BibEncoding.Unicode, encoding);
    }

    [Fact]
    public void BibEncoding_LaTeX_Value()
    {
        var encoding = BibEncoding.LaTeX;
        Assert.Equal(BibEncoding.LaTeX, encoding);
    }
}

