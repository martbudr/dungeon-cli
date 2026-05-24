using game.Models.Domain.Entities.Player;

namespace game.Models.Domain.Rules.Combat;

public class SneakAttack : IAttackHandler
{
    public AttackResult OnHeavy(Player p)
        => new AttackResult
        {
            Damage = (p.Strength + p.Aggression) / 2,
            Defense = p.Strength
        };

    public AttackResult OnLight(Player p)
        => new AttackResult
        {
            Damage = (p.Dexterity + p.Luck) * 2,
            Defense = p.Dexterity
        };

    public AttackResult OnMagic(Player p)
        => new AttackResult
        {
            Damage = 1,
            Defense = 0
        };

    public AttackResult OnNone(Player p)
        => new AttackResult
        {
            Damage = 0,
            Defense = 0
        };
}