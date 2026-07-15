using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manager central del mapa
/// Gestiona lanes, torres, objetivos
/// </summary>
public class MapManager : MonoBehaviour
{
    public static MapManager Instance { get; private set; }
    
    [SerializeField] private Lane[] lanes;
    [SerializeField] private Vector3 blueBase;      // Posición base azul
    [SerializeField] private Vector3 redBase;       // Posición base roja
    [SerializeField] private Transform[] jungleBushes;  // Áreas de espesura (FOW)
    
    private Dictionary<Lane.LaneType, Lane> _lanesByType = new();
    
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
        InitializeMap();
    }
    
    private void InitializeMap()
    {
        foreach (var lane in lanes)
        {
            _lanesByType[lane.GetLaneType()] = lane;
        }
        
        Debug.Log($"Map initialized with {lanes.Length} lanes");
    }
    
    public Lane GetLane(Lane.LaneType laneType)
    {
        if (_lanesByType.ContainsKey(laneType))
            return _lanesByType[laneType];
        return null;
    }
    
    public Lane[] GetAllLanes() => lanes;
    public Vector3 GetBlueBase() => blueBase;
    public Vector3 GetRedBase() => redBase;
}
