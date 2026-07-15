using UnityEngine;

/// <summary>
/// Detector de entidades basado en visibilidad
/// Esconde/muestra entidades enemigas según fog of war
/// </summary>
public class VisibilityChecker : MonoBehaviour
{
    [SerializeField] private int ownerTeam;
    
    private FogOfWarSystem _fogSystem;
    private bool _isVisible = true;
    
    private void Start()
    {
        _fogSystem = FogOfWarSystem.Instance;
    }
    
    private void Update()
    {
        // Verificar si esta entidad es visible para el equipo del jugador
        bool shouldBeVisible = _fogSystem.CanSeeEntity(transform.position, 0);  // TODO: Get player team
        
        if (shouldBeVisible != _isVisible)
        {
            _isVisible = shouldBeVisible;
            SetVisibility(_isVisible);
        }
    }
    
    private void SetVisibility(bool visible)
    {
        // Esconde/muestra mesh renderers
        foreach (var renderer in GetComponentsInChildren<Renderer>())
        {
            renderer.enabled = visible;
        }
        
        // Esconde/muestra UI encima de la entidad
        foreach (var text in GetComponentsInChildren<UnityEngine.UI.Text>())
        {
            text.enabled = visible;
        }
    }
}
