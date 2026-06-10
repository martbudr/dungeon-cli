using game.Client.Networking;
using game.Shared.DTOs.Common;

namespace game.Server.Commands;

public class PlayerJoinCommand : IGameCommand
{
    public PlayerJoinDto Dto { get; }
    public ClientConnection Connection { get; }

    public PlayerJoinCommand(PlayerJoinDto dto, ClientConnection connection)
    {
        Dto = dto;
        Connection = connection;
    }
}