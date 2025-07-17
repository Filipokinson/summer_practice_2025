using System.Collections.Concurrent;

public interface ICommand
{
    void Execute();
}

public class ServerThread
{
    private Thread? worker;
    private volatile bool isRunning = true;
    private volatile bool isSoftStop = false;
    private BlockingCollection<ICommand> commandsQueue = new BlockingCollection<ICommand>();

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
        try
        {
            while (isRunning)
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
                    command.Execute();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Во время выполнения команды возникла ошибка: {e.Message}");
                }

                if (isSoftStop && commandsQueue.Count == 0)
                {
                    isRunning = false;
                }
            }
        }
        finally
        {
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
