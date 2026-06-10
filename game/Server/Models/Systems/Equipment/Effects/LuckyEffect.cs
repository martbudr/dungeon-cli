using Game.Effects;
using game.Models.Domain.Equipment;
using game.Models.Domain.Rules.Combat;
using game.Server.Models.Entities.Items;
using game.Server.Models.Entities.Player;
using game.Server.Models.Equipment;
using game.Server.Models.Rules.Combat;
using game.Server.Models.Systems.Equipment;
using game.Server.Models.Systems.Equipment.Effects;

namespace game.Models.Systems.Equipment.Effects;

public class LuckyEffect : ItemEffectDecorator, IStatModifier, IEquippable
{
    private readonly int _luckBonus;

    public LuckyEffect(IItem item, int luckBonus = 2) : base(item)
    {
        _luckBonus = luckBonus;
    }

    public override string Name => $"{base.Name} (Lucky)";

    // --- IStatModifier ---
    public int Strength => 0;
    public int Dexterity => 0;
    public int Luck => _luckBonus;
    public int Aggression => 0;
    public int Wisdom => 0;

    // --- IEquippable ---
    // Musimy przekazać zdolność zakładania dalej, 
    // aby LuckyEffect nałożony na miecz nie zepsuł miecza.
    public IEquipStrategy? EquipStrategy => (InnerItem as IEquippable)?.EquipStrategy;

    
    public override AttackResult Calculate(Player player, IAttackHandler attack)
    {
        // Po prostu pytamy przedmiot w środku, nie dodajemy nic od siebie do ataku
        return InnerItem.Calculate(player, attack);
    }
    public void Equip(Player player, bool left)
    {
        // Jeśli przedmiot pod spodem jest zakładalny, wywołaj jego logikę
        if (InnerItem is IEquippable eq)
        {
            eq.EquipStrategy?.Equip(player, this, left);
        }
    }

    public void Unequip(Player player, bool left)
    {
        if (InnerItem is IEquippable eq)
        {
            eq.EquipStrategy?.Unequip(player, this, left);
        }
    }
}