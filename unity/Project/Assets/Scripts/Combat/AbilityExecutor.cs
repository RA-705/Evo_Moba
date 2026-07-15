using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Maneja el casteo y ejecución de habilidades
/// </summary>
public class AbilityExecutor : MonoBehaviour
{
    private HeroStats _heroStats;
    private AbilityData[] _abilities = new AbilityData[4];  // Q, W, E, R
    private float[] _cooldownTimers = new float[4];
    
    public event System.Action<int> OnAbilityCast;  // int = slot
    public event System.Action<int> OnAbilityReady;  // int = slot
    
    private void Start()
    {
        _heroStats = GetComponent<HeroStats>();
    }
    
    private void Update()
    {
        UpdateCooldowns();
    }
    
    /// <summary>
    /// Intenta castear una habilidad
    /// </summary>
    public bool TryCastAbility(int slot, Vector3 targetPosition)
    {
        if (slot < 0 || slot >= 4) return false;
        if (_abilities[slot] == null) return false;
        if (_cooldownTimers[slot] > 0) return false;
        if (!_heroStats.CanCast()) return false;
        
        AbilityData ability = _abilities[slot];
        
        // Verificar mana
        if (!_heroStats.TryConsumeMana(ability.manaCost))
        {
            Debug.Log($"Not enough mana for {ability.abilityName}");
            return false;
        }
        
        // Castear
        ExecuteAbility(ability, slot, targetPosition);
        return true;
    }
    
    private void ExecuteAbility(AbilityData ability, int slot, Vector3 targetPosition)
    {
        Debug.Log($"Cast: {ability.abilityName} at {targetPosition}");
        
        // Calcular daño
        float damage = DamageCalculator.CalculateAbilityDamage(
            ability,
            GetComponent<HeroStats>().GetComponent<HeroData>(),  // Simplificado
            _heroStats.GetLevel(),
            (int)_heroStats.GetAbilityPower(),
            null,  // Target (would need to fetch)
            1,
            0,
            true
        );
        
        Debug.Log($"Ability damage: {damage}");
        
        // TODO: Aplicar daño a enemigos en rango
        // TODO: Aplicar CC effects
        // TODO: Enviar a servidor
        
        OnAbilityCast?.Invoke(slot);
        _cooldownTimers[slot] = ability.cooldown;
    }
    
    private void UpdateCooldowns()
    {
        for (int i = 0; i < 4; i++)
        {
            if (_cooldownTimers[i] > 0)
            {
                _cooldownTimers[i] -= Time.deltaTime;
                if (_cooldownTimers[i] <= 0)
                {
                    OnAbilityReady?.Invoke(i);
                }
            }
        }
    }
    
    public void SetAbility(int slot, AbilityData ability)
    {
        if (slot >= 0 && slot < 4)
        {
            _abilities[slot] = ability;
        }
    }
    
    public float GetCooldown(int slot) => Mathf.Max(0, _cooldownTimers[slot]);
    public bool IsAbilityReady(int slot) => _cooldownTimers[slot] <= 0 && _abilities[slot] != null;
    public AbilityData GetAbility(int slot) => _abilities[slot];
}
