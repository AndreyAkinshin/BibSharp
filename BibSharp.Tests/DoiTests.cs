using Xunit;

namespace BibSharp.Tests;

public class DoiTests
{
    [Fact]
    public void Constructor_WithPlainIdentifier_StoresCorrectly()
    {
        var doi = new Doi("10.1234/abcd.5678");

        Assert.Equal("10.1234/abcd.5678", doi.Identifier);
        Assert.Equal("https://doi.org/10.1234/abcd.5678", doi.Url);
    }

    [Fact]
    public void Constructor_WithHttpsUrl_ExtractsIdentifier()
    {
        var doi = new Doi("https://doi.org/10.1234/abcd.5678");

        Assert.Equal("10.1234/abcd.5678", doi.Identifier);
        Assert.Equal("https://doi.org/10.1234/abcd.5678", doi.Url);
    }

    [Fact]
    public void Constructor_WithHttpUrl_ExtractsIdentifier()
    {
        var doi = new Doi("http://doi.org/10.1234/abcd.5678");

        Assert.Equal("10.1234/abcd.5678", doi.Identifier);
        Assert.Equal("https://doi.org/10.1234/abcd.5678", doi.Url);
    }

    [Fact]
    public void Constructor_WithDoiPrefix_ExtractsIdentifier()
    {
        var doi = new Doi("doi:10.1234/abcd.5678");

        Assert.Equal("10.1234/abcd.5678", doi.Identifier);
        Assert.Equal("https://doi.org/10.1234/abcd.5678", doi.Url);
    }

    [Fact]
    public void Constructor_WithDoiPrefixAndSpace_ExtractsIdentifier()
    {
        var doi = new Doi("doi: 10.1234/abcd.5678");

        Assert.Equal("10.1234/abcd.5678", doi.Identifier);
        Assert.Equal("https://doi.org/10.1234/abcd.5678", doi.Url);
    }

    [Fact]
    public void Constructor_WithWhitespace_TrimsCorrectly()
    {
        var doi = new Doi("  10.1234/abcd.5678  ");

        Assert.Equal("10.1234/abcd.5678", doi.Identifier);
        Assert.Equal("https://doi.org/10.1234/abcd.5678", doi.Url);
    }

    [Fact]
    public void Constructor_WithNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new Doi(null!));
    }

    [Fact]
    public void Constructor_WithEmptyString_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new Doi(string.Empty));
    }

    [Fact]
    public void Constructor_WithWhitespaceOnly_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new Doi("   "));
    }

    [Fact]
    public void TryParse_WithValidDoi_ReturnsTrue()
    {
        bool result = Doi.TryParse("10.1234/abcd.5678", out Doi doi);

        Assert.True(result);
        Assert.Equal("10.1234/abcd.5678", doi.Identifier);
    }

    [Fact]
    public void TryParse_WithValidUrl_ReturnsTrue()
    {
        bool result = Doi.TryParse("https://doi.org/10.1234/abcd.5678", out Doi doi);

        Assert.True(result);
        Assert.Equal("10.1234/abcd.5678", doi.Identifier);
    }

    [Fact]
    public void TryParse_WithNull_ReturnsFalse()
    {
        bool result = Doi.TryParse(null, out Doi doi);

        Assert.False(result);
        Assert.Equal(string.Empty, doi.Identifier);
    }

    [Fact]
    public void TryParse_WithEmptyString_ReturnsFalse()
    {
        bool result = Doi.TryParse(string.Empty, out Doi doi);

        Assert.False(result);
        Assert.Equal(string.Empty, doi.Identifier);
    }

    [Fact]
    public void TryParse_WithWhitespace_ReturnsFalse()
    {
        bool result = Doi.TryParse("   ", out Doi doi);

        Assert.False(result);
        Assert.Equal(string.Empty, doi.Identifier);
    }

    [Fact]
    public void ImplicitConversion_ToStringFromDoi_ReturnsIdentifier()
    {
        var doi = new Doi("10.1234/abcd.5678");
        string doiString = doi;

        Assert.Equal("10.1234/abcd.5678", doiString);
    }

    [Fact]
    public void ImplicitConversion_FromStringToDoi_CreatesDoiCorrectly()
    {
        Doi doi = "10.1234/abcd.5678";

        Assert.Equal("10.1234/abcd.5678", doi.Identifier);
        Assert.Equal("https://doi.org/10.1234/abcd.5678", doi.Url);
    }

    [Fact]
    public void Equals_WithSameDoi_ReturnsTrue()
    {
        var doi1 = new Doi("10.1234/abcd.5678");
        var doi2 = new Doi("10.1234/abcd.5678");

        Assert.True(doi1.Equals(doi2));
        Assert.True(doi1 == doi2);
        Assert.False(doi1 != doi2);
    }

    [Fact]
    public void Equals_WithDifferentFormats_ReturnsTrue()
    {
        var doi1 = new Doi("10.1234/abcd.5678");
        var doi2 = new Doi("https://doi.org/10.1234/abcd.5678");
        var doi3 = new Doi("doi:10.1234/abcd.5678");

        Assert.True(doi1.Equals(doi2));
        Assert.True(doi1.Equals(doi3));
        Assert.True(doi2.Equals(doi3));
    }

    [Fact]
    public void Equals_WithDifferentDois_ReturnsFalse()
    {
        var doi1 = new Doi("10.1234/abcd.5678");
        var doi2 = new Doi("10.5678/efgh.1234");

        Assert.False(doi1.Equals(doi2));
        Assert.False(doi1 == doi2);
        Assert.True(doi1 != doi2);
    }

    [Fact]
    public void Equals_CaseInsensitive_ReturnsTrue()
    {
        var doi1 = new Doi("10.1234/ABCD.5678");
        var doi2 = new Doi("10.1234/abcd.5678");

        Assert.True(doi1.Equals(doi2));
        Assert.True(doi1 == doi2);
    }

    [Fact]
    public void GetHashCode_WithSameDoi_ReturnsSameHashCode()
    {
        var doi1 = new Doi("10.1234/abcd.5678");
        var doi2 = new Doi("10.1234/abcd.5678");

        Assert.Equal(doi1.GetHashCode(), doi2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_WithDifferentFormats_ReturnsSameHashCode()
    {
        var doi1 = new Doi("10.1234/abcd.5678");
        var doi2 = new Doi("https://doi.org/10.1234/abcd.5678");

        Assert.Equal(doi1.GetHashCode(), doi2.GetHashCode());
    }

    [Fact]
    public void ToString_ReturnsIdentifier()
    {
        var doi = new Doi("10.1234/abcd.5678");

        Assert.Equal("10.1234/abcd.5678", doi.ToString());
    }

    [Fact]
    public void ToString_WithUrl_ReturnsIdentifier()
    {
        var doi = new Doi("https://doi.org/10.1234/abcd.5678");

        Assert.Equal("10.1234/abcd.5678", doi.ToString());
    }

    [Fact]
    public void Url_AlwaysReturnsHttps()
    {
        var doi1 = new Doi("10.1234/abcd.5678");
        var doi2 = new Doi("http://doi.org/10.1234/abcd.5678");
        var doi3 = new Doi("https://doi.org/10.1234/abcd.5678");

        Assert.Equal("https://doi.org/10.1234/abcd.5678", doi1.Url);
        Assert.Equal("https://doi.org/10.1234/abcd.5678", doi2.Url);
        Assert.Equal("https://doi.org/10.1234/abcd.5678", doi3.Url);
    }

    [Fact]
    public void RealWorldDoi_ACM_ParsesCorrectly()
    {
        var doi = new Doi("10.1145/3377811.3380330");

        Assert.Equal("10.1145/3377811.3380330", doi.Identifier);
        Assert.Equal("https://doi.org/10.1145/3377811.3380330", doi.Url);
    }

    [Fact]
    public void RealWorldDoi_IEEE_ParsesCorrectly()
    {
        var doi = new Doi("10.1109/5.771073");

        Assert.Equal("10.1109/5.771073", doi.Identifier);
        Assert.Equal("https://doi.org/10.1109/5.771073", doi.Url);
    }

    [Fact]
    public void RealWorldDoi_Nature_ParsesCorrectly()
    {
        var doi = new Doi("https://doi.org/10.1038/nature12373");

        Assert.Equal("10.1038/nature12373", doi.Identifier);
        Assert.Equal("https://doi.org/10.1038/nature12373", doi.Url);
    }

    [Fact]
    public void ComplexDoi_WithMultipleSlashes_ParsesCorrectly()
    {
        var doi = new Doi("10.1000/xyz123/abc/def");

        Assert.Equal("10.1000/xyz123/abc/def", doi.Identifier);
        Assert.Equal("https://doi.org/10.1000/xyz123/abc/def", doi.Url);
    }

    [Fact]
    public void DoiWithSpecialCharacters_ParsesCorrectly()
    {
        var doi = new Doi("10.1234/abc(def)ghi");

        Assert.Equal("10.1234/abc(def)ghi", doi.Identifier);
        Assert.Equal("https://doi.org/10.1234/abc(def)ghi", doi.Url);
    }

    [Fact]
    public void Default_Doi_HasEmptyValues()
    {
        Doi doi = default;

        Assert.Equal(string.Empty, doi.Identifier);
        Assert.Equal(string.Empty, doi.Url);
    }

    [Fact]
    public void Constructor_WithInvalidFormat_ThrowsArgumentException()
    {
        // DOI must start with "10."
        var exception = Assert.Throws<ArgumentException>(() => new Doi("invalid-doi"));
        Assert.Contains("Invalid DOI format", exception.Message);
    }

    [Fact]
    public void Constructor_WithInvalidPrefix_ThrowsArgumentException()
    {
        // DOI must start with "10." not "11."
        var exception = Assert.Throws<ArgumentException>(() => new Doi("11.1234/abcd"));
        Assert.Contains("Invalid DOI format", exception.Message);
    }

    [Fact]
    public void Constructor_WithTooFewDigits_ThrowsArgumentException()
    {
        // DOI must have at least 4 digits after "10."
        var exception = Assert.Throws<ArgumentException>(() => new Doi("10.123/abcd"));
        Assert.Contains("Invalid DOI format", exception.Message);
    }

    [Fact]
    public void Constructor_WithMissingSlash_ThrowsArgumentException()
    {
        // DOI must have a "/" after the numeric part
        var exception = Assert.Throws<ArgumentException>(() => new Doi("10.1234"));
        Assert.Contains("Invalid DOI format", exception.Message);
    }

    [Fact]
    public void Constructor_WithNothingAfterSlash_ThrowsArgumentException()
    {
        // DOI must have content after the "/"
        var exception = Assert.Throws<ArgumentException>(() => new Doi("10.1234/"));
        Assert.Contains("Invalid DOI format", exception.Message);
    }

    [Fact]
    public void TryParse_WithInvalidFormat_ReturnsFalse()
    {
        bool result = Doi.TryParse("invalid-doi", out Doi doi);

        Assert.False(result);
        Assert.Equal(string.Empty, doi.Identifier);
    }

    [Fact]
    public void TryParse_WithInvalidUrl_ReturnsFalse()
    {
        bool result = Doi.TryParse("https://doi.org/invalid", out Doi doi);

        Assert.False(result);
        Assert.Equal(string.Empty, doi.Identifier);
    }
}

