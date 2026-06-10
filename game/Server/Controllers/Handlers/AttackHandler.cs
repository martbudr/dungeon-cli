using game.Models.Domain.Rules.Combat;
using game.Server.Bridges;
using game.Server.Models.Entities.Board;
using game.Server.Models.Entities.Player;
using game.Server.Models.Rules.Combat;
using game.Server.Models.Systems.Combat;
using game.Shared.DTOs.Patch;
using game.Shared.Enums;
using game.Shared.Events.Combat;
using game.Shared.Events.Dispatchers;
using game.Shared.Events.UI;

namespace game.Server.Controllers.Handlers;

class AttackHandler : InputHandler
{
    private Player player;
    private Board board;
    private readonly UiEventDispatcher uiDispatcher;
    private readonly CombatSystem combatSystem;

    // Czy jesteśmy w trybie oczekiwania na wybór ataku
    private bool waitingForAttackKey = false;

    public AttackHandler(Player player, Board board,  UiEventDispatcher uiDispatcher, CombatSystem combatSystem)
    {
        this.player = player;
        this.board = board;
        this.uiDispatcher = uiDispatcher;
        this.combatSystem = combatSystem;
    }

    public override List<IGamePatch> Handle(ConsoleKey key)
    {
        var patches = new List<IGamePatch>();
        
        if (!waitingForAttackKey)
        {
            if (key == ConsoleKey.H) // klawisz inicjujący atak
            {
                waitingForAttackKey = true;
                var eventToPublish = new GameMessageEvent("Choose attack type: N, K, M", player.Id);
                var newPatches = UiEventBridge.PublishAndPatch(eventToPublish, uiDispatcher);
                patches.AddRange(newPatches);
            }
            
            return patches;
        }

        // Jeśli jesteśmy w trybie oczekiwania, akceptujemy tylko N/K/M
        IAttackHandler? attack = key switch
        {
            ConsoleKey.N => new NormalAttack(),
            ConsoleKey.K => new SneakAttack(),
            ConsoleKey.M => new MagicAttack(),
            _ => null
        };

        if (attack != null)
        {
            var x = player.Position.X;
            var y = player.Position.Y;
            
            var enemy = board.GetEnemyAt(x, y);
            if (enemy != null)
            {
                var combat = combatSystem.ExecuteTurn(player, enemy, attack);
                patches.AddRange(combat.Patches);
                
                patches.Add(new CombatUpdatePatch 
                { 
                    PlayerId = player.Id, 
                    EnemyId = enemy.Id,
                    PlayerCurrentHp = player.Health,
                    EnemyCurrentHp = enemy.Health
                });

                if (combat.Result == CombatResult.EnemyDied)
                {
                    board.Enemies[x, y] = null;
                    
                    var eventToPublish = new DeathEvent(player.Name, false);
                    patches.AddRange( UiEventBridge.PublishAndPatch(eventToPublish, uiDispatcher));
                    
                    patches.Add(new EnemyDiedPatch 
                    { 
                        EnemyId = enemy.Id, 
                        X = x, 
                        Y = y 
                    });
                }

                if (combat.Result == CombatResult.PlayerDied)
                {
                    board.Players[x, y] = null;
                    var eventToPublish = new DeathEvent(player.Name, true);
                    patches.AddRange(UiEventBridge.PublishAndPatch(eventToPublish, uiDispatcher));
                    
                    patches.Add(new PlayerDiedPatch
                    {
                        PlayerId = player.Id,
                        X = x,
                        Y = y
                    });
                }
            }

            waitingForAttackKey = false; // wychodzimy z trybu blokowania
        }
        else
        {
            var newPatches = UiEventBridge.PublishAndPatch(new InvalidAttackKeyEvent(), uiDispatcher);
            patches.AddRange(newPatches);
        }
        
        return patches;
    }
}