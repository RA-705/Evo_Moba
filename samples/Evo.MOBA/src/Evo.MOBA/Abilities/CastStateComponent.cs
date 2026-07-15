using Evo.Core.ECS;
using Evo.Shared.Math;

namespace Evo.MOBA.Abilities;

public struct CastTarget
{
    public bool IsValid;
    public bool IsUnit;
    public EntityId TargetUnitId;
    public EvoVector3 TargetPoint;

    public static CastTarget Unit(EntityId id) =>
        new() { IsValid = true, IsUnit = true, TargetUnitId = id };

    public static CastTarget Point(EvoVector3 pos) =>
        new() { IsValid = true, IsUnit = false, TargetPoint = pos };
}

public struct ActiveCastState
{
    public int AbilityDataId;
    public CastTarget Target;
    public float TotalCastTime;
    public float ElapsedTime;

    public readonly float Progress => ElapsedTime / TotalCastTime;
    public readonly bool IsComplete => ElapsedTime >= TotalCastTime;
}

public struct CastStateComponent : IComponent
{
    public ActiveCastState State;

    public readonly bool IsCasting => State.Target.IsValid;
}
