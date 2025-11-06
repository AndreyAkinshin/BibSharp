namespace BibSharp.Tests;

public class MonthTests
{
    [Fact]
    public void Constructor_CreatesMonth()
    {
        var month = new Month(1, "jan", "january");
        Assert.Equal(1, month.Number);
        Assert.Equal("jan", month.Abbreviation);
        Assert.Equal("january", month.FullName);
    }

    [Fact]
    public void Constructor_ConvertsToLowerCase()
    {
        var month = new Month(2, "FEB", "FEBRUARY");
        Assert.Equal("feb", month.Abbreviation);
        Assert.Equal("february", month.FullName);
    }

    [Fact]
    public void Constructor_ThrowsOnInvalidNumber()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Month(0, "invalid", "invalid"));
        Assert.Throws<ArgumentOutOfRangeException>(() => new Month(13, "invalid", "invalid"));
        Assert.Throws<ArgumentOutOfRangeException>(() => new Month(-1, "invalid", "invalid"));
    }

    [Fact]
    public void Constructor_ThrowsOnNullAbbreviation()
    {
        Assert.Throws<ArgumentNullException>(() => new Month(1, null!, "january"));
    }

    [Fact]
    public void Constructor_ThrowsOnNullFullName()
    {
        Assert.Throws<ArgumentNullException>(() => new Month(1, "jan", null!));
    }

    [Fact]
    public void ToString_ReturnsAbbreviation()
    {
        var month = Month.January;
        Assert.Equal("jan", month.ToString());
    }

    [Fact]
    public void ImplicitConversion_ToString()
    {
        Month month = Month.February;
        string value = month;
        Assert.Equal("feb", value);
    }

    [Fact]
    public void ImplicitConversion_ToInt()
    {
        Month month = Month.March;
        int value = month;
        Assert.Equal(3, value);
    }

    [Fact]
    public void Equals_SameMonth_ReturnsTrue()
    {
        var jan1 = Month.January;
        var jan2 = Month.January;
        Assert.True(jan1.Equals(jan2));
        Assert.True(jan1 == jan2);
        Assert.False(jan1 != jan2);
    }

    [Fact]
    public void Equals_DifferentMonth_ReturnsFalse()
    {
        Assert.False(Month.January.Equals(Month.February));
        Assert.False(Month.January == Month.February);
        Assert.True(Month.January != Month.February);
    }

    [Fact]
    public void Equals_WithObject_ReturnsFalse()
    {
        object obj = 1;
        Assert.False(Month.January.Equals(obj));
    }

    [Fact]
    public void GetHashCode_SameMonth_SameHash()
    {
        var jan1 = Month.January;
        var jan2 = Month.FromNumber(1);
        Assert.Equal(jan1.GetHashCode(), jan2.GetHashCode());
    }

    [Fact]
    public void FromNumber_AllMonths()
    {
        Assert.Equal(1, Month.FromNumber(1).Number);
        Assert.Equal(2, Month.FromNumber(2).Number);
        Assert.Equal(3, Month.FromNumber(3).Number);
        Assert.Equal(4, Month.FromNumber(4).Number);
        Assert.Equal(5, Month.FromNumber(5).Number);
        Assert.Equal(6, Month.FromNumber(6).Number);
        Assert.Equal(7, Month.FromNumber(7).Number);
        Assert.Equal(8, Month.FromNumber(8).Number);
        Assert.Equal(9, Month.FromNumber(9).Number);
        Assert.Equal(10, Month.FromNumber(10).Number);
        Assert.Equal(11, Month.FromNumber(11).Number);
        Assert.Equal(12, Month.FromNumber(12).Number);
    }

    [Fact]
    public void FromNumber_InvalidNumber_ThrowsException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Month.FromNumber(0));
        Assert.Throws<ArgumentOutOfRangeException>(() => Month.FromNumber(13));
        Assert.Throws<ArgumentOutOfRangeException>(() => Month.FromNumber(-1));
    }

    [Fact]
    public void TryParse_NumericString_ReturnsTrue()
    {
        Assert.True(Month.TryParse("1", out var month));
        Assert.Equal(1, month.Number);
        
        Assert.True(Month.TryParse("12", out month));
        Assert.Equal(12, month.Number);
    }

    [Fact]
    public void TryParse_Abbreviation_ReturnsTrue()
    {
        Assert.True(Month.TryParse("jan", out var month));
        Assert.Equal(1, month.Number);
        
        Assert.True(Month.TryParse("JAN", out month));
        Assert.Equal(1, month.Number);
        
        Assert.True(Month.TryParse("dec", out month));
        Assert.Equal(12, month.Number);
    }

    [Fact]
    public void TryParse_FullName_ReturnsTrue()
    {
        Assert.True(Month.TryParse("january", out var month));
        Assert.Equal(1, month.Number);
        
        Assert.True(Month.TryParse("JANUARY", out month));
        Assert.Equal(1, month.Number);
        
        Assert.True(Month.TryParse("december", out month));
        Assert.Equal(12, month.Number);
    }

    [Fact]
    public void TryParse_AllMonthAbbreviations()
    {
        Assert.True(Month.TryParse("jan", out var m) && m.Number == 1);
        Assert.True(Month.TryParse("feb", out m) && m.Number == 2);
        Assert.True(Month.TryParse("mar", out m) && m.Number == 3);
        Assert.True(Month.TryParse("apr", out m) && m.Number == 4);
        Assert.True(Month.TryParse("may", out m) && m.Number == 5);
        Assert.True(Month.TryParse("jun", out m) && m.Number == 6);
        Assert.True(Month.TryParse("jul", out m) && m.Number == 7);
        Assert.True(Month.TryParse("aug", out m) && m.Number == 8);
        Assert.True(Month.TryParse("sep", out m) && m.Number == 9);
        Assert.True(Month.TryParse("oct", out m) && m.Number == 10);
        Assert.True(Month.TryParse("nov", out m) && m.Number == 11);
        Assert.True(Month.TryParse("dec", out m) && m.Number == 12);
    }

    [Fact]
    public void TryParse_AllMonthFullNames()
    {
        Assert.True(Month.TryParse("january", out var m) && m.Number == 1);
        Assert.True(Month.TryParse("february", out m) && m.Number == 2);
        Assert.True(Month.TryParse("march", out m) && m.Number == 3);
        Assert.True(Month.TryParse("april", out m) && m.Number == 4);
        Assert.True(Month.TryParse("may", out m) && m.Number == 5);
        Assert.True(Month.TryParse("june", out m) && m.Number == 6);
        Assert.True(Month.TryParse("july", out m) && m.Number == 7);
        Assert.True(Month.TryParse("august", out m) && m.Number == 8);
        Assert.True(Month.TryParse("september", out m) && m.Number == 9);
        Assert.True(Month.TryParse("october", out m) && m.Number == 10);
        Assert.True(Month.TryParse("november", out m) && m.Number == 11);
        Assert.True(Month.TryParse("december", out m) && m.Number == 12);
    }

    [Fact]
    public void TryParse_InvalidString_ReturnsFalse()
    {
        Assert.False(Month.TryParse("invalid", out var month));
        Assert.Equal(0, month.Number);
    }

    [Fact]
    public void TryParse_NullOrEmpty_ReturnsFalse()
    {
        Assert.False(Month.TryParse(null!, out var month));
        Assert.False(Month.TryParse("", out month));
        Assert.False(Month.TryParse("   ", out month));
    }

    [Fact]
    public void TryParse_InvalidNumber_ReturnsFalse()
    {
        Assert.False(Month.TryParse("0", out var month));
        Assert.False(Month.TryParse("13", out month));
        Assert.False(Month.TryParse("-1", out month));
    }

    [Fact]
    public void TryParse_WithWhitespace_ReturnsTrue()
    {
        Assert.True(Month.TryParse("  jan  ", out var month));
        Assert.Equal(1, month.Number);
    }

    [Fact]
    public void PredefinedMonths_January()
    {
        Assert.Equal(1, Month.January.Number);
        Assert.Equal("jan", Month.January.Abbreviation);
        Assert.Equal("january", Month.January.FullName);
    }

    [Fact]
    public void PredefinedMonths_February()
    {
        Assert.Equal(2, Month.February.Number);
        Assert.Equal("feb", Month.February.Abbreviation);
        Assert.Equal("february", Month.February.FullName);
    }

    [Fact]
    public void PredefinedMonths_March()
    {
        Assert.Equal(3, Month.March.Number);
        Assert.Equal("mar", Month.March.Abbreviation);
        Assert.Equal("march", Month.March.FullName);
    }

    [Fact]
    public void PredefinedMonths_April()
    {
        Assert.Equal(4, Month.April.Number);
        Assert.Equal("apr", Month.April.Abbreviation);
        Assert.Equal("april", Month.April.FullName);
    }

    [Fact]
    public void PredefinedMonths_May()
    {
        Assert.Equal(5, Month.May.Number);
        Assert.Equal("may", Month.May.Abbreviation);
        Assert.Equal("may", Month.May.FullName);
    }

    [Fact]
    public void PredefinedMonths_June()
    {
        Assert.Equal(6, Month.June.Number);
        Assert.Equal("jun", Month.June.Abbreviation);
        Assert.Equal("june", Month.June.FullName);
    }

    [Fact]
    public void PredefinedMonths_July()
    {
        Assert.Equal(7, Month.July.Number);
        Assert.Equal("jul", Month.July.Abbreviation);
        Assert.Equal("july", Month.July.FullName);
    }

    [Fact]
    public void PredefinedMonths_August()
    {
        Assert.Equal(8, Month.August.Number);
        Assert.Equal("aug", Month.August.Abbreviation);
        Assert.Equal("august", Month.August.FullName);
    }

    [Fact]
    public void PredefinedMonths_September()
    {
        Assert.Equal(9, Month.September.Number);
        Assert.Equal("sep", Month.September.Abbreviation);
        Assert.Equal("september", Month.September.FullName);
    }

    [Fact]
    public void PredefinedMonths_October()
    {
        Assert.Equal(10, Month.October.Number);
        Assert.Equal("oct", Month.October.Abbreviation);
        Assert.Equal("october", Month.October.FullName);
    }

    [Fact]
    public void PredefinedMonths_November()
    {
        Assert.Equal(11, Month.November.Number);
        Assert.Equal("nov", Month.November.Abbreviation);
        Assert.Equal("november", Month.November.FullName);
    }

    [Fact]
    public void PredefinedMonths_December()
    {
        Assert.Equal(12, Month.December.Number);
        Assert.Equal("dec", Month.December.Abbreviation);
        Assert.Equal("december", Month.December.FullName);
    }
}

