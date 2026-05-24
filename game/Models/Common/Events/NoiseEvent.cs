namespace game.Models.Common.Events;

public class NoiseEvent : IGameEvent
{
    public int SourceX { get; }
    public int SourceY { get; }
    public Dictionary<(int x, int y), int> ReachedTiles { get; }

    public NoiseEvent(int x, int y, Dictionary<(int, int), int> reachedTiles)
    {
        SourceX = x;
        SourceY = y;
        ReachedTiles = reachedTiles;
    }
}