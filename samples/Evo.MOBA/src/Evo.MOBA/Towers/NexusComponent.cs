using Evo.Core.ECS;
using Evo.MOBA.Combat;
using Evo.MOBA.Systems;
using Evo.Shared.Math;

namespace Evo.MOBA.Towers;

public struct NexusComponent : IComponent
{
    public int TeamId;
}

public sealed class NexusFactory
{
    public static Entity CreateNexus(World world, int teamId, EvoVector3 position)
    {
        var entity = world.Create();

        world.AddComponent(entity, new PositionComponent { Value = position });
        world.AddComponent(entity, new TeamComponent { TeamId = teamId });
        world.AddComponent(entity, new HealthComponent { Current = 1000, Max = 1000 });
        world.AddComponent(entity, new ArmorComponent { BaseArmor = 30 });
        world.AddComponent(entity, new NexusComponent { TeamId = teamId });

        return entity;
    }
}
