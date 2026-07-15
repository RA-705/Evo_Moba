# EVO Core

Zero-dependency C# ECS engine + MATE AI framework. Game-agnostic core, MOBA sample included.

```
Solution (9 projects)
├── platform/          ← NuGet packages (EVO.Foundation, EVO.Core, EVO.Shared)
│   └── Evo.Tests/
├── platform/          ← Includes MateOS.Core AI framework (NuGet)
├── samples/           ← Sample games (Evo.Gameplay, Evo.MOBA)
├── adapters/          ← Unity adapter (Evo.UnityAdapter)
├── tools/             ← Dedicated server (MATE.Playground)
└── benchmarks/        ← Performance benchmarks (BenchmarkDotNet)
```

## Core Packages

| Package | Description |
|---------|-------------|
| **EVO.Foundation** | `Result<T>`, `Option<T>`, `Guard`, `StrongId<T>` |
| **EVO.Core** | ECS: World, ComponentPool, ISystem, IEventBus, FixedTickLoop |
| **EVO.Shared** | EvoVector3, blittable snapshots, zero-alloc serializer |
| **MateOS.Core** | CognitiveCore, GoapPlanner, UtilitySelector |

## Build

```bash
dotnet restore Project-EVOLUTION.sln
dotnet build Project-EVOLUTION.sln --configuration Release
dotnet test platform/Evo.Tests/src/Evo.Tests/Evo.Tests.csproj --configuration Release
```
