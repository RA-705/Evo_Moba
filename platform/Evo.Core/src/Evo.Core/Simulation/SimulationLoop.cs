namespace Evo.Core.Simulation;

public sealed class SimulationLoop
{
    private readonly List<ISimulationSystem> _systems;

    public bool IsRunning { get; private set; }

    public SimulationLoop(IEnumerable<ISimulationSystem> systems)
    {
        _systems = systems.ToList();
    }

    public void StartMatch()
    {
        IsRunning = true;
    }

    public void EndMatch()
    {
        IsRunning = false;
    }

    public void Tick(float fixedDeltaTime)
    {
        if (!IsRunning)
            return;

        foreach (var system in _systems)
            system.Update(fixedDeltaTime);
    }
}
