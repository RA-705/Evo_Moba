using System.Collections.Generic;
using Evo.Core.ECS;
using Evo.MOBA.Combat;
using Evo.MOBA.Systems;
using Evo.Shared.Math;

namespace Evo.MOBA.AI.MATE;

public enum TeamGoalType : byte
{
    Farm,
    PushMid,
    SplitPush,
    Defend,
    Group,
    Roshan,
}

public struct TeamGoalComponent : IComponent
{
    public TeamGoalType Goal;
    public EntityId TargetId;
    public EvoVector3 TargetPosition;
    public float PriorityScore;
}

public sealed class TeamPlannerSystem : ISystem
{
    private const float MidLaneCenterX = 50f;
    private const float MidLaneCenterZ = 50f;
    private const float RoshanPitX = 35f;
    private const float RoshanPitZ = 35f;
    private const float BaseX = 0f;
    private const float BaseZ = 0f;

    private int _tickCounter;
    private readonly List<EntityId> _entityBuffer = new();

    public void OnTick(World world, float deltaTime)
    {
        _tickCounter++;
        if (_tickCounter % 30 != 0)
            return;

        _entityBuffer.Clear();
        foreach (var id in world.GetEntityIds<TeamGoalComponent>())
            _entityBuffer.Add(id);

        foreach (var id in _entityBuffer)
        {
            if (!world.TryGetComponent<TeamGoalComponent>(id, out var goal) ||
                !world.TryGetComponent<TeamComponent>(id, out var team))
                continue;

            goal.Goal = DecideTeamGoal(world, team.TeamId, id);
            world.SetComponent(id, goal);
        }
    }

    private static TeamGoalType DecideTeamGoal(World world, int teamId, EntityId selfId)
    {
        int allyCount = 0, enemyCount = 0;
        float allyHealthSum = 0f;
        bool enemyVisible = false;

        foreach (var eid in world.GetEntityIds<TeamComponent>())
        {
            if (!world.TryGetComponent<TeamComponent>(eid, out var t))
                continue;

            if (t.TeamId == teamId)
            {
                allyCount++;
                if (world.TryGetComponent<HealthComponent>(eid, out var h))
                    allyHealthSum += h.Current / h.Max;
            }
            else
            {
                enemyCount++;
                if (world.TryGetComponent<PositionComponent>(eid, out _))
                    enemyVisible = true;
            }
        }

        float avgHealth = allyCount > 0 ? allyHealthSum / allyCount : 0f;
        bool outnumbered = allyCount < enemyCount;

        if (outnumbered && avgHealth < 0.4f)
            return TeamGoalType.Defend;

        if (enemyCount <= 1 && allyCount >= 3 && avgHealth > 0.5f)
            return TeamGoalType.Roshan;

        if (!enemyVisible && allyCount >= enemyCount)
            return TeamGoalType.PushMid;

        if (allyCount >= enemyCount && avgHealth > 0.6f)
            return TeamGoalType.Group;

        if (allyCount >= enemyCount + 2)
            return TeamGoalType.SplitPush;

        return TeamGoalType.Farm;
    }
}
