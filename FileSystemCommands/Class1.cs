
namespace FileSystemCommands
{
    public class DirectorySizeCommand : ICommand
    {
        public string path;
        public long size;

        public DirectorySizeCommand(string path)
        {
            this.path = path;
        }

        public void Execute()
        {
            var dirInfo = new DirectoryInfo(path);
            this.size = DirSize(dirInfo);
        }

        public static long DirSize(DirectoryInfo d)
        {
            long total = 0;
            foreach (FileInfo file in d.GetFiles())
                total += file.Length;
            foreach (DirectoryInfo dir in d.GetDirectories())
                total += DirSize(dir);
            return total;
        }
    }

    public class FindFilesCommand : ICommand
    {
        public string path;
        public string pattern;
        public string[]? matchedFiles;

        public FindFilesCommand(string path, string pattern)
        {
            this.path = path;
            this.pattern = pattern;
        }

        public void Execute()
        {
            this.matchedFiles = Directory.GetFiles(path, pattern);
        }
    }
}
