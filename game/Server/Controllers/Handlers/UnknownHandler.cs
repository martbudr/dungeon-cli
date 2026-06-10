using game.Server.Bridges;
using game.Shared.DTOs.Patch;
using game.Shared.Events.Dispatchers;
using game.Shared.Events.UI;

namespace game.Server.Controllers.Handlers;
class UnknownHandler : InputHandler
{
    private readonly UiEventDispatcher uiDispatcher;

    public UnknownHandler(UiEventDispatcher uiDispatcher)
    {
        this.uiDispatcher = uiDispatcher;
    }

    public override List<IGamePatch> Handle(ConsoleKey key)
    {
        // Jeśli doszliśmy tutaj, oznacza to, że żaden wcześniejszy handler 
        // nie rozpoznał tego klawisza
        var patches = new List<IGamePatch>();
        
        var newPatches = UiEventBridge.PublishAndPatch(new UnknownKeyEvent(key), uiDispatcher);
        patches.AddRange(newPatches);
        
        patches.AddRange(base.Handle(key));
        return patches;
    }
}