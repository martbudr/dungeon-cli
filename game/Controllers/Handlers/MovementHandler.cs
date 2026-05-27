using game.Infrastructure.Services;
using game.Models.Common.Events;
using game.Models.Common.Events.Noise;
using game.Models.Domain.Entities;
using game.Models.Domain.Entities.Board;
using game.Models.Domain.Entities.Player;
using game.Models.DTOs;

namespace game.Controllers.Handlers;

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

    public override List<IUpdateDto> Handle(ConsoleKey key)
    {
        int dx = 0, dy = 0;
        
        switch (key)
        {
            case ConsoleKey.W: dx = -1; break;
            case ConsoleKey.S: dx = 1; break;
            case ConsoleKey.A: dy = -1; break;
            case ConsoleKey.D: dy = 1; break;
            default: return base.Handle(key);
        }
        
        var result = Move(dx, dy, board);

        return result;
    }
    
    public List<IUpdateDto> Move(int dx, int dy, Board board)
    {
        var result = new List<IUpdateDto>();
        
        var oldPos = player.Position;
        
        var nx = player.Position.X + dx;
        var ny = player.Position.Y + dy;

        if (!Board.IdxInRange(nx, ny))
        {
            uiDispatcher.Publish(new OutOfBoundsEvent());
            return result;
        }

        if (board.IsWall(nx, ny))
        {
            uiDispatcher.Publish(new WallCollisionEvent());
            return result;
        }
        
        var tiles = player.MakeNoise(NoiseProfile.Movement, board);
        player.SetPosition(new Position(nx, ny));
        
        gameplayDispatcher.Publish(new NoiseEvent(oldPos.X, oldPos.Y, tiles));
        result.Add(new PlayerMovedDto(player.Id, nx, ny));
        
        return result;
    }
}