using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Evo.Core.ECS;
using Evo.MOBA.Combat;
using Evo.MOBA.Systems;

namespace Evo.MOBA.Match;

public struct MatchStatsTracker : IComponent
{
    public float TotalDamageDealt;
    public int Kills;
    public int Deaths;
}

public sealed class MatchStatsReportSystem : ISystem
{
    public void OnTick(World world, float deltaTime)
    {
        foreach (var evtId in world.GetEntityIds<DamageDealtEventComponent>())
        {
            if (!world.TryGetComponent<DamageDealtEventComponent>(evtId, out var evt))
                continue;

            if (evt.SourceId != Entity.Null.Id && world.TryGetComponent<MatchStatsTracker>(evt.SourceId, out var srcStats))
            {
                srcStats.TotalDamageDealt += evt.FinalDamage;
                world.SetComponent(evt.SourceId, srcStats);
            }

            if (evt.TargetId != Entity.Null.Id && world.TryGetComponent<HealthComponent>(evt.TargetId, out var targetHealth))
            {
                if (targetHealth.Current <= 0f && world.TryGetComponent<MatchStatsTracker>(evt.TargetId, out var tgtStats))
                {
                    tgtStats.Deaths++;
                    world.SetComponent(evt.TargetId, tgtStats);
                }

                if (targetHealth.Current <= 0f && evt.SourceId != Entity.Null.Id &&
                    world.TryGetComponent<MatchStatsTracker>(evt.SourceId, out var killerStats))
                {
                    killerStats.Kills++;
                    world.SetComponent(evt.SourceId, killerStats);
                }
            }
        }
    }
}

public sealed class MatchStatsReport
{
    private readonly int _winnerTeam;
    private readonly TimeSpan _duration;
    private readonly List<HeroStats> _heroes = new();

    public sealed class HeroStats
    {
        public int EntityId;
        public int TeamId;
        public float FinalHealth;
        public float MaxHealth;
        public float TotalDamageDealt;
        public int Kills;
        public int Deaths;
    }

    public MatchStatsReport(World world, int winnerTeam, TimeSpan duration)
    {
        _winnerTeam = winnerTeam;
        _duration = duration;

        foreach (var id in world.GetEntityIds<HealthComponent>())
        {
            if (!world.TryGetComponent<HealthComponent>(id, out var health) ||
                !world.TryGetComponent<TeamComponent>(id, out var team))
                continue;

            float totalDmg = 0;
            int kills = 0;
            int deaths = 0;

            if (world.TryGetComponent<MatchStatsTracker>(id, out var stats))
            {
                totalDmg = stats.TotalDamageDealt;
                kills = stats.Kills;
                deaths = stats.Deaths;
            }

            _heroes.Add(new HeroStats
            {
                EntityId = id.Value,
                TeamId = team.TeamId,
                FinalHealth = health.Current,
                MaxHealth = health.Max,
                TotalDamageDealt = totalDmg,
                Kills = kills,
                Deaths = deaths,
            });
        }
    }

    public void Export(string filePath)
    {
        var sb = new StringBuilder();
        sb.AppendLine("{");
        sb.AppendLine($"  \"winner_team\": {_winnerTeam},");
        sb.AppendLine($"  \"duration_seconds\": {_duration.TotalSeconds:F1},");
        sb.AppendLine("  \"heroes\": [");

        for (int i = 0; i < _heroes.Count; i++)
        {
            var h = _heroes[i];
            sb.AppendLine("    {");
            sb.AppendLine($"      \"entity_id\": {h.EntityId},");
            sb.AppendLine($"      \"team_id\": {h.TeamId},");
            sb.AppendLine($"      \"final_health\": {h.FinalHealth:F1},");
            sb.AppendLine($"      \"max_health\": {h.MaxHealth:F1},");
            sb.AppendLine($"      \"total_damage\": {h.TotalDamageDealt:F1},");
            sb.AppendLine($"      \"kills\": {h.Kills},");
            sb.AppendLine($"      \"deaths\": {h.Deaths}");
            sb.Append(i < _heroes.Count - 1 ? "    }," : "    }");
            sb.AppendLine();
        }

        sb.AppendLine("  ]");
        sb.AppendLine("}");

        File.WriteAllText(filePath, sb.ToString());
    }
}
