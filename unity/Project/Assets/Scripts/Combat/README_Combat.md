# Combat System Documentation

## Overview

Sistema de combate completo para MOBA con:
- Cálculo de daño dinámico
- Escalado de atributos (AD/AP)
- Sistema de CC (Crowd Control)
- Buffs/Debuffs
- Experiencia y leveo
- Sistema de oro
- Ataques básicos + habilidades

## Core Components

### DamageCalculator
Calcula daño considerando:
- Daño base de la habilidad
- Escalado de atributos (AD, AP)
- Nivel del jugador
- Resistencias del objetivo (Armor, Magic Resist)
- Fórmula: `Reduction = Resistance / (Resistance + 100)`

**Ejemplo:**
```csharp
float damage = DamageCalculator.CalculateAbilityDamage(
    fireball,
    caster,
    casterLevel: 5,
    casterItemBonus: 40,  // AP from items
    target,
    targetLevel: 5,
    targetResistance: 30,  // Magic Resist
    isMagicDamage: true
);
```

### HeroStats
Maneja todos los stats del héroe:
- Health, Mana, AD, AP, AS, MS, Armor, MR
- Equipamiento de items
- Aplicación de CC
- Death/Respawn

**Features:**
- Recalcula stats automáticamente al equipar items
- Considera buffs activos
- Verifica si puede castear/moverse considerando CC

### AbilityExecutor
Gestiona el casteo de habilidades:
- Verifica cooldown, mana, CC
- Ejecuta lógica de habilidad
- Calcula daño
- Aplicaproperty CC effects

### AutoAttackSystem
Sistema de ataques básicos:
- Rango de ataque
- Attack speed
- Calcula daño físico
- Reset de attack timer

### ExperienceSystem
- Tabla de experiencia por nivel (0-17)
- Detección automática de level up
- Incrementa stats al subir de nivel

### GoldSystem
- Gold por kills (300g)
- Gold por assists (150g)
- Gold por minions (50g)
- Gold por towers (200g)
- Sistema de compra (items)

### CombatHUD
UI de combate mostrando:
- Health/Mana bars
- Experience bar
- Gold count
- Ability cooldowns (Q, W, E, R)

## Combat Flow

1. **Jugador castea habilidad Q** → AbilityExecutor valida
2. **Consume mana** → HeroStats.TryConsumeMana()
3. **Calcula daño** → DamageCalculator.CalculateAbilityDamage()
4. **Aplica a objetivo** → HeroStats.TakeDamage()
5. **Aplica CC** → CCEffectHandler
6. **Objetivo recibe reward** → GoldSystem, ExperienceSystem

## CC System

Tipos de CC:
- **Stun**: No puede moverse ni castear
- **Slow**: Reduce velocidad de movimiento
- **Root**: No puede moverse pero puede castear
- **Silence**: No puede castear pero puede moverse
- **Knockback**: Empuja hacia una dirección

**Aplicar CC:**
```csharp
var slowEffect = new CCEffectHandler.CCEffect
{
    type = CCEffectHandler.CCType.Slow,
    duration = 2f,
    value = 40f  // 40% slow
};
enemy.ApplyCC(slowEffect);
```

## Buff System

Buffs dinámicos que modifican stats temporalmente:
- AD/AP bonuses
- Armor/MR bonuses
- Movement speed bonuses
- Attack speed bonuses

**Agregar buff:**
```csharp
var buff = new BuffSystem.Buff
{
    buffId = "attack_boost",
    duration = 5f,
    adBonus = 30f
};
buffSystem.AddBuff(buff);
```

## Resistances

**Armor** → Reduce daño físico
**Magic Resist** → Reduce daño mágico

**Fórmula:** `Damage Reduction = Resistance / (Resistance + 100)`

**Ejemplo:**
- 100 Armor = 50% reduction
- 200 Armor = 66% reduction
- Max reduction ~= 100% (pero nunca 100%)

## Scaling

**AD Scaling**: Daño que aumenta con Attack Damage del héroe
**AP Scaling**: Daño que aumenta con Ability Power (AP)

**Ejemplo fireball:**
- Base damage: 150
- Escalado: 0.4 AD + 0.8 AP
- Con 50 AD + 100 AP: 150 + (0.4 × 50) + (0.8 × 100) = 150 + 20 + 80 = 250 damage

## Level-Up Progression

```
Level 1: 0 XP
Level 2: 100 XP
Level 3: 260 XP
Level 4: 480 XP
...
Level 17: 8800 XP
```

Cada nivel:
- +Health
- +Mana
- +Attack Damage

## Integration with Server

Todo el cálculo de daño es **CLIENT-SIDE PREDICTION**.
El servidor verifica y confirma:
- Daño real aplicado
- Kills/Assists
- Gold otorgado
- Experience otorgado
