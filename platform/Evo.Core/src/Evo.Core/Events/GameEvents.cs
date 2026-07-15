using Evo.Core.ECS;

namespace Evo.Core.Events;

public readonly record struct PositionUpdatedEvent(EntityId EntityId, float X, float Y, float Z) : IGameEvent;

public readonly record struct DamageDealtEvent(EntityId AttackerId, EntityId VictimId, float Damage) : IGameEvent;

public readonly record struct EntityDiedEvent(EntityId VictimId, EntityId KillerId) : IGameEvent;
