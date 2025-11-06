using System;
using System.Collections.Generic;
using System.Linq;
using BibSharp.Models;
using BibSharp.Utilities;
using Spectre.Console;

namespace BibSharp.Demo;

class KeyGenerationDemo
{
    public static void RunDemo()
    {
        AnsiConsole.Write(
            new FigletText("Key Generation")
                .Centered()
                .Color(Color.Aqua));
            
        AnsiConsole.WriteLine();
            
        var choicePrompt = new SelectionPrompt<string>()
            .Title("What would you like to demo?")
            .PageSize(10)
            .AddChoices(
                "Generate key for a single entry",
                "Generate keys for multiple entries",
                "Different key formats",
                "Handling key collisions");
            
        var choice = AnsiConsole.Prompt(choicePrompt);
            
        switch (choice)
        {
            case "Generate key for a single entry":
                DemoSingleKeyGeneration();
                break;
            case "Generate keys for multiple entries":
                DemoMultipleKeyGeneration();
                break;
            case "Different key formats":
                DemoKeyFormats();
                break;
            case "Handling key collisions":
                DemoKeyCollisions();
                break;
        }
    }
        
    private static void DemoSingleKeyGeneration()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule("[yellow]Generate Key for a Single Entry[/]"));
        AnsiConsole.WriteLine();
            
        // Create an entry interactively
        var entry = CreateEntryInteractively();
            
        // Display the entry
        DisplayEntry(entry, "Created Entry");
            
        // Generate a key
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[bold]Generate Key:[/]");
            
        var format = PromptForKeyFormat();
            
        AnsiConsole.Status()
            .Start("Generating key...", ctx => 
            {
                // For visual effect
                System.Threading.Thread.Sleep(500);
                    
                string generatedKey = entry.GenerateKey(format);
                    
                ctx.Status("Key generated!");
                System.Threading.Thread.Sleep(300);
            });
            
        // Display the entry with the new key
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[bold green]Key generation successful![/]");
        AnsiConsole.WriteLine();
            
        DisplayEntry(entry, "Entry with Generated Key");
            
        // Show the BibTeX
        AnsiConsole.WriteLine();
        var bibTexPanel = new Panel(entry.ToBibTeX())
        {
            Header = new PanelHeader("BibTeX with Generated Key"),
            Border = BoxBorder.Rounded,
            Expand = true
        };
            
        AnsiConsole.Write(bibTexPanel);
    }
        
    private static void DemoMultipleKeyGeneration()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule("[yellow]Generate Keys for Multiple Entries[/]"));
        AnsiConsole.WriteLine();
            
        // Create sample entries
        var entries = CreateSampleEntries();
            
        // Display the entries before key generation
        var table = new Table().Border(TableBorder.Rounded);
        table.AddColumn("Entry Type");
        table.AddColumn("Title");
        table.AddColumn("Authors");
        table.AddColumn("Year");
        table.AddColumn("Current Key");
            
        foreach (var entry in entries)
        {
            var authorString = string.Join(", ", 
                entry.Authors.Select(a => a.LastName));
                
            table.AddRow(
                entry.EntryType,
                entry.Title ?? "",
                authorString,
                entry.Year?.ToString() ?? "",
                entry.Key
            );
        }
            
        AnsiConsole.MarkupLine("[bold]Entries Before Key Generation:[/]");
        AnsiConsole.WriteLine();
        AnsiConsole.Write(table);
            
        // Ask about preserving existing keys
        AnsiConsole.WriteLine();
        bool preserveExisting = AnsiConsole.Confirm("Preserve existing keys?", true);
            
        // Select key format
        var format = PromptForKeyFormat();
            
        // Generate keys
        Dictionary<BibEntry, string> keyMap = null!;
            
        AnsiConsole.Status()
            .Start("Generating keys...", ctx => 
            {
                // For visual effect
                System.Threading.Thread.Sleep(800);
                    
                keyMap = BibEntry.RegenerateKeys(entries, format, preserveExisting);
                    
                ctx.Status("Keys generated!");
                System.Threading.Thread.Sleep(300);
            });
            
        // Display the entries after key generation
        var newTable = new Table().Border(TableBorder.Rounded);
        newTable.AddColumn("Entry Type");
        newTable.AddColumn("Title");
        newTable.AddColumn("Authors");
        newTable.AddColumn("Year");
        newTable.AddColumn("Generated Key");
            
        foreach (var entry in entries)
        {
            var authorString = string.Join(", ", 
                entry.Authors.Select(a => a.LastName));
                
            newTable.AddRow(
                entry.EntryType,
                entry.Title ?? "",
                authorString,
                entry.Year?.ToString() ?? "",
                Markup.Escape(entry.Key)
            );
        }
            
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[bold green]Keys generated successfully![/]");
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[bold]Entries After Key Generation:[/]");
        AnsiConsole.WriteLine();
        AnsiConsole.Write(newTable);
            
        // Show the BibTeX
        AnsiConsole.WriteLine();
        var bibTexPanel = new Panel(string.Join("\n\n", entries.Select(e => e.ToBibTeX())))
        {
            Header = new PanelHeader("BibTeX with Generated Keys"),
            Border = BoxBorder.Rounded,
            Expand = true
        };
            
        AnsiConsole.Write(bibTexPanel);
    }
        
    private static void DemoKeyFormats()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule("[yellow]Different Key Formats[/]"));
        AnsiConsole.WriteLine();
            
        // Create a sample entry
        var entry = new BibEntry("article")
        {
            Title = "A Comprehensive Study of Citation Key Generation",
            Year = 2023,
            Journal = "Journal of Computer Science",
            Volume = 15,
            Number = "2",
            Pages = new PageRange("123--145")
        };
            
        entry.AddAuthor(new Author("Smith", "John", "A."));
        entry.AddAuthor(new Author("Johnson", "Robert", "B."));
            
        // Display the entry
        DisplayEntry(entry, "Sample Entry");
            
        // Generate keys with different formats
        var formats = Enum.GetValues<BibKeyGenerator.KeyFormat>();
            
        var table = new Table().Border(TableBorder.Rounded);
        table.AddColumn("Format");
        table.AddColumn("Generated Key");
        table.AddColumn("Description");
            
        foreach (var format in formats)
        {
            // Clone the entry to not modify the original
            var clonedEntry = entry.Clone();
                
            string key = clonedEntry.GenerateKey(format);
            string description = GetFormatDescription(format);
                
            table.AddRow(format.ToString(), key, description);
        }
            
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[bold]Keys Generated with Different Formats:[/]");
        AnsiConsole.WriteLine();
        AnsiConsole.Write(table);
            
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[bold]Format Details:[/]");
        AnsiConsole.MarkupLine("- [green]AuthorYear[/]: Uses first author's last name and publication year");
        AnsiConsole.MarkupLine("- [green]AuthorTitleYear[/]: Adds the first significant word from the title");
        AnsiConsole.MarkupLine("- [green]AuthorTitleYearShort[/]: Uses abbreviated forms (first 3 letters)");
        AnsiConsole.MarkupLine("- [green]AuthorJournalYear[/]: Uses journal abbreviation instead of title");
    }
        
    private static void DemoKeyCollisions()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule("[yellow]Handling Key Collisions[/]"));
        AnsiConsole.WriteLine();
            
        // Create entries that would have the same base key
        var entries = new List<BibEntry>();
            
        // First entry
        var entry1 = new BibEntry("article")
        {
            Title = "First Study on Citation Keys",
            Year = 2023,
            Journal = "Journal of Documentation"
        };
        entry1.AddAuthor(new Author("Smith", "John"));
        entries.Add(entry1);
            
        // Second entry (same author and year)
        var entry2 = new BibEntry("article")
        {
            Title = "Second Study on Citation Keys",
            Year = 2023,
            Journal = "Journal of Information Science"
        };
        entry2.AddAuthor(new Author("Smith", "Jane"));
        entries.Add(entry2);
            
        // Third entry (same author and year)
        var entry3 = new BibEntry("book")
        {
            Title = "Advanced Citation Key Generation",
            Year = 2023,
            Publisher = "Academic Press"
        };
        entry3.AddAuthor(new Author("Smith", "David"));
        entries.Add(entry3);
            
        // Display the entries before key generation
        AnsiConsole.MarkupLine("[bold]Entries with Potential Key Conflicts:[/]");
        AnsiConsole.WriteLine();
            
        var beforeTable = new Table().Border(TableBorder.Rounded);
        beforeTable.AddColumn("Entry Type");
        beforeTable.AddColumn("Title");
        beforeTable.AddColumn("Author");
        beforeTable.AddColumn("Year");
            
        foreach (var entry in entries)
        {
            beforeTable.AddRow(
                entry.EntryType,
                entry.Title ?? "",
                entry.Authors.Count > 0 ? entry.Authors[0].LastName : "None",
                entry.Year?.ToString() ?? ""
            );
        }
            
        AnsiConsole.Write(beforeTable);
            
        // Generate keys
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[bold]Notice that all entries have the same author last name and year.[/]");
        AnsiConsole.MarkupLine("When we generate keys, BibSharp will automatically add suffixes to avoid conflicts.");
        AnsiConsole.WriteLine();
            
        var format = BibKeyGenerator.KeyFormat.AuthorYear;
            
        AnsiConsole.Status()
            .Start("Generating keys and resolving conflicts...", ctx => 
            {
                // For visual effect
                System.Threading.Thread.Sleep(1000);
                    
                BibEntry.RegenerateKeys(entries, format, false);
                    
                ctx.Status("Keys generated with conflict resolution!");
                System.Threading.Thread.Sleep(300);
            });
            
        // Display the results
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[bold green]Keys generated with automatic conflict resolution:[/]");
        AnsiConsole.WriteLine();
            
        var afterTable = new Table().Border(TableBorder.Rounded);
        afterTable.AddColumn("Entry Type");
        afterTable.AddColumn("Title");
        afterTable.AddColumn("Author");
        afterTable.AddColumn("Year");
        afterTable.AddColumn("Generated Key");
            
        foreach (var entry in entries)
        {
            afterTable.AddRow(
                entry.EntryType,
                entry.Title ?? "",
                entry.Authors.Count > 0 ? entry.Authors[0].LastName : "None",
                entry.Year?.ToString() ?? "",
                entry.Key
            );
        }
            
        AnsiConsole.Write(afterTable);
            
        // Explanation
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[bold]How Conflict Resolution Works:[/]");
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("1. The first entry gets the base key (e.g., [green]smith2023[/])");
        AnsiConsole.MarkupLine("2. The second entry gets the base key with 'a' suffix (e.g., [green]smith2023a[/])");
        AnsiConsole.MarkupLine("3. The third entry gets the base key with 'b' suffix (e.g., [green]smith2023b[/])");
        AnsiConsole.MarkupLine("4. This continues with 'c', 'd', etc. for additional conflicts");
        AnsiConsole.MarkupLine("5. If all letter suffixes are exhausted, numeric suffixes are used");
    }
        
    private static BibEntry CreateEntryInteractively()
    {
        var entryType = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select an entry type")
                .AddChoices("article", "book", "inproceedings", "misc"));
            
        var entry = new BibEntry(entryType);
            
        var title = AnsiConsole.Ask<string>("Enter title: ", "Sample Article");
        entry.Title = title;
            
        var year = AnsiConsole.Ask<int>("Enter year: ", 2023);
        entry.Year = year;
            
        if (entryType == "article")
        {
            var journal = AnsiConsole.Ask<string>("Enter journal: ", "Journal of Computer Science");
            entry.Journal = journal;
                
            var volume = AnsiConsole.Ask<int>("Enter volume: ", 10);
            entry.Volume = volume;
        }
        else if (entryType == "book")
        {
            var publisher = AnsiConsole.Ask<string>("Enter publisher: ", "Academic Press");
            entry.Publisher = publisher;
        }
        else if (entryType == "inproceedings")
        {
            var booktitle = AnsiConsole.Ask<string>("Enter conference name: ", "International Conference on Data Science");
            entry.BookTitle = booktitle;
        }
            
        var authorCount = AnsiConsole.Ask<int>("How many authors? ", 1);
            
        for (int i = 0; i < authorCount; i++)
        {
            var lastName = AnsiConsole.Ask<string>($"Enter author {i+1} last name: ", "Smith");
            var firstName = AnsiConsole.Ask<string>($"Enter author {i+1} first name: ", "John");
                
            entry.AddAuthor(new Author(lastName, firstName));
        }
            
        return entry;
    }
        
    private static BibKeyGenerator.KeyFormat PromptForKeyFormat()
    {
        var formatPrompt = new SelectionPrompt<BibKeyGenerator.KeyFormat>()
            .Title("Select a key format")
            .AddChoices(
                BibKeyGenerator.KeyFormat.AuthorYear,
                BibKeyGenerator.KeyFormat.AuthorTitleYear,
                BibKeyGenerator.KeyFormat.AuthorTitleYearShort,
                BibKeyGenerator.KeyFormat.AuthorJournalYear);
            
        return AnsiConsole.Prompt(formatPrompt);
    }
        
    private static void DisplayEntry(BibEntry entry, string title)
    {
        var table = new Table().Border(TableBorder.Rounded);
        table.Title = new TableTitle(title);
            
        table.AddColumn("Field");
        table.AddColumn("Value");
            
        table.AddRow("[bold]Entry Type[/]", entry.EntryType);
        table.AddRow("[bold]Key[/]", entry.Key);
        table.AddRow("[bold]Title[/]", entry.Title ?? "");
            
        // Format authors
        var authorNames = new List<string>();
        foreach (var author in entry.Authors)
        {
            authorNames.Add(author.ToDisplayString());
        }
            
        table.AddRow("[bold]Authors[/]", string.Join(", ", authorNames));
        table.AddRow("[bold]Year[/]", entry.Year?.ToString() ?? "");
            
        if (!string.IsNullOrEmpty(entry.Journal))
        {
            table.AddRow("[bold]Journal[/]", entry.Journal);
        }
            
        if (!string.IsNullOrEmpty(entry.BookTitle))
        {
            table.AddRow("[bold]Book Title[/]", entry.BookTitle);
        }
            
        if (!string.IsNullOrEmpty(entry.Publisher))
        {
            table.AddRow("[bold]Publisher[/]", entry.Publisher);
        }
            
        if (entry.Volume.HasValue)
        {
            table.AddRow("[bold]Volume[/]", entry.Volume.ToString()!);
        }
            
        AnsiConsole.Write(table);
    }
        
    private static List<BibEntry> CreateSampleEntries()
    {
        var entries = new List<BibEntry>();
            
        // Sample 1: Article with key already set
        var article1 = new BibEntry("article")
        {
            Key = "existing-key",
            Title = "An Analysis of Citation Key Generation Algorithms",
            Year = 2023,
            Journal = "Journal of Information Science",
            Volume = 48,
            Number = "2",
            Pages = new PageRange("123--145")
        };
        article1.AddAuthor(new Author("Smith", "John", "A."));
        article1.AddAuthor(new Author("Johnson", "Robert", "B."));
        entries.Add(article1);
            
        // Sample 2: Article without key
        var article2 = new BibEntry("article")
        {
            Title = "Automatic Key Generation for BibTeX Entries",
            Year = 2022,
            Journal = "Journal of Documentation",
            Volume = 78,
            Number = "5",
            Pages = new PageRange("587--602")
        };
        article2.AddAuthor(new Author("Brown", "Alice", "C."));
        article2.AddAuthor(new Author("Davis", "William", "D."));
        entries.Add(article2);
            
        // Sample 3: Book without key
        var book = new BibEntry("book")
        {
            Title = "Modern Bibliography Management",
            Year = 2021,
            Publisher = "Academic Press",
            Address = "London",
            Edition = "2nd"
        };
        book.AddAuthor(new Author("Jones", "Michael", "E."));
        entries.Add(book);
            
        // Sample 4: Conference paper without key
        var inproceedings = new BibEntry("inproceedings")
        {
            Title = "A Survey of BibTeX Tools",
            Year = 2023,
            BookTitle = "Proceedings of the International Conference on Documentation Systems",
            Pages = new PageRange("45--58")
        };
        inproceedings.AddAuthor(new Author("Wilson", "Emma", "F."));
        inproceedings.AddAuthor(new Author("Taylor", "James", "G."));
        entries.Add(inproceedings);
            
        return entries;
    }
        
    private static string GetFormatDescription(BibKeyGenerator.KeyFormat format)
    {
        return format switch
        {
            BibKeyGenerator.KeyFormat.AuthorYear => "Author's last name + publication year",
            BibKeyGenerator.KeyFormat.AuthorTitleYear => "Author + first significant word from title + year",
            BibKeyGenerator.KeyFormat.AuthorTitleYearShort => "Abbreviated author + title + year (3 chars each)",
            BibKeyGenerator.KeyFormat.AuthorJournalYear => "Author + journal abbreviation + year",
            _ => "Unknown format"
        };
    }
}