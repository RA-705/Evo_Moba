using System;
using System.IO;
using System.Text.Json;
using Evo.MOBA.Match;

namespace Evo.MOBA.Configuration;

public sealed class EvoConfig
{
    public int GridWidth { get; set; } = 30;
    public int GridDepth { get; set; } = 30;
    public float CellSize { get; set; } = 1.5f;
    public int HeroesPerTeam { get; set; } = 3;
    public float TicksPerSecond { get; set; } = 10f;
    public int ListenPort { get; set; } = 7777;
    public bool EnableNetworking { get; set; } = true;
    public bool EnableTrajectoryRecording { get; set; } = true;
    public bool EnableFogOfWar { get; set; } = true;
    public bool EnableTeamPlanner { get; set; } = true;
    public bool EnableWards { get; set; } = true;
    public string LogDirectory { get; set; } = "./logs";
    public float MatchTimeoutMinutes { get; set; } = 30f;
    public float CreepWaveInterval { get; set; } = 15f;
    public float RespawnDelay { get; set; } = 5f;
    public int StartGold { get; set; } = 500;
    public string[] AdminWhitelist { get; set; } = Array.Empty<string>();
    public string ServerName { get; set; } = "EVO Match";
    public string UpdateUrl { get; set; } = "";

    public static EvoConfig Load(string filePath = "./evoconfig.json")
    {
        if (!File.Exists(filePath))
        {
            var defaults = new EvoConfig();
            var json = JsonSerializer.Serialize(defaults, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
            Console.WriteLine($"[EvoConfig] Created default config at {filePath}");
            return defaults;
        }

        try
        {
            var text = File.ReadAllText(filePath);
            var config = JsonSerializer.Deserialize<EvoConfig>(text);
            Console.WriteLine($"[EvoConfig] Loaded config from {filePath}");
            return config ?? new EvoConfig();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[EvoConfig] Failed to load config: {ex.Message}. Using defaults.");
            return new EvoConfig();
        }
    }

    public MatchConfig ToMatchConfig() => new()
    {
        GridWidth = GridWidth,
        GridDepth = GridDepth,
        CellSize = CellSize,
        HeroesPerTeam = HeroesPerTeam,
        TicksPerSecond = TicksPerSecond,
        ListenPort = ListenPort,
        EnableNetworking = EnableNetworking,
        LogDirectory = LogDirectory,
        MatchTimeoutMinutes = MatchTimeoutMinutes,
        EnableTrajectoryRecording = EnableTrajectoryRecording,
        EnableFogOfWar = EnableFogOfWar,
        EnableTeamPlanner = EnableTeamPlanner,
        EnableWards = EnableWards,
        CreepWaveInterval = CreepWaveInterval,
        RespawnDelay = RespawnDelay,
        StartGold = StartGold,
        AdminWhitelist = AdminWhitelist,
        ServerName = ServerName,
    };
}
