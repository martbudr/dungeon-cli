using game.Client.Models;

namespace game.Shared.DTOs.Patch;

public class ServerShutdownPatch : IGamePatch
{
    public void Apply(ClientGameState state)
    {
        // Klient przerywa swoją pętlę i czysto się zamyka
        state.ShouldExit = true;
    }
}