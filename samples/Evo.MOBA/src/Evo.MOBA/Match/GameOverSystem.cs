using System;
using Evo.Core.ECS;
using Evo.MOBA.Combat;
using Evo.MOBA.Towers;

namespace Evo.MOBA.Match;

public sealed class GameOverSystem : ISystem
{
    private bool _gameOver;

    public event Action<int>? OnGameOver;

    public bool IsGameOver => _gameOver;

    public void OnTick(World world, float deltaTime)
    {
        if (_gameOver) return;

        bool nexus0Alive = false, nexus1Alive = false;

        foreach (var id in world.GetEntityIds<NexusComponent>())
        {
            if (!world.TryGetComponent<NexusComponent>(id, out var nexus) ||
                !world.TryGetComponent<HealthComponent>(id, out var health))
                continue;

            if (health.Current <= 0f) continue;

            if (nexus.TeamId == 0) nexus0Alive = true;
            else nexus1Alive = true;
        }

        if (!nexus0Alive || !nexus1Alive)
        {
            _gameOver = true;
            int winner = nexus1Alive ? 1 : 0;
            OnGameOver?.Invoke(winner);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[GameOver] Team {winner} wins! (Nexus0 alive: {nexus0Alive}, Nexus1 alive: {nexus1Alive})");
            Console.ResetColor();
            Console.Out.Flush();
        }
    }
}
