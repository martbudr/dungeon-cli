using game.Client.Models;

namespace game.Shared.DTOs.Patch;

public class JournalStatePatch : IGamePatch
{
    public bool IsOpen { get; set; }

    public void Apply(ClientGameState state)
    {
        state.IsJournalOpen = IsOpen;
    }
}
