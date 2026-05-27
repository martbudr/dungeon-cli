using System.Reflection;
using game.Models.DTOs;

namespace game.Models.Domain.Entities;

public static class EnemyFactory
{
    private static readonly Dictionary<string, Type> EnemyTypes =
        typeof(Enemy).Assembly
            .GetTypes()
            .Where(t =>
                typeof(Enemy).IsAssignableFrom(t) &&
                !t.IsAbstract)
            .ToDictionary(t => t.Name, t => t);

    public static Enemy Create(EnemyDto dto)
    {
        if (!EnemyTypes.TryGetValue(dto.Species, out var type))
        {
            throw new ArgumentException($"Unknown enemy type: {dto.Species}");
        }

        // znajdź "deserialization constructor"
        var ctor = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance)
            .FirstOrDefault(c =>
            {
                var p = c.GetParameters();
                return p.Length == 5
                       && p[0].ParameterType == typeof(int)
                       && p[1].ParameterType == typeof(string)
                       && p[2].ParameterType == typeof(int)
                       && p[3].ParameterType == typeof(int)
                       && p[4].ParameterType == typeof(int);
            });

        if (ctor is null)
        {
            throw new InvalidOperationException(
                $"No matching constructor for {type.Name}"
            );
        }

        var enemy = (Enemy)ctor.Invoke(new object[]
        {
            dto.Id,
            dto.Name,
            dto.Health,
            dto.Damage,
            dto.Armor
        });

        // Position ustawiamy osobno (bo i tak jest mutable)
        enemy.Position = new Position(
            dto.Position.X,
            dto.Position.Y
        );

        return enemy;
    }
}