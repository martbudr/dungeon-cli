using game.Models.Common.Events;
using game.Server.Models.Entities.Board;
using game.Server.Models.Systems;
using game.Server.Models.Systems.Combat;
using game.Shared.DTOs.Patch;
using game.Shared.Events.Core;
using game.Shared.Events.Dispatchers;

namespace game.Server.Models.Core;

public class GameSystems
{
    public DamageSystem DamageSystem { get; }

    public CombatSystem CombatSystem { get; }

    public EnemyMovementSystem EnemyMovementSystem { get; }

    public GameSystems(
        Board board,
        UiEventDispatcher uiDispatcher,
        GameplayEventDispatcher gameplayDispatcher)
    {
        DamageSystem = new DamageSystem(
            uiDispatcher,
            gameplayDispatcher);

        CombatSystem = new CombatSystem(
            DamageSystem,
            uiDispatcher,
            board);

        EnemyMovementSystem = new EnemyMovementSystem();
    }
}