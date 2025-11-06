using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Spectre.Console;

namespace BibSharp.Demo;

class DoiDemo
{
    // Examples of DOIs that should resolve
    private static readonly string[] sampleDois = new[]
    {
        "10.1145/3360592", // An ACM paper - System FR: formalized foundations for the stainless verifier
        "10.1109/5.771073", // An IEEE paper - Toward unique identifiers
        "10.1016/j.cpc.2021.108171", // An Elsevier paper - LAMMPS simulation tool
    };

    public static async Task RunDemo()
    {
        AnsiConsole.Write(
            new FigletText("DOI Resolution")
                .Centered()
                .Color(Color.Purple));

        AnsiConsole.WriteLine();

        // Let the user choose a DOI or enter their own
        var choices = new List<string>(sampleDois);
        choices.Add("Enter a custom DOI...");

        var selectionPrompt = new SelectionPrompt<string>()
            .Title("Select a [green]DOI[/] to resolve or enter a custom one")
            .PageSize(10)
            .HighlightStyle(new Style(foreground: Color.Green))
            .AddChoices(choices);

        string selectedDoi = AnsiConsole.Prompt(selectionPrompt);

        if (selectedDoi == "Enter a custom DOI...")
        {
            selectedDoi = AnsiConsole.Ask<string>("Enter DOI to resolve: ");
        }

        await ResolveAndDisplayDoi(selectedDoi);
    }

    private static async Task ResolveAndDisplayDoi(string doi)
    {
        AnsiConsole.MarkupLine($"[bold green]Resolving DOI:[/] [yellow]{doi}[/]");

        await AnsiConsole.Status()
            .StartAsync("Contacting DOI resolver...", async ctx =>
            {
                ctx.Spinner(Spinner.Known.Dots);
                ctx.SpinnerStyle(Style.Parse("green"));

                try
                {
                    var entry = await BibEntry.FromDoiAsync(doi);

                    if (entry != null)
                    {
                        ctx.Status("Processing result...");
                        await Task.Delay(400); // Just for show

                        // Display the result in a structured way
                        var doiPanel = new Panel(entry.ToBibTeX())
                        {
                            Header = new PanelHeader("BibTeX Entry"),
                            Border = BoxBorder.Rounded,
                            Expand = true
                        };

                        // Create a table with entry details
                        var table = new Table();
                        table.Border = TableBorder.Rounded;

                        table.AddColumn("Field");
                        table.AddColumn("Value");

                        table.AddRow("[bold]Title[/]", entry.Title ?? "");

                        // Create author display names
                        var authorDisplayNames = new List<string>();
                        foreach (var author in entry.Authors)
                        {
                            authorDisplayNames.Add(author.ToDisplayString());
                        }

                        table.AddRow("[bold]Authors[/]", string.Join(", ", authorDisplayNames));
                        table.AddRow("[bold]Year[/]", entry.Year?.ToString() ?? "");
                        table.AddRow("[bold]Journal/Publication[/]", entry.Journal ?? "");

                        if (!string.IsNullOrEmpty(entry.Doi))
                        {
                            table.AddRow("[bold]DOI[/]", entry.Doi);
                        }

                        if (!string.IsNullOrEmpty(entry.Url))
                        {
                            table.AddRow("[bold]URL[/]", entry.Url);
                        }

                        AnsiConsole.WriteLine();
                        AnsiConsole.Write(new Rule("Resolved Entry"));
                        AnsiConsole.WriteLine();

                        AnsiConsole.Write(table);
                        AnsiConsole.WriteLine();
                        AnsiConsole.Write(doiPanel);
                    }
                    else
                    {
                        AnsiConsole.MarkupLine("[bold red]Failed to resolve the DOI.[/]");
                    }
                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLine($"[bold red]Error:[/] {ex}");
                }
            });
    }
}