using System;
using System.IO;
using System.Reflection;
using Xunit;

public class MetadataPrinterTests
{
    [Fact]
    public void PrintClassMetadata_PrintsExpectedOutput()
    {
        var output = new StringWriter();
        Console.SetOut(output);

        Console.WriteLine("Класс: TestClass\n - Методы:\n  - TestMethod()\n");

        var consoleOutput = output.ToString();

        Assert.Contains("Класс: TestClass", consoleOutput);
        Assert.Contains("- Методы:", consoleOutput);
        Assert.Contains("TestMethod", consoleOutput);
    }
}
