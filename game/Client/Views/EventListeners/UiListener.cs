using game.Models.Common.Events;
using game.Models.Common.Events.Noise;
using game.Shared.Events.Combat;
using game.Shared.Events.Core;
using game.Shared.Events.Dispatchers;
using game.Shared.Events.UI;

namespace game.Client.Views.EventListeners;

public class UiListener : IEventListener<IUiEvent>, IDisposable
{
    private readonly UiEventDispatcher ui;

    public UiListener(UiEventDispatcher ui)
    {
        this.ui = ui;
        ui.Subscribe(this);
    }
    
    public void Handle(IUiEvent e)
    {
        switch (e)
        {
            case AttackDamageEvent d:
                DealDamage(d);
                break;
            
            case DefenseDamageEvent d:
                HandleDamage(d);
                break;

            case DeathEvent d:
                HandleDeath(d);
                break;
            
            case NoiseHeardEvent d:
                HandleNoise(d);
                break;
            
            case OutOfBoundsEvent:
                ui.Publish(new GameMessageEvent("Trying to go outside the map."));
                break;
            
            case WallCollisionEvent:
                ui.Publish(new GameMessageEvent("Trying to bump into a wall."));
                break;
            
            case PlayerCollisionEvent:
                ui.Publish(new GameMessageEvent("You can't move onto a tile occupied by another player."));
                break;
            
            case InvalidAttackKeyEvent:
                ui.Publish(new GameMessageEvent("Attack: [N] Normal, [B] Sneak/backstab, [M] Magic"));
                break;
            
            case UnknownKeyEvent u:
                ui.Publish(new GameMessageEvent($"The key {u.Key} does nothing here."));
                break;
            
            case GameStartedEvent g:
                ui.Publish(new GameMessageEvent(g.WelcomeMessage));
                break;
            case ItemEquippedEvent i:
                ui.Publish(new GameMessageEvent($"You equipped the {i.ItemName}."));
                break;

            case JournalOpenedEvent:
                ui.Publish(new GameMessageEvent("Reading journal... Any key closes; [X] quits."));
                break;
            
            case JournalClosedEvent:
                ui.Publish(new GameMessageEvent("Closed journal."));
                break;

            case ItemEquippedInHandEvent i:
                var handText = i.Hand switch
                {
                    HandType.Both => "both hands",
                    HandType.Left => "left hand",
                    HandType.Right => "right hand",
                    _ => "hand"
                };

                ui.Publish(new GameMessageEvent(
                    $"You take the {i.ItemName} in your {handText}."
                ));
                break;

            case ItemCannotBeEquippedEvent:
                ui.Publish(new GameMessageEvent("This item cannot be equipped."));
                break;
        }
    }

    private void DealDamage(AttackDamageEvent a)
    {
        if (a.IsPlayer)
        {
            ui.Publish(new GameMessageEvent(
                a.IsPlayer
                    ? $"You use {a.AttackName} and deal {a.Damage} damage!"
                    : $"{a.Name} attacks and deals {a.Damage} damage."
                ));
        }
    }
    
    private void HandleDamage(DefenseDamageEvent e)
    {
        if (e.Blocked <= 0)
        {
            ui.Publish(new GameMessageEvent(
                e.IsPlayer
                    ? $"You block the attack! (Armor: {e.Blocked})"
                    : $"{e.Name} blocks the attack! (Armor: {e.Blocked})"
            ));
            return;
        }

        ui.Publish(new GameMessageEvent(
            e.IsPlayer
                ? $"You take {e.Damage} damage{(e.Blocked > 0 ? $" ({e.Blocked} blocked)" : "")}."
                : $"{e.Name} takes {e.Damage} damage{(e.Blocked > 0 ? $" ({e.Blocked} blocked)" : "")}."
        ));
    }
    
    private void HandleDeath(DeathEvent e)
    {
        ui.Publish(new GameMessageEvent(
            e.IsPlayer
                ? $"CRITICAL: {e.Name} has fallen..."
                : $"Enemy {e.Name} has fallen!"
        ));
    }
    
    private void HandleNoise(NoiseHeardEvent e)
    {
        ui.Publish(new GameMessageEvent(
            $"{e.EnemyName} at ({e.EnemyX}, {e.EnemyY}) heard noise from ({e.SourceX}, {e.SourceY}) at {e.Distance}"
        ));
    }
    
    public void Dispose()
    {
        ui.Unsubscribe(this);
    }
}
