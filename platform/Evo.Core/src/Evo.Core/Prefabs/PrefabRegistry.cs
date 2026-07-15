using System.Text.Json;

namespace Evo.Core.Prefabs;

public sealed class PrefabRegistry
{
    private readonly Dictionary<string, PrefabDefinition> _prefabs = new(StringComparer.OrdinalIgnoreCase);

    public IReadOnlyCollection<PrefabDefinition> All => _prefabs.Values;

    public static PrefabRegistry Load(string path)
    {
        var json = File.ReadAllText(path);
        return Parse(json);
    }

    public static PrefabRegistry Parse(string json)
    {
        var registry = new PrefabRegistry();
        var root = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, JsonElement>>>(json);

        if (root is null)
            return registry;

        foreach (var (prefabName, components) in root)
        {
            var list = new List<PrefabComponentDefinition>();
            foreach (var (typeName, data) in components)
                list.Add(new PrefabComponentDefinition(typeName, data));

            registry._prefabs[prefabName] = new PrefabDefinition(prefabName, list);
        }

        return registry;
    }

    public PrefabDefinition Get(string name)
    {
        if (!_prefabs.TryGetValue(name, out var prefab))
            throw new KeyNotFoundException($"Prefab '{name}' not found.");
        return prefab;
    }

    public bool TryGet(string name, out PrefabDefinition? prefab) =>
        _prefabs.TryGetValue(name, out prefab);
}
