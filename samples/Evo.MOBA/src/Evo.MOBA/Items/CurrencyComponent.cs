using Evo.Core.ECS;

namespace Evo.MOBA.Items;

public enum CurrencyType : byte
{
    BattlePoints = 0,
    Diamonds = 1,
    Fragments = 2,
    Tickets = 3,
}

public struct CurrencyComponent : IComponent
{
    public int BattlePoints;
    public int Diamonds;
    public int Fragments;
    public int Tickets;

    public int Get(CurrencyType type) => type switch
    {
        CurrencyType.BattlePoints => BattlePoints,
        CurrencyType.Diamonds => Diamonds,
        CurrencyType.Fragments => Fragments,
        CurrencyType.Tickets => Tickets,
        _ => 0,
    };

    public void Add(CurrencyType type, int amount)
    {
        switch (type)
        {
            case CurrencyType.BattlePoints: BattlePoints += amount; break;
            case CurrencyType.Diamonds: Diamonds += amount; break;
            case CurrencyType.Fragments: Fragments += amount; break;
            case CurrencyType.Tickets: Tickets += amount; break;
        }
    }

    public bool TrySpend(CurrencyType type, int amount)
    {
        if (Get(type) < amount) return false;
        switch (type)
        {
            case CurrencyType.BattlePoints: BattlePoints -= amount; break;
            case CurrencyType.Diamonds: Diamonds -= amount; break;
            case CurrencyType.Fragments: Fragments -= amount; break;
            case CurrencyType.Tickets: Tickets -= amount; break;
        }
        return true;
    }
}
