namespace okai;

public class ConsoleUserInput : IUserInput
{
    public string? ReadLine()
    {
        return Console.ReadLine();
    }
}
