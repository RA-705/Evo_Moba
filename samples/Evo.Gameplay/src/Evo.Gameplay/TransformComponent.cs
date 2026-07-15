namespace Evo.Gameplay;

public sealed class TransformComponent : EntityComponent
{
    public float X;
    public float Y;
    public float Z;
    public float VelocityX;
    public float VelocityY;
    public float VelocityZ;

    public override void SaveState(BinaryWriter writer)
    {
        writer.Write(X);
        writer.Write(Y);
        writer.Write(Z);
        writer.Write(VelocityX);
        writer.Write(VelocityY);
        writer.Write(VelocityZ);
    }

    public override void LoadState(BinaryReader reader)
    {
        X = reader.ReadSingle();
        Y = reader.ReadSingle();
        Z = reader.ReadSingle();
        VelocityX = reader.ReadSingle();
        VelocityY = reader.ReadSingle();
        VelocityZ = reader.ReadSingle();
    }
}
