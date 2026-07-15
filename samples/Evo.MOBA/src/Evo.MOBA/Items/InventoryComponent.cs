using Evo.Core.ECS;

namespace Evo.MOBA.Items;

public struct InventoryComponent : IComponent
{
    public const int MaxSlots = 6;

    public int[] ItemIds;
    public int Gold;

    public InventoryComponent(int startingGold)
    {
        ItemIds = new int[MaxSlots];
        Gold = startingGold;
    }

    public bool TryAddItem(int itemId)
    {
        for (int i = 0; i < MaxSlots; i++)
        {
            if (ItemIds[i] == 0)
            {
                ItemIds[i] = itemId;
                return true;
            }
        }
        return false;
    }

    public void RemoveItem(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < MaxSlots)
            ItemIds[slotIndex] = 0;
    }

    public int TotalBonusHp()
    {
        int sum = 0;
        for (int i = 0; i < MaxSlots; i++)
        {
            if (ItemIds[i] > 0 && ItemRegistry.Database.TryGetValue(ItemIds[i], out var item))
                sum += (int)item.BonusHp;
        }
        return sum;
    }

    public float TotalBonusDamage()
    {
        float sum = 0;
        for (int i = 0; i < MaxSlots; i++)
        {
            if (ItemIds[i] > 0 && ItemRegistry.Database.TryGetValue(ItemIds[i], out var item))
                sum += item.BonusDamage;
        }
        return sum;
    }

    public float TotalBonusArmor()
    {
        float sum = 0;
        for (int i = 0; i < MaxSlots; i++)
        {
            if (ItemIds[i] > 0 && ItemRegistry.Database.TryGetValue(ItemIds[i], out var item))
                sum += item.BonusArmor;
        }
        return sum;
    }

    public float TotalBonusMagicResist()
    {
        float sum = 0;
        for (int i = 0; i < MaxSlots; i++)
        {
            if (ItemIds[i] > 0 && ItemRegistry.Database.TryGetValue(ItemIds[i], out var item))
                sum += item.BonusMagicResist;
        }
        return sum;
    }

    public float TotalBonusSpeed()
    {
        float sum = 0;
        for (int i = 0; i < MaxSlots; i++)
        {
            if (ItemIds[i] > 0 && ItemRegistry.Database.TryGetValue(ItemIds[i], out var item))
                sum += item.BonusSpeed;
        }
        return sum;
    }
}
