using BibSharp.Utilities;

namespace BibSharp.Tests.Utilities;

public class BibTexEncoderComprehensiveTests
{
    [Fact]
    public void ToLatex_GermanUmlauts()
    {
        Assert.Equal("\\\"a", BibTexEncoder.ToLatex("ä"));
        Assert.Equal("\\\"o", BibTexEncoder.ToLatex("ö"));
        Assert.Equal("\\\"u", BibTexEncoder.ToLatex("ü"));
        Assert.Equal("\\\"A", BibTexEncoder.ToLatex("Ä"));
        Assert.Equal("\\\"O", BibTexEncoder.ToLatex("Ö"));
        Assert.Equal("\\\"U", BibTexEncoder.ToLatex("Ü"));
        Assert.Equal("\\ss{}", BibTexEncoder.ToLatex("ß"));
    }

    [Fact]
    public void ToLatex_FrenchAccents()
    {
        Assert.Equal("\\'e", BibTexEncoder.ToLatex("é"));
        Assert.Equal("\\`e", BibTexEncoder.ToLatex("è"));
        Assert.Equal("\\^e", BibTexEncoder.ToLatex("ê"));
        Assert.Equal("\\\"e", BibTexEncoder.ToLatex("ë"));
        Assert.Equal("\\'a", BibTexEncoder.ToLatex("á"));
        Assert.Equal("\\`a", BibTexEncoder.ToLatex("à"));
        Assert.Equal("\\^a", BibTexEncoder.ToLatex("â"));
    }

    [Fact]
    public void ToLatex_SpanishCharacters()
    {
        Assert.Equal("\\~n", BibTexEncoder.ToLatex("ñ"));
        Assert.Equal("\\~N", BibTexEncoder.ToLatex("Ñ"));
        Assert.Equal("\\c{c}", BibTexEncoder.ToLatex("ç"));
        Assert.Equal("\\c{C}", BibTexEncoder.ToLatex("Ç"));
        Assert.Equal("?`", BibTexEncoder.ToLatex("¿"));
        Assert.Equal("!`", BibTexEncoder.ToLatex("¡"));
    }

    [Fact]
    public void ToLatex_ScandinavianCharacters()
    {
        Assert.Equal("\\aa{}", BibTexEncoder.ToLatex("å"));
        Assert.Equal("\\AA{}", BibTexEncoder.ToLatex("Å"));
        Assert.Equal("\\o{}", BibTexEncoder.ToLatex("ø"));
        Assert.Equal("\\O{}", BibTexEncoder.ToLatex("Ø"));
        Assert.Equal("\\ae{}", BibTexEncoder.ToLatex("æ"));
        Assert.Equal("\\AE{}", BibTexEncoder.ToLatex("Æ"));
    }

    [Fact]
    public void ToLatex_EasternEuropeanCharacters()
    {
        Assert.Equal("\\k{a}", BibTexEncoder.ToLatex("ą"));
        Assert.Equal("\\k{A}", BibTexEncoder.ToLatex("Ą"));
        Assert.Equal("\\'c", BibTexEncoder.ToLatex("ć"));
        Assert.Equal("\\'C", BibTexEncoder.ToLatex("Ć"));
        Assert.Equal("\\l{}", BibTexEncoder.ToLatex("ł"));
        Assert.Equal("\\L{}", BibTexEncoder.ToLatex("Ł"));
    }

    [Fact]
    public void ToLatex_GreekLetters()
    {
        Assert.Equal("\\alpha", BibTexEncoder.ToLatex("α"));
        Assert.Equal("\\beta", BibTexEncoder.ToLatex("β"));
        Assert.Equal("\\gamma", BibTexEncoder.ToLatex("γ"));
        Assert.Equal("\\Gamma", BibTexEncoder.ToLatex("Γ"));
        Assert.Equal("\\delta", BibTexEncoder.ToLatex("δ"));
        Assert.Equal("\\Delta", BibTexEncoder.ToLatex("Δ"));
        Assert.Equal("\\pi", BibTexEncoder.ToLatex("π"));
        Assert.Equal("\\Pi", BibTexEncoder.ToLatex("Π"));
        Assert.Equal("\\sigma", BibTexEncoder.ToLatex("σ"));
        Assert.Equal("\\Sigma", BibTexEncoder.ToLatex("Σ"));
        Assert.Equal("\\omega", BibTexEncoder.ToLatex("ω"));
        Assert.Equal("\\Omega", BibTexEncoder.ToLatex("Ω"));
    }

    [Fact]
    public void ToLatex_SpecialSymbols()
    {
        Assert.Equal("\\textdegree", BibTexEncoder.ToLatex("°"));
        Assert.Equal("\\dag", BibTexEncoder.ToLatex("†"));
        Assert.Equal("\\ddag", BibTexEncoder.ToLatex("‡"));
        Assert.Equal("\\bullet", BibTexEncoder.ToLatex("•"));
        Assert.Equal("\\ldots", BibTexEncoder.ToLatex("…"));
        Assert.Equal("\\S", BibTexEncoder.ToLatex("§"));
        Assert.Equal("\\copyright", BibTexEncoder.ToLatex("©"));
        Assert.Equal("\\pounds", BibTexEncoder.ToLatex("£"));
    }

    [Fact]
    public void ToLatex_ReservedCharacters()
    {
        Assert.Equal("\\$", BibTexEncoder.ToLatex("$"));
        Assert.Equal("\\%", BibTexEncoder.ToLatex("%"));
        Assert.Equal("\\&", BibTexEncoder.ToLatex("&"));
        Assert.Equal("\\#", BibTexEncoder.ToLatex("#"));
        Assert.Equal("\\_", BibTexEncoder.ToLatex("_"));
        Assert.Equal("\\{", BibTexEncoder.ToLatex("{"));
        Assert.Equal("\\}", BibTexEncoder.ToLatex("}"));
        Assert.Equal("\\textbackslash{}", BibTexEncoder.ToLatex("\\"));
        Assert.Equal("\\textasciitilde{}", BibTexEncoder.ToLatex("~"));
        Assert.Equal("\\textasciicircum{}", BibTexEncoder.ToLatex("^"));
    }

    [Fact]
    public void ToLatex_MathematicalSymbols()
    {
        Assert.Equal("\\pm", BibTexEncoder.ToLatex("±"));
        Assert.Equal("\\times", BibTexEncoder.ToLatex("×"));
        Assert.Equal("\\div", BibTexEncoder.ToLatex("÷"));
        Assert.Equal("\\leq", BibTexEncoder.ToLatex("≤"));
        Assert.Equal("\\geq", BibTexEncoder.ToLatex("≥"));
        Assert.Equal("\\neq", BibTexEncoder.ToLatex("≠"));
        Assert.Equal("\\approx", BibTexEncoder.ToLatex("≈"));
        Assert.Equal("\\infty", BibTexEncoder.ToLatex("∞"));
    }

    [Fact]
    public void ToLatex_Quotes()
    {
        Assert.Equal("\\textquotedbl{}", BibTexEncoder.ToLatex("\""));
        Assert.Equal("\\textquoteleft{}", BibTexEncoder.ToLatex("\u2018"));
        Assert.Equal("\\textquoteright{}", BibTexEncoder.ToLatex("\u2019"));
        Assert.Equal("\\textquotedblleft{}", BibTexEncoder.ToLatex("\u201C"));
        Assert.Equal("\\textquotedblright{}", BibTexEncoder.ToLatex("\u201D"));
    }

    [Fact]
    public void ToLatex_NullOrEmpty()
    {
        Assert.Null(BibTexEncoder.ToLatex(null!));
        Assert.Equal("", BibTexEncoder.ToLatex(""));
    }

    [Fact]
    public void ToLatex_NoSpecialCharacters()
    {
        Assert.Equal("Hello World", BibTexEncoder.ToLatex("Hello World"));
        Assert.Equal("Test123", BibTexEncoder.ToLatex("Test123"));
    }

    [Fact]
    public void ToLatex_MixedContent()
    {
        Assert.Equal("Caf\\'e", BibTexEncoder.ToLatex("Café"));
        Assert.Equal("M\\\"uller", BibTexEncoder.ToLatex("Müller"));
        Assert.Equal("Se\\~nor", BibTexEncoder.ToLatex("Señor"));
    }

    [Fact]
    public void ToUnicode_GermanUmlauts()
    {
        Assert.Contains("ä", BibTexEncoder.ToUnicode("\\\"a"));
        Assert.Contains("ö", BibTexEncoder.ToUnicode("\\\"o"));
        Assert.Contains("ü", BibTexEncoder.ToUnicode("\\\"u"));
        Assert.Contains("Ä", BibTexEncoder.ToUnicode("\\\"A"));
        Assert.Contains("Ö", BibTexEncoder.ToUnicode("\\\"O"));
        Assert.Contains("Ü", BibTexEncoder.ToUnicode("\\\"U"));
        Assert.Contains("ß", BibTexEncoder.ToUnicode("\\ss{}"));
    }

    [Fact]
    public void ToUnicode_FrenchAccents()
    {
        Assert.Contains("é", BibTexEncoder.ToUnicode("\\'e"));
        Assert.Contains("è", BibTexEncoder.ToUnicode("\\`e"));
        Assert.Contains("ê", BibTexEncoder.ToUnicode("\\^e"));
    }

    [Fact]
    public void ToUnicode_ComplexCommands()
    {
        Assert.Contains("\\", BibTexEncoder.ToUnicode("\\textbackslash"));
        Assert.Contains("~", BibTexEncoder.ToUnicode("\\textasciitilde"));
        Assert.Contains("^", BibTexEncoder.ToUnicode("\\textasciicircum"));
        Assert.Contains("…", BibTexEncoder.ToUnicode("\\ldots"));
        Assert.Contains("°", BibTexEncoder.ToUnicode("\\textdegree"));
    }

    [Fact]
    public void ToUnicode_Quotes()
    {
        Assert.Contains("\u201C", BibTexEncoder.ToUnicode("\\textquotedblleft"));
        Assert.Contains("\u201D", BibTexEncoder.ToUnicode("\\textquotedblright"));
        Assert.Contains("\u2018", BibTexEncoder.ToUnicode("\\textquoteleft"));
        Assert.Contains("\u2019", BibTexEncoder.ToUnicode("\\textquoteright"));
        Assert.Contains("\"", BibTexEncoder.ToUnicode("\\textquotedbl"));
    }

    [Fact]
    public void ToUnicode_NullOrEmpty()
    {
        Assert.Null(BibTexEncoder.ToUnicode(null!));
        Assert.Equal("", BibTexEncoder.ToUnicode(""));
    }

    [Fact]
    public void ToUnicode_NoLatexCommands()
    {
        Assert.Equal("Hello World", BibTexEncoder.ToUnicode("Hello World"));
        Assert.Equal("Test123", BibTexEncoder.ToUnicode("Test123"));
    }

    [Fact]
    public void ContainsSpecialCharacters_True()
    {
        Assert.True(BibTexEncoder.ContainsSpecialCharacters("Café"));
        Assert.True(BibTexEncoder.ContainsSpecialCharacters("Müller"));
        Assert.True(BibTexEncoder.ContainsSpecialCharacters("Señor"));
        Assert.True(BibTexEncoder.ContainsSpecialCharacters("α"));
        Assert.True(BibTexEncoder.ContainsSpecialCharacters("$100"));
        Assert.True(BibTexEncoder.ContainsSpecialCharacters("50%"));
        Assert.True(BibTexEncoder.ContainsSpecialCharacters("x ± y"));
    }

    [Fact]
    public void ContainsSpecialCharacters_False()
    {
        Assert.False(BibTexEncoder.ContainsSpecialCharacters("Hello World"));
        Assert.False(BibTexEncoder.ContainsSpecialCharacters("Test123"));
        Assert.False(BibTexEncoder.ContainsSpecialCharacters("ABC"));
    }

    [Fact]
    public void ContainsSpecialCharacters_NullOrEmpty()
    {
        Assert.False(BibTexEncoder.ContainsSpecialCharacters(null!));
        Assert.False(BibTexEncoder.ContainsSpecialCharacters(""));
    }

    [Fact]
    public void ToLatex_CompleteWord()
    {
        var input = "Café Müller";
        var output = BibTexEncoder.ToLatex(input);
        Assert.Equal("Caf\\'e M\\\"uller", output);
    }

    [Fact]
    public void ToUnicode_CompleteWord()
    {
        var input = "Caf\\'e M\\\"uller";
        var output = BibTexEncoder.ToUnicode(input);
        Assert.Contains("Café", output);
        Assert.Contains("Müller", output);
    }

    [Fact]
    public void RoundTrip_LatexToUnicodeToLatex()
    {
        var original = "Café Müller Señor";
        var latex = BibTexEncoder.ToLatex(original);
        var unicode = BibTexEncoder.ToUnicode(latex);
        
        // Should preserve the special characters
        Assert.Contains("é", unicode);
        Assert.Contains("ü", unicode);
        Assert.Contains("ñ", unicode);
    }

    [Fact]
    public void ToLatex_AllReservedCharactersInSentence()
    {
        var input = "Cost is $100, not 50% & {not} 75% #tag";
        var output = BibTexEncoder.ToLatex(input);
        
        Assert.Contains("\\$", output);
        Assert.Contains("\\%", output);
        Assert.Contains("\\&", output);
        Assert.Contains("\\{", output);
        Assert.Contains("\\}", output);
        Assert.Contains("\\#", output);
    }

    [Fact]
    public void ToLatex_PolishCharacters()
    {
        Assert.Equal("\\k{a}", BibTexEncoder.ToLatex("ą"));
        Assert.Equal("\\k{e}", BibTexEncoder.ToLatex("ę"));
        Assert.Equal("\\'c", BibTexEncoder.ToLatex("ć"));
        Assert.Equal("\\'n", BibTexEncoder.ToLatex("ń"));
        Assert.Equal("\\'s", BibTexEncoder.ToLatex("ś"));
        Assert.Equal("\\'z", BibTexEncoder.ToLatex("ź"));
        Assert.Equal("\\.z", BibTexEncoder.ToLatex("ż"));
    }

    [Fact]
    public void ToLatex_CurrencySymbols()
    {
        Assert.Equal("\\pounds", BibTexEncoder.ToLatex("£"));
        Assert.Equal("\\euro", BibTexEncoder.ToLatex("€"));
    }

    [Fact]
    public void ToLatex_MultipleGreekLetters()
    {
        var input = "αβγδ";
        var output = BibTexEncoder.ToLatex(input);
        Assert.Contains("\\alpha", output);
        Assert.Contains("\\beta", output);
        Assert.Contains("\\gamma", output);
        Assert.Contains("\\delta", output);
    }
}

