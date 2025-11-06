using System;
using System.Threading.Tasks;
using BibSharp.Models;
using BibSharp.Utilities;
using Spectre.Console;

namespace BibSharp.Demo;

class Program
{
    static async Task Main(string[] args)
    {
        AnsiConsole.Clear();
        RenderHeader();
            
        // Create a BibTeX entry manually
        AnsiConsole.MarkupLine("[bold yellow]Creating a new BibTeX entry:[/]");
        var entry = new BibEntry("article")
        {
            Key = "sample2025",
            Title = "Sample Article for BibSharp",
            Year = 2025,
            Journal = "Journal of .NET Libraries",
            Volume = 10,
            Pages = new PageRange(42, 50)
        };

        entry.AddAuthor(new Author("Smith", "John"));
        entry.AddAuthor(new Author("Doe", "Jane"));

        // Display the entry as BibTeX
        var entryPanel = new Panel(entry.ToBibTeX())
        {
            Header = new PanelHeader("BibTeX Entry"),
            Border = BoxBorder.Rounded,
            Expand = true
        };
        entryPanel.Border = BoxBorder.Rounded;
        AnsiConsole.Write(entryPanel);

        // Access fields both by property and indexer
        var table = new Table();
        table.Border = TableBorder.Rounded;
            
        table.AddColumn("Field Access Method");
        table.AddColumn("Value");
            
        table.AddRow("[bold]Title (property)[/]", entry.Title ?? "");
        table.AddRow("[bold]Title (indexer)[/]", entry["title"] ?? "");
        table.AddRow("[bold]Custom field[/]", entry["custom"] ?? "(not set)");
            
        // Add a custom field
        entry["custom"] = "Custom field value";
        table.AddRow("[bold]Custom field (after setting)[/]", entry["custom"] ?? "");
            
        AnsiConsole.MarkupLine("[bold yellow]Accessing fields:[/]");
        AnsiConsole.Write(table);

        // Validate the entry
        AnsiConsole.MarkupLine("[bold yellow]Validating entry:[/]");
        var validationResult = entry.Validate();
        if (validationResult.IsValid)
        {
            AnsiConsole.MarkupLine("[bold green]Entry is valid.[/]");
        }
        else
        {
            AnsiConsole.MarkupLine("[bold red]Entry has issues:[/]");
            foreach (var error in validationResult.Errors)
            {
                AnsiConsole.MarkupLine($"[red]- Error: {error}[/]");
            }
            foreach (var warning in validationResult.Warnings)
            {
                AnsiConsole.MarkupLine($"[yellow]- Warning: {warning}[/]");
            }
        }

        // Parse a BibTeX file
        string sampleBib = @"
@string{JCS = ""Journal of Computer Science""}

@article{smith2025,
  author = {Smith, John and Doe, Jane},
  title = {A Comprehensive Study of BibTeX Processing},
  journal = JCS,
  year = {2025},
  volume = {42},
  number = {1},
  pages = {123--145}
}

@book{brown2024,
  author = {Brown, Alice},
  title = {Modern BibTeX Usage},
  publisher = {Technical Publishing},
  year = {2024}
}";

        AnsiConsole.MarkupLine("[bold yellow]Parsing BibTeX content:[/]");
        var parser = new BibParser();
        var entries = parser.ParseString(sampleBib);
            
        var parsedTable = new Table();
        parsedTable.Border = TableBorder.Rounded;
            
        parsedTable.AddColumn("Type");
        parsedTable.AddColumn("Key");
        parsedTable.AddColumn("Title");
        parsedTable.AddColumn("Year");
            
        foreach (var parsedEntry in entries)
        {
            parsedTable.AddRow(
                parsedEntry.EntryType.Value,
                parsedEntry.Key ?? "",
                parsedEntry.Title ?? "",
                parsedEntry.Year?.ToString() ?? ""
            );
        }
            
        AnsiConsole.MarkupLine($"[bold]Found {entries.Count} entries:[/]");
        AnsiConsole.Write(parsedTable);

        // Serialize back to BibTeX
        AnsiConsole.MarkupLine("[bold yellow]Serializing entries to BibTeX:[/]");
        var serializer = new BibSerializer(new BibSerializerSettings
        {
            FieldOrder = new[] { "author", "title", "year" }
        });
        string serialized = serializer.Serialize(entries);
            
        var serializedPanel = new Panel(serialized)
        {
            Header = new PanelHeader("Serialized BibTeX"),
            Border = BoxBorder.Rounded,
            Expand = true
        };
        AnsiConsole.Write(serializedPanel);
            
        // Offer examples
        bool exit = false;
        while (!exit)
        {
            var selection = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold]Select a demo to run:[/]")
                    .PageSize(10)
                    .HighlightStyle(new Style(foreground: Color.Green))
                    .AddChoices(new[]
                    {
                        "DOI Resolution Demo",
                        "Extended Fields Demo",
                        "Author Format Demo",
                        "Internationalization Demo",
                        "BibFormatter Demo",
                        "Facade Methods Demo",
                        "Key Generation Demo",
                        "Search Demo",
                        "Exit"
                    }));
                
            AnsiConsole.Clear();
            RenderHeader();
                
            switch (selection)
            {
                case "DOI Resolution Demo":
                    await DoiDemo.RunDemo();
                    break;
                case "Extended Fields Demo":
                    ExtendedFieldsDemo.RunDemo();
                    break;
                case "Author Format Demo":
                    AuthorFormatDemo.RunDemo();
                    break;
                case "Internationalization Demo":
                    InternationalizationDemo.RunDemo();
                    break;
                case "BibFormatter Demo":
                    BibFormatterDemo.RunDemo();
                    break;
                case "Facade Methods Demo":
                    FacadeMethodsDemo.RunDemo();
                    break;
                case "Key Generation Demo":
                    KeyGenerationDemo.RunDemo();
                    break;
                case "Search Demo":
                    SearchDemo.Run();
                    break;
                case "Exit":
                    exit = true;
                    break;
            }
                
            if (!exit)
            {
                AnsiConsole.MarkupLine("\n[dim italic]Press any key to return to the main menu...[/]");
                Console.ReadKey(true);
                AnsiConsole.Clear();
                RenderHeader();
            }
        }
    }
        
    private static void RenderHeader()
    {
        AnsiConsole.Write(
            new FigletText("BibSharp")
                .LeftJustified()
                .Color(Color.Green));
            
        AnsiConsole.Write(
            new Rule("[yellow]A modern .NET library for BibTeX processing[/]")
                .LeftJustified());
            
        AnsiConsole.WriteLine();
    }
}