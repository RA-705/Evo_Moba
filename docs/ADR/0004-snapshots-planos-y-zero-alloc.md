# ADR-0004: Snapshots Planos Blittable y Zero-Allocation

**Estado:** Aceptado

## Contexto

El servidor autoritativo debe enviar el estado completo del mundo a los clientes entre 10 y 60 veces por segundo. Cada envío incluye la posición, vida, equipo y estado de cada entidad visible. Si la serialización usa JSON, protocol buffers con asignación de objetos, o structs anidados complejos, ocurren dos problemas graves:

1. **Presión de GC:** cada paquete serializado crea objetos en el heap que el recolector de basura debe limpiar, provocando pausas impredecibles en el servidor.
2. **Ancho de banda desperdiciado:** la sobrecarga de texto (JSON) o metadatos de campo (Protobuf) multiplica el tamaño del paquete.

Además, queremos que el cliente (Unity) pueda leer los datos de red **sin conocer el ECS interno**, solo recibiendo bytes planos.

## Decisión

Crear contratos de red en `Evo.Shared.Networking` usando exclusivamente **structs blittable** (tipos primitivos: `float`, `int`, `uint`) y buffers de tamaño fijo para cero asignaciones:

- **`EntitySnapshot`** — struct de 28 bytes con campos planos: `EntityId` (int), `PosX/Y/Z` (float), `CurrentHealth` (float), `TeamId` (int), `StateFlags` (uint). Sin referencias, sin sub-objetos, sin strings.
- **`WorldSnapshot`** — struct `unsafe` con `fixed byte EntitiesRaw[7168]` (buffer inline para 256 entidades × 28 bytes). Incluye `TickNumber` (uint) y `EntityCount` (int). El acceso a las entidades se hace mediante `Span<EntitySnapshot>` que apunta directamente al buffer fijo, sin copias.
- **`SnapshotGenerator`** — en el servidor (`Evo.MOBA.Networking`), itera las entidades del ECS y escribe los `EntitySnapshot`s directamente en el buffer fijo mediante el `Span`. **Cero asignaciones de memoria en el hot path** — ni una sola `new` dentro del bucle.
- **`ClientReplicator`** — en el cliente (Unity), recibe el `WorldSnapshot` ya deserializado, busca la `EntityView` correspondiente y aplica `Vector3.Lerp` para interpolación suave entre ticks.

```
ECS World → SnapshotGenerator (0 alloc) → fixed buffer → red → ClientReplicator → Lerp → Transform
```

## Consecuencias

### Positivas

- **Reducción drástica del GC Pause en el servidor:** el buffer de 7 KB se reusa cada tick; no hay objetos nuevos.
- **Ancho de banda mínimo:** 28 bytes por entidad, sin sobrecarga de serialización.
- **Copia directa a la pila de red:** el buffer binario puede enviarse con `Socket.Send` directamente (previo encabezado de longitud).
- **Separación de concerns:** el cliente recibe datos planos y no necesita referencia al ECS ni a `Evo.Core`.

### Negativas

- **Mapeo manual:** cada campo del ECS (`EvoVector3`, `HealthComponent`, etc.) debe copiarse explícitamente a los campos planos del snapshot (`PosX`, `PosY`, `PosZ`, `CurrentHealth`). No hay magia ni reflection.
- **Tamaño fijo máximo:** el límite de 256 entidades por snapshot es arbitrario. Si el juego escala más allá, habrá que ajustar la constante.
- **Código `unsafe`:** el buffer fijo requiere `AllowUnsafeBlocks`, lo que desactiva algunas verificaciones del runtime en esa unidad de compilación.
- **Mantenimiento frágil:** si se agrega un campo a una entidad (ej. `ManaComponent`), hay que actualizar `EntitySnapshot`, `SnapshotGenerator` y `ClientReplicator` manualmente.
