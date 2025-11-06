using BibSharp.Models;
using BibSharp.Utilities;

namespace BibSharp.Tests.Utilities;

public class BibComparerComprehensiveTests
{
    [Fact]
    public void ByAuthor_BothHaveAuthors_ComparesCorrectly()
    {
        var entry1 = new BibEntry { Key = "test1" };
        entry1.AddAuthor(new Author("Aardvark", "John"));
        
        var entry2 = new BibEntry { Key = "test2" };
        entry2.AddAuthor(new Author("Zebra", "Jane"));

        var comparer = BibComparer.ByAuthor();
        var result = comparer.Compare(entry1, entry2);

        Assert.True(result < 0); // Aardvark < Zebra
    }

    [Fact]
    public void ByAuthor_BothNoAuthors_ReturnsZero()
    {
        var entry1 = new BibEntry { Key = "test1" };
        var entry2 = new BibEntry { Key = "test2" };

        var comparer = BibComparer.ByAuthor();
        var result = comparer.Compare(entry1, entry2);

        Assert.Equal(0, result);
    }

    [Fact]
    public void ByAuthor_OneHasNoAuthor_SortsCorrectly()
    {
        var entry1 = new BibEntry { Key = "test1" };
        
        var entry2 = new BibEntry { Key = "test2" };
        entry2.AddAuthor(new Author("Smith", "John"));

        var comparer = BibComparer.ByAuthor();
        var result = comparer.Compare(entry1, entry2);

        Assert.True(result < 0); // null author comes first
    }

    [Fact]
    public void ByAuthor_CaseInsensitive()
    {
        var entry1 = new BibEntry { Key = "test1" };
        entry1.AddAuthor(new Author("smith", "john"));
        
        var entry2 = new BibEntry { Key = "test2" };
        entry2.AddAuthor(new Author("SMITH", "JOHN"));

        var comparer = BibComparer.ByAuthor();
        var result = comparer.Compare(entry1, entry2);

        Assert.Equal(0, result);
    }

    [Fact]
    public void ByYear_Descending_SortsNewestFirst()
    {
        var entry1 = new BibEntry { Key = "test1", Year = 2020 };
        var entry2 = new BibEntry { Key = "test2", Year = 2025 };

        var comparer = BibComparer.ByYear(descending: true);
        var result = comparer.Compare(entry1, entry2);

        Assert.True(result > 0); // 2020 comes after 2025 in descending order
    }

    [Fact]
    public void ByYear_Ascending_SortsOldestFirst()
    {
        var entry1 = new BibEntry { Key = "test1", Year = 2020 };
        var entry2 = new BibEntry { Key = "test2", Year = 2025 };

        var comparer = BibComparer.ByYear(descending: false);
        var result = comparer.Compare(entry1, entry2);

        Assert.True(result < 0); // 2020 comes before 2025 in ascending order
    }

    [Fact]
    public void ByYear_BothNull_ReturnsZero()
    {
        var entry1 = new BibEntry { Key = "test1" };
        var entry2 = new BibEntry { Key = "test2" };

        var comparer = BibComparer.ByYear();
        var result = comparer.Compare(entry1, entry2);

        Assert.Equal(0, result);
    }

    [Fact]
    public void ByYear_OneNull_SortsCorrectly()
    {
        var entry1 = new BibEntry { Key = "test1" };
        var entry2 = new BibEntry { Key = "test2", Year = 2025 };

        var comparer = BibComparer.ByYear(descending: true);
        var result = comparer.Compare(entry1, entry2);

        Assert.True(result > 0); // null (0) after 2025 in descending
    }

    [Fact]
    public void ByTitle_ComparesCorrectly()
    {
        var entry1 = new BibEntry { Key = "test1", Title = "Alpha" };
        var entry2 = new BibEntry { Key = "test2", Title = "Beta" };

        var comparer = BibComparer.ByTitle();
        var result = comparer.Compare(entry1, entry2);

        Assert.True(result < 0); // Alpha < Beta
    }

    [Fact]
    public void ByTitle_CaseInsensitive()
    {
        var entry1 = new BibEntry { Key = "test1", Title = "test" };
        var entry2 = new BibEntry { Key = "test2", Title = "TEST" };

        var comparer = BibComparer.ByTitle();
        var result = comparer.Compare(entry1, entry2);

        Assert.Equal(0, result);
    }

    [Fact]
    public void ByTitle_BothNull_ReturnsZero()
    {
        var entry1 = new BibEntry { Key = "test1" };
        var entry2 = new BibEntry { Key = "test2" };

        var comparer = BibComparer.ByTitle();
        var result = comparer.Compare(entry1, entry2);

        Assert.Equal(0, result);
    }

    [Fact]
    public void ByKey_ComparesCorrectly()
    {
        var entry1 = new BibEntry { Key = "aaa" };
        var entry2 = new BibEntry { Key = "zzz" };

        var comparer = BibComparer.ByKey();
        var result = comparer.Compare(entry1, entry2);

        Assert.True(result < 0); // aaa < zzz
    }

    [Fact]
    public void ByKey_CaseInsensitive()
    {
        var entry1 = new BibEntry { Key = "test" };
        var entry2 = new BibEntry { Key = "TEST" };

        var comparer = BibComparer.ByKey();
        var result = comparer.Compare(entry1, entry2);

        Assert.Equal(0, result);
    }

    [Fact]
    public void ByField_ComparesCustomField()
    {
        var entry1 = new BibEntry { Key = "test1" };
        entry1["custom"] = "Alpha";
        
        var entry2 = new BibEntry { Key = "test2" };
        entry2["custom"] = "Beta";

        var comparer = BibComparer.ByField("custom");
        var result = comparer.Compare(entry1, entry2);

        Assert.True(result < 0); // Alpha < Beta
    }

    [Fact]
    public void ByField_BothNull_ReturnsZero()
    {
        var entry1 = new BibEntry { Key = "test1" };
        var entry2 = new BibEntry { Key = "test2" };

        var comparer = BibComparer.ByField("nonexistent");
        var result = comparer.Compare(entry1, entry2);

        Assert.Equal(0, result);
    }

    [Fact]
    public void Chain_CombinesComparers()
    {
        var entry1 = new BibEntry { Key = "test1", Year = 2025, Title = "Alpha" };
        entry1.AddAuthor(new Author("Smith", "John"));
        
        var entry2 = new BibEntry { Key = "test2", Year = 2025, Title = "Beta" };
        entry2.AddAuthor(new Author("Smith", "Jane"));

        var comparer = BibComparer.Chain(
            BibComparer.ByYear(),
            BibComparer.ByTitle()
        );
        
        var result = comparer.Compare(entry1, entry2);

        // Same year, so should compare by title
        Assert.True(result < 0); // Alpha < Beta
    }

    [Fact]
    public void Chain_UsesFirstNonZeroResult()
    {
        var entry1 = new BibEntry { Key = "test1", Year = 2020, Title = "Zebra" };
        var entry2 = new BibEntry { Key = "test2", Year = 2025, Title = "Alpha" };

        var comparer = BibComparer.Chain(
            BibComparer.ByYear(descending: false),
            BibComparer.ByTitle()
        );
        
        var result = comparer.Compare(entry1, entry2);

        // Different years, so title shouldn't matter
        Assert.True(result < 0); // 2020 < 2025 (ascending)
    }

    [Fact]
    public void OrderBy_WithComparer_WorksCorrectly()
    {
        var entries = new[]
        {
            new BibEntry { Key = "test3", Year = 2020 },
            new BibEntry { Key = "test1", Year = 2025 },
            new BibEntry { Key = "test2", Year = 2022 }
        };

        var sorted = entries.OrderBy(e => e, BibComparer.ByYear(descending: false)).ToList();

        Assert.Equal(2020, sorted[0].Year);
        Assert.Equal(2022, sorted[1].Year);
        Assert.Equal(2025, sorted[2].Year);
    }
}

