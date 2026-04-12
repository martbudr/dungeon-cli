using Game.Combat;

namespace Game;

public class DetailsRenderer
{
    public void Render(UIPanel p, Player player) // Usunąłem Board, skoro go nie używasz
    {
        p.WriteTitle("EQUIPPED");

        // Renderujemy sloty w pętli lub dedykowanej metodzie
        RenderSlot(p, "LEFT HAND", player.LeftHandItem);
        p.SkipLine();
        RenderSlot(p, "RIGHT HAND", player.RightHandItem);
    }

    private void RenderSlot(UIPanel p, string label, IItem? item)
    {
        string itemName = item?.Name ?? "Empty";
        p.WriteLine($"{label}: {itemName}");
    }
}