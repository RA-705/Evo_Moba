using UnityEngine;

/// <summary>
/// Controla la visión de un héroe
/// </summary>
public class HeroVision : MonoBehaviour
{
    [SerializeField] private float baseVisionRange = 15f;
    [SerializeField] private int team;
    
    private FogOfWarSystem.VisionSource _visionSource;
    private FogOfWarSystem _fogSystem;
    private HeroStats _heroStats;
    
    private void Start()
    {
        _fogSystem = FogOfWarSystem.Instance;
        _heroStats = GetComponent<HeroStats>();
        
        _visionSource = new FogOfWarSystem.VisionSource
        {
            position = transform.position,
            radius = baseVisionRange,
            team = team,
            type = FogOfWarSystem.VisionSourceType.Hero
        };
        
        _fogSystem.RegisterVisionSource(_visionSource);
    }
    
    private void Update()
    {
        // Actualizar posición de visión
        _visionSource.position = transform.position;
        
        // TODO: Modificar rango de visión por items o hechizos
        // _visionSource.radius = baseVisionRange + itemBonus;
    }
    
    public void SetTeam(int newTeam)
    {
        team = newTeam;
        _visionSource.team = newTeam;
    }
    
    private void OnDestroy()
    {
        if (_fogSystem != null)
            _fogSystem.UnregisterVisionSource(_visionSource);
    }
}
