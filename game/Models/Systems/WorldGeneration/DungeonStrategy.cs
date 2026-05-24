using Game;

namespace game.Models.Systems.WorldGeneration;

public abstract class DungeonStrategy
{
    public abstract void Build(GameState state);
}