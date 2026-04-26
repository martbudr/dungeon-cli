namespace Game;

class StandardDungeonStrategy : DungeonStrategy
{
    public override void Build(GameContext context, DungeonTheme theme)
    {
        var builder = new DungeonBuilder(context);
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