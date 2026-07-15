# ADR-0001: Arquitectura Hexagonal — Unity como Renderizador Tonto

**Estado:** Aceptado

## Contexto

Necesitamos un servidor autoritativo headless para el MOBA, pero también queremos usar Unity para los gráficos y la visualización en cliente. Si la lógica del juego depende de las APIs de Unity (MonoBehaviour, Vector3, GameObject, etc.), el servidor headless no podrá compilarse, pues Unity no está disponible en un entorno de servidor .NET puro. Además, acoplar la lógica del negocio a un motor gráfico específico dificulta realizar pruebas unitarias rápidas y migrar a otro motor en el futuro.

## Decisión

Implementar **Arquitectura Hexagonal** (también conocida como Ports & Adapters):

- **Núcleo (dominio):** Toda la lógica del juego reside en C# puro, cero dependencias de Unity. Los proyectos `Evo.Core` (ECS, GameLoop), `Evo.MOBA` (sistemas de juego, combate, IA, navegación) y `Evo.Foundation` (tipos funcionales) compilan como bibliotecas de clase estándar de .NET.
- **Adaptador (infraestructura):** Unity vive exclusivamente en `adapters/Evo.UnityAdapter/`. Allí residen los `MonoBehaviour`s, los puentes (`WorldBridge`, `EvoBootstrap`) y las conversiones de tipos (`EvoVector3` → `Vector3`). El adaptador solo lee datos del núcleo y los proyecta en la escena; nunca contiene lógica de juego.
- **Comunicación:** El núcleo expone estructuras planas (`WorldSnapshot`) y el adaptador las consume. El sentido de la dependencia es **siempre** del adaptador hacia el núcleo, nunca al revés.

```
┌──────────────────────────────────────────────┐
│           adapters/Evo.UnityAdapter          │
│   (WorldBridge, ClientReplicator, Views)     │
│             Depende de →                     │
├──────────────────────────────────────────────┤
│        game/Evo.MOBA, platform/*            │
│   (Sistemas, Componentes, ECS, Foundation)   │
│             C# puro, sin Unity               │
└──────────────────────────────────────────────┘
```

## Consecuencias

### Positivas

- El servidor headless compila como una aplicación de consola .NET pura. No requiere Unity instalado.
- Las pruebas unitarias sobre la lógica del juego se ejecutan en milisegundos (sin inicialización de escena, sin MonoBehaviour).
- Podríamos reemplazar Unity por Godot, un renderizador custom en Silk.NET, o incluso una salida tipo roguelike ASCII sin modificar una línea de `Evo.MOBA` o `Evo.Core`.

### Negativas

- Overhead inicial de desarrollo: cada nuevo tipo que el cliente necesita visualizar requiere escribir un puente (conversión de `EvoVector3` a `Vector3`, asignación de componentes a `EntityView`s, etc.).
- Duplicación de tipos de datos: `EvoVector3` coexiste con `UnityEngine.Vector3`, y hay que convertirlos explícitamente.
- Mayor número de proyectos y archivos en la solución.
