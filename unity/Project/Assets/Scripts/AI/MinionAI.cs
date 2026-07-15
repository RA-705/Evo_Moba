using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Comportamiento de minion (IA básica)
/// </summary>
public class MinionAI : MonoBehaviour
{
    [SerializeField] private MinionData minionData;
    
    private int _health;
    private Rigidbody _rb;
    private Transform _target;
    private float _attackTimer;
    private bool _isDead;
    
    public event System.Action<int> OnDeath;  // int = team
    
    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        if (_rb == null)
            _rb = gameObject.AddComponent<Rigidbody>();
        
        _health = minionData.health;
    }
    
    private void Update()
    {
        if (_isDead) return;
        
        UpdateTargeting();
        UpdateMovement();
        UpdateAttacks();
    }
    
    private void UpdateTargeting()
    {
        // Buscar enemigos cercanos
        Collider[] enemiesNearby = Physics.OverlapSphere(transform.position, minionData.visionRange);
        
        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;
        
        foreach (var collider in enemiesNearby)
        {
            // TODO: Check if enemy and not same team
            float distance = Vector3.Distance(transform.position, collider.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = collider.transform;
            }
        }
        
        _target = closestEnemy;
    }
    
    private void UpdateMovement()
    {
        if (_target == null) return;
        if (_isDead) return;
        
        float distanceToTarget = Vector3.Distance(transform.position, _target.position);
        
        // Si estoy en rango, no me muevo (para atacar)
        if (distanceToTarget <= minionData.attackRange)
        {
            _rb.velocity = Vector3.zero;
            return;
        }
        
        // Movimiento hacia el objetivo
        Vector3 direction = (_target.position - transform.position).normalized;
        Vector3 newVelocity = direction * minionData.movementSpeed;
        newVelocity.y = _rb.velocity.y;  // Mantener gravedad
        _rb.velocity = newVelocity;
    }
    
    private void UpdateAttacks()
    {
        if (_target == null) return;
        
        _attackTimer -= Time.deltaTime;
        
        float distanceToTarget = Vector3.Distance(transform.position, _target.position);
        if (distanceToTarget <= minionData.attackRange && _attackTimer <= 0)
        {
            AttackTarget();
        }
    }
    
    private void AttackTarget()
    {
        if (_target == null) return;
        
        var targetStats = _target.GetComponent<HeroStats>();
        if (targetStats != null)
        {
            targetStats.TakeDamage(minionData.attackDamage, false);
            Debug.Log($"Minion attacked for {minionData.attackDamage}");
        }
        
        float attackDelay = 1f / minionData.attackSpeed;
        _attackTimer = attackDelay;
    }
    
    public void TakeDamage(int damage)
    {
        _health -= damage;
        
        if (_health <= 0)
        {
            Die();
        }
    }
    
    public void Die()
    {
        _isDead = true;
        OnDeath?.Invoke(0);  // TODO: Get actual team
        Destroy(gameObject, 0.5f);
    }
    
    public int GetHealth() => _health;
    public int GetMaxHealth() => minionData.health;
}
