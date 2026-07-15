namespace Evo.Core.Events;

public interface IEventBus
{
    void Subscribe<T>(Action<T> handler) where T : struct, IGameEvent;
    void Unsubscribe<T>(Action<T> handler) where T : struct, IGameEvent;
    void Publish<T>(T eventData) where T : struct, IGameEvent;
}
