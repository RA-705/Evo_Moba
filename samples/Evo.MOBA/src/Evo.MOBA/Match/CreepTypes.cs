using System.Collections.Generic;
using Evo.Core.ECS;
using Evo.MOBA.AI.MATE;
using Evo.MOBA.Combat;
using Evo.MOBA.Navigation;
using Evo.MOBA.Progression;
using Evo.MOBA.Systems;
using Evo.Shared.Math;

namespace Evo.MOBA.Match;

public enum CreepClass : byte
{
    Melee,
    Ranged,
    Cannon,
}

public struct CreepTypeComponent : IComponent
{
    public CreepClass Class;
}

public sealed class CreepWaveSpawner : ISystem
{
    private readonly NavGrid _navGrid;
    private readonly float _waveInterval;
    private float _timer;
    private int _waveNumber;

    public CreepWaveSpawner(NavGrid navGrid, float waveInterval)
    {
        _navGrid = navGrid;
        _waveInterval = waveInterval;
        _timer = waveInterval * 0.5f;
    }

    public void OnTick(World world, float deltaTime)
    {
        _timer -= deltaTime;
        if (_timer > 0f) return;

        _timer = _waveInterval;
        _waveNumber++;

        int melee = 3;
        int ranged = _waveNumber >= 2 ? 2 : 0;
        int cannon = _waveNumber >= 5 && (_waveNumber % 3 == 0) ? 1 : 0;

        for (int teamId = 0; teamId < 2; teamId++)
        {
            float baseX = teamId == 0 ? 2f : _navGrid.Width * _navGrid.CellSize - 2f;
            float baseZ = 10f;

            for (int i = 0; i < melee; i++)
                SpawnCreep(world, teamId, CreepClass.Melee, new EvoVector3(baseX, 0, baseZ + i * 1.5f));

            for (int i = 0; i < ranged; i++)
                SpawnCreep(world, teamId, CreepClass.Ranged, new EvoVector3(baseX + (teamId == 0 ? 1f : -1f), 0, baseZ + 4f + i * 1.5f));

            for (int i = 0; i < cannon; i++)
                SpawnCreep(world, teamId, CreepClass.Cannon, new EvoVector3(baseX, 0, baseZ + 8f));
        }
    }

    private static void SpawnCreep(World world, int teamId, CreepClass creepClass, EvoVector3 position)
    {
        var (hp, damage, range, speed, xp, gold) = creepClass switch
        {
            CreepClass.Melee => (50f, 8f, 1.5f, 4f, 20f, 25f),
            CreepClass.Ranged => (35f, 12f, 4f, 3.5f, 30f, 35f),
            CreepClass.Cannon => (120f, 20f, 2f, 3f, 60f, 70f),
            _ => (40f, 6f, 2f, 4f, 15f, 20f),
        };

        var entity = world.Create();
        world.AddComponent(entity, new PositionComponent { Value = position });
        world.AddComponent(entity, new VelocityComponent { Value = EvoVector3.Zero });
        world.AddComponent(entity, new HealthComponent { Current = hp, Max = hp });
        world.AddComponent(entity, new ManaComponent { Current = 0f, Max = 0f });
        world.AddComponent(entity, new TeamComponent { TeamId = teamId });
        world.AddComponent(entity, new AttackComponent
        {
            Damage = damage, Range = range, CooldownTime = 1f,
            TimeUntilNextAttack = 0f, CurrentTargetId = Entity.Null.Id,
        });
        world.AddComponent(entity, new MateBrainComponent
        {
            PerceptionRange = 8, IsAggressive = true,
        });
        world.AddComponent(entity, new MOBAComponent());
        world.AddComponent(entity, new PathfollowComponent
        {
            Waypoints = new List<EvoVector3>(), CurrentWaypointIndex = 0,
            StoppingDistance = 0.5f,
        });
        world.AddComponent(entity, new XpGrantComponent { Reward = xp });
        world.AddComponent(entity, new CreepTypeComponent { Class = creepClass });
    }
}
