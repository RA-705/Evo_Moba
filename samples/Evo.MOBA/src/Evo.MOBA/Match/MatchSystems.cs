using System;
using System.Collections.Generic;
using System.IO;
using Evo.Core.ECS;
using Evo.MOBA.Combat;
using Evo.MOBA.Navigation;
using Evo.MOBA.Systems;
using Evo.Shared.Math;

namespace Evo.MOBA.Match;

public interface IWatchdogHook
{
    bool IsRunning { get; }
}

public sealed class MatchSystems : IWatchdogHook
{
    private readonly World _world;
    private readonly MatchConfig _config;
    private readonly List<IDisposable> _disposables = new();
    private DateTime _startTime;
    private bool _running;

    public bool IsRunning => _running;

    public MatchSystems(World world, MatchConfig config)
    {
        _world = world;
        _config = config;
    }

    public void StartMatch()
    {
        _startTime = DateTime.UtcNow;
        _running = true;
        Console.WriteLine($"[MatchSystems] Match started at {_startTime:HH:mm:ss}");

        if (!Directory.Exists(_config.LogDirectory))
            Directory.CreateDirectory(_config.LogDirectory);
    }

    public void EndMatch()
    {
        if (!_running) return;
        _running = false;

        var duration = DateTime.UtcNow - _startTime;
        var winner = DetermineWinner();

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"[MatchSystems] Match ended. Duration: {duration.Minutes}m {duration.Seconds}s");
        var stats = new MatchStatsReport(_world, winner, duration);
        stats.Export(Path.Combine(_config.LogDirectory, $"match_{_startTime:yyyy-MM-dd_HH-mm-ss}.json"));
        Console.ResetColor();

        foreach (var d in _disposables)
            d.Dispose();
    }

    private int DetermineWinner()
    {
        int aliveTeam1 = 0, aliveTeam2 = 0;
        foreach (var id in _world.GetEntityIds<HealthComponent>())
        {
            if (!_world.TryGetComponent<HealthComponent>(id, out var health) ||
                !_world.TryGetComponent<TeamComponent>(id, out var team))
                continue;

            if (health.Current > 0f)
            {
                if (team.TeamId == 0) aliveTeam1++;
                else aliveTeam2++;
            }
        }

        if (aliveTeam1 > aliveTeam2) return 0;
        if (aliveTeam2 > aliveTeam1) return 1;
        return -1;
    }
}
