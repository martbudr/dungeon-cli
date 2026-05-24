namespace game.Views.Rendering;

public class UIPanel
{
    private readonly int startX;
    private int currentY;
    private readonly int width;

    public UIPanel(int x, int y, int width)
    {
        startX = x;
        currentY = y;
        this.width = width;
    }

    public void WriteTitle(string text)
    {
        WriteLine($"=== {text} ===", ConsoleColor.Magenta);
    }

    public void WriteLine(string text, ConsoleColor color = ConsoleColor.Gray)
    {
        Console.SetCursorPosition(startX, currentY++);
        Console.ForegroundColor = color;
        Console.Write(text.PadRight(width));
        Console.ResetColor();
    }

    public void SkipLine() => currentY++;

    public void ClearLine()
    {
        if (currentY >= 0 && currentY < Console.BufferHeight)
        {
            Console.SetCursorPosition(startX, currentY);
            Console.Write("".PadRight(width));
            currentY++;
        }
    }
}