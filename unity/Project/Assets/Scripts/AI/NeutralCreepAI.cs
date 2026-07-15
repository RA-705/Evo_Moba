using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Comportamiento de creeps neutros (Buffs, Raptors, etc)
/// </summary>
public class NeutralCreepAI : MonoBehaviour
{
    [System.Serializable]
    public class CreepType
    {
        public string creepName;
        public int health = 100;
        public float attackDamage = 8f;
        public float movementSpeed = 2f;
        public int goldReward = 100;
        public int experienceReward = 150;
        public string buffName;  // Buff que da al matar
    }
    
    [SerializeField] private CreepType creepData;
    [SerializeField] private Transform[] patrolPoints;  // Puntos de patrulla
    
    private int _health;
    private bool _isDead;
    private bool _isPatrolling = true;
    private int _currentPatrolIndex;
    private Rigidbody _rb;
    private Transform _target;
    
    public event System.Action<int> OnDeath;  // int = team that killed
    
    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        if (_rb == null)
            _rb = gameObject.AddComponent<Rigidbody>();
        
        _health = creepData.health;
        gameObject.name = creepData.creepName;
    }
    
    private void Update()
    {
        if (_isDead) return;
        
        if (_isPatrolling)
        {
            Patrol();
        }
        else if (_target != null)
        {
            ChaseTarget();
        }
    }
    
    private void Patrol()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
            return;
        
        Transform targetPoint = patrolPoints[_currentPatrolIndex];
        float distanceToPoint = Vector3.Distance(transform.position, targetPoint.position);
        
        if (distanceToPoint < 1f)
        {
            _currentPatrolIndex = (_currentPatrolIndex + 1) % patrolPoints.Length;
        }
        else
        {
            Vector3 direction = (targetPoint.position - transform.position).normalized;
            Vector3 newVelocity = direction * creepData.movementSpeed;
            newVelocity.y = _rb.velocity.y;
            _rb.velocity = newVelocity;
        }
    }
    
    private void ChaseTarget()
    {
        float distanceToTarget = Vector3.Distance(transform.position, _target.position);
        
        Vector3 direction = (_target.position - transform.position).normalized;
        Vector3 newVelocity = direction * creepData.movementSpeed;
        newVelocity.y = _rb.velocity.y;
        _rb.velocity = newVelocity;
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
        OnDeath?.Invoke(0);  // TODO: Get killer team
        Debug.Log($"{creepData.creepName} defeated!");
        Destroy(gameObject, 0.5f);
    }
    
    public int GetGoldReward() => creepData.goldReward;
    public int GetExperienceReward() => creepData.experienceReward;
    public string GetBuffName() => creepData.buffName;
}
