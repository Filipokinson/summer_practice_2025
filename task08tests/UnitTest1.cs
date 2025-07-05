using Xunit;
using FileSystemCommands;

public class FileSystemCommandTests
{
    [Fact]
    public void DirectorySizeCommand_ShouldCalculateSize()
    {
        var testDir = Path.Combine(Path.GetTempPath(), "TestDir");
        Directory.CreateDirectory(testDir);
        var file1 = Path.Combine(testDir, "test1.txt");
        var file2 = Path.Combine(testDir, "test2.txt");
        File.WriteAllText(file1, "Hello");
        File.WriteAllText(file2, "World");

        var expectedSize = new FileInfo(file1).Length + new FileInfo(file2).Length;

        var command = new DirectorySizeCommand(testDir);
        command.Execute();

        Assert.Equal(expectedSize, command.size);

        Directory.Delete(testDir, true);
    }

    [Fact]
    public void FindFilesCommand_ShouldFindMatchingFiles()
    {
        var testDir = Path.Combine(Path.GetTempPath(), "TestDir");
        Directory.CreateDirectory(testDir);
        var file1 = Path.Combine(testDir, "file1.txt");
        var file2 = Path.Combine(testDir, "file2.log");
        File.WriteAllText(file1, "Text");
        File.WriteAllText(file2, "Log");

        var command = new FindFilesCommand(testDir, "*.txt");
        command.Execute();

        Assert.Single(command.matchedFiles);
        Assert.Contains(file1, command.matchedFiles);

        Directory.Delete(testDir, true);
    }

}