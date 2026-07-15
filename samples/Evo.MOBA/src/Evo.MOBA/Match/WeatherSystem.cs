using System;
using Evo.Core.ECS;
using Evo.MOBA.Systems;

namespace Evo.MOBA.Match;

public enum WeatherType : byte
{
    Clear,
    Rain,
    Fog,
    Storm,
    Sandstorm,
}

public struct WeatherComponent : IComponent
{
    public WeatherType CurrentWeather;
    public float Duration;
    public float TimeRemaining;
    public float TransitionProgress;
}

public struct WeatherEffects
{
    public float MoveSpeedMultiplier;
    public float VisionRangeMultiplier;
    public float DamageMultiplier;
    public float CooldownMultiplier;
    public float LifeStealMultiplier;

    public static WeatherEffects GetEffects(WeatherType weather) => weather switch
    {
        WeatherType.Clear => new()
        {
            MoveSpeedMultiplier = 1f,
            VisionRangeMultiplier = 1f,
            DamageMultiplier = 1f,
            CooldownMultiplier = 1f,
            LifeStealMultiplier = 1f,
        },
        WeatherType.Rain => new()
        {
            MoveSpeedMultiplier = 0.9f,
            VisionRangeMultiplier = 0.85f,
            DamageMultiplier = 0.95f,
            CooldownMultiplier = 1.05f,
            LifeStealMultiplier = 1.1f,
        },
        WeatherType.Fog => new()
        {
            MoveSpeedMultiplier = 0.95f,
            VisionRangeMultiplier = 0.6f,
            DamageMultiplier = 1f,
            CooldownMultiplier = 1f,
            LifeStealMultiplier = 1f,
        },
        WeatherType.Storm => new()
        {
            MoveSpeedMultiplier = 0.8f,
            VisionRangeMultiplier = 0.7f,
            DamageMultiplier = 1.15f,
            CooldownMultiplier = 1.2f,
            LifeStealMultiplier = 0.8f,
        },
        WeatherType.Sandstorm => new()
        {
            MoveSpeedMultiplier = 0.7f,
            VisionRangeMultiplier = 0.4f,
            DamageMultiplier = 0.9f,
            CooldownMultiplier = 1.3f,
            LifeStealMultiplier = 0.9f,
        },
        _ => new()
        {
            MoveSpeedMultiplier = 1f,
            VisionRangeMultiplier = 1f,
            DamageMultiplier = 1f,
            CooldownMultiplier = 1f,
            LifeStealMultiplier = 1f,
        },
    };
}

public sealed class WeatherSystem : ISystem
{
    private readonly Random _rng = new();
    private const float MinWeatherDuration = 30f;
    private const float MaxWeatherDuration = 120f;

    public void OnTick(World world, float deltaTime)
    {
        foreach (var id in world.GetEntityIds<WeatherComponent>())
        {
            ref var weather = ref world.GetComponent<WeatherComponent>(id);

            weather.TimeRemaining -= deltaTime;
            weather.TransitionProgress = Math.Clamp(1f - (weather.TimeRemaining / weather.Duration), 0f, 1f);

            if (weather.TimeRemaining <= 0f)
            {
                var newWeather = (WeatherType)_rng.Next(0, 5);
                weather.CurrentWeather = newWeather;
                weather.Duration = MinWeatherDuration + (float)_rng.NextDouble() * (MaxWeatherDuration - MinWeatherDuration);
                weather.TimeRemaining = weather.Duration;
                weather.TransitionProgress = 0f;
                world.SetComponent(id, weather);
            }
        }
    }
}
