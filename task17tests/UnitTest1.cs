using Xunit;
using System.Collections.Generic;
using System.Threading;

public class UnitTest1
{
    public class TestCommand : ICommand
    {
        public bool flag;
        public void Execute() => flag = true;
    }

    [Fact]
    public void Executes_All_Commands()
    {
        var server = new ServerThread();
        var c1 = new TestCommand();
        var c2 = new TestCommand();

        server.Start();
        server.AddCommands(new List<ICommand> { c1, c2 });

        Thread.Sleep(100);
        Assert.True(c1.flag);
        Assert.True(c2.flag);
    }

    [Fact]
    public void SoftStop_Completes_Queue()
    {
        var server = new ServerThread();
        var c1 = new TestCommand();
        var c2 = new TestCommand();
        server.Start();
        server.AddCommands(new List<ICommand> { c1, new SoftStopCommand(server), c2 });

        Thread.Sleep(100);
        Assert.True(c1.flag);
        Assert.True(c2.flag);
    }

    [Fact]
    public void HardStop_Stops_Immediately()
    {
        var server = new ServerThread();
        var c1 = new TestCommand();
        var c2 = new TestCommand();
        server.Start();
        server.AddCommands(new List<ICommand> { c1, new HardStopCommand(server), c2 });

        Thread.Sleep(100);
        Assert.True(c1.flag);
        Assert.False(c2.flag);
    }

    [Fact]
    public void SoftStop_Throws_Outside_Thread()
    {
        var server = new ServerThread();
        server.Start();
        var ex = Assert.Throws<InvalidOperationException>(() => new SoftStopCommand(server).Execute());
        Assert.Contains("SoftStop", ex.Message);
    }

    [Fact]
    public void HardStop_Throws_Outside_Thread()
    {
        var server = new ServerThread();
        server.Start();
        var ex = Assert.Throws<InvalidOperationException>(() => new HardStopCommand(server).Execute());
        Assert.Contains("HardStop", ex.Message);
    }
}
