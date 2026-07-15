using System;
using System.Collections.Generic;
using Evo.Core.ECS;

namespace Evo.MOBA.Items;

public enum GachaPool : byte
{
    Standard,
    Premium,
    Event,
}

public struct GachaPityComponent : IComponent
{
    public int StandardPulls;
    public int PremiumPulls;
    public int EventPulls;
    public const int PityThreshold = 50;
}

public struct GachaResult
{
    public int EntryId;
    public ItemRarity Rarity;
    public bool IsPity;
}

public static class GachaService
{
    private static readonly Random _rng = new();

    private static readonly Dictionary<GachaPool, (int[] entryIds, float[] weights)> Pools = new()
    {
        [GachaPool.Standard] = new (
            new[] { 2001, 2002, 2003, 2004, 4001, 4002 },
            new[] { 0.35f, 0.20f, 0.10f, 0.25f, 0.05f, 0.05f }
        ),
        [GachaPool.Premium] = new (
            new[] { 2002, 2003, 2004, 4001, 4002 },
            new[] { 0.30f, 0.15f, 0.35f, 0.10f, 0.10f }
        ),
        [GachaPool.Event] = new (
            new[] { 2003, 2004 },
            new[] { 0.70f, 0.30f }
        ),
    };

    public static GachaResult Pull(World world, EntityId playerId, GachaPool pool, CurrencyType costCurrency, int costAmount)
    {
        if (!world.TryGetComponent<CurrencyComponent>(playerId, out var currency))
            return new GachaResult { EntryId = -1 };

        if (!currency.TrySpend(costCurrency, costAmount))
            return new GachaResult { EntryId = -1 };

        world.SetComponent(playerId, currency);

        if (!world.TryGetComponent<GachaPityComponent>(playerId, out var pity))
            pity = new GachaPityComponent();

        ref var pityRef = ref world.GetComponent<GachaPityComponent>(playerId);

        int pulls = pool switch
        {
            GachaPool.Standard => pityRef.StandardPulls,
            GachaPool.Premium => pityRef.PremiumPulls,
            GachaPool.Event => pityRef.EventPulls,
            _ => 0,
        };

        bool isPity = pulls >= GachaPityComponent.PityThreshold;

        int entryId;
        ItemRarity rarity;

        if (isPity)
        {
            var (entryIds, _) = Pools[pool];
            entryId = entryIds[0];
            rarity = ShopCatalog.Database[entryId].Rarity;

            switch (pool)
            {
                case GachaPool.Standard: pityRef.StandardPulls = 0; break;
                case GachaPool.Premium: pityRef.PremiumPulls = 0; break;
                case GachaPool.Event: pityRef.EventPulls = 0; break;
            }
        }
        else
        {
            var (entryIds, weights) = Pools[pool];
            entryId = WeightedPick(entryIds, weights);
            rarity = ShopCatalog.Database[entryId].Rarity;

            switch (pool)
            {
                case GachaPool.Standard: pityRef.StandardPulls++; break;
                case GachaPool.Premium: pityRef.PremiumPulls++; break;
                case GachaPool.Event: pityRef.EventPulls++; break;
            }
        }

        world.SetComponent(playerId, pityRef);

        return new GachaResult
        {
            EntryId = entryId,
            Rarity = rarity,
            IsPity = isPity,
        };
    }

    private static int WeightedPick(int[] ids, float[] weights)
    {
        float total = 0;
        foreach (var w in weights) total += w;

        float roll = (float)_rng.NextDouble() * total;
        float cumulative = 0;

        for (int i = 0; i < ids.Length; i++)
        {
            cumulative += weights[i];
            if (roll <= cumulative)
                return ids[i];
        }

        return ids[^1];
    }
}
