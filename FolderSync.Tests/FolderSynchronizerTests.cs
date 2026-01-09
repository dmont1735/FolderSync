using Xunit;
namespace FolderSync.Tests;

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

    public void Dispose()
    {
        if (Directory.Exists(_testRoot))
        {
            Directory.Delete(_testRoot, recursive: true);
        }
    }
}