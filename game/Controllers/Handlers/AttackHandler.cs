using game.Models.Common.Enums;
using game.Models.Common.Events;
using game.Models.Domain.Entities.Board;
using game.Models.Domain.Entities.Player;
using game.Models.Domain.Rules.Combat;
using game.Models.DTOs;
using game.Models.Systems.Combat;

namespace game.Controllers.Handlers;

class AttackHandler : InputHandler
{
    private Player player;
    private Board board;
    private readonly UiEventDispatcher uiDispatcher;
    private readonly CombatSystem combatSystem;

    // Czy jesteśmy w trybie oczekiwania na wybór ataku
    private bool waitingForAttackKey = false;

    public AttackHandler(Player player, Board board,  UiEventDispatcher uiDispatcher, CombatSystem combatSystem)
    {
        this.player = player;
        this.board = board;
        this.uiDispatcher = uiDispatcher;
        this.combatSystem = combatSystem;
    }

    public override List<IUpdateDto> Handle(ConsoleKey key)
    {
        var updates = new List<IUpdateDto>();
        
        if (!waitingForAttackKey)
        {
            if (key == ConsoleKey.H) // klawisz inicjujący atak
            {
                waitingForAttackKey = true;
                return updates;
            }

            return base.Handle(key);
        }

        // Jeśli jesteśmy w trybie oczekiwania, akceptujemy tylko N/K/M
        IAttackHandler? attack = key switch
        {
            ConsoleKey.N => new NormalAttack(),
            ConsoleKey.K => new SneakAttack(),
            ConsoleKey.M => new MagicAttack(),
            _ => null
        };

        if (attack != null)
        {
            var x = player.Position.X;
            var y = player.Position.Y;
            
            var enemy = board.GetEnemyAt(x, y);
            if (enemy != null)
            {
                var result = combatSystem.ExecuteTurn(player, enemy, attack);

                if (result == CombatResult.EnemyDied)
                {
                    updates.Add(new EnemyDiedDto(enemy.Id, x, y));
                    board.Enemies[x, y] = null;
                    uiDispatcher.Publish(new DeathEvent(enemy.Name, false));
                }
            }

            waitingForAttackKey = false; // wychodzimy z trybu blokowania
        }
        else
        {
            uiDispatcher.Publish(new InvalidAttackKeyEvent());
        }

        return updates;
    }
}