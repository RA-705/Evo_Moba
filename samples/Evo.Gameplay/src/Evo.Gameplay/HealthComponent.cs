namespace Evo.Gameplay;

public sealed class HealthComponent : EntityComponent
{
    public float CurrentHealth;
    public float MaxHealth;

    public void ModifyHealth(float amount)
    {
        CurrentHealth = Math.Clamp(CurrentHealth + amount, 0f, MaxHealth);
    }

    public override void SaveState(BinaryWriter writer)
    {
        writer.Write(CurrentHealth);
        writer.Write(MaxHealth);
    }

    public override void LoadState(BinaryReader reader)
    {
        CurrentHealth = reader.ReadSingle();
        MaxHealth = reader.ReadSingle();
    }
}
