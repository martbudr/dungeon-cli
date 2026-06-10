using game.Shared.DTOs.Input;

namespace game.Server.Commands;

public record PlayerInputCommand(PlayerInputDto Dto) : IGameCommand;