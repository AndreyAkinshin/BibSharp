using System.Collections.Generic;
using BibSharp.Models;
using BibSharp.Utilities;
using Spectre.Console;

namespace BibSharp.Demo;

class InternationalizationDemo
{
    public static void RunDemo()
    {
        AnsiConsole.Write(
            new FigletText("i18n Support")
                .Centered()
                .Color(Color.Purple));
            
        AnsiConsole.WriteLine();
            
        var choicePrompt = new SelectionPrompt<string>()
            .Title("Select a demo to explore")
            .PageSize(10)
            .AddChoices(
                "Multilingual entries",
                "LaTeX encoding conversion",
                "BibTexEncoder utilities",
                "Character support comparison");
            
        var choice = AnsiConsole.Prompt(choicePrompt);
            
        switch (choice)
        {
            case "Multilingual entries":
                ShowMultilingualEntries();
                break;
            case "LaTeX encoding conversion":
                ShowLatexConversion();
                break;
            case "BibTexEncoder utilities":
                ShowBibTexEncoder();
                break;
            case "Character support comparison":
                ShowCharacterComparison();
                break;
        }
    }
        
    private static void ShowMultilingualEntries()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule("[yellow]Multilingual BibTeX Entries[/]"));
        AnsiConsole.WriteLine();
            
        // Create entries in different languages
        AnsiConsole.Status()
            .Start("Creating sample multilingual entries...", ctx => 
            {
                // Just a small delay for visual effect
                System.Threading.Thread.Sleep(600);
            });
            
        var entries = new List<BibEntry>
        {
            CreateFrenchEntry(),
            CreateGermanEntry(),
            CreateSpanishEntry()
        };
            
        // Display the entries
        var languagePrompt = new SelectionPrompt<string>()
            .Title("Choose a language to view")
            .PageSize(4)
            .AddChoices("French", "German", "Spanish", "All");
            
        var languageChoice = AnsiConsole.Prompt(languagePrompt);
            
        BibEntry? selectedEntry = null;
            
        switch (languageChoice)
        {
            case "French":
                selectedEntry = entries[0];
                break;
            case "German":
                selectedEntry = entries[1];
                break;
            case "Spanish":
                selectedEntry = entries[2];
                break;
        }
            
        // Show selected entry or all entries
        if (selectedEntry != null)
        {
            ShowSingleEntry(selectedEntry, languageChoice);
        }
        else
        {
            ShowAllEntries(entries);
        }
    }
        
    private static void ShowSingleEntry(BibEntry entry, string language)
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule($"[yellow]{language} BibTeX Entry[/]"));
        AnsiConsole.WriteLine();
            
        // Show the entry details
        var table = new Table();
        table.Border = TableBorder.Rounded;
            
        table.AddColumn("Field");
        table.AddColumn("Value");
            
        table.AddRow("[bold]Key[/]", entry.Key ?? "");
        table.AddRow("[bold]Title[/]", entry.Title ?? "");
            
        // Format authors
        var authorNames = new List<string>();
        foreach (var author in entry.Authors)
        {
            authorNames.Add(author.ToDisplayString());
        }
            
        table.AddRow("[bold]Authors[/]", string.Join(", ", authorNames));
        table.AddRow("[bold]Journal[/]", entry.Journal ?? "");
        table.AddRow("[bold]Year[/]", entry.Year?.ToString() ?? "");
        table.AddRow("[bold]Volume[/]", entry.Volume?.ToString() ?? "");
        table.AddRow("[bold]Pages[/]", entry.Pages?.ToString() ?? "");
            
        AnsiConsole.Write(table);
            
        // Show encoding options
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[bold]Select encoding format:[/]");
            
        var encodingPrompt = new SelectionPrompt<string>()
            .AddChoices("Unicode", "LaTeX");
            
        var encodingChoice = AnsiConsole.Prompt(encodingPrompt);
            
        BibEncoding encoding = encodingChoice == "Unicode" ? BibEncoding.Unicode : BibEncoding.LaTeX;
            
        var serializer = new BibSerializer(new BibSerializerSettings { Encoding = encoding });
        string serialized = serializer.Serialize(new[] { entry });
            
        var panel = new Panel(serialized)
        {
            Header = new PanelHeader($"{encoding} Encoding"),
            Border = BoxBorder.Rounded,
            Expand = true
        };
            
        AnsiConsole.WriteLine();
        AnsiConsole.Write(panel);
            
        // Show explanation
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[italic]Notice how special characters are preserved with Unicode encoding but converted to LaTeX escape sequences with LaTeX encoding.[/]");
    }
        
    private static void ShowAllEntries(List<BibEntry> entries)
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule("[yellow]Multilingual BibTeX Entries[/]"));
        AnsiConsole.WriteLine();
            
        // Choose encoding
        var encodingPrompt = new SelectionPrompt<string>()
            .Title("Choose an encoding format")
            .AddChoices("Unicode", "LaTeX");
            
        var encodingChoice = AnsiConsole.Prompt(encodingPrompt);
            
        BibEncoding encoding = encodingChoice == "Unicode" ? BibEncoding.Unicode : BibEncoding.LaTeX;
            
        // Create serializer with the selected encoding
        var serializer = new BibSerializer(new BibSerializerSettings { Encoding = encoding });
        string serialized = serializer.Serialize(entries);
            
        // Create a panel for the serialized content
        var panel = new Panel(serialized)
        {
            Header = new PanelHeader($"Entries with {encoding} Encoding"),
            Border = BoxBorder.Rounded,
            Expand = true
        };
            
        AnsiConsole.Write(panel);
    }
        
    private static void ShowLatexConversion()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule("[yellow]LaTeX Encoding Conversion[/]"));
        AnsiConsole.WriteLine();
            
        // Example of LaTeX encoded BibTeX
        string latexEncoded = @"@article{m\""uller2024,
  author = {M\""uller, J\""org and Garc\'ia, Jos\'e},
  title = {Unicode and BibTeX: Managing Special Characters},
  journal = {Journal of Documentation},
  year = {2024},
  volume = {10},
  pages = {1--20}
}";
            
        // Display the LaTeX encoded content
        var originalPanel = new Panel(latexEncoded)
        {
            Header = new PanelHeader("Original LaTeX Entry"),
            Border = BoxBorder.Rounded,
            Expand = true
        };
            
        AnsiConsole.Write(originalPanel);
        AnsiConsole.WriteLine();
            
        // Parse with LaTeX to Unicode conversion
        AnsiConsole.Status()
            .Start("Parsing entry with LaTeX to Unicode conversion...", ctx => 
            {
                System.Threading.Thread.Sleep(800); // Just for visual effect
            });
            
        var parser = new BibParser(new BibParserSettings
        {
            ConvertLatexToUnicode = true
        });
            
        var parsedEntries = parser.ParseString(latexEncoded);
            
        if (parsedEntries.Count > 0)
        {
            var entry = parsedEntries[0];
                
            // Show the parsed entry
            var table = new Table();
            table.Border = TableBorder.Rounded;
                
            table.AddColumn("Field");
            table.AddColumn("Value");
                
            table.AddRow("[bold]Key[/]", entry.Key ?? "");
            table.AddRow("[bold]Title[/]", entry.Title ?? "");
                
            // Format authors
            var authorNames = new List<string>();
            foreach (var author in entry.Authors)
            {
                authorNames.Add(author.ToDisplayString());
            }
                
            table.AddRow("[bold]Authors[/]", string.Join(", ", authorNames));
                
            AnsiConsole.MarkupLine("[bold green]After parsing with ConvertLatexToUnicode=true:[/]");
            AnsiConsole.WriteLine();
            AnsiConsole.Write(table);
                
            // Show the entry as Unicode BibTeX
            var unicodeSerializer = new BibSerializer(new BibSerializerSettings
            {
                Encoding = BibEncoding.Unicode
            });
                
            string unicodeSerialized = unicodeSerializer.Serialize(new[] { entry });
                
            var unicodePanel = new Panel(unicodeSerialized)
            {
                Header = new PanelHeader("Unicode Encoded"),
                Border = BoxBorder.Rounded,
                Expand = true
            };
                
            AnsiConsole.WriteLine();
            AnsiConsole.Write(unicodePanel);
        }
            
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[italic]BibSharp can automatically convert LaTeX encoded characters to Unicode when parsing BibTeX entries, making it easier to work with international characters.[/]");
    }
        
    private static void ShowBibTexEncoder()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule("[yellow]BibTexEncoder Utilities[/]"));
        AnsiConsole.WriteLine();
            
        // Let user input custom text
        var userText = AnsiConsole.Ask<string>("Enter text with special characters to encode: ", 
            "Characters from different languages: ä ö ü ß é è ê ñ ç å ø € ≈ ≤ ≥");
            
        // Show the encoding results
        string latexText = BibTexEncoder.ToLatex(userText);
        string backToUnicode = BibTexEncoder.ToUnicode(latexText);
            
        var table = new Table();
        table.Border = TableBorder.Rounded;
        table.Expand();
            
        table.AddColumn("Format");
        table.AddColumn("Text");
            
        table.AddRow("[bold]Original[/]", userText);
        table.AddRow("[bold]As LaTeX[/]", latexText);
        table.AddRow("[bold]Back to Unicode[/]", backToUnicode);
            
        AnsiConsole.Write(table);
            
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[italic]The BibTexEncoder class provides utility methods to convert between Unicode and LaTeX escape sequences. This is useful when working with BibTeX files that need to be compatible with traditional LaTeX systems.[/]");
            
        // Interactive encoder
        AnsiConsole.WriteLine();
        if (AnsiConsole.Confirm("Do you want to try the encoding converter interactively?"))
        {
            ShowInteractiveEncoder();
        }
    }
        
    private static void ShowInteractiveEncoder()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule("[yellow]Interactive Encoder[/]"));
        AnsiConsole.WriteLine();
            
        bool exit = false;
        while (!exit)
        {
            var modePrompt = new SelectionPrompt<string>()
                .Title("Choose conversion direction")
                .AddChoices("Unicode to LaTeX", "LaTeX to Unicode", "Exit");
                
            var mode = AnsiConsole.Prompt(modePrompt);
                
            if (mode == "Exit")
            {
                exit = true;
                continue;
            }
                
            string input = AnsiConsole.Ask<string>("Enter text to convert: ");
            string result = mode == "Unicode to LaTeX" 
                ? BibTexEncoder.ToLatex(input) 
                : BibTexEncoder.ToUnicode(input);
                
            AnsiConsole.MarkupLine($"[bold green]Result:[/] {result}");
            AnsiConsole.WriteLine();
        }
    }
        
    private static void ShowCharacterComparison()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule("[yellow]Character Support Comparison[/]"));
        AnsiConsole.WriteLine();
            
        // Create a table to compare character handling
        var table = new Table();
        table.Border = TableBorder.Rounded;
        table.Expand();
            
        table.AddColumn("Character Group");
        table.AddColumn("Example");
        table.AddColumn("Unicode (raw)");
        table.AddColumn("LaTeX Encoding");
            
        table.AddRow(
            "European Diacritics",
            "ä ö ü ß é è ê",
            "ä ö ü ß é è ê",
            @"\""a \""o \""u \ss \'e \`e \^e");
            
        table.AddRow(
            "Other Latin Characters",
            "ñ ç å ø",
            "ñ ç å ø",
            @"\~n \c{c} \aa \o");
            
        table.AddRow(
            "Symbols",
            "© ® ™ € £ ¥",
            "© ® ™ € £ ¥",
            @"\copyright \textregistered \texttrademark \euro \pounds \yen");
            
        table.AddRow(
            "Mathematical",
            "≈ ≤ ≥ ± ∑ ∫",
            "≈ ≤ ≥ ± ∑ ∫",
            @"$\approx$ $\leq$ $\geq$ $\pm$ $\sum$ $\int$");
            
        table.AddRow(
            "Greek Letters",
            "α β γ δ Ω",
            "α β γ δ Ω",
            @"$\alpha$ $\beta$ $\gamma$ $\delta$ $\Omega$");
            
        AnsiConsole.Write(table);
            
        // Explanation
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[italic]BibSharp handles various encoding formats to ensure compatibility between systems. The Unicode encoding preserves special characters as-is, while LaTeX encoding converts them to LaTeX escape sequences for use in LaTeX documents.[/]");
            
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[bold]When to use each encoding:[/]");
            
        var encodingTable = new Table();
        encodingTable.Border = TableBorder.Rounded;
            
        encodingTable.AddColumn("Encoding");
        encodingTable.AddColumn("Best for");
            
        encodingTable.AddRow("[bold]Unicode[/]", "Modern systems, web applications, and non-LaTeX outputs");
        encodingTable.AddRow("[bold]LaTeX[/]", "BibTeX files used in LaTeX documents");
            
        AnsiConsole.Write(encodingTable);
    }
        
    private static BibEntry CreateFrenchEntry()
    {
        var frenchEntry = new BibEntry("article")
        {
            Key = "dubois2025",
            Title = "L'Analyse des Données Scientifiques",
            Year = 2025,
            Journal = "Journal Français de Science",
            Volume = 15,
            Pages = new PageRange("45-60")
        };
            
        frenchEntry.AddAuthor(new Author("Dubois", "Amélie"));
        frenchEntry.AddAuthor(new Author("Léveillé", "François"));
            
        return frenchEntry;
    }
        
    private static BibEntry CreateGermanEntry()
    {
        var germanEntry = new BibEntry("article")
        {
            Key = "müller2025",
            Title = "Eine Studie über Datenverarbeitung",
            Year = 2025,
            Journal = "Zeitschrift für Informatik",
            Volume = 42,
            Pages = new PageRange("101-120")
        };
            
        germanEntry.AddAuthor(new Author("Müller", "Hans"));
        germanEntry.AddAuthor(new Author("Schäfer", "Eva"));
            
        return germanEntry;
    }
        
    private static BibEntry CreateSpanishEntry()
    {
        var spanishEntry = new BibEntry("article")
        {
            Key = "garcía2025",
            Title = "Avances en Programación: Un Análisis",
            Year = 2025,
            Journal = "Revista Española de Informática",
            Volume = 28,
            Pages = new PageRange("15-32")
        };
            
        spanishEntry.AddAuthor(new Author("García", "José"));
        spanishEntry.AddAuthor(new Author("Martínez", "María"));
            
        return spanishEntry;
    }
}