using UnityEngine;

/// <summary>
/// Control de visión de Torre
/// Las torres proporcionan visión del área
/// </summary>
public class TowerVision : MonoBehaviour
{
    [SerializeField] private float visionRange = 20f;
    [SerializeField] private int team;
    
    private FogOfWarSystem.VisionSource _visionSource;
    private FogOfWarSystem _fogSystem;
    
    private void Start()
    {
        _fogSystem = FogOfWarSystem.Instance;
        
        _visionSource = new FogOfWarSystem.VisionSource
        {
            position = transform.position,
            radius = visionRange,
            team = team,
            type = FogOfWarSystem.VisionSourceType.Tower
        };
        
        _fogSystem.RegisterVisionSource(_visionSource);
    }
    
    private void Update()
    {
        // Torres no se mueven, pero actualizar por si acaso
        _visionSource.position = transform.position;
    }
    
    private void OnDestroy()
    {
        if (_fogSystem != null)
            _fogSystem.UnregisterVisionSource(_visionSource);
    }
}
