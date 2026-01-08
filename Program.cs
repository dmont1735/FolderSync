namespace FolderSync;
using FolderSync;
class Program
{
    static void Main(string[] args)
    {

        var sourcePath = args[0];
        var replicaPath = args[1];
        var logFilePath = args[2];
        
        FolderSynchronizer synchronizer = new FolderSynchronizer(logFilePath);
        synchronizer.Synchronize(sourcePath,replicaPath);
    }
}