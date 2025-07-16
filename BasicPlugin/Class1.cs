[PluginLoad]
public class BasicPlugin : IPlugin
{
    public string Name => "BasicPlugin";
    public string Description => "Плагин без зависимостей";
    
    public void Execute()
    {
        Console.WriteLine("Плагин без зависимостей выполнен");
    }
}
