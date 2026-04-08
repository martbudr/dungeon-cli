using game.Combat;
using Game.Combat;

namespace Game.InputHandlers;

class AttackHandler : InputHandler
{
    private Player player;
    private Board board;

    // Czy jesteśmy w trybie oczekiwania na wybór ataku
    private bool waitingForAttackKey = false;

    public AttackHandler(Player player, Board board)
    {
        this.player = player;
        this.board = board;
    }

    public override bool Handle(ConsoleKey key)
    {
        if (!waitingForAttackKey)
        {
            if (key == ConsoleKey.H) // klawisz inicjujący atak
            {
                waitingForAttackKey = true;
                player.ShowMessage("Choose attack: N = normal, K = sneak, M = magic");
                return true;
            }

            return base.Handle(key);
        }

        // Jeśli jesteśmy w trybie oczekiwania, akceptujemy tylko Z/X/C
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
                var result = CombatSystem.SingleTurn(player, enemy, attack);

                if (result == CombatResult.EnemyDied)
                {
                    board.Enemies[player.X, player.Y] = null;
                    player.ShowMessage("Enemy defeated!");
                }

                if (result == CombatResult.PlayerDied)
                {
                    player.Kill();
                }
            }

            waitingForAttackKey = false; // wychodzimy z trybu blokowania
            return true;
        }
        else
        {
            player.ShowMessage("Invalid key! Press N, K, or M to attack.");
            return true; // blokujemy wszystko inne
        }
    }
}