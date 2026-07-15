using System.Runtime.InteropServices;

namespace Evo.Shared.Networking;

[StructLayout(LayoutKind.Sequential)]
public struct EntitySnapshot
{
    public int EntityId;
    public float PosX;
    public float PosY;
    public float PosZ;
    public float CurrentHealth;
    public int TeamId;
    public uint StateFlags;
}

public static class EntityStateFlags
{
    public const uint Idle = 0;
    public const uint Moving = 1 << 0;
    public const uint Attacking = 1 << 1;
    public const uint Fleeing = 1 << 2;
    public const uint Dead = 1 << 3;
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct WorldSnapshot
{
    public const int MaxEntities = 256;

    public uint TickNumber;
    public int EntityCount;
    public fixed byte EntitiesRaw[7168]; // 256 * sizeof(EntitySnapshot) = 256 * 28

    public Span<EntitySnapshot> Entities
    {
        get
        {
            fixed (byte* ptr = EntitiesRaw)
                return new Span<EntitySnapshot>(ptr, MaxEntities);
        }
    }
}
