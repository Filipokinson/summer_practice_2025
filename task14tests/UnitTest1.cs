using System;
using Xunit;

public class DefiniteIntegralTests
{
    [Fact]
    public void Test_LinearFunction_SymmetricInterval()
    {
        Func<double, double> X = x => x;
        double result = DefiniteIntegral.Solve(-1, 1, X, 1e-4, 2);
        Assert.Equal(0, result, 4);
    }

    [Fact]
    public void Test_SineFunction_SymmetricInterval()
    {
        Func<double, double> SIN = Math.Sin;
        double result = DefiniteIntegral.Solve(-1, 1, SIN, 1e-5, 8);
        Assert.Equal(0, result, 4);
    }

    [Fact]
    public void Test_LinearFunction_0to5()
    {
        Func<double, double> X = x => x;
        double result = DefiniteIntegral.Solve(0, 5, X, 1e-6, 8);
        Assert.Equal(12.5, result, 5);
    }
}
