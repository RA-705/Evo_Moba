using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manager de torres en el juego
/// Rastrea todas las torres, su estado y destrucciones
/// </summary>
public class TowerManager : MonoBehaviour
{
    public static TowerManager Instance { get; private set; }
    
    private List<Tower> _allTowers = new();
    private Dictionary<int, List<Tower>> _towersByTeam = new();  // team -> list of towers
    
    public event System.Action<Tower, int> OnTowerDestroyed;  // tower, destroyer team
    public event System.Action<Tower> OnTowerDamaged;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    
    public void RegisterTower(Tower tower)
    {
        _allTowers.Add(tower);
        
        int team = tower.GetTeam();
        if (!_towersByTeam.ContainsKey(team))
            _towersByTeam[team] = new List<Tower>();
        
        _towersByTeam[team].Add(tower);
        
        tower.OnDestroyed += () => OnTowerDestroyedEvent(tower);
        tower.OnHealthChanged += health => OnTowerDamaged?.Invoke(tower);
    }
    
    private void OnTowerDestroyedEvent(Tower tower)
    {
        // TODO: Determine destroyer team from last damage dealer
        OnTowerDestroyed?.Invoke(tower, 0);
    }
    
    public List<Tower> GetTowersByTeam(int team)
    {
        if (_towersByTeam.ContainsKey(team))
            return _towersByTeam[team];
        return new List<Tower>();
    }
    
    public int GetDestroyedTowerCount(int team)
    {
        int count = 0;
        foreach (var tower in GetTowersByTeam(team))
        {
            if (tower.IsDestroyed())
                count++;
        }
        return count;
    }
    
    public int GetActiveTowerCount(int team)
    {
        int count = 0;
        foreach (var tower in GetTowersByTeam(team))
        {
            if (!tower.IsDestroyed())
                count++;
        }
        return count;
    }
    
    public bool IsTeamBaseDestroyed(int team)
    {
        // Base destruida cuando todas las torres caen
        return GetActiveTowerCount(team) == 0;
    }
}
