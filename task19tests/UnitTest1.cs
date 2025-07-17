using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Xunit;

namespace Task19Tests
{
    public class TestCommand : ISteppableCommand
    {
        private readonly int id;
        private int counter = 0;
        private readonly Action<string>? log;

        public TestCommand(int id, Action<string>? log = null)
        {
            this.id = id;
            this.log = log;
        }

        public bool ExecuteStep()
        {
            string msg = $"Поток {id} вызов {++counter}";
            if (log != null) log(msg);
            else Console.WriteLine(msg);
            return counter >= 3;
        }

        public void Execute()
        {
            while (!ExecuteStep()) { }
        }

        public int Counter => counter;
        public int Id => id;
        public bool IsCompleted => counter >= 3;
    }

    public class HardStopCommand : ICommand
    {
        private readonly ServerThread thread;
        public HardStopCommand(ServerThread thread) => this.thread = thread;
        public void Execute() => thread.RequestHardStop();
    }

    public class UnitTest1
    {
        [Fact]
        public void MultipleTestCommands_AllThreeSteps_AndServerStops()
        {
            var stepLog = new List<string>();

            var server = new ServerThread();
            server.Start();

            var commands = Enumerable.Range(1, 5)
                .Select(i => new TestCommand(i, msg => stepLog.Add(msg)))
                .ToList();

            foreach (var cmd in commands)
                server.Enqueue(cmd);

            new Thread(() =>
            {
                while (true)
                {
                    if (commands.All(c => c.IsCompleted))
                    {
                        server.Enqueue(new HardStopCommand(server));
                        break;
                    }
                    Thread.Sleep(10);
                }
            }).Start();

            server.Join();

            foreach (var cmd in commands)
                Assert.Equal(3, cmd.Counter);

            Assert.Equal(15, stepLog.Count);
            Assert.True(commands.All(c => c.IsCompleted));

            for (int i = 1; i <= 5; i++)
                Assert.Contains($"Поток {i} вызов 1", stepLog);
        }
    }
}
