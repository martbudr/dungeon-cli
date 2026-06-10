namespace game.Server.Models.Entities.Items;

public static class ItemIdGenerator
{
    private static int _currentId = 0;

    public static int Next() => Interlocked.Increment(ref _currentId);
}