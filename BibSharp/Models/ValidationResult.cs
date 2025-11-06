using System;
using System.Collections.Generic;
using System.Linq;

namespace BibSharp.Models;

/// <summary>
/// Represents the result of validating a BibTeX entry.
/// </summary>
public class ValidationResult : IEquatable<ValidationResult>
{
    /// <summary>
    /// Gets a value indicating whether the entry is valid (has no errors).
    /// An entry can be valid but still have warnings.
    /// </summary>
    public bool IsValid => Errors.Count == 0;
    
    /// <summary>
    /// Gets a value indicating whether the entry has errors (but might have warnings).
    /// </summary>
    public bool HasErrors => Errors.Count > 0;
    
    /// <summary>
    /// Gets a value indicating whether the entry has warnings.
    /// </summary>
    public bool HasWarnings => Warnings.Count > 0;
    
    /// <summary>
    /// Gets the list of validation errors.
    /// </summary>
    public List<string> Errors { get; } = new List<string>();
    
    /// <summary>
    /// Gets the list of validation warnings.
    /// </summary>
    public List<string> Warnings { get; } = new List<string>();

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationResult"/> class.
    /// </summary>
    public ValidationResult()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationResult"/> class with an error.
    /// </summary>
    /// <param name="error">The validation error.</param>
    public ValidationResult(string error)
    {
        Errors.Add(error);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationResult"/> class with a collection of errors.
    /// </summary>
    /// <param name="errors">The collection of validation errors.</param>
    public ValidationResult(IEnumerable<string> errors)
    {
        Errors.AddRange(errors);
    }

    /// <summary>
    /// Adds an error to the validation result.
    /// </summary>
    /// <param name="error">The error to add.</param>
    /// <returns>This <see cref="ValidationResult"/> instance to enable method chaining.</returns>
    public ValidationResult AddError(string error)
    {
        Errors.Add(error);
        return this;
    }

    /// <summary>
    /// Adds errors to the validation result.
    /// </summary>
    /// <param name="errors">The errors to add.</param>
    /// <returns>This <see cref="ValidationResult"/> instance to enable method chaining.</returns>
    public ValidationResult AddErrors(IEnumerable<string> errors)
    {
        Errors.AddRange(errors);
        return this;
    }

    /// <summary>
    /// Adds a warning to the validation result.
    /// </summary>
    /// <param name="warning">The warning to add.</param>
    /// <returns>This <see cref="ValidationResult"/> instance to enable method chaining.</returns>
    public ValidationResult AddWarning(string warning)
    {
        Warnings.Add(warning);
        return this;
    }

    /// <summary>
    /// Adds warnings to the validation result.
    /// </summary>
    /// <param name="warnings">The warnings to add.</param>
    /// <returns>This <see cref="ValidationResult"/> instance to enable method chaining.</returns>
    public ValidationResult AddWarnings(IEnumerable<string> warnings)
    {
        Warnings.AddRange(warnings);
        return this;
    }

    /// <summary>
    /// Merges another validation result into this one.
    /// </summary>
    /// <param name="other">The other validation result to merge.</param>
    /// <returns>This <see cref="ValidationResult"/> instance to enable method chaining.</returns>
    public ValidationResult Merge(ValidationResult other)
    {
        Errors.AddRange(other.Errors);
        Warnings.AddRange(other.Warnings);
        return this;
    }

    /// <summary>
    /// Creates a new valid validation result.
    /// </summary>
    /// <returns>A new valid <see cref="ValidationResult"/> instance.</returns>
    public static ValidationResult Valid()
    {
        return new ValidationResult();
    }

    /// <summary>
    /// Creates a new validation result with an error.
    /// </summary>
    /// <param name="error">The validation error.</param>
    /// <returns>A new <see cref="ValidationResult"/> instance with the error.</returns>
    public static ValidationResult Invalid(string error)
    {
        return new ValidationResult(error);
    }

    /// <summary>
    /// Creates a new validation result with errors.
    /// </summary>
    /// <param name="errors">The validation errors.</param>
    /// <returns>A new <see cref="ValidationResult"/> instance with the errors.</returns>
    public static ValidationResult Invalid(IEnumerable<string> errors)
    {
        return new ValidationResult(errors);
    }
    
    /// <summary>
    /// Determines whether the specified ValidationResult is equal to the current ValidationResult.
    /// </summary>
    /// <param name="other">The ValidationResult to compare with the current object.</param>
    /// <returns>true if the validation results have the same errors and warnings; otherwise, false.</returns>
    public bool Equals(ValidationResult? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Errors.SequenceEqual(other.Errors) && Warnings.SequenceEqual(other.Warnings);
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
    public override bool Equals(object? obj)
    {
        return obj is ValidationResult other && Equals(other);
    }

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>A hash code for this instance.</returns>
    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            foreach (var error in Errors)
            {
                hash = hash * 31 + (error?.GetHashCode() ?? 0);
            }
            foreach (var warning in Warnings)
            {
                hash = hash * 31 + (warning?.GetHashCode() ?? 0);
            }
            return hash;
        }
    }
}