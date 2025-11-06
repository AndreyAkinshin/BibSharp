using BibSharp.Utilities;

namespace BibSharp.Tests.Utilities;

public class DoiResolverTests
{
    [Fact(Skip = "External API call - run manually")]
    public async Task ResolveDoiAsync_WithValidDoi_ReturnsBibEntry()
    {
        // ACM paper - System FR: formalized foundations for the stainless verifier
        var resolver = new DoiResolver();
        var entry = await resolver.ResolveDoiAsync("10.1145/3360592");
        
        Assert.NotNull(entry);
        Assert.NotNull(entry.Title);
        Assert.NotEmpty(entry.Authors);
        Assert.NotNull(entry.Doi);
        Assert.Equal("10.1145/3360592", entry.Doi);
    }
    
    [Fact(Skip = "External API call - run manually")]
    public async Task FromDoiAsync_WithValidDoi_ReturnsBibEntry()
    {
        // IEEE paper
        var entry = await BibEntry.FromDoiAsync("10.1109/5.771073");
        
        Assert.NotNull(entry);
        Assert.NotNull(entry.Title);
        Assert.NotEmpty(entry.Authors);
        Assert.NotNull(entry.Doi);
        Assert.Equal("10.1109/5.771073", entry.Doi);
    }
    
    [Fact(Skip = "External API call - run manually")]
    public async Task ResolveDoiAsync_WithUrlFormattedDoi_ReturnsBibEntry()
    {
        // Same DOI but with URL format
        var resolver = new DoiResolver();
        var entry = await resolver.ResolveDoiAsync("https://doi.org/10.1145/3360592");
        
        Assert.NotNull(entry);
        Assert.NotNull(entry.Title);
        Assert.NotEmpty(entry.Authors);
        Assert.NotNull(entry.Doi);
        Assert.Equal("10.1145/3360592", entry.Doi);
    }
    
    [Fact]
    public async Task ResolveDoiAsync_WithInvalidDoi_ReturnsNull()
    {
        // Invalid DOI
        var resolver = new DoiResolver();
        var entry = await resolver.ResolveDoiAsync("10.0000/invalid-doi-xyz-123");
        
        Assert.Null(entry);
    }
    
    [Fact]
    public async Task ResolveDoiAsync_WithEmptyDoi_ThrowsException()
    {
        // Empty DOI should throw
        var resolver = new DoiResolver();
        await Assert.ThrowsAsync<ArgumentException>(() => resolver.ResolveDoiAsync(""));
    }
}