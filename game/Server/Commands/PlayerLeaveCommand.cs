namespace game.Server.Commands;

public record PlayerLeaveCommand(int PlayerId) : IGameCommand;
