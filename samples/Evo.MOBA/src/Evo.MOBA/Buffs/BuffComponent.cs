using System.Collections.Generic;
using Evo.Core.ECS;
using Evo.MOBA.Combat;

namespace Evo.MOBA.Buffs;

public enum BuffEffectType
{
    Stun,
    Slow,
    DamageOverTime,
    Haste,
    ArmorUp,
    MagicResistUp,
}

public struct BuffInstance
{
    public BuffEffectType Effect;
    public float RemainingDuration;
    public float TickInterval;
    public float TickTimer;
    public float Value;
    public EntityId SourceId;
}

public struct BuffComponent : IComponent
{
    public List<BuffInstance> Buffs;

    public BuffComponent()
    {
        Buffs = new List<BuffInstance>(4);
    }

    public void Add(BuffInstance buff) => Buffs.Add(buff);

    public bool HasEffect(BuffEffectType effect)
    {
        for (int i = 0; i < Buffs.Count; i++)
        {
            if (Buffs[i].Effect == effect)
                return true;
        }
        return false;
    }

    public float GetEffectValue(BuffEffectType effect)
    {
        float sum = 0;
        for (int i = 0; i < Buffs.Count; i++)
        {
            if (Buffs[i].Effect == effect)
                sum += Buffs[i].Value;
        }
        return sum;
    }
}

public sealed class BuffSystem : ISystem
{
    public void OnTick(World world, float deltaTime)
    {
        foreach (var id in world.GetEntityIds<BuffComponent>())
        {
            ref var buffs = ref world.GetComponent<BuffComponent>(id);

            for (int i = buffs.Buffs.Count - 1; i >= 0; i--)
            {
                var buff = buffs.Buffs[i];
                buff.RemainingDuration -= deltaTime;

                if (buff.Effect == BuffEffectType.DamageOverTime)
                {
                    buff.TickTimer -= deltaTime;
                    if (buff.TickTimer <= 0f)
                    {
                        buff.TickTimer = buff.TickInterval;

                        if (world.TryGetComponent<HealthComponent>(id, out var health))
                        {
                            health.Current -= buff.Value;
                            world.SetComponent(id, health);
                        }
                    }
                }

                if (buff.RemainingDuration <= 0f)
                {
                    buffs.Buffs.RemoveAt(i);
                }
                else
                {
                    buffs.Buffs[i] = buff;
                }
            }
        }
    }
}
