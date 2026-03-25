using System;
using System.Collections.Generic;

namespace Game;

public static class GlobalConstants
{
    public const int HEIGHT = 20;
    public const int WIDTH = 40;

    public const char WALL = '█';
    public const char PLAYER = '¶';
    public const char EMPTY = ' ';
} 

class Program
{
    static void Main()
    {
        Console.Clear();
        Game game = new();
        game.Run();
    }
}