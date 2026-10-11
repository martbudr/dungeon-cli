using game.Models.Common.Events;
using game.Models.Common.Events.Noise;
using game.Server.Bridges;
using game.Server.Models.Entities.Board;
using game.Server.Models.Entities.Player;
using game.Shared.DTOs.Patch;
using game.Shared.Events.Dispatchers;
using game.Shared.Events.UI;

namespace game.Server.Controllers.Handlers;

class MovementHandler : InputHandler
{
    private Player player;
    private Board board;
    private readonly UiEventDispatcher uiDispatcher;
    private readonly GameplayEventDispatcher gameplayDispatcher;

    public MovementHandler(Player player, Board board, UiEventDispatcher uiDispatcher, GameplayEventDispatcher gameplayDispatcher)
    {
        this.player = player;
        this.board = board;
        this.uiDispatcher = uiDispatcher;
        this.gameplayDispatcher = gameplayDispatcher;
    }

    public override List<IGamePatch> Handle(ConsoleKey key)
    {
        if (!IsMovementKey(key)) return base.Handle(key);
        
        var patches = new List<IGamePatch>();
        
        var (nx, ny) = CalculateNewPosition(key);
        
        if (!Board.IdxInRange(nx, ny))
        {
            var newPatches = UiEventBridge.PublishAndPatch(new OutOfBoundsEvent(), uiDispatcher);
            patches.AddRange(newPatches);
            
            return patches;
        }

        if (board.IsWall(nx, ny))
        {
            var newPatches = UiEventBridge.PublishAndPatch(new WallCollisionEvent(), uiDispatcher);
            patches.AddRange(newPatches);
            
            return patches;
        }
        
        if (board.Players[nx, ny] != null)
        {
            var newPatches = UiEventBridge.PublishAndPatch(new PlayerCollisionEvent(), uiDispatcher);
            patches.AddRange(newPatches);

            return patches;
        }

        player.MakeNoise(NoiseProfile.Movement, board);
        
        board.SetPlayerPosition(player, nx, ny);
        patches.Add(new PlayerMovedPatch
        {
            PlayerId = player.Id,
            X = nx,
            Y = ny,
        });
        
        return patches;
    }

    private bool IsMovementKey(ConsoleKey key)
    {
        return key is ConsoleKey.W
            or ConsoleKey.S
            or ConsoleKey.A
            or ConsoleKey.D;
    }

    private (int x, int y) CalculateNewPosition(ConsoleKey key)
    {
        int dx = 0, dy = 0;
        switch (key)
        {
            case  ConsoleKey.W:
                dx = -1;
                break;
            case ConsoleKey.S:
                dx = 1;
                break;
            case ConsoleKey.A:
                dy = -1;
                break;
            case ConsoleKey.D:
                dy = 1;
                break;
        }
        return (player.Position.X + dx, player.Position.Y + dy);
    }
}
