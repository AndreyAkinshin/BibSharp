# BibSharp Examples

This directory contains example applications showcasing the BibSharp library.

## DemoApp

A modern interactive console application built with [Spectre.Console](https://spectreconsole.net/) demonstrating the full functionality of BibSharp:

- Creating BibTeX entries programmatically
- Accessing entry fields through properties and indexers
- Validating entries
- Parsing BibTeX content
- Serializing entries back to BibTeX format
- Converting and resolving DOIs to BibTeX entries
- Managing extended BibTeX fields
- Formatting author names in different styles
- Handling international characters in BibTeX
- Formatting citations in various academic styles (Chicago, MLA, APA, Harvard, IEEE)

### Running the demo

```bash
cd BibSharp.Demo
dotnet run
```

The DemoApp provides an interactive experience with rich console UI, allowing you to explore different features of BibSharp through a user-friendly interface.

### Demo Features

- **DOI Resolution Demo**: Demonstrates fetching BibTeX entries from DOIs
- **Extended Fields Demo**: Shows how BibSharp handles additional BibTeX fields beyond the standard set
- **Author Format Demo**: Explores different ways to parse and format author names
- **Internationalization Demo**: Showcases handling of international characters in BibTeX entries
- **BibFormatter Demo**: Displays citations formatted in various academic styles

## More Examples

For more examples, see the unit tests in the `BibSharp.Tests` project, which demonstrate all features of the library in detail.