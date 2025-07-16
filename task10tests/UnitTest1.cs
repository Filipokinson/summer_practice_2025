public class PluginLoaderTest
{
    [Fact]
    public void PluginLoader_ThrowsExceptionDirectoryNotFound()
    {
        var slnDirectory = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.Parent?.FullName;
        if (slnDirectory == null)
            throw new DirectoryNotFoundException();
        var randomDirectory = Path.Combine(slnDirectory, "SomeNonExistentDir");
        var pluginLoader = new PluginLoader();

        Assert.Throws<DirectoryNotFoundException>(() => pluginLoader.PluginsLoad(randomDirectory));
    }

    [Fact]
    public void PluginLoader_Loads_Basic_And_Depended_In_Correct_Order()
    {
        var slnDirectory = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.Parent?.FullName;
        if (slnDirectory == null)
            throw new DirectoryNotFoundException();

        var pluginsDirectory = Path.Combine(slnDirectory, "Plugins");
        var pluginLoader = new PluginLoader();
        var output = new StringWriter();
        Console.SetOut(output);

        pluginLoader.PluginsLoad(pluginsDirectory);

        string result = output.ToString();
        Assert.Contains("Плагин без зависимостей выполнен", result);
        Assert.Contains("Зависимый плагин выполнен", result);
        int indexBasic = result.IndexOf("Плагин без зависимостей выполнен");
        int indexDepended = result.IndexOf("Зависимый плагин выполнен");
        Assert.True(indexBasic < indexDepended, "BasicPlugin должен выполняться раньше DependedPlugin");
    }
}
