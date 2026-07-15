using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Evo.MOBA.Learning;

public sealed class TrajectoryExporter
{
    private const string DateFormat = "yyyy-MM-dd_HH-mm-ss";
    private string _logPath;

    public TrajectoryExporter(string logDirectory)
    {
        _logPath = Path.Combine(logDirectory, $"{DateTimeOffset.Now:yyyy-MM-dd_HH-mm-ss}_trajectory");

        if (!Directory.Exists(_logPath))
            Directory.CreateDirectory(_logPath);
    }

    public void ExportTrajectory(List<TrajectoryStep> steps, int episodeId)
    {
        var fileName = $"episode_{episodeId:D4}.csv";
        var filePath = Path.Combine(_logPath, fileName);

        var sb = new StringBuilder();
        AppendRow(sb, "tick", "pos_x", "pos_z", "health_pct", "action_id", "reward");

        foreach (var step in steps)
        {
            AppendRow(sb,
                step.Tick.ToString(),
                step.PosX.ToString("F3"),
                step.PosZ.ToString("F3"),
                step.HealthPercent.ToString("F3"),
                step.ActionId.ToString(),
                step.Reward.ToString("F4"));
        }

        File.WriteAllText(filePath, sb.ToString());
    }

    public void ExportAsMlcubeDataset(List<List<TrajectoryStep>> episodes, int totalSteps)
    {
        var filePath = Path.Combine(_logPath, "mlcube_data.csv");
        var sb = new StringBuilder();

        AppendRow(sb, "step_id", "episode",
            "pos_x", "pos_z", "health_pct",
            "action_id", "reward");

        var globalStep = 0;
        for (var episodeIdx = 0; episodeIdx < episodes.Count; episodeIdx++)
        {
            var episode = episodes[episodeIdx];
            for (var stepIdx = 0; stepIdx < episode.Count; stepIdx++)
            {
                var step = episode[stepIdx];
                AppendRow(sb, globalStep.ToString(), episodeIdx.ToString(),
                    step.PosX.ToString("F3"),
                    step.PosZ.ToString("F3"),
                    step.HealthPercent.ToString("F3"),
                    step.ActionId.ToString(),
                    step.Reward.ToString("F4"));
                globalStep++;
            }
            sb.AppendLine();
        }

        File.WriteAllText(filePath, sb.ToString());
    }

    private static void AppendRow(StringBuilder sb, params string[] values)
    {
        sb.AppendLine(string.Join(",", values));
    }
}
