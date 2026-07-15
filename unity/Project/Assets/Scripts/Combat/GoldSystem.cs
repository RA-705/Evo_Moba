using UnityEngine;

/// <summary>
/// Maneja el sistema de oro y recompensas
/// </summary>
public class GoldSystem : MonoBehaviour
{
    private HeroStats _heroStats;
    private int _currentGold;
    
    // Oro por kill/assist/minion
    public const int KILL_REWARD = 300;
    public const int ASSIST_REWARD = 150;
    public const int MINION_KILL = 50;
    public const int TOWER_KILL = 200;
    
    public event System.Action<int> OnGoldChanged;
    
    private void Start()
    {
        _heroStats = GetComponent<HeroStats>();
        _currentGold = 500;  // Starting gold
        OnGoldChanged?.Invoke(_currentGold);
    }
    
    public void AddGold(int amount)
    {
        _currentGold += amount;
        _heroStats.AddGold(amount);
        OnGoldChanged?.Invoke(_currentGold);
    }
    
    public bool TrySpendGold(int amount)
    {
        if (_currentGold >= amount)
        {
            _currentGold -= amount;
            OnGoldChanged?.Invoke(_currentGold);
            return true;
        }
        return false;
    }
    
    public void OnEnemyKilled()
    {
        AddGold(KILL_REWARD);
    }
    
    public void OnAssist()
    {
        AddGold(ASSIST_REWARD);
    }
    
    public void OnMinionKilled()
    {
        AddGold(MINION_KILL);
    }
    
    public void OnTowerKilled()
    {
        AddGold(TOWER_KILL);
    }
    
    public int GetGold() => _currentGold;
}
