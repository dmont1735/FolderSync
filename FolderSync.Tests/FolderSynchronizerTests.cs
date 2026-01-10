namespace FolderSync.Tests;
using Xunit;

public class FolderSynchronizerTests : IDisposable
{
    private readonly string _testRoot;
    private readonly string _sourcePath;
    private readonly string _replicaPath;
    private readonly string _logPath;
    private readonly Logger _logger;
    private readonly FolderSynchronizer _synchronizer;

     public FolderSynchronizerTests()
    {
        _testRoot = Path.Combine(Path.GetTempPath(),$"FolderSynchronizerTests_{Guid.NewGuid()}");
        _sourcePath = Path.Combine(_testRoot, "source");
        _replicaPath = Path.Combine(_testRoot, "replica");
        _logPath = Path.Combine(_testRoot, "test.log");

        Directory.CreateDirectory(_sourcePath);
        Directory.CreateDirectory(_replicaPath);

        _logger = new Logger(_logPath);
        _synchronizer =  new FolderSynchronizer(_logger);
    }

    [Fact]
    public void Synchronize_CopiesNewFile_ToReplica()
    {
        var sourceFile = Path.Combine(_sourcePath, "newFile.txt");
        File.WriteAllText(sourceFile, "file to copy");

        _synchronizer.Synchronize(_sourcePath, _replicaPath);

        var replicaFile = Path.Combine(_replicaPath, "newFile.txt");

        Assert.True(File.Exists(replicaFile));
        Assert.Equal("file to copy", File.ReadAllText(replicaFile));
    }

    [Fact]
    public void Synchronize_DeletesOldFile_FromReplica()
    {
        var replicaFile = Path.Combine(_replicaPath, "oldFile.txt");
        File.WriteAllText(replicaFile, "old file");

        _synchronizer.Synchronize(_sourcePath, _replicaPath);

        Assert.False(File.Exists(replicaFile));
    }

    [Fact]
    public void Synchronize_OverwritesFile_InReplica()
    {
        var replicaFile = Path.Combine(_replicaPath, "file.txt");
        File.WriteAllText(replicaFile, "old content");
        
        var sourceFile = Path.Combine(_sourcePath, "file.txt");
        File.WriteAllText(sourceFile, "new content");

        _synchronizer.Synchronize(_sourcePath, _replicaPath);

        Assert.Equal("new content", File.ReadAllText(replicaFile));
    }

    [Fact]
    public void Synchronize_IgnoresIdenticalFile_InReplica()
    {
        var sourceFile = Path.Combine(_sourcePath, "file.txt");
        var replicaFile = Path.Combine(_replicaPath, "file.txt");

        File.WriteAllText(sourceFile, "same content");
        File.WriteAllText(replicaFile, "same content");

        var replicaFileLastWritten = new FileInfo(replicaFile).LastWriteTime;
        Thread.Sleep(1000);

        _synchronizer.Synchronize(_sourcePath, _replicaPath);

        Assert.Equal(replicaFileLastWritten, new FileInfo(replicaFile).LastWriteTime);
    }

    [Fact]
    public void Synchronize_CreatesReplicaDirectory_IfNonExistent()
    {
        var sourceFile = Path.Combine(_sourcePath, "file.txt");
        File.WriteAllText(sourceFile, "file to copy");

        var _nonExistentFolder = Path.Combine(_testRoot, "nonExistentFolder");

        _synchronizer.Synchronize(_sourcePath, _nonExistentFolder);

        Assert.True(Directory.Exists(_nonExistentFolder));
    }

    [Fact]
    public void Synchronize_CreatesNestedDirectories_WhenCopying()
    {
        var subfolder = Path.Combine(_sourcePath, "subfolder");
        Directory.CreateDirectory(subfolder);
        var sourceFile = Path.Combine(_sourcePath, "subfolder", "file.txt");
        File.WriteAllText(sourceFile, "file to copy");

        _synchronizer.Synchronize(_sourcePath, _replicaPath);

        Assert.True(Directory.Exists(Path.Combine(_replicaPath, "subfolder")));
    }

    [Fact]
    public void Synchronize_DeletesEmptyDirectories_FromReplica_AfterSync()
    {
        var sourceFile = Path.Combine(_sourcePath, "file.txt");
        File.WriteAllText(sourceFile, "file to copy");

        var emptyReplicaFolder = Path.Combine(_replicaPath, "emptyReplicaFolder");
        Directory.CreateDirectory(emptyReplicaFolder);

        _synchronizer.Synchronize(_sourcePath, _replicaPath);

        Assert.False(Directory.Exists(emptyReplicaFolder));
    }

    public void Dispose()
    {
        if (Directory.Exists(_testRoot))
        {
            Directory.Delete(_testRoot, recursive: true);
        }
    }
}