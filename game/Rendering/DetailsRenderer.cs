namespace Game;

public class DetailsRenderer
{
    public void Render(UIPanel p, Board board, Player player)
    {
        p.WriteTitle("EQUIPPED");

        var left = player.LeftHandItem;
        var right = player.RightHandItem;

        p.WriteLine($"LEFT HAND: {(left?.Name ?? "Empty")}");
        p.SkipLine();
        p.WriteLine($"RIGHT HAND: {(right?.Name ?? "Empty")}");
    }
}