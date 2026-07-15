using UnityEngine;

/// <summary>
/// Punto defensivo (edificios menores)
/// </summary>
public class DefensiveStructure : MonoBehaviour
{
    [SerializeField] private int team;
    [SerializeField] private int health = 500;
    [SerializeField] private float attackDamage = 50f;
    [SerializeField] private float attackRange = 8f;
    
    private int _currentHealth;
    private bool _isDestroyed;
    
    private void Start()
    {
        _currentHealth = health;
    }
    
    public void TakeDamage(float damage)
    {
        if (_isDestroyed) return;
        
        _currentHealth -= (int)damage;
        
        if (_currentHealth <= 0)
        {
            Destroy();
        }
    }
    
    public void Destroy()
    {
        _isDestroyed = true;
        Destroy(gameObject);
    }
    
    public bool IsDestroyed() => _isDestroyed;
    public int GetHealth() => _currentHealth;
}
