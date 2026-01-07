using System.Security.Cryptography;

public class FolderSynchronizer
{
    public bool FilesAreDifferent(string filePath1, string filePath2)
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

    public void Synchronize(string sourcePath, string replicaPath)
    {
        var sourceFiles = Directory.GetFiles(sourcePath, "*", SearchOption.AllDirectories);
        var replicaFiles = Directory.GetFiles(replicaPath, "*", SearchOption.AllDirectories);

        var relativeReplicaPaths = new Dictionary<string, string>();
        foreach (string file in replicaFiles)
        {
            var relativePath = Path.GetRelativePath(replicaPath, file);
            relativeReplicaPaths[relativePath] = file;
        }

        var relativeSourcePaths = new Dictionary<string, string>();
        foreach (string file in sourceFiles)
        {
            var relativePath = Path.GetRelativePath(sourcePath, file);
            relativeSourcePaths[relativePath] = file;
        }

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

            if (!File.Exists(replicaFullPath)){
                Directory.CreateDirectory(Path.GetDirectoryName(replicaFullPath));
                File.Copy(fullPath,replicaFullPath);
            }
            else
            {
                if (FilesAreDifferent(fullPath, replicaFullPath))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(replicaFullPath));
                    File.Copy(fullPath,replicaFullPath, overwrite: true);
                }
            }
        }
    }
}
