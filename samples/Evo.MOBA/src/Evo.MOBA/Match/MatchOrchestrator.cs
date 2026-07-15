using System;
using System.Collections.Generic;
using Evo.Core;
using Evo.Core.ECS;
using Evo.MOBA.Abilities;
using Evo.MOBA.AI.MATE;
using Evo.MOBA.Combat;
using Evo.MOBA.Buffs;
using Evo.MOBA.Heroes;
using Evo.MOBA.Items;
using Evo.MOBA.Learning;
using Evo.MOBA.Navigation;
using Evo.MOBA.Networking;
using Evo.MOBA.Progression;
using Evo.MOBA.Systems;
using Evo.MOBA.Admin;
using Evo.MOBA.Towers;
using Evo.MOBA.Wards;
using Evo.Shared.Math;

namespace Evo.MOBA.Match;

public sealed class MatchConfig
{
    public int GridWidth { get; set; } = 30;
    public int GridDepth { get; set; } = 30;
    public float CellSize { get; set; } = 1.5f;
    public int HeroesPerTeam { get; set; } = 3;
    public float TicksPerSecond { get; set; } = 10f;
    public int ListenPort { get; set; } = 7777;
    public bool EnableNetworking { get; set; } = true;
    public string LogDirectory { get; set; } = "./logs";
    public float MatchTimeoutMinutes { get; set; } = 30f;
    public bool EnableTrajectoryRecording { get; set; } = true;
    public bool EnableFogOfWar { get; set; } = true;
    public bool EnableTeamPlanner { get; set; } = true;
    public bool EnableWards { get; set; } = true;
    public float CreepWaveInterval { get; set; } = 15f;
    public float RespawnDelay { get; set; } = 5f;
    public int StartGold { get; set; } = 500;
    public string[] AdminWhitelist { get; set; } = Array.Empty<string>();
    public string ServerName { get; set; } = "EVO Match";
}

public sealed class MatchOrchestrator
{
    private readonly MatchConfig _config;
    private readonly CancellationTokenSource _cts = new();

    private MatchStateMachine _stateMachine = null!;
    private NavGrid _navGrid = null!;
    private World _world = null!;
    private FixedTickLoop _loop = null!;
    private SnapshotRecorder _recorder = null!;
    private GameNetServer _server = null!;
    private Thread _tickThread = null!;

    public MatchOrchestrator(MatchConfig config)
    {
        _config = config;
    }

    public void Run()
    {
        Console.WriteLine("[MatchOrchestrator] Starting match...");
        Console.WriteLine($"  Heroes per team: {_config.HeroesPerTeam}");
        Console.WriteLine($"  Grid: {_config.GridWidth}x{_config.GridDepth} @ {_config.CellSize}m/cell");
        Console.WriteLine($"  Ticks/s: {_config.TicksPerSecond}");
        Console.WriteLine($"  Fog of War: {_config.EnableFogOfWar}");
        Console.WriteLine($"  Team Planner: {_config.EnableTeamPlanner}");

        BootstrapWorld();

        if (_config.EnableNetworking)
            BootstrapNetworking();

        var matchSystems = new MatchSystems(_world, _config);
        matchSystems.StartMatch();
        Console.WriteLine("[MatchOrchestrator] Match started.");

        _ = new AdminConsole(_world, _stateMachine, _cts);

        Console.CancelKeyPress += (_, _) =>
        {
            Console.WriteLine("[MatchOrchestrator] Shutting down...");
            Shutdown(matchSystems);
        };

        Console.WriteLine("[MatchOrchestrator] Press Ctrl+C to stop.");
        _tickThread.Join();

        Shutdown(matchSystems);
    }

    private void BootstrapWorld()
    {
        _stateMachine = new MatchStateMachine();
        _navGrid = new NavGrid(_config.GridWidth, _config.GridDepth, _config.CellSize);
        _world = new World();

        var fogOfWar = new FogOfWarSystem(_navGrid);
        _world.AddSystem(fogOfWar);
        _world.AddSystem(new MatePerceptionSystem(fogOfWar));

        _world.AddSystem(new MateEngageSystem(_navGrid));
        _world.AddSystem(new BotChatSystem());
        _world.AddSystem(new CooldownSystem());
        _world.AddSystem(new MateAbilitySystem());
        _world.AddSystem(new AbilityValidationSystem());
        _world.AddSystem(new CastExecutionSystem());
        _world.AddSystem(new ProjectileMovementSystem());
        _world.AddSystem(new PathfollowSystem());
        _world.AddSystem(new MeleeAttackSystem());
        _world.AddSystem(new AutoRespawnSystem(5f));
        _world.AddSystem(new BuffSystem());
        _world.AddSystem(new WardSystem());

        _world.AddSystem(new WeatherSystem());
        _world.AddSystem(new SynergyApplySystem());
        _world.AddSystem(new AdaptiveDifficultySystem());
        _world.AddSystem(new ObservationalLearningSystem());
        _world.AddSystem(new EmotionalSystem());
        _world.AddSystem(new MetaAdaptationSystem());
        _world.AddSystem(new TeamCoordinationSystem());

        if (_config.EnableTeamPlanner)
            _world.AddSystem(new TeamPlannerSystem());

        _world.AddSystem(new CreepWaveSpawner(_navGrid, 15f));
        _world.AddSystem(new TowerAttackSystem());
        _world.AddSystem(new MatchStatsReportSystem());
        _world.AddSystem(new DamageEventSystem());
        _world.AddSystem(new ShopSystem());
        _world.AddSystem(new RotatingDealsSystem());
        _world.AddSystem(new KillGoldSystem());
        _world.AddSystem(new XpSystem());

        var gameOverSystem = new GameOverSystem();
        _world.AddSystem(gameOverSystem);

        if (_config.EnableTrajectoryRecording)
        {
            _recorder = new SnapshotRecorder();
            _world.AddSystem(_recorder);
        }

        var stateSystem = new MatchStateSystem(_stateMachine, 30, 10);
        stateSystem.BindSystems(new ISystem[] { gameOverSystem });
        _world.AddSystem(stateSystem);

        SpawnHeroes();
        SpawnTowersAndNexus();
        SpawnWeatherEntity();
        _loop = new FixedTickLoop(_config.TicksPerSecond);

        _tickThread = new Thread(() =>
        {
            try { _loop.Start(_world, _cts.Token); }
            catch (Exception ex) { Console.WriteLine($"[FATAL] {ex}"); Console.Out.Flush(); }
        })
        { IsBackground = true };
        _tickThread.Start();
    }

    private void BootstrapNetworking()
    {
        _server = new GameNetServer();
        _server.Start(_config.ListenPort);

        _world.AddSystem(new InputProcessSystem(_server, _navGrid));
        _world.AddSystem(new ServerTickSystem(_server));

        Console.WriteLine($"[MatchOrchestrator] UDP server on port {_config.ListenPort}");
    }

    private void SpawnHeroes()
    {
        var colors = new[] { ConsoleColor.Blue, ConsoleColor.Red };
        var heroIds = new[] { 1, 2, 3 };

        for (int teamId = 0; teamId < 2; teamId++)
        {
            for (int h = 0; h < _config.HeroesPerTeam; h++)
            {
                float baseX = teamId == 0 ? 3f : _config.GridWidth * _config.CellSize - 3f;
                float baseZ = 3f + h * 5f;
                int heroId = heroIds[h % heroIds.Length];

                var entity = HeroFactory.CreateHero(_world, heroId, teamId,
                    new EvoVector3(baseX, 0, baseZ), _config);

                if (HeroRegistry.Database.TryGetValue(heroId, out var hero))
                {
                    Console.Write("  ");
                    Console.ForegroundColor = colors[teamId];
                    Console.WriteLine($"[Team {teamId}] {hero.Name} spawned @ ({baseX}, 0, {baseZ})");
                    Console.ResetColor();
                }
            }
        }
        Console.Out.Flush();
    }

    private void SpawnTowersAndNexus()
    {
        float mapSizeX = _config.GridWidth * _config.CellSize;
        float mapSizeZ = _config.GridDepth * _config.CellSize;

        // Team 0 nexus + towers
        NexusFactory.CreateNexus(_world, 0, new EvoVector3(2f, 0, 2f));
        TowerFactory.CreateTower(_world, 0, new EvoVector3(6f, 0, 2f));
        TowerFactory.CreateTower(_world, 0, new EvoVector3(2f, 0, 6f));

        // Team 1 nexus + towers
        NexusFactory.CreateNexus(_world, 1, new EvoVector3(mapSizeX - 2f, 0, mapSizeZ - 2f));
        TowerFactory.CreateTower(_world, 1, new EvoVector3(mapSizeX - 6f, 0, mapSizeZ - 2f));
        TowerFactory.CreateTower(_world, 1, new EvoVector3(mapSizeX - 2f, 0, mapSizeZ - 6f));

        Console.WriteLine("[MatchOrchestrator] Nexus and towers deployed.");
        Console.Out.Flush();
    }

    private void SpawnWeatherEntity()
    {
        var entity = _world.Create();
        _world.AddComponent(entity, new WeatherComponent
        {
            CurrentWeather = WeatherType.Clear,
            Duration = 60f,
            TimeRemaining = 60f,
            TransitionProgress = 0f,
        });
        Console.WriteLine("[MatchOrchestrator] Weather system active.");
        Console.Out.Flush();
    }

    private void Shutdown(MatchSystems matchSystems)
    {
        if (!_cts.IsCancellationRequested)
        {
            _cts.Cancel();
            matchSystems.EndMatch();
            ExportTrajectory();
        }
    }

    private void ExportTrajectory()
    {
        if (_recorder is null || _recorder.Steps.Count == 0)
            return;

        var exporter = new TrajectoryExporter(_config.LogDirectory);
        exporter.ExportTrajectory(new List<TrajectoryStep>(_recorder.Steps), 0);
        Console.WriteLine($"[MatchOrchestrator] Trajectory exported ({_recorder.Steps.Count} steps)");
        Console.Out.Flush();
    }

    private sealed class ServerTickSystem : ISystem
    {
        private readonly GameNetServer _server;
        private uint _tick;
        public ServerTickSystem(GameNetServer server) => _server = server;
        public void OnTick(World world, float deltaTime)
        {
            _tick++;
            _server.PollEvents();
            _server.BroadcastSnapshot(world, _tick);
        }
    }
}
