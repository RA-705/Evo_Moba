using System.Collections.Generic;
using MateOS.Core;

namespace Evo.MOBA.AI.MATE;

public static class MOBAGoals
{
    public static void PopulateGoals(MOBAPerception p, MOBAMemory memory, List<GoapGoal> goals)
    {
        goals.Clear();

        goals.Add(new GoapGoal(
            "Survive",
            () => p.SelfHP <= 0,
            priority: 100
        ));

        goals.Add(new GoapGoal(
            "DefendBase",
            () => !HasThreatNearBase(p),
            priority: 90
        ));

        goals.Add(new GoapGoal(
            "Retreat",
            () => !ShouldRetreat(p, memory),
            priority: 85
        ));

        goals.Add(new GoapGoal(
            "KillEnemy",
            () => !p.VisibleEnemies.Exists(e => e.Health <= 0),
            priority: 80
        ));

        goals.Add(new GoapGoal(
            "PushLane",
            () => !IsLaneClear(p),
            priority: 70
        ));

        goals.Add(new GoapGoal(
            "Farm",
            () => !HasCreepNearby(p),
            priority: 50
        ));

        goals.Add(new GoapGoal(
            "Patrol",
            () => false,
            priority: 10
        ));
    }

    public static List<GoapGoal> CreateGoals(MOBAPerception p, MOBAMemory memory)
    {
        var goals = new List<GoapGoal>();
        PopulateGoals(p, memory, goals);
        return goals;
    }

    private static bool HasThreatNearBase(MOBAPerception p)
    {
        float baseRangeSq = 15f * 15f;
        foreach (var e in p.VisibleEnemies)
        {
            if (e.DistanceSq <= baseRangeSq)
                return true;
        }
        return false;
    }

    private static bool ShouldRetreat(MOBAPerception p, MOBAMemory memory)
    {
        if (p.SelfHealthRatio > 0.35f) return false;
        if (p.IsOutnumbered()) return true;
        if (p.CountEnemiesInRange(8f * 8f) > 0 && p.SelfHealthRatio < 0.25f) return true;
        return false;
    }

    private static bool IsLaneClear(MOBAPerception p)
    {
        foreach (var e in p.VisibleEnemies)
        {
            if (e.Type == MOBAEntityType.Creep && e.TeamId != p.SelfTeam)
                return false;
        }
        return true;
    }

    private static bool HasCreepNearby(MOBAPerception p)
    {
        float farmRangeSq = 5f * 5f;
        foreach (var e in p.VisibleEnemies)
        {
            if (e.Type == MOBAEntityType.Creep && e.DistanceSq <= farmRangeSq)
                return true;
        }
        return false;
    }
}

public sealed class MOBAMemory
{
    private float _lastRetreatTime;
    private float _lastAttackTime;
    private float _lastFarmTime;
    private int _deaths;
    private int _kills;

    public float TimeSinceRetreat(float gameTime) => gameTime - _lastRetreatTime;
    public float TimeSinceAttack(float gameTime) => gameTime - _lastAttackTime;
    public float TimeSinceFarm(float gameTime) => gameTime - _lastFarmTime;
    public int Deaths => _deaths;
    public int Kills => _kills;

    public void RecordRetreat(float time) => _lastRetreatTime = time;
    public void RecordAttack(float time) => _lastAttackTime = time;
    public void RecordFarm(float time) => _lastFarmTime = time;
    public void RecordDeath() => _deaths++;
    public void RecordKill() => _kills++;
}
