using UnityEngine;
using UnityEngine.UI;

public class EntityView : MonoBehaviour
{
    [SerializeField] private Image healthBar;
    [SerializeField] private Text healthText;
    [SerializeField] private Text entityNameText;
    
    private int _entityId;
    private string _entityType;
    private EntitySnapshot _lastSnapshot;
    
    public void Initialize(EntitySnapshot snapshot)
    {
        _entityId = snapshot.EntityId;
        _entityType = snapshot.EntityType;
        _lastSnapshot = snapshot;
        
        gameObject.name = $"{_entityType}_{_entityId}";
        UpdateFromSnapshot(snapshot);
    }
    
    public void UpdateFromSnapshot(EntitySnapshot snapshot)
    {
        _lastSnapshot = snapshot;
        
        // Update position and rotation
        transform.position = snapshot.Position;
        transform.rotation = Quaternion.AngleAxis(snapshot.Rotation, Vector3.up);
        
        // Update health bar
        if (healthBar != null)
        {
            float healthPercent = (float)snapshot.Health / snapshot.MaxHealth;
            healthBar.fillAmount = healthPercent;
        }
        
        if (healthText != null)
        {
            healthText.text = $"{snapshot.Health}/{snapshot.MaxHealth}";
        }
    }
    
    private void OnDestroy()
    {
        Destroy(gameObject);
    }
}
