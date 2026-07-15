# Towers & Lanes System Documentation

## Overview

Sistema completo de torretas y carriles:
- **3 Lanes**: Top, Mid, Bottom
- **Torres defensivas**: 2-3 torres por carril
- **Vision**: Torres proporcionan visión
- **Gold reward**: Oro por destruir torres
- **Progresión**: Destruir torres abre camino a base enemiga

## Components

### Lane
Representa un carril del mapa:

**Configuración:**
- Type: Top, Mid, Bottom
- Tower positions: Donde spawnan torres
- Ally/Enemy spawn points: Donde spawnan minions
- Lane width: Ancho del carril

**API:**
```csharp
Lane topLane = mapManager.GetLane(Lane.LaneType.Top);
Tower[] towers = topLane.GetTowers();
int destroyedCount = topLane.GetDestroyedTowerCount();
```

### Tower
Torre defensiva del equipo:

**Stats (from TowerData):**
- Health: 1800
- Attack Damage: 100
- Attack Range: 12
- Attack Speed: 1 att/s
- Vision Range: 15
- Armor: 40

**Behavior:**
1. Busca enemigos en rango
2. Prioriza heroes sobre minions
3. Ataca cada segundo
4. Proporciona visión al equipo
5. Da oro al destruirse

**Ejemplo:**
```csharp
// Torre ataca
tower.TakeDamage(hero.GetAttackDamage());

// Si muere
if (tower.GetHealth() <= 0) {
    tower.Destroy();
    team.AddGold(200);
}
```

### TowerManager (Singleton)
Gestor central de todas las torres:

**Features:**
- Registra torres por equipo
- Rastrea destrucciones
- Eventos globales

**API:**
```csharp
TowerManager manager = TowerManager.Instance;

// Get torres del equipo
var blueTowers = manager.GetTowersByTeam(0);

// Contar torres destruidas
int destroyed = manager.GetDestroyedTowerCount(0);

// Check si base destruida
bool baseDown = manager.IsTeamBaseDestroyed(1);
```

### MapManager (Singleton)
Gestor del mapa completo:

**Features:**
- Gestiona 3 carriles
- Posiciones de bases
- Áreas de jungla

**API:**
```csharp
MapManager map = MapManager.Instance;

// Get carril
Lane midLane = map.GetLane(Lane.LaneType.Mid);

// Get todas las lanes
Lane[] allLanes = map.GetAllLanes();
```

### TowerHUD
UI mostrando estado de torres:

**Display:**
- Fila de indicadores de torres azules
- Fila de indicadores de torres rojas
- Color: Azul/Rojo si activa, Gris si destruida

### TowerStatusPanel
Panel de información individual de torre:

**Muestra:**
- Nombre de torre
- Barra de salud
- Texto de salud (HP/Max HP)

### DefensiveStructure
Edificios defensivos menores

**Stats:**
- Health: 500 (configurable)
- Attack: 50 daño
- Range: 8 units

### LaneMinionController
Controlador de minions en cada carril:
- Gestiona minions spawneados
- Interacción con torres
- Pathing del carril

---

## Tower Layout

```
                  RED BASE
                     ||
              RED INNER TOWER
                     ||
              RED MID TOWER
                     ||
              RED OUTER TOWER
                     ||
            NEUTRAL TERRAIN (Jungla)
                     ||
              BLUE OUTER TOWER
                     ||
              BLUE MID TOWER
                     ||
              BLUE INNER TOWER
                     ||
                  BLUE BASE
```

**Cada carril (Top, Mid, Bottom):**
- Outer Tower (más alejada)
- Mid Tower (centro)
- Inner Tower (cerca de base)

**Total: 9 torres (3 lanes x 3 torres)**

---

## Tower Targeting Priority

1. **Heroes**: Más prioritario
2. **Minions**: Si no hay heroes
3. **Estructura defensiva**: Ultimo recurso

**Fórmula:**
```
Target = Closest enemy within range
IF hero exists in range:
  prioritizeHeroes = true
ELSE:
  target minion
```

---

## Gold Rewards

| Evento | Gold |
|--------|------|
| Kill torre | 200g |
| Destroy inner tower | +50g bonus |
| Destroy mid tower | +25g bonus |
| Destroy outer tower | +10g bonus |

**Distribución:**
- Killer: 50% del oro
- Nearby allies: 50% distribuido

---

## Vision from Towers

Cada torre proporciona:
- 20 units de visión
- Auto-registrada en FogOfWarSystem
- Permanente mientras esté viva

**Ventaja:**
- Torres controlan FOW
- Permiten ver emboscadas
- Control de mapa

---

## Lane Progression

```
Minute 0: Minions spawn en outer tower
          Equipos avanzan a pelear
          
Minute 5: Primera torre muere (outer)
          Equipos avanzan hacia mid tower
          
Minute 10: Mid tower cae
          Presencia cerca de base aumenta
          
Minute 15: Inner tower bajo ataque
          Base vulnerable
          
Minute 20+: Push final hacia base
           Destruir todas las torres = Victoria
```

---

## Tower Integration with Combat

```csharp
// Hero ataca torre
tower.TakeDamage(hero.GetAttackDamage());

// Torre contraataca
tower.FireAtTarget();  // Damage a hero

// Torre destruida
if (tower.IsDestroyed()) {
    goldSystem.AddGold(200);
    experienceSystem.AddExperience(200);
}
```

---

## Defensive Structures

**Edificios secundarios:**
- Inhibitor: Previene respawn de minions (TODO)
- Ward posts: Puntos para colocar wards (TODO)
- Turrets: Mini-torres (DefensiveStructure)

---

## Server Synchronization

Todo el estado de torres es **deterministic**:
- Posiciones fijas
- Health sincronizado desde servidor
- Destrucciones validadas
- Rewards distribuidas por servidor

**Flow:**
```
Client ataca torre
    ↓
Envía comando al servidor
    ↓
Servidor calcula daño
    ↓
Servidor actualiza health de torre
    ↓
Servidor envia snapshot
    ↓
Client recibe update
    ↓
GUI actualiza health bar
```
