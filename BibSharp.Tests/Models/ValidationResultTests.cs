using BibSharp.Models;

namespace BibSharp.Tests.Models;

public class ValidationResultTests
{
    [Fact]
    public void Constructor_WithNoParameters_CreatesValidResult()
    {
        var result = new ValidationResult();
        
        Assert.True(result.IsValid);
        Assert.False(result.HasErrors);
        Assert.False(result.HasWarnings);
        Assert.Empty(result.Errors);
        Assert.Empty(result.Warnings);
    }
    
    [Fact]
    public void Constructor_WithErrorString_CreatesInvalidResult()
    {
        var result = new ValidationResult("Test error");
        
        Assert.False(result.IsValid);
        Assert.True(result.HasErrors);
        Assert.False(result.HasWarnings);
        Assert.Single(result.Errors);
        Assert.Equal("Test error", result.Errors[0]);
    }
    
    [Fact]
    public void Constructor_WithErrorCollection_CreatesInvalidResult()
    {
        var errors = new[] { "Error 1", "Error 2" };
        var result = new ValidationResult(errors);
        
        Assert.False(result.IsValid);
        Assert.True(result.HasErrors);
        Assert.False(result.HasWarnings);
        Assert.Equal(2, result.Errors.Count);
        Assert.Contains("Error 1", result.Errors);
        Assert.Contains("Error 2", result.Errors);
    }
    
    [Fact]
    public void AddError_AppendsError()
    {
        var result = new ValidationResult();
        result.AddError("Test error");
        
        Assert.False(result.IsValid);
        Assert.True(result.HasErrors);
        Assert.Single(result.Errors);
        Assert.Equal("Test error", result.Errors[0]);
    }
    
    [Fact]
    public void AddErrors_AppendsMultipleErrors()
    {
        var result = new ValidationResult();
        result.AddErrors(new[] { "Error 1", "Error 2" });
        
        Assert.False(result.IsValid);
        Assert.True(result.HasErrors);
        Assert.Equal(2, result.Errors.Count);
        Assert.Contains("Error 1", result.Errors);
        Assert.Contains("Error 2", result.Errors);
    }
    
    [Fact]
    public void AddWarning_AppendsWarning()
    {
        var result = new ValidationResult();
        result.AddWarning("Test warning");
        
        // IsValid should be true - warnings don't make an entry invalid, only errors do
        Assert.True(result.IsValid);
        Assert.False(result.HasErrors);
        Assert.True(result.HasWarnings);
        Assert.Single(result.Warnings);
        Assert.Equal("Test warning", result.Warnings[0]);
    }
    
    [Fact]
    public void AddWarnings_AppendsMultipleWarnings()
    {
        var result = new ValidationResult();
        result.AddWarnings(new[] { "Warning 1", "Warning 2" });
        
        // IsValid should be true - warnings don't make an entry invalid, only errors do
        Assert.True(result.IsValid);
        Assert.False(result.HasErrors);
        Assert.True(result.HasWarnings);
        Assert.Equal(2, result.Warnings.Count);
        Assert.Contains("Warning 1", result.Warnings);
        Assert.Contains("Warning 2", result.Warnings);
    }
    
    [Fact]
    public void HasErrorsAndWarnings_ReportsBoth()
    {
        var result = new ValidationResult();
        result.AddError("Test error");
        result.AddWarning("Test warning");
        
        Assert.False(result.IsValid);
        Assert.True(result.HasErrors);
        Assert.True(result.HasWarnings);  // HasWarnings is true when there are warnings
    }
    
    [Fact]
    public void Merge_CombinesResults()
    {
        var result1 = new ValidationResult("Error 1");
        var result2 = new ValidationResult();
        result2.AddError("Error 2");
        result2.AddWarning("Warning 1");
        
        result1.Merge(result2);
        
        Assert.False(result1.IsValid);
        Assert.True(result1.HasErrors);
        Assert.Equal(2, result1.Errors.Count);
        Assert.Single(result1.Warnings);
        Assert.Contains("Error 1", result1.Errors);
        Assert.Contains("Error 2", result1.Errors);
        Assert.Contains("Warning 1", result1.Warnings);
    }
    
    [Fact]
    public void Valid_CreatesValidResult()
    {
        var result = ValidationResult.Valid();
        
        Assert.True(result.IsValid);
        Assert.False(result.HasErrors);
        Assert.False(result.HasWarnings);
    }
    
    [Fact]
    public void Invalid_WithErrorString_CreatesInvalidResult()
    {
        var result = ValidationResult.Invalid("Test error");
        
        Assert.False(result.IsValid);
        Assert.True(result.HasErrors);
        Assert.Single(result.Errors);
        Assert.Equal("Test error", result.Errors[0]);
    }
    
    [Fact]
    public void Invalid_WithErrorCollection_CreatesInvalidResult()
    {
        var errors = new[] { "Error 1", "Error 2" };
        var result = ValidationResult.Invalid(errors);
        
        Assert.False(result.IsValid);
        Assert.True(result.HasErrors);
        Assert.Equal(2, result.Errors.Count);
        Assert.Contains("Error 1", result.Errors);
        Assert.Contains("Error 2", result.Errors);
    }
    
    [Fact]
    public void MethodChaining_Works()
    {
        var result = new ValidationResult()
            .AddError("Error 1")
            .AddWarning("Warning 1")
            .AddErrors(new[] { "Error 2" })
            .AddWarnings(new[] { "Warning 2" });
        
        Assert.False(result.IsValid);
        Assert.True(result.HasErrors);
        Assert.Equal(2, result.Errors.Count);
        Assert.Equal(2, result.Warnings.Count);
    }
}