using System;
using System.Text.RegularExpressions;

namespace BibSharp.Models;

/// <summary>
/// Represents a range of pages in a BibTeX entry.
/// </summary>
public class PageRange : IEquatable<PageRange>
{
    private static readonly Regex multipleDashesRegex = new Regex("-+", RegexOptions.Compiled, TimeSpan.FromMilliseconds(1000));

    /// <summary>
    /// Gets or sets the start page of the range.
    /// </summary>
    public int StartPage { get; set; }
    
    /// <summary>
    /// Gets or sets the end page of the range.
    /// </summary>
    public int? EndPage { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PageRange"/> class with a single page.
    /// </summary>
    /// <param name="startPage">The page number.</param>
    /// <exception cref="ArgumentException">Thrown when startPage is negative.</exception>
    public PageRange(int startPage)
    {
        if (startPage < 0)
        {
            throw new ArgumentException($"Start page ({startPage}) cannot be negative", nameof(startPage));
        }
        
        StartPage = startPage;
        EndPage = null;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PageRange"/> class with a range of pages.
    /// </summary>
    /// <param name="startPage">The start page of the range.</param>
    /// <param name="endPage">The end page of the range.</param>
    /// <exception cref="ArgumentException">Thrown when endPage is less than startPage or when page numbers are negative.</exception>
    public PageRange(int startPage, int endPage)
    {
        if (startPage < 0)
        {
            throw new ArgumentException($"Start page ({startPage}) cannot be negative", nameof(startPage));
        }
        
        if (endPage < 0)
        {
            throw new ArgumentException($"End page ({endPage}) cannot be negative", nameof(endPage));
        }
        
        if (endPage < startPage)
        {
            throw new ArgumentException(
                $"End page ({endPage}) cannot be less than start page ({startPage}). " +
                $"Did you mean PageRange({endPage}, {startPage})?", 
                nameof(endPage));
        }
        
        StartPage = startPage;
        EndPage = endPage;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PageRange"/> class from a string representation.
    /// </summary>
    /// <param name="pageRangeString">The string representation of the page range (e.g., "123", "123-145", "123--145").</param>
    /// <exception cref="ArgumentException">Thrown when the page range string is invalid or contains non-numeric pages.</exception>
    /// <remarks>
    /// This constructor accepts various dash formats (-, --, ---) and normalizes them to a single dash.
    /// It expects either a single page number or a range in the format "start-end".
    /// </remarks>
    public PageRange(string pageRangeString)
    {
        if (string.IsNullOrWhiteSpace(pageRangeString))
        {
            throw new ArgumentException("Page range string cannot be null or empty", nameof(pageRangeString));
        }

        // Clean up the input string (trim whitespace, replace multiple consecutive dashes with a single one)
        string cleanedRange = multipleDashesRegex.Replace(pageRangeString.Trim(), "-");
        
        // Split the range by the dash (this handles cases like "123-145")
        // Note: If the input was "123-125-127", after normalization it becomes "123-125-127" -> splits to ["123", "125", "127"]
        // We only use the first two parts (start and end), treating additional parts as invalid
        string[] parts = cleanedRange.Split('-', StringSplitOptions.RemoveEmptyEntries);
        
        // Parse the start page
        if (parts.Length == 0)
        {
            throw new ArgumentException(
                $"Invalid page range: '{pageRangeString}'. No valid page numbers found.", 
                nameof(pageRangeString));
        }
        
        if (!int.TryParse(parts[0], out int start))
        {
            throw new ArgumentException(
                $"Invalid start page: '{parts[0]}'. Page numbers must be numeric.", 
                nameof(pageRangeString));
        }
        
        if (start < 0)
        {
            throw new ArgumentException(
                $"Invalid start page: {start}. Page numbers cannot be negative.", 
                nameof(pageRangeString));
        }
        
        StartPage = start;
        
        // If there's an end page, parse it
        if (parts.Length > 1)
        {
            if (!int.TryParse(parts[1], out int end))
            {
                throw new ArgumentException(
                    $"Invalid end page: '{parts[1]}'. Page numbers must be numeric.", 
                    nameof(pageRangeString));
            }
            
            if (end < 0)
            {
                throw new ArgumentException(
                    $"Invalid end page: {end}. Page numbers cannot be negative.", 
                    nameof(pageRangeString));
            }
            
            if (end < start)
            {
                throw new ArgumentException(
                    $"End page ({end}) cannot be less than start page ({start}) in range '{pageRangeString}'. " +
                    $"Did you mean '{end}--{start}'?", 
                    nameof(pageRangeString));
            }
            
            EndPage = end;
        }
        else
        {
            EndPage = null;
        }
    }

    /// <summary>
    /// Returns a string that represents the current object in BibTeX format.
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString()
    {
        return EndPage.HasValue ? $"{StartPage}--{EndPage}" : StartPage.ToString();
    }

    /// <summary>
    /// Returns the total number of pages in the range.
    /// </summary>
    /// <returns>The number of pages or 1 if only a single page is specified.</returns>
    public int GetPageCount()
    {
        return EndPage.HasValue ? EndPage.Value - StartPage + 1 : 1;
    }

    /// <summary>
    /// Determines whether the specified PageRange is equal to the current PageRange.
    /// </summary>
    /// <param name="other">The PageRange to compare with the current object.</param>
    /// <returns>true if the specified PageRange is equal to the current PageRange; otherwise, false.</returns>
    public bool Equals(PageRange? other)
    {
        if (other is null)
        {
            return false;
        }

        return StartPage == other.StartPage && EndPage == other.EndPage;
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
    public override bool Equals(object? obj)
    {
        return obj is PageRange other && Equals(other);
    }

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>A hash code for this instance.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(StartPage, EndPage);
    }
    
    /// <summary>Determines whether two PageRange instances are equal.</summary>
    public static bool operator ==(PageRange? left, PageRange? right)
    {
        if (left is null)
        {
            return right is null;
        }
        return left.Equals(right);
    }
    
    /// <summary>Determines whether two PageRange instances are not equal.</summary>
    public static bool operator !=(PageRange? left, PageRange? right)
    {
        return !(left == right);
    }
}