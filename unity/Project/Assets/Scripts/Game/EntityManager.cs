using System.Collections.Generic;
using UnityEngine;
using Evo.Client;

namespace Evo.UnityClient
{
    public class EntityManager : MonoBehaviour
    {
        public static EntityManager Instance { get; private set; }

        private readonly Dictionary<int, NetworkEntity> _entities = new Dictionary<int, NetworkEntity>();
        private readonly Dictionary<int, GameObject> _entityObjects = new Dictionary<int, GameObject>();

        [Header("Entity Prefabs")]
        public GameObject HeroPrefab;
        public GameObject CreepPrefab;
        public GameObject TowerPrefab;
        public GameObject MonsterPrefab;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void SpawnEntity(int entityId, int heroId, int teamId, Vector3 position, string entityType = "Hero")
        {
            if (_entities.ContainsKey(entityId)) return;

            GameObject prefab = GetPrefabForType(entityType);
            if (prefab == null) return;

            var go = Instantiate(prefab, position, Quaternion.identity);
            go.name = $"Entity_{entityId}_Hero{heroId}";
            
            var netEntity = go.GetComponent<NetworkEntity>();
            if (netEntity == null)
                netEntity = go.AddComponent<NetworkEntity>();

            netEntity.Initialize(entityId, heroId, teamId, entityType);
            
            _entities[entityId] = netEntity;
            _entityObjects[entityId] = go;
        }

        public void UpdateEntitySnapshot(Evo.Client.ServerPackets.EntitySnapshot snapshot)
        {
            if (_entities.TryGetValue(snapshot.EntityId, out var entity))
            {
                entity.UpdateFromSnapshot(snapshot);
            }
        }

        public void DespawnEntity(int entityId)
        {
            if (_entities.TryGetValue(entityId, out var entity))
            {
                entity.OnDespawn();
                _entities.Remove(entityId);
            }

            if (_entityObjects.TryGetValue(entityId, out var go))
            {
                Destroy(go);
                _entityObjects.Remove(entityId);
            }
        }

        public NetworkEntity GetEntity(int entityId)
        {
            _entities.TryGetValue(entityId, out var entity);
            return entity;
        }

        public GameObject GetEntityObject(int entityId)
        {
            _entityObjects.TryGetValue(entityId, out var go);
            return go;
        }

        private GameObject GetPrefabForType(string entityType)
        {
            return entityType switch
            {
                "Hero" => HeroPrefab,
                "Creep" => CreepPrefab,
                "Tower" => TowerPrefab,
                "Monster" => MonsterPrefab,
                _ => HeroPrefab
            };
        }
    }
}