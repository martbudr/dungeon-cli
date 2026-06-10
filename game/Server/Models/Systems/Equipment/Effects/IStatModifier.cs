namespace Game.Effects;

public interface IStatModifier
{
    int Strength { get; }
    int Dexterity { get; }
    int Luck { get; }
    int Aggression { get; }
    int Wisdom { get; }
}