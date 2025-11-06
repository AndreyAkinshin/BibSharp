using BibSharp.Exceptions;

namespace BibSharp.Tests;

public class ExceptionTests
{
    [Fact]
    public void BibParseException_DefaultConstructor()
    {
        var ex = new BibParseException();
        Assert.NotNull(ex);
        Assert.Equal(0, ex.LineNumber);
        Assert.Equal(0, ex.ColumnNumber);
        Assert.Null(ex.SourceText);
    }

    [Fact]
    public void BibParseException_MessageConstructor()
    {
        var ex = new BibParseException("Test error");
        Assert.Equal("Test error", ex.Message);
        Assert.Equal(0, ex.LineNumber);
        Assert.Equal(0, ex.ColumnNumber);
        Assert.Null(ex.SourceText);
    }

    [Fact]
    public void BibParseException_MessageWithLocation()
    {
        var ex = new BibParseException("Test error", 10, 5);
        Assert.Contains("Test error", ex.Message);
        Assert.Contains("line 10", ex.Message);
        Assert.Contains("column 5", ex.Message);
        Assert.Equal(10, ex.LineNumber);
        Assert.Equal(5, ex.ColumnNumber);
        Assert.Null(ex.SourceText);
    }

    [Fact]
    public void BibParseException_MessageWithLocationAndSource()
    {
        var ex = new BibParseException("Test error", 10, 5, "@article{bad");
        Assert.Contains("Test error", ex.Message);
        Assert.Contains("line 10", ex.Message);
        Assert.Contains("column 5", ex.Message);
        Assert.Contains("@article{bad", ex.Message);
        Assert.Equal(10, ex.LineNumber);
        Assert.Equal(5, ex.ColumnNumber);
        Assert.Equal("@article{bad", ex.SourceText);
    }

    [Fact]
    public void BibParseException_WithInnerException()
    {
        var innerEx = new InvalidOperationException("Inner error");
        var ex = new BibParseException("Test error", innerEx);
        Assert.Equal("Test error", ex.Message);
        Assert.Same(innerEx, ex.InnerException);
        Assert.Equal(0, ex.LineNumber);
        Assert.Equal(0, ex.ColumnNumber);
    }

    [Fact]
    public void BibValidationException_MessageConstructor()
    {
        var ex = new BibValidationException("Validation failed");
        Assert.Equal("Validation failed", ex.Message);
        Assert.Null(ex.EntryKey);
        Assert.Single(ex.ValidationErrors);
        Assert.Equal("Validation failed", ex.ValidationErrors[0]);
    }

    [Fact]
    public void BibValidationException_MessageWithKey()
    {
        var ex = new BibValidationException("Validation failed", "test2025");
        Assert.Contains("Validation failed", ex.Message);
        Assert.Contains("test2025", ex.Message);
        Assert.Equal("test2025", ex.EntryKey);
        Assert.Single(ex.ValidationErrors);
        Assert.Equal("Validation failed", ex.ValidationErrors[0]);
    }

    [Fact]
    public void BibValidationException_MessageWithKeyAndErrors()
    {
        var errors = new List<string>
        {
            "Missing author field",
            "Missing title field"
        };
        var ex = new BibValidationException("Validation failed", "test2025", errors);
        
        Assert.Contains("Validation failed", ex.Message);
        Assert.Contains("test2025", ex.Message);
        Assert.Contains("Missing author field", ex.Message);
        Assert.Contains("Missing title field", ex.Message);
        Assert.Equal("test2025", ex.EntryKey);
        Assert.Equal(2, ex.ValidationErrors.Count);
        Assert.Equal("Missing author field", ex.ValidationErrors[0]);
        Assert.Equal("Missing title field", ex.ValidationErrors[1]);
    }

    [Fact]
    public void BibValidationException_EmptyErrorsList()
    {
        var errors = new List<string>();
        var ex = new BibValidationException("Validation failed", "test2025", errors);
        
        Assert.Equal("test2025", ex.EntryKey);
        Assert.Empty(ex.ValidationErrors);
    }

    [Fact]
    public void BibValidationException_CanCatchAsException()
    {
        try
        {
            throw new BibValidationException("Test");
        }
        catch (Exception ex)
        {
            Assert.IsType<BibValidationException>(ex);
        }
    }

    [Fact]
    public void BibParseException_CanCatchAsException()
    {
        try
        {
            throw new BibParseException("Test");
        }
        catch (Exception ex)
        {
            Assert.IsType<BibParseException>(ex);
        }
    }
}

