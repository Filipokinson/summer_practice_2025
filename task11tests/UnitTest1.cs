using Xunit;

public class CalculatorTests
{
    private readonly ICalculator calc;

    public CalculatorTests()
    {
        calc = ClassGenerator.GenerateCalculator();
    }

    [Fact]
    public void Add_Test()
    {
        Assert.Equal(7, calc.Add(3, 4));
    }

    [Fact]
    public void Minus_Test()
    {
        Assert.Equal(2, calc.Minus(5, 3));
    }

    [Fact]
    public void Mul_Test()
    {
        Assert.Equal(20, calc.Mul(4, 5));
    }

    [Fact]
    public void Div_Test()
    {
        Assert.Equal(2, calc.Div(10, 5));
    }

    [Fact]
    public void Div_By_Zero_ShouldThrow()
    {
        Assert.Throws<DivideByZeroException>(() => calc.Div(10, 0));
    }
}
