namespace Evo.Gameplay;

[Flags]
public enum ComponentMask : uint
{
    None = 0,
    Transform = 1 << 0,
    Health = 1 << 1,
}
