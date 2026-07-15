using Evo.Core.ECS;

namespace Evo.MOBA.Combat;

public struct DamageDealtEventComponent : IComponent
{
    public EntityId SourceId;
    public EntityId TargetId;
    public float RawDamage;
    public float FinalDamage;
    public DamageType DamageType;
}

public sealed class DamageEventSystem : ISystem
{
    public void OnTick(World world, float deltaTime)
    {
        foreach (var id in world.GetEntityIds<DamageDealtEventComponent>())
        {
            world.RemoveComponent<DamageDealtEventComponent>(id);
            world.Destroy(new Entity(id));
        }
    }
}
