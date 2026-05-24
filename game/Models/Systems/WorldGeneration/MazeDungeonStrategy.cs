using game.Models;
using game.Models.Systems.WorldGeneration;

namespace Game;

class MazeDungeonStrategy : DungeonStrategy
{
    public override void Build(GameState state)
    {
        var builder = new DungeonBuilder(state);
        builder.FilledDungeon();

        builder.GenerateMaze();
        
        builder.AddThemeItems(state.Theme, 10);
        builder.AddThemeWeapons(state.Theme, 5);
        builder.AddThemeEnemies(state.Theme, 6);
        builder.PlaceArtifact(state.Theme);
    }
}