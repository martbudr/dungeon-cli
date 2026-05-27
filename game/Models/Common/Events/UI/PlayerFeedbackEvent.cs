namespace game.Models.Common.Events;

public record OutOfBoundsEvent : IUiEvent;

public record WallCollisionEvent : IUiEvent;

public record InvalidAttackKeyEvent : IUiEvent;

public record UnknownKeyEvent(ConsoleKey Key) : IUiEvent;

public record GameStartedEvent(string WelcomeMessage) : IUiEvent;

public record ItemEquippedEvent(string ItemName) : IUiEvent;

public record ItemEquippedInHandEvent(string ItemName, HandType Hand) : IUiEvent;

public enum HandType
{
    Left,
    Right,
    Both
}

public record JournalOpenedEvent : IUiEvent;

public record ItemCannotBeEquippedEvent : IUiEvent;