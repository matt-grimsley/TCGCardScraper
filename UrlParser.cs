using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TCGStoreScraper
{
    partial class UrlParser
    {
        [GeneratedRegex(@"([?&])page=\d+")]
        private static partial Regex PageRegex(); // Partial method for compile-time regex generation

        public static string UpdatePageParameter(string url, int newPageNumber)
        {
            if (url.Contains('?'))
                if (PageRegex().IsMatch(url))
                    return PageRegex().Replace(url, $"$1page={newPageNumber}");
                else
                    return $"{url}&page=2";
            else
                return $"{url}&page=2";
        }
    }
}
