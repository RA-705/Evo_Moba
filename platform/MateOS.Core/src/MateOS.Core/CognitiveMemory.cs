using System.Collections.Generic;

namespace MateOS.Core;

public readonly struct MemoryEntry<T>
{
    public T Data { get; }
    public float Timestamp { get; }

    public MemoryEntry(T data, float timestamp)
    {
        Data = data;
        Timestamp = timestamp;
    }
}

public readonly struct MemoryEntry
{
    public object Data { get; }
    public float Timestamp { get; }

    public MemoryEntry(object data, float timestamp)
    {
        Data = data;
        Timestamp = timestamp;
    }
}

public class CognitiveMemory
{
    private readonly Dictionary<string, MemoryEntry> _events = new();

    public void RecordEvent(string key, object data, float currentTime) =>
        _events[key] = new MemoryEntry(data, currentTime);

    public float TimeSince(string key, float currentTime) =>
        _events.TryGetValue(key, out var entry) ? currentTime - entry.Timestamp : float.MaxValue;

    public bool TryGetEvent(string key, out MemoryEntry entry) =>
        _events.TryGetValue(key, out entry);

    public void Clear() => _events.Clear();
}

public class WorldModel<TPerception>
{
    public CognitiveMemory Memory { get; } = new();
    public TPerception CurrentSnapshot { get; private set; } = default!;

    public void Update(TPerception snapshot)
    {
        CurrentSnapshot = snapshot;
    }
}
