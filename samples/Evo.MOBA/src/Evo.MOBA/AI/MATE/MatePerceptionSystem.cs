using System.Collections.Generic;
using Evo.Core.ECS;
using Evo.MOBA.Abilities;
using Evo.MOBA.Combat;
using Evo.MOBA.Systems;
using Evo.Shared.Math;

namespace Evo.MOBA.AI.MATE;

public struct MateBrainComponent : IComponent
{
    public float PerceptionRange;
    public bool IsAggressive;
    public float ReactionTime;
    public float ReactionTimer;
    public float PathRecalculationCooldown;
    public float PathRecalcTimer;
    public float RetreatHealthThreshold;
    public float ClumsinessFactor;
    public bool IsRetreating;
    public EvoVector3 RetreatTarget;
    public float NextAbilityCheckTime;
}

public sealed class MatePerceptionSystem : ISystem
{
    private readonly FogOfWarSystem _fogOfWar;
    private float _gameTime;
    private readonly List<VisibleEntity> _enemyBuffer = new();
    private readonly List<VisibleEntity> _allyBuffer = new();

    public MatePerceptionSystem(FogOfWarSystem fogOfWar)
    {
        _fogOfWar = fogOfWar;
    }

    public void OnTick(World world, float deltaTime)
    {
        _gameTime += deltaTime;

        foreach (var id in world.GetEntityIds<MOBAComponent>())
        {
            if (!world.TryGetComponent<MOBAComponent>(id, out var moba) ||
                !world.TryGetComponent<TeamComponent>(id, out var team) ||
                !world.TryGetComponent<PositionComponent>(id, out var pos) ||
                !world.TryGetComponent<HealthComponent>(id, out var health))
                continue;

            float mana = 100f;
            float maxMana = 100f;
            if (world.TryGetComponent<ManaComponent>(id, out var manaComp))
            {
                mana = manaComp.Current;
                maxMana = manaComp.Max;
            }

            bool isRetreating = moba.Core.Memory.TimeSinceRetreat(_gameTime) < 2f;

            _enemyBuffer.Clear();
            _allyBuffer.Clear();

            float perceptionRange = 15f * 15f;

            foreach (var targetId in world.GetEntityIds<HealthComponent>())
            {
                if (targetId == id)
                    continue;

                if (!world.TryGetComponent<TeamComponent>(targetId, out var targetTeam) ||
                    !world.TryGetComponent<PositionComponent>(targetId, out var targetPos))
                    continue;

                var dx = targetPos.Value.X - pos.Value.X;
                var dz = targetPos.Value.Z - pos.Value.Z;
                float distSq = dx * dx + dz * dz;

                if (distSq > perceptionRange)
                    continue;

                if (!_fogOfWar.IsVisibleToTeam(team.TeamId, targetId))
                    continue;

                world.TryGetComponent<HealthComponent>(targetId, out var targetHealth);

                var entityType = MOBAEntityType.Creep;
                if (targetId.Value >= 1000 && targetId.Value < 2000) entityType = MOBAEntityType.Tower;
                else if (targetId.Value >= 2000 && targetId.Value < 3000) entityType = MOBAEntityType.Nexus;
                else if (targetId.Value < 1000) entityType = MOBAEntityType.Hero;

                var visible = new VisibleEntity(
                    targetId.Value, entityType, targetTeam.TeamId, distSq,
                    targetHealth.Current, targetHealth.Max,
                    targetPos.Value.X, targetPos.Value.Z);

                if (targetTeam.TeamId == team.TeamId)
                    _allyBuffer.Add(visible);
                else
                    _enemyBuffer.Add(visible);
            }

            var emotion = BotEmotion.Neutral;
            float emotionIntensity = 0f;
            float aggressionMod = 0f;
            float cautionMod = 0f;
            float confidenceMod = 0f;

            if (world.TryGetComponent<BotIntelligenceComponent>(id, out var intel))
            {
                emotion = intel.CurrentEmotion;
                emotionIntensity = intel.EmotionIntensity;
                aggressionMod = intel.AggressionModifier;
                cautionMod = intel.CautionModifier;
                confidenceMod = intel.ConfidenceModifier;
            }

            var perception = new MOBAPerception(
                id.Value, team.TeamId, health.Current, health.Max,
                mana, maxMana, pos.Value.X, pos.Value.Z,
                _gameTime, isRetreating,
                emotion, emotionIntensity, aggressionMod, cautionMod, confidenceMod,
                _enemyBuffer, _allyBuffer);

            moba.Core.Think(perception, deltaTime);

            world.SetComponent(id, moba);
        }
    }
}
