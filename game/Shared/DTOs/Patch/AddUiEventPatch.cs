using game.Client.Models;
using game.Models.Common.Events;
using game.Shared.Events.Core;
using game.Shared.Events.UI;

namespace game.Shared.DTOs.Patch;

public class AddUiEventPatch : IGamePatch
{
    public required IUiEvent UiEvent { get; set; }

    public void Apply(ClientGameState state)
    {
        try
        {
            state.UiDispatcher.Publish(UiEvent);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message); 
            Console.ReadKey();
        }
    }
}
