# ADR-0003: Programación Funcional en Evo.Foundation

**Estado:** Aceptado

## Contexto

En un servidor de juego autoritativo, las excepciones son costosas: cada `throw` captura un stack trace, descarrila el pipeline de la CPU y puede causar pausas de latencia si ocurren en el hot path del tick loop. Los valores `null` son aún más peligrosos: un solo `NullReferenceException` no capturado en un hilo de fondo mata el servidor entero. Necesitamos un sistema que haga explícitos los casos de error y ausencia de valor en **tiempo de compilación**, no en tiempo de ejecución.

## Decisión

Crear la biblioteca `Evo.Foundation` con tipos funcionales que reemplazan excepciones y nulos:

- **`Result`** y **`Result<T>`** — un tipo unión que representa éxito o fracaso. En lugar de `throw` para validaciones de dominio, se retorna `Result.Failure(error)`. El llamante debe inspeccionar el resultado explícitamente (`result.IsSuccess`, `result.Match(success, failure)`).
- **`Option<T>`** — un tipo que representa un valor que puede estar presente o ausente. Reemplaza `null` de forma segura. El compilador fuerza al desarrollador a manejar ambos casos (`option.Match(some, none)` o `option.UnwrapOr(fallback)`).
- **`Guard`** — métodos estáticos de validación (`Guard.Against.Null(value)`, `Guard.Against.OutOfRange(index, max)`) que retornan `Result` en lugar de lanzar excepciones.
- **`ValueObject`** — clase base abstracta para objetos de valor inmutables con igualdad estructural basada en `GetEqualityComponents()`.
- **`StrongId`** — clase base abstracta para identificadores fuertemente tipados (como `EntityId`) que evita confusiones entre `int`s con distintos significados.

**Prohibición explícita:** ninguna excepción puede usarse para flujo de control en `Evo.MOBA` o `Evo.Core`. Las únicas excepciones permitidas son las que indican bugs (precondiciones violadas, `KeyNotFoundException` en componentes que se espera existan).

## Consecuencias

### Positivas

- **Código a prueba de nulos:** `Option<T>` hace imposible el `NullReferenceException` en las capas de dominio.
- **Errores explícitos en tipos de retorno:** el llamante sabe qué puede fallar y debe manejarlo. No hay excepciones ocultas en el hot path.
- **Servidor ultra estable:** las pausas por GC provocadas por la construcción de stack traces en excepciones frecuentes se eliminan.
- **Inmutabilidad por diseño:** `ValueObject` y `StrongId` fomentan estructuras inmutables, más fáciles de razonar en un entorno multihilo.

### Negativas

- **Verbosidad:** escribir `result.Match(success => ..., failure => ...)` ocupa más líneas que `return value;` con un `throw` opcional.
- **Fricción con librerías externas:** las APIs de .NET (LINQ, serializadores, ORMs) esperan `null` y excepciones; hay que convertir en los bordes del sistema.
- **Curva de adopción:** los desarrolladores sin experiencia funcional encuentran los patrones `Result` y `Option` incómodos al principio.
