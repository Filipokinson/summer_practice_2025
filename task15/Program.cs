using System.Diagnostics;


public class OptimalParameters
{
    public double a = -100, b = 100;
    public Func<double, double> SIN = (double x) => Math.Sin(x);

    double TrueIntegralValue(double a, double b) => -Math.Cos(b) + Math.Cos(a);

    public double FindMinStep(double[] steps, double tolerance = 1e-4)
    {
        double trueValue = TrueIntegralValue(a, b);
        foreach (double step in steps)
        {
            double value = DefiniteIntegralSingleThread.SolveSimple(a, b, SIN, step);
            double absError = Math.Abs(value - trueValue);
            if (absError <= tolerance)
                return step;
        }
        return steps.Last();
    }

    public double[] Steps(out List<double> meanTimes)
    {
        double[] steps = { 1e-1, 1e-2, 1e-3, 1e-4, 1e-5, 1e-6 };
        int repeats = 4;
        meanTimes = new List<double>();
        foreach (double step in steps)
        {
            Console.WriteLine($"Шаг: {step}");
            double sumTime = 0.0;
            string allIterationsTime = "";
            for (int i = 0; i < repeats; i++)
            {
                var watch = Stopwatch.StartNew();
                DefiniteIntegral.Solve(a, b, SIN, step, 4);
                watch.Stop();
                sumTime += watch.Elapsed.TotalMilliseconds;
                allIterationsTime += $"{watch.Elapsed.TotalMilliseconds} мс, ";
            }
            double avg = sumTime / repeats;
            meanTimes.Add(avg);
            Console.WriteLine($"Среднее время выполнения: {avg} мс, {allIterationsTime}");
        }
        return steps;
    }

    public void Threads(double step, out Dictionary<int, double> time)
    {
        int[] threads = { 1, 2, 4, 8, 16, 32, 64 };
        int repeats = 4;
        time = new Dictionary<int, double>();
        foreach (int thread in threads)
        {
            Console.WriteLine($"Поток: {thread}");
            double sumTime = 0.0;
            string allIterationsTime = "";
            for (int i = 0; i < repeats; i++)
            {
                var watch = Stopwatch.StartNew();
                DefiniteIntegral.Solve(a, b, SIN, step, thread);
                watch.Stop();
                sumTime += watch.Elapsed.TotalMilliseconds;
                allIterationsTime += $"{watch.Elapsed.TotalMilliseconds} мс, ";
            }
            double avg = sumTime / repeats;
            time[thread] = avg;
            Console.WriteLine($"Среднее время выполнения: {avg} мс, {allIterationsTime}");
        }
    }

    public void SingleThread(double step, out double mean, out string allIterationsTime)
    {
        int repeats = 4;
        double sumTime = 0.0;
        allIterationsTime = "";
        for (int i = 0; i < repeats; i++)
        {
            var watch = Stopwatch.StartNew();
            DefiniteIntegralSingleThread.SolveSimple(a, b, SIN, step);
            watch.Stop();
            sumTime += watch.Elapsed.TotalMilliseconds;
            allIterationsTime += $"{watch.Elapsed.TotalMilliseconds} мс, ";
        }
        mean = sumTime / repeats;
        Console.WriteLine($"Среднее время выполнения: {mean} мс, {allIterationsTime}");
    }

    class Program
    {
        static void Main()
        {
            var stepper = new OptimalParameters();
            double[] steps = { 1e-1, 1e-2, 1e-3, 1e-4, 1e-5, 1e-6 };
            double minStep = stepper.FindMinStep(steps);

            List<double> stepsMeanTimes;
            var usedSteps = stepper.Steps(out stepsMeanTimes);
            Console.WriteLine("\n\n\n");

            Dictionary<int, double> threadTimes;
            stepper.Threads(minStep, out threadTimes);
            Console.WriteLine("\n\n\n");

            double mean;
            string allIterationsTime;
            stepper.SingleThread(minStep, out mean, out allIterationsTime);
            Console.WriteLine("\n\n\n");

            Console.WriteLine("==== 4 замера однопоточного вычисления интеграла ====");
            for (int i = 0; i < 4; i++)
            {
                var watch = Stopwatch.StartNew();
                DefiniteIntegralSingleThread.SolveSimple(stepper.a, stepper.b, stepper.SIN, minStep);
                watch.Stop();
                Console.WriteLine($"Замер {i + 1}: {watch.Elapsed.TotalMilliseconds} мс");
            }

            Console.WriteLine("\n==== 4 замера вычисления интеграла на 16 потоках ====");
            for (int i = 0; i < 4; i++)
            {
                var watch = Stopwatch.StartNew();
                DefiniteIntegral.Solve(stepper.a, stepper.b, stepper.SIN, minStep, 16);
                watch.Stop();
                Console.WriteLine($"Замер {i + 1}: {watch.Elapsed.TotalMilliseconds} мс");
            }
        }
    }
}