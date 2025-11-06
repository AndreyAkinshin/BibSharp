using BibSharp.Utilities;

namespace BibSharp.Tests.Utilities;

public class DoiResolverComprehensiveTests
{
    [Fact]
    public void Constructor_WithZeroTimeout_ThrowsException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new DoiResolver(0, 100));
    }

    [Fact]
    public void Constructor_WithNegativeTimeout_ThrowsException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new DoiResolver(-1, 100));
    }

    [Fact]
    public void Constructor_WithValidTimeout_Succeeds()
    {
        var resolver = new DoiResolver(60, 100);
        Assert.NotNull(resolver);
    }
    
    [Fact]
    public void Constructor_WithTimeoutTooHigh_ThrowsException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new DoiResolver(301, 100));
    }

    [Fact]
    public void Constructor_WithNegativeDelay_ThrowsException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new DoiResolver(30, -1));
    }
    
    [Fact]
    public void Constructor_WithDelayTooHigh_ThrowsException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new DoiResolver(30, 10001));
    }

    [Fact]
    public void Constructor_WithZeroDelay_Succeeds()
    {
        var resolver = new DoiResolver(30, 0);
        Assert.NotNull(resolver);
    }

    [Fact]
    public void Constructor_WithValidDelay_Succeeds()
    {
        var resolver = new DoiResolver(30, 500);
        Assert.NotNull(resolver);
    }
    
    [Fact]
    public void Constructor_Default_Succeeds()
    {
        var resolver = new DoiResolver();
        Assert.NotNull(resolver);
    }

    [Fact]
    public async Task ResolveDoiAsync_NullDoi_ThrowsException()
    {
        var resolver = new DoiResolver();
        await Assert.ThrowsAsync<ArgumentException>(async () => 
            await resolver.ResolveDoiAsync(null!));
    }

    [Fact]
    public async Task ResolveDoiAsync_EmptyDoi_ThrowsException()
    {
        var resolver = new DoiResolver();
        await Assert.ThrowsAsync<ArgumentException>(async () => 
            await resolver.ResolveDoiAsync(""));
    }

    [Fact]
    public async Task ResolveDoiAsync_WhitespaceDoi_ThrowsException()
    {
        var resolver = new DoiResolver();
        await Assert.ThrowsAsync<ArgumentException>(async () => 
            await resolver.ResolveDoiAsync("   "));
    }

}

