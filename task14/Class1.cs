using System;
using System.Threading;

public class DefiniteIntegral
{
    public static double Solve(double a, double b, Func<double, double> function, double step, int threadsNumber)
    {
        double result = 0.0;
        object locker = new object();

        int totalSteps = (int)Math.Ceiling((b - a) / step);
        int stepsPerThread = totalSteps / threadsNumber;
        int remainder = totalSteps % threadsNumber;

        Thread[] threads = new Thread[threadsNumber];

        for (int i = 0; i < threadsNumber; i++)
        {
            int threadIdx = i;
            threads[i] = new Thread(() =>
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
            threads[i].Start();
        }

        foreach (var t in threads) t.Join();

        return result;
    }
}
