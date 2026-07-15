# EVO.Core

Cache-friendly ECS engine for .NET 8 + Unity. No heap alloc in hot path.

## Concepts

- **World**: single container of all entities and component pools
- **ComponentPool\<T\>**: `Dictionary<EntityId, Box<T>>` with ref return — portable across NET8 + Unity Mono
- **ISystem**: `OnTick(World, float dt)` — iterate, query, mutate
- **IEventBus**: decoupled pub/sub via `Publish<T>` / `Subscribe<T>`
- **FixedTickLoop**: deterministic `TimeSpan`-based ticker

## Example

```csharp
var world = new World();
world.AddSystem(new MovementSystem());

var e = world.Create();
world.AddComponent(e, new PositionComponent { Value = new(0,0,0) });
world.AddComponent(e, new VelocityComponent { Value = new(1,0,0) });

world.Tick(1f/60f);
```
