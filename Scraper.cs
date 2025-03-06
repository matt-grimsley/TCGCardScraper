using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Playwright;
using TCGStoreScraper.Models;

namespace TCGCardScraper
{
    internal static class Scraper
    {
        private static List<CardScrapeInfo> _scrapeRequests = [];

        private static IPlaywright? _playwright;
        private static IBrowser? _browser;

        public static void Scrape()
        {

        }
    }
}
