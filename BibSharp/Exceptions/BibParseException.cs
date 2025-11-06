using System;

namespace BibSharp.Exceptions;

/// <summary>
/// Exception thrown when an error occurs during BibTeX parsing.
/// </summary>
public class BibParseException : Exception
{
    /// <summary>
    /// Gets the line number where the error occurred.
    /// </summary>
    public int LineNumber { get; }
    
    /// <summary>
    /// Gets the column number where the error occurred.
    /// </summary>
    public int ColumnNumber { get; }
    
    /// <summary>
    /// Gets the source text that caused the error.
    /// </summary>
    public string? SourceText { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BibParseException"/> class.
    /// </summary>
    public BibParseException()
    {
        LineNumber = 0;
        ColumnNumber = 0;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BibParseException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public BibParseException(string message) : base(message)
    {
        LineNumber = 0;
        ColumnNumber = 0;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BibParseException"/> class with line number information.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="lineNumber">The line number where the error occurred.</param>
    /// <param name="columnNumber">The column number where the error occurred.</param>
    public BibParseException(string message, int lineNumber, int columnNumber) 
        : base($"{message} at line {lineNumber}, column {columnNumber}")
    {
        LineNumber = lineNumber;
        ColumnNumber = columnNumber;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BibParseException"/> class with line number and source text information.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="lineNumber">The line number where the error occurred.</param>
    /// <param name="columnNumber">The column number where the error occurred.</param>
    /// <param name="sourceText">The source text that caused the error.</param>
    public BibParseException(string message, int lineNumber, int columnNumber, string sourceText) 
        : base($"{message} at line {lineNumber}, column {columnNumber}: '{sourceText}'")
    {
        LineNumber = lineNumber;
        ColumnNumber = columnNumber;
        SourceText = sourceText;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BibParseException"/> class with an inner exception.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception that caused this exception.</param>
    public BibParseException(string message, Exception innerException) : base(message, innerException)
    {
        LineNumber = 0;
        ColumnNumber = 0;
    }
}