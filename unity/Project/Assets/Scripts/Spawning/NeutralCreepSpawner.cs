using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Controlador de spawning de creeps neutros
/// </summary>
public class NeutralCreepSpawner : MonoBehaviour
{
    [System.Serializable]
    public class CreepSpawn
    {
        public string creepName;
        public Transform spawnLocation;
        public float respawnTime = 120f;  // 2 minutos
        public GameObject creepPrefab;
    }
    
    [SerializeField] private CreepSpawn[] creepSpawns;
    private Dictionary<CreepSpawn, float> _respawnTimers = new();
    private Dictionary<CreepSpawn, GameObject> _activeCreeps = new();
    
    private void Start()
    {
        foreach (var spawn in creepSpawns)
        {
            _respawnTimers[spawn] = 0;  // Spawn inmediato
            SpawnCreep(spawn);
        }
    }
    
    private void Update()
    {
        foreach (var spawn in creepSpawns)
        {
            if (!_activeCreeps.ContainsKey(spawn) || _activeCreeps[spawn] == null)
            {
                _respawnTimers[spawn] -= Time.deltaTime;
                
                if (_respawnTimers[spawn] <= 0)
                {
                    SpawnCreep(spawn);
                    _respawnTimers[spawn] = spawn.respawnTime;
                }
            }
        }
    }
    
    private void SpawnCreep(CreepSpawn spawn)
    {
        var creep = Instantiate(spawn.creepPrefab, spawn.spawnLocation.position, Quaternion.identity);
        _activeCreeps[spawn] = creep;
        Debug.Log($"Spawned {spawn.creepName}");
    }
    
    public float GetNextRespawnTime(CreepSpawn spawn)
    {
        return Mathf.Max(0, _respawnTimers[spawn]);
    }
}
