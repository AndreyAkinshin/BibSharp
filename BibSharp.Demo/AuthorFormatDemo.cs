using System;
using BibSharp.Models;
using BibSharp.Utilities;
using Spectre.Console;

namespace BibSharp.Demo;

class AuthorFormatDemo
{
    public static void RunDemo()
    {
        AnsiConsole.Write(
            new FigletText("Author Formats")
                .Centered()
                .Color(Color.Blue));

        AnsiConsole.WriteLine();

        // Let user choose a demo section
        var sectionPrompt = new SelectionPrompt<string>()
            .Title("Select a section to explore")
            .PageSize(10)
            .AddChoices(
                "Parsing author names",
                "Author name output formats",
                "BibEntry with different author formats",
                "BibSerializer with different formats");

        var section = AnsiConsole.Prompt(sectionPrompt);

        switch (section)
        {
            case "Parsing author names":
                ShowAuthorParsing();
                break;
            case "Author name output formats":
                ShowAuthorOutputFormats();
                break;
            case "BibEntry with different author formats":
                ShowBibEntryWithDifferentFormats();
                break;
            case "BibSerializer with different formats":
                ShowBibSerializerWithDifferentFormats();
                break;
        }
    }

    private static void ShowAuthorParsing()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule("[yellow]Parsing Author Names[/]"));
        AnsiConsole.WriteLine();

        // Let user enter an author name
        var authorName = AnsiConsole.Ask<string>("Enter an author name to parse: ", "Akinshin, Andrey");

        try
        {
            var author = Author.FromFormattedString(authorName);

            var table = new Table();
            table.Border = TableBorder.Rounded;

            table.AddColumn("Field");
            table.AddColumn("Value");

            table.AddRow("[bold]Last Name[/]", author.LastName ?? "");
            table.AddRow("[bold]First Name[/]", author.FirstName ?? "");

            if (!string.IsNullOrEmpty(author.MiddleName))
            {
                table.AddRow("[bold]Middle Name[/]", author.MiddleName);
            }

            if (!string.IsNullOrEmpty(author.Suffix))
            {
                table.AddRow("[bold]Suffix[/]", author.Suffix);
            }

            AnsiConsole.WriteLine();
            AnsiConsole.Write(table);

            // Also show some preset examples
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[bold]Other examples:[/]");

            var examples = new Tree("Author Name Parsing");
            examples.Style = Style.Parse("yellow");

            var lastFirstNode = examples.AddNode("[bold]'LastName, FirstName' format[/]");
            var author1 = Author.FromFormattedString("Akinshin, Andrey");
            lastFirstNode.AddNode($"Last Name: {author1.LastName}, First Name: {author1.FirstName}");

            var firstLastNode = examples.AddNode("[bold]'FirstName LastName' format[/]");
            var author2 = Author.FromFormattedString("Andrey Akinshin");
            firstLastNode.AddNode($"Last Name: {author2.LastName}, First Name: {author2.FirstName}");

            var complexNode = examples.AddNode("[bold]With middle name and suffix[/]");
            var author3 = Author.FromFormattedString("John Robert Smith Jr.");
            complexNode.AddNode($"Last Name: {author3.LastName}, First Name: {author3.FirstName}, " +
                                $"Middle Name: {author3.MiddleName}, Suffix: {author3.Suffix}");

            AnsiConsole.Write(examples);
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[bold red]Error parsing author name:[/] {ex}");
        }
    }

    private static void ShowAuthorOutputFormats()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule("[yellow]Author Name Output Formats[/]"));
        AnsiConsole.WriteLine();

        // Create an author
        var author = new Author("Akinshin", "Andrey", "A.");

        var table = new Table();
        table.Border = TableBorder.Rounded;

        table.AddColumn("Format");
        table.AddColumn("Output");

        table.AddRow("[bold]LastNameFirst[/]", author.ToBibTeXString(AuthorFormat.LastNameFirst));
        table.AddRow("[bold]FirstNameFirst[/]", author.ToBibTeXString(AuthorFormat.FirstNameFirst));

        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[bold]The same author displayed in different formats:[/]");
        AnsiConsole.WriteLine();
        AnsiConsole.Write(table);

        // Let user try with their own author
        AnsiConsole.WriteLine();
        if (AnsiConsole.Confirm("Do you want to try with a custom author name?"))
        {
            var lastName = AnsiConsole.Ask<string>("Enter [green]last name[/]: ");
            var firstName = AnsiConsole.Ask<string>("Enter [green]first name[/]: ");
            string? middleName = null;

            if (AnsiConsole.Confirm("Add a middle name or initial?"))
            {
                middleName = AnsiConsole.Ask<string>("Enter [green]middle name[/]: ");
            }

            var customAuthor = new Author(lastName, firstName, middleName);

            var customTable = new Table();
            customTable.Border = TableBorder.Rounded;

            customTable.AddColumn("Format");
            customTable.AddColumn("Output");

            customTable.AddRow("[bold]LastNameFirst[/]", customAuthor.ToBibTeXString(AuthorFormat.LastNameFirst));
            customTable.AddRow("[bold]FirstNameFirst[/]", customAuthor.ToBibTeXString(AuthorFormat.FirstNameFirst));

            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[bold]Your custom author displayed in different formats:[/]");
            AnsiConsole.WriteLine();
            AnsiConsole.Write(customTable);
        }
    }

    private static void ShowBibEntryWithDifferentFormats()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule("[yellow]BibEntry with Different Author Formats[/]"));
        AnsiConsole.WriteLine();

        // Create a BibEntry with authors
        var entry = new BibEntry("article")
        {
            Key = "akinshin2025",
            Title = "Author Format Options in BibTeX",
            Year = 2025,
            Journal = "Journal of Bibliography"
        };

        entry.AddAuthor(new Author("Akinshin", "Andrey"));
        entry.AddAuthor(new Author("Smith", "John", "R."));

        // Display the entry with different formats
        var grid = new Grid();
        grid.AddColumn();

        var lastNameFirstPanel = new Panel(entry.ToBibTeX())
        {
            Header = new PanelHeader("Default format (LastNameFirst)"),
            Border = BoxBorder.Rounded,
            Expand = true
        };

        var firstNameFirstPanel = new Panel(entry.ToBibTeX(AuthorFormat.FirstNameFirst))
        {
            Header = new PanelHeader("FirstNameFirst format"),
            Border = BoxBorder.Rounded,
            Expand = true
        };

        grid.AddRow(lastNameFirstPanel);
        grid.AddRow(firstNameFirstPanel);

        AnsiConsole.Write(grid);
    }

    private static void ShowBibSerializerWithDifferentFormats()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule("[yellow]BibSerializer with Different Author Formats[/]"));
        AnsiConsole.WriteLine();

        // Create a BibEntry with authors
        var entry = new BibEntry("article")
        {
            Key = "akinshin2025",
            Title = "Author Format Options in BibTeX",
            Year = 2025,
            Journal = "Journal of Bibliography"
        };

        entry.AddAuthor(new Author("Akinshin", "Andrey"));
        entry.AddAuthor(new Author("Smith", "John", "R."));

        var entries = new[] { entry };

        // Create serializers with different settings
        var defaultSerializer = new BibSerializer(new BibSerializerSettings
        {
            ValidateBeforeSerialization = false
        });

        var customSerializer = new BibSerializer(new BibSerializerSettings
        {
            AuthorFormat = AuthorFormat.FirstNameFirst,
            ValidateBeforeSerialization = false
        });

        // Display the serialized entries
        var grid = new Grid();
        grid.AddColumn();

        var defaultPanel = new Panel(defaultSerializer.Serialize(entries))
        {
            Header = new PanelHeader("Default serializer (LastNameFirst)"),
            Border = BoxBorder.Rounded,
            Expand = true
        };

        var customPanel = new Panel(customSerializer.Serialize(entries))
        {
            Header = new PanelHeader("Custom serializer (FirstNameFirst)"),
            Border = BoxBorder.Rounded,
            Expand = true
        };

        grid.AddRow(defaultPanel);
        grid.AddRow(customPanel);

        AnsiConsole.Write(grid);

        // Explanation
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[bold]Explanation:[/]");
        AnsiConsole.MarkupLine("The [blue]BibSerializer[/] allows you to set a default [green]AuthorFormat[/] " +
                               "in its settings to control how author names are formatted in the output.");
    }
}
