using Game;
using game.Models.Domain.Entities;
using game.Models.Domain.Entities.Items;
using game.Models.Domain.Entities.Player;

namespace game.Views.Rendering;

public class DetailsRenderer
{
    public void Render(UIPanel p, Player player)
    {
        p.WriteTitle("EQUIPPED");

        // Renderujemy sloty w pętli lub dedykowanej metodzie
        RenderSlot(p, "LEFT HAND", player.LeftHandItem);
        p.SkipLine();
        RenderSlot(p, "RIGHT HAND", player.RightHandItem);
    }

    private void RenderSlot(UIPanel p, string label, IItem? item)
    {
        var itemName = item?.Name ?? "Empty";
        p.WriteLine($"{label}: {itemName}");
    }
}