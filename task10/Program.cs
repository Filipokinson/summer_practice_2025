using System.Reflection;

public class PluginLoader
{
    public void PluginsLoad(string path)
    {
        var dllPaths = Directory.GetFiles(path, "*.dll", SearchOption.AllDirectories);

        var allPlugins = dllPaths
            .SelectMany(p => Assembly.LoadFrom(p).GetTypes())
            .Where(t => t.GetCustomAttribute<PluginLoadAttribute>() != null)
            .ToList();

        var independentPlugins = allPlugins
            .Where(p => p.GetCustomAttribute<PluginLoadAttribute>()!.Dependencies.Length == 0)
            .ToList();

        List<Type> pluginOrder = new List<Type>();

        while (independentPlugins.Any())
        {
            var curPlugin = independentPlugins.First();
            pluginOrder.Add(curPlugin);
            independentPlugins.RemoveAt(0);

            foreach (var p in allPlugins.Except(pluginOrder))
            {
                var dependencies = p
                    .GetCustomAttribute<PluginLoadAttribute>()!
                    .Dependencies
                    .Except(pluginOrder.Select(d => d.Name));

                if (!dependencies.Any()) 
                    independentPlugins.Add(p);
            }
        }

        foreach (var p in pluginOrder)
        {
            var plugin = Activator.CreateInstance(p);
            var method = p.GetMethod("Execute");
            method!.Invoke(plugin, null);
        }
    }
}
