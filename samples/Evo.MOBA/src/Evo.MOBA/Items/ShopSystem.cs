using Evo.Core.ECS;
using Evo.MOBA.Combat;

namespace Evo.MOBA.Items;

public struct BuyOrderComponent : IComponent
{
    public EntityId BuyerId;
    public int ItemId;
}

public sealed class ShopSystem : ISystem
{
    public void OnTick(World world, float deltaTime)
    {
        foreach (var id in world.GetEntityIds<BuyOrderComponent>())
        {
            ref var order = ref world.GetComponent<BuyOrderComponent>(id);

            if (!world.TryGetComponent<InventoryComponent>(order.BuyerId, out var inv))
                continue;

            if (!ItemRegistry.Database.TryGetValue(order.ItemId, out var item))
                continue;

            if (inv.Gold < item.Cost)
                continue;

            if (!inv.TryAddItem(order.ItemId))
                continue;

            inv.Gold -= item.Cost;
            world.SetComponent(order.BuyerId, inv);
            ApplyItemStats(world, order.BuyerId, item);

            world.RemoveComponent<BuyOrderComponent>(id);
        }
    }

    private static void ApplyItemStats(World world, EntityId entityId, ItemData item)
    {
        if (!world.TryGetComponent<HealthComponent>(entityId, out var health))
            return;

        health.Max += item.BonusHp;
        health.Current += item.BonusHp;
        world.SetComponent(entityId, health);
    }
}

public sealed class KillGoldSystem : ISystem
{
    public void OnTick(World world, float deltaTime)
    {
        foreach (var id in world.GetEntityIds<HealthComponent>())
        {
            if (!world.TryGetComponent<HealthComponent>(id, out var health) ||
                !world.TryGetComponent<TeamComponent>(id, out var team))
                continue;

            if (health.Current > 0f) continue;

            // Award gold to all alive enemies
            int goldAward = team.TeamId == 0 ? 50 : 50;

            foreach (var killerId in world.GetEntityIds<InventoryComponent>())
            {
                if (!world.TryGetComponent<TeamComponent>(killerId, out var killerTeam))
                    continue;

                if (killerTeam.TeamId == team.TeamId) continue;

                ref var inv = ref world.GetComponent<InventoryComponent>(killerId);
                inv.Gold += goldAward;
            }
        }
    }
}
