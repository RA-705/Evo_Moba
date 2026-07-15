using Evo.Core.ECS;

namespace Evo.Gameplay;

public static class EntityFactory
{
    private static readonly Dictionary<EntityType, ComponentMask> Presets = new()
    {
        { EntityType.Hero, ComponentMask.Transform | ComponentMask.Health },
        { EntityType.Projectile, ComponentMask.Transform },
    };

    public static GameEntity Create(EntityType type, byte teamId)
    {
        var id = EntityId.Next();
        var entity = new GameEntity(id);
        var mask = Presets[type];
        var components = new List<EntityComponent>();

        if ((mask & ComponentMask.Transform) != 0)
            components.Add(new TransformComponent());

        if ((mask & ComponentMask.Health) != 0)
            components.Add(new HealthComponent());

        entity.Initialize(teamId, null, components);
        return entity;
    }
}
