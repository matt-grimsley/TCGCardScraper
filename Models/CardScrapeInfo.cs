namespace TCGStoreScraper.Models
{
    public readonly struct CardScrapeInfo(string url, int pages)
    {
        public readonly string URL { get; } = url;
        public readonly int Pages { get; } = pages;
    }
}