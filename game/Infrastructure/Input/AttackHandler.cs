using Game.Combat;
using game.Domain.Entities;
using game.Features.Combat;
using Game.Services;

namespace Game.InputHandlers;

class AttackHandler : InputHandler
{
    private Player player;
    private Board board;
    private IMessageService logger;

    // Czy jesteśmy w trybie oczekiwania na wybór ataku
    private bool waitingForAttackKey = false;

    public AttackHandler(Player player, Board board, IMessageService logger)
    {
        this.player = player;
        this.board = board;
        this.logger = logger;
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
            Enemy? enemy = board.GetEnemyAt(player.X, player.Y);
            if (enemy != null)
            {
                var result = CombatSystem.SingleTurn(player, enemy, attack, logger);

                if (result == CombatResult.EnemyDied)
                {
                    board.Enemies[player.X, player.Y] = null;
                    logger.ShowMessage("Enemy defeated!");
                }
            }

            waitingForAttackKey = false; // wychodzimy z trybu blokowania
        }
        else
        {
            logger.ShowMessage("Invalid key! Press N, K, or M to attack.");
        }

        return true;
    }
}