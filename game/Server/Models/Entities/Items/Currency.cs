using Game;
using game.Models.Domain.Equipment.Weapons;
using game.Models.Domain.Rules.Combat;
using game.Server.Models.Equipment.Weapons;
using game.Server.Models.Rules.Combat;
using game.Shared.Enums;

namespace game.Server.Models.Entities.Items;

public class Currency : IItem
{
    public int Id { get; }
    public string Name { get; }
    public char Symbol { get; }

    private readonly int value;
    private readonly CurrencyType type;
    private readonly IWeaponType weaponType = new NoWeaponType();
    public int Noise => weaponType.Noise;

    public Currency(string name, char symbol, CurrencyType type, int value)
    {
        Id = ItemIdGenerator.Next();
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