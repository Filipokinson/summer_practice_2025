using Xunit;
using System.Reflection;

public class AttributeReflectionTests
{
    [Fact]
    public void Class_HasDisplayNameAttribute()
    {
        var type = typeof(SampleClass);
        var attribute = type.GetCustomAttribute<DisplayNameAttribute>();
        Assert.NotNull(attribute);
        Assert.Equal("Пример класса", attribute.DisplayName);
    }

    [Fact]
    public void Method_HasDisplayNameAttribute()
    {
        var method = typeof(SampleClass).GetMethod("TestMethod");
        var attribute = method.GetCustomAttribute<DisplayNameAttribute>();
        Assert.NotNull(attribute);
        Assert.Equal("Тестовый метод", attribute.DisplayName);
    }

    [Fact]
    public void Property_HasDisplayNameAttribute()
    {
        var prop = typeof(SampleClass).GetProperty("Number");
        var attribute = prop.GetCustomAttribute<DisplayNameAttribute>();
        Assert.NotNull(attribute);
        Assert.Equal("Числовое свойство", attribute.DisplayName);
    }

    [Fact]
    public void Class_HasVersionAttribute()
    {
        var type = typeof(SampleClass);
        var attribute = type.GetCustomAttribute<VersionAttribute>();
        Assert.NotNull(attribute);
        Assert.Equal(1, attribute.Major);
        Assert.Equal(0, attribute.Minor);
    }
    [Fact]
    public void PrintTypeInfo_PrintsExpectedFragments()
    {
        var type = typeof(SampleClass);
        using (var sw = new StringWriter())
        {
            var originalOut = Console.Out;
            Console.SetOut(sw);
            ReflectionHelper.PrintTypeInfo(type);

            var output = sw.ToString();

            Assert.Contains("Класс: Пример класса", output);
            Assert.Contains("Версия: 1.0", output);
            Assert.Contains("Свойство: Числовое свойство", output);
            Assert.Contains("Метод: Тестовый метод", output);
            Console.SetOut(originalOut);
        }
    }
}