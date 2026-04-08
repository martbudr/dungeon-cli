namespace Game.Combat;

public interface IAttackHandler
{
    AttackResult OnHeavy(Player player);
    AttackResult OnLight(Player player);
    AttackResult OnMagic(Player player);
    AttackResult OnNone(Player player);
}