public class ServerThreadTests
{
    class TestFlagCommand : ICommand
    {
        public bool Executed = false;
        public void Execute() => Executed = true;
    }

    class TestLongCommand : ISteppableCommand
    {
        public int Counter = 0;
        public int Steps;
        public bool ExecuteStep()
        {
            Counter++;
            return Counter >= Steps;
        }
        public void Execute() { while (!ExecuteStep()) ; }
    }

    [Fact]
    public void ServerThread_Executes_All_Normal_Commands()
    {
        var server = new ServerThread();
        var c1 = new TestFlagCommand();
        var c2 = new TestFlagCommand();
        server.Start();
        server.AddCommands(new List<ICommand> { c1, c2, new SoftStopCommand(server) });

        server.Join();

        Assert.True(c1.Executed);
        Assert.True(c2.Executed);
    }

    [Fact]
    public void ServerThread_Executes_LongCommands_RoundRobin()
    {
        var server = new ServerThread();
        var l1 = new TestLongCommand { Steps = 3 };
        var l2 = new TestLongCommand { Steps = 2 };
        server.Start();
        server.AddCommands(new List<ICommand>
        {
            l1,
            l2,
            new SoftStopCommand(server)
        });

        server.Join();

        Assert.Equal(3, l1.Counter);
        Assert.Equal(2, l2.Counter);
    }

    [Fact]
    public void HardStop_Terminates_Immediately()
    {
        var server = new ServerThread();
        var c1 = new TestFlagCommand();
        var c2 = new TestFlagCommand();
        server.Start();
        server.AddCommands(new List<ICommand>
        {
            c1,
            new HardStopCommand(server),
            c2
        });

        server.Join();

        Assert.True(c1.Executed);
        Assert.False(c2.Executed);
    }

    [Fact]
    public void SoftStop_Waits_For_Ongoing_LongCommands()
    {
        var server = new ServerThread();
        var l1 = new TestLongCommand { Steps = 5 };
        var l2 = new TestLongCommand { Steps = 4 };
        server.Start();
        server.AddCommands(new List<ICommand>
        {
            l1,
            l2,
            new SoftStopCommand(server)
        });

        server.Join();

        Assert.Equal(5, l1.Counter);
        Assert.Equal(4, l2.Counter);
    }

    [Fact]
    public void SoftStop_Throws_If_Called_From_Wrong_Thread()
    {
        var server = new ServerThread();
        server.Start();
        Thread.Sleep(50);
        var ex = Assert.Throws<InvalidOperationException>(() =>
            new SoftStopCommand(server).Execute()
        );
        Assert.Contains("SoftStop", ex.Message);
        server.Enqueue(new HardStopCommand(server));
        server.Join();
    }

    [Fact]
    public void HardStop_Throws_If_Called_From_Wrong_Thread()
    {
        var server = new ServerThread();
        server.Start();
        Thread.Sleep(50);
        var ex = Assert.Throws<InvalidOperationException>(() =>
            new HardStopCommand(server).Execute()
        );
        Assert.Contains("HardStop", ex.Message);
        server.Enqueue(new SoftStopCommand(server));
        server.Join();
    }
}
