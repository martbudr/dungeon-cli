using game.Client.Models;

namespace game.Shared.DTOs.Patch;

public class ClientExitPatch : IGamePatch
{
    public int PlayerId { get; set; }

    public void Apply(ClientGameState state)
    {
        if (state.LocalPlayerId == PlayerId)
            state.ShouldExit = true;
    }
}
