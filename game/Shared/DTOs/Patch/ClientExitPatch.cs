using game.Client.Models;

namespace game.Shared.DTOs.Patch;

public class ClientExitPatch : IGamePatch
{
    public void Apply(ClientGameState state)
    {
        state.ShouldExit = true;
    }
}