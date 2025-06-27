public interface ISpaceship
{
    void MoveForward();
    void Rotate(int angle);
    void Fire();
    int Speed { get; }
    int FirePower { get; }
}

public class Cruiser : ISpaceship
{
    public int Speed { get; } = 50;
    public int FirePower { get; } = 100;
    public void MoveForward()
    {
        Console.WriteLine("Крейсер переместился вперед");
    }
    public void Rotate(int angle)
    {
        Console.WriteLine($"Крейсер повернулся на {angle} градусов");
    }
    public void Fire()
    {
        Console.WriteLine("Крейсер выстрелил фотонной ракетой");
    }
}

public class Fighter : ISpaceship
{
    public int Speed { get; } = 100;
    public int FirePower { get; } = 50;
    public void MoveForward()
    {
        Console.WriteLine("Истребитель переместился вперед");
    }
    public void Rotate(int angle)
    {
        Console.WriteLine($"Истребитель повернулся на {angle} градусов");
    }
    public void Fire()
    {
        Console.WriteLine("Истребитель выстрелил фотонной ракетой");
    }
}
