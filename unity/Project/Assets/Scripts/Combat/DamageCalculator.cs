using UnityEngine;
using System.Collections.Generic;

public class DamageCalculator
{
    /// <summary>
    /// Calcula el daño final de una habilidad considerando escalado y resistencias
    /// </summary>
    public static float CalculateAbilityDamage(
        AbilityData ability,
        HeroData caster,
        int casterLevel,
        int casterItemBonus,  // AD + AP from items
        HeroData target,
        int targetLevel,
        int targetResistance,  // Armor or Magic Resist
        bool isMagicDamage = true)
    {
        // 1. Base damage
        float baseDamage = ability.baseDamage;
        
        // 2. Escalado de daño
        float scaledDamage = baseDamage;
        if (isMagicDamage)
        {
            // AP Scaling
            float totalAP = caster.baseMana + casterItemBonus;  // Simplified
            scaledDamage += ability.scalingAP * totalAP;
        }
        else
        {
            // AD Scaling
            float totalAD = caster.baseAttackDamage + casterItemBonus;
            scaledDamage += ability.scalingAD * totalAD;
        }
        
        // 3. Bonificación por nivel del hechicero
        scaledDamage += ability.baseDamage * 0.02f * casterLevel;  // 2% por nivel
        
        // 4. Resistencias reduce daño
        float damageReduction = CalculateResistanceReduction(targetResistance);
        float finalDamage = scaledDamage * (1 - damageReduction);
        
        // 5. Nunca puede ser 0 o negativo
        return Mathf.Max(finalDamage, 1);
    }
    
    /// <summary>
    /// Calcula el daño de ataque básico
    /// </summary>
    public static float CalculateAutoAttackDamage(
        HeroData attacker,
        int attackerLevel,
        int itemAD,
        HeroData defender,
        int defenderArmor)
    {
        float totalAD = attacker.baseAttackDamage + itemAD + (attacker.damagePerLevel * attackerLevel);
        
        // Armor reduction
        float armorReduction = CalculateArmorReduction(defenderArmor);
        float finalDamage = totalAD * (1 - armorReduction);
        
        return Mathf.Max(finalDamage, 1);
    }
    
    /// <summary>
    /// Calcula reducción de daño por armor
    /// Formula: Reduction = Armor / (Armor + 100)
    /// </summary>
    private static float CalculateArmorReduction(int armor)
    {
        return Mathf.Clamp01((float)armor / (armor + 100));
    }
    
    /// <summary>
    /// Calcula reducción de daño por resistencia mágica
    /// Formula: Reduction = MR / (MR + 100)
    /// </summary>
    private static float CalculateResistanceReduction(int magicResist)
    {
        return Mathf.Clamp01((float)magicResist / (magicResist + 100));
    }
    
    /// <summary>
    /// Calcula penetración (reduce resistencias del enemigo)
    /// </summary>
    public static int CalculateEffectiveArmor(int baseArmor, int armorPenetration)
    {
        return Mathf.Max(0, baseArmor - armorPenetration);
    }
}

/// <summary>
/// Maneja efectos de control de multitud (CC)
/// </summary>
public class CCEffectHandler
{
    public enum CCType { Stun, Slow, Knockback, Root, Silence }
    
    [System.Serializable]
    public class CCEffect
    {
        public CCType type;
        public float duration;
        public float value;  // Porcentaje para slow, fuerza para knockback
    }
    
    public static bool CanMove(List<CCEffect> activeEffects)
    {
        foreach (var effect in activeEffects)
        {
            if (effect.type == CCType.Stun || effect.type == CCType.Root)
                return false;
        }
        return true;
    }
    
    public static float GetMovementSpeedMultiplier(List<CCEffect> activeEffects)
    {
        float multiplier = 1f;
        foreach (var effect in activeEffects)
        {
            if (effect.type == CCType.Slow)
                multiplier *= (1 - effect.value / 100f);
        }
        return Mathf.Max(multiplier, 0.2f);  // Minimum 20% speed
    }
    
    public static bool CanCastAbility(List<CCEffect> activeEffects)
    {
        foreach (var effect in activeEffects)
        {
            if (effect.type == CCType.Stun || effect.type == CCType.Silence)
                return false;
        }
        return true;
    }
}

/// <summary>
/// Maneja cálculos de sanación y lifesteal
/// </summary>
public class HealingCalculator
{
    public static float CalculateHealing(
        float baseHealing,
        float apScaling,
        float totalAP,
        HeroData healer,
        int healerLevel)
    {
        float healing = baseHealing + (apScaling * totalAP);
        healing += baseHealing * 0.01f * healerLevel;  // 1% per level
        return healing;
    }
    
    public static float CalculateLifesteal(
        float damageDealt,
        float lifestealpercent)
    {
        return damageDealt * (lifestealpercent / 100f);
    }
}

/// <summary>
/// Maneja buffs y debuffs dinámicos
/// </summary>
public class BuffSystem
{
    [System.Serializable]
    public class Buff
    {
        public string buffId;
        public string buffName;
        public float duration;
        public float elapsedTime;
        public bool isActive => elapsedTime < duration;
        
        // Stat modifications
        public float adBonus;
        public float apBonus;
        public float armorBonus;
        public float mrBonus;
        public float msBonus;  // Percentage
        public float attackSpeedBonus;  // Percentage
    }
    
    private List<Buff> _activeBuffs = new();
    
    public void AddBuff(Buff buff)
    {
        _activeBuffs.Add(buff);
    }
    
    public void RemoveBuff(string buffId)
    {
        _activeBuffs.RemoveAll(b => b.buffId == buffId && !b.isActive);
    }
    
    public void UpdateBuffs(float deltaTime)
    {
        _activeBuffs.RemoveAll(b => !b.isActive);
        foreach (var buff in _activeBuffs)
        {
            buff.elapsedTime += deltaTime;
        }
    }
    
    public float GetTotalADBonus()
    {
        float total = 0;
        foreach (var buff in _activeBuffs)
        {
            if (buff.isActive) total += buff.adBonus;
        }
        return total;
    }
    
    public float GetTotalAPBonus()
    {
        float total = 0;
        foreach (var buff in _activeBuffs)
        {
            if (buff.isActive) total += buff.apBonus;
        }
        return total;
    }
    
    public float GetTotalArmorBonus()
    {
        float total = 0;
        foreach (var buff in _activeBuffs)
        {
            if (buff.isActive) total += buff.armorBonus;
        }
        return total;
    }
    
    public float GetTotalMRBonus()
    {
        float total = 0;
        foreach (var buff in _activeBuffs)
        {
            if (buff.isActive) total += buff.mrBonus;
        }
        return total;
    }
}
