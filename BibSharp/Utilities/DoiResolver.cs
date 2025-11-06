using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using BibSharp.Models;

namespace BibSharp.Utilities;

/// <summary>
/// Resolves DOI references to BibTeX entries by querying online APIs.
/// </summary>
/// <remarks>
/// This class supports both instance-based and static usage.
/// For better testability and dependency injection support, use instance-based approach.
/// Instance methods are thread-safe with per-instance rate limiting.
/// This class implements IDisposable to properly clean up HTTP client and semaphore resources.
/// </remarks>
public class DoiResolver : IDisposable
{
    // Instance members
    private readonly HttpClient httpClient;
    private readonly bool ownsHttpClient;
    private readonly SemaphoreSlim rateLimitSemaphore;
    private long lastRequestTimeTicks = DateTime.MinValue.Ticks;
    private readonly int timeoutSeconds;
    private readonly int minRequestDelayMs;
    private bool disposed;

    private const string crossrefApiUrl = "https://api.crossref.org/works/";
    private const string doiOrgUrl = "https://doi.org/";

    /// <summary>
    /// Initializes a new instance of the <see cref="DoiResolver"/> class with default settings.
    /// </summary>
    public DoiResolver() : this(30, 100)
    {
        ownsHttpClient = true;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DoiResolver"/> class with custom settings.
    /// </summary>
    /// <param name="timeoutSeconds">The timeout for HTTP requests in seconds (1-300).</param>
    /// <param name="minRequestDelayMs">The minimum delay between API requests in milliseconds (0-10000).</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when parameters are out of valid range.</exception>
    public DoiResolver(int timeoutSeconds, int minRequestDelayMs)
    {
        if (timeoutSeconds <= 0 || timeoutSeconds > 300)
        {
            throw new ArgumentOutOfRangeException(nameof(timeoutSeconds), 
                "TimeoutSeconds must be between 1 and 300");
        }
        
        if (minRequestDelayMs < 0 || minRequestDelayMs > 10000)
        {
            throw new ArgumentOutOfRangeException(nameof(minRequestDelayMs), 
                "MinRequestDelayMs must be between 0 and 10000");
        }

        this.timeoutSeconds = timeoutSeconds;
        this.minRequestDelayMs = minRequestDelayMs;
        this.httpClient = CreateHttpClient(timeoutSeconds);
        this.ownsHttpClient = true;
        this.rateLimitSemaphore = new SemaphoreSlim(1, 1);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DoiResolver"/> class with a custom HttpClient.
    /// Useful for dependency injection and testing.
    /// </summary>
    /// <param name="httpClient">The HTTP client to use for API requests.</param>
    /// <param name="minRequestDelayMs">The minimum delay between API requests in milliseconds.</param>
    /// <remarks>
    /// When using this constructor, the caller is responsible for disposing the HttpClient.
    /// </remarks>
    public DoiResolver(HttpClient httpClient, int minRequestDelayMs = 100)
    {
        this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        this.ownsHttpClient = false; // Don't dispose injected HttpClient
        this.minRequestDelayMs = minRequestDelayMs;
        this.timeoutSeconds = (int)httpClient.Timeout.TotalSeconds;
        this.rateLimitSemaphore = new SemaphoreSlim(1, 1);
    }

    private static HttpClient CreateHttpClient(int timeoutSeconds)
    {
        var client = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(timeoutSeconds)
        };

        // Set default headers for the HTTP client
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        // User-Agent intentionally kept simple with just library name - no version or framework details
        // to avoid exposing implementation details and reduce maintenance burden
        client.DefaultRequestHeaders.Add("User-Agent", "BibSharp");
        
        return client;
    }

    /// <summary>
    /// Resolves a DOI to a BibEntry by querying online APIs.
    /// </summary>
    /// <param name="doi">The DOI to resolve.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>
    /// A BibEntry representing the resolved DOI if successful; null if the DOI could not be resolved.
    /// Reasons for null return include: network errors, invalid DOI, API unavailability, or malformed API responses.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when doi is null or empty.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is cancelled.</exception>
    /// <remarks>
    /// This method first attempts to resolve the DOI using the CrossRef API.
    /// If that fails, it falls back to DOI.org's content negotiation service.
    /// The method implements rate limiting to avoid overwhelming the APIs.
    /// All network and API exceptions are caught and result in a null return value rather than being propagated.
    /// </remarks>
    public async Task<BibEntry?> ResolveDoiAsync(string doi, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(doi))
        {
            throw new ArgumentException("DOI cannot be null or empty", nameof(doi));
        }

        // Apply rate limiting
        await ApplyInstanceRateLimitAsync(cancellationToken).ConfigureAwait(false);

        // Clean the DOI
        doi = NormalizeDoi(doi);

        // Try to resolve using Crossref API
        try
        {
            return await ResolveFromCrossrefAsync(doi, cancellationToken, httpClient).ConfigureAwait(false);
        }
        catch (Exception)
        {
            // If Crossref fails, try DOI.org's content negotiation
            try
            {
                return await ResolveFromDoiOrgAsync(doi, cancellationToken, httpClient).ConfigureAwait(false);
            }
            catch (Exception)
            {
                // Both methods failed
                return null;
            }
        }
    }

    private static string NormalizeDoi(string doi)
    {
        // Use the shared normalization method from the Doi struct
        return Doi.NormalizeDoi(doi);
    }

    private async Task ApplyInstanceRateLimitAsync(CancellationToken cancellationToken)
    {
        await rateLimitSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            // Use Interlocked for thread-safe read of lastRequestTimeTicks
            long lastTicks = Interlocked.Read(ref lastRequestTimeTicks);
            var lastRequestTime = new DateTime(lastTicks);
            var timeSinceLastRequest = DateTime.UtcNow - lastRequestTime;
            var minDelay = TimeSpan.FromMilliseconds(minRequestDelayMs);

            if (timeSinceLastRequest < minDelay)
            {
                var delayNeeded = minDelay - timeSinceLastRequest;
                await Task.Delay(delayNeeded, cancellationToken).ConfigureAwait(false);
            }

            // Use Interlocked for thread-safe write
            Interlocked.Exchange(ref lastRequestTimeTicks, DateTime.UtcNow.Ticks);
        }
        finally
        {
            rateLimitSemaphore.Release();
        }
    }

    private static async Task<BibEntry?> ResolveFromCrossrefAsync(string doi, CancellationToken cancellationToken, HttpClient client)
    {
        string url = $"{crossrefApiUrl}{Uri.EscapeDataString(doi)}";
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        using var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        using var document = JsonDocument.Parse(responseString);
        var root = document.RootElement;
        var message = root.GetProperty("message");

        // Determine entry type
        string entryType = DetermineEntryType(message);
        var entry = new BibEntry(entryType)
        {
            Key = GenerateKey(message),
            Doi = doi
        };

        // Set common fields
        if (message.TryGetProperty("title", out var titleArray) && titleArray.GetArrayLength() > 0)
        {
            entry.Title = titleArray[0].GetString();
        }

        if (message.TryGetProperty("published-print", out var publishedPrint) &&
            publishedPrint.TryGetProperty("date-parts", out var dateParts) &&
            dateParts.GetArrayLength() > 0)
        {
            var dateArray = dateParts[0];
            if (dateArray.GetArrayLength() > 0 && dateArray[0].TryGetInt32(out int year))
            {
                entry.Year = year;
            }
        }
        else if (message.TryGetProperty("created", out var created) &&
                 created.TryGetProperty("date-parts", out var createdDateParts) &&
                 createdDateParts.GetArrayLength() > 0)
        {
            var dateArray = createdDateParts[0];
            if (dateArray.GetArrayLength() > 0 && dateArray[0].TryGetInt32(out int year))
            {
                entry.Year = year;
            }
        }

        // Set journal/publication
        if (message.TryGetProperty("container-title", out var containerTitle) &&
            containerTitle.GetArrayLength() > 0)
        {
            entry.Journal = containerTitle[0].GetString();
        }

        // Set volume, issue, pages
        if (message.TryGetProperty("volume", out var volume))
        {
            entry["volume"] = volume.GetString();
        }

        if (message.TryGetProperty("issue", out var issue))
        {
            entry["number"] = issue.GetString();
        }

        if (message.TryGetProperty("page", out var page))
        {
            string pageStr = page.GetString() ?? string.Empty;
            if (!string.IsNullOrEmpty(pageStr))
            {
                try
                {
                    entry.Pages = new PageRange(pageStr);
                }
                catch
                {
                    // If page range parsing fails, store as string
                    entry["pages"] = pageStr;
                }
            }
        }

        // Set URL
        if (message.TryGetProperty("URL", out var urlProp))
        {
            entry.Url = urlProp.GetString();
        }

        // Set authors
        if (message.TryGetProperty("author", out var authors))
        {
            foreach (var author in authors.EnumerateArray())
            {
                string? given = null;
                string? family = null;

                if (author.TryGetProperty("given", out var givenElement))
                {
                    given = givenElement.GetString();
                }

                if (author.TryGetProperty("family", out var familyElement))
                {
                    family = familyElement.GetString();
                }

                if (!string.IsNullOrEmpty(family))
                {
                    entry.AddAuthor(new Author(family, given));
                }
            }
        }

        // Set publisher
        if (message.TryGetProperty("publisher", out var publisher))
        {
            entry["publisher"] = publisher.GetString();
        }

        return entry;
    }

    private static async Task<BibEntry?> ResolveFromDoiOrgAsync(string doi, CancellationToken cancellationToken, HttpClient client)
    {
        string url = $"{doiOrgUrl}{Uri.EscapeDataString(doi)}";

        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-bibtex"));

        using var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        string bibtex = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        if (string.IsNullOrWhiteSpace(bibtex))
        {
            return null;
        }

        var parser = new BibParser();
        var entries = parser.ParseString(bibtex);
        return entries.Count > 0 ? entries[0] : null;
    }

    private static string DetermineEntryType(JsonElement message)
    {
        if (message.TryGetProperty("type", out var typeElement))
        {
            string typeStr = typeElement.GetString()?.ToLowerInvariant() ?? "misc";

            // Map Crossref types to BibTeX types
            return typeStr switch
            {
                "journal-article" => "article",
                "book-chapter" => "incollection",
                "book" => "book",
                "proceedings-article" => "inproceedings",
                "conference-paper" => "inproceedings",
                "report" => "techreport",
                "dissertation" => "phdthesis",
                // Add more mappings as needed
                _ => "misc"
            };
        }

        return "misc";
    }

    private static string GenerateKey(JsonElement message)
    {
        // Generate a BibTeX key based on available data
        // Format: [FirstAuthorLastName][Year][FirstTitleWord]

        string authorName = "Unknown";
        int? year = null;
        string firstTitleWord = "Unknown";

        // Get author
        if (message.TryGetProperty("author", out var authors) &&
            authors.GetArrayLength() > 0 &&
            authors[0].TryGetProperty("family", out var familyName))
        {
            authorName = familyName.GetString() ?? "Unknown";
        }

        // Get year
        if (message.TryGetProperty("published-print", out var publishedPrint) &&
            publishedPrint.TryGetProperty("date-parts", out var dateParts) &&
            dateParts.GetArrayLength() > 0)
        {
            var dateArray = dateParts[0];
            if (dateArray.GetArrayLength() > 0 && dateArray[0].TryGetInt32(out int y))
            {
                year = y;
            }
        }
        else if (message.TryGetProperty("created", out var created) &&
                 created.TryGetProperty("date-parts", out var createdDateParts) &&
                 createdDateParts.GetArrayLength() > 0)
        {
            var dateArray = createdDateParts[0];
            if (dateArray.GetArrayLength() > 0 && dateArray[0].TryGetInt32(out int y))
            {
                year = y;
            }
        }

        // Get first title word
        if (message.TryGetProperty("title", out var titles) &&
            titles.GetArrayLength() > 0)
        {
            string? title = titles[0].GetString();
            if (!string.IsNullOrEmpty(title))
            {
                string[] words = title.Split(new[] { ' ', '\t', '\n', '\r', '.', ',', ';', ':', '!', '?' },
                    StringSplitOptions.RemoveEmptyEntries);

                if (words.Length > 0)
                {
                    // Use StringBuilder for efficient string manipulation
                    var cleanWord = new StringBuilder(words[0].Length);
                    foreach (char c in words[0])
                    {
                        if (char.IsLetterOrDigit(c))
                        {
                            cleanWord.Append(c);
                        }
                    }
                    if (cleanWord.Length > 0)
                    {
                        firstTitleWord = cleanWord.ToString();
                    }
                }
            }
        }

        // Build the key using StringBuilder for better performance
        var keyBuilder = new StringBuilder(authorName.Length + 10);
        keyBuilder.Append(authorName.ToLowerInvariant());

        if (year.HasValue)
        {
            keyBuilder.Append(year.Value);
        }

        if (!string.IsNullOrEmpty(firstTitleWord) && firstTitleWord != "Unknown")
        {
            keyBuilder.Append(firstTitleWord.ToLowerInvariant());
        }

        return keyBuilder.ToString();
    }

    /// <summary>
    /// Releases all resources used by the <see cref="DoiResolver"/>.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases the unmanaged resources used by the <see cref="DoiResolver"/> and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                // Dispose managed resources
                rateLimitSemaphore?.Dispose();
                
                // Only dispose HttpClient if we own it (not injected)
                if (ownsHttpClient)
                {
                    httpClient?.Dispose();
                }
            }

            disposed = true;
        }
    }
}
