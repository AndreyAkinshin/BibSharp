using System.Collections.Generic;
using BibSharp.Models;
using BibSharp.Utilities;
using Spectre.Console;

namespace BibSharp.Demo;

class BibFormatterDemo
{
    public static void RunDemo()
    {
        AnsiConsole.Write(
            new FigletText("Citation Styles")
                .Centered()
                .Color(Color.Yellow));
            
        // Create example entries
        var entries = CreateSampleEntries();
            
        // Format individual entries in different citation styles
        var article = entries[0];
        var book = entries[1];
        var inProceedings = entries[2];
            
        // Show available citation styles
        var stylePrompt = new SelectionPrompt<CitationStyle>()
            .Title("Choose a citation style to explore")
            .PageSize(10)
            .AddChoices(
                CitationStyle.Chicago,
                CitationStyle.Mla,
                CitationStyle.Apa,
                CitationStyle.Harvard,
                CitationStyle.Ieee
            );
            
        var style = AnsiConsole.Prompt(stylePrompt);
            
        AnsiConsole.Clear();
            
        // Display header for the selected style
        AnsiConsole.Write(new Rule($"[bold yellow]{style} Style Citations[/]"));
            
        // Show entry type tabs
        var entryTypePrompt = new SelectionPrompt<string>()
            .Title("Select an entry type to view")
            .PageSize(5)
            .AddChoices("Article", "Book", "Conference Paper", "Full Bibliography");
            
        var entryType = AnsiConsole.Prompt(entryTypePrompt);
            
        switch (entryType)
        {
            case "Article":
                DisplayFormattedEntry(article, style);
                break;
            case "Book":
                DisplayFormattedEntry(book, style);
                break;
            case "Conference Paper":
                DisplayFormattedEntry(inProceedings, style);
                break;
            case "Full Bibliography":
                DisplayBibliography(entries, style);
                break;
        }
    }
        
    private static void DisplayFormattedEntry(BibEntry entry, CitationStyle style)
    {
        var formattedText = BibFormatter.Format(entry, style);
            
        // Show entry details and formatted citation
        var grid = new Grid()
            .AddColumn(new GridColumn().NoWrap());
            
        var panel = new Panel(entry.ToBibTeX())
        {
            Header = new PanelHeader("BibTeX Source"),
            Border = BoxBorder.Rounded,
            Expand = true
        };
            
        grid.AddRow(panel);
            
        AnsiConsole.Write(grid);
            
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[bold yellow]Formatted Citation:[/]");
        AnsiConsole.WriteLine();
            
        var citationPanel = new Panel(formattedText)
        {
            Border = BoxBorder.Rounded,
            Expand = true,
            Padding = new Padding(1, 0, 1, 0)
        };
            
        AnsiConsole.Write(citationPanel);
    }
        
    private static void DisplayBibliography(List<BibEntry> entries, CitationStyle style)
    {
        var formattedBibliography = BibFormatter.FormatBibliography(entries, style);
            
        AnsiConsole.MarkupLine("[bold yellow]Formatted Bibliography:[/]");
        AnsiConsole.WriteLine();
            
        var bibliographyPanel = new Panel(formattedBibliography)
        {
            Border = BoxBorder.Rounded,
            Expand = true,
            Padding = new Padding(1, 0, 1, 0)
        };
            
        AnsiConsole.Write(bibliographyPanel);
    }
        
    private static List<BibEntry> CreateSampleEntries()
    {
        // Create an article entry
        var article = new BibEntry("article")
        {
            Key = "smith2023",
            Title = "Recent Advances in Citation Formatting",
            Year = 2023,
            Journal = "Journal of Academic Publishing",
            Volume = 15,
            Number = "2",
            Pages = new PageRange("123--145"),
            Doi = "10.1000/j.acad.2023.04.005",
            Url = "https://example.org/publications/j-acad-pub/15/2/123"
        };
            
        article.AddAuthor(new Author("Smith", "John", "A."));
        article.AddAuthor(new Author("Johnson", "Robert", "B."));
        article.AddAuthor(new Author("Williams", "Sarah", "C."));
            
        // Create a book entry
        var book = new BibEntry("book")
        {
            Key = "miller2022",
            Title = "The Complete Guide to Bibliographic References",
            Year = 2022,
            Publisher = "Academic Publishing House",
            Address = "New York, NY",
            Edition = "3rd",
            Isbn = "978-3-16-148410-0"
        };
            
        book.AddAuthor(new Author("Miller", "Alice", "D."));
        book.AddAuthor(new Author("Thompson", "David", "E."));
            
        // Create a conference paper entry
        var inProceedings = new BibEntry("inproceedings")
        {
            Key = "brown2021",
            Title = "Automating Citation Style Formatting",
            Year = 2021,
            BookTitle = "Proceedings of the 10th International Conference on Document Engineering",
            Pages = new PageRange("78--92"),
            Address = "London, UK",
            Doi = "10.1109/doceng.2021.12345",
            Publisher = "IEEE Press"
        };
            
        inProceedings.AddAuthor(new Author("Brown", "Thomas", "F."));
            
        inProceedings.AddEditor(new Author("Davis", "Michael", "G."));
        inProceedings.AddEditor(new Author("Wilson", "Jennifer", "H."));
            
        return new List<BibEntry> { article, book, inProceedings };
    }
}