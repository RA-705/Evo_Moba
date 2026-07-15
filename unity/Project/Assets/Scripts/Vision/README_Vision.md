# Minimap & Fog of War System Documentation

## Overview

Sistema completo de:
- **Fog of War (FOW)**: Visión based en posición de héroes, torres y wards
- **Minimap**: Visualización en tiempo real del mapa
- **Vision Sources**: Héroes, torres, wards, sentinelas
- **Visibility Culling**: Esconde entidades enemigas fuera de visión

## Components

### FogOfWarSystem (Singleton)
Gestor central de visión:

**Features:**
- Grid-based FOW (256x256 celdas)
- Per-team vision tracking
- Multiple vision sources
- Real-time updates

**Vision Ranges:**
- Hero: 15 units
- Tower: 20 units
- Ward: 10 units
- Sentry Ward: 8 units

**Fórmula de visión:**
```
Visible if: distance <= visionRange
Radius effect: Circular area around source
```

**API:**
```csharp
// Registrar fuente de visión
var heroVision = new FogOfWarSystem.VisionSource {
    position = hero.transform.position,
    radius = 15f,
    team = 0,
    type = FogOfWarSystem.VisionSourceType.Hero
};
fogSystem.RegisterVisionSource(heroVision);

// Verificar visibilidad
bool isVisible = fogSystem.CanSeeEntity(position, playerTeam);
```

### MinimapRenderer
Visualización en tiempo real del mapa:

**Features:**
- Minimap 256x256
- Actualiza cada frame
- Muestra FOW (negro = oscuro, gris = revelado)
- Dibuja unidades (verdes = aliados, rojos = enemigos)
- Dibuja viewport de cámara
- Click para moverse

**Colores:**
- Black: Fog (no visible)
- Gray: Revealed (visto antes)
- Green: Ally heroes
- Red: Enemy heroes/minions
- Yellow: Neutral
- Cyan: Towers

**Ejemplo de click:**
```csharp
// En minimap, click para ir a esa posición
// Convierte coordenadas minimap -> mundo
Vector3 worldPos = MinimapToWorldPosition(clickPos);
SendMoveCommand(worldPos);
```

### HeroVision
Controla visión de héroe:

**Features:**
- Rango de visión base: 15 units
- Posición updatea cada frame
- Afectado por items (upgrades)
- Per-team

**Ejemplo:**
```csharp
// En hero
var heroVision = hero.AddComponent<HeroVision>();
heroVision.SetTeam(0);  // Team 0
```

### TowerVision
Visión de torres:

**Features:**
- Rango: 20 units
- Fija en posición
- No se mueve
- Proporciona visión al equipo

### Ward
Centinela emplazada:

**Features:**
- Rango: 10 units
- Duración: 3 minutos (configurable)
- Se esconde después del tiempo
- Destruible por enemigos

**Ejemplo:**
```csharp
Ward ward = Instantiate(wardPrefab, position, Quaternion.identity);
// Automáticamente registra visión
// Desaparece después de 180s
```

### VisibilityChecker
Detector de visibilidad per-entidad:

**Features:**
- Esconde/muestra mesh renderers
- Esconde/muestra UI
- Actualiza cada frame
- Reduce vision-cheating

**Uso:**
```csharp
// En enemigo
var checker = enemy.AddComponent<VisibilityChecker>();
checker.SetTeam(1);  // Enemy team
// Si no es visible, se desactiva el renderer
```

---

## Vision System Flow

```
1. HeroVision.Update()
   - Updatea posición en VisionSource
   - Registra en FogOfWarSystem

2. FogOfWarSystem.Update()
   - Limpia mapa de visión anterior
   - Para cada VisionSource:
     - Calcula area visible
     - Agrega a _teamVisionMap
   - Actualiza texture FOW

3. MinimapRenderer.Update()
   - Lee FOW texture
   - Dibuja unidades visibles
   - Actualiza minimap texture

4. VisibilityChecker.Update()
   - Verifica si entidad es visible
   - Si no: desactiva renderer
```

---

## FOW Grid System

```
Mapa: 200 x 200 units
Grid: 256 x 256 celdas
Cell size: ~0.78 units

Cada celda puede ser:
- Oscura (0,0,0): No visible
- Revelada (0.1,0.1,0.1): Visto antes
- Visible (1,1,1): Actualmente visible
```

**Ventaja:** Bajo overhead, rápido cómputo

---

## Vision Range by Source Type

| Source | Range | Notes |
|--------|-------|-------|
| Hero | 15 | Base, puede aumentar con items |
| Tower | 20 | Mayor rango que héroe |
| Ward | 10 | Portable, 3min duración |
| Sentry | 8 | Detecta wards enemigas |
| Minion | 8 | Visión limitada |

---

## Minimap Controls

**Click en minimap:**
- Convierte click a posición del mundo
- Envía comando de movimiento
- Permite ping (TODO)

**Viewport indicator:**
- Cuadrado blanco en minimap
- Muestra área visible en cámara principal
- Ayuda a orientarse

---

## Client-Side Fog of War

**Important:** FOW es **client-side prediction**.

El servidor valida:
- Si realmente puedes ver esa entidad
- Si es un ataque válido
- Previene vision-hacking

**Flujo:**
```
Client FOW (lo que ves)
    ↓
Intenta atacar entidad escondida
    ↓
Envía comando al servidor
    ↓
Servidor: "No puedes verla" → Rechaza comando
```

---

## Performance Optimization

- Grid-based: O(1) lookup
- Texture caching: Update solo pixels que cambiaron
- Vision culling: Desactiva renderers invisibles
- Per-team tracking: Evita calcular visión de enemigos

---

## Integration with Combat

```csharp
// Al atacar
if (!fogSystem.CanSeeEntity(target.position, playerTeam)) {
    Debug.Log("Target is not visible!");
    return;  // No puedes atacar lo que no ves
}

// Ejecutar ataque
DealDamage(target);
```
