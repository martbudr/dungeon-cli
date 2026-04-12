using Game.Combat;

namespace Game.Effects;

public abstract class ItemEffectDecorator : IItem, IEquippable
{
    protected readonly IItem _innerItem;

    protected ItemEffectDecorator(IItem item)
    {
        _innerItem = item ?? throw new ArgumentNullException(nameof(item), "Decorator cannot wrap null");
    }

    public virtual string Name => _innerItem.Name;
    public virtual char Symbol => _innerItem.Symbol;
    
    public virtual AttackResult Calculate(Player player, IAttackHandler attack) 
        => _innerItem.Calculate(player, attack);
    
    public virtual IEquipStrategy? EquipStrategy => (_innerItem as IEquippable)?.EquipStrategy;
    
    public virtual void Equip(Player player, bool left)
    {
        // Jeśli przedmiot w środku nie jest zakładalny, ten dekorator 
        // po prostu nie robi nic w kwestii zakładania.
        if (_innerItem is IEquippable eq)
        {
            eq.EquipStrategy?.Equip(player, this, left);
        }
    }
    
    public virtual void Unequip(Player player, bool left)
    {
        if (_innerItem is IEquippable eq)
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
            current = dec._innerItem;
        }
        if (current is IStatModifier baseMod) yield return baseMod;
    }
}