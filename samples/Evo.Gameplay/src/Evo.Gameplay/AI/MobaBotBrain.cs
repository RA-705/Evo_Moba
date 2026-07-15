using System.Collections.Generic;
using Evo.Core.ECS;
using MateOS.Core;
using Evo.Gameplay.Systems;

namespace Evo.Gameplay.AI;

public readonly record struct MobaPerception(EntityId BotId, List<GameEntity> NearEntities, float X, float Z, int TeamGoal = 0);

public class MobaBotBrain : CognitiveCore<MobaPerception>
{
    private const float MaxAttackRange = 12f;
    private static readonly Random _rng = new();

    private float _elapsedTime;
    private float _patrolX;
    private float _patrolZ;

    private static Guid ToGuid(EntityId id) =>
        new((uint)id.Value, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);

    protected override void ProcessPerception(MobaPerception perception, float dt)
    {
        _elapsedTime += dt;

        var seeker = perception.NearEntities.Find(e => e.Id == perception.BotId);
        if (seeker is null) return;

        var nearest = TargetSystem.GetNearestEnemy(seeker, perception.NearEntities, MaxAttackRange);
        if (nearest is not null)
            World.Memory.RecordEvent("nearest_enemy_" + perception.BotId, nearest.Id, _elapsedTime);

        World.Memory.RecordEvent("team_goal_" + perception.BotId, perception.TeamGoal, _elapsedTime);
    }

    protected override void UpdateInternalMemory(float dt)
    {
    }

    protected override void GenerateDecisions()
    {
        var perception = World.CurrentSnapshot;
        var seeker = perception.NearEntities.Find(e => e.Id == perception.BotId);
        if (seeker is null)
        {
            EnqueueIntention(Intention<MobaPerception>.Stop());
            return;
        }

        // GOAP-style priority: fight > team goal > patrol
        if (World.Memory.TryGetEvent("nearest_enemy_" + perception.BotId, out var enemyEntry))
        {
            var secondsSinceLastSeen = _elapsedTime - enemyEntry.Timestamp;
            if (secondsSinceLastSeen < 2f)
            {
                var enemyId = (EntityId)enemyEntry.Data;
                EnqueueIntention(Intention<MobaPerception>.Attack(ToGuid(enemyId)));
                return;
            }
        }

        // Team goal drives patrol direction
        float goalX = perception.X, goalZ = perception.Z;
        bool hasGoal = false;

        if (World.Memory.TryGetEvent("team_goal_" + perception.BotId, out var goalEntry))
        {
            var goalType = (int)goalEntry.Data;
            switch (goalType)
            {
                case 1: // PushMid
                    goalX = 45f; goalZ = 45f; hasGoal = true;
                    break;
                case 2: // SplitPush
                    goalX = 55f; goalZ = 50f; hasGoal = true;
                    break;
                case 3: // Defend
                    goalX = 5f; goalZ = 5f; hasGoal = true;
                    break;
                case 4: // Group
                    goalX = perception.X + 10f; goalZ = perception.Z + 10f; hasGoal = true;
                    break;
                case 5: // Roshan
                    goalX = 35f; goalZ = 35f; hasGoal = true;
                    break;
            }
        }

        if (hasGoal)
        {
            var dx = goalX - perception.X;
            var dz = goalZ - perception.Z;
            if (dx * dx + dz * dz > 4f)
            {
                EnqueueIntention(Intention<MobaPerception>.Move(goalX, 0f, goalZ));
                return;
            }
        }

        // Default patrol
        var patrolDx = _patrolX - perception.X;
        var patrolDz = _patrolZ - perception.Z;
        if (patrolDx * patrolDx + patrolDz * patrolDz < 4f)
        {
            var angle = (float)(_rng.NextDouble() * System.Math.PI * 2);
            var dist = (float)(_rng.NextDouble() * 15f);
            _patrolX = perception.X + (float)System.Math.Cos(angle) * dist;
            _patrolZ = perception.Z + (float)System.Math.Sin(angle) * dist;
        }

        EnqueueIntention(Intention<MobaPerception>.Move(_patrolX, 0f, _patrolZ));
    }
}
