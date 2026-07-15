using System;
using System.Collections.Generic;
using Evo.Core.ECS;

namespace Evo.MOBA.Items;

public struct DailyDealsComponent : IComponent
{
    public int[] DealEntryIds;
    public float[] DealDiscounts;
    public DateTime ResetTime;
}

public struct WeeklyDealsComponent : IComponent
{
    public int[] DealEntryIds;
    public float[] DealDiscounts;
    public DateTime ResetTime;
}

public sealed class RotatingDealsSystem : ISystem
{
    private readonly Random _rng = new();
    private readonly int[] _allShopIds;

    public RotatingDealsSystem()
    {
        var ids = new List<int>();
        foreach (var entry in ShopCatalog.Database.Values)
        {
            if (entry.Category != ShopCategory.Items)
                ids.Add(entry.Id);
        }
        _allShopIds = ids.ToArray();
    }

    public void OnTick(World world, float deltaTime)
    {
        var now = DateTime.UtcNow;

        foreach (var id in world.GetEntityIds<DailyDealsComponent>())
        {
            ref var deals = ref world.GetComponent<DailyDealsComponent>(id);

            if (now >= deals.ResetTime)
            {
                GenerateDailyDeals(ref deals, now);
                world.SetComponent(id, deals);
            }
        }

        foreach (var id in world.GetEntityIds<WeeklyDealsComponent>())
        {
            ref var deals = ref world.GetComponent<WeeklyDealsComponent>(id);

            if (now >= deals.ResetTime)
            {
                GenerateWeeklyDeals(ref deals, now);
                world.SetComponent(id, deals);
            }
        }
    }

    private void GenerateDailyDeals(ref DailyDealsComponent deals, DateTime now)
    {
        int count = 4;
        deals.DealEntryIds = new int[count];
        deals.DealDiscounts = new float[count];
        deals.ResetTime = now.Date.AddDays(1);

        var shuffled = (int[])_allShopIds.Clone();
        Shuffle(shuffled);

        for (int i = 0; i < count && i < shuffled.Length; i++)
        {
            deals.DealEntryIds[i] = shuffled[i];
            deals.DealDiscounts[i] = _rng.Next(10, 40) / 100f;
        }
    }

    private void GenerateWeeklyDeals(ref WeeklyDealsComponent deals, DateTime now)
    {
        int count = 6;
        deals.DealEntryIds = new int[count];
        deals.DealDiscounts = new float[count];
        deals.ResetTime = now.Date.AddDays((7 - (int)now.DayOfWeek) % 7);

        var shuffled = (int[])_allShopIds.Clone();
        Shuffle(shuffled);

        for (int i = 0; i < count && i < shuffled.Length; i++)
        {
            deals.DealEntryIds[i] = shuffled[i];
            deals.DealDiscounts[i] = _rng.Next(20, 50) / 100f;
        }
    }

    private void Shuffle(int[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = _rng.Next(i + 1);
            (array[i], array[j]) = (array[j], array[i]);
        }
    }
}
