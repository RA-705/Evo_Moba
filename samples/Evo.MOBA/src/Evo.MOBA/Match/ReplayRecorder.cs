using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Evo.Core.ECS;
using Evo.MOBA.Combat;
using Evo.MOBA.Systems;
using Evo.Shared.Math;

namespace Evo.MOBA.Match;

public struct ReplayFrame
{
    public float Timestamp;
    public Dictionary<int, EvoVector3> Positions;
    public Dictionary<int, float> HealthValues;
    public List<ReplayEvent> Events;
}

public struct ReplayEvent
{
    public float Timestamp;
    public string Type;
    public int SourceId;
    public int TargetId;
    public float Value;
}

public sealed class ReplayRecorder : ISystem
{
    private readonly List<ReplayFrame> _frames = new();
    private readonly List<ReplayEvent> _pendingEvents = new();
    private float _gameTime;
    private float _frameInterval = 0.1f;
    private float _timeSinceLastFrame;

    public IReadOnlyList<ReplayFrame> Frames => _frames;

    public void OnTick(World world, float deltaTime)
    {
        _gameTime += deltaTime;
        _timeSinceLastFrame += deltaTime;

        if (_timeSinceLastFrame >= _frameInterval)
        {
            _timeSinceLastFrame = 0f;
            CaptureFrame(world);
        }
    }

    private void CaptureFrame(World world)
    {
        var positions = new Dictionary<int, EvoVector3>();
        var healthValues = new Dictionary<int, float>();

        foreach (var id in world.GetEntityIds<PositionComponent>())
        {
            if (world.TryGetComponent<PositionComponent>(id, out var pos))
                positions[id.Value] = pos.Value;
        }

        foreach (var id in world.GetEntityIds<HealthComponent>())
        {
            if (world.TryGetComponent<HealthComponent>(id, out var health))
                healthValues[id.Value] = health.Current;
        }

        var events = new List<ReplayEvent>(_pendingEvents);
        _pendingEvents.Clear();

        _frames.Add(new ReplayFrame
        {
            Timestamp = _gameTime,
            Positions = positions,
            HealthValues = healthValues,
            Events = events,
        });
    }

    public void RecordEvent(string type, int sourceId, int targetId, float value)
    {
        _pendingEvents.Add(new ReplayEvent
        {
            Timestamp = _gameTime,
            Type = type,
            SourceId = sourceId,
            TargetId = targetId,
            Value = value,
        });
    }

    public void Export(string filePath)
    {
        var sb = new StringBuilder();
        sb.AppendLine("{");
        sb.AppendLine($"  \"duration\": {_gameTime:F1},");
        sb.AppendLine($"  \"frame_count\": {_frames.Count},");
        sb.AppendLine("  \"frames\": [");

        for (int i = 0; i < _frames.Count; i++)
        {
            var f = _frames[i];
            sb.AppendLine("    {");
            sb.AppendLine($"      \"t\": {f.Timestamp:F2},");

            sb.Append("      \"pos\": {");
            bool first = true;
            foreach (var kv in f.Positions)
            {
                if (!first) sb.Append(",");
                sb.Append($"\"{kv.Key}\":[{kv.Value.X:F1},{kv.Value.Z:F1}]");
                first = false;
            }
            sb.AppendLine("},");

            sb.Append("      \"hp\": {");
            first = true;
            foreach (var kv in f.HealthValues)
            {
                if (!first) sb.Append(",");
                sb.Append($"\"{kv.Key}\":{kv.Value:F0}");
                first = false;
            }
            sb.AppendLine("}");

            sb.Append(i < _frames.Count - 1 ? "    }," : "    }");
            sb.AppendLine();
        }

        sb.AppendLine("  ]");
        sb.AppendLine("}");

        File.WriteAllText(filePath, sb.ToString());
    }
}
