using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Spawner automático de minions en oleadas
/// </summary>
public class CreepWaveSpawner : MonoBehaviour
{
    [SerializeField] private MinionData minionData;
    [SerializeField] private MinionData siegeMinionData;
    
    [Header("Spawning")]
    [SerializeField] private Transform[] spawnPoints;  // Lane spawners
    [SerializeField] private float waveSpawnInterval = 30f;  // Cada 30 segundos
    [SerializeField] private int minionsPerWave = 3;
    [SerializeField] private float spawnDelay = 0.5f;  // Delay entre minions
    
    [Header("Siege Minions")]
    [SerializeField] private float siegeMinionInterval = 120f;  // Cada 2 minutos
    [SerializeField] private int siegeMinionCount = 1;
    
    [Header("Prefabs")]
    [SerializeField] private GameObject minionPrefab;
    
    private float _waveTimer;
    private float _siegeTimer;
    private int _waveCount;
    private List<GameObject> _activeMinions = new();
    
    public event System.Action<int> OnWaveSpawned;  // int = wave number
    public event System.Action OnSiegeMinionSpawned;
    
    private void Start()
    {
        _waveTimer = waveSpawnInterval;
        _siegeTimer = siegeMinionInterval;
    }
    
    private void Update()
    {
        UpdateWaveSpawning();
        UpdateSiegeMinionSpawning();
        CleanupDeadMinions();
    }
    
    private void UpdateWaveSpawning()
    {
        _waveTimer -= Time.deltaTime;
        
        if (_waveTimer <= 0)
        {
            SpawnWave();
            _waveTimer = waveSpawnInterval;
        }
    }
    
    private void UpdateSiegeMinionSpawning()
    {
        _siegeTimer -= Time.deltaTime;
        
        if (_siegeTimer <= 0)
        {
            SpawnSiegeMinions();
            _siegeTimer = siegeMinionInterval;
        }
    }
    
    private void SpawnWave()
    {
        _waveCount++;
        Debug.Log($"[CreepWaveSpawner] Wave {_waveCount} spawned!");
        
        // Cada spawnpoint spawna un grupo de minions
        foreach (var spawnPoint in spawnPoints)
        {
            StartCoroutine(SpawnMinionGroup(spawnPoint, minionsPerWave));
        }
        
        OnWaveSpawned?.Invoke(_waveCount);
    }
    
    private System.Collections.IEnumerator SpawnMinionGroup(Transform spawnPoint, int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject minion = Instantiate(minionPrefab, 
                spawnPoint.position + Vector3.right * i * 1.5f, 
                Quaternion.identity);
            
            _activeMinions.Add(minion);
            
            yield return new WaitForSeconds(spawnDelay);
        }
    }
    
    private void SpawnSiegeMinions()
    {
        Debug.Log("[CreepWaveSpawner] Siege minions spawned!");
        
        foreach (var spawnPoint in spawnPoints)
        {
            for (int i = 0; i < siegeMinionCount; i++)
            {
                GameObject siegeMinion = Instantiate(minionPrefab,
                    spawnPoint.position + Vector3.right * i * 2f,
                    Quaternion.identity);
                
                _activeMinions.Add(siegeMinion);
            }
        }
        
        OnSiegeMinionSpawned?.Invoke();
    }
    
    private void CleanupDeadMinions()
    {
        _activeMinions.RemoveAll(m => m == null);
    }
    
    public int GetActiveMinions() => _activeMinions.Count;
    public int GetWaveCount() => _waveCount;
    public float GetNextWaveIn() => Mathf.Max(0, _waveTimer);
}
