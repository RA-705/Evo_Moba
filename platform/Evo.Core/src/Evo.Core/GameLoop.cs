using System.Diagnostics;

namespace Evo.Core;

public interface ITickable
{
    void Tick(float deltaTime);
}

public sealed class FixedTickLoop
{
    private readonly float _fixedDeltaTime;

    public FixedTickLoop(float ticksPerSecond)
    {
        _fixedDeltaTime = 1f / ticksPerSecond;
    }

    public void Start(ITickable engine, CancellationToken token)
    {
        var stopwatch = Stopwatch.StartNew();
        var accumulator = 0d;

        while (!token.IsCancellationRequested)
        {
            accumulator += stopwatch.Elapsed.TotalSeconds;
            stopwatch.Restart();

            while (accumulator >= _fixedDeltaTime)
            {
                engine.Tick(_fixedDeltaTime);
                accumulator -= _fixedDeltaTime;
            }

            var sleepMs = (int)((_fixedDeltaTime - accumulator) * 1000);
            if (sleepMs > 0)
                Thread.Sleep(sleepMs);
        }
    }
}
