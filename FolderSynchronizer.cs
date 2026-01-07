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
}
