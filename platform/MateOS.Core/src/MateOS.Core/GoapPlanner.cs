using System;
using System.Collections.Generic;

namespace MateOS.Core;

public sealed class GoapPlanner<TPerception>
{
    private const int MaxDepth = 10;

    public List<GoapAction<TPerception>>? Plan(List<GoapGoal> goals, List<GoapAction<TPerception>> actions)
    {
        goals.Sort((a, b) => b.Priority.CompareTo(a.Priority));

        foreach (var goal in goals)
        {
            if (goal.IsSatisfied())
                continue;

            var result = DepthFirstSearch(goal, actions, new List<GoapAction<TPerception>>(), 0);
            if (result is not null)
                return result;
        }

        return null;
    }

    private static List<GoapAction<TPerception>>? DepthFirstSearch(
        GoapGoal goal,
        List<GoapAction<TPerception>> actions,
        List<GoapAction<TPerception>> currentPlan,
        int depth)
    {
        if (goal.IsSatisfied())
            return new List<GoapAction<TPerception>>(currentPlan);

        if (depth >= MaxDepth)
            return null;

        foreach (var action in actions)
        {
            if (!action.CanExecute())
                continue;

            currentPlan.Add(action);
            action.ApplyEffects();

            var result = DepthFirstSearch(goal, actions, currentPlan, depth + 1);

            action.Effects.ForEach(e => e());
            currentPlan.RemoveAt(currentPlan.Count - 1);

            if (result is not null)
                return result;
        }

        return null;
    }
}
