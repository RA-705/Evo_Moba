namespace Evo.Core.Events;

public sealed class EventBus : IEventBus
{
    private readonly Dictionary<Type, Delegate> _handlers = new();

    public void Subscribe<T>(Action<T> handler) where T : struct, IGameEvent
    {
        var type = typeof(T);
        if (_handlers.TryGetValue(type, out var existing))
            _handlers[type] = Delegate.Combine(existing, handler);
        else
            _handlers[type] = handler;
    }

    public void Unsubscribe<T>(Action<T> handler) where T : struct, IGameEvent
    {
        var type = typeof(T);
        if (_handlers.TryGetValue(type, out var existing))
        {
            var combined = Delegate.Remove(existing, handler);
            if (combined is null)
                _handlers.Remove(type);
            else
                _handlers[type] = combined;
        }
    }

    public void Publish<T>(T eventData) where T : struct, IGameEvent
    {
        if (_handlers.TryGetValue(typeof(T), out var d))
            ((Action<T>)d)(eventData);
    }
}
