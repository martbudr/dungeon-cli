using game.Client.Models;

namespace game.Client.Views.Rendering;

public class DetailsRenderer
{
    public void Render(UIPanel p, Player player)
    {
        p.WriteTitle("EQUIPPED");

        // Renderujemy sloty w pętli lub dedykowanej metodzie
        RenderSlot(p, "LEFT HAND", player.Equipment.LeftHandItem?.Name ?? string.Empty);
        p.SkipLine();
        RenderSlot(p, "RIGHT HAND", player.Equipment.RightHandItem?.Name ?? string.Empty);
    }

    private void RenderSlot(UIPanel p, string label, string? item)
    {
        var itemName = item ?? "Empty";
        p.WriteLine($"{label}: {itemName}");
    }
}