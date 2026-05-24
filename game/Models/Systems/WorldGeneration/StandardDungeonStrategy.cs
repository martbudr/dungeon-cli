using game.Models;
using game.Models.Systems.WorldGeneration;

namespace Game;

class StandardDungeonStrategy : DungeonStrategy
{
    public override void Build(GameState state)
    {
        var builder = new DungeonBuilder(state);
        builder.FilledDungeon();
        
        builder.AddCentralHall(7, 12);
        builder.AddIrregularHalls(4, 6, 10);
        builder.DigTunnels(10, 20); // Łączniki

        builder.AddThemeItems(state.Theme, 8);
        builder.AddThemeWeapons(state.Theme, 12); 
        builder.AddThemeEnemies(state.Theme, 8);
        builder.PlaceArtifact(state.Theme);
    }
}