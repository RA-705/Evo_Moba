using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Torre defensiva en el MOBA
/// Dispara a enemigos cercanos, proporciona visión
/// </summary>
public class Tower : MonoBehaviour
{
    [SerializeField] private TowerData towerData;
    [SerializeField] private int team;  // 0 = Azul, 1 = Rojo
    [SerializeField] private float projectileSpeed = 20f;
    
    private int _currentHealth;
    private bool _isDestroyed;
    private float _attackTimer;
    private Transform _currentTarget;
    private TowerVision _towerVision;
    
    public event System.Action OnDestroyed;
    public event System.Action<int> OnHealthChanged;  // int = new health
    
    private void Start()
    {
        _currentHealth = towerData.health;
        _towerVision = GetComponent<TowerVision>();
        
        if (_towerVision == null)
            _towerVision = gameObject.AddComponent<TowerVision>();
    }
    
    private void Update()
    {
        if (_isDestroyed) return;
        
        UpdateTargeting();
        UpdateAttacks();
    }
    
    private void UpdateTargeting()
    {
        // Buscar objetivo más cercano
        Collider[] nearby = Physics.OverlapSphere(transform.position, towerData.targetingRange);
        
        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;
        
        foreach (var collider in nearby)
        {
            // TODO: Check if enemy team
            HeroStats heroStats = collider.GetComponent<HeroStats>();
            MinionAI minion = collider.GetComponent<MinionAI>();
            
            if (heroStats != null || minion != null)
            {
                float distance = Vector3.Distance(transform.position, collider.transform.position);
                
                if (distance < closestDistance)
                {
                    // Prioridad: Heroes > Minions
                    if (towerData.prioritizeHeroes && heroStats != null)
                    {
                        closestDistance = distance;
                        closestEnemy = collider.transform;
                    }
                    else if (minion != null && (closestEnemy == null || !towerData.prioritizeHeroes))
                    {
                        closestDistance = distance;
                        closestEnemy = collider.transform;
                    }
                }
            }
        }
        
        _currentTarget = closestEnemy;
    }
    
    private void UpdateAttacks()
    {
        _attackTimer -= Time.deltaTime;
        
        if (_currentTarget == null)
            return;
        
        float distanceToTarget = Vector3.Distance(transform.position, _currentTarget.position);
        
        if (distanceToTarget <= towerData.attackRange && _attackTimer <= 0)
        {
            FireAtTarget();
        }
    }
    
    private void FireAtTarget()
    {
        if (_currentTarget == null) return;
        
        // Calcular daño
        HeroStats targetStats = _currentTarget.GetComponent<HeroStats>();
        if (targetStats != null)
        {
            targetStats.TakeDamage(towerData.attackDamage, false);
            Debug.Log($"Tower attacked for {towerData.attackDamage} damage");
        }
        
        // TODO: Instanciar proyectil visual
        
        float attackDelay = 1f / towerData.attackSpeed;
        _attackTimer = attackDelay;
    }
    
    public void TakeDamage(float damage)
    {
        if (_isDestroyed) return;
        
        _currentHealth -= (int)damage;
        OnHealthChanged?.Invoke(_currentHealth);
        
        Debug.Log($"Tower took {damage} damage. Health: {_currentHealth}");
        
        if (_currentHealth <= 0)
        {
            Destroy();
        }
    }
    
    public void Destroy()
    {
        _isDestroyed = true;
        OnDestroyed?.Invoke();
        Debug.Log("Tower destroyed!");
        
        // TODO: Efecto de explosión
        // TODO: Dar oro al equipo que mató
        
        Destroy(gameObject, 1f);
    }
    
    public bool IsDestroyed() => _isDestroyed;
    public int GetHealth() => _currentHealth;
    public int GetMaxHealth() => towerData.health;
    public int GetTeam() => team;
    public float GetHealthPercent() => (float)_currentHealth / towerData.health;
}
