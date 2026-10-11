using game.Shared.DTOs.Patch;

namespace game.Server.Models.Systems.Game;

public class PlayerActionResult
{
    public List<IGamePatch> SharedPatches { get; } = [];
    public List<IGamePatch> PrivatePatches { get; } = [];

    public void AddRange(IEnumerable<IGamePatch> patches)
    {
        foreach (var patch in patches)
        {
            if (patch is AddUiEventPatch or JournalStatePatch or ClientExitPatch)
                PrivatePatches.Add(patch);
            else
                SharedPatches.Add(patch);
        }
    }
}
