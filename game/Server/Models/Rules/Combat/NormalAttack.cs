using game.Models.Domain.Rules.Combat;
using game.Server.Models.Entities.Player;

namespace game.Server.Models.Rules.Combat;

public class NormalAttack : IAttackHandler
{
    public AttackResult OnHeavy(Player p)
        => new AttackResult
        {
            Damage = p.Strength + p.Aggression,
            Defense = p.Strength + p.Luck
        };

    public AttackResult OnLight(Player p)
        => new AttackResult
        {
            Damage = p.Dexterity + p.Luck,
            Defense = p.Dexterity + p.Luck
        };

    public AttackResult OnMagic(Player p)
        => new AttackResult
        {
            Damage = 1,
            Defense = p.Dexterity + p.Luck
        };

    public AttackResult OnNone(Player p)
        => new AttackResult
        {
            Damage = 0,
            Defense = p.Dexterity
        };
}