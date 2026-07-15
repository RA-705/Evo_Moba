using System;
using System.Threading;
using Evo.Core.ECS;

namespace Evo.MOBA.Match;

public sealed class WatchdogTimer : IDisposable
{
    private readonly TimeSpan _timeout;
    private readonly IWatchdogHook _hook;
    private readonly Timer _timer;
    private DateTime _lastActivity;

    public event Action? OnTimeout;

    public WatchdogTimer(TimeSpan timeout, IWatchdogHook hook)
    {
        _timeout = timeout;
        _hook = hook;
        _lastActivity = DateTime.UtcNow;

        _timer = new Timer(Check!, null,
            TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10));
    }

    public void Ping() => _lastActivity = DateTime.UtcNow;

    private void Check(object? state)
    {
        if (!_hook.IsRunning)
            return;

        if (DateTime.UtcNow - _lastActivity > _timeout)
        {
            Console.WriteLine($"[Watchdog] Match timed out ({_timeout.TotalMinutes:F0} min)");
            OnTimeout?.Invoke();
        }
    }

    public void Dispose() => _timer?.Dispose();
}
