namespace game.Client.Views.Rendering;

public class UIPanel
{
    private readonly int startX;
    private readonly int startY;
    private int currentY;
    private readonly int width;
    public int Width => width;

    public UIPanel(int x, int y, int width)
    {
        startX = x;
        startY = y;
        currentY = y;
        this.width = width;
    }
    
    public void ClearLine()
    {
        if (currentY >= 0 && currentY < Console.BufferHeight)
        {
            Console.SetCursorPosition(startX, currentY);
            Console.Write("".PadRight(width));
            currentY++;
        }
    }

    public void WriteTitle(string text)
    {
        WriteLine($"=== {text} ===", ConsoleColor.Magenta);
    }

    public void WriteLine(string text, ConsoleColor color = ConsoleColor.Gray)
    {
        Console.ForegroundColor = color;

        for (int i = 0; i < text.Length; i += width)
        {
            string line = text.Substring(i, Math.Min(width, text.Length - i));

            Console.SetCursorPosition(startX, currentY++);
            Console.Write(line.PadRight(width));
        }

        if (text.Length == 0)
        {
            Console.SetCursorPosition(startX, currentY++);
            Console.Write(new string(' ', width));
        }

        Console.ResetColor();
    }

    public void SkipLine() => currentY++;
}