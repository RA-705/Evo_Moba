# EVO.Shared

Shared math and wireless protocol for EVO.

## Math

- `EvoVector3` — blittable `struct` with `X, Y, Z` floats, operators, `SqrMagnitude`, `Normalize`

## Networking

- `EntitySnapshot` — 28-byte blittable entity state
- `WorldSnapshot` — fixed `byte[7168]` (up to 256 entities), zero-alloc serialization
- `SnapshotSerializer` — `Serialize`/`Deserialize` via `Span<T>` + `MemoryMarshal`
