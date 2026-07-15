using UnityEngine;

/// <summary>
/// Ward (Centinela) - Proporciona visión del área
/// </summary>
public class Ward : MonoBehaviour
{
    [SerializeField] private float visionRange = 10f;
    [SerializeField] private int team;
    [SerializeField] private float lifetime = 180f;  // 3 minutos
    
    private float _timeRemaining;
    private FogOfWarSystem.VisionSource _visionSource;
    private FogOfWarSystem _fogSystem;
    
    private void Start()
    {
        _fogSystem = FogOfWarSystem.Instance;
        _timeRemaining = lifetime;
        
        // Registrar como fuente de visión
        _visionSource = new FogOfWarSystem.VisionSource
        {
            position = transform.position,
            radius = visionRange,
            team = team,
            type = FogOfWarSystem.VisionSourceType.Ward
        };
        
        _fogSystem.RegisterVisionSource(_visionSource);
    }
    
    private void Update()
    {
        _timeRemaining -= Time.deltaTime;
        
        if (_timeRemaining <= 0)
        {
            Die();
        }
        else
        {
            // Actualizar posición de visión
            _visionSource.position = transform.position;
        }
    }
    
    public void Die()
    {
        _fogSystem.UnregisterVisionSource(_visionSource);
        Destroy(gameObject);
    }
    
    public float GetTimeRemaining() => _timeRemaining;
}
