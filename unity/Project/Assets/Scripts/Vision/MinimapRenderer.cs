using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Minimap que muestra el mapa y posiciones de unidades
/// </summary>
public class MinimapRenderer : MonoBehaviour
{
    [SerializeField] private RawImage minimapDisplay;
    [SerializeField] private Canvas minimapCanvas;
    [SerializeField] private int minimapWidth = 256;
    [SerializeField] private int minimapHeight = 256;
    [SerializeField] private float mapWorldWidth = 200f;
    [SerializeField] private float mapWorldHeight = 200f;
    
    private Texture2D _minimapTexture;
    private Color[] _minimapPixels;
    
    [SerializeField] private Color fogColor = Color.black;
    [SerializeField] private Color revealedColor = new Color(0.1f, 0.1f, 0.1f);
    [SerializeField] private Color allyColor = Color.green;
    [SerializeField] private Color enemyColor = Color.red;
    [SerializeField] private Color neutralColor = Color.yellow;
    [SerializeField] private Color towerColor = Color.cyan;
    
    private FogOfWarSystem _fogSystem;
    private CameraController _mainCamera;
    
    private void Start()
    {
        _fogSystem = FogOfWarSystem.Instance;
        _mainCamera = Camera.main.GetComponent<CameraController>();
        
        InitializeMinimapTexture();
        minimapDisplay.texture = _minimapTexture;
    }
    
    private void InitializeMinimapTexture()
    {
        _minimapTexture = new Texture2D(minimapWidth, minimapHeight, TextureFormat.RGBA32, false);
        _minimapPixels = new Color[minimapWidth * minimapHeight];
        
        for (int i = 0; i < _minimapPixels.Length; i++)
        {
            _minimapPixels[i] = fogColor;
        }
        
        _minimapTexture.SetPixels(_minimapPixels);
        _minimapTexture.Apply();
    }
    
    private void Update()
    {
        UpdateMinimapTexture();
    }
    
    private void UpdateMinimapTexture()
    {
        // Limpiar minimap
        for (int i = 0; i < _minimapPixels.Length; i++)
        {
            _minimapPixels[i] = fogColor;
        }
        
        // Dibuja fog of war
        if (_fogSystem != null)
        {
            DrawFogOfWar();
        }
        
        // Dibuja unidades
        DrawEntities();
        
        // Dibuja vista principal
        DrawMainCameraViewport();
        
        _minimapTexture.SetPixels(_minimapPixels);
        _minimapTexture.Apply();
    }
    
    private void DrawFogOfWar()
    {
        Texture2D fogTexture = _fogSystem.GetFogTexture();
        if (fogTexture == null) return;
        
        // Escalar fog texture a minimap resolution
        for (int x = 0; x < minimapWidth; x++)
        {
            for (int y = 0; y < minimapHeight; y++)
            {
                int pixelIndex = y * minimapWidth + x;
                
                // Map minimap pixel to fog system pixel
                int fogX = Mathf.RoundToInt((float)x / minimapWidth * _fogSystem.GetGridWidth());
                int fogY = Mathf.RoundToInt((float)y / minimapHeight * _fogSystem.GetGridHeight());
                
                fogX = Mathf.Clamp(fogX, 0, _fogSystem.GetGridWidth() - 1);
                fogY = Mathf.Clamp(fogY, 0, _fogSystem.GetGridHeight() - 1);
                
                Color fogPixel = fogTexture.GetPixel(fogX, fogY);
                
                if (fogPixel.r > 0.5f)  // Visible
                {
                    _minimapPixels[pixelIndex] = revealedColor;
                }
            }
        }
    }
    
    private void DrawEntities()
    {
        // Dibuja héroes
        var heroViews = FindObjectsOfType<EntityView>();
        foreach (var hero in heroViews)
        {
            Vector2Int minimapPos = WorldToMinimapPosition(hero.transform.position);
            if (IsValidMinimapPosition(minimapPos))
            {
                int pixelIndex = minimapPos.y * minimapWidth + minimapPos.x;
                
                // TODO: Detectar equipo del jugador
                _minimapPixels[pixelIndex] = enemyColor;
            }
        }
        
        // Dibuja minions
        var minions = FindObjectsOfType<MinionAI>();
        foreach (var minion in minions)
        {
            Vector2Int minimapPos = WorldToMinimapPosition(minion.transform.position);
            if (IsValidMinimapPosition(minimapPos))
            {
                int pixelIndex = minimapPos.y * minimapWidth + minimapPos.x;
                _minimapPixels[pixelIndex] = enemyColor;
            }
        }
        
        // Dibuja torres (placeholder)
        // TODO: Add towers
    }
    
    private void DrawMainCameraViewport()
    {
        if (_mainCamera == null) return;
        
        // Dibuja cuadrado en minimap mostrando viewport principal
        Vector3 cameraPos = _mainCamera.transform.position;
        Vector2Int minimapPos = WorldToMinimapPosition(cameraPos);
        
        // Dibuja cuadrado pequeño
        int size = 3;
        for (int x = minimapPos.x - size; x <= minimapPos.x + size; x++)
        {
            for (int y = minimapPos.y - size; y <= minimapPos.y + size; y++)
            {
                if (IsValidMinimapPosition(new Vector2Int(x, y)))
                {
                    int pixelIndex = y * minimapWidth + x;
                    _minimapPixels[pixelIndex] = Color.white;
                }
            }
        }
    }
    
    private Vector2Int WorldToMinimapPosition(Vector3 worldPos)
    {
        float normalizedX = worldPos.x / mapWorldWidth;
        float normalizedY = worldPos.z / mapWorldHeight;
        
        int minimapX = Mathf.RoundToInt(normalizedX * minimapWidth);
        int minimapY = Mathf.RoundToInt(normalizedY * minimapHeight);
        
        return new Vector2Int(minimapX, minimapY);
    }
    
    private bool IsValidMinimapPosition(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < minimapWidth && pos.y >= 0 && pos.y < minimapHeight;
    }
    
    public void OnMinimapClick(BaseEventData data)
    {
        PointerEventData pointerData = data as PointerEventData;
        if (pointerData == null) return;
        
        // Convertir posición de click en minimap a posición del mundo
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            minimapDisplay.rectTransform,
            pointerData.position,
            null,
            out Vector2 localPoint);
        
        Vector2 normalizedPoint = new Vector2(
            (localPoint.x + minimapDisplay.rectTransform.rect.width / 2) / minimapDisplay.rectTransform.rect.width,
            (localPoint.y + minimapDisplay.rectTransform.rect.height / 2) / minimapDisplay.rectTransform.rect.height);
        
        Vector3 worldPos = new Vector3(
            normalizedPoint.x * mapWorldWidth,
            0,
            normalizedPoint.y * mapWorldHeight);
        
        Debug.Log($"Clicked minimap at world position: {worldPos}");
        // TODO: Mover hero a esa posición o enviar comando al servidor
    }
}
