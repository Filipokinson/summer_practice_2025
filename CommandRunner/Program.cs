using System.Reflection;

class Program
{
    static void Main()
    {
        string path = Path.Combine(Directory.GetCurrentDirectory(), "TestDir");

        Assembly assembly = Assembly.LoadFrom("../FileSystemCommands/bin/Debug/net9.0/FileSystemCommands.dll");

        var dszType = assembly.GetType("FileSystemCommands.DirectorySizeCommand");
        var dszInstance = (ICommand)Activator.CreateInstance(dszType, path);
        dszInstance.Execute();

        var ffcType = assembly.GetType("FileSystemCommands.FindFilesCommand");
        var ffcInstance = (ICommand)Activator.CreateInstance(ffcType, path);
        ffcInstance.Execute();
    }
}
