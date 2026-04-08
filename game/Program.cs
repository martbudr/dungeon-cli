using System;
using System.Collections.Generic;

namespace Game;

public static class GlobalConstants
{
    public const int HEIGHT = 21;
    public const int WIDTH = 41;

    public const char WALL = '█';
    public const char PLAYER = '¶';
    public const char EMPTY = ' ';
    
    public const char ENEMY_SYMBOL = '!';
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