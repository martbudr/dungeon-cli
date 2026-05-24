using Game;

namespace game.Models.Systems.WorldGeneration;

class DenseDungeonStrategy : DungeonStrategy
{
    public override void Build(GameState state)
    {
        var builder = new DungeonBuilder(state);
        builder.FilledDungeon();

        builder.DigTunnels(25, 40); // Gęsta sieć korytarzy
        builder.AddRooms(10, 2); // Małe wyrobiska

        builder.AddThemeItems(state.Theme, 20);
        builder.AddThemeWeapons(state.Theme, 5);
        builder.AddThemeEnemies(state.Theme, 6);
        builder.PlaceArtifact(state.Theme);
    }
}