# EVO.Foundation

Functional primitives for game server logic. Zero-dependency, zero-exception.

## Types

| Type | Purpose |
|------|---------|
| `Result<T>` | Success/failure with optional error, no try/catch |
| `Option<T>` | Explicit optional value, no nulls |
| `Guard` | Precondition checks (`AgainstNull`, `InRange`) |
| `StrongId<T>` | Type-safe entity/component IDs |
| `ValueObject` | Base for value equality |

## Usage

```csharp
public Result<HealthComponent> ApplyDamage(int amount)
{
    return Guard.Against.Negative(amount)
        .Map(_ => new HealthComponent { Value -= amount });
}
```
