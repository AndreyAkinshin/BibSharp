using BibSharp.Models;

namespace BibSharp.Tests.Models;

public class ValidationResultComprehensiveTests
{
    [Fact]
    public void Constructor_Default_CreatesValidResult()
    {
        var result = new ValidationResult();
        
        Assert.True(result.IsValid);
        Assert.False(result.HasErrors);
        Assert.False(result.HasWarnings);
        Assert.Empty(result.Errors);
        Assert.Empty(result.Warnings);
    }

    [Fact]
    public void Constructor_WithError_CreatesInvalidResult()
    {
        var result = new ValidationResult("Test error");
        
        Assert.False(result.IsValid);
        Assert.True(result.HasErrors);
        Assert.False(result.HasWarnings);
        Assert.Single(result.Errors);
        Assert.Equal("Test error", result.Errors[0]);
    }

    [Fact]
    public void Constructor_WithErrors_CreatesInvalidResult()
    {
        var errors = new[] { "Error 1", "Error 2" };
        var result = new ValidationResult(errors);
        
        Assert.False(result.IsValid);
        Assert.True(result.HasErrors);
        Assert.Equal(2, result.Errors.Count);
        Assert.Equal("Error 1", result.Errors[0]);
        Assert.Equal("Error 2", result.Errors[1]);
    }

    [Fact]
    public void AddError_AddsToErrorsList()
    {
        var result = new ValidationResult();
        result.AddError("Error 1");
        
        Assert.False(result.IsValid);
        Assert.True(result.HasErrors);
        Assert.Single(result.Errors);
    }

    [Fact]
    public void AddError_ReturnsThis_ForChaining()
    {
        var result = new ValidationResult();
        var returned = result.AddError("Error 1");
        
        Assert.Same(result, returned);
    }

    [Fact]
    public void AddErrors_AddsMultiple()
    {
        var result = new ValidationResult();
        result.AddErrors(new[] { "Error 1", "Error 2", "Error 3" });
        
        Assert.Equal(3, result.Errors.Count);
    }

    [Fact]
    public void AddErrors_ReturnsThis_ForChaining()
    {
        var result = new ValidationResult();
        var returned = result.AddErrors(new[] { "Error 1" });
        
        Assert.Same(result, returned);
    }

    [Fact]
    public void AddWarning_AddsToWarningsList()
    {
        var result = new ValidationResult();
        result.AddWarning("Warning 1");
        
        Assert.True(result.IsValid); // Still valid, just has warning
        Assert.False(result.HasErrors);
        Assert.True(result.HasWarnings);
        Assert.Single(result.Warnings);
    }

    [Fact]
    public void AddWarning_ReturnsThis_ForChaining()
    {
        var result = new ValidationResult();
        var returned = result.AddWarning("Warning 1");
        
        Assert.Same(result, returned);
    }

    [Fact]
    public void AddWarnings_AddsMultiple()
    {
        var result = new ValidationResult();
        result.AddWarnings(new[] { "Warning 1", "Warning 2" });
        
        Assert.Equal(2, result.Warnings.Count);
    }

    [Fact]
    public void AddWarnings_ReturnsThis_ForChaining()
    {
        var result = new ValidationResult();
        var returned = result.AddWarnings(new[] { "Warning 1" });
        
        Assert.Same(result, returned);
    }

    [Fact]
    public void HasWarnings_WithErrorsAndWarnings_ReturnsTrue()
    {
        var result = new ValidationResult();
        result.AddError("Error 1");
        result.AddWarning("Warning 1");
        
        // HasWarnings returns true if there are any warnings, regardless of errors
        Assert.True(result.HasWarnings);
        Assert.True(result.HasErrors);
    }

    [Fact]
    public void HasWarnings_WithOnlyWarnings_ReturnsTrue()
    {
        var result = new ValidationResult();
        result.AddWarning("Warning 1");
        
        Assert.True(result.HasWarnings);
        Assert.False(result.HasErrors);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Merge_CombinesErrors()
    {
        var result1 = new ValidationResult("Error 1");
        var result2 = new ValidationResult("Error 2");
        
        result1.Merge(result2);
        
        Assert.Equal(2, result1.Errors.Count);
        Assert.Contains("Error 1", result1.Errors);
        Assert.Contains("Error 2", result1.Errors);
    }

    [Fact]
    public void Merge_CombinesWarnings()
    {
        var result1 = new ValidationResult();
        result1.AddWarning("Warning 1");
        
        var result2 = new ValidationResult();
        result2.AddWarning("Warning 2");
        
        result1.Merge(result2);
        
        Assert.Equal(2, result1.Warnings.Count);
        Assert.Contains("Warning 1", result1.Warnings);
        Assert.Contains("Warning 2", result1.Warnings);
    }

    [Fact]
    public void Merge_ReturnsThis_ForChaining()
    {
        var result1 = new ValidationResult();
        var result2 = new ValidationResult("Error");
        
        var returned = result1.Merge(result2);
        
        Assert.Same(result1, returned);
    }

    [Fact]
    public void Valid_CreatesValidResult()
    {
        var result = ValidationResult.Valid();
        
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
        Assert.Empty(result.Warnings);
    }

    [Fact]
    public void Invalid_WithError_CreatesInvalidResult()
    {
        var result = ValidationResult.Invalid("Test error");
        
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal("Test error", result.Errors[0]);
    }

    [Fact]
    public void Invalid_WithErrors_CreatesInvalidResult()
    {
        var errors = new[] { "Error 1", "Error 2" };
        var result = ValidationResult.Invalid(errors);
        
        Assert.False(result.IsValid);
        Assert.Equal(2, result.Errors.Count);
    }

    [Fact]
    public void MethodChaining_WorksCorrectly()
    {
        var result = new ValidationResult()
            .AddError("Error 1")
            .AddError("Error 2")
            .AddWarning("Warning 1")
            .AddWarning("Warning 2");
        
        Assert.Equal(2, result.Errors.Count);
        Assert.Equal(2, result.Warnings.Count);
    }

    [Fact]
    public void Merge_Multiple_WorksCorrectly()
    {
        var result = new ValidationResult()
            .AddError("Error 1")
            .Merge(new ValidationResult("Error 2"))
            .Merge(new ValidationResult().AddWarning("Warning 1"));
        
        Assert.Equal(2, result.Errors.Count);
        Assert.Single(result.Warnings);
    }
}

