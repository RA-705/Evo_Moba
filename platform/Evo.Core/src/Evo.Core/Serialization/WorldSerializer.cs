using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Evo.Core.ECS;
using Evo.Foundation.Common;

namespace Evo.Core.Serialization;

public sealed record ComponentData(string Type, JsonElement Data);

public sealed record EntityData(int Id, List<ComponentData> Components);

public sealed record WorldData(List<EntityData> Entities);

public sealed class WorldSerializer
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        IncludeFields = true,
        PropertyNameCaseInsensitive = true,
    };

    public string Serialize(World world)
    {
        var entities = new Dictionary<int, List<ComponentData>>();
        var pools = ReadPools(world);

        foreach (var (type, pool) in pools)
        {
            var getMethod = typeof(ComponentPool<>).MakeGenericType(type)
                .GetMethod("Get", [typeof(EntityId)]);

            if (getMethod is null)
                continue;

            foreach (var id in pool.EntityIds)
            {
                var component = getMethod.Invoke(pool, [id]);
                var json = JsonSerializer.Serialize(component, type, JsonOptions);
                var data = JsonSerializer.Deserialize<JsonElement>(json);

                if (!entities.TryGetValue(id.Value, out var list))
                {
                    list = new List<ComponentData>();
                    entities[id.Value] = list;
                }
                list.Add(new ComponentData(type.AssemblyQualifiedName ?? type.FullName ?? type.Name, data));
            }
        }

        var entityList = entities
            .OrderBy(e => e.Key)
            .Select(e => new EntityData(e.Key, e.Value))
            .ToList();

        return JsonSerializer.Serialize(new WorldData(entityList), JsonOptions);
    }

    public void Deserialize(string json, World world)
    {
        var data = JsonSerializer.Deserialize<WorldData>(json, JsonOptions);
        if (data?.Entities is null)
            return;

        foreach (var entityData in data.Entities)
        {
            var entity = world.Create();

            try
            {
                var idField = typeof(StrongId)
                    .GetField("<Value>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
                idField?.SetValue(entity.Id, entityData.Id);
            }
            catch { }

            foreach (var comp in entityData.Components)
            {
                var type = Type.GetType(comp.Type);
                if (type is null)
                    continue;

                var instance = JsonSerializer.Deserialize(comp.Data.GetRawText(), type, JsonOptions);
                if (instance is null)
                    continue;

                var addMethod = typeof(World).GetMethod(nameof(World.AddComponent))!
                    .MakeGenericMethod(type);

                addMethod.Invoke(world, [entity, instance]);
            }
        }

        if (data.Entities.Count > 0)
        {
            var maxId = data.Entities.Max(e => e.Id);
            try
            {
                var nextIdField = typeof(EntityId)
                    .GetField("_nextId", BindingFlags.Static | BindingFlags.NonPublic);
                nextIdField?.SetValue(null, maxId);
            }
            catch { }
        }
    }

    private static List<(Type Type, IComponentPool Pool)> ReadPools(World world)
    {
        var result = new List<(Type, IComponentPool)>();

        try
        {
            var poolsField = typeof(World)
                .GetField("_pools", BindingFlags.Instance | BindingFlags.NonPublic);

            if (poolsField?.GetValue(world) is Dictionary<Type, IComponentPool> pools)
            {
                foreach (var kv in pools)
                    result.Add((kv.Key, kv.Value));
            }
        }
        catch { }

        return result;
    }
}
