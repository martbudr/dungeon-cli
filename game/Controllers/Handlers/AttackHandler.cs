using game.Models.Common.Enums;
using game.Models.Common.Events;
using game.Models.Domain.Entities.Board;
using game.Models.Domain.Entities.Player;
using game.Models.Domain.Rules.Combat;
using game.Models.Systems.Combat;

namespace game.Controllers.Handlers;

class AttackHandler : InputHandler
{
    private Player player;
    private Board board;
    private readonly EventDispatcher dispatcher;
    private readonly CombatSystem combatSystem;

    // Czy jesteśmy w trybie oczekiwania na wybór ataku
    private bool waitingForAttackKey = false;

    public AttackHandler(Player player, Board board,  EventDispatcher dispatcher, CombatSystem combatSystem)
    {
        this.player = player;
        this.board = board;
        this.dispatcher = dispatcher;
        this.combatSystem = combatSystem;
    }

    public override bool Handle(ConsoleKey key)
    {
        if (!waitingForAttackKey)
        {
            if (key == ConsoleKey.H) // klawisz inicjujący atak
            {
                waitingForAttackKey = true;
                return true;
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
            var enemy = board.GetEnemyAt(player.X, player.Y);
            if (enemy != null)
            {
                var result = combatSystem.ExecuteTurn(player, enemy, attack);

                if (result == CombatResult.EnemyDied)
                {
                    board.Enemies[player.X, player.Y] = null;
                    dispatcher.Publish(new GameMessageEvent("Enemy defeated!"));
                }
            }

            waitingForAttackKey = false; // wychodzimy z trybu blokowania
        }
        else
        {
            dispatcher.Publish(new GameMessageEvent("Invalid key! Press N, K, or M to attack."));
        }

        return true;
    }
}