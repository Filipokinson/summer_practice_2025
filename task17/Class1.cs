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

public class LongCommand : ISteppableCommand
{
    private int counter = 0;
    private readonly int steps;
    public bool IsFinished => counter >= steps;

    public LongCommand(int steps)
    {
        this.steps = steps;
    }
    public bool ExecuteStep()
    {
        counter++;
        Console.WriteLine($"LongCommand step {counter}");
        return counter >= steps;
    }
    public void Execute()
    {
        while (!ExecuteStep()) { }
    }
}

public class QuickCommand : ICommand
{
    public bool Done { get; private set; }
    public void Execute()
    {
        Done = true;
    }
}
