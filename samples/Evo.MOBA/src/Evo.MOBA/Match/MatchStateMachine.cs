using System;

namespace Evo.MOBA.Match;

public enum MatchPhase : byte
{
    Lobby,
    PickPhase,
    Game,
    PostGame,
}

public sealed class MatchStateMachine
{
    public MatchPhase CurrentPhase { get; private set; } = MatchPhase.Lobby;
    public double PhaseElapsed { get; private set; }
    public DateTime PhaseStartedAt { get; private set; }

    public event Action<MatchPhase, MatchPhase>? OnPhaseChanged;

    public void TransitionTo(MatchPhase newPhase)
    {
        var old = CurrentPhase;
        CurrentPhase = newPhase;
        PhaseElapsed = 0;
        PhaseStartedAt = DateTime.UtcNow;
        OnPhaseChanged?.Invoke(old, newPhase);
    }

    public void Update(double deltaTime) => PhaseElapsed += deltaTime;

    public bool IsInPhase(MatchPhase phase) => CurrentPhase == phase;
}
