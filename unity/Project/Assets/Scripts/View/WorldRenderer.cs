using System.Collections.Generic;
using UnityEngine;

public class WorldRenderer : MonoBehaviour
{
    [SerializeField] private GameObject heroPrefab;
    [SerializeField] private GameObject minionPrefab;
    [SerializeField] private GameObject towerPrefab;
    
    private Dictionary<int, EntityView> _entityViews = new();
    
    private void Start()
    {
        GameClient.Instance.OnSnapshotReceived += OnSnapshotReceived;
    }
    
    private void OnSnapshotReceived(WorldSnapshot snapshot)
    {
        UpdateEntities(snapshot);
    }
    
    private void UpdateEntities(WorldSnapshot snapshot)
    {
        // Remove entities that no longer exist
        var keysToRemove = new List<int>();
        foreach (var kvp in _entityViews)
        {
            if (!snapshot.Entities.Exists(e => e.EntityId == kvp.Key))
            {
                keysToRemove.Add(kvp.Key);
            }
        }
        
        foreach (var key in keysToRemove)
        {
            Destroy(_entityViews[key].gameObject);
            _entityViews.Remove(key);
        }
        
        // Update or create entities
        foreach (var entitySnapshot in snapshot.Entities)
        {
            if (_entityViews.TryGetValue(entitySnapshot.EntityId, out var view))
            {
                view.UpdateFromSnapshot(entitySnapshot);
            }
            else
            {
                CreateEntity(entitySnapshot);
            }
        }
    }
    
    private void CreateEntity(EntitySnapshot snapshot)
    {
        GameObject prefab = snapshot.EntityType switch
        {
            "Hero" => heroPrefab,
            "Minion" => minionPrefab,
            "Tower" => towerPrefab,
            _ => null
        };
        
        if (prefab == null) return;
        
        var instance = Instantiate(prefab, snapshot.Position, Quaternion.identity);
        var view = instance.AddComponent<EntityView>();
        view.Initialize(snapshot);
        
        _entityViews[snapshot.EntityId] = view;
    }
    
    private void OnDestroy()
    {
        if (GameClient.Instance != null)
            GameClient.Instance.OnSnapshotReceived -= OnSnapshotReceived;
    }
}
