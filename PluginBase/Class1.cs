public interface IPlugin
{
    void Execute();
    string Name { get; }
    string Description { get; }
}

[AttributeUsage(AttributeTargets.Class)]
public class PluginLoadAttribute : Attribute
{
    public string[] Dependencies { get; set; } = new string[0];
    
    public PluginLoadAttribute(params string[] dependencies)
    {
        Dependencies = dependencies ?? new string[0];
    }
}
