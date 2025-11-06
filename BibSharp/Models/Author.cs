using System;
using System.Collections.Generic;

namespace BibSharp.Models;

/// <summary>
/// Represents an author with first and last name.
/// </summary>
public class Author
{
    /// <summary>
    /// Gets or sets the last name of the author.
    /// </summary>
    public string LastName { get; set; }

    /// <summary>
    /// Gets or sets the first name of the author.
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// Gets or sets the middle name of the author.
    /// </summary>
    public string? MiddleName { get; set; }

    /// <summary>
    /// Gets or sets the suffix of the author (e.g., "Jr.", "Sr.", "III").
    /// </summary>
    public string? Suffix { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Author"/> class.
    /// </summary>
    /// <param name="lastName">The last name of the author.</param>
    /// <param name="firstName">The first name of the author.</param>
    /// <param name="middleName">The middle name of the author.</param>
    /// <param name="suffix">The suffix of the author.</param>
    /// <exception cref="ArgumentException">Thrown when lastName is null, empty, or whitespace.</exception>
    public Author(string lastName, string? firstName = null, string? middleName = null, string? suffix = null)
    {
        if (string.IsNullOrWhiteSpace(lastName))
        {
            throw new ArgumentException("Author last name cannot be null, empty, or whitespace", nameof(lastName));
        }
        
        LastName = lastName;
        FirstName = firstName;
        MiddleName = middleName;
        Suffix = suffix;
    }

    /// <summary>
    /// Creates an author from a formatted string in "LastName, FirstName" or "FirstName LastName" format.
    /// </summary>
    /// <param name="formattedName">The formatted name string.</param>
    /// <returns>A new <see cref="Author"/> instance.</returns>
    public static Author FromFormattedString(string formattedName)
    {
        if (string.IsNullOrWhiteSpace(formattedName))
        {
            throw new ArgumentException("Author name cannot be empty", nameof(formattedName));
        }

        formattedName = formattedName.Trim();
        
        // Case 1: "LastName, FirstName MiddleName" format
        if (formattedName.Contains(',', StringComparison.Ordinal))
        {
            return ParseLastNameFirstFormat(formattedName);
        }
        
        // Case 2: "FirstName MiddleName LastName" format
        return ParseFirstNameFirstFormat(formattedName);
    }
    
    private static Author ParseLastNameFirstFormat(string formattedName)
    {
        string[] parts = formattedName.Split(',', 2);
        
        string lastName = parts[0].Trim();
        if (parts.Length == 1 || string.IsNullOrWhiteSpace(parts[1]))
        {
            return new Author(lastName);
        }

        string[] firstParts = parts[1].Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        
        if (firstParts.Length == 0)
        {
            return new Author(lastName);
        }
        
        if (firstParts.Length == 1)
        {
            return new Author(lastName, firstParts[0]);
        }
        
        // Handle suffix like "Jr." or "III"
        string? suffix = ExtractSuffix(ref firstParts);

        string firstName = firstParts[0];
        string? middleName = null;
        if (firstParts.Length > 1)
        {
            var middleNameParts = new List<string>();
            for (int i = 1; i < firstParts.Length; i++)
            {
                middleNameParts.Add(firstParts[i]);
            }
            middleName = string.Join(" ", middleNameParts);
        }

        return new Author(lastName, firstName, middleName, suffix);
    }
    
    private static Author ParseFirstNameFirstFormat(string formattedName)
    {
        // Split the name into parts
        string[] parts = formattedName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        
        if (parts.Length == 0)
        {
            throw new ArgumentException("Author name cannot be empty", nameof(formattedName));
        }
        
        // If there's only one part, assume it's the last name
        if (parts.Length == 1)
        {
            return new Author(parts[0]);
        }
        
        // Check for suffix like "Jr." or "III"
        string? suffix = ExtractSuffix(ref parts);
        
        // The last remaining part is the last name
        string lastName = parts[parts.Length - 1];
        
        // If there's only one part left after suffix removal, it must be the last name
        if (parts.Length == 1)
        {
            return new Author(lastName, null, null, suffix);
        }
        
        // The first part is the first name
        string firstName = parts[0];
        
        // If there are parts between first and last name, they form the middle name
        string? middleName = null;
        if (parts.Length > 2)
        {
            var middleNameParts = new List<string>();
            for (int i = 1; i < parts.Length - 1; i++)
            {
                middleNameParts.Add(parts[i]);
            }
            middleName = string.Join(" ", middleNameParts);
        }
        
        return new Author(lastName, firstName, middleName, suffix);
    }
    
    /// <summary>
    /// Set of recognized author suffixes that will be automatically detected and normalized during parsing.
    /// Suffixes not in this set will be accepted but not normalized.
    /// </summary>
    /// <remarks>
    /// Includes generational suffixes (Jr., Sr., II, III, etc.) and professional/academic titles (Ph.D., M.D., etc.).
    /// </remarks>
    private static readonly HashSet<string> recognizedSuffixes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        // Generational
        "Jr", "Jr.", "Junior",
        "Sr", "Sr.", "Senior",
        "II", "III", "IV", "V", "VI", "VII", "VIII", "IX", "X",
        "2nd", "3rd", "4th", "5th",
        
        // Professional/Academic
        "Esq", "Esq.",
        "Ph.D", "Ph.D.", "PhD",
        "M.D", "M.D.", "MD",
        "D.D.S", "D.D.S.", "DDS",
        "J.D", "J.D.", "JD",
        "M.B.A", "M.B.A.", "MBA",
        "CPA",
        "RN",
    };
    
    private static string? ExtractSuffix(ref string[] nameParts)
    {
        if (nameParts.Length < 2)
        {
            return null;
        }
        
        string lastPart = nameParts[nameParts.Length - 1];
        
        // Check if the last part is a recognizable suffix
        if (recognizedSuffixes.Contains(lastPart))
        {
            // Normalize suffix capitalization
            string suffix = NormalizeSuffix(lastPart);
            
            // Remove the suffix from the parts array
            var tempList = new List<string>(nameParts);
            tempList.RemoveAt(nameParts.Length - 1);
            nameParts = tempList.ToArray();
            
            return suffix;
        }
        
        return null;
    }
    
    /// <summary>
    /// Normalizes a recognized suffix to its standard format.
    /// Custom suffixes not in the predefined set are returned as-is without normalization.
    /// </summary>
    /// <param name="suffix">The suffix to normalize.</param>
    /// <returns>The normalized suffix, or the original value if not recognized.</returns>
    private static string NormalizeSuffix(string suffix)
    {
        return suffix.ToLowerInvariant() switch
        {
            // Generational suffixes
            "jr" or "jr." or "junior" => "Jr.",
            "sr" or "sr." or "senior" => "Sr.",
            "ii" => "II",
            "iii" => "III",
            "iv" => "IV",
            "v" => "V",
            "vi" => "VI",
            "vii" => "VII",
            "viii" => "VIII",
            "ix" => "IX",
            "x" => "X",
            "2nd" => "II",
            "3rd" => "III",
            "4th" => "IV",
            "5th" => "V",
            
            // Professional/Academic suffixes
            "esq" or "esq." => "Esq.",
            "ph.d" or "ph.d." or "phd" => "Ph.D.",
            "m.d" or "m.d." or "md" => "M.D.",
            "d.d.s" or "d.d.s." or "dds" => "D.D.S.",
            "j.d" or "j.d." or "jd" => "J.D.",
            "m.b.a" or "m.b.a." or "mba" => "M.B.A.",
            "cpa" => "CPA",
            "rn" => "RN",
            
            // Return as-is if no normalization rule exists
            _ => suffix
        };
    }

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>A string in "LastName, FirstName" format.</returns>
    public override string ToString()
    {
        string name = LastName;
        
        if (!string.IsNullOrEmpty(FirstName))
        {
            name += ", " + FirstName;
            
            if (!string.IsNullOrEmpty(MiddleName))
            {
                name += " " + MiddleName;
            }

            if (!string.IsNullOrEmpty(Suffix))
            {
                name += " " + Suffix;
            }
        }
        
        return name;
    }

    /// <summary>
    /// Returns the formatted name in "FirstName LastName" format.
    /// </summary>
    /// <returns>A string in "FirstName LastName" format.</returns>
    public string ToDisplayString()
    {
        string name = "";
        
        if (!string.IsNullOrEmpty(FirstName))
        {
            name = FirstName;
            
            if (!string.IsNullOrEmpty(MiddleName))
            {
                name += " " + MiddleName;
            }

            name += " " + LastName;

            if (!string.IsNullOrEmpty(Suffix))
            {
                name += " " + Suffix;
            }
        }
        else
        {
            name = LastName;
        }
        
        return name;
    }

    /// <summary>
    /// Returns the formatted name in BibTeX format.
    /// </summary>
    /// <param name="format">The format to use when formatting the author name. Defaults to LastNameFirst.</param>
    /// <returns>A string formatted for BibTeX use.</returns>
    public string ToBibTeXString(AuthorFormat format = AuthorFormat.LastNameFirst)
    {
        return format switch
        {
            AuthorFormat.LastNameFirst => ToString(),
            AuthorFormat.FirstNameFirst => ToDisplayString(),
            _ => ToString() // Default to LastNameFirst (traditional BibTeX format)
        };
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
    public override bool Equals(object? obj)
    {
        if (obj is not Author other)
        {
            return false;
        }

        return string.Equals(LastName, other.LastName, StringComparison.OrdinalIgnoreCase) &&
               string.Equals(FirstName, other.FirstName, StringComparison.OrdinalIgnoreCase) &&
               string.Equals(MiddleName, other.MiddleName, StringComparison.OrdinalIgnoreCase) &&
               string.Equals(Suffix, other.Suffix, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>A hash code for this instance.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(
            LastName?.ToLowerInvariant(),
            FirstName?.ToLowerInvariant(),
            MiddleName?.ToLowerInvariant(),
            Suffix?.ToLowerInvariant());
    }
}