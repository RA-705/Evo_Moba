using Evo.Core.ECS;

namespace Evo.MOBA.AI.MATE;

public struct MOBAComponent : IComponent
{
    public MOBACognitiveCore Core;

    public MOBAComponent()
    {
        Core = new MOBACognitiveCore();
    }
}
