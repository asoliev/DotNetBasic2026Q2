namespace Module3.FSVisitorLib.Tests;

public class FileSystemVisitorTests
{
    [Fact]
    public void Constructor_WithMissingDirectory_ThrowsDirectoryNotFoundException()
    {
        string missingPath = Path.Combine(Path.GetTempPath(), $"fsvisitor-missing-{Guid.NewGuid():N}");

        Assert.Throws<DirectoryNotFoundException>(() => new FileSystemVisitor(missingPath));
    }

    [Fact]
    public void EnumeratingWithoutFilter_ReturnsAllItems_RaisesLifecycleEvents_AndCountsEverything()
    {
        using var fileSystem = new TemporaryFileSystem();
        string docsDirectory = fileSystem.CreateDirectory("docs");
        string imagesDirectory = fileSystem.CreateDirectory("images");
        string alphaFile = fileSystem.CreateFile("alpha.txt");
        string betaFile = fileSystem.CreateFile("beta.log");
        string gammaFile = fileSystem.CreateFile(Path.Combine("docs", "gamma.txt"));
        string deltaFile = fileSystem.CreateFile(Path.Combine("images", "delta.png"));

        var visitor = new FileSystemVisitor(fileSystem.RootPath);
        var lifecycleEvents = new List<string>();
        var foundDirectories = new List<string>();
        var foundFiles = new List<string>();

        visitor.SearchStarted += (_, _) => lifecycleEvents.Add("SearchStarted");
        visitor.DirectoryFound += (_, args) => foundDirectories.Add(args.Path);
        visitor.FileFound += (_, args) => foundFiles.Add(args.Path);
        visitor.SearchFinished += (_, _) => lifecycleEvents.Add("SearchFinished");

        List<string> results = [.. visitor];

        Assert.Equal("SearchStarted", lifecycleEvents.First());
        Assert.Equal("SearchFinished", lifecycleEvents.Last());
        AssertPathsEqual(
            [fileSystem.RootPath, docsDirectory, imagesDirectory, alphaFile, betaFile, gammaFile, deltaFile],
            results);
        AssertPathsEqual([fileSystem.RootPath, docsDirectory, imagesDirectory], foundDirectories);
        AssertPathsEqual([alphaFile, betaFile, gammaFile, deltaFile], foundFiles);
        Assert.Equal(4, visitor.AllFilesFoundCount);
        Assert.Equal(3, visitor.AllDirectoriesFoundCount);
    }

    [Fact]
    public void EnumeratingWithFilter_ReturnsOnlyMatches_AndRaisesFilteredEvents()
    {
        using var fileSystem = new TemporaryFileSystem();
        string keepDirectory = fileSystem.CreateDirectory("keep-dir");
        fileSystem.CreateDirectory("skip-dir");
        string keepFile = fileSystem.CreateFile("keep.txt");
        fileSystem.CreateFile("skip.log");
        string nestedKeepFile = fileSystem.CreateFile(Path.Combine("keep-dir", "nested.txt"));
        fileSystem.CreateFile(Path.Combine("skip-dir", "other.log"));

        static bool Filter(string path) =>
            path.EndsWith(".txt", StringComparison.OrdinalIgnoreCase)
            || Path.GetFileName(path).Contains("keep", StringComparison.OrdinalIgnoreCase);

        var visitor = new FileSystemVisitor(fileSystem.RootPath, Filter);
        var filteredDirectories = new List<string>();
        var filteredFiles = new List<string>();
        int directoryFoundCount = 0;
        int fileFoundCount = 0;

        visitor.DirectoryFound += (_, _) => directoryFoundCount++;
        visitor.FileFound += (_, _) => fileFoundCount++;
        visitor.FilteredDirectoryFound += (_, args) => filteredDirectories.Add(args.Path);
        visitor.FilteredFileFound += (_, args) => filteredFiles.Add(args.Path);

        List<string> results = [.. visitor];

        AssertPathsEqual([keepDirectory, keepFile, nestedKeepFile], results);
        AssertPathsEqual([keepDirectory], filteredDirectories);
        AssertPathsEqual([keepFile, nestedKeepFile], filteredFiles);
        Assert.Equal(3, directoryFoundCount);
        Assert.Equal(4, fileFoundCount);
        Assert.Equal(3, visitor.AllDirectoriesFoundCount);
        Assert.Equal(4, visitor.AllFilesFoundCount);
    }

    [Fact]
    public void ExcludeFlags_SkipItemsFromResults_ButTraversalContinues()
    {
        using var fileSystem = new TemporaryFileSystem();
        string excludedDirectory = fileSystem.CreateDirectory("exclude-dir");
        string filteredExcludedDirectory = fileSystem.CreateDirectory("exclude-filtered-dir");
        string keptDirectory = fileSystem.CreateDirectory("keep-dir");
        string childFromExcludedDirectory = fileSystem.CreateFile(Path.Combine("exclude-dir", "child.txt"));
        string childFromFilteredExcludedDirectory = fileSystem.CreateFile(Path.Combine("exclude-filtered-dir", "child2.txt"));
        string keptNestedFile = fileSystem.CreateFile(Path.Combine("keep-dir", "keep.txt"));
        fileSystem.CreateFile("exclude-raw.txt");
        fileSystem.CreateFile("exclude-filtered.txt");
        string keptRootFile = fileSystem.CreateFile("keep-root.txt");

        static bool Filter(string path) =>
            path.EndsWith(".txt", StringComparison.OrdinalIgnoreCase)
            || Path.GetFileName(path).Contains("dir", StringComparison.OrdinalIgnoreCase);

        var visitor = new FileSystemVisitor(fileSystem.RootPath, Filter);

        visitor.DirectoryFound += (_, args) =>
        {
            if (Path.GetFileName(args.Path) == Path.GetFileName(excludedDirectory))
                args.Exclude = true;
        };
        visitor.FilteredDirectoryFound += (_, args) =>
        {
            if (Path.GetFileName(args.Path) == Path.GetFileName(filteredExcludedDirectory))
                args.Exclude = true;
        };
        visitor.FileFound += (_, args) =>
        {
            if (Path.GetFileName(args.Path) == "exclude-raw.txt")
                args.Exclude = true;
        };
        visitor.FilteredFileFound += (_, args) =>
        {
            if (Path.GetFileName(args.Path) == "exclude-filtered.txt")
                args.Exclude = true;
        };

        List<string> results = [.. visitor];

        AssertPathsEqual(
            [keptDirectory, childFromExcludedDirectory, childFromFilteredExcludedDirectory, keptNestedFile, keptRootFile
            ],
            results);
        Assert.Equal(4, visitor.AllDirectoriesFoundCount);
        Assert.Equal(6, visitor.AllFilesFoundCount);
    }

    [Fact]
    public void AbortFromFilteredFileFound_StopsEnumeration_AndStillRaisesSearchFinished()
    {
        using var fileSystem = new TemporaryFileSystem();
        fileSystem.CreateDirectory("later");
        fileSystem.CreateFile(Path.Combine("later", "later.txt"));
        fileSystem.CreateFile("stop.txt");

        var visitor = new FileSystemVisitor(fileSystem.RootPath, _ => true);
        var lifecycleEvents = new List<string>();
        bool abortTriggered = false;

        visitor.SearchStarted += (_, _) => lifecycleEvents.Add("SearchStarted");
        visitor.FilteredFileFound += (_, args) =>
        {
            if (Path.GetFileName(args.Path) != "stop.txt") return;
            abortTriggered = true;
            args.Abort = true;
        };
        visitor.SearchFinished += (_, _) => lifecycleEvents.Add("SearchFinished");

        List<string> results = [.. visitor];

        Assert.True(abortTriggered);
        AssertPathsEqual([fileSystem.RootPath], results);
        Assert.Equal("SearchStarted", lifecycleEvents.First());
        Assert.Equal("SearchFinished", lifecycleEvents.Last());
        Assert.Equal(1, visitor.AllDirectoriesFoundCount);
        Assert.Equal(1, visitor.AllFilesFoundCount);
    }

    [Fact]
    public void DeletedSubdirectoryDuringTraversal_DoesNotThrow_AndTraversalContinues()
    {
        using var fileSystem = new TemporaryFileSystem();
        string volatileDirectory = fileSystem.CreateDirectory("volatile");
        fileSystem.CreateFile(Path.Combine("volatile", "temp.txt"));
        string stableDirectory = fileSystem.CreateDirectory("stable");
        string stableFile = fileSystem.CreateFile(Path.Combine("stable", "keep.txt"));

        var visitor = new FileSystemVisitor(fileSystem.RootPath);
        var lifecycleEvents = new List<string>();

        visitor.SearchStarted += (_, _) => lifecycleEvents.Add("SearchStarted");
        visitor.DirectoryFound += (_, args) =>
        {
            if (Path.GetFullPath(args.Path) == Path.GetFullPath(volatileDirectory) && Directory.Exists(volatileDirectory))
                Directory.Delete(volatileDirectory, recursive: true);
        };
        visitor.SearchFinished += (_, _) => lifecycleEvents.Add("SearchFinished");

        Exception? exception = Record.Exception(() => visitor.ToList());

        Assert.Null(exception);
        Assert.Equal("SearchStarted", lifecycleEvents.First());
        Assert.Equal("SearchFinished", lifecycleEvents.Last());
        Assert.True(Directory.Exists(stableDirectory));
        Assert.True(File.Exists(stableFile));
    }

    private static void AssertPathsEqual(IEnumerable<string> expected, IEnumerable<string> actual)
    {
        string[] normalizedExpected = [.. expected
            .Select(Path.GetFullPath)
            .OrderBy(path => path, StringComparer.Ordinal)];
        string[] normalizedActual = [.. actual
            .Select(Path.GetFullPath)
            .OrderBy(path => path, StringComparer.Ordinal)];

        Assert.Equal(normalizedExpected, normalizedActual);
    }
}

file sealed class TemporaryFileSystem : IDisposable
{
    public TemporaryFileSystem()
    {
        RootPath = Path.Combine(Path.GetTempPath(), $"fsvisitor-tests-{Guid.NewGuid():N}");
        Directory.CreateDirectory(RootPath);
    }

    public string RootPath { get; }

    public string CreateDirectory(string relativePath)
    {
        string fullPath = Path.Combine(RootPath, relativePath);
        Directory.CreateDirectory(fullPath);
        return fullPath;
    }

    public string CreateFile(string relativePath, string contents = "test")
    {
        string fullPath = Path.Combine(RootPath, relativePath);
        string? directoryPath = Path.GetDirectoryName(fullPath);

        if (!string.IsNullOrEmpty(directoryPath))
            Directory.CreateDirectory(directoryPath);

        File.WriteAllText(fullPath, contents);
        return fullPath;
    }

    public void Dispose()
    {
        if (Directory.Exists(RootPath))
            Directory.Delete(RootPath, recursive: true);
    }
}
