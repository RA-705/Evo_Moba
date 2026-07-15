using System;
using System.Collections.Generic;
using MateOS.Core;

namespace Evo.MOBA.AI.MATE;

public static class MOBAActions
{
    public static void PopulateActions(MOBAPerception p, MOBAMemory memory, List<GoapAction<MOBAPerception>> actions)
    {
        actions.Clear();
        actions.Add(CreateRetreat(p, memory));
        actions.Add(CreateAttack(p, memory));
        actions.Add(CreateCastAbility(p, memory));
        actions.Add(CreateFarm(p, memory));
        actions.Add(CreatePushLane(p, memory));
        actions.Add(CreateMoveToBase(p, memory));
        actions.Add(CreatePatrol(p, memory));
    }

    public static List<GoapAction<MOBAPerception>> CreateActions(MOBAPerception p, MOBAMemory memory)
    {
        var actions = new List<GoapAction<MOBAPerception>>();
        PopulateActions(p, memory, actions);
        return actions;
    }

    private static GoapAction<MOBAPerception> CreateRetreat(MOBAPerception p, MOBAMemory memory)
    {
        var action = new GoapAction<MOBAPerception>("Retreat", cost: 1f);
        action.Preconditions.Add(() => true);
        action.Effects.Add(() => { });
        action.ActionFactory = () =>
        {
            float baseX = p.SelfTeam == 0 ? 3f : 42f;
            return Intention<MOBAPerception>.Move(baseX, 0, 8f);
        };
        return action;
    }

    private static GoapAction<MOBAPerception> CreateAttack(MOBAPerception p, MOBAMemory memory)
    {
        var action = new GoapAction<MOBAPerception>("Attack", cost: 2f);
        action.Preconditions.Add(() => p.VisibleEnemies.Count > 0);
        action.Effects.Add(() => { });
        action.ActionFactory = () =>
        {
            var target = p.GetClosestEnemy();
            if (target.HasValue)
                return Intention<MOBAPerception>.Attack(target.Value.EntityId);
            return Intention<MOBAPerception>.Stop();
        };
        return action;
    }

    private static GoapAction<MOBAPerception> CreateCastAbility(MOBAPerception p, MOBAMemory memory)
    {
        var action = new GoapAction<MOBAPerception>("CastAbility", cost: 3f);
        action.Preconditions.Add(() => p.SelfManaRatio > 0.1f);
        action.Preconditions.Add(() => p.VisibleEnemies.Count > 0);
        action.Effects.Add(() => { });
        action.ActionFactory = () =>
        {
            var target = p.GetClosestEnemy();
            if (target.HasValue)
                return Intention<MOBAPerception>.Attack(target.Value.EntityId);
            return Intention<MOBAPerception>.Stop();
        };
        return action;
    }

    private static GoapAction<MOBAPerception> CreateFarm(MOBAPerception p, MOBAMemory memory)
    {
        var action = new GoapAction<MOBAPerception>("Farm", cost: 2f);
        action.Preconditions.Add(() =>
        {
            float farmRangeSq = 5f * 5f;
            foreach (var e in p.VisibleEnemies)
                if (e.Type == MOBAEntityType.Creep && e.DistanceSq <= farmRangeSq)
                    return true;
            return false;
        });
        action.Effects.Add(() => { });
        action.ActionFactory = () =>
        {
            float closestDist = float.MaxValue;
            int closestId = -1;
            foreach (var e in p.VisibleEnemies)
            {
                if (e.Type == MOBAEntityType.Creep && e.DistanceSq < closestDist)
                {
                    closestDist = e.DistanceSq;
                    closestId = e.EntityId;
                }
            }
            if (closestId >= 0)
                return Intention<MOBAPerception>.Attack(closestId);
            return Intention<MOBAPerception>.Stop();
        };
        return action;
    }

    private static GoapAction<MOBAPerception> CreatePushLane(MOBAPerception p, MOBAMemory memory)
    {
        var action = new GoapAction<MOBAPerception>("PushLane", cost: 2f);
        action.Preconditions.Add(() =>
        {
            foreach (var e in p.VisibleEnemies)
                if (e.Type == MOBAEntityType.Creep && e.TeamId != p.SelfTeam)
                    return false;
            return true;
        });
        action.Effects.Add(() => { });
        action.ActionFactory = () =>
        {
            float pushX = p.SelfTeam == 0 ? 45f : 3f;
            return Intention<MOBAPerception>.Move(pushX, 0, p.SelfPosZ);
        };
        return action;
    }

    private static GoapAction<MOBAPerception> CreateMoveToBase(MOBAPerception p, MOBAMemory memory)
    {
        var action = new GoapAction<MOBAPerception>("MoveToBase", cost: 1f);
        action.Preconditions.Add(() => p.SelfHealthRatio < 0.3f);
        action.Effects.Add(() => { });
        action.ActionFactory = () =>
        {
            float baseX = p.SelfTeam == 0 ? 3f : 42f;
            return Intention<MOBAPerception>.Move(baseX, 0, 8f);
        };
        return action;
    }

    private static GoapAction<MOBAPerception> CreatePatrol(MOBAPerception p, MOBAMemory memory)
    {
        var action = new GoapAction<MOBAPerception>("Patrol", cost: 1f);
        action.Preconditions.Add(() => true);
        action.Effects.Add(() => { });
        action.ActionFactory = () =>
        {
            float patrolX = p.SelfTeam == 0 ? 15f : 30f;
            return Intention<MOBAPerception>.Move(patrolX, 0, p.SelfPosZ);
        };
        return action;
    }
}
