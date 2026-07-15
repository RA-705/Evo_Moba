namespace Evo.Gameplay;

public abstract class EntityComponent
{
    public abstract void SaveState(BinaryWriter writer);
    public abstract void LoadState(BinaryReader reader);
}
