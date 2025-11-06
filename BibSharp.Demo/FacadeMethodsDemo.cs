using System;
using BibSharp.Models;
using BibSharp.Utilities;
using Spectre.Console;

namespace BibSharp.Demo;

class FacadeMethodsDemo
{
    public static void RunDemo()
    {
        AnsiConsole.Write(
            new FigletText("Facade Methods")
                .Centered()
                .Color(Color.Green));

        AnsiConsole.WriteLine();

        var choicePrompt = new SelectionPrompt<string>()
            .Title("What would you like to demo?")
            .PageSize(10)
            .AddChoices(
                "Parse a single BibTeX entry",
                "Parse multiple BibTeX entries",
                "Format an entry in different styles");

        var choice = AnsiConsole.Prompt(choicePrompt);

        switch (choice)
        {
            case "Parse a single BibTeX entry":
                DemoParseSingle();
                break;
            case "Parse multiple BibTeX entries":
                DemoParseMultiple();
                break;
            case "Format an entry in different styles":
                DemoFormatEntry();
                break;
        }
    }

    private static void DemoParseSingle()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule("[yellow]Parse a Single BibTeX Entry[/]"));
        AnsiConsole.WriteLine();

        // Example BibTeX entry
        string exampleBibTeX = @"@article{smith2023,
  author = {Smith, John and Doe, Jane},
  title = {Example Article Title},
  journal = {Journal of Examples},
  year = {2023},
  volume = {10},
  number = {2},
  pages = {100--120},
  doi = {10.1000/example.2023.04.005}
}";

        // Display the example
        var examplePanel = new Panel(exampleBibTeX)
        {
            Header = new PanelHeader("Example BibTeX Entry"),
            Border = BoxBorder.Rounded,
            Expand = true
        };

        AnsiConsole.Write(examplePanel);
        AnsiConsole.WriteLine();

        // Use the Parse method
        AnsiConsole.Status()
            .Start("Parsing entry...", ctx =>
            {
                try
                {
                    var entry = BibEntry.Parse(exampleBibTeX);

                    // Show the parsed entry properties
                    var table = new Table();
                    table.Border = TableBorder.Rounded;

                    table.AddColumn("Field");
                    table.AddColumn("Value");

                    table.AddRow("[bold]Entry Type[/]", entry.EntryType);
                    table.AddRow("[bold]Key[/]", entry.Key);
                    table.AddRow("[bold]Title[/]", entry.Title ?? "");

                    // Format authors
                    var authorNames = new System.Collections.Generic.List<string>();
                    foreach (var author in entry.Authors)
                    {
                        authorNames.Add(author.ToDisplayString());
                    }

                    table.AddRow("[bold]Authors[/]", string.Join(", ", authorNames));
                    table.AddRow("[bold]Journal[/]", entry.Journal ?? "");
                    table.AddRow("[bold]Year[/]", entry.Year?.ToString() ?? "");
                    table.AddRow("[bold]Volume[/]", entry.Volume?.ToString() ?? "");
                    table.AddRow("[bold]Number[/]", entry.Number ?? "");
                    table.AddRow("[bold]Pages[/]", entry.Pages?.ToString() ?? "");
                    table.AddRow("[bold]DOI[/]", entry.Doi ?? "");

                    AnsiConsole.WriteLine();
                    AnsiConsole.MarkupLine("[bold green]Successfully parsed entry:[/]");
                    AnsiConsole.WriteLine();
                    AnsiConsole.Write(table);

                    // Show formatted citation
                    var citationStyle = CitationStyle.Chicago;
                    string formattedCitation = entry.ToFormattedString(citationStyle);

                    var citationPanel = new Panel(formattedCitation)
                    {
                        Header = new PanelHeader($"{citationStyle} Formatted Citation"),
                        Border = BoxBorder.Rounded,
                        Expand = true
                    };

                    AnsiConsole.WriteLine();
                    AnsiConsole.Write(citationPanel);
                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLine($"[bold red]Error:[/] {ex}");
                }

                // Short delay for visibility
                System.Threading.Thread.Sleep(500);
            });

        // Show TryParse example
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[bold]Using TryParse:[/]");
        AnsiConsole.WriteLine();

        if (BibEntry.TryParse(exampleBibTeX, out var parsedEntry) && parsedEntry != null)
        {
            AnsiConsole.MarkupLine("[green]Successfully parsed:[/] " +
                                   $"{parsedEntry.Authors.Count} authors, " +
                                   $"title: \"{parsedEntry.Title}\", " +
                                   $"published in {parsedEntry.Journal}, {parsedEntry.Year}");
        }
        else
        {
            AnsiConsole.MarkupLine("[red]Failed to parse the entry[/]");
        }
    }

    private static void DemoParseMultiple()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule("[yellow]Parse Multiple BibTeX Entries[/]"));
        AnsiConsole.WriteLine();

        // Example BibTeX with multiple entries
        string exampleBibTeX = @"@article{smith2023,
  author = {Smith, John and Doe, Jane},
  title = {First Example Article},
  journal = {Journal of Examples},
  year = {2023},
  volume = {10},
  pages = {100--120}
}

@book{jones2022,
  author = {Jones, Robert},
  title = {Example Book Title},
  publisher = {Example Publishing},
  year = {2022},
  address = {New York}
}

@inproceedings{miller2021,
  author = {Miller, Sarah},
  title = {Conference Paper Example},
  booktitle = {Proceedings of the Example Conference},
  year = {2021},
  pages = {45--52}
}";

        // Display the example
        var examplePanel = new Panel(exampleBibTeX)
        {
            Header = new PanelHeader("Example BibTeX Entries"),
            Border = BoxBorder.Rounded,
            Expand = true
        };

        AnsiConsole.Write(examplePanel);
        AnsiConsole.WriteLine();

        // Use the ParseAll method
        AnsiConsole.Status()
            .Start("Parsing entries...", ctx =>
            {
                try
                {
                    var entries = BibEntry.ParseAll(exampleBibTeX);

                    // Create a table of entries
                    var table = new Table();
                    table.Border = TableBorder.Rounded;

                    table.AddColumn("Type");
                    table.AddColumn("Key");
                    table.AddColumn("Title");
                    table.AddColumn("Authors");
                    table.AddColumn("Year");

                    foreach (var entry in entries)
                    {
                        var authorString = string.Join(", ",
                            System.Linq.Enumerable.Select(entry.Authors, a => a.LastName));

                        table.AddRow(
                            entry.EntryType,
                            entry.Key,
                            entry.Title ?? "",
                            authorString,
                            entry.Year?.ToString() ?? ""
                        );
                    }

                    AnsiConsole.WriteLine();
                    AnsiConsole.MarkupLine($"[bold green]Successfully parsed {entries.Count} entries:[/]");
                    AnsiConsole.WriteLine();
                    AnsiConsole.Write(table);

                    // Format a bibliography
                    var style = CitationStyle.Apa;
                    string bibliography = BibFormatter.FormatBibliography(entries, style);

                    var bibPanel = new Panel(bibliography)
                    {
                        Header = new PanelHeader($"{style} Style Bibliography"),
                        Border = BoxBorder.Rounded,
                        Expand = true
                    };

                    AnsiConsole.WriteLine();
                    AnsiConsole.Write(bibPanel);
                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLine($"[bold red]Error:[/] {ex}");
                }

                // Short delay for visibility
                System.Threading.Thread.Sleep(500);
            });
    }

    private static void DemoFormatEntry()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule("[yellow]Format an Entry in Different Styles[/]"));
        AnsiConsole.WriteLine();

        // Create a sample entry
        var entry = new BibEntry("article")
        {
            Key = "smith2023",
            Title = "The Impact of Facade Methods on Library Usability",
            Year = 2023,
            Journal = "Journal of Software Engineering",
            Volume = 15,
            Number = "2",
            Pages = new PageRange("123--145"),
            Doi = "10.1000/example.2023.04.005",
            Url = "https://example.org/articles/facade-methods"
        };

        entry.AddAuthor(new Author("Smith", "John", "A."));
        entry.AddAuthor(new Author("Johnson", "Robert", "B."));
        entry.AddAuthor(new Author("Williams", "Sarah", "C."));

        // Display the BibTeX
        var bibPanel = new Panel(entry.ToBibTeX())
        {
            Header = new PanelHeader("BibTeX Entry"),
            Border = BoxBorder.Rounded,
            Expand = true
        };

        AnsiConsole.Write(bibPanel);
        AnsiConsole.WriteLine();

        // Format in all available styles
        var styles = Enum.GetValues<CitationStyle>();

        var table = new Table();
        table.Border = TableBorder.Rounded;
        table.Expand();

        table.AddColumn("Citation Style");
        table.AddColumn("Formatted Citation");

        foreach (var style in styles)
        {
            string formatted = entry.ToFormattedString(style);
            table.AddRow($"[bold]{style}[/]", formatted);
        }

        AnsiConsole.MarkupLine("[bold]Formatted Citations:[/]");
        AnsiConsole.WriteLine();
        AnsiConsole.Write(table);

        // Show example usage
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[bold]Example Usage:[/]");
        AnsiConsole.WriteLine();

        var codePanel = new Panel(
            @"// Parse a BibTeX string into a single entry
var entry = BibEntry.Parse(bibTexString);

// Format the entry according to a citation style
string chicagoCitation = entry.ToFormattedString(CitationStyle.Chicago);

// Convert back to BibTeX
string bibTeX = entry.ToBibTeX();

// Try to parse, with error handling
if (BibEntry.TryParse(bibTexString, out var parsedEntry))
{
    // Use the parsed entry
    Console.WriteLine(parsedEntry.Title);
}

// Parse multiple entries
var entries = BibEntry.ParseAll(bibTexString);")
        {
            Header = new PanelHeader("C# Code Example"),
            Border = BoxBorder.Rounded,
            Expand = true
        };

        AnsiConsole.Write(codePanel);
    }
}