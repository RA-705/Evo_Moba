using System;
using System.Collections.Generic;
using Evo.Core.ECS;

namespace Evo.MOBA.Items;

public struct BattlePassReward
{
    public int Level;
    public CurrencyType Currency;
    public int Amount;
    public int ShopEntryId;
    public bool IsPremium;
}

public struct BattlePassComponent : IComponent
{
    public int CurrentLevel;
    public int CurrentXp;
    public int XpToNextLevel;
    public bool IsPremium;
    public bool[] ClaimedRewards;
    public DateTime SeasonEnd;
}

public static class BattlePassService
{
    public const int MaxLevel = 30;
    public const int BaseXpPerLevel = 1000;
    public const float XpScaling = 1.1f;

    public static readonly BattlePassReward[] Rewards = new BattlePassReward[]
    {
        new() { Level = 1, Currency = CurrencyType.BattlePoints, Amount = 500, IsPremium = false },
        new() { Level = 1, Currency = CurrencyType.Diamonds, Amount = 50, IsPremium = true },

        new() { Level = 2, Currency = CurrencyType.Fragments, Amount = 20, IsPremium = false },
        new() { Level = 2, ShopEntryId = 2004, IsPremium = true },

        new() { Level = 3, Currency = CurrencyType.BattlePoints, Amount = 800, IsPremium = false },
        new() { Level = 3, Currency = CurrencyType.Tickets, Amount = 10, IsPremium = true },

        new() { Level = 5, ShopEntryId = 4001, IsPremium = false },
        new() { Level = 5, ShopEntryId = 2001, IsPremium = true },

        new() { Level = 7, Currency = CurrencyType.BattlePoints, Amount = 1200, IsPremium = false },
        new() { Level = 7, Currency = CurrencyType.Diamonds, Amount = 100, IsPremium = true },

        new() { Level = 10, ShopEntryId = 4002, IsPremium = false },
        new() { Level = 10, ShopEntryId = 2002, IsPremium = true },

        new() { Level = 15, Currency = CurrencyType.Fragments, Amount = 50, IsPremium = false },
        new() { Level = 15, ShopEntryId = 2003, IsPremium = true },

        new() { Level = 20, Currency = CurrencyType.BattlePoints, Amount = 3000, IsPremium = false },
        new() { Level = 20, Currency = CurrencyType.Diamonds, Amount = 300, IsPremium = true },

        new() { Level = 25, Currency = CurrencyType.Tickets, Amount = 50, IsPremium = false },
        new() { Level = 25, ShopEntryId = 2003, IsPremium = true },

        new() { Level = 30, Currency = CurrencyType.BattlePoints, Amount = 10000, IsPremium = false },
        new() { Level = 30, ShopEntryId = 2003, IsPremium = true },
    };

    public static void AddXp(World world, EntityId playerId, int xp)
    {
        if (!world.TryGetComponent<BattlePassComponent>(playerId, out var bp))
            return;

        bp.CurrentXp += xp;

        while (bp.CurrentXp >= bp.XpToNextLevel && bp.CurrentLevel < MaxLevel)
        {
            bp.CurrentXp -= bp.XpToNextLevel;
            bp.CurrentLevel++;
            bp.XpToNextLevel = (int)(BaseXpPerLevel * MathF.Pow(XpScaling, bp.CurrentLevel - 1));
        }

        world.SetComponent(playerId, bp);
    }

    public static bool ClaimReward(World world, EntityId playerId, int level)
    {
        if (!world.TryGetComponent<BattlePassComponent>(playerId, out var bp))
            return false;

        if (level < 1 || level > bp.CurrentLevel)
            return false;

        int idx = level - 1;
        if (idx < 0 || idx >= bp.ClaimedRewards.Length)
            return false;

        if (bp.ClaimedRewards[idx])
            return false;

        bp.ClaimedRewards[idx] = true;
        world.SetComponent(playerId, bp);

        foreach (var reward in Rewards)
        {
            if (reward.Level != level) continue;
            if (reward.IsPremium && !bp.IsPremium) continue;

            if (reward.ShopEntryId > 0)
            {
                // Item reward — add to inventory
                if (world.TryGetComponent<InventoryComponent>(playerId, out var inv))
                {
                    inv.TryAddItem(reward.ShopEntryId);
                    world.SetComponent(playerId, inv);
                }
            }
            else if (reward.Amount > 0)
            {
                if (world.TryGetComponent<CurrencyComponent>(playerId, out var currency))
                {
                    currency.Add(reward.Currency, reward.Amount);
                    world.SetComponent(playerId, currency);
                }
            }
        }

        return true;
    }
}
