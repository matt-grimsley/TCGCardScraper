namespace TCGCardScraper;

internal static class Logger
{
    private static readonly string Red = "\x1b[31m";
    private static readonly string Green = "\x1b[32m";
    private static readonly string Yellow = "\x1b[33m";
    private static readonly string Cyan = "\x1b[36m";
    private static readonly string Grey = "\x1b[38;5;240m";
    private static readonly string Reset = "\x1b[0m";

    private static readonly Configuration Config = Configuration.Instance;

    private static readonly LogLevel CurrentLogLevel = Config.LogLevel;
    private static readonly bool LogToFile = Config.LogToFile;
    private static readonly string? LogFilePath = Path.Combine(Config.LogFilePath, Config.LogFileName);

    internal enum LogLevel
    {
        DEBUG,
        TRACE,
        INFO,
        WARNING,
        ERROR,
    }

    internal static void Log(LogLevel level, string message, bool overwrite = false)
    {
        if (level < CurrentLogLevel)
        {
            return;
        }

        if (!overwrite || CurrentLogLevel == LogLevel.DEBUG)
        {
            Console.WriteLine(GetConsoleLogText(level, message));
        }
        else
        {
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(GetConsoleLogText(level, message));
        }

        if (LogToFile)
        {
            WriteToLogFile(message);
        }
    }

    private static string GetConsoleLogText(LogLevel level, string message) => level switch
    {
        LogLevel.DEBUG => $"{Grey}DEBUG: {message}{Reset}",
        LogLevel.TRACE => $"{Cyan}TRACE:{Reset} {message}",
        LogLevel.INFO => $"{Green}INFO:{Reset} {message}",
        LogLevel.WARNING => $"{Yellow}WARNING: {message}{Reset}",
        LogLevel.ERROR => $"{Red}ERROR: {message}{Reset}",
        _ => message,
    };

    private static void WriteToLogFile(string message)
    {
        try
        {
            File.AppendAllText(LogFilePath!, $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff}  {message}{Environment.NewLine}");
        }
        catch (Exception ex)
        {
            Console.WriteLine(GetConsoleLogText(LogLevel.ERROR, $"Failed writing to log file! {ex.Message}"));
        }
    }
}
