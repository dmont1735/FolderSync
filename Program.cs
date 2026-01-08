namespace FolderSync;
using System;
class Program
{
    static void Main(string[] args)
    {
        if (args.Length != 4)
        {
            Console.WriteLine("Usage: FolderSync <source_path> <replica_path> <interval_seconds> <log_file_path>");
            return;
        }

        var sourcePath = args[0];
        var replicaPath = args[1];
        var logFilePath = args[3];

        if (!int.TryParse(args[2], out var intervalSeconds) || intervalSeconds <= 0)
        {
            Console.WriteLine("Input validation error: Interval must be a positive integer");
            return;
        }

        Logger logger = new Logger(logFilePath);
        FolderSynchronizer synchronizer = new FolderSynchronizer(logger);

        if (!Directory.Exists(sourcePath))
        {
            Console.WriteLine($"Input validation error: Source folder could not be found at given location {sourcePath}");
            return;
        }

        try
        {
            File.AppendAllText(logFilePath, "");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Input validation error: Cannot write to log file: {ex.Message}");
            return;
        }

        Console.WriteLine("The Synchronization will run with the following configuration:");
        Console.WriteLine("------------------------------------------------------------");
        Console.WriteLine($"Source: {sourcePath}");
        Console.WriteLine($"Replica: {replicaPath}");
        Console.WriteLine($"Interval: every {intervalSeconds} seconds");
        Console.WriteLine($"Log file: {logFilePath}");
        Console.WriteLine("------------------------------------------------------------");

        synchronizer.Synchronize(sourcePath,replicaPath);

        var timer = new System.Timers.Timer(intervalSeconds * 1000);
        timer.Elapsed += (sender, e) =>
        {
            try
            {
                synchronizer.Synchronize(sourcePath,replicaPath);
                logger.WriteLog($"[SCHEDULED] - Synchronization will run every {intervalSeconds} seconds");
                Console.WriteLine("Press Enter to Abort...");
            }
            catch(Exception ex)
            {
                logger.WriteLog($"Error during synchronization: {ex.Message}");
            }
        };

        timer.Start();
        logger.WriteLog($"[SCHEDULED] - Synchronization will run every {intervalSeconds} seconds");
        Console.WriteLine("Press Enter to Abort...");
        Console.ReadLine();

        timer.Stop();
        logger.WriteLog("[ABORTED] - Synchronization stopped by user");
    }
}