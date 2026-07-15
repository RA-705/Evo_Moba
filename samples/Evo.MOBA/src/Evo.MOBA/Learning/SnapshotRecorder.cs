using System;
using System.Collections.Generic;
using Evo.Core.ECS;
using Evo.MOBA.AI.MATE;
using Evo.MOBA.Combat;
using Evo.MOBA.Systems;
using Evo.Shared.Math;

namespace Evo.MOBA.Learning;

public readonly struct TrajectoryStep
{
    public int Tick { get; }
    public EntityId EntityId { get; }
    public float PosX { get; }
    public float PosZ { get; }
    public float HealthPercent { get; }
    public int TeamId { get; }
    public int ActionId { get; }
    public float Reward { get; }

    public TrajectoryStep(int tick, EntityId entityId, float posX, float posZ,
        float healthPercent, int teamId, int actionId, float reward)
    {
        Tick = tick;
        EntityId = entityId;
        PosX = posX;
        PosZ = posZ;
        HealthPercent = healthPercent;
        TeamId = teamId;
        ActionId = actionId;
        Reward = reward;
    }
}

public sealed class SnapshotRecorder : ISystem
{
    private readonly List<TrajectoryStep> _steps = new();
    private readonly Dictionary<EntityId, float> _lastHealth = new();
    private int _tick;

    public IReadOnlyList<TrajectoryStep> Steps => _steps;

    public void OnTick(World world, float deltaTime)
    {
        _tick++;

        if (_tick % 10 != 0)
            return;

        foreach (var id in world.GetEntityIds<HealthComponent>())
        {
            if (!world.TryGetComponent<HealthComponent>(id, out var health) ||
                !world.TryGetComponent<PositionComponent>(id, out var pos) ||
                !world.TryGetComponent<TeamComponent>(id, out var team))
                continue;

            float healthPct = health.Max > 0f ? health.Current / health.Max : 0f;
            float reward = 0f;

            if (_lastHealth.TryGetValue(id, out var lastHp))
            {
                float delta = health.Current - lastHp;
                if (delta < 0f)
                    reward = delta / health.Max;
            }

            _lastHealth[id] = health.Current;

            int actionId = 0;
            if (world.TryGetComponent<MateBrainComponent>(id, out var brain))
                actionId = brain.IsRetreating ? 1 : 0;

            _steps.Add(new TrajectoryStep(
                _tick, id,
                pos.Value.X, pos.Value.Z,
                healthPct, team.TeamId,
                actionId, reward
            ));
        }
    }

    public void ExportCsv(string filePath)
    {
        using var writer = new System.IO.StreamWriter(filePath);
        writer.WriteLine("tick,entity_id,pos_x,pos_z,health_pct,team_id,action_id,reward");

        foreach (var step in _steps)
        {
            writer.WriteLine(
                $"{step.Tick},{step.EntityId.Value}," +
                $"{step.PosX:F3},{step.PosZ:F3}," +
                $"{step.HealthPercent:F3},{step.TeamId}," +
                $"{step.ActionId},{step.Reward:F4}"
            );
        }
    }

    public void Clear()
    {
        _steps.Clear();
        _lastHealth.Clear();
        _tick = 0;
    }
}
