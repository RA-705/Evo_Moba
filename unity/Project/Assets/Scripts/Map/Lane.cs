using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Sistema de carriles (lanes) en el MOBA
/// Cada carril tiene torres y puntos de defensa
/// </summary>
public class Lane : MonoBehaviour
{
    public enum LaneType { Top, Mid, Bottom }
    
    [SerializeField] private LaneType laneType;
    [SerializeField] private Transform[] towerPositions;  // Posiciones donde spawnan torres
    [SerializeField] private Vector3 allySpawnPoint;      // Donde spawnan aliados
    [SerializeField] private Vector3 enemySpawnPoint;     // Donde spawnan enemigos
    [SerializeField] private float laneWidth = 50f;
    
    private Tower[] _towers;  // Torres en este carril
    private List<MinionAI> _activeLaneMinions = new();
    
    public event System.Action<Tower> OnTowerDestroyed;
    
    private void Start()
    {
        InitializeLane();
    }
    
    private void InitializeLane()
    {
        _towers = new Tower[towerPositions.Length];
        
        for (int i = 0; i < towerPositions.Length; i++)
        {
            // TODO: Instanciar towers en posiciones
            Debug.Log($"Tower {i} position initialized at {towerPositions[i].position}");
        }
    }
    
    public LaneType GetLaneType() => laneType;
    public Vector3 GetAllySpawnPoint() => allySpawnPoint;
    public Vector3 GetEnemySpawnPoint() => enemySpawnPoint;
    public Tower[] GetTowers() => _towers;
    public int GetTowerCount() => _towers.Length;
    public int GetDestroyedTowerCount()
    {
        int count = 0;
        foreach (var tower in _towers)
        {
            if (tower != null && tower.IsDestroyed())
                count++;
        }
        return count;
    }
}
