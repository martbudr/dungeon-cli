using Game;
using game.Models.Common.Enums;
using game.Models.Domain.Equipment.Weapons;
using game.Models.Domain.Rules.Combat;

namespace game.Models.Domain.Entities.Items;

public class Currency : IItem
{
    public string Name { get; }
    public char Symbol { get; }

    private readonly int value;
    private readonly CurrencyType type;
    private readonly IWeaponType weaponType = new NoWeaponType();
    public int Noise => weaponType.Noise;

    public Currency(string name, char symbol, CurrencyType type, int value)
    {
        Name = name;
        Symbol = symbol;
        this.type = type;
        this.value = value;
    }
    
    public CurrencyType CurrencyType => type;
    public int GetValue() => value;
    
    public AttackResult Calculate(Player.Player player, IAttackHandler attack)
        => weaponType.ResolveAttack(attack, player);
}