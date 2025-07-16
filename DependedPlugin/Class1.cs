[PluginLoad("BasicPlugin")]
public class DependedPlugin : IPlugin
{
    public string Name => "DependedPlugin";
    public string Description => "Плагин с зависимостью";
    
    public void Execute()
    {
        Console.WriteLine("Зависимый плагин выполнен");
    }
}
