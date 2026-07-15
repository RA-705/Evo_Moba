# ADR-0002: Entity Component System (ECS) sobre OOP Tradicional

**Estado:** Aceptado

## Contexto

Un MOBA puede tener cientos de entidades activas (héroes, súbditos, torres, proyectiles) que deben actualizarse simultáneamente cada tick del servidor (10–60 Hz). Con Programación Orientada a Objetos tradicional (clases como `class Minion : MonoBehaviour`), cada entidad es un objeto en el heap, lo que provoca:

- **Fragmentación de caché:** los objetos se dispersan en la RAM, y la CPU pierde ciclos trayendo datos de distintas partes de la memoria.
- **Presión del GC:** la creación y destrucción constante de objetos (proyectiles, súbditos muertos) forza pausas de Garbage Collection.
- **Acoplamiento:** las jerarquías de herencia (Minion → Unit → Entity) hacen difícil añadir nuevos comportamientos sin tocar clases existentes.

## Decisión

Implementar un **Entity Component System (ECS)** en `Evo.Core.ECS` con el siguiente diseño:

- **Entities** son identificadores ligeros (`EntityId : StrongId`, un `int` encapsulado). No contienen datos ni comportamiento.
- **Components** son `struct`s que implementan `IComponent` y solo contienen datos planos (ej. `PositionComponent { EvoVector3 Value }`, `HealthComponent { float Current; float Max; }`). Son `readonly record struct` en los casos que aplique.
- **Systems** son clases que implementan `ISystem` con un método `OnTick(World, float)`. Cada sistema iterra entidades que poseen los componentes que le interesan (`world.GetEntityIds<T>()`) y ejecuta lógica.
- **World** contiene `ComponentPool<T>` (basado en `Dictionary<EntityId, T>` interno) y expone acceso por `ref` a los componentes mediante `CollectionsMarshal.GetValueRefOrNullRef` para mutación directa en memoria, sin copias.

```
EntityId (5) → ComponentPool<PositionComponent> → _components[5] = ref PositionComponent
             → ComponentPool<HealthComponent>    → _components[5] = ref HealthComponent
             → ComponentPool<TeamComponent>      → _components[5] = ref TeamComponent
```

## Consecuencias

### Positivas

- **Rendimiento predecible:** los componentes se almacenan en arrays/diccionarios contiguos; la iteración es amigable con la caché de la CPU.
- **Data-Oriented Design:** los sistemas procesan datos en lote, no objetos individuales.
- **Mutación directa:** `ref T` permite modificar structs inline sin copiarlos fuera y dentro del pool.
- **Composición sobre herencia:** cualquier entidad puede tener cualquier combinación de componentes. No hay jerarquías rígidas.
- **Cero GC en hot path:** los structs se asignan en el heap solo cuando el diccionario interno crece. La iteración no genera basura.

### Negativas

- **Curva de aprendizaje:** los desarrolladores acostumbrados a `class Minion : MonoBehaviour` deben adaptarse a pensar en términos de datos y sistemas separados.
- **Código menos intuitivo:** operaciones simples como "mover una entidad" requieren coordinar varios sistemas (PathfollowSystem + VelocityComponent), en lugar de un método `.Move()` directo.
- **Sin polimorfismo:** no hay herencia de componentes; cada tipo de comportamiento se modela con datos, no con clases.
