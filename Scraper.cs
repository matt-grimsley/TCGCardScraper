namespace TCGCardScraper;

using System.Globalization;
using System.Web;
using Microsoft.Playwright;
using TCGCardScraper.Models;
using static TCGCardScraper.Logger;

internal sealed class Scraper : IAsyncDisposable
{
    private static readonly Configuration Config = Configuration.Instance;

    private readonly List<Uri> _scrapeRequestUris = [];
    private readonly List<Card> _processedListings = [];

    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private IBrowserContext? _context;
    private IPage? _page;

    private Scraper() { }

    internal static async Task<Scraper> CreateAsync()
    {
        var scraper = new Scraper();
        await scraper.InitializeAsync();
        await scraper.LoadScrapeRequestsFromFileAsync();

        Log(LogLevel.INFO, "Scraper initialized and ready for processing!");

        return scraper;
    }

    private async Task InitializeAsync()
    {
        Log(LogLevel.INFO, "Initializing Playwright...");
        _playwright = await Playwright.CreateAsync();

        var headless = Config.HeadlessMode;
        Log(LogLevel.TRACE, $"Launching Chromium - Headless: {headless}");
        _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = headless });

        Log(LogLevel.TRACE, "Creating browser context, setting User Agent");
        _context = await _browser.NewContextAsync(new BrowserNewContextOptions { UserAgent = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/134.0.0.0 Safari/537.36" });

        _page = await _context.NewPageAsync();
        Log(LogLevel.INFO, "Playwright and browser initialized!");
    }

    private IPage GetPage() => _page is not null ? _page : throw new InvalidOperationException("Playwright hasn't been initialized!");

    internal async Task ScrapeAsync()
    {
        Log(LogLevel.INFO, "Starting scraping process...");

        Log(LogLevel.TRACE, "Loading the initial page...");
        await LoadInitialPageAsync();

        Log(LogLevel.TRACE, "Expanding the Listings / Page...");
        await ExpandListingsAsync();

        foreach (var uri in _scrapeRequestUris)
        {
            Log(LogLevel.INFO, $"Processing: {Uri.UnescapeDataString(uri.PathAndQuery)}");

            _processedListings.AddRange(await ProcessURLAsync(HandlePageQueryParameter(uri, 1)));

            var listingsCount = await GetListingsCount();
            Log(LogLevel.TRACE, $"Found {listingsCount} listings...");

            var totalPages = DetermineTotalNumberOfPages(listingsCount);
            Log(LogLevel.INFO, $"Processed page 1 of {totalPages}...");

            for (var i = 2; i <= totalPages; i++)
            {
                _processedListings.AddRange(await ProcessURLAsync(HandlePageQueryParameter(uri, i)));
                Log(LogLevel.INFO, $"Processed page {i} of {totalPages}...");
            }
        }

        Log(LogLevel.INFO, "Scraping process completed!");

        await StoreMatchAnalyzer.Analyze(_processedListings);

        await DisposeAsync();

        if (Config.OpenResultsFileOnExit)
        {
            StoreMatchAnalyzer.OpenResultsFile();
        }
    }

    private async Task LoadScrapeRequestsFromFileAsync()
    {
        var file = Path.Combine(Config.InputFilePath, Config.InputFileName);

        Log(LogLevel.INFO, $"Loading scrape requests from {file}...");
        var urls = await File.ReadAllLinesAsync(file);

        var emptyUrls = 0;
        var commentedUrls = 0;
        var totalRequests = 0;

        if (urls.Length == 0)
        {
            throw new InvalidOperationException($"{file} contains no records!");
        }

        foreach (var url in urls)
        {
            if (string.IsNullOrEmpty(url))
            {
                Log(LogLevel.TRACE, "Empty URL found in file, skipping...");
                emptyUrls++;
                continue;
            }

            if (url.StartsWith("--", StringComparison.OrdinalIgnoreCase))
            {
                Log(LogLevel.TRACE, "Disabled URL found in file, skipping...");
                commentedUrls++;
                continue;
            }

            _scrapeRequestUris.Add(new Uri(Uri.UnescapeDataString(url)));
            Log(LogLevel.TRACE, $"Added request #{++totalRequests}...");
        }

        if (emptyUrls > 0)
        {
            Log(LogLevel.TRACE, $"Skipped {emptyUrls} blank lines");
        }

        if (commentedUrls > 0)
        {
            Log(LogLevel.TRACE, $"Skipped {commentedUrls} commented lines");
        }

        if (_scrapeRequestUris is null || _scrapeRequestUris.Count == 0)
        {
            throw new InvalidOperationException($"{file} contains no requests!");
        }

        if (_scrapeRequestUris?.Count == 1)
        {
            throw new InvalidOperationException($"{file} contains less than two requests!");
        }

        Log(LogLevel.INFO, "Scrape requests loaded!");
        Log(LogLevel.TRACE, $"Requests loaded: {totalRequests}");
    }

    private async Task<List<Card>> ProcessURLAsync(string url)
    {
        var listings = new List<Card>();

        await NavigateAsync(url);

        await GetPage().WaitForSelectorAsync("h1.product-details__name", new PageWaitForSelectorOptions { State = WaitForSelectorState.Attached });
        var productElement = await GetPage().QuerySelectorAsync("h1.product-details__name");

        var cardName = RegexParser.ParseCardName(await productElement!.InnerTextAsync());

        foreach (var item in await GetPage().QuerySelectorAllAsync(".listing-item"))
        {
            listings.Add(new Card()
            {
                Name = cardName,
                Seller = await ScrapeSellerAsync(item),
                Listing = await ScrapeListingAsync(item),
                Shipping = await ScrapeShippingAsync(item)
            });
        }

        return listings;
    }

    private async Task LoadInitialPageAsync()
    {
        var url = HandlePageQueryParameter(_scrapeRequestUris.First(), 1);

        Log(LogLevel.TRACE, $"First page URL: {url}");
        await NavigateAsync(url);
    }

    private async Task ExpandListingsAsync()
    {

        Log(LogLevel.DEBUG, "WaitForSelector label span:has-text('Listings / Page')");
        await GetPage().WaitForSelectorAsync("label span:has-text('Listings / Page')", new() { State = WaitForSelectorState.Visible });

        Log(LogLevel.DEBUG, "Locate div[contains(@class, 'tcg-input-select__trigger-container')]");
        var dropdownTrigger = GetPage().Locator("label span:has-text('Listings / Page') >> xpath=ancestor::div[contains(@class, 'tcg-input-field')]//div[contains(@class, 'tcg-input-select__trigger-container')]");

        Log(LogLevel.DEBUG, "ScrollIntoViewIfNeeded");
        await dropdownTrigger.ScrollIntoViewIfNeededAsync();

        Log(LogLevel.DEBUG, "Click");
        await dropdownTrigger.ClickAsync();

        Log(LogLevel.DEBUG, "WaitForSelector li[aria-label='50']");
        await GetPage().WaitForSelectorAsync("li[aria-label='50']", new() { State = WaitForSelectorState.Visible });

        Log(LogLevel.DEBUG, "Locate li[aria-label='50']");
        var option50 = GetPage().Locator("li[aria-label='50']");

        Log(LogLevel.DEBUG, "ScrollIntoViewIfNeeded");
        await option50.ScrollIntoViewIfNeededAsync();

        Log(LogLevel.DEBUG, "Click");
        await option50.ClickAsync();

        Log(LogLevel.DEBUG, "WaitForFunction document.querySelectorAll('.listing-item').length >= 1");
        await GetPage().WaitForFunctionAsync("document.querySelectorAll('.listing-item').length >= 1");
    }

    private static async Task Cooldown()
    {
        var duration = Random.Shared.Next(Config.ThrottleMin, Config.ThrottleMax);

        Log(LogLevel.TRACE, $"Cooldown timer waiting {duration / 1000.0} seconds...");

        await Task.Delay(duration);
    }

    private static string HandlePageQueryParameter(Uri url, int pageNumber)
    {
        var queryParams = HttpUtility.ParseQueryString(url.Query);

        queryParams["page"] = pageNumber.ToString(CultureInfo.InvariantCulture);

        return Uri.UnescapeDataString(new UriBuilder(url) { Query = queryParams.ToString() }.Uri.AbsoluteUri);
    }

    private async Task NavigateAsync(string url)
    {
        Log(LogLevel.TRACE, $"Navigating to {url}");
        await GetPage().GotoAsync(url);
        await GetPage().WaitForURLAsync(url, new PageWaitForURLOptions { WaitUntil = WaitUntilState.Load, Timeout = 15000 });

        await Cooldown();
    }

    private async Task<int> GetListingsCount()
    {
        var locator = GetPage().Locator("div.heading >> text=/\\d+\\sListings/");

        var elementText = await locator.TextContentAsync();

        return RegexParser.ParseListingsCount(elementText!);
    }

    private static int DetermineTotalNumberOfPages(int totalListings, int listingsPerPage = 50) => totalListings == 0 ? 0 : (int)Math.Ceiling((double)totalListings / listingsPerPage);

    #region Scraping
    private static async Task<SellerData> ScrapeSellerAsync(IElementHandle item)
    {
        Log(LogLevel.DEBUG, "Scraping seller...");

        var sellerData = new SellerData();

        var name = await item.QuerySelectorAsync(".seller-info__name");
        sellerData.Name = name != null ? await name.InnerTextAsync() : "UNKNOWN";
        Log(LogLevel.DEBUG, $"Seller name: {sellerData.Name}");

        var direct = await item.QuerySelectorAsync("a[title='Direct Seller']");
        sellerData.IsDirectSeller = direct != null;
        Log(LogLevel.DEBUG, $"Is Direct Seller: {sellerData.IsDirectSeller}");

        var hobby = await item.QuerySelectorAsync("a[title='Certified Hobby Shop']");
        sellerData.IsCertifiedHobbyShop = hobby != null;
        Log(LogLevel.DEBUG, $"Is Certified Hobby Shop: {sellerData.IsCertifiedHobbyShop}");

        var gold = await item.QuerySelectorAsync("a[title='Gold Star Seller']");
        sellerData.IsGoldStarSeller = gold != null;
        Log(LogLevel.DEBUG, $"Is Gold Star Seller: {sellerData.IsGoldStarSeller}");

        var rating = await item.QuerySelectorAsync(".seller-info__rating");
        sellerData.Rating = rating != null ? await rating.InnerTextAsync() : "UNKNOWN";
        Log(LogLevel.DEBUG, $"Seller rating: {sellerData.Rating}");

        var sales = await item.QuerySelectorAsync(".seller-info__sales");
        sellerData.Sales = sales != null ? await sales.InnerTextAsync() : "UNKNOWN";
        Log(LogLevel.DEBUG, $"Seller sales: {sellerData.Sales}");

        return sellerData;
    }

    private static async Task<ListingData> ScrapeListingAsync(IElementHandle item)
    {
        Log(LogLevel.DEBUG, "Scraping listing...");

        var listingData = new ListingData();

        var price = await item.QuerySelectorAsync(".listing-item__listing-data__info__price");
        listingData.Price = await Formatting.FormatListingItemPriceData(price!);

        Log(LogLevel.DEBUG, $"Item price: {listingData.Price}");

        var parent = await item.QuerySelectorAsync(".listing-item__listing-data");
        if (parent != null)
        {
            var h3 = await item.QuerySelectorAsync("h3");
            if (h3 != null)
            {
                var hyperlink = h3.QuerySelectorAsync("a").Result;
                if (hyperlink != null)
                {
                    var hyperlinkText = hyperlink.TextContentAsync().Result;
                    if (hyperlinkText != null)
                    {
                        listingData.Condition = hyperlinkText;
                        Log(LogLevel.DEBUG, $"Item condition: {listingData.Condition}");
                    }
                }
            }
        }

        return listingData;
    }

    private static async Task<ShippingData> ScrapeShippingAsync(IElementHandle item)
    {
        Log(LogLevel.DEBUG, "Scraping shipping...");

        var shippingData = new ShippingData
        {
            Price = string.Empty,
            Included = false,
            FreeOverMinimum = false,
            FreeDirect = false
        };

        var sibling = await item.QuerySelectorAsync(".listing-item__listing-data__info__price");

        if (sibling != null)
        {
            var shippingDivHandle = await sibling.EvaluateHandleAsync("el => el.nextElementSibling");
            if (shippingDivHandle != null)
            {
                var shippingDiv = shippingDivHandle.AsElement();
                if (shippingDiv != null)
                {
                    shippingData.Included = await shippingDiv.QuerySelectorAsync("div:has-text('Shipping: Included')") != null;
                    Log(LogLevel.DEBUG, $"Shipping included: {shippingData.Included}");

                    shippingData.FreeOverMinimum = await shippingDiv.QuerySelectorAsync(".free-shipping-over-min") != null;
                    Log(LogLevel.DEBUG, $"Free shipping over minimum: {shippingData.FreeOverMinimum}");

                    shippingData.FreeDirect = await shippingDiv.QuerySelectorAsync(".free-shipping-direct") != null;
                    Log(LogLevel.DEBUG, $"Free direct shipping: {shippingData.FreeDirect}");

                    if (shippingData.Included)
                    {
                        shippingData.Price = "Included";
                    }
                    else if (shippingData.FreeDirect)
                    {
                        shippingData.Price = "Direct";
                    }
                    else
                    {
                        var price = await shippingDiv.QuerySelectorAsync(".shipping-messages__price");
                        shippingData.Price = price != null ? await price.InnerTextAsync() : "UNKNOWN";
                        Log(LogLevel.DEBUG, $"Shipping price: {shippingData.Price}");
                    }
                }
            }
        }

        return shippingData;
    }

    #endregion

    async public ValueTask DisposeAsync()
    {
        Log(LogLevel.INFO, "Shutting down Playwright...");

        if (_page != null)
        {
            Log(LogLevel.TRACE, "Closing page...");
            await _page.CloseAsync();
        }

        if (_context != null)
        {
            Log(LogLevel.TRACE, "Closing browser context...");
            await _context.CloseAsync();
        }

        if (_browser != null)
        {
            Log(LogLevel.TRACE, "Closing browser...");
            await _browser.CloseAsync();
        }

        Log(LogLevel.TRACE, "Disposing Playwright...");
        _playwright?.Dispose();

        Log(LogLevel.INFO, "Playwright shut down successfully!");
    }
}

