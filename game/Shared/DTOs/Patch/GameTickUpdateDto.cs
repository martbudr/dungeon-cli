namespace game.Shared.DTOs.Patch;

public class GameTickUpdateDto
{
    // System.Text.Json automatycznie rozpozna podtypy
    public List<IGamePatch> Patches { get; set; } = [];
}