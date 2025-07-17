using System.Collections.Concurrent;

public interface ICommand
{
    void Execute();
}

public interface ISteppableCommand : ICommand
{
    bool ExecuteStep();
}

public interface IScheduler
{
    bool HasCommand();
    ICommand Select();
    void Add(ICommand cmd);
    void Remove(ICommand cmd);
}

public class RoundRobinScheduler : IScheduler
{
    private readonly Queue<ICommand> queue = new();

    public bool HasCommand() => queue.Count > 0;

    public ICommand Select()
    {
        var cmd = queue.Dequeue();
        queue.Enqueue(cmd);
        return cmd;
    }

    public void Add(ICommand cmd)
    {
        queue.Enqueue(cmd);
    }

    public void Remove(ICommand cmd)
    {
        var list = queue.ToList();
        list.Remove(cmd);
        queue.Clear();
        foreach (var c in list) queue.Enqueue(c);
    }
}

public class ServerThread
{
    private Thread? worker;
    private volatile bool isRunning = true;
    private volatile bool isSoftStop = false;
    private BlockingCollection<ICommand> commandsQueue = new BlockingCollection<ICommand>();
    private readonly IScheduler scheduler = new RoundRobinScheduler();

    public void Start()
    {
        if (worker != null)
        {
            Console.WriteLine("Поток уже был запущен.");
            return;
        }
        worker = new Thread(ThreadLoop) { IsBackground = true };
        worker.Start();
    }

    private void ThreadLoop()
    {
        while (isRunning)
        {
            if (scheduler.HasCommand())
            {
                var command = scheduler.Select();
                try
                {
                    if (command is ISteppableCommand stepCmd)
                    {
                        var finished = stepCmd.ExecuteStep();
                        if (finished)
                            scheduler.Remove(command);
                    }
                    else
                    {
                        command.Execute();
                        scheduler.Remove(command);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Во время выполнения команды возникла ошибка: {e.Message}");
                    scheduler.Remove(command);
                }
            }
            else
            {
                ICommand? command;
                try
                {
                    command = commandsQueue.Take();
                }
                catch (InvalidOperationException)
                {
                    break;
                }

                try
                {
                    if (command is ISteppableCommand stepCmd)
                    {
                        var finished = stepCmd.ExecuteStep();
                        if (!finished)
                            scheduler.Add(command);
                    }
                    else
                    {
                        command.Execute();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Во время выполнения команды возникла ошибка: {e.Message}");
                }
            }

            if (isSoftStop && commandsQueue.Count == 0 && !scheduler.HasCommand())
            {
                isRunning = false;
            }
        }
    }

    public void AddCommands(List<ICommand> commands)
    {
        foreach (var command in commands)
            commandsQueue.Add(command);
    }

    public void Enqueue(ICommand command)
    {
        commandsQueue.Add(command);
    }

    public void RequestHardStop()
    {
        if (Thread.CurrentThread.ManagedThreadId != worker?.ManagedThreadId)
            throw new InvalidOperationException("Операция HardStop доступна только из потока ее выполнения.");
        isRunning = false;
        commandsQueue.CompleteAdding();
    }

    public void RequestSoftStop()
    {
        if (Thread.CurrentThread.ManagedThreadId != worker?.ManagedThreadId)
            throw new InvalidOperationException("Операция SoftStop доступна только из потока ее выполнения.");
        isSoftStop = true;
    }

    public void Join()
    {
        worker?.Join();
    }
}

public class HardStopCommand : ICommand
{
    private ServerThread thisThread;
    public HardStopCommand(ServerThread thread) => thisThread = thread;
    public void Execute() => thisThread.RequestHardStop();
}

public class SoftStopCommand : ICommand
{
    private ServerThread thisThread;
    public SoftStopCommand(ServerThread thread) => thisThread = thread;
    public void Execute() => thisThread.RequestSoftStop();
}

public class TestCommand : ISteppableCommand
{
    private readonly int id;
    private int counter = 0;
    public TestCommand(int id) { this.id = id; }
    public bool ExecuteStep()
    {
        Console.WriteLine($"Поток {id} вызов {++counter}");
        return counter >= 3;
    }
    public void Execute()
    {
        while (!ExecuteStep()) { }
    }
}

class Program
{
    static void Main()
    {
        var server = new ServerThread();
        server.Start();

        for (int i = 1; i <= 5; i++)
            server.Enqueue(new TestCommand(i));

        server.Enqueue(new HardStopCommand(server));
        server.Join();

        Console.WriteLine("Выполнение завершено");
    }
}
