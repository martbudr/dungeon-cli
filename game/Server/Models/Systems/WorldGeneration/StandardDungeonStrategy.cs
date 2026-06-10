using game.Server.Models.Core;

namespace game.Server.Models.Systems.WorldGeneration;

class StandardDungeonStrategy : DungeonStrategy
{
    public override void Build(GameState state, DungeonTheme theme)
    {
        var builder = new DungeonBuilder(state);
        builder.FilledDungeon();
        
        builder.AddCentralHall(7, 12);
        builder.AddIrregularHalls(4, 6, 10);
        builder.DigTunnels(10, 20); // Łączniki

        builder.AddThemeItems(theme, 8);
        builder.AddThemeWeapons(theme, 12); 
        builder.AddThemeEnemies(theme, 8);
        builder.PlaceArtifact(theme);
    }
}