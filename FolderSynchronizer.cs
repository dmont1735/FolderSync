using System.Security.Cryptography;

public class FolderSynchronizer
{
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

    public void Synchronize(string sourcePath, string replicaPath)
    {
        var relativeReplicaPaths = GetRelativeFilePaths(replicaPath);
        var relativeSourcePaths = GetRelativeFilePaths(sourcePath);

        foreach(var(relativePath, fullPath) in relativeReplicaPaths)
        {
            if (!relativeSourcePaths.ContainsKey(relativePath))
            {
                File.Delete(fullPath);
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
                }
                File.Copy(fullPath,replicaFullPath);
            }
            else
            {
                if (FilesAreDifferent(fullPath, replicaFullPath))
                {
                    File.Copy(fullPath,replicaFullPath, overwrite: true);
                }
            }
        }
    }
}
