using game.Models.Domain.Entities.Player;

namespace game.Models.Domain.Rules.Combat;

public class MagicAttack : IAttackHandler
{
    public AttackResult OnHeavy(Player p)
        => new AttackResult
        {
            Damage = 1,
            Defense = p.Luck
        };

    public AttackResult OnLight(Player p)
        => new AttackResult
        {
            Damage = 1,
            Defense = p.Luck
        };

    public AttackResult OnMagic(Player p)
        => new AttackResult
        {
            Damage = p.Wisdom,
            Defense = p.Wisdom * 2
        };

    public AttackResult OnNone(Player p)
        => new AttackResult
        {
            Damage = 0,
            Defense = p.Luck
        };
}