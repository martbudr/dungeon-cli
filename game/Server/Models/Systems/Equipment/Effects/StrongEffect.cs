using Game.Effects;
using game.Models.Domain.Rules.Combat;
using game.Server.Models.Entities.Items;
using game.Server.Models.Entities.Player;
using game.Server.Models.Rules.Combat;

namespace game.Server.Models.Systems.Equipment.Effects;

public class StrongEffect : ItemEffectDecorator, IStatModifier
{
    private readonly int extraDamage;

    public StrongEffect(IItem item, int extraDamage = 3) : base(item)
    {
        this.extraDamage = extraDamage; 
    }

    public override string Name => $"{base.Name} (Strong)";

    // Damage is added in Calculate rather than through the player's strength stat.
    public int Strength => 0;
    public int Dexterity => 0;
    public int Luck => 0;
    public int Aggression => 0;
    public int Wisdom => 0;

    public override AttackResult Calculate(Player player, IAttackHandler attack)
    {
        var baseResult = base.Calculate(player, attack);

        return new AttackResult 
        { 
            Damage = baseResult.Damage + extraDamage, 
            Defense = baseResult.Defense,
            Noise = baseResult.Noise
        };
    }
}
