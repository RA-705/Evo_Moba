using System;
using System.Collections.Generic;
using System.Threading;
using Evo.Core;
using Evo.Core.ECS;
using Evo.MOBA.Combat;
using Evo.MOBA.Match;
using Evo.MOBA.Systems;
using Evo.Shared.Math;

namespace Evo.MOBA.Admin;

public sealed class AdminConsole
{
    private readonly World _world;
    private readonly MatchStateMachine _state;
    private readonly CancellationTokenSource _cts;
    private readonly Thread _inputThread;

    public AdminConsole(World world, MatchStateMachine state, CancellationTokenSource cts)
    {
        _world = world;
        _state = state;
        _cts = cts;
        _inputThread = new Thread(Listen) { IsBackground = true };
        _inputThread.Start();
    }

    private void Listen()
    {
        while (!_cts.IsCancellationRequested)
        {
            var line = Console.ReadLine();
            if (line is null) continue;
            ProcessCommand(line.Trim().ToLowerInvariant());
        }
    }

    private void ProcessCommand(string cmd)
    {
        var parts = cmd.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0) return;

        switch (parts[0])
        {
            case "help" or "?":
                PrintHelp();
                break;

            case "status":
                PrintStatus();
                break;

            case "players":
                PrintPlayers();
                break;

            case "restart":
                Console.WriteLine("[Admin] Restart requested. Exiting...");
                _cts.Cancel();
                break;

            case "stop":
                Console.WriteLine("[Admin] Stop requested. Exiting...");
                _cts.Cancel();
                break;

            case "phase":
                if (parts.Length > 1 && Enum.TryParse<MatchPhase>(parts[1], true, out var phase))
                {
                    _state.TransitionTo(phase);
                    Console.WriteLine($"[Admin] Force transition to {phase}");
                }
                else
                {
                    Console.WriteLine($"[Admin] Current phase: {_state.CurrentPhase}");
                }
                break;

            case "spawn":
                if (parts.Length >= 5 && int.TryParse(parts[1], out var teamId))
                {
                    // usage: spawn <teamId> <heroId> <x> <z>
                    if (int.TryParse(parts[2], out var heroId) &&
                        float.TryParse(parts[3], out var x) &&
                        float.TryParse(parts[4], out var z))
                    {
                        var entity = _world.Create();
                        _world.AddComponent(entity, new PositionComponent { Value = new EvoVector3(x, 0, z) });
                        _world.AddComponent(entity, new HealthComponent { Current = 100, Max = 100 });
                        _world.AddComponent(entity, new TeamComponent { TeamId = teamId });
                        Console.WriteLine($"[Admin] Spawned entity E{entity.Id.Value} for team {teamId}");
                    }
                }
                break;

            case "gc":
                GC.Collect();
                GC.WaitForPendingFinalizers();
                Console.WriteLine("[Admin] GC forced");
                break;

            default:
                Console.WriteLine($"[Admin] Unknown command: {parts[0]}. Type 'help'.");
                break;
        }

        Console.Out.Flush();
    }

    private void PrintHelp()
    {
        Console.WriteLine("=== Admin Commands ===");
        Console.WriteLine("  help/?      - Print this help");
        Console.WriteLine("  status      - Show match + server status");
        Console.WriteLine("  players     - List all entities with health");
        Console.WriteLine("  phase [name]- Show/force phase transition (Lobby/PickPhase/Game/PostGame)");
        Console.WriteLine("  spawn <t> <h> <x> <z> - Spawn entity");
        Console.WriteLine("  restart     - Restart server");
        Console.WriteLine("  stop        - Stop server");
        Console.WriteLine("  gc          - Force garbage collection");
    }

    private void PrintStatus()
    {
        int entityCount = 0;
        foreach (var _ in _world.GetEntityIds<HealthComponent>()) entityCount++;

        Console.WriteLine($"=== Server Status ===");
        Console.WriteLine($"  Phase: {_state.CurrentPhase} ({_state.PhaseElapsed:F1}s)");
        Console.WriteLine($"  Entities: {entityCount}");
        Console.WriteLine($"  Memory: {GC.GetTotalMemory(false) / 1024} KB");
    }

    private void PrintPlayers()
    {
        int alive = 0, dead = 0;
        foreach (var id in _world.GetEntityIds<HealthComponent>())
        {
            if (!_world.TryGetComponent<TeamComponent>(id, out var team) ||
                !_world.TryGetComponent<HealthComponent>(id, out var health))
                continue;

            string status = health.Current > 0f ? "ALIVE" : "DEAD";
            if (health.Current > 0f) alive++; else dead++;
            Console.WriteLine($"  E{id.Value} Team {team.TeamId} HP {health.Current}/{health.Max} [{status}]");
        }

        Console.WriteLine($"  Total: {alive} alive, {dead} dead");
    }
}
