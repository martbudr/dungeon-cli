namespace Game;

class DenseDungeonStrategy : DungeonStrategy
{
    public override void Build(GameContext context, DungeonTheme theme)
    {
        var builder = new DungeonBuilder(context);
        builder.FilledDungeon();

        builder.DigTunnels(25, 40); // Gęsta sieć korytarzy
        builder.AddRooms(10, 2); // Małe wyrobiska

        builder.AddThemeItems(theme, 20);
        builder.AddThemeWeapons(theme, 5);
        builder.AddThemeEnemies(theme, 6);
        builder.PlaceArtifact(theme);
    }
}