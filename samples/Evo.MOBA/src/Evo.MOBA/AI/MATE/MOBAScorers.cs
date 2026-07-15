using MateOS.Core;

namespace Evo.MOBA.AI.MATE;

public class HealthScorer : ICognitiveScorer<MOBAPerception>
{
    public float Evaluate(WorldModel<MOBAPerception> world)
    {
        var p = world.CurrentSnapshot;
        float score = 0f;
        if (p.SelfHealthRatio > 0.5f) score = 0.2f;
        else if (p.SelfHealthRatio > 0.25f) score = 0.6f;
        else score = 1.0f;

        score -= p.CautionModifier * 0.3f;
        return Math.Clamp(score, 0f, 1f);
    }
}

public class DangerScorer : ICognitiveScorer<MOBAPerception>
{
    public float Evaluate(WorldModel<MOBAPerception> world)
    {
        var p = world.CurrentSnapshot;
        int enemiesNear = p.CountEnemiesInRange(8f * 8f);
        int alliesNear = p.CountAlliesInRange(10f * 10f);

        if (enemiesNear == 0) return 0f;

        float enemyThreat = enemiesNear * 0.3f;
        float allyProtection = alliesNear * 0.15f;
        float hpFactor = 1f - p.SelfHealthRatio;

        float score = Math.Clamp(enemyThreat - allyProtection + hpFactor, 0f, 1f);

        score += p.CautionModifier * 0.2f;
        if (p.Emotion == BotEmotion.Frustrated) score *= 0.8f;
        if (p.Emotion == BotEmotion.Confident) score *= 0.6f;

        return Math.Clamp(score, 0f, 1f);
    }
}

public class KillOpportunityScorer : ICognitiveScorer<MOBAPerception>
{
    public float Evaluate(WorldModel<MOBAPerception> world)
    {
        var p = world.CurrentSnapshot;
        if (p.VisibleEnemies.Count == 0) return 0f;

        float bestScore = 0f;
        foreach (var e in p.VisibleEnemies)
        {
            float score = 0f;

            score += (1f - e.HealthRatio) * 0.4f;
            score += e.Type == MOBAEntityType.Hero ? 0.3f : 0.1f;
            score += p.SelfHealthRatio > 0.5f ? 0.2f : -0.1f;

            float distFactor = 1f / (1f + e.DistanceSq * 0.01f);
            score *= distFactor;

            score += p.AggressionModifier * 0.2f;
            score += p.ConfidenceModifier * 0.15f;

            if (p.Emotion == BotEmotion.Frustrated) score *= 1.3f;
            if (p.Emotion == BotEmotion.Confident) score *= 1.2f;
            if (p.Emotion == BotEmotion.Desperate) score *= 0.7f;

            if (score > bestScore) bestScore = score;
        }

        return Math.Clamp(bestScore, 0f, 1f);
    }
}

public class FarmValueScorer : ICognitiveScorer<MOBAPerception>
{
    public float Evaluate(WorldModel<MOBAPerception> world)
    {
        var p = world.CurrentSnapshot;
        float farmRangeSq = 5f * 5f;
        int creepsInRange = 0;

        foreach (var e in p.VisibleEnemies)
        {
            if (e.Type == MOBAEntityType.Creep && e.DistanceSq <= farmRangeSq)
                creepsInRange++;
        }

        if (creepsInRange == 0) return 0f;

        float baseScore = creepsInRange * 0.25f;
        float hpBonus = p.SelfHealthRatio > 0.6f ? 0.2f : 0f;
        float dangerPenalty = p.CountEnemiesInRange(10f * 10f) > 0 ? -0.3f : 0f;

        return System.Math.Clamp(baseScore + hpBonus + dangerPenalty, 0f, 1f);
    }
}

public class ObjectiveScorer : ICognitiveScorer<MOBAPerception>
{
    public float Evaluate(WorldModel<MOBAPerception> world)
    {
        var p = world.CurrentSnapshot;

        bool hasEnemyTower = false;
        bool hasEnemyCreeps = false;

        foreach (var e in p.VisibleEnemies)
        {
            if (e.Type == MOBAEntityType.Tower) hasEnemyTower = true;
            if (e.Type == MOBAEntityType.Creep) hasEnemyCreeps = true;
        }

        float score = 0f;

        if (!hasEnemyCreeps && !hasEnemyTower)
            score += 0.6f;
        else if (!hasEnemyCreeps)
            score += 0.3f;

        if (p.SelfHealthRatio > 0.7f)
            score += 0.2f;

        int alliesNear = p.CountAlliesInRange(12f * 12f);
        if (alliesNear >= 2)
            score += 0.2f;

        return System.Math.Clamp(score, 0f, 1f);
    }
}

public class ManaScorer : ICognitiveScorer<MOBAPerception>
{
    public float Evaluate(WorldModel<MOBAPerception> world)
    {
        var p = world.CurrentSnapshot;
        if (p.SelfManaRatio > 0.5f) return 0.8f;
        if (p.SelfManaRatio > 0.2f) return 0.4f;
        return 0.1f;
    }
}

public class LowHpRetreatScorer : ICognitiveScorer<MOBAPerception>
{
    public float Evaluate(WorldModel<MOBAPerception> world)
    {
        var p = world.CurrentSnapshot;
        if (p.SelfHealthRatio > 0.35f) return 0f;
        if (p.IsOutnumbered()) return 0.9f;
        return 0.5f;
    }
}
