using Evo.Core.ECS;
using Evo.Core.Serialization;

namespace Evo.Tests;

struct SerPos : IComponent
{
    public float X;
    public float Y;
}

struct SerHealth : IComponent
{
    public float Max;
    public float Current;
}

struct SerLabel : IComponent
{
    public string Name;
}

public class WorldSerializerTests
{
    [Fact]
    public void SerializeDeserialize_RoundTrip_PreservesComponents()
    {
        var src = new World();
        var e = src.Create();
        src.AddComponent(e, new SerPos { X = 10, Y = 20 });
        src.AddComponent(e, new SerHealth { Max = 100, Current = 75 });

        var serializer = new WorldSerializer();
        var json = serializer.Serialize(src);

        var dst = new World();
        serializer.Deserialize(json, dst);

        var entities = dst.GetEntityIds<SerPos>().ToArray();
        Assert.NotEmpty(entities);

        var pos = dst.GetComponent<SerPos>(new Entity(entities[0]));
        Assert.Equal(10, pos.X);
        Assert.Equal(20, pos.Y);

        var hp = dst.GetComponent<SerHealth>(new Entity(entities[0]));
        Assert.Equal(100, hp.Max);
        Assert.Equal(75, hp.Current);
    }

    [Fact]
    public void SerializeDeserialize_MultipleEntities()
    {
        var src = new World();
        var a = src.Create(); src.AddComponent(a, new SerPos { X = 1, Y = 2 });
        var b = src.Create(); src.AddComponent(b, new SerPos { X = 3, Y = 4 });

        var serializer = new WorldSerializer();
        var json = serializer.Serialize(src);

        var dst = new World();
        serializer.Deserialize(json, dst);

        var ids = dst.GetEntityIds<SerPos>().ToArray();
        Assert.Equal(2, ids.Length);
    }

    [Fact]
    public void SerializeDeserialize_MultipleComponentTypes()
    {
        var src = new World();
        var e = src.Create();
        src.AddComponent(e, new SerPos());
        src.AddComponent(e, new SerHealth());
        src.AddComponent(e, new SerLabel { Name = "Test" });

        var serializer = new WorldSerializer();
        var json = serializer.Serialize(src);

        var dst = new World();
        serializer.Deserialize(json, dst);

        var label = dst.GetComponent<SerLabel>(e);
        Assert.Equal("Test", label.Name);
    }

    [Fact]
    public void Deserialize_EmptyJson_DoesNothing()
    {
        var serializer = new WorldSerializer();
        var dst = new World();

        serializer.Deserialize("{}", dst);

        Assert.Empty(dst.GetEntityIds<SerPos>());
    }

    [Fact]
    public void RoundTrip_PreservesEntityCount()
    {
        var src = new World();
        for (int i = 0; i < 10; i++)
        {
            var e = src.Create();
            src.AddComponent(e, new SerPos { X = i, Y = i * 2 });
        }

        var serializer = new WorldSerializer();
        var json = serializer.Serialize(src);

        var dst = new World();
        serializer.Deserialize(json, dst);

        Assert.Equal(10, dst.GetEntityIds<SerPos>().Count());
    }
}
