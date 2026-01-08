public class Logger
{
    private readonly string _logFilePath = "C:/Users/danie/OneDrive/Desktop/testing/logs/log.txt";

    // public Logger(string logFilePath)
    // {
    //     _logFilePath = logFilePath;
    // }

    public void WriteLog(string logInfo)
    {
        var logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] - {logInfo}";

        Console.WriteLine(logMessage);
        File.AppendAllText(_logFilePath, logMessage + Environment.NewLine);
    }

}