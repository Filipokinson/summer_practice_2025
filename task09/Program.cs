using System;
using System.IO;
using System.Linq;
using System.Reflection;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0 || !File.Exists(args[0]))
        {
            Console.WriteLine("Укажите корректный путь к DLL-файлу.");
            return;
        }

        string dllPath = args[0];
        Assembly assembly = Assembly.LoadFrom(dllPath);

        foreach (Type type in assembly.GetTypes())
        {
            if (!type.IsClass) continue;

            Console.WriteLine($"# Класс: {type.FullName}");

            Console.WriteLine(" - Атрибуты:");
            var attributes = type.GetCustomAttributes(false);
            foreach (var attribute in attributes)
            {
                Console.WriteLine($"  - {attribute.GetType().Name}");
            }

            Console.WriteLine(" - Конструкторы:");
            foreach (var constructor in type.GetConstructors())
            {
                var parameters = constructor.GetParameters();
                string parametersString = string.Join(", ",
                    parameters.Select(p => $"{p.ParameterType.Name} {p.Name}"));
                Console.WriteLine($"  - {constructor.Name}({parametersString})");
            }

            Console.WriteLine(" - Методы:");
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            foreach (var method in methods)
            {
                var parameters = method.GetParameters();
                string parametersString = string.Join(", ",
                    parameters.Select(p => $"{p.ParameterType.Name} {p.Name}"));
                Console.WriteLine($"  - {method.Name}({parametersString})");
            }

            Console.WriteLine();
        }
    }
}
