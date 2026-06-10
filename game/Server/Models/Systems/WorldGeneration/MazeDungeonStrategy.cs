using game.Server.Models.Core;

namespace game.Server.Models.Systems.WorldGeneration;

class MazeDungeonStrategy : DungeonStrategy
{
    public override void Build(GameState state, DungeonTheme theme)
    {
        var builder = new DungeonBuilder(state);
        builder.FilledDungeon();

        builder.GenerateMaze();
        
        builder.AddThemeItems(theme, 10);
        builder.AddThemeWeapons(theme, 5);
        builder.AddThemeEnemies(theme, 6);
        builder.PlaceArtifact(theme);
    }
}