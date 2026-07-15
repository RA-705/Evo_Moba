using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Sistema de combate melee/rango basado en ataques básicos
/// </summary>
public class AutoAttackSystem : MonoBehaviour
{
    [SerializeField] private float attackRange = 5.5f;
    [SerializeField] private float attackAnimationDuration = 0.5f;
    
    private HeroStats _heroStats;
    private Transform _target;
    private float _attackTimer;
    private bool _isAttacking;
    
    public event System.Action<Transform> OnAttackHit;
    
    private void Start()
    {
        _heroStats = GetComponent<HeroStats>();
    }
    
    private void Update()
    {
        if (!_heroStats.CanMove()) return;
        
        _attackTimer -= Time.deltaTime;
        
        if (_target != null && Vector3.Distance(transform.position, _target.position) <= attackRange)
        {
            if (_attackTimer <= 0 && !_heroStats.IsDead)
            {
                PerformAttack(_target);
            }
        }
    }
    
    public void SetTarget(Transform target)
    {
        _target = target;
    }
    
    private void PerformAttack(Transform target)
    {
        // Calcular daño
        HeroStats targetStats = target.GetComponent<HeroStats>();
        if (targetStats == null) return;
        
        float damage = DamageCalculator.CalculateAutoAttackDamage(
            GetComponent<HeroStats>().GetComponent<HeroData>(),  // Simplificado
            _heroStats.GetLevel(),
            (int)_heroStats.GetAttackDamage(),
            targetStats.GetComponent<HeroData>(),
            targetStats.GetArmor()
        );
        
        targetStats.TakeDamage(damage, false);  // Physical damage
        
        OnAttackHit?.Invoke(target);
        
        // Reset attack timer basado en attack speed
        float attackDelay = 1f / _heroStats.GetAttackSpeed();
        _attackTimer = attackDelay;
        
        Debug.Log($"Attack hit for {damage} damage!");
    }
    
    public float GetAttackRange() => attackRange;
}
