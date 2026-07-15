using System;
using System.Collections.Generic;
using Evo.Core.ECS;
using Evo.MOBA.Combat;
using Evo.MOBA.Systems;
using Evo.Shared.Math;

namespace Evo.MOBA.AI.MATE;

public sealed class ObservationalLearningSystem : ISystem
{
    public void OnTick(World world, float deltaTime)
    {
        foreach (var botId in world.GetEntityIds<BotIntelligenceComponent>())
        {
            if (!world.TryGetComponent<BotIntelligenceComponent>(botId, out var bot))
                continue;

            if (!world.TryGetComponent<PositionComponent>(botId, out var botPos))
                continue;

            if (!world.TryGetComponent<PlayerPatternTracker>(botId, out var tracker))
                continue;

            if (tracker.Patterns == null)
                tracker.Patterns = new Dictionary<int, PlayerPattern>();

            foreach (var targetId in world.GetEntityIds<HealthComponent>())
            {
                if (targetId == botId) continue;
                if (!world.TryGetComponent<TeamComponent>(targetId, out var targetTeam)) continue;
                if (!world.TryGetComponent<PositionComponent>(targetId, out var targetPos)) continue;
                if (!world.TryGetComponent<HealthComponent>(targetId, out var targetHealth)) continue;

                int rawId = targetId.Value;
                float dx = targetPos.Value.X - botPos.Value.X;
                float dz = targetPos.Value.Z - botPos.Value.Z;
                float distSq = dx * dx + dz * dz;

                if (distSq > 20f * 20f) continue;

                if (!tracker.Patterns.TryGetValue(rawId, out var pattern))
                {
                    pattern = new PlayerPattern
                    {
                        PlayerId = rawId,
                        RetreatHpThreshold = 0.3f,
                        AggressionLevel = 0.5f,
                        PreferredLane = targetPos.Value.Z,
                        ReactionTime = 0.3f,
                        PositionHistory = new float[10],
                    };
                }

                float hpRatio = targetHealth.Current / targetHealth.Max;

                int botTeamId = world.TryGetComponent<TeamComponent>(botId, out var bt) ? bt.TeamId : -1;

                if (hpRatio < 0.3f && targetTeam.TeamId != botTeamId)
                {
                    pattern.RetreatHpThreshold = Math.Max(pattern.RetreatHpThreshold, hpRatio);
                }

                float dist = MathF.Sqrt(distSq);
                if (dist < 3f)
                {
                    pattern.AggressionLevel = Math.Clamp(pattern.AggressionLevel + 0.01f, 0f, 1f);
                }
                else if (dist > 10f)
                {
                    pattern.AggressionLevel = Math.Clamp(pattern.AggressionLevel - 0.005f, 0f, 1f);
                }

                pattern.LastSeenTime = 0f;
                tracker.Patterns[rawId] = pattern;
            }

            world.SetComponent(botId, tracker);
        }
    }
}

public sealed class EmotionalSystem : ISystem
{
    private float _gameTime;

    public void OnTick(World world, float deltaTime)
    {
        _gameTime += deltaTime;

        foreach (var id in world.GetEntityIds<BotIntelligenceComponent>())
        {
            ref var bot = ref world.GetComponent<BotIntelligenceComponent>(id);

            if (!world.TryGetComponent<HealthComponent>(id, out var health))
                continue;

            if (health.Current <= 0f)
            {
                bot.ConsecutiveDeaths++;
                bot.ConsecutiveKills = 0;
                bot.LastDeathTime = _gameTime;
            }

            if (bot.ConsecutiveDeaths >= 3)
            {
                bot.CurrentEmotion = BotEmotion.Frustrated;
                bot.EmotionIntensity = Math.Min(1f, bot.ConsecutiveDeaths * 0.2f);
                bot.AggressionModifier = 0.3f;
                bot.CautionModifier = -0.2f;
            }
            else if (bot.ConsecutiveKills >= 3)
            {
                bot.CurrentEmotion = BotEmotion.Confident;
                bot.EmotionIntensity = Math.Min(1f, bot.ConsecutiveKills * 0.15f);
                bot.AggressionModifier = 0.2f;
                bot.ConfidenceModifier = 0.3f;
            }
            else if (health.Current / health.Max < 0.2f)
            {
                bot.CurrentEmotion = BotEmotion.Desperate;
                bot.EmotionIntensity = 0.8f;
                bot.CautionModifier = 0.4f;
            }
            else
            {
                bot.CurrentEmotion = BotEmotion.Neutral;
                bot.EmotionIntensity = Math.Max(0f, bot.EmotionIntensity - deltaTime * 0.1f);
                bot.AggressionModifier = Math.Max(0f, bot.AggressionModifier - deltaTime * 0.05f);
                bot.CautionModifier = Math.Max(0f, bot.CautionModifier - deltaTime * 0.05f);
                bot.ConfidenceModifier = Math.Max(0f, bot.ConfidenceModifier - deltaTime * 0.05f);
            }

            if (_gameTime - bot.LastDeathTime > 30f)
                bot.ConsecutiveDeaths = Math.Max(0, bot.ConsecutiveDeaths - 1);

            if (_gameTime - bot.LastKillTime > 30f)
                bot.ConsecutiveKills = Math.Max(0, bot.ConsecutiveKills - 1);

            world.SetComponent(id, bot);
        }
    }
}

public sealed class MetaAdaptationSystem : ISystem
{
    private float _lastAnalysisTime;
    private const float AnalysisInterval = 30f;

    public void OnTick(World world, float deltaTime)
    {
        _lastAnalysisTime += deltaTime;
        if (_lastAnalysisTime < AnalysisInterval) return;
        _lastAnalysisTime = 0f;

        foreach (var botId in world.GetEntityIds<MetaAdaptationComponent>())
        {
            if (!world.TryGetComponent<TeamComponent>(botId, out var botTeam))
                continue;

            ref var meta = ref world.GetComponent<MetaAdaptationComponent>(botId);

            meta.EnemyPhysicalDamageCount = 0;
            meta.EnemyMagicDamageCount = 0;
            meta.EnemyTankCount = 0;
            meta.EnemyAssassinCount = 0;

            foreach (var targetId in world.GetEntityIds<HealthComponent>())
            {
                if (!world.TryGetComponent<TeamComponent>(targetId, out var targetTeam)) continue;
                if (targetTeam.TeamId == botTeam.TeamId) continue;

                if (world.TryGetComponent<AttackComponent>(targetId, out var atk))
                {
                    if (atk.Damage > 15f) meta.EnemyPhysicalDamageCount++;
                }

                if (world.TryGetComponent<HealthComponent>(targetId, out var hp))
                {
                    if (hp.Max > 200f) meta.EnemyTankCount++;
                }

                if (targetId.Value < 1000)
                {
                    meta.EnemyAssassinCount++;
                }
            }

            meta.PhysicalDefensePriority = meta.EnemyPhysicalDamageCount * 0.3f;
            meta.MagicDefensePriority = meta.EnemyMagicDamageCount * 0.3f;
            meta.LastMetaAnalysisTime = 0f;

            world.SetComponent(botId, meta);
        }
    }
}

public sealed class TeamCoordinationSystem : ISystem
{
    private float _gameTime;
    private const float CoordinationCooldown = 5f;

    public void OnTick(World world, float deltaTime)
    {
        _gameTime += deltaTime;

        var teamBots = new Dictionary<int, List<EntityId>>();

        foreach (var id in world.GetEntityIds<TeamCoordinationComponent>())
        {
            if (!world.TryGetComponent<TeamComponent>(id, out var team)) continue;
            if (!teamBots.ContainsKey(team.TeamId))
                teamBots[team.TeamId] = new List<EntityId>();
            teamBots[team.TeamId].Add(id);
        }

        foreach (var (teamId, bots) in teamBots)
        {
            if (bots.Count < 2) continue;

            foreach (var botId in bots)
            {
                if (!world.TryGetComponent<TeamCoordinationComponent>(botId, out var coord))
                    continue;

                if (_gameTime - coord.LastCoordinationTime < CoordinationCooldown)
                    continue;

                if (!world.TryGetComponent<PositionComponent>(botId, out var botPos))
                    continue;

                EntityId? closestAlly = null;
                float closestDist = float.MaxValue;

                foreach (var allyId in bots)
                {
                    if (allyId == botId) continue;
                    if (!world.TryGetComponent<PositionComponent>(allyId, out var allyPos)) continue;

                    float dist = EvoVector3.Distance(botPos.Value, allyPos.Value);
                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        closestAlly = allyId;
                    }
                }

                if (closestAlly != null && closestDist < 10f)
                {
                    coord.CoordinatedWithId = closestAlly.Value;
                    coord.LastCoordinationTime = _gameTime;

                    if (world.TryGetComponent<AttackComponent>(botId, out var atk) &&
                        atk.CurrentTargetId != Entity.Null.Id)
                    {
                        coord.IsFlanking = closestDist > 5f;
                        coord.IsCovering = !coord.IsFlanking;
                    }
                }

                world.SetComponent(botId, coord);
            }
        }
    }
}
