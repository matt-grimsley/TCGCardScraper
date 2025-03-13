namespace TCGCardScraper;
internal sealed class Program
{
    private static async Task Main()
    {
        try
        {
            var scraper = await Scraper.CreateAsync();
            await scraper.ScrapeAsync();
        }
        catch (Exception ex)
        {
            Logger.Log(Logger.LogLevel.ERROR, ex.Message);
            Logger.Log(Logger.LogLevel.TRACE, ex.ToString());
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
        finally
        {
            //Console.WriteLine("Press any key to exit...");
            //Console.ReadKey();
        }
    }
}
