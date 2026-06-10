using game.Models.Common.Events;
using game.Server.Models.Systems.WorldGeneration;
using game.Server.Services;
using game.Shared.DTOs.Patch;
using game.Shared.Events.Dispatchers;

namespace game.Server.Models.Core;

public class GameContext
{
    public GameState State { get; }
    public List<IGamePatch> Patches { get; set; }

    public GameSystems Systems { get; }

    public UiEventDispatcher UiDispatcher { get; }

    public GameplayEventDispatcher GameplayDispatcher { get; }

    public IMessageService Logger { get; }
    
    public DungeonTheme Theme { get; }
    
    public GameContext(
        GameState state,
        GameSystems systems,
        IMessageService logger,
        UiEventDispatcher uiDispatcher,
        GameplayEventDispatcher gameplayDispatcher,
        DungeonTheme theme)
    {
        State = state;
        Patches = [];
        Systems = systems;
        Logger = logger;
        UiDispatcher = uiDispatcher;
        GameplayDispatcher = gameplayDispatcher;
        Theme = theme;
    }
}