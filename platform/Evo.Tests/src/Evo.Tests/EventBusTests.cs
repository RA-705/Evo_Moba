using Evo.Core.Events;

namespace Evo.Tests;

readonly record struct TestEvent(int Value) : IGameEvent;
readonly record struct AnotherEvent(string Label) : IGameEvent;

public class EventBusTests
{
    [Fact]
    public void SubscribeAndPublish_CallsHandler()
    {
        var bus = new EventBus();
        var called = false;

        bus.Subscribe<TestEvent>(e => called = true);
        bus.Publish(new TestEvent(42));

        Assert.True(called);
    }

    [Fact]
    public void Publish_PassesCorrectData()
    {
        var bus = new EventBus();
        var received = 0;

        bus.Subscribe<TestEvent>(e => received = e.Value);
        bus.Publish(new TestEvent(99));

        Assert.Equal(99, received);
    }

    [Fact]
    public void MultipleSubscribers_AllCalled()
    {
        var bus = new EventBus();
        var count = 0;

        bus.Subscribe<TestEvent>(e => count++);
        bus.Subscribe<TestEvent>(e => count++);

        bus.Publish(new TestEvent(1));

        Assert.Equal(2, count);
    }

    [Fact]
    public void Unsubscribe_RemovesHandler()
    {
        var bus = new EventBus();
        var count = 0;

        void Handler(TestEvent e) => count++;

        bus.Subscribe<TestEvent>(Handler);
        bus.Unsubscribe<TestEvent>(Handler);
        bus.Publish(new TestEvent(1));

        Assert.Equal(0, count);
    }

    [Fact]
    public void DifferentEventTypes_AreIndependent()
    {
        var bus = new EventBus();
        var testVal = 0;
        var anotherVal = "";

        bus.Subscribe<TestEvent>(e => testVal = e.Value);
        bus.Subscribe<AnotherEvent>(e => anotherVal = e.Label);

        bus.Publish(new TestEvent(42));
        bus.Publish(new AnotherEvent("hello"));

        Assert.Equal(42, testVal);
        Assert.Equal("hello", anotherVal);
    }

    [Fact]
    public void Publish_WithNoSubscribers_DoesNotThrow()
    {
        var bus = new EventBus();
        bus.Publish(new TestEvent(1));
    }
}
