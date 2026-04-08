namespace Game;

public class UIPanel
{
    private readonly int _startX;
    private int _currentY;
    private readonly int _width;

    public UIPanel(int x, int y, int width)
    {
        _startX = x;
        _currentY = y;
        _width = width;
    }

    public void WriteTitle(string text)
    {
        WriteLine($"=== {text} ===", ConsoleColor.Magenta);
    }

    public void WriteLine(string text, ConsoleColor color = ConsoleColor.Gray)
    {
        Console.SetCursorPosition(_startX, _currentY++);
        Console.ForegroundColor = color;
        Console.Write(text.PadRight(_width));
        Console.ResetColor();
    }

    public void SkipLine() => _currentY++;

    public void ClearLine()
    {
        if (_currentY >= 0 && _currentY < Console.BufferHeight)
        {
            Console.SetCursorPosition(_startX, _currentY);
            Console.Write("".PadRight(_width));
            _currentY++;
        }
    }
}