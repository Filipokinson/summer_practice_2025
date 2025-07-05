using System.Reflection;

public class DisplayNameAttribute : Attribute
{
    public string DisplayName { get; }
    public DisplayNameAttribute(string displayName)
    {
        DisplayName = displayName;
    }
}

public class VersionAttribute : Attribute
{
    public int Major { get; }
    public int Minor { get; }
    public VersionAttribute(int major, int minor)
    {
        Major = major;
        Minor = minor;
    }
}

[DisplayName("Пример класса")]
[Version(1, 0)]
public class SampleClass
{
    [DisplayName("Числовое свойство")]
    public int Number { get; set; }

    [DisplayName("Тестовый метод")]
    public void TestMethod() { }
}

public static class ReflectionHelper
{
    public static void PrintTypeInfo(Type type)
    {
        var displayNameAttribute = type.GetCustomAttribute<DisplayNameAttribute>();
        if (displayNameAttribute != null)
            Console.WriteLine($"Класс: {displayNameAttribute.DisplayName}");
        else
            Console.WriteLine($"Класс: {type.Name}");

        var versionAttribute = type.GetCustomAttribute<VersionAttribute>();
        if (versionAttribute != null)
            Console.WriteLine($"Версия: {versionAttribute.Major}.{versionAttribute.Minor}");

        foreach (var prop in type.GetProperties())
        {
            var propDisplayNameAttribute = prop.GetCustomAttribute<DisplayNameAttribute>();
            string propName = propDisplayNameAttribute != null ? propDisplayNameAttribute.DisplayName : prop.Name;
            Console.WriteLine($"Свойство: {propName}");
        }

        foreach (var method in type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
        {
            var methodDisplayNameAttribute = method.GetCustomAttribute<DisplayNameAttribute>();
            if (methodDisplayNameAttribute != null)
                Console.WriteLine($"Метод: {methodDisplayNameAttribute.DisplayName}");
        }
    }
}