using UnityEngine;
using LiteNetLib.Utils;

namespace Evo.Client
{
    public abstract class NetworkEntity : MonoBehaviour
    {
    public int EntityId { get; protected set; }
    public int HeroId { get; set; }
    public int TeamId { get; protected set; }
    public string EntityType { get; protected set; }

        public float CurrentHealth { get; protected set; }
        public float MaxHealth { get; protected set; }
        public float CurrentMana { get; protected set; }
        public float MaxMana { get; protected set; }
        public bool IsAlive { get; protected set; } = true;

        protected int _lastSnapshotTick;

        public virtual void Initialize(int entityId, int heroId, int teamId, string entityType)
        {
            EntityId = entityId;
            HeroId = heroId;
            TeamId = teamId;
            EntityType = entityType;
            IsAlive = true;
        }

        public virtual void UpdateFromSnapshot(Client.ServerPackets.EntitySnapshot snapshot)
        {
            _lastSnapshotTick = snapshot.EntityId; // Using EntityId as tick for now
            
            // Update health/mana
            CurrentHealth = snapshot.Health;
            MaxHealth = snapshot.MaxHealth;
            CurrentMana = snapshot.Mana;
            MaxMana = snapshot.MaxMana;
            IsAlive = snapshot.IsAlive;

            // Position will be interpolated by derived classes
        }

        public virtual void OnDespawn() { }

        public virtual void OnNetworkDestroy()
        {
            Destroy(gameObject);
        }

        protected virtual void Update()
        {
            // Base update logic
        }
    }
}