using game.Shared.DTOs.Patch;
using game.Shared.Events.Core;
using game.Shared.Events.Dispatchers;

namespace game.Server.Bridges;

public static class UiEventBridge
{
    public static List<IGamePatch> PublishAndPatch(
        IUiEvent uiEvent,
        UiEventDispatcher dispatcher)
    {
        dispatcher.Publish(uiEvent);

        return
        [
            new AddUiEventPatch
            {
                UiEvent = uiEvent
            }
        ];
    }
}