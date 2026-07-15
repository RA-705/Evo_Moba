using Evo.Core.ECS;
using Evo.Core.Events;
using Evo.Core.Simulation;

namespace Evo.Gameplay.Systems;

public sealed class MovementSystem : ISimulationSystem
{
    private readonly Dictionary<EntityId, GameEntity> _entities = new();
    private readonly Dictionary<EntityId, (float X, float Z)> _moveRequests = new();
    private readonly IEventBus _eventBus;

    public MovementSystem(IEventBus eventBus) =>
        _eventBus = eventBus;

    public void RegisterEntity(GameEntity entity)
    {
        _entities[entity.Id] = entity;
    }

    public void UnregisterEntity(EntityId id)
    {
        _entities.Remove(id);
        _moveRequests.Remove(id);
    }

    public void MoveTo(EntityId id, float x, float z)
    {
        _moveRequests[id] = (x, z);
    }

    public void Stop(EntityId id)
    {
        _moveRequests.Remove(id);
    }

    public void Update(float deltaTime)
    {
        foreach (var (id, (destX, destZ)) in _moveRequests)
        {
            if (!_entities.TryGetValue(id, out var entity))
                continue;

            if (entity.TryGetComponent<HealthComponent>(out var health) && health.CurrentHealth <= 0f)
                continue;

            var transform = entity.GetComponent<TransformComponent>();
            var dx = destX - transform.X;
            var dz = destZ - transform.Z;
            var dist = MathF.Sqrt(dx * dx + dz * dz);

            if (dist < 0.01f)
            {
                Stop(id);
                continue;
            }

            var speed = MathF.Sqrt(
                transform.VelocityX * transform.VelocityX +
                transform.VelocityY * transform.VelocityY +
                transform.VelocityZ * transform.VelocityZ);

            if (speed < 0.001f)
                speed = 5.0f;

            var step = speed * deltaTime;
            if (step >= dist)
            {
                transform.X = destX;
                transform.Z = destZ;
                Stop(id);
            }
            else
            {
                var t = step / dist;
                transform.X += dx * t;
                transform.Z += dz * t;
            }

            _eventBus.Publish(new PositionUpdatedEvent(id, transform.X, transform.Y, transform.Z));
        }
    }
}
