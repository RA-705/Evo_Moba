using System.Text.Json;

namespace Evo.Core.Prefabs;

public sealed record PrefabComponentDefinition(string TypeName, JsonElement Data);

public sealed record PrefabDefinition(string Name, IReadOnlyList<PrefabComponentDefinition> Components);
