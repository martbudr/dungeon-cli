using game.Client.Models;

namespace game.Shared.DTOs.Patch;

public class JournalStatePatch : IGamePatch
{
    public bool IsOpen { get; set; }

    public void Apply(ClientGameState state)
    {
        // Klient ustawia swoją lokalną flagę, co automatycznie 
        // wymusi na GameRenderer narysowanie (lub schowanie) okna dziennika
        state.IsJournalOpen = IsOpen; 
    }
}