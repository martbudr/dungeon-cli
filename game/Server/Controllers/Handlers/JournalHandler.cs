using game.Server.Bridges;
using game.Server.Models.Entities.Player;
using game.Shared.DTOs.Patch;
using game.Shared.Events.Dispatchers;
using game.Shared.Events.UI;

namespace game.Server.Controllers.Handlers;

class JournalHandler : InputHandler
{
    private readonly Player player;
    private readonly UiEventDispatcher uiDispatcher;

    public JournalHandler(Player player, UiEventDispatcher uiDispatcher)
    {
        this.player = player;
        this.uiDispatcher = uiDispatcher;
    }

    public override List<IGamePatch> Handle(ConsoleKey key)
    {
        var patches = new List<IGamePatch>();

        if (player.IsJournalOpen)
        {
            player.IsJournalOpen = false;
            
            var newPatches = UiEventBridge.PublishAndPatch(new JournalClosedEvent(), uiDispatcher);
            patches.AddRange(newPatches);
            
            patches.Add(new JournalStatePatch { IsOpen = false });
            
            return patches;
        }
        
        if (key == ConsoleKey.J)
        {
            player.IsJournalOpen = true;
            
            var newPatches = UiEventBridge.PublishAndPatch(new JournalOpenedEvent(), uiDispatcher);
            patches.AddRange(newPatches);
            
            patches.Add(new JournalStatePatch { IsOpen = true });
            
            return patches; 
        }

        patches.AddRange(base.Handle(key));
        return patches;
    }
}