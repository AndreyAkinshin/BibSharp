using BibSharp.Models;
using BibSharp.Utilities;

namespace BibSharp.Tests;

public class RealWorldBibTeXTests
{
    [Fact]
    public void Parse_RealWorldArticle_WithAllFields()
    {
        var bibtex = @"
@article{knuth1984literate,
  author = {Knuth, Donald E.},
  title = {Literate Programming},
  journal = {The Computer Journal},
  year = {1984},
  volume = {27},
  number = {2},
  pages = {97--111},
  month = {May},
  doi = {10.1093/comjnl/27.2.97},
  url = {https://academic.oup.com/comjnl/article/27/2/97/343244},
  issn = {0010-4620},
  abstract = {The author and his associates have been experimenting for the past several years with a programming language and documentation system called WEB.},
  keywords = {programming, documentation, literate programming}
}";

        var parser = new BibParser();
        var entries = parser.ParseString(bibtex);

        Assert.Single(entries);
        var entry = entries[0];
        Assert.Equal("knuth1984literate", entry.Key);
        Assert.Equal("Knuth", entry.Authors[0].LastName);
        Assert.Equal("Donald", entry.Authors[0].FirstName);
        Assert.Equal("E.", entry.Authors[0].MiddleName);
        Assert.Equal("Literate Programming", entry.Title);
        Assert.Equal("The Computer Journal", entry.Journal);
        Assert.Equal(1984, entry.Year);
        Assert.Equal(27, entry.Volume);
        Assert.Equal("2", entry.Number);
        Assert.Equal(97, entry.Pages?.StartPage);
        Assert.Equal(111, entry.Pages?.EndPage);
        Assert.NotNull(entry.Doi);
        Assert.NotNull(entry.Abstract);
        Assert.NotNull(entry.Keywords);
    }

    [Fact]
    public void Parse_RealWorldBook_EditedVolume()
    {
        var bibtex = @"
@book{gamma1994design,
  author = {Gamma, Erich and Helm, Richard and Johnson, Ralph and Vlissides, John},
  title = {Design Patterns: Elements of Reusable Object-Oriented Software},
  publisher = {Addison-Wesley},
  year = {1994},
  isbn = {0-201-63361-2},
  address = {Reading, MA}
}";

        var parser = new BibParser();
        var entries = parser.ParseString(bibtex);

        Assert.Single(entries);
        var entry = entries[0];
        Assert.Equal(4, entry.Authors.Count);
        Assert.Equal("Gamma", entry.Authors[0].LastName);
        Assert.Equal("Helm", entry.Authors[1].LastName);
        Assert.Equal("Johnson", entry.Authors[2].LastName);
        Assert.Equal("Vlissides", entry.Authors[3].LastName);
    }

    [Fact]
    public void Parse_RealWorldConferencePaper()
    {
        var bibtex = @"
@inproceedings{lecun1998gradient,
  author = {LeCun, Yann and Bottou, L\'{e}on and Bengio, Yoshua and Haffner, Patrick},
  title = {Gradient-based learning applied to document recognition},
  booktitle = {Proceedings of the IEEE},
  year = {1998},
  volume = {86},
  number = {11},
  pages = {2278--2324},
  publisher = {IEEE}
}";

        var parser = new BibParser();
        var entries = parser.ParseString(bibtex);

        Assert.Single(entries);
        var entry = entries[0];
        Assert.Equal("lecun1998gradient", entry.Key);
        Assert.Equal(4, entry.Authors.Count);
        Assert.Equal("LeCun", entry.Authors[0].LastName);
        Assert.Contains("Gradient", entry.Title);
    }

    [Fact]
    public void Parse_PhdThesis()
    {
        var bibtex = @"
@phdthesis{turing1938systems,
  author = {Turing, Alan Mathison},
  title = {Systems of Logic Based on Ordinals},
  school = {Princeton University},
  year = {1938},
  type = {PhD dissertation},
  address = {Princeton, NJ}
}";

        var parser = new BibParser();
        var entries = parser.ParseString(bibtex);

        Assert.Single(entries);
        var entry = entries[0];
        Assert.Equal(EntryType.PhdThesis, entry.EntryType);
        Assert.Equal("Turing", entry.Authors[0].LastName);
        Assert.Equal("Alan", entry.Authors[0].FirstName);
        Assert.Equal("Mathison", entry.Authors[0].MiddleName);
        Assert.Equal("Princeton University", entry.School);
    }

    [Fact]
    public void Parse_TechReport()
    {
        var bibtex = @"
@techreport{lamport1986latex,
  author = {Lamport, Leslie},
  title = {LaTeX: A Document Preparation System},
  institution = {Stanford University},
  year = {1986},
  number = {STAN-CS-86-1115},
  type = {Technical Report}
}";

        var parser = new BibParser();
        var entries = parser.ParseString(bibtex);

        Assert.Single(entries);
        var entry = entries[0];
        Assert.Equal(EntryType.TechReport, entry.EntryType);
        Assert.Equal("Lamport", entry.Authors[0].LastName);
        Assert.Equal("Stanford University", entry.Institution);
    }

    [Fact]
    public void RoundTrip_ComplexEntry_PreservesAll()
    {
        var original = new BibEntry(EntryType.Article)
        {
            Key = "complex2025",
            Title = "A Complex Study with Special Characters: Café & Résumé",
            Journal = "International Journal of Testing",
            Year = 2025,
            Volume = 15,
            Number = "3",
            Doi = "10.1234/ijt.2025.15.3.123",
            Url = "https://example.com/article",
            Issn = "1234-5678",
            Abstract = "This is a comprehensive abstract that discusses various aspects of the study.",
            Keywords = "testing, validation, comprehensive",
            Language = "en",
            Note = "Published online first"
        };
        original.AddAuthor(new Author("Smith", "John", "Andrew", "Jr."));
        original.AddAuthor(new Author("Müller", "Hans"));
        original.AddEditor(new Author("García", "María"));
        original.Pages = new PageRange(123, 145);

        var serializer = new BibSerializer(new BibSerializerSettings
        {
            ValidateBeforeSerialization = false,
            Encoding = BibEncoding.Unicode
        });

        var bibtex = serializer.Serialize(new[] { original });

        var parser = new BibParser();
        var reparsed = parser.ParseString(bibtex);

        Assert.Single(reparsed);
        var entry = reparsed[0];
        
        Assert.Equal(original.Key, entry.Key);
        Assert.Equal(original.Title, entry.Title);
        Assert.Equal(original.Journal, entry.Journal);
        Assert.Equal(original.Year, entry.Year);
        Assert.Equal(original.Volume, entry.Volume);
        Assert.Equal(original.Authors.Count, entry.Authors.Count);
        Assert.Equal(original.Editors.Count, entry.Editors.Count);
    }

    [Fact]
    public void Serialize_Bibliography_MultipleEntries()
    {
        var entries = new[]
        {
            new BibEntry(EntryType.Article) { Key = "article1", Title = "Article 1", Year = 2025 },
            new BibEntry(EntryType.Book) { Key = "book1", Title = "Book 1", Publisher = "P", Year = 2024 },
            new BibEntry(EntryType.InProceedings) { Key = "paper1", Title = "Paper 1", BookTitle = "Conf", Year = 2023 }
        };

        foreach (var entry in entries)
        {
            entry.AddAuthor(new Author("Author", "Test"));
        }

        var serializer = new BibSerializer(new BibSerializerSettings
        {
            ValidateBeforeSerialization = false
        });

        var bibtex = serializer.Serialize(entries);

        Assert.Contains("@article{article1,", bibtex);
        Assert.Contains("@book{book1,", bibtex);
        Assert.Contains("@inproceedings{paper1,", bibtex);
    }
}

