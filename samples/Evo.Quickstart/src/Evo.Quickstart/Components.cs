using Evo.Core.ECS;
using Evo.Shared.Math;

namespace Evo.Quickstart;

public struct Position : IComponent
{
    public EvoVector3 Value { get; set; }
}

public struct Velocity : IComponent
{
    public EvoVector3 Value { get; set; }
}
