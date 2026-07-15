using System;
using Evo.Core.ECS;

namespace Evo.MOBA.Match;

public sealed class MatchStateSystem : ISystem
{
    private readonly MatchStateMachine _state;
    private readonly double _pickDuration;
    private readonly double _postGameDuration;

    public MatchStateSystem(MatchStateMachine state, double pickSeconds = 30, double postGameSeconds = 10)
    {
        _state = state;
        _pickDuration = pickSeconds;
        _postGameDuration = postGameSeconds;
    }

    public MatchPhase Phase => _state.CurrentPhase;

    public void OnTick(World world, float deltaTime)
    {
        _state.Update(deltaTime);

        switch (_state.CurrentPhase)
        {
            case MatchPhase.Lobby:
                if (_state.PhaseElapsed >= 5)
                    _state.TransitionTo(MatchPhase.PickPhase);
                break;

            case MatchPhase.PickPhase:
                if (_state.PhaseElapsed >= _pickDuration)
                    _state.TransitionTo(MatchPhase.Game);
                break;

            case MatchPhase.Game:
                var gameOver = false;
                foreach (var sys in _systems)
                {
                    if (sys is GameOverSystem gos && gos.IsGameOver)
                        gameOver = true;
                }
                if (gameOver)
                    _state.TransitionTo(MatchPhase.PostGame);
                break;

            case MatchPhase.PostGame:
                if (_state.PhaseElapsed >= _postGameDuration)
                {
                    Console.WriteLine("[MatchStateSystem] Server shutting down after post-game.");
                    Console.Out.Flush();
                    Environment.Exit(0);
                }
                break;
        }
    }

    private ISystem[] _systems = Array.Empty<ISystem>();
    public void BindSystems(ISystem[] systems) => _systems = systems;
}
