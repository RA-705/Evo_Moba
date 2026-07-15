using Evo.Core.ECS;
using Evo.MOBA.Systems;

namespace Evo.MOBA.AI.MATE;

public static class EntityHelper
{
    public static EntityId? FindEntityId(World world, int rawId)
    {
        foreach (var id in world.GetEntityIds<PositionComponent>())
        {
            if (id.Value == rawId)
                return id;
        }
        return null;
    }
}
