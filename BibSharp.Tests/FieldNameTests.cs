namespace BibSharp.Tests;

public class FieldNameTests
{
    [Fact]
    public void Constructor_CreatesFieldName()
    {
        var fieldName = new FieldName("author");
        Assert.Equal("author", fieldName.Value);
    }

    [Fact]
    public void Constructor_ConvertsToLowerCase()
    {
        var fieldName = new FieldName("TITLE");
        Assert.Equal("title", fieldName.Value);
    }

    [Fact]
    public void Constructor_ThrowsOnNull()
    {
        Assert.Throws<ArgumentNullException>(() => new FieldName(null!));
    }

    [Fact]
    public void ToString_ReturnsValue()
    {
        var fieldName = new FieldName("journal");
        Assert.Equal("journal", fieldName.ToString());
    }

    [Fact]
    public void ImplicitConversion_FromString()
    {
        FieldName fieldName = "year";
        Assert.Equal("year", fieldName.Value);
    }

    [Fact]
    public void ImplicitConversion_ToString()
    {
        var fieldName = new FieldName("pages");
        string value = fieldName;
        Assert.Equal("pages", value);
    }

    [Fact]
    public void Equals_SameValue_ReturnsTrue()
    {
        var field1 = new FieldName("author");
        var field2 = new FieldName("author");
        Assert.True(field1.Equals(field2));
        Assert.True(field1 == field2);
        Assert.False(field1 != field2);
    }

    [Fact]
    public void Equals_DifferentValue_ReturnsFalse()
    {
        var field1 = new FieldName("author");
        var field2 = new FieldName("title");
        Assert.False(field1.Equals(field2));
        Assert.False(field1 == field2);
        Assert.True(field1 != field2);
    }

    [Fact]
    public void Equals_CaseInsensitive()
    {
        var field1 = new FieldName("Author");
        var field2 = new FieldName("AUTHOR");
        Assert.True(field1.Equals(field2));
        Assert.True(field1 == field2);
    }

    [Fact]
    public void Equals_WithObject_ReturnsFalse()
    {
        var field1 = new FieldName("author");
        object obj = "author";
        Assert.False(field1.Equals(obj));
    }

    [Fact]
    public void GetHashCode_SameValue_SameHash()
    {
        var field1 = new FieldName("title");
        var field2 = new FieldName("TITLE");
        Assert.Equal(field1.GetHashCode(), field2.GetHashCode());
    }

    [Fact]
    public void PredefinedFields_Author()
    {
        Assert.Equal("author", FieldName.Author.Value);
    }

    [Fact]
    public void PredefinedFields_Title()
    {
        Assert.Equal("title", FieldName.Title.Value);
    }

    [Fact]
    public void PredefinedFields_Journal()
    {
        Assert.Equal("journal", FieldName.Journal.Value);
    }

    [Fact]
    public void PredefinedFields_Year()
    {
        Assert.Equal("year", FieldName.Year.Value);
    }

    [Fact]
    public void PredefinedFields_Volume()
    {
        Assert.Equal("volume", FieldName.Volume.Value);
    }

    [Fact]
    public void PredefinedFields_Number()
    {
        Assert.Equal("number", FieldName.Number.Value);
    }

    [Fact]
    public void PredefinedFields_Pages()
    {
        Assert.Equal("pages", FieldName.Pages.Value);
    }

    [Fact]
    public void PredefinedFields_Month()
    {
        Assert.Equal("month", FieldName.Month.Value);
    }

    [Fact]
    public void PredefinedFields_Doi()
    {
        Assert.Equal("doi", FieldName.Doi.Value);
    }

    [Fact]
    public void PredefinedFields_Note()
    {
        Assert.Equal("note", FieldName.Note.Value);
    }

    [Fact]
    public void PredefinedFields_Url()
    {
        Assert.Equal("url", FieldName.Url.Value);
    }

    [Fact]
    public void PredefinedFields_Issn()
    {
        Assert.Equal("issn", FieldName.Issn.Value);
    }

    [Fact]
    public void PredefinedFields_Keywords()
    {
        Assert.Equal("keywords", FieldName.Keywords.Value);
    }

    [Fact]
    public void PredefinedFields_Abstract()
    {
        Assert.Equal("abstract", FieldName.Abstract.Value);
    }

    [Fact]
    public void PredefinedFields_Language()
    {
        Assert.Equal("language", FieldName.Language.Value);
    }

    [Fact]
    public void PredefinedFields_Copyright()
    {
        Assert.Equal("copyright", FieldName.Copyright.Value);
    }

    [Fact]
    public void PredefinedFields_Publisher()
    {
        Assert.Equal("publisher", FieldName.Publisher.Value);
    }

    [Fact]
    public void PredefinedFields_Editor()
    {
        Assert.Equal("editor", FieldName.Editor.Value);
    }

    [Fact]
    public void PredefinedFields_Series()
    {
        Assert.Equal("series", FieldName.Series.Value);
    }

    [Fact]
    public void PredefinedFields_Address()
    {
        Assert.Equal("address", FieldName.Address.Value);
    }

    [Fact]
    public void PredefinedFields_Edition()
    {
        Assert.Equal("edition", FieldName.Edition.Value);
    }

    [Fact]
    public void PredefinedFields_Isbn()
    {
        Assert.Equal("isbn", FieldName.Isbn.Value);
    }

    [Fact]
    public void PredefinedFields_BookTitle()
    {
        Assert.Equal("booktitle", FieldName.BookTitle.Value);
    }

    [Fact]
    public void PredefinedFields_Organization()
    {
        Assert.Equal("organization", FieldName.Organization.Value);
    }

    [Fact]
    public void PredefinedFields_Chapter()
    {
        Assert.Equal("chapter", FieldName.Chapter.Value);
    }

    [Fact]
    public void PredefinedFields_Type()
    {
        Assert.Equal("type", FieldName.Type.Value);
    }

    [Fact]
    public void PredefinedFields_HowPublished()
    {
        Assert.Equal("howpublished", FieldName.HowPublished.Value);
    }

    [Fact]
    public void PredefinedFields_School()
    {
        Assert.Equal("school", FieldName.School.Value);
    }

    [Fact]
    public void PredefinedFields_Institution()
    {
        Assert.Equal("institution", FieldName.Institution.Value);
    }

    [Fact]
    public void PredefinedFields_Annote()
    {
        Assert.Equal("annote", FieldName.Annote.Value);
    }

    [Fact]
    public void PredefinedFields_Arxiv()
    {
        Assert.Equal("arxiv", FieldName.Arxiv.Value);
    }
}

