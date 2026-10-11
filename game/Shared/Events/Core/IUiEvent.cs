using System.Text.Json.Serialization;
using game.Models.Common.Events;
using game.Models.Common.Events.Noise;
using game.Shared.Events.Combat;
using game.Shared.Events.UI;

namespace game.Shared.Events.Core;

[JsonDerivedType(typeof(AttackDamageEvent), typeDiscriminator: "AttackDamage")]
[JsonDerivedType(typeof(DeathEvent), typeDiscriminator: "Death")]
[JsonDerivedType(typeof(DefenseDamageEvent), typeDiscriminator: "DefenseDamage")]
[JsonDerivedType(typeof(GameMessageEvent), typeDiscriminator: "GameMessage")]
[JsonDerivedType(typeof(GameStartedEvent), typeDiscriminator: "GameStarted")]
[JsonDerivedType(typeof(InvalidAttackKeyEvent), typeDiscriminator: "InvalidAttackKey")]
[JsonDerivedType(typeof(ItemCannotBeEquippedEvent), typeDiscriminator: "ItemCannotBeEquipped")]
[JsonDerivedType(typeof(ItemEquippedEvent), typeDiscriminator: "ItemEquipped")]
[JsonDerivedType(typeof(ItemEquippedInHandEvent), typeDiscriminator: "ItemEquippedInHand")]
[JsonDerivedType(typeof(JournalClosedEvent), typeDiscriminator: "JournalClosed")]
[JsonDerivedType(typeof(JournalOpenedEvent), typeDiscriminator: "JournalOpened")]
[JsonDerivedType(typeof(NoiseHeardEvent), typeDiscriminator: "NoiseHeard")]
[JsonDerivedType(typeof(OutOfBoundsEvent), typeDiscriminator: "OutOfBounds")]
[JsonDerivedType(typeof(UnknownKeyEvent), typeDiscriminator: "UnknownKey")]
[JsonDerivedType(typeof(WallCollisionEvent), typeDiscriminator: "WallCollision")]
[JsonDerivedType(typeof(PlayerCollisionEvent), typeDiscriminator: "PlayerCollision")]
public interface IUiEvent : IGameEvent
{
}
