using System;
using Evo.Core.ECS;
using Evo.MOBA.AI.MATE;
using Evo.MOBA.Combat;
using Evo.MOBA.Systems;

namespace Evo.MOBA.Match;

public struct PlayerSkillTracker : IComponent
{
    public float KillDeathRatio;
    public float WinRate;
    public float AverageDamagePerMinute;
    public float GamesPlayed;
    public float CurrentStreak;
    public float SkillRating;
}

public struct AdaptiveDifficultyComponent : IComponent
{
    public float AggressionLevel;
    public float ReactionTimeBonus;
    public float AimAccuracy;
    public float DecisionQuality;
}

public sealed class AdaptiveDifficultySystem : ISystem
{
    private const float KFactor = 32f;
    private const float BaseRating = 1500f;

    public void OnTick(World world, float deltaTime)
    {
        foreach (var id in world.GetEntityIds<MOBAComponent>())
        {
            if (!world.TryGetComponent<PlayerSkillTracker>(id, out var tracker))
                continue;

            if (!world.TryGetComponent<AdaptiveDifficultyComponent>(id, out var difficulty))
                continue;

            float skill = CalculateSkill(tracker);
            tracker.SkillRating = skill;
            world.SetComponent(id, tracker);

            AdjustDifficulty(ref difficulty, skill);
            world.SetComponent(id, difficulty);

            ApplyToBrain(world, id, difficulty);
        }
    }

    private static float CalculateSkill(PlayerSkillTracker tracker)
    {
        float rating = BaseRating;
        rating += tracker.KillDeathRatio * 50f;
        rating += tracker.WinRate * 200f;
        rating += tracker.AverageDamagePerMinute * 2f;
        rating += tracker.CurrentStreak * 10f;
        return Math.Clamp(rating, 500f, 3000f);
    }

    private static void AdjustDifficulty(ref AdaptiveDifficultyComponent diff, float skill)
    {
        float normalized = (skill - 500f) / 2500f;

        diff.AggressionLevel = Math.Clamp(0.3f + normalized * 0.5f, 0.2f, 0.9f);
        diff.ReactionTimeBonus = Math.Clamp(normalized * 0.1f, 0f, 0.12f);
        diff.AimAccuracy = Math.Clamp(0.5f + normalized * 0.35f, 0.4f, 0.95f);
        diff.DecisionQuality = Math.Clamp(0.4f + normalized * 0.45f, 0.3f, 0.95f);
    }

    private static void ApplyToBrain(World world, EntityId id, AdaptiveDifficultyComponent diff)
    {
        if (!world.TryGetComponent<MateBrainComponent>(id, out var brain))
            return;

        brain.ClumsinessFactor = 1f - diff.DecisionQuality;
        brain.ReactionTime = Math.Max(0.05f, 0.2f - diff.ReactionTimeBonus);
        brain.IsAggressive = diff.AggressionLevel > 0.5f;
        brain.PerceptionRange = 12f + diff.DecisionQuality * 8f;

        world.SetComponent(id, brain);
    }
}
