using UnityEngine;
using Evo.AI;

namespace Evo.Battle
{
    public enum DamageType { Physical, Magic, True }

    public static class CombatManager
    {
        public static float CalculateDamage(float attackDamage, float abilityPower, float targetArmor,
            float targetMagicResist, DamageType damageType, bool isCrit = false, float critMultiplier = 2f)
        {
            float baseDamage = damageType == DamageType.Magic ? abilityPower : attackDamage;
            float mitigation = damageType switch
            {
                DamageType.Physical => targetArmor / (targetArmor + 100f),
                DamageType.Magic => targetMagicResist / (targetMagicResist + 100f),
                DamageType.True => 0f,
                _ => 0f
            };
            float finalDamage = baseDamage * (1f - mitigation);
            if (isCrit) finalDamage *= critMultiplier;
            return Mathf.Max(1f, finalDamage);
        }

        public static float CalculateAbilityDamage(float baseDamage, float scalingFactor, float scalingStat,
            DamageType damageType, float targetArmor, float targetMagicResist)
        {
            float rawDamage = baseDamage + scalingFactor * scalingStat;
            float mitigation = damageType switch
            {
                DamageType.Physical => targetArmor / (targetArmor + 100f),
                DamageType.Magic => targetMagicResist / (targetMagicResist + 100f),
                DamageType.True => 0f,
                _ => 0f
            };
            return Mathf.Max(1f, rawDamage * (1f - mitigation));
        }

        public static float CalculateHeal(float baseHeal, float scalingFactor, float abilityPower)
        {
            return baseHeal + scalingFactor * abilityPower;
        }

        public static bool IsInRange(Vector3 attacker, Vector3 target, float range)
        {
            float dist = Vector3.Distance(attacker, target);
            return dist <= range;
        }

        public static void ApplyDamage(HeroController source, HeroController target, float amount, DamageType type)
        {
            if (source == null || target == null || !target.IsAlive) return;
            float finalDmg = CalculateDamage(amount, 0, target.Armor, target.MagicResist, type);
            target.TakeDamage(finalDmg, source);
        }
    }
}
