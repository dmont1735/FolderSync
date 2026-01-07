namespace FolderSync;
using FolderSync;
class Program
{
    static void Main(string[] args)
    {
        FolderSynchronizer synchronizer = new FolderSynchronizer();

        var sourcePath = args[0];
        var replicaPath = args[1];
        
        synchronizer.Synchronize(sourcePath,replicaPath);
    }
}