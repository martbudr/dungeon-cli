using game.Domain.Entities;

namespace Game;

public class DungeonTheme
{
    public string Name { get; set; }
    public string WelcomeMessage { get; set; }
    public DungeonStrategy Style { get; set; } // Tu przypisujesz np. MazeDungeonStrategy
    
    // Dane dla Buildera
    public List<Func<Weapon>> WeaponFactories { get; set; }
    public List<Func<GameContext, Enemy>> EnemyFactories { get; set; }
    public List<Func<IItem>> ItemPool { get; set; }
    public Func<IItem>? ArtifactFactory { get; set; } // only one artifact
}