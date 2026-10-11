using Game.Effects;
using game.Server.Models.Entities.Items;

namespace game.Server.Models.Systems.Equipment.Effects;

public class LuckyEffect : ItemEffectDecorator, IStatModifier
{
    private readonly int luckBonus;

    public LuckyEffect(IItem item, int luckBonus = 2) : base(item)
    {
        this.luckBonus = luckBonus;
    }

    public override string Name => $"{base.Name} (Lucky)";

    // --- IStatModifier ---
    public int Strength => 0;
    public int Dexterity => 0;
    public int Luck => luckBonus;
    public int Aggression => 0;
    public int Wisdom => 0;

}
