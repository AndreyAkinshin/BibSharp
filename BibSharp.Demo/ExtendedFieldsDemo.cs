using System.Collections.Generic;
using BibSharp.Models;
using Spectre.Console;

namespace BibSharp.Demo;

class ExtendedFieldsDemo
{
    public static void RunDemo()
    {
        AnsiConsole.Write(
            new FigletText("Extended Fields")
                .Centered()
                .Color(Color.Orange1));
            
        AnsiConsole.WriteLine();
            
        // Create a book entry with extended fields
        var book = CreateBookWithExtendedFields();
            
        // Show entry details in an interactive panel
        var choicePrompt = new SelectionPrompt<string>()
            .Title("What would you like to see?")
            .PageSize(10)
            .AddChoices(
                "View entry details",
                "Explore keywords",
                "Modify entry",
                "View BibTeX output");
            
        var choice = AnsiConsole.Prompt(choicePrompt);
            
        switch (choice)
        {
            case "View entry details":
                ShowEntryDetails(book);
                break;
            case "Explore keywords":
                ExploreKeywords(book);
                break;
            case "Modify entry":
                ModifyEntry(book);
                break;
            case "View BibTeX output":
                ShowBibTeXOutput(book);
                break;
        }
    }
        
    private static BibEntry CreateBookWithExtendedFields()
    {
        var book = new BibEntry("book")
        {
            Key = "advancedBook2025",
            Title = "Advanced BibTeX Processing with BibSharp",
            Year = 2025,
            Publisher = "Technical Publishing",
            Edition = "2nd",
            Series = "Programming Libraries Series",
            Address = "Seattle, WA",
            Isbn = "978-1-23456-789-0",
            Language = "English",
            Abstract = "This comprehensive guide shows how to use BibSharp for advanced BibTeX processing in .NET applications."
        };
            
        // Add authors
        book.AddAuthor(new Author("Smith", "John", "A."));
        book.AddAuthor(new Author("Johnson", "Mary", "E."));
            
        // Add keywords
        book.SetKeywordList(new[] {
            "BibTeX", 
            "bibliography", 
            ".NET", 
            "C#", 
            "citation management"
        });
            
        // Set the month using the month helper
        book.SetMonthByNumber(5, true); // Sets to "may"
            
        return book;
    }
        
    private static void ShowEntryDetails(BibEntry book)
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule("[yellow]Book Entry Details[/]"));
        AnsiConsole.WriteLine();
            
        var table = new Table();
        table.Border = TableBorder.Rounded;
            
        table.AddColumn("Field");
        table.AddColumn("Value");
            
        table.AddRow("[bold]Title[/]", book.Title ?? "");
            
        // Format authors
        var authorNames = new List<string>();
        foreach (var author in book.Authors)
        {
            authorNames.Add(author.ToDisplayString());
        }
            
        table.AddRow("[bold]Authors[/]", string.Join(", ", authorNames));
        table.AddRow("[bold]Year[/]", book.Year?.ToString() ?? "");
        table.AddRow("[bold]Publisher[/]", book.Publisher ?? "");
        table.AddRow("[bold]Edition[/]", book.Edition ?? "");
        table.AddRow("[bold]Series[/]", book.Series ?? "");
        table.AddRow("[bold]Address[/]", book.Address ?? "");
        table.AddRow("[bold]ISBN[/]", book.Isbn ?? "");
        table.AddRow("[bold]Month[/]", $"{book.Month} (Month number: {book.GetMonthAsNumber()})");
        table.AddRow("[bold]Language[/]", book.Language ?? "");
        table.AddRow("[bold]Keywords[/]", book.Keywords ?? "");
        table.AddRow("[bold]Abstract[/]", book.Abstract ?? "");
            
        AnsiConsole.Write(table);
            
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[italic]BibSharp supports many extended fields beyond the basic BibTeX fields. These are properly handled during parsing and serialization.[/]");
    }
        
    private static void ExploreKeywords(BibEntry book)
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule("[yellow]Keyword Management[/]"));
        AnsiConsole.WriteLine();
            
        // Show current keywords
        AnsiConsole.MarkupLine("[bold]Current keywords:[/]");
            
        var keywords = book.GetKeywordList();
            
        var grid = new Grid();
        grid.AddColumn();
            
        var tree = new Tree("Keywords");
        tree.Style = Style.Parse("orange3");
            
        foreach (var keyword in keywords)
        {
            tree.AddNode(keyword);
        }
            
        grid.AddRow(tree);
        AnsiConsole.Write(grid);
            
        // Demonstrate adding a keyword
        AnsiConsole.WriteLine();
        if (AnsiConsole.Confirm("Would you like to add a new keyword?"))
        {
            var newKeyword = AnsiConsole.Ask<string>("Enter a new keyword: ");
                
            AnsiConsole.Status()
                .Start("Adding keyword...", ctx => 
                {
                    book.AddKeyword(newKeyword);
                    System.Threading.Thread.Sleep(500); // For demonstration
                });
                
            AnsiConsole.MarkupLine($"[bold green]Added keyword:[/] {newKeyword}");
                
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[bold]Updated keywords:[/]");
                
            var updatedKeywords = book.GetKeywordList();
            var updatedTree = new Tree("Keywords");
            updatedTree.Style = Style.Parse("green");
                
            foreach (var keyword in updatedKeywords)
            {
                var node = updatedTree.AddNode(keyword);
                if (keyword == newKeyword)
                {
                    // No need to style individual nodes
                }
            }
                
            AnsiConsole.Write(updatedTree);
        }
            
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[italic]BibSharp provides methods to manage keywords as lists, making it easy to add, remove, or modify keywords in BibTeX entries.[/]");
    }
        
    private static void ModifyEntry(BibEntry book)
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule("[yellow]Modify Entry Fields[/]"));
        AnsiConsole.WriteLine();
            
        var fieldPrompt = new SelectionPrompt<string>()
            .Title("Select a field to modify")
            .PageSize(10)
            .AddChoices(
                "Title",
                "Year",
                "Publisher",
                "Month",
                "Add a custom field");
            
        var field = AnsiConsole.Prompt(fieldPrompt);
            
        switch (field)
        {
            case "Title":
                var newTitle = AnsiConsole.Ask<string>("Enter new title: ", book.Title ?? "");
                book.Title = newTitle;
                break;
            case "Year":
                var yearStr = AnsiConsole.Ask<string>("Enter new year: ", book.Year?.ToString() ?? "");
                if (int.TryParse(yearStr, out int year))
                {
                    book.Year = year;
                }
                break;
            case "Publisher":
                var newPublisher = AnsiConsole.Ask<string>("Enter new publisher: ", book.Publisher ?? "");
                book.Publisher = newPublisher;
                break;
            case "Month":
                var monthNumberPrompt = new SelectionPrompt<int>()
                    .Title("Select a month")
                    .PageSize(12)
                    .AddChoices(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 });
                    
                var monthNumber = AnsiConsole.Prompt(monthNumberPrompt);
                    
                var abbreviated = AnsiConsole.Confirm("Use abbreviated month name?", true);
                book.SetMonthByNumber(monthNumber, abbreviated);
                break;
            case "Add a custom field":
                var fieldName = AnsiConsole.Ask<string>("Enter field name: ");
                var fieldValue = AnsiConsole.Ask<string>("Enter field value: ");
                book[fieldName] = fieldValue;
                break;
        }
            
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[bold green]Entry updated![/]");
            
        // Show updated entry in BibTeX format
        var panel = new Panel(book.ToBibTeX())
        {
            Header = new PanelHeader("Updated BibTeX"),
            Border = BoxBorder.Rounded,
            Expand = true
        };
            
        AnsiConsole.WriteLine();
        AnsiConsole.Write(panel);
    }
        
    private static void ShowBibTeXOutput(BibEntry book)
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule("[yellow]BibTeX Output[/]"));
        AnsiConsole.WriteLine();
            
        var panel = new Panel(book.ToBibTeX())
        {
            Header = new PanelHeader("BibTeX Entry with Extended Fields"),
            Border = BoxBorder.Rounded,
            Expand = true
        };
            
        AnsiConsole.Write(panel);
            
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[italic]Notice how BibSharp properly formats all the extended fields in the BibTeX output.[/]");
            
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[bold]Extended fields include:[/]");
            
        var table = new Table();
        table.Border = TableBorder.Rounded;
            
        table.AddColumn("Field");
        table.AddColumn("Description");
            
        table.AddRow("abstract", "Abstract or summary of the publication");
        table.AddRow("address", "Publisher's address or location");
        table.AddRow("edition", "Edition of the book");
        table.AddRow("isbn", "International Standard Book Number");
        table.AddRow("keywords", "List of keywords for indexing and classification");
        table.AddRow("language", "Language of the publication");
        table.AddRow("series", "Series in which the book was published");
            
        AnsiConsole.Write(table);
    }
}