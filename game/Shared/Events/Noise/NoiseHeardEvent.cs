using game.Shared.Events.Core;

namespace game.Models.Common.Events.Noise;

public record NoiseHeardEvent(
    string EnemyName,
    int EnemyX,
    int EnemyY,
    int SourceX,
    int SourceY,
    float Distance
) : IUiEvent;