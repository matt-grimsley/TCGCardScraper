using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TCGStoreScraper.Models;

namespace TCGStoreScraper
{
    partial class CardNameParser
    {
        [GeneratedRegex(@"\(([^)]+)\)")]
        private static partial Regex SetCodeRegex(); // Partial method for compile-time regex generation

        public static CardName ParseCardName(string fullname)
        {
            string[] parts = fullname.Split(" - ", 2, StringSplitOptions.TrimEntries);

            string name = parts[0];
            string set = parts.Length > 1 ? parts[1] : string.Empty;
            string setCode = SetCodeRegex().Match(set).Groups[1].Value;

            return new CardName()
            {
                FullName = fullname,
                Name = name,
                Set = set,
                SetCode = setCode
            };
        }
    }
}
