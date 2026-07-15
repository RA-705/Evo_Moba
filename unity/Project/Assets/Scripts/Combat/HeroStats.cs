using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Gestiona la salud, armadura y resistencias de una entidad combativa
/// </summary>
public class HeroStats : MonoBehaviour
{
    [SerializeField] private HeroData heroData;
    private int _level = 1;
    private int _currentHealth;
    private int _currentMana;
    private int _gold;
    
    private BuffSystem _buffSystem = new();
    private List<CCEffectHandler.CCEffect> _activeCC = new();
    
    // Stats modificados por items y buffs
    private int _totalHealth;
    private int _totalMana;
    private float _totalAD;
    private float _totalAP;
    private float _totalAS;  // Attack Speed
    private float _totalMS;  // Movement Speed
    private int _totalArmor;
    private int _totalMR;
    
    private List<ItemData> _equippedItems = new();
    
    private bool _isDead;
    public bool IsDead => _isDead;
    
    public event System.Action<int> OnHealthChanged;
    public event System.Action OnDeath;
    public event System.Action OnRespawn;
    
    private void Start()
    {
        ResetStats();
    }
    
    public void ResetStats()
    {
        _level = 1;
        _totalHealth = heroData.baseHealth;
        _totalMana = heroData.baseMana;
        _totalAD = heroData.baseAttackDamage;
        _totalAP = 0;
        _totalAS = heroData.baseAttackSpeed;
        _totalMS = heroData.baseMovementSpeed;
        _totalArmor = heroData.baseArmor;
        _totalMR = heroData.baseMagicResist;
        _currentHealth = _totalHealth;
        _currentMana = _totalMana;
        _isDead = false;
    }
    
    public void LevelUp()
    {
        _level++;
        _totalHealth += heroData.healthPerLevel;
        _totalMana += heroData.manaPerLevel;
        _totalAD += heroData.damagePerLevel;
        _currentHealth = _totalHealth;  // Full heal on level up
        _currentMana = _totalMana;
    }
    
    /// <summary>
    /// Equipa un item y actualiza stats
    /// </summary>
    public void EquipItem(ItemData item)
    {
        _equippedItems.Add(item);
        RecalculateStats();
    }
    
    /// <summary>
    /// Recalcula todos los stats basados en hero base + items + buffs
    /// </summary>
    private void RecalculateStats()
    {
        // Reset a base
        _totalHealth = heroData.baseHealth + (_level - 1) * heroData.healthPerLevel;
        _totalMana = heroData.baseMana + (_level - 1) * heroData.manaPerLevel;
        _totalAD = heroData.baseAttackDamage + (_level - 1) * heroData.damagePerLevel;
        _totalAP = 0;
        _totalAS = heroData.baseAttackSpeed;
        _totalMS = heroData.baseMovementSpeed;
        _totalArmor = heroData.baseArmor;
        _totalMR = heroData.baseMagicResist;
        
        // Añadir items
        foreach (var item in _equippedItems)
        {
            _totalHealth += item.healthBonus;
            _totalMana += item.manaBonus;
            _totalAD += item.attackDamageBonus;
            _totalAP += item.abilityPowerBonus;
            _totalAS *= (1 + item.attackSpeedBonus / 100f);
            _totalMS *= (1 + item.movementSpeedBonus / 100f);
            _totalArmor += (int)item.armorBonus;
            _totalMR += (int)item.magicResistBonus;
        }
        
        // Añadir buffs
        _totalAD += _buffSystem.GetTotalADBonus();
        _totalAP += _buffSystem.GetTotalAPBonus();
        _totalArmor += (int)_buffSystem.GetTotalArmorBonus();
        _totalMR += (int)_buffSystem.GetTotalMRBonus();
        
        // Clamp current health/mana
        _currentHealth = Mathf.Min(_currentHealth, _totalHealth);
        _currentMana = Mathf.Min(_currentMana, _totalMana);
    }
    
    /// <summary>
    /// Recibe daño
    /// </summary>
    public void TakeDamage(float damage, bool isMagicDamage = true)
    {
        if (_isDead) return;
        
        float resistance = isMagicDamage ? _totalMR : _totalArmor;
        float reduction = resistance / (resistance + 100f);
        float actualDamage = damage * (1 - reduction);
        
        _currentHealth -= (int)actualDamage;
        OnHealthChanged?.Invoke(_currentHealth);
        
        if (_currentHealth <= 0)
        {
            Die();
        }
    }
    
    /// <summary>
    /// Recibe sanación
    /// </summary>
    public void Heal(float healing)
    {
        _currentHealth = Mathf.Min(_currentHealth + (int)healing, _totalHealth);
        OnHealthChanged?.Invoke(_currentHealth);
    }
    
    /// <summary>
    /// Consume mana para castear habilidad
    /// </summary>
    public bool TryConsumeMana(int amount)
    {
        if (_currentMana >= amount)
        {
            _currentMana -= amount;
            return true;
        }
        return false;
    }
    
    /// <summary>
    /// Regenera mana (cada tick)
    /// </summary>
    public void RegenerateMana(float regenPerTick)
    {
        _currentMana = Mathf.Min(_currentMana + (int)regenPerTick, _totalMana);
    }
    
    /// <summary>
    /// Aplica efecto de CC
    /// </summary>
    public void ApplyCC(CCEffectHandler.CCEffect effect)
    {
        _activeCC.Add(effect);
    }
    
    /// <summary>
    /// Obtiene multiplicador de velocidad considerando slows
    /// </summary>
    public float GetMovementSpeedMultiplier()
    {
        return CCEffectHandler.GetMovementSpeedMultiplier(_activeCC);
    }
    
    /// <summary>
    /// Puede castear?
    /// </summary>
    public bool CanCast()
    {
        return !_isDead && CCEffectHandler.CanCastAbility(_activeCC);
    }
    
    /// <summary>
    /// Puede moverse?
    /// </summary>
    public bool CanMove()
    {
        return !_isDead && CCEffectHandler.CanMove(_activeCC);
    }
    
    public void Die()
    {
        _isDead = true;
        OnDeath?.Invoke();
        Debug.Log($"{gameObject.name} died!");
    }
    
    public void Respawn()
    {
        _isDead = false;
        _currentHealth = _totalHealth;
        _currentMana = _totalMana;
        _activeCC.Clear();
        OnRespawn?.Invoke();
    }
    
    public void AddGold(int amount)
    {
        _gold += amount;
    }
    
    // Getters
    public int GetHealth() => _currentHealth;
    public int GetMaxHealth() => _totalHealth;
    public int GetMana() => _currentMana;
    public int GetMaxMana() => _totalMana;
    public float GetAttackDamage() => _totalAD;
    public float GetAbilityPower() => _totalAP;
    public float GetAttackSpeed() => _totalAS;
    public float GetMovementSpeed() => _totalMS * GetMovementSpeedMultiplier();
    public int GetArmor() => _totalArmor;
    public int GetMagicResist() => _totalMR;
    public int GetGold() => _gold;
    public int GetLevel() => _level;
}
