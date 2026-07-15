using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manager de todos los minions en el mapa
/// Rastrea kills, oro, experiencia
/// </summary>
public class MinionManager : MonoBehaviour
{
    public static MinionManager Instance { get; private set; }
    
    private List<MinionAI> _allMinions = new();
    private CreepWaveSpawner _waveSpawner;
    
    public event System.Action<MinionAI> OnMinionSpawned;
    public event System.Action<MinionAI, int> OnMinionKilled;  // minion, killer team
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    
    private void Start()
    {
        _waveSpawner = GetComponent<CreepWaveSpawner>();
        if (_waveSpawner != null)
        {
            _waveSpawner.OnWaveSpawned += OnWaveSpawned;
        }
    }
    
    public void RegisterMinion(MinionAI minion)
    {
        _allMinions.Add(minion);
        minion.OnDeath += team => OnMinionKilled?.Invoke(minion, team);
        OnMinionSpawned?.Invoke(minion);
    }
    
    public void UnregisterMinion(MinionAI minion)
    {
        _allMinions.Remove(minion);
    }
    
    private void OnWaveSpawned(int waveNumber)
    {
        Debug.Log($"Wave {waveNumber} spawned with {_allMinions.Count} total minions");
    }
    
    public List<MinionAI> GetMinionsByTeam(int team)
    {
        var minionsByTeam = new List<MinionAI>();
        foreach (var minion in _allMinions)
        {
            // TODO: Check minion team
            // if (minion.GetTeam() == team)
            //     minionsByTeam.Add(minion);
        }
        return minionsByTeam;
    }
    
    public int GetMinionCount() => _allMinions.Count;
}
