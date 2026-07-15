using System.Reflection;
using System.Text.Json;
using Evo.Core.ECS;

namespace Evo.Core.Prefabs;

public static class WorldExtensions
{
    private static readonly Dictionary<string, Type> TypeCache = new(StringComparer.OrdinalIgnoreCase);

    public static Entity Spawn(this World world, string prefabName, PrefabRegistry registry)
    {
        var prefab = registry.Get(prefabName);
        return Spawn(world, prefab);
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        IncludeFields = true,
        PropertyNameCaseInsensitive = true,
    };

    public static Entity Spawn(this World world, PrefabDefinition prefab)
    {
        var entity = world.Create();

        foreach (var component in prefab.Components)
        {
            var type = ResolveType(component.TypeName);
            var instance = JsonSerializer.Deserialize(component.Data.GetRawText(), type, JsonOptions);
            if (instance is null)
                continue;

            var addMethod = typeof(World).GetMethod(nameof(World.AddComponent));
            if (addMethod is null)
                continue;

            addMethod.MakeGenericMethod(type).Invoke(world, [entity, instance]);
        }

        return entity;
    }

    private static Type ResolveType(string typeName)
    {
        if (TypeCache.TryGetValue(typeName, out var cached) && cached is not null)
            return cached;

        var type = Type.GetType(typeName);
        if (type is null)
            throw new InvalidOperationException(
                $"Cannot resolve component type '{typeName}'. Ensure the fully qualified type name " +
                "(including assembly) is correct.");

        TypeCache[typeName] = type;
        return type;
    }
}
