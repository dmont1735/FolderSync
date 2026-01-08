namespace FolderSync;
using FolderSync;
using System;
using System.Timers;
class Program
{
    static void Main(string[] args)
    {

        var sourcePath = args[0];
        var replicaPath = args[1];
        var logFilePath = args[2];
        
        FolderSynchronizer synchronizer = new FolderSynchronizer(logFilePath);
        var intervalSeconds = int.Parse(args[2]);
        var logFilePath = args[3];
        Logger logger = new Logger(logFilePath);
        FolderSynchronizer synchronizer = new FolderSynchronizer(logger);

        synchronizer.Synchronize(sourcePath,replicaPath);

        var timer = new System.Timers.Timer(intervalSeconds * 1000);
        timer.Elapsed += (sender, e) =>
        {
            try
            {
                synchronizer.Synchronize(sourcePath,replicaPath);
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