using BibSharp.Models;

namespace BibSharp.Tests.Models;

public class AuthorFormatEnumTests
{
    [Fact]
    public void AuthorFormat_LastNameFirst()
    {
        var format = AuthorFormat.LastNameFirst;
        Assert.Equal(AuthorFormat.LastNameFirst, format);
    }

    [Fact]
    public void AuthorFormat_FirstNameFirst()
    {
        var format = AuthorFormat.FirstNameFirst;
        Assert.Equal(AuthorFormat.FirstNameFirst, format);
    }
}

