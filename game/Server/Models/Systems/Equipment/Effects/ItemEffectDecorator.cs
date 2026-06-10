using Game.Effects;
using game.Models.Domain.Equipment;
using game.Models.Domain.Rules.Combat;
using game.Models.Systems.Equipment;
using game.Server.Models.Entities.Items;
using game.Server.Models.Entities.Player;
using game.Server.Models.Equipment;
using game.Server.Models.Rules.Combat;

namespace game.Server.Models.Systems.Equipment.Effects;

public abstract class ItemEffectDecorator : IItem, IEquippable
{
    protected readonly IItem InnerItem;

    protected ItemEffectDecorator(IItem item)
    {
        InnerItem = item ?? throw new ArgumentNullException(nameof(item), "Decorator cannot wrap null");
    }
    
    internal IItem WrappedItem => InnerItem;
    public virtual string Name => InnerItem.Name;
    public virtual char Symbol => InnerItem.Symbol;
    public int Noise => InnerItem.Noise;
    public int Id => InnerItem.Id;

    public virtual AttackResult Calculate(Player player, IAttackHandler attack) 
        => InnerItem.Calculate(player, attack);
    
    public virtual IEquipStrategy? EquipStrategy => (InnerItem as IEquippable)?.EquipStrategy;
    
    public virtual void Equip(Player player, bool left)
    {
        // Jeśli przedmiot w środku nie jest zakładalny, ten dekorator 
        // po prostu nie robi nic w kwestii zakładania.
        if (InnerItem is IEquippable eq)
        {
            eq.EquipStrategy?.Equip(player, this, left);
        }
    }
    
    public virtual void Unequip(Player player, bool left)
    {
        if (InnerItem is IEquippable eq)
        {
            eq.EquipStrategy?.Unequip(player, this, left);
        }
    }

    public IEnumerable<IStatModifier> GetAllModifiers()
    {
        IItem current = this;
        while (current is ItemEffectDecorator dec)
        {
            if (dec is IStatModifier mod) yield return mod;
            current = dec.InnerItem;
        }
        if (current is IStatModifier baseMod) yield return baseMod;
    }
}