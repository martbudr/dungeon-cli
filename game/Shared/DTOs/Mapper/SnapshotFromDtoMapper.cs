using game.Client.Models;
using game.Models.Domain.Entities;
using game.Shared.DTOs.Entities;
using game.Shared.DTOs.Snapshot;
using game.Shared.DTOs.Stats;

namespace game.Shared.DTOs.Mapper;

public static class SnapshotFromDtoMapper
{
    public static Player FromDto(PlayerSnapshotDto dto)
    {
        return new Player
        {
            Id = dto.Id,
            Name = dto.Name,
            Position = FromDto(dto.Position),
            Health = dto.Health,
            Coins = dto.Coins,
            Gold = dto.Gold,
            BaseStats = FromDto(dto.BaseStats),
            Stats = FromDto(dto.Stats),
            Equipment = new Equipment
            {
                LeftHandItem = dto.Equipment.LeftHandItem != null ? FromDto(dto.Equipment.LeftHandItem) : null,
                RightHandItem = dto.Equipment.RightHandItem != null ? FromDto(dto.Equipment.RightHandItem) : null,
            },
            Inventory = dto.Inventory.Items.Count > 0
                ? dto.Inventory.Items.Select(FromDto).ToList() 
                : [],
            SelectedItem = dto.SelectedItem
        };
    }

    public static Position FromDto(PositionDto dto)
    {
        return new Position { X = dto.X, Y = dto.Y };
    }

    public static PlayerStats FromDto(PlayerStatsDto dto)
    {
        return new PlayerStats
        {
            Strength = dto.Strength,
            Dexterity = dto.Dexterity,
            Health = dto.Health,
            Luck = dto.Luck,
            Aggression = dto.Aggression,
            Wisdom = dto.Wisdom,
            Coins = dto.Coins,
            Gold = dto.Gold
        };
    }

    public static PlayerBaseStats FromDto(PlayerBaseStatsDto dto)
    {
        return new PlayerBaseStats
        {
            BaseAggression = dto.BaseAggression,
            BaseDexterity = dto.BaseDexterity,
            BaseLuck = dto.BaseLuck,
            BaseStrength = dto.BaseStrength,
            BaseWisdom = dto.BaseWisdom
        };
    }

    public static Enemy FromDto(EnemySnapshotDto dto)
    {
        return new Enemy
        {
            Id = dto.Id,
            Name = dto.Name,
            Position = new Position(dto.Position.X, dto.Position.Y),
            Species = dto.Species,
            Health = dto.Health
        };
    }

    public static Board FromDto(BoardSnapshotDto dto)
    {
        var board = new Board
        {
            Grid = new char[GlobalConstants.HEIGHT][],
            Items = new List<GenericItem>[GlobalConstants.HEIGHT, GlobalConstants.WIDTH],
            Enemies = new Enemy?[GlobalConstants.HEIGHT, GlobalConstants.WIDTH],
            Players = new Player?[GlobalConstants.HEIGHT, GlobalConstants.WIDTH]
        };

        for (int y = 0; y < GlobalConstants.HEIGHT; y++)
        {
            board.Grid[y] = new char[GlobalConstants.WIDTH];

            for (int x = 0; x < GlobalConstants.WIDTH; x++)
            {
                var cell = dto.Grid[y][x];

                board.Grid[y][x] = cell.Tile;

                board.Items[y, x] = cell.Items != null
                    ? cell.Items.Select(FromDto).ToList()
                    : new List<GenericItem>();

                if (cell.Enemy != null)
                    board.Enemies[y, x] = FromDto(cell.Enemy);

                if (cell.Player != null)
                    board.Players[y, x] = FromDto(cell.Player);
            }
        }

        return board;
    }

    public static GenericItem FromDto(ItemDto dto)
    {
        return new GenericItem
        {
            Id = dto.Id,
            Name = dto.Name,
            Symbol = dto.Symbol,
            Type = dto.ItemType
        };
    }
}