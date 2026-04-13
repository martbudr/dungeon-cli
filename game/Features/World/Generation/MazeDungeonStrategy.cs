namespace Game;

class MazeDungeonStrategy : DungeonStrategy
{
    public override void Build(Board board, DungeonTheme theme)
    {
        var builder = new DungeonBuilder(board);
        builder.FilledDungeon();

        builder.GenerateMaze();
        
        builder.AddThemeItems(theme, 10);
        builder.AddThemeWeapons(theme, 5);
        builder.AddThemeEnemies(theme, 6);
        builder.PlaceArtifact(theme);
    }
}