# Minion & Creep System Documentation

## Overview

Sistema completo de minions y creeps neutros con:
- Oleadas automáticas cada 30 segundos
- Minions sitiadores cada 2 minutos
- Creeps neutros con respawn
- Sistema de recompensas (oro, XP)
- IA de comportamiento básica

## Components

### MinionData (ScriptableObject)
Define características de minion:
- Health, Attack Damage, Attack Speed
- Movement Speed, Armor, Magic Resist
- Gold & XP reward
- Vision range, Aggression range

**Ejemplo:**
```json
{
  "minionId": "ranged_minion",
  "minionName": "Ranged Minion",
  "health": 50,
  "attackDamage": 5,
  "attackSpeed": 1,
  "goldReward": 20,
  "experienceReward": 60
}
```

### MinionAI
Comportamiento de IA para minions:
- **Targeting**: Busca enemigos dentro de vision range
- **Movement**: Patrulla o se mueve hacia objetivo
- **Combat**: Ataca cuando está en rango
- **Death**: Triggerea evento con equipo ganador

**Features:**
- Pathfinding simple (movimiento directo)
- Attack cooldown basado en attack speed
- Physics-based movement

### CreepWaveSpawner
Gestor automático de oleadas:
- Spawna minions cada 30 segundos (configurable)
- Spawn con delay entre minions (0.5s)
- Minions sitiadores cada 2 minutos
- Tracking de oleadas

**Configuración:**
```csharp
waveSpawnInterval = 30f;  // Cada 30s
minionsPerWave = 3;        // 3 minions por oleada
spawnDelay = 0.5f;         // Delay entre spawns
siegeMinionInterval = 120f; // Cada 2 minutos
```

### MinionManager (Singleton)
Gestor centralizado:
- Registra todos los minions
- Rastrea kills
- Distribuye recompensas
- Eventos globales

**Eventos:**
- `OnMinionSpawned`
- `OnMinionKilled`

### NeutralCreepAI
Comportamiento de creeps neutros (Buffs, Raptors, etc):
- Patrullan puntos específicos
- Atacan si son atacados
- Dropean buffs al morir
- Rewards especiales (100-300g)

**Tipos de creeps:**
- **Red Buff**: Lento + Quemadura
- **Blue Buff**: Regeneración de mana
- **Raptors**: Visión
- **Wolves**: Velocidad
- **Krugs**: Ataque rápido

### NeutralCreepSpawner
Gestor de spawning de creeps neutros:
- Respawn automático (2 minutos por defecto)
- Ubicaciones fijas en el mapa
- Control de creeps activos

### CreepRewardSystem
Distribuye oro y XP:
- 20g por minion regular
- 100-300g por creep neutral
- XP basado en tipo de creep
- Integration con GoldSystem y ExperienceSystem

### CreepWaveHUD
UI mostrando:
- Número de oleada actual
- Tiempo para próxima oleada
- Conteo de minions activos

---

## Wave Spawning Flow

```
1. CreepWaveSpawner.Update() 
2. Decrement waveTimer
3. Timer reaches 0 -> SpawnWave()
4. For each spawnPoint:
   - Spawn 3 minions con delay 0.5s
5. MinionAI instantiados
6. MinionManager.RegisterMinion()
7. Minions start moving hacia enemigos
8. Hero mata minion -> OnMinionKilled event
9. CreepRewardSystem.OnMinionKilled()
10. +20g +60xp para hero
```

## AI Behavior

### Minion AI Loop
```csharp
Update() {
  UpdateTargeting()    // Busca enemigos
  UpdateMovement()     // Move hacia objetivo
  UpdateAttacks()      // Ataca si está en rango
}
```

### Neutral Creep AI Loop
```csharp
Update() {
  if isPatrolling {
    Patrol()          // Muévete entre puntos
  } else if hasTarget {
    ChaseTarget()     // Ataca solo si lo atacan
  }
}
```

---

## Creep Types Configuration

### Regular Minions
- **Health**: 50
- **Damage**: 5
- **Gold**: 20g
- **XP**: 60xp
- **Spawning**: Cada 30s, 3 por oleada

### Siege Minions
- **Health**: 80 (más fuerte)
- **Damage**: 12
- **Gold**: 40g
- **XP**: 120xp
- **Spawning**: Cada 2 minutos, 1 por oleada

### Neutral Creeps

#### Red Buff (Krugs)
- **Gold**: 100g
- **XP**: 150xp
- **Buff**: Slow + Damage over time (2min)

#### Blue Buff (Wolves)
- **Gold**: 100g
- **XP**: 150xp
- **Buff**: Mana regeneration (2min)

#### Raptors
- **Gold**: 150g
- **XP**: 200xp
- **Buff**: Vision (2min)

#### Golems
- **Gold**: 200g
- **XP**: 250xp
- **Buff**: Shield (2min)

---

## Progression Over Time

```
Minute 0:   Wave 1 spawns
Minute 0:30: Wave 2 spawns
Minute 1:00: Wave 3 spawns
Minute 2:00: Wave 4 + SIEGE MINIONS
Minute 2:30: Wave 5 spawns
Minute 4:00: Wave 8 + SIEGE MINIONS
```

Creeps neutrales respawnean cada 2 minutos después de muertos.

---

## Integration with Combat

```csharp
// Hero mata minion
minion.Die();

// MinionManager event
OnMinionKilled?.Invoke(minion, killerTeam);

// CreepRewardSystem
creepRewardSystem.OnMinionKilled(minion, playerId);

// Gold & XP
goldSystem.AddGold(20);
experienceSystem.AddExperience(60);
```

---

## Server Synchronization

Todo el spawning de minions es **determinístico**:
- Mismo seed del servidor
- Mismo spawn time
- Client puede predecir spawns
- Server valida kills y recompensas
