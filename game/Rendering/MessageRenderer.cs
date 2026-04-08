namespace Game;

public class MessageRenderer
{
    public void Render(string message, int y)
    {
        Console.SetCursorPosition(0, y);
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("MSG: " + message.PadRight(Console.WindowWidth - 6));
        Console.ResetColor();
    }
}