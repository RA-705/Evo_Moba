using System;

namespace MateOS.Core;

public sealed class GoapGoal
{
    public string Name { get; }
    public Func<bool> IsSatisfied { get; }
    public int Priority { get; }

    public GoapGoal(string name, Func<bool> isSatisfied, int priority)
    {
        Name = name;
        IsSatisfied = isSatisfied;
        Priority = priority;
    }
}
