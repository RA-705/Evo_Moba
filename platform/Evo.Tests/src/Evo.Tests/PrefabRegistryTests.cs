using Evo.Core.ECS;
using Evo.Core.Prefabs;

namespace Evo.Tests;

struct PrefabHealth : IComponent
{
    public float Max;
    public float Current;
}

struct PrefabLabel : IComponent
{
#pragma warning disable CS0649
    public string Name;
#pragma warning restore CS0649
}

public class PrefabRegistryTests
{
    [Fact]
    public void Parse_LoadsPrefabs()
    {
        var json = """
        {
          "Hero": {
            "Evo.Tests.PrefabHealth, Evo.Tests": { "Max": 500, "Current": 500 },
            "Evo.Tests.PrefabLabel, Evo.Tests": { "Name": "TestHero" }
          }
        }
        """;

        var reg = PrefabRegistry.Parse(json);
        var prefab = reg.Get("Hero");

        Assert.Equal("Hero", prefab.Name);
        Assert.Equal(2, prefab.Components.Count);
    }

    [Fact]
    public void Get_ThrowsForMissing()
    {
        var reg = PrefabRegistry.Parse("{}");

        Assert.Throws<KeyNotFoundException>(() => reg.Get("Missing"));
    }

    [Fact]
    public void TryGet_ReturnsFalseForMissing()
    {
        var reg = PrefabRegistry.Parse("{}");

        var found = reg.TryGet("Missing", out _);

        Assert.False(found);
    }

    [Fact]
    public void All_ReturnsAllPrefabs()
    {
        var json = """
        {
          "A": { "Evo.Tests.PrefabHealth, Evo.Tests": { "Max": 100, "Current": 100 } },
          "B": { "Evo.Tests.PrefabHealth, Evo.Tests": { "Max": 200, "Current": 200 } }
        }
        """;

        var reg = PrefabRegistry.Parse(json);

        Assert.Equal(2, reg.All.Count);
    }

    [Fact]
    public void Spawn_CreatesEntityWithComponents()
    {
        var json = """
        {
          "Orc": {
            "Evo.Tests.PrefabHealth, Evo.Tests": { "Max": 300, "Current": 300 },
            "Evo.Tests.PrefabLabel, Evo.Tests": { "Name": "Grunt" }
          }
        }
        """;

        var world = new World();
        var reg = PrefabRegistry.Parse(json);

        var entity = world.Spawn("Orc", reg);

        var health = world.GetComponent<PrefabHealth>(entity);
        Assert.Equal(300, health.Max);
        Assert.Equal(300, health.Current);

        var label = world.GetComponent<PrefabLabel>(entity);
        Assert.Equal("Grunt", label.Name);
    }

    [Fact]
    public void Parse_EmptyJson_ReturnsEmpty()
    {
        var reg = PrefabRegistry.Parse("{}");

        Assert.Empty(reg.All);
    }

    [Fact]
    public void Spawn_MultipleEntities_EachHasOwnComponents()
    {
        var json = """
        {
          "Unit": {
            "Evo.Tests.PrefabHealth, Evo.Tests": { "Max": 100, "Current": 100 }
          }
        }
        """;

        var world = new World();
        var reg = PrefabRegistry.Parse(json);

        var a = world.Spawn("Unit", reg);
        var b = world.Spawn("Unit", reg);

        ref var ha = ref world.GetComponent<PrefabHealth>(a);
        ha.Current = 50;

        var hb = world.GetComponent<PrefabHealth>(b);
        Assert.Equal(100, hb.Current);
    }
}
