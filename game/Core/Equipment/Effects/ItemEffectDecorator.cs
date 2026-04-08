using Game.Combat;

namespace Game.Effects;

public abstract class ItemEffectDecorator : IItem, IEquippable
{
    protected IItem _innerItem;

    protected ItemEffectDecorator(IItem item)
    {
        _innerItem = item;
    }

    // Przekazujemy nazwę + efekt
    public virtual string Name => _innerItem.Name;

    public virtual char Symbol => _innerItem.Symbol;

    public virtual bool IsCurrency => _innerItem.IsCurrency;

    public virtual AttackResult Calculate(Player player, IAttackHandler attack)
        => _innerItem.Calculate(player, attack);
    
    public IEnumerable<IStatModifier> GetAllModifiers()
    {
        var current = this as IItem;

        while (current is ItemEffectDecorator dec)
        {
            if (dec is IStatModifier mod)
                yield return mod;

            current = dec._innerItem;
        }

        if (current is IStatModifier baseMod)
            yield return baseMod;
    }

    // --- IEquippable ---

    public virtual IEquipStrategy? EquipStrategy => _innerItem is IEquippable eq2 ? eq2.EquipStrategy : null;

    public virtual void Equip(Player player, bool left)
    {
        if (_innerItem is IEquippable eq)
            eq.Equip(player, left);
        else
            throw new InvalidOperationException("This item cannot be equipped.");
    }

    public virtual void Unequip(Player player, bool left)
    {
        if (_innerItem is IEquippable eq)
            eq.Unequip(player, left);
        else
            throw new InvalidOperationException("This item cannot be unequipped.");
    }
}