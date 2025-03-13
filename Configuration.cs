namespace TCGCardScraper;

using System.Globalization;
using Microsoft.Extensions.Configuration;

internal sealed class Configuration
{
    private const Logger.LogLevel DefaultLogLevel = Logger.LogLevel.INFO;
    private const string DefaultInputFileName = "urls.txt";
    private const int DefaultThrottleMin = 2000;
    private const int DefaultThrottleMax = 10000;
    private static readonly string DefaultLogFileName = $"{AppDomain.CurrentDomain.FriendlyName}-Log-{DateTime.Now:yyyyMMdd}.log";
    private static readonly string DefaultResultsFileName = $"{AppDomain.CurrentDomain.FriendlyName}-AnalyzerResults.txt";
    private const int DefaultResultsFileCharacterWidth = 80;
    private const decimal DefaultMaximumSingleCardPrice = 99.99m;

    private static readonly Lazy<Configuration> ConfigInstance = new(() => new Configuration());
    private readonly IConfigurationRoot _config;

    private Configuration() => _config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

    internal static Configuration Instance => ConfigInstance.Value;

    #region Options
    internal string InputFilePath
    {
        get
        {
            var value = _config["Options:InputFilePath"];

            var directoryPath = string.IsNullOrWhiteSpace(value) ? AppContext.BaseDirectory : value;

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            return directoryPath;
        }
    }

    internal string InputFileName
    {
        get
        {
            var value = _config["Options:InputFileName"];

            return string.IsNullOrWhiteSpace(value) ? DefaultInputFileName : value;
        }
    }

    internal int ThrottleMin
    {
        get
        {
            var min = int.TryParse(_config["Options:Throttle:Min"], out var parsedMin) ? parsedMin : DefaultThrottleMin;

            var max = int.TryParse(_config["Options:Throttle:Max"], out var parsedMax) ? parsedMax : DefaultThrottleMax;

            return min > max ? max : min;
        }
    }

    internal int ThrottleMax
    {
        get
        {
            var min = int.TryParse(_config["Options:Throttle:Min"], out var parsedMin) ? parsedMin : DefaultThrottleMin;

            var max = int.TryParse(_config["Options:Throttle:Max"], out var parsedMax) ? parsedMax : DefaultThrottleMax;

            return min > max ? min : max;
        }
    }

    internal bool OpenResultsFileOnExit => bool.TryParse(_config["Options:OpenResultsFileOnExit"], out var value) && value;
    #endregion

    #region Logging
    internal Logger.LogLevel LogLevel => Enum.TryParse(_config["Logging:LogLevel"], true, out Logger.LogLevel result) ? result : DefaultLogLevel;

    internal bool LogToFile => bool.TryParse(_config["Logging:LogToFile"], out var value) && value;

    internal string LogFilePath
    {
        get
        {
            var value = _config["Logging:LogFilePath"];

            var directoryPath = string.IsNullOrWhiteSpace(value) ? AppContext.BaseDirectory : value;

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            return directoryPath;
        }
    }

    internal string LogFileName
    {
        get
        {
            var value = _config["Logging:LogFileName"];

            return string.IsNullOrWhiteSpace(value)
                ? DefaultLogFileName
                : value;
        }
    }
    #endregion

    #region Analyzer

    internal string ResultsFilePath
    {
        get
        {
            var value = _config["Analyzer:ResultsFilePath"];

            var directoryPath = string.IsNullOrWhiteSpace(value) ? AppContext.BaseDirectory : value;

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            return directoryPath;
        }
    }

    internal string ResultsFileName
    {
        get
        {
            var value = _config["Analyzer:ResultsFileName"];

            return string.IsNullOrWhiteSpace(value)
                ? DefaultResultsFileName
                : value;
        }
    }

    internal int ResultsFileCharacterWidth
    {
        get
        {
            var value = _config["Analyzer:ResultsFileCharacterWidth"];

            return int.TryParse(value, out var result) && !string.IsNullOrWhiteSpace(value)
                ? result
                : DefaultResultsFileCharacterWidth;
        }
    }

    internal bool ExcludeDirectSellers => bool.TryParse(_config["Analyzer:ExcludeDirectSellers"], out var value) && value;

    internal bool ForceFreeShipping => bool.TryParse(_config["Analyzer:ForceFreeShipping"], out var value) && value;

    internal decimal MaximumSingleCardPrice
    {
        get
        {
            var value = _config["Analyzer:MaximumSingleCardPrice"];

            return decimal.TryParse(value, NumberStyles.AllowCurrencySymbol, CultureInfo.InvariantCulture, out var result) && !string.IsNullOrWhiteSpace(value)
                ? result
                : DefaultMaximumSingleCardPrice;

        }
    }
    #endregion

    #region Playwright
    internal bool HeadlessMode => bool.TryParse(_config["Playwright:HeadlessMode"], out var value) && value;
    #endregion
}
