using game.Models.Domain.Entities;
using game.Server.Models.Entities;
using game.Server.Models.Entities.Items;
using game.Shared.DTOs.Entities;
using game.Shared.DTOs.Snapshot;
using game.Shared.DTOs.Stats;
using Board = game.Server.Models.Entities.Board.Board;
using Player = game.Server.Models.Entities.Player.Player;
using PlayerBaseStats = game.Server.Models.Entities.Player.PlayerBaseStats;

namespace game.Shared.DTOs.Mapper;

public static class SnapshotToDtoMapper
{
    public static PlayerSnapshotDto ToDto(Player player)
    {
        var baseStats = player.GetBaseStats();
        
        return new PlayerSnapshotDto
        {
            Id = player.Id,
            Name = player.Name,
            Health = player.Health,
            Coins = player.Coins,
            Gold = player.Gold,
            Position = ToDto(player.Position),
            BaseStats = new PlayerBaseStatsDto
            {
                BaseAggression = baseStats.BaseAggression,
                BaseDexterity = baseStats.BaseDexterity,
                BaseLuck = baseStats.BaseLuck,
                BaseStrength = baseStats.BaseStrength,
                BaseWisdom = baseStats.BaseWisdom
            },
            Stats = ToStatsDto(player),
            Equipment = new EquipmentSnapshotDto
            {
                LeftHandItem =  player.LeftHandItem != null ? ToDto(player.LeftHandItem) : null,
                RightHandItem = player.RightHandItem != null ? ToDto(player.RightHandItem) : null,
            },
            Inventory = new InventorySnapshotDto
            {
                Items = player.Inventory.Select(i => ToDto(i)).ToList()
            },
            SelectedItem = player.SelectedItem
        };
    }

    public static PlayerStatsDto ToStatsDto(Player player)
    {
        return new PlayerStatsDto
        {
            Aggression = player.Aggression,
            Coins = player.Coins,
            Gold = player.Gold,
            Dexterity = player.Dexterity,
            Luck = player.Luck,
            Strength = player.Strength,
            Wisdom = player.Wisdom,
            Health = player.Health
        };
    }

    private static PositionDto ToDto(Position position)
    {
        return new PositionDto
        {
            X = position.X,
            Y = position.Y
        };
    }

    public static BoardSnapshotDto ToDto(Board board)
    {
        var grid = new CellDto[GlobalConstants.HEIGHT][];
        
        for (var i = 0; i < GlobalConstants.HEIGHT; ++i)
        {
            grid[i] = new CellDto[GlobalConstants.WIDTH];
            
            for (var j = 0; j < GlobalConstants.WIDTH; ++j)
            {
                grid[i][j] = new CellDto
                {
                    Enemy = ToDto(board.Enemies[i, j]),
                    Items = ToDto(board.Items[i, j]),
                    Player = board.Players[i,j] != null ? ToDto(board.Players[i, j]!) : null,
                    Tile = board.Grid[i, j]
                };
            }
        }

        return new BoardSnapshotDto
        {
            Grid = grid
        };
    }

    public static EnemySnapshotDto? ToDto(Enemy? enemy)
    {
        if (enemy is null) return null;
        
        return new EnemySnapshotDto
        {
            Id = enemy.Id,
            Name = enemy.Name,
            Position = enemy.Position,
            Species = enemy.Species,
            Health = enemy.Health
        };
    }
    
    public static ItemDto ToDto(IItem item)
    {
        return new ItemDto
        {
            Id = item.Id,
            Name = item.Name,
            Symbol = item.Symbol,
            ItemType = item.GetType().Name
        };
    }
    
    private static List<ItemDto>? ToDto(List<IItem>? items)
    {
        if (items == null || items.Count == 0)
            return null;

        return items
            .Select(ToDto)
            .ToList();
    }
}
