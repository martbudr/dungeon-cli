using System.Collections.Concurrent;
using game.Models.Common.Events;
using game.Models.Systems;
using game.Server.Models.Entities.Board;
using game.Server.Models.Entities.Player;
using game.Server.Models.Systems;

namespace game.Server.Models.Core;

public class GameState
{
    public Board Board { get; set; }

    public ConcurrentDictionary<int, Player> Players { get; }

    public bool ShouldExit { get; set; }

    public GameState(Board board)
    {
        Board = board;
        Players = new ConcurrentDictionary<int, Player>();
        ShouldExit = false;
    }
}