using BibSharp.Utilities;

namespace BibSharp.Tests.Utilities;

public class LineEndingTests
{
    [Fact]
    public void LineEnding_SystemDefault_Value()
    {
        var lineEnding = LineEnding.SystemDefault;
        Assert.Equal(LineEnding.SystemDefault, lineEnding);
    }

    [Fact]
    public void LineEnding_Lf_Value()
    {
        var lineEnding = LineEnding.Lf;
        Assert.Equal(LineEnding.Lf, lineEnding);
    }

    [Fact]
    public void LineEnding_Crlf_Value()
    {
        var lineEnding = LineEnding.Crlf;
        Assert.Equal(LineEnding.Crlf, lineEnding);
    }
}

