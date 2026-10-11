using game.Shared.DTOs.Input;
using game.Client.Networking;

namespace game.Server.Commands;

public record PlayerInputCommand(PlayerInputDto Dto, ClientConnection Connection) : IGameCommand;
