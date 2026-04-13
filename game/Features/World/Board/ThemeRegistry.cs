using Game.EquipStrategies;

namespace Game;

public static class ThemeRegistry
{
    public static DungeonTheme GetEarthTheme() => new DungeonTheme
    {
        Name = "Earthen Depths",
        WelcomeMessage = "The air is heavy, and the deep earth groans around you...",
        Style = new DenseDungeonStrategy(),
        
        // UNIKALNY ARTEFAKT ZIEMI
        ArtifactFactory = () => new Weapon("Earthshaker", 'M', new HeavyWeaponType(), new TwoHandedEquipStrategy()),

        // SPECYFICZNE BRONIE ZIEMI
        WeaponFactories = new List<Func<Weapon>> {
            () => new Weapon("Stone Hatchet", 'h', new HeavyWeaponType(), new OneHandedEquipStrategy()),
            () => new Weapon("Granite Hammer", 'm', new HeavyWeaponType(), new TwoHandedEquipStrategy()),
            () => new Weapon("Claymore of Dust", 'c', new HeavyWeaponType(), new TwoHandedEquipStrategy())
        },

        EnemyFactories = new List<Func<Enemy>> {
            () => new Enemy("Stone Golem", 40, 10, 8),
            () => new Enemy("Cave Crawler", 15, 12, 1)
        },

        ItemPool = new List<Func<IItem>> {
            () => new SimpleItem("Heavy Pebble", 'o'),
            () => new SimpleItem("Iron Ore", 'i')
        }
    };

    public static DungeonTheme GetAirTheme() => new DungeonTheme
    {
        Name = "Howling Labyrinth",
        WelcomeMessage = "A sharp breeze whistles through the winding passages...",
        Style = new MazeDungeonStrategy(),

        // UNIKALNY ARTEFAKT POWIETRZA
        ArtifactFactory = () => new Weapon("Sky-Piercer", 'L', new LightWeaponType(), new OneHandedEquipStrategy()),

        // SPECYFICZNE BRONIE POWIETRZA
        WeaponFactories = new List<Func<Weapon>> {
            () => new Weapon("Wind-Razor", 'r', new LightWeaponType(), new OneHandedEquipStrategy()),
            () => new Weapon("Feathered Dagger", 'd', new LightWeaponType(), new OneHandedEquipStrategy()),
            () => new Weapon("Whistling Spear", 's', new LightWeaponType(), new OneHandedEquipStrategy())
        },

        EnemyFactories = new List<Func<Enemy>> {
            () => new Enemy("Wind Wraith", 20, 15, 0),
            () => new Enemy("Storm Harpy", 25, 10, 2)
        },

        ItemPool = new List<Func<IItem>> {
            () => new SimpleItem("Feather", 'f'),
            () => new SimpleItem("Cloud Essence", 'e')
        }
    };

    public static DungeonTheme GetFireTheme() => new DungeonTheme
    {
        Name = "Inferno Forge",
        WelcomeMessage = "Sweat stings your eyes. The walls are warm to the touch.",
        Style = new StandardDungeonStrategy(),

        // UNIKALNY ARTEFAKT OGNIA
        ArtifactFactory = () => new Weapon("Phoenix Blade", 'P', new LightWeaponType(), new OneHandedEquipStrategy()),

        // SPECYFICZNE BRONIE OGNIA
        WeaponFactories = new List<Func<Weapon>> {
            () => new Weapon("Searing Dagger", 'd', new LightWeaponType(), new OneHandedEquipStrategy()),
            () => new Weapon("Obsidian Maul", 'm', new HeavyWeaponType(), new TwoHandedEquipStrategy()),
            () => new Weapon("Flame Whip", 'w', new LightWeaponType(), new OneHandedEquipStrategy())
        },

        EnemyFactories = new List<Func<Enemy>> {
            () => new Enemy("Flame Skull", 15, 20, 0),
            () => new Enemy("Magma Slime", 30, 8, 5)
        },

        ItemPool = new List<Func<IItem>> {
            () => new SimpleItem("Coal", 'c'),
            () => new SimpleItem("Fire Crystal", 'x')
        }
    };

    // Metoda pomocnicza do losowania motywu
    public static DungeonTheme GetRandomTheme()
    {
        var rng = new Random();
        return rng.Next(3) switch
        {
            0 => GetEarthTheme(),
            1 => GetAirTheme(),
            2 => GetFireTheme(),
            _ => GetEarthTheme()
        };
    }
}