using BibSharp.Utilities;

namespace BibSharp.Tests.Utilities;

public class BibTexEncoderTests
{
    [Fact]
    public void ToLatex_ConvertsGermanUmlauts()
    {
        // Arrange
        string input = "Müller hätte über die Straße gehen können";
        string expected = "M\\\"uller h\\\"atte \\\"uber die Stra\\ss{}e gehen k\\\"onnen";
        
        // Act
        string result = BibTexEncoder.ToLatex(input);
        
        // Assert
        Assert.Equal(expected, result);
    }
    
    [Fact]
    public void ToLatex_ConvertsFrenchAccents()
    {
        // Arrange
        string input = "René François était à l'école";
        string expected = "Ren\\'e Fran\\c{c}ois \\'etait \\`a l'\\'ecole";
        
        // Act
        string result = BibTexEncoder.ToLatex(input);
        
        // Assert
        Assert.Equal(expected, result);
    }
    
    [Fact]
    public void ToLatex_ConvertsSpanishCharacters()
    {
        // Arrange
        string input = "El niño está en España";
        string expected = "El ni\\~no est\\'a en Espa\\~na";
        
        // Act
        string result = BibTexEncoder.ToLatex(input);
        
        // Assert
        Assert.Equal(expected, result);
    }
    
    [Fact]
    public void ToLatex_ConvertsMathematicalSymbols()
    {
        // Arrange
        string input = "The result is approximately ≈ 3.14 ± 0.01";
        string expected = "The result is approximately \\approx 3.14 \\pm 0.01";
        
        // Act
        string result = BibTexEncoder.ToLatex(input);
        
        // Assert
        Assert.Equal(expected, result);
    }
    
    [Fact]
    public void ToLatex_HandlesSpecialCharacters()
    {
        // Arrange
        string input = "The cost is $10 & includes 25% discount";
        string expected = "The cost is \\$10 \\& includes 25\\% discount";
        
        // Act
        string result = BibTexEncoder.ToLatex(input);
        
        // Assert
        Assert.Equal(expected, result);
    }
    
    [Fact]
    public void ToUnicode_ConvertsLatexToUnicode()
    {
        // Arrange - Using verbatim string literal @ to avoid escaping backslashes
        string input = "M\\\"u" + "ller and \\'e" + "cole \\`a la \\c{c}on";
        string expected = "Müller and école à la çon";
        
        // Act
        string result = BibTexEncoder.ToUnicode(input);
        
        // Assert
        Assert.Equal(expected, result);
    }
    
    [Fact]
    public void ContainsSpecialCharacters_ReturnsFalseForAscii()
    {
        // Arrange
        string input = "This is a regular ASCII string without special characters.";
        
        // Act
        bool result = BibTexEncoder.ContainsSpecialCharacters(input);
        
        // Assert
        Assert.False(result);
    }
    
    [Fact]
    public void ContainsSpecialCharacters_ReturnsTrueForNonAscii()
    {
        // Arrange
        string input = "This string contains an umlaut: ö";
        
        // Act
        bool result = BibTexEncoder.ContainsSpecialCharacters(input);
        
        // Assert
        Assert.True(result);
    }
}