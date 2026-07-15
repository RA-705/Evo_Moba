# ADR 0005: Unity Visual Abandonado

- **Fecha**: 2026-07-02
- **Estado**: Aceptado

## Contexto

Se intentó crear un cliente visual en Unity 6000.5.1f1 que leyera snapshots del servidor ECS headless. Después de varias sesiones de debug, la integración resultó frágil:

- Conflictos de `EntityId` entre `Evo.Core.ECS` y `UnityEngine.EntityId`
- `global using System.Diagnostics` colisionando con `UnityEngine.Debug`
- La dependencia de `csc.rsp` con `-langVersion:12` para collection expressions
- Dificultad para mantener sincronizados los ~86 archivos `.cs` entre solución .NET y Unity
- El tiempo de debug superó el valor de tener un render visible

## Decisión

Se abandona el cliente visual Unity. El proyecto EVO se enfoca exclusivamente en:

1. **Servidor headless** (ya funcional, 60 tests, 0 warnings, jugable)
2. **Benchmarks** (BenchmarkDotNet ya configurado)
3. **CI/CD** (GitHub Actions ya funcionando)
4. **NuGet packages** (4 core packages listos)

## Consecuencias

- El directorio `unity/` se archiva pero no se mantiene activamente
- El adaptador `Evo.UnityAdapter` queda como referencia no soportada
- El servidor headless es el único target de ejecución
- Cualquier futuro cliente visual debería ser una aplicación separada que consuma snapshots UDP, no un proyecto dentro del mismo repo con sync de código
