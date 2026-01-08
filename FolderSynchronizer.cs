using System.Security.Cryptography;

public class FolderSynchronizer
{
    private readonly Logger logger;
    public FolderSynchronizer(string logFilePath)
    {
        logger = new Logger(logFilePath);
    }

    private bool FilesAreDifferent(string filePath1, string filePath2)
    {
        var info1 = new FileInfo(filePath1);
        var info2 = new FileInfo(filePath2);

        if (info1.Length != info2.Length)
            return true;

        using var md5 = MD5.Create();
        using var stream1 = File.OpenRead(filePath1);
        byte[] hash1 = md5.ComputeHash(stream1);

        using var stream2 = File.OpenRead(filePath2);
        byte[] hash2 = md5.ComputeHash(stream2);

        return !hash1.SequenceEqual(hash2);
    }

    private Dictionary<string, string> GetRelativeFilePaths(string directoryPath)
    {
        var sourceFiles = Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories);
        var relativePaths = new Dictionary<string, string>();

        foreach (string file in sourceFiles)
        {
            var relativePath = Path.GetRelativePath(directoryPath, file);
            relativePaths[relativePath] = file;
        }

        return relativePaths;
    }

    private void DeleteEmptySubdirectories(string directoryPath)
    {
        foreach(var directory in Directory.GetDirectories(directoryPath, "*" ,SearchOption.AllDirectories))
        {
            if (!Directory.EnumerateFileSystemEntries(directory).Any())
            {
                Directory.Delete(directory);
                logger.WriteLog($"DELETED folder {directory}");
            }
        }
    }

    public void Synchronize(string sourcePath, string replicaPath)
    {
        logger.WriteLog($"[INITIALIZING] - Synchronization of folder {sourcePath} to folder {replicaPath} started");
        var startTime = DateTime.Now;

        try
        {
            if (!Directory.Exists(sourcePath)){
                var ex = new DirectoryNotFoundException(message: $"Source folder could not be found at given location {sourcePath}");
                throw ex;
            }
            if (!Directory.Exists(replicaPath)){
                Directory.CreateDirectory(replicaPath);
                logger.WriteLog($"CREATED folder {replicaPath}");
            }

            var relativeReplicaPaths = GetRelativeFilePaths(replicaPath);
            var relativeSourcePaths = GetRelativeFilePaths(sourcePath);

            foreach(var(relativePath, fullPath) in relativeReplicaPaths)
            {
                if (!relativeSourcePaths.ContainsKey(relativePath))
                {
                    File.Delete(fullPath);
                    logger.WriteLog($"DELETED file {fullPath}");
                }
            }

            foreach(var (relativePath, fullPath) in relativeSourcePaths)
            {
                var replicaFullPath = Path.Combine(replicaPath,relativePath);

                if (!relativeReplicaPaths.ContainsKey(relativePath))
                {
                    var directoryPath = Path.GetDirectoryName(replicaFullPath);
                    if (!string.IsNullOrEmpty(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                        logger.WriteLog($"CREATED folder {directoryPath}");
                    }
                    File.Copy(fullPath, replicaFullPath);
                    logger.WriteLog($"COPIED file {fullPath} to replica location at {replicaFullPath}");
                }
                else
                {
                    if (FilesAreDifferent(fullPath, replicaFullPath))
                    {
                        File.Copy(fullPath, replicaFullPath, overwrite: true);
                        logger.WriteLog($"UPDATED file {replicaFullPath} with source copy from {fullPath}");
                    }
                }
                DeleteEmptySubdirectories(replicaPath);
            }
            var syncDuration = DateTime.Now - startTime;
            logger.WriteLog($"[SUCCESS] - Synchronization of folder {sourcePath} to folder {replicaPath} completed in {syncDuration.TotalSeconds} seconds");
        }
        catch(Exception e)
        {
            logger.WriteLog($"[ERROR] - {e.Message}");
        }
    }
}
