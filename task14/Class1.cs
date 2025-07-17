using System;
using System.Threading.Tasks;

public class DefiniteIntegral
{
    public static double Solve(double a, double b, Func<double, double> function, double step, int threadsNumber)
    {
        double result = 0.0;
        object locker = new object();

        int totalSteps = (int)Math.Ceiling((b - a) / step);
        int stepsPerThread = totalSteps / threadsNumber;
        int remainder = totalSteps % threadsNumber;

        Parallel.For(0, threadsNumber, new ParallelOptions { MaxDegreeOfParallelism = threadsNumber }, threadIdx =>
        {
            int startIdx = threadIdx * stepsPerThread + Math.Min(threadIdx, remainder);
            int count = stepsPerThread + (threadIdx < remainder ? 1 : 0);
            double localA = a + startIdx * step;
            double localB = localA + count * step;
            if (localB > b) localB = b;

            double localResult = 0.0;
            double x0 = localA;
            for (int j = 0; j < count; j++)
            {
                double x1 = x0 + step;
                if (x1 > b) x1 = b;
                double f0 = function(x0);
                double f1 = function(x1);
                localResult += 0.5 * (f0 + f1) * (x1 - x0);
                x0 = x1;
            }

            lock (locker)
            {
                result += localResult;
            }
        });

        return result;
    }
}

public class DefiniteIntegralSingleThread
{
    public static double SolveSimple(double a, double b, Func<double, double> function, double step)
    {
        double result = 0.0;
        double x = a;

        while (x + step < b)
        {
            double f0 = function(x);
            double f1 = function(x + step);
            result += 0.5 * (f0 + f1) * step;
            x += step;
        }
        if (x < b)
        {
            double f0 = function(x);
            double f1 = function(b);
            result += 0.5 * (f0 + f1) * (b - x);
        }

        return result;
    }
}
