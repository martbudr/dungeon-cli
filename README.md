# Dungeon Crawler

Dungeon Crawler is a console-based multiplayer dungeon game written in C# and .NET. The project runs as either a TCP game server or a console client. The server owns the authoritative game state, while each client renders a local view of the game and sends keyboard input back to the server.

The codebase is structured around an MVC-style split and uses several object-oriented design patterns to keep game logic, networking, rendering, and input handling separated.

## Game Objective

The game currently has no defined objective or win condition. Players can explore the dungeon, collect items and currency, equip weapons, and fight enemies, but there is no final boss, completion screen, or victory trigger. Play continues until the player dies, exits, or the server stops.

## Requirements

- .NET 9 SDK (the `global.json` policy allows newer .NET 9 SDK feature bands and minor versions, but does not roll forward to .NET 10)
- A terminal/console that supports interactive keyboard input
- Network access between the server and client processes when running on different machines

The project targets `net9.0`.

## Gameplay Screenshot (client)

![Client](screenshot.png)

## Build

Restore and build the solution from the repository root:

```bash
dotnet build game.sln
```

## Running

The application has two modes: server and client. Start the server first, then connect clients in separate terminals. The server accepts up to nine simultaneous TCP connections; additional connections are closed.

### Start a server

Default port:

```bash
dotnet run --project game/game.csproj -- --server
```

Custom port:

```bash
dotnet run --project game/game.csproj -- --server 6000
```

### Start a client

Connect to a local server:

```bash
dotnet run --project game/game.csproj -- --client 127.0.0.1:5555
```

Connect to a server on another machine:

```bash
dotnet run --project game/game.csproj -- --client SERVER_IP:5555
```

If no port is provided in client mode, the client defaults to port `5555`.

For remote play, allow incoming TCP traffic on the chosen server port and use the same port in the client command. Server and clients should run the same version of the project.

### Stopping the game

Press `X` in a client to leave. Stop the server with `Ctrl+C` in its terminal. There is currently no server-side quit command or custom `Ctrl+C` handler, so this terminates the process rather than guaranteeing a graceful shutdown notification. Clients detect the closed connection and exit. Restarting the server generates a new dungeon.

## Configuration

The entry point reads `config.json` from the process working directory, falling back to the copy beside the executable. The repository configuration is copied during build and publish. If the file is missing or invalid, default values are used.

Available configuration values are defined in `game/Shared/GameConfig.cs`:

```json
{
  "PlayerName": "Hero",
  "LogDirectory": "./Logs"
}
```

The repository currently includes an empty `config.json`, so the game uses defaults unless values are added.

`PlayerName` labels the log files; in-game names are assigned by the server as `Player 1`, `Player 2`, and so on.

## Controls

- `W`, `A`, `S`, `D`: move
- `E`: pick up an item
- `Up Arrow`, `Down Arrow`: select an inventory item
- `1`: equip selected item in the left hand
- `2`: equip selected item in the right hand
- `3`: unequip left hand
- `4`: unequip right hand
- `G`: drop selected item
- `Space`: open attack selection
- `N`: normal attack after pressing `Space`
- `B`: sneak/backstab attack after pressing `Space`
- `M`: magic attack after pressing `Space`
- `J`: open journal
- `X`: exit the client

Successful movement, pickup, equipment changes, drops, and attacks advance enemy movement. Opening the attack menu, viewing the journal, selecting inventory items, and failed actions do not consume a turn. While the journal is open, any key closes it; `X` exits instead.

## Combat

Move onto the same tile as an enemy, then press `Space` and choose `N`, `B`, or `M`. The player attacks first; if the enemy survives, it immediately retaliates. At zero health the client displays game over and exits. There is no in-session respawn.

| Attack | Light weapons | Heavy weapons | Magic weapons |
| --- | --- | --- | --- |
| Normal (`N`) | Damage uses Dexterity + Luck | Damage uses Strength + Aggression | 1 base damage |
| Sneak (`B`) | Twice normal damage, with lower defense | Half normal damage, with lower defense | 1 base damage, no weapon defense |
| Magic (`M`) | 1 base damage | 1 base damage | Damage uses Wisdom; weapon defense is twice Wisdom |

Damage from two distinct equipped items is added; a two-handed item is counted once. Empty hands and ordinary items using `NoWeaponType` contribute zero base damage. Strong adds damage, and Lucky increases the Luck stat used by applicable attacks and defenses. Magic weapon behavior exists in code, but the current themes do not generate magic weapons.

Enemy armor reduces incoming damage, with a minimum of zero. Enemy retaliation varies by up to 2 points around its attack value, then is reduced by the player's Dexterity plus equipment defense for the selected attack. Enemy species affect ally-death reactions, as described below; they do not provide special attack resistances or weaknesses.

## Map-Symbol Legend

| Symbol | Meaning | Color |
| --- | --- | --- |
| `█` | Wall; blocks movement | Terminal default |
| Blank space | Empty floor | Terminal default |
| Player ID (e.g. `1`, `2`) | Player position | Dark magenta for you; white for other players |
| `!` | Living enemy | Red |
| Item letter | An item on the floor; see theme-specific symbols below | Cyan |

Item symbols depend on the dungeon theme:

| Theme | Weapons | Artifact | Other items |
| --- | --- | --- | --- |
| Earthen Depths | `h`: Stone Hatchet; `m`: Granite Hammer; `c`: Claymore of Dust | `M`: Earthshaker | `o`: Heavy Pebble; `i`: Iron Ore |
| Howling Labyrinth | `r`: Wind-Razor; `d`: Feathered Dagger; `s`: Whistling Spear | `L`: Sky-Piercer | `f`: Feather; `e`: Cloud Essence |
| Inferno Forge | `d`: Searing Dagger; `m`: Obsidian Maul; `w`: Flame Whip | `P`: Phoenix Blade | `c`: Coal; `x`: Fire Crystal |

Rendering priority is player, then living enemy, then item, then terrain. If several items occupy one tile, only the first item's symbol is shown. Item effects such as Lucky and Strong preserve the original item's symbol.

## Dungeon Themes

The server randomly selects one of three currently implemented themes when it starts. These are examples of the theme system, rather than a fixed limit: adding another theme is straightforward when reusing existing generation strategies, weapon types, and enemy behaviors.

### Earthen Depths

A dense network of tunnels and small rooms, with heavy equipment and stone-themed loot. Stone Hatchet is a one-handed heavy weapon; Granite Hammer and Claymore of Dust are two-handed heavy weapons. The artifact, Earthshaker, is also a two-handed heavy weapon. Heavy Pebble and Iron Ore are ordinary `SimpleItem` objects, using the default one-handed equipment strategy and `NoWeaponType` combat behavior.

### Howling Labyrinth

A maze-style dungeon built around light, one-handed weapons: Wind-Razor, Feathered Dagger, and Whistling Spear. Its artifact, Sky-Piercer, uses the same light, one-handed configuration. Feather and Cloud Essence are ordinary `SimpleItem` objects with default one-handed equipment and `NoWeaponType` combat behavior.

### Inferno Forge

A dungeon with a central hall, irregular rooms, and connecting tunnels. Searing Dagger and Flame Whip are light, one-handed weapons; Obsidian Maul is a heavy, two-handed weapon. The artifact, Phoenix Blade, is light and one-handed. Coal and Fire Crystal are ordinary `SimpleItem` objects with default one-handed equipment and `NoWeaponType` combat behavior.

### Enemy Types

Each theme supplies named enemy variants with different health, attack, and armor values. Their names describe the theme; their underlying species determines how they react when another enemy of the same species dies.

| Theme | Enemy | Combat profile | Underlying type |
| --- | --- | --- | --- |
| Earthen Depths | Stone Golem | High health and armor | Skeleton |
| Earthen Depths | Earth Goblin | Low health, attack, and armor | Goblin |
| Howling Labyrinth | Wind Wraith | Strong attack, no armor | Skeleton |
| Howling Labyrinth | Storm Harpy | Moderate health and attack, light armor | Goblin |
| Inferno Forge | Flame Skull | Highest base attack, low health, no armor | Skeleton |
| Inferno Forge | Magma Slime | High health and armor, lower attack | Goblin |

Skeleton-type enemies gain 2 attack when an ally of the same species dies. Goblin-type enemies lose 1 attack and 1 armor, with each value bounded below by 1 and 0 respectively.

### Adding a Theme

Define a new `DungeonTheme` in `ThemeRegistry`, providing its name, welcome message, generation `Style`, weapon and enemy factories, ordinary item pool, and optional artifact factory. Then include it in `GetRandomTheme()` and update the random selection range. Existing strategies and behaviors can be reused; new layout or combat behavior only needs a new strategy implementation when the existing options do not fit. Themes are currently defined in C# rather than loaded from `config.json`.

## Project Structure

```text
game.sln                 Solution
global.json              .NET SDK selection
config.json              Player/logging configuration
game/
  game.csproj            Shared server/client executable
  Client/                Local models, networking, console views
  Server/                Bootstrap, commands, controllers, domain logic, services
  Shared/                DTOs, patches, events, enums, network contracts
```

## Architecture

### MVC Model

The project follows an MVC-inspired architecture:

- Model: game state and domain rules live mostly under `game/Server/Models`, `game/Client/Models`, and `game/Shared/DTOs`. Examples include `GameState`, `Board`, `Player`, `Enemy`, equipment, combat rules, dungeon generation, and client-side snapshot models.
- View: console rendering lives under `game/Client/Views`. Renderers such as `GameRenderer`, `BoardRenderer`, `InventoryRenderer`, `StatsRenderer`, `JournalRenderer`, and `LogView` are responsible for presenting the current client model.
- Controller: input handling and server orchestration live under `game/Server/Controllers` and `game/Server/Networking`. `ServerInputController` converts player key input into game actions, while `GameServer` processes queued commands and publishes state patches.

### Client/Server Flow

1. The server starts a `ServerNetworkSession` and waits for TCP clients.
2. A client connects through `ClientNetworkSession`.
3. The server sends an initial snapshot and player assignment.
4. The client renders the received snapshot.
5. Keyboard input is sent as `PlayerInputDto`.
6. The server processes input through commands, handlers, systems, and rules.
7. Resulting `IGamePatch` objects are broadcast to clients.
8. Each client applies patches locally and redraws.

## Object-Oriented Patterns

The codebase uses several OOP patterns directly:

- Chain of Responsibility: `ServerInputController` builds a chain of `InputHandler` implementations, including `ExitHandler`, `JournalHandler`, `MovementHandler`, `PickupHandler`, `EquipmentHandler`, `DropHandler`, `SelectionHandler`, `AttackHandler`, and `UnknownHandler`. Each handler either processes a key or passes it to the next handler.
- Strategy: dungeon generation uses `DungeonStrategy` with implementations such as `StandardDungeonStrategy`, `DenseDungeonStrategy`, and `MazeDungeonStrategy`. Equipment behavior uses `IEquipStrategy` with one-handed and two-handed strategies. Combat actions use `IAttackHandler` with `NormalAttack`, `SneakAttack`, and `MagicAttack`.
- Decorator: item effects are layered through `ItemEffectDecorator`, with concrete effects such as `LuckyEffect` and `StrongEffect` wrapping an item while preserving the item/equipment interface.
- Observer/Event Dispatcher: `UiEventDispatcher` and `GameplayEventDispatcher` publish events to subscribed listeners. This decouples game systems from rendering and UI feedback.
- Command: network actions are represented as command objects such as `PlayerJoinCommand`, `PlayerInputCommand`, and `PlayerLeaveCommand`, which are queued by networking and consumed by the game server loop.
- Builder: dungeon creation is coordinated through `DungeonBuilder`, which exposes reusable generation steps used by different dungeon strategies.
- Adapter: the event dispatchers use typed listener adapters internally so strongly typed listeners can be stored and invoked through a common dispatcher interface.

Shared DTOs and `IGamePatch` implementations additionally carry snapshots and incremental updates between server and clients.

## Known Limitations

- Game state is held in memory; there is no save/load or session recovery after a server restart.
- A disconnected player must start a new client session; automatic reconnection is not implemented.
- Networking uses plain TCP without authentication or encryption.
- Rendering requires an interactive terminal; a narrow window can wrap the board and panels. Enlarge the terminal if the layout is broken.

## Development Notes

For manual verification, start one server and two clients. Check movement visibility, inventory and equipment changes (including Lucky stat bonuses), combat, journal controls, client exit, and what happens when the server stops.
