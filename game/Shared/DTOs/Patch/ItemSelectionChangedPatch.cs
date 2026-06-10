using game.Client.Models;

namespace game.Shared.DTOs.Patch;

public class ItemSelectionChangedPatch : IGamePatch
{
    public int PlayerId { get; set; }
    public int SelectedIndex { get; set; }

    public void Apply(ClientGameState state)
    {
        if (!state.Players.TryGetValue(PlayerId, out var player))
            return;

        // 2. Jeśli inwentarz istnieje i indeks mieści się w granicach listy...
        if (SelectedIndex < 0) return;

        player.SelectedItem = SelectedIndex;
    }
}