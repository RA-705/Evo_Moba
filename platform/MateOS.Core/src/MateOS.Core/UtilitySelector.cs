using System.Collections.Generic;

namespace MateOS.Core;

public interface ICognitiveScorer<TPerception>
{
    float Evaluate(WorldModel<TPerception> world);
}

public class ScoredAction<TPerception>
{
    private readonly List<ICognitiveScorer<TPerception>> _scorers = new();

    public float Weight { get; set; } = 1.0f;
    public Func<Intention<TPerception>> ActionFactory { get; set; } = null!;

    public void AddScorer(ICognitiveScorer<TPerception> scorer) =>
        _scorers.Add(scorer);

    public float CalculateUtility(WorldModel<TPerception> world)
    {
        if (_scorers.Count == 0)
            return 0f;

        var total = 0f;

        foreach (var scorer in _scorers)
            total += scorer.Evaluate(world);

        return total / _scorers.Count * Weight;
    }
}

public class UtilitySelector<TPerception>
{
    private readonly List<ScoredAction<TPerception>> _actions = new();

    public void AddAction(ScoredAction<TPerception> action) =>
        _actions.Add(action);

    public Intention<TPerception> SelectBestAction(WorldModel<TPerception> world)
    {
        var bestScore = float.MinValue;
        ScoredAction<TPerception>? bestAction = null;

        foreach (var action in _actions)
        {
            var utility = action.CalculateUtility(world);

            if (utility > bestScore)
            {
                bestScore = utility;
                bestAction = action;
            }
        }

        return bestAction?.ActionFactory() ?? Intention<TPerception>.Stop();
    }
}
