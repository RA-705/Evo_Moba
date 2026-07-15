using System;
using System.Collections.Generic;
using System.IO;
using Evo.Core.ECS;
using Evo.MOBA.Abilities;
using Evo.MOBA.Combat;
using Evo.MOBA.Items;
using Evo.MOBA.Navigation;
using Evo.MOBA.Systems;
using Evo.Shared.Math;

namespace Evo.MOBA.Networking;

public enum ClientInputType : byte
{
    MoveTo = 1,
    AttackTarget = 2,
    CastAbility = 3,
    BuyItem = 4,
    PlaceWard = 5,
    GachaPull = 6,
    ClaimBattlePass = 7,
}

public sealed class InputProcessSystem : ISystem
{
    private readonly GameNetServer _server;
    private readonly NavGrid _navGrid;
    private readonly Queue<byte[]> _inputQueue = new();

    public InputProcessSystem(GameNetServer server, NavGrid navGrid)
    {
        _server = server;
        _navGrid = navGrid;
        _server.OnInputReceived += (_, data) => _inputQueue.Enqueue(data);
    }

    public void OnTick(World world, float deltaTime)
    {
        while (_inputQueue.Count > 0)
        {
            var data = _inputQueue.Dequeue();
            ProcessInput(world, data);
        }
    }

    private void ProcessInput(World world, byte[] data)
    {
        if (data.Length < 5) return;

        using var ms = new MemoryStream(data);
        using var reader = new BinaryReader(ms);

        var inputType = (ClientInputType)reader.ReadByte();
        int rawEntityId = reader.ReadInt32();

        EntityId? found = null;
        foreach (var id in world.GetEntityIds<PositionComponent>())
        {
            if (id.Value == rawEntityId)
            {
                found = id;
                break;
            }
        }

        if (found == null) return;
        var entityId = found;

        switch (inputType)
        {
            case ClientInputType.MoveTo:
                ProcessMoveTo(world, entityId, reader);
                break;
            case ClientInputType.AttackTarget:
                ProcessAttackTarget(world, entityId, reader);
                break;
            case ClientInputType.CastAbility:
                ProcessCastAbility(world, entityId, reader);
                break;
            case ClientInputType.BuyItem:
                ProcessBuyItem(world, entityId, reader);
                break;
            case ClientInputType.GachaPull:
                ProcessGachaPull(world, entityId, reader);
                break;
            case ClientInputType.ClaimBattlePass:
                ProcessClaimBattlePass(world, entityId, reader);
                break;
        }
    }

    private static EntityId? FindEntityId(World world, int rawId)
    {
        foreach (var id in world.GetEntityIds<PositionComponent>())
        {
            if (id.Value == rawId)
                return id;
        }
        return null;
    }

    private void ProcessMoveTo(World world, EntityId entityId, BinaryReader reader)
    {
        float x = reader.ReadSingle();
        float z = reader.ReadSingle();
        var targetPos = new EvoVector3(x, 0, z);

        var currentPos = world.GetComponent<PositionComponent>(entityId).Value;
        var path = AStarPathfinder.FindPath(_navGrid, currentPos, targetPos);

        world.AddComponent(new Entity(entityId), new PathfollowComponent
        {
            Waypoints = path,
            CurrentWaypointIndex = 0,
            StoppingDistance = 0.3f,
        });

        if (world.TryGetComponent<AttackComponent>(entityId, out var attack))
        {
            attack.CurrentTargetId = Entity.Null.Id;
            world.SetComponent(entityId, attack);
        }
    }

    private void ProcessAttackTarget(World world, EntityId entityId, BinaryReader reader)
    {
        int targetRawId = reader.ReadInt32();
        var targetId = FindEntityId(world, targetRawId);
        if (targetId == null) return;

        if (!world.TryGetComponent<HealthComponent>(targetId, out _))
            return;

        if (world.TryGetComponent<AttackComponent>(entityId, out var attack))
        {
            attack.CurrentTargetId = targetId;
            world.SetComponent(entityId, attack);
        }

        world.RemoveComponent<PathfollowComponent>(entityId);
    }

    private void ProcessCastAbility(World world, EntityId entityId, BinaryReader reader)
    {
        int abilityIndex = reader.ReadInt32();
        float targetX = reader.ReadSingle();
        float targetZ = reader.ReadSingle();
        int targetUnitRawId = reader.ReadInt32();

        if (!world.TryGetComponent<AbilitySlotComponent>(entityId, out var slots))
            return;

        AbilitySlot slot = abilityIndex switch
        {
            0 => slots.Q,
            1 => slots.W,
            2 => slots.E,
            3 => slots.R,
            _ => default,
        };

        if (slot.DataId <= 0) return;

        EntityId targetUnitId = Entity.Null.Id;
        bool isUnit = false;
        if (targetUnitRawId >= 0)
        {
            var found = FindEntityId(world, targetUnitRawId);
            if (found != null)
            {
                targetUnitId = found;
                isUnit = true;
            }
        }

        var target = new CastTarget
        {
            TargetPoint = new EvoVector3(targetX, 0, targetZ),
            IsUnit = isUnit,
            TargetUnitId = targetUnitId,
        };

        world.AddComponent(new Entity(entityId), new PendingCastComponent
        {
            AbilityDataId = slot.DataId,
            Target = target,
        });
    }

    private static void ProcessBuyItem(World world, EntityId entityId, BinaryReader reader)
    {
        int itemId = reader.ReadInt32();

        if (!ItemRegistry.Database.ContainsKey(itemId))
            return;

        if (!world.TryGetComponent<InventoryComponent>(entityId, out var inv))
            return;

        if (inv.Gold < ItemRegistry.Database[itemId].Cost)
            return;

        if (!inv.TryAddItem(itemId))
            return;

        inv.Gold -= ItemRegistry.Database[itemId].Cost;
        world.SetComponent(entityId, inv);

        if (world.TryGetComponent<HealthComponent>(entityId, out var health))
        {
            health.Max += ItemRegistry.Database[itemId].BonusHp;
            health.Current += ItemRegistry.Database[itemId].BonusHp;
            world.SetComponent(entityId, health);
        }
    }

    private static void ProcessGachaPull(World world, EntityId entityId, BinaryReader reader)
    {
        byte poolByte = reader.ReadByte();
        var pool = (GachaPool)poolByte;

        var costCurrency = pool switch
        {
            GachaPool.Standard => CurrencyType.BattlePoints,
            GachaPool.Premium => CurrencyType.Diamonds,
            GachaPool.Event => CurrencyType.Tickets,
            _ => CurrencyType.Diamonds,
        };

        int cost = pool switch
        {
            GachaPool.Standard => 300,
            GachaPool.Premium => 100,
            GachaPool.Event => 50,
            _ => 100,
        };

        var result = GachaService.Pull(world, entityId, pool, costCurrency, cost);

        if (result.EntryId > 0)
        {
            if (world.TryGetComponent<InventoryComponent>(entityId, out var inv))
            {
                inv.TryAddItem(result.EntryId);
                world.SetComponent(entityId, inv);
            }
        }
    }

    private static void ProcessClaimBattlePass(World world, EntityId entityId, BinaryReader reader)
    {
        int level = reader.ReadInt32();
        BattlePassService.ClaimReward(world, entityId, level);
    }
}
