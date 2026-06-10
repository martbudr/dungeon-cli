using game.Server.Models.Core;

namespace game.Server.Models.Systems.WorldGeneration;

public abstract class DungeonStrategy
{
    public abstract void Build(GameState state, DungeonTheme theme);
}