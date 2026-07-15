using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Sistema de Fog of War (niebla de guerra)
/// Cada hero/equipo tiene su propia visión
/// </summary>
public class FogOfWarSystem : MonoBehaviour
{
    [SerializeField] private int mapWidth = 200;
    [SerializeField] private int mapHeight = 200;
    [SerializeField] private float cellSize = 1f;
    [SerializeField] private float heroVisionRange = 15f;
    [SerializeField] private float towerVisionRange = 20f;
    [SerializeField] private float wardVisionRange = 10f;
    
    // Texture2D para visualizar fog of war
    private Texture2D _fogTexture;
    private Color[] _fogPixels;
    private int _gridWidth;
    private int _gridHeight;
    
    // Rastreo de visibilidad por equipo
    private Dictionary<int, HashSet<Vector2Int>> _teamVisionMap = new();
    private Dictionary<int, HashSet<Vector2Int>> _lastVisionMap = new();
    
    private List<VisionSource> _visionSources = new();
    
    public static FogOfWarSystem Instance { get; private set; }
    
    [System.Serializable]
    public class VisionSource
    {
        public Vector3 position;
        public float radius;
        public int team;
        public VisionSourceType type;  // Hero, Tower, Ward
    }
    
    public enum VisionSourceType { Hero, Tower, Ward, Sentry }
    
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
        InitializeFogTexture();
    }
    
    private void InitializeFogTexture()
    {
        _gridWidth = Mathf.CeilToInt(mapWidth / cellSize);
        _gridHeight = Mathf.CeilToInt(mapHeight / cellSize);
        
        _fogTexture = new Texture2D(_gridWidth, _gridHeight, TextureFormat.RGBA32, false);
        _fogPixels = new Color[_gridWidth * _gridHeight];
        
        for (int i = 0; i < _fogPixels.Length; i++)
        {
            _fogPixels[i] = Color.black;  // Inicialmente todo oscuro
        }
        
        _fogTexture.SetPixels(_fogPixels);
        _fogTexture.Apply();
    }
    
    private void Update()
    {
        UpdateFogOfWar();
    }
    
    private void UpdateFogOfWar()
    {
        // Clear previous vision
        foreach (var team in _teamVisionMap.Keys)
        {
            _teamVisionMap[team].Clear();
        }
        
        // Update vision from all sources
        foreach (var source in _visionSources)
        {
            if (!_teamVisionMap.ContainsKey(source.team))
                _teamVisionMap[source.team] = new HashSet<Vector2Int>();
            
            AddVisionArea(source.position, source.radius, source.team);
        }
        
        // Update fog texture
        for (int x = 0; x < _gridWidth; x++)
        {
            for (int y = 0; y < _gridHeight; y++)
            {
                int pixelIndex = y * _gridWidth + x;
                
                // Por defecto, oscuro (no visible)
                Color pixelColor = Color.black;
                
                // Verificar cada equipo
                foreach (var team in _teamVisionMap.Keys)
                {
                    if (_teamVisionMap[team].Contains(new Vector2Int(x, y)))
                    {
                        pixelColor = Color.white;  // Visible
                        break;
                    }
                }
                
                _fogPixels[pixelIndex] = pixelColor;
            }
        }
        
        _fogTexture.SetPixels(_fogPixels);
        _fogTexture.Apply();
    }
    
    private void AddVisionArea(Vector3 position, float radius, int team)
    {
        Vector2Int gridPos = WorldToGridPosition(position);
        int gridRadius = Mathf.CeilToInt(radius / cellSize);
        
        for (int x = gridPos.x - gridRadius; x <= gridPos.x + gridRadius; x++)
        {
            for (int y = gridPos.y - gridRadius; y <= gridPos.y + gridRadius; y++)
            {
                if (x >= 0 && x < _gridWidth && y >= 0 && y < _gridHeight)
                {
                    Vector3 cellWorldPos = GridToWorldPosition(new Vector2Int(x, y));
                    float distance = Vector3.Distance(cellWorldPos, position);
                    
                    if (distance <= radius)
                    {
                        _teamVisionMap[team].Add(new Vector2Int(x, y));
                    }
                }
            }
        }
    }
    
    public void RegisterVisionSource(VisionSource source)
    {
        _visionSources.Add(source);
    }
    
    public void UnregisterVisionSource(VisionSource source)
    {
        _visionSources.Remove(source);
    }
    
    public bool IsPositionVisible(Vector3 position, int team)
    {
        if (!_teamVisionMap.ContainsKey(team))
            return false;
        
        Vector2Int gridPos = WorldToGridPosition(position);
        return _teamVisionMap[team].Contains(gridPos);
    }
    
    public bool CanSeeEntity(Vector3 entityPosition, int observerTeam)
    {
        return IsPositionVisible(entityPosition, observerTeam);
    }
    
    private Vector2Int WorldToGridPosition(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt(worldPos.x / cellSize);
        int y = Mathf.FloorToInt(worldPos.z / cellSize);
        return new Vector2Int(x, y);
    }
    
    private Vector3 GridToWorldPosition(Vector2Int gridPos)
    {
        float x = gridPos.x * cellSize;
        float z = gridPos.y * cellSize;
        return new Vector3(x, 0, z);
    }
    
    public Texture2D GetFogTexture() => _fogTexture;
    public int GetGridWidth() => _gridWidth;
    public int GetGridHeight() => _gridHeight;
}
