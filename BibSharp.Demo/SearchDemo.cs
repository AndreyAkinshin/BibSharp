using System;
using System.Collections.Generic;
using System.Linq;
using BibSharp.Models;
using BibSharp.Utilities;
using Spectre.Console;

namespace BibSharp.Demo;

/// <summary>
/// Demonstrates the BibEntry search and matching functionality
/// </summary>
public class SearchDemo
{
    public static void Run()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule("[aqua]BibEntry Search and Matching[/]").RuleStyle("grey").LeftJustified());
        AnsiConsole.WriteLine();
            
        AnsiConsole.MarkupLine("[grey]This demo shows how to search for matching entries and find duplicates in bibliographies.[/]");
        AnsiConsole.WriteLine();
            
        // Create a sample database of entries
        var bibliography = CreateSampleBibliography();
            
        // Display the sample bibliography
        AnsiConsole.MarkupLine("[underline]Sample Bibliography:[/]");
        foreach (var entry in bibliography)
        {
            AnsiConsole.MarkupLine($"[yellow]Entry Key:[/] {entry.Key}");
            if (!string.IsNullOrEmpty(entry.Title))
                AnsiConsole.MarkupLine($"  Title: {entry.Title}");
                    
            if (entry.Authors.Count > 0)
                AnsiConsole.MarkupLine($"  Authors: {string.Join(", ", entry.Authors.Select(a => a.ToDisplayString()))}");
                    
            if (entry.Year.HasValue)
                AnsiConsole.MarkupLine($"  Year: {entry.Year}");
                    
            if (!string.IsNullOrEmpty(entry.Doi))
                AnsiConsole.MarkupLine($"  DOI: {entry.Doi}");
                    
            if (!string.IsNullOrEmpty(entry["url"]))
                AnsiConsole.MarkupLine($"  URL: {entry["url"]}");
                    
            AnsiConsole.WriteLine();
        }
            
        // Demonstrate finding a match for a new entry
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[underline]1. Finding a Match by DOI:[/]");
            
        var searchEntry1 = new BibEntry
        {
            Key = "newEntry1",
            Title = "Different Title", // Different title but same DOI
            Doi = "10.1234/abcd.5678"
        };
            
        var match1 = searchEntry1.FindMatch(bibliography);
            
        AnsiConsole.MarkupLine("[yellow]Search Entry:[/]");
        AnsiConsole.MarkupLine($"  Key: {searchEntry1.Key}");
        AnsiConsole.MarkupLine($"  Title: {searchEntry1.Title}");
        AnsiConsole.MarkupLine($"  DOI: {searchEntry1.Doi}");
        AnsiConsole.WriteLine();
            
        if (match1 != null)
        {
            AnsiConsole.MarkupLine("[green]Match found:[/]");
            AnsiConsole.MarkupLine($"  Key: {match1.Key}");
            AnsiConsole.MarkupLine($"  Title: {match1.Title}");
            AnsiConsole.MarkupLine($"  DOI: {match1.Doi}");
            AnsiConsole.MarkupLine($"  [grey]Matched by DOI[/]");
        }
        else
        {
            AnsiConsole.MarkupLine("[red]No match found[/]");
        }
            
        // Demonstrate finding a match by URL
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[underline]2. Finding a Match by URL:[/]");
            
        var searchEntry2 = new BibEntry { Key = "newEntry2", Title = "New URL Paper" };
        searchEntry2["url"] = "https://example.com/paper2/";
            
        var match2 = searchEntry2.FindMatch(bibliography);
            
        AnsiConsole.MarkupLine("[yellow]Search Entry:[/]");
        AnsiConsole.MarkupLine($"  Key: {searchEntry2.Key}");
        AnsiConsole.MarkupLine($"  Title: {searchEntry2.Title}");
        AnsiConsole.MarkupLine($"  URL: {searchEntry2["url"]}");
        AnsiConsole.WriteLine();
            
        if (match2 != null)
        {
            AnsiConsole.MarkupLine("[green]Match found:[/]");
            AnsiConsole.MarkupLine($"  Key: {match2.Key}");
            AnsiConsole.MarkupLine($"  Title: {match2.Title}");
            AnsiConsole.MarkupLine($"  URL: {match2["url"]}");
            AnsiConsole.MarkupLine($"  [grey]Matched by URL[/]");
        }
        else
        {
            AnsiConsole.MarkupLine("[red]No match found[/]");
        }
            
        // Demonstrate finding a match by author, title and year
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[underline]3. Finding a Match by Author, Title and Year:[/]");
            
        var searchEntry3 = new BibEntry
        {
            Key = "newEntry3",
            Title = "Neural Networks in Machine Learning",
            Year = 2023
        };
        searchEntry3.AddAuthor(new Author("Smith", "John"));
            
        var match3 = searchEntry3.FindMatch(bibliography);
            
        AnsiConsole.MarkupLine("[yellow]Search Entry:[/]");
        AnsiConsole.MarkupLine($"  Key: {searchEntry3.Key}");
        AnsiConsole.MarkupLine($"  Title: {searchEntry3.Title}");
        AnsiConsole.MarkupLine($"  Authors: {string.Join(", ", searchEntry3.Authors.Select(a => a.ToDisplayString()))}");
        AnsiConsole.MarkupLine($"  Year: {searchEntry3.Year}");
        AnsiConsole.WriteLine();
            
        if (match3 != null)
        {
            AnsiConsole.MarkupLine("[green]Match found:[/]");
            AnsiConsole.MarkupLine($"  Key: {match3.Key}");
            AnsiConsole.MarkupLine($"  Title: {match3.Title}");
            AnsiConsole.MarkupLine($"  Authors: {string.Join(", ", match3.Authors.Select(a => a.ToDisplayString()))}");
            AnsiConsole.MarkupLine($"  Year: {match3.Year}");
            AnsiConsole.MarkupLine($"  [grey]Matched by author, title similarity and year[/]");
        }
        else
        {
            AnsiConsole.MarkupLine("[red]No match found[/]");
        }
            
        // Demonstrate finding duplicates
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[underline]4. Finding Duplicates in a Bibliography:[/]");
            
        // Create a bibliography with some duplicates
        var duplicateBibliography = CreateBibliographyWithDuplicates();
            
        var duplicateGroups = duplicateBibliography.FindDuplicates().ToList();
            
        AnsiConsole.MarkupLine($"Found [yellow]{duplicateGroups.Count}[/] duplicate groups:");
        AnsiConsole.WriteLine();
            
        for (int i = 0; i < duplicateGroups.Count; i++)
        {
            var group = duplicateGroups[i];
            AnsiConsole.MarkupLine($"[aqua]Duplicate Group {i+1}:[/]");
                
            foreach (var entry in group)
            {
                AnsiConsole.MarkupLine($"  [yellow]Key:[/] {entry.Key}");
                AnsiConsole.MarkupLine($"    Title: {entry.Title}");
                    
                if (entry.Authors.Count > 0)
                    AnsiConsole.MarkupLine($"    Authors: {string.Join(", ", entry.Authors.Select(a => a.ToDisplayString()))}");
                        
                if (entry.Year.HasValue)
                    AnsiConsole.MarkupLine($"    Year: {entry.Year}");
                        
                if (!string.IsNullOrEmpty(entry.Doi))
                    AnsiConsole.MarkupLine($"    DOI: {entry.Doi}");
            }
                
            AnsiConsole.WriteLine();
        }
            
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[grey]Press any key to return to the main menu...[/]");
        Console.ReadKey(true);
    }
        
    private static List<BibEntry> CreateSampleBibliography()
    {
        return new List<BibEntry>
        {
            new BibEntry
                {
                    Key = "smith2023a",
                    Title = "Machine Learning Techniques",
                    Year = 2023,
                    Doi = "10.1234/abcd.5678"
                }.AddAuthor(new Author("Smith", "John"))
                .AddAuthor(new Author("Johnson", "Alice")),
                
            new BibEntry
            {
                Key = "brown2022",
                Title = "Artificial Intelligence Overview",
                Year = 2022,
                Doi = "10.5678/efgh.1234"
            }.AddAuthor(new Author("Brown", "Robert")),
                
            new BibEntry
                {
                    Key = "johnson2023",
                    Title = "Deep Learning Methods",
                    Year = 2023,
                }.AddAuthor(new Author("Johnson", "Alice"))
                .AddAuthor(new Author("Wilson", "David")),
                
            new BibEntry
                {
                    Key = "wilson2023",
                    Title = "Neural Networks in Machine Learning",
                    Year = 2023,
                }.AddAuthor(new Author("Smith", "John"))
                .AddAuthor(new Author("Wilson", "David")),
                
            new BibEntry
            {
                Key = "davis2021",
                Title = "Data Mining Techniques",
                Year = 2021,
            }.AddAuthor(new Author("Davis", "Michael"))
        };
    }
        
    private static List<BibEntry> CreateBibliographyWithDuplicates()
    {
        var bibliography = new List<BibEntry>
        {
            // Original entries
            new BibEntry
                {
                    Key = "smith2023",
                    Title = "Machine Learning Techniques",
                    Year = 2023,
                    Doi = "10.1234/abcd.5678"
                }.AddAuthor(new Author("Smith", "John"))
                .AddAuthor(new Author("Johnson", "Alice")),
                
            new BibEntry
            {
                Key = "brown2022",
                Title = "Artificial Intelligence Overview",
                Year = 2022,
                Doi = "10.5678/efgh.1234"
            }.AddAuthor(new Author("Brown", "Robert")),
                
            new BibEntry
                {
                    Key = "wilson2023",
                    Title = "Neural Networks in Machine Learning",
                    Year = 2023,
                }.AddAuthor(new Author("Smith", "John"))
                .AddAuthor(new Author("Wilson", "David")),
                
            // Duplicate entries
            new BibEntry
                {
                    Key = "smith2023_duplicate",
                    Title = "Advanced Machine Learning Techniques", // Slightly different title
                    Year = 2023,
                    Doi = "10.1234/abcd.5678" // Same DOI
                }.AddAuthor(new Author("Smith", "John"))
                .AddAuthor(new Author("Johnson", "Alice")),
                
            new BibEntry
                {
                    Key = "wilson2023_duplicate",
                    Title = "Neural Networks and Their Applications in ML", // Similar title
                    Year = 2023,
                }.AddAuthor(new Author("Smith", "John")) // Same first author
                .AddAuthor(new Author("Wilson", "David"))
        };
            
        // Add URLs to some entries
        bibliography[1]["url"] = "https://example.com/paper1";
        bibliography[2]["url"] = "https://example.com/paper2";
            
        return bibliography;
    }
}