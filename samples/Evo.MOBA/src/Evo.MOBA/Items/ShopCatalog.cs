using System;
using System.Collections.Generic;

namespace Evo.MOBA.Items;

public enum ShopCategory : byte
{
    Heroes,
    Skins,
    Items,
    Emblems,
    Daily,
    Weekly,
}

public enum ItemRarity : byte
{
    Common,
    Rare,
    Epic,
    Legend,
}

public readonly struct ShopEntry
{
    public int Id { get; init; }
    public string Name { get; init; }
    public ShopCategory Category { get; init; }
    public ItemRarity Rarity { get; init; }
    public CurrencyType PriceCurrency { get; init; }
    public int Price { get; init; }
    public int OriginalPrice { get; init; }
    public bool IsLimited { get; init; }
    public DateTime? LimitedUntil { get; init; }
    public int UnlockLevel { get; init; }
}

public static class ShopCatalog
{
    public static readonly Dictionary<int, ShopEntry> Database = new()
    {
        [1001] = new ShopEntry { Id = 1001, Name = "Warden", Category = ShopCategory.Heroes, Rarity = ItemRarity.Common, PriceCurrency = CurrencyType.BattlePoints, Price = 2000, OriginalPrice = 2000, UnlockLevel = 1 },
        [1002] = new ShopEntry { Id = 1002, Name = "Pyromancer", Category = ShopCategory.Heroes, Rarity = ItemRarity.Rare, PriceCurrency = CurrencyType.BattlePoints, Price = 4000, OriginalPrice = 4000, UnlockLevel = 1 },
        [1003] = new ShopEntry { Id = 1003, Name = "Blade", Category = ShopCategory.Heroes, Rarity = ItemRarity.Epic, PriceCurrency = CurrencyType.BattlePoints, Price = 6000, OriginalPrice = 6000, UnlockLevel = 5 },

        [2001] = new ShopEntry { Id = 2001, Name = "Warden - Dark Knight", Category = ShopCategory.Skins, Rarity = ItemRarity.Rare, PriceCurrency = CurrencyType.Diamonds, Price = 200, OriginalPrice = 200, UnlockLevel = 1 },
        [2002] = new ShopEntry { Id = 2002, Name = "Pyromancer - Inferno", Category = ShopCategory.Skins, Rarity = ItemRarity.Epic, PriceCurrency = CurrencyType.Diamonds, Price = 500, OriginalPrice = 500, UnlockLevel = 1 },
        [2003] = new ShopEntry { Id = 2003, Name = "Blade - Shadow", Category = ShopCategory.Skins, Rarity = ItemRarity.Legend, PriceCurrency = CurrencyType.Diamonds, Price = 1000, OriginalPrice = 1000, UnlockLevel = 1 },
        [2004] = new ShopEntry { Id = 2004, Name = "Warden - Classic", Category = ShopCategory.Skins, Rarity = ItemRarity.Common, PriceCurrency = CurrencyType.Fragments, Price = 50, OriginalPrice = 50, UnlockLevel = 1 },

        [3001] = new ShopEntry { Id = 3001, Name = "Health Potion", Category = ShopCategory.Items, Rarity = ItemRarity.Common, PriceCurrency = CurrencyType.BattlePoints, Price = 50, OriginalPrice = 50, UnlockLevel = 1 },
        [3002] = new ShopEntry { Id = 3002, Name = "Long Sword", Category = ShopCategory.Items, Rarity = ItemRarity.Common, PriceCurrency = CurrencyType.BattlePoints, Price = 350, OriginalPrice = 350, UnlockLevel = 1 },
        [3003] = new ShopEntry { Id = 3003, Name = "Chain Vest", Category = ShopCategory.Items, Rarity = ItemRarity.Common, PriceCurrency = CurrencyType.BattlePoints, Price = 400, OriginalPrice = 400, UnlockLevel = 1 },

        [4001] = new ShopEntry { Id = 4001, Name = "Attack Emblem", Category = ShopCategory.Emblems, Rarity = ItemRarity.Rare, PriceCurrency = CurrencyType.Fragments, Price = 100, OriginalPrice = 100, UnlockLevel = 3 },
        [4002] = new ShopEntry { Id = 4002, Name = "Defense Emblem", Category = ShopCategory.Emblems, Rarity = ItemRarity.Rare, PriceCurrency = CurrencyType.Fragments, Price = 100, OriginalPrice = 100, UnlockLevel = 3 },
    };
}
