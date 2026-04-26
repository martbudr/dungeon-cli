namespace game.Domain.Events;

public class NoiseEvent : IGameEvent
{
    public int SourceX { get; }
    public int SourceY { get; }
    public int Range { get; }
    public Dictionary<(int x, int y), int> ReachedTiles { get; }

    public NoiseEvent(int x, int y, int range, Dictionary<(int, int), int> reachedTiles)
    {
        SourceX = x;
        SourceY = y;
        Range = range;
        ReachedTiles = reachedTiles;
    }
}