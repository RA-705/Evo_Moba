using Evo.Core.ECS;

namespace Evo.Tests;

#pragma warning disable CS0649
struct QueryCompA : IComponent
{
    public int Value;
}

struct QueryCompB : IComponent
{
    public string Label;
}

struct QueryCompC : IComponent
{
    public float Weight;
}
#pragma warning restore CS0649

public class QueryTests
{
    [Fact]
    public void Query_T2_ReturnsEntitiesWithBoth()
    {
        var w = new World();
        var a = w.Create(); w.AddComponent(a, new QueryCompA());
        var b = w.Create(); w.AddComponent(b, new QueryCompB());
        var ab = w.Create(); w.AddComponent(ab, new QueryCompA()); w.AddComponent(ab, new QueryCompB());

        var result = w.Query<QueryCompA, QueryCompB>().ToArray();

        Assert.Single(result);
        Assert.Equal(ab.Id, result[0]);
    }

    [Fact]
    public void Query_T2_WithNoneMatching_ReturnsEmpty()
    {
        var w = new World();
        w.Create(); w.AddComponent(w.Create(), new QueryCompA());
        w.Create(); w.AddComponent(w.Create(), new QueryCompB());

        var result = w.Query<QueryCompA, QueryCompB>().ToArray();

        Assert.Empty(result);
    }

    [Fact]
    public void Query_T3_ReturnsEntitiesWithAllThree()
    {
        var w = new World();
        var abc = w.Create();
        w.AddComponent(abc, new QueryCompA());
        w.AddComponent(abc, new QueryCompB());
        w.AddComponent(abc, new QueryCompC());

        w.Create(); w.AddComponent(w.Create(), new QueryCompA());
        w.Create(); w.AddComponent(w.Create(), new QueryCompB());

        var result = w.Query<QueryCompA, QueryCompB, QueryCompC>().ToArray();

        Assert.Single(result);
        Assert.Equal(abc.Id, result[0]);
    }

    [Fact]
    public void Query_T2_WithNoPools_ReturnsEmpty()
    {
        var w = new World();
        var result = w.Query<QueryCompA, QueryCompB>().ToArray();

        Assert.Empty(result);
    }
}
