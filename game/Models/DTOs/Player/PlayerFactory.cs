using game.Models.Common.Events;
using game.Models.Domain.Entities.Board;

namespace game.Models.DTOs.Player;

public static class PlayerFactory
{
    public static Domain.Entities.Player.Player Create(
        PlayerDto dto,
        UiEventDispatcher uiDispatcher,
        GameplayEventDispatcher gameplayDispatcher,
        Board board)
    {
        return new Domain.Entities.Player.Player(
            dto.Id,
            dto.Name,
            uiDispatcher,
            gameplayDispatcher,
            board
        );
    }
}