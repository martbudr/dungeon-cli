using System.Text.Json.Serialization;
using game.Client.Models;

namespace game.Shared.DTOs.Patch;

[JsonDerivedType(typeof(PlayerMovedPatch), typeDiscriminator: "move")]
[JsonDerivedType(typeof(PlayerJoinedPatch), typeDiscriminator: "player_joined")]
[JsonDerivedType(typeof(PlayerLeftPatch), typeDiscriminator: "player_left")]
[JsonDerivedType(typeof(PlayerDiedPatch), typeDiscriminator: "death")]
[JsonDerivedType(typeof(ItemPickedUpPatch), typeDiscriminator: "item_picked_up")]
[JsonDerivedType(typeof(ItemSelectionChangedPatch), typeDiscriminator: "item_selection_changed")]
[JsonDerivedType(typeof(ItemDroppedPatch), typeDiscriminator: "item_dropped")]
[JsonDerivedType(typeof(EquipmentPatch), typeDiscriminator: "equipment_changed")] 
[JsonDerivedType(typeof(InventoryPatch), typeDiscriminator: "inventory_changed")] 
[JsonDerivedType(typeof(AddUiEventPatch), typeDiscriminator: "added_log_text")]
[JsonDerivedType(typeof(CombatUpdatePatch), typeDiscriminator: "combat_update")]
[JsonDerivedType(typeof(EnemyMovedPatch), typeDiscriminator: "enemy_moved")]
[JsonDerivedType(typeof(EnemyDiedPatch), typeDiscriminator: "enemy_died")]
[JsonDerivedType(typeof(JournalStatePatch), typeDiscriminator: "journal_state")]
[JsonDerivedType(typeof(ServerShutdownPatch), typeDiscriminator: "shutdown")]
[JsonDerivedType(typeof(ClientExitPatch), typeDiscriminator: "client_exit")]
public interface IGamePatch
{
    // Metoda wykonywana po stronie klienta
    void Apply(ClientGameState state);
}
