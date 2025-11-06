using System;
using System.Collections.Generic;
using System.Linq;

namespace BibSharp.Exceptions;

/// <summary>
/// Exception thrown when a BibTeX entry fails validation.
/// </summary>
public class BibValidationException : Exception
{
    /// <summary>
    /// Gets the key of the entry that failed validation.
    /// </summary>
    public string? EntryKey { get; }
    
    /// <summary>
    /// Gets the list of validation errors.
    /// </summary>
    public List<string> ValidationErrors { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BibValidationException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public BibValidationException(string message) : base(message)
    {
        ValidationErrors = new List<string> { message };
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BibValidationException"/> class with entry key.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="entryKey">The key of the entry that failed validation.</param>
    public BibValidationException(string message, string entryKey) 
        : base($"{message} (Entry: {entryKey})")
    {
        EntryKey = entryKey;
        ValidationErrors = new List<string> { message };
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BibValidationException"/> class with multiple validation errors.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="entryKey">The key of the entry that failed validation.</param>
    /// <param name="validationErrors">The list of validation errors.</param>
    public BibValidationException(string message, string entryKey, List<string> validationErrors) 
        : base(FormatMessage(message, entryKey, validationErrors))
    {
        EntryKey = entryKey;
        ValidationErrors = validationErrors;
    }

    private static string FormatMessage(string message, string entryKey, List<string> validationErrors)
    {
        return $"{message} (Entry: {entryKey})\n" +
               string.Join("\n", validationErrors.Select(e => $"- {e}"));
    }
}