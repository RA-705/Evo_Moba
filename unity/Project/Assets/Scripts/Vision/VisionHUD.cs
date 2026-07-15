using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// HUD de visión mostrando información del fog of war
/// </summary>
public class VisionHUD : MonoBehaviour
{
    [SerializeField] private Text visionStatusText;
    [SerializeField] private MinimapRenderer minimapRenderer;
    
    private FogOfWarSystem _fogSystem;
    private int _playerTeam = 0;  // TODO: Get from game state
    
    private void Start()
    {
        _fogSystem = FogOfWarSystem.Instance;
    }
    
    private void Update()
    {
        UpdateVisionInfo();
    }
    
    private void UpdateVisionInfo()
    {
        if (visionStatusText != null)
        {
            // Mostrar estadísticas de visión
            visionStatusText.text = "FOW Active";
        }
    }
}
