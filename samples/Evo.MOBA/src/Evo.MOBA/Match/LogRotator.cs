using System;
using System.IO;
using System.Linq;
using System.Timers;

namespace Evo.MOBA.Match;

public sealed class LogRotator : IDisposable
{
    private readonly string _logDirectory;
    private readonly int _maxFiles;
    private readonly System.Timers.Timer _timer;

    public LogRotator(string logDirectory, int maxFiles = 50)
    {
        _logDirectory = logDirectory;
        _maxFiles = maxFiles;

        if (!Directory.Exists(logDirectory))
            Directory.CreateDirectory(logDirectory);

        _timer = new System.Timers.Timer(300_000);
        _timer.Elapsed += (_, _) => Rotate();
        _timer.AutoReset = true;
        _timer.Start();

        Rotate();
    }

    private void Rotate()
    {
        try
        {
            var files = Directory.GetFiles(_logDirectory, "*.*")
                .Select(f => new FileInfo(f))
                .OrderByDescending(f => f.LastWriteTime)
                .ToList();

            if (files.Count <= _maxFiles) return;

            foreach (var file in files.Skip(_maxFiles))
            {
                file.Delete();
            }
        }
        catch
        {
            // Silent — log rotator never crashes
        }
    }

    public void Dispose() => _timer?.Dispose();
}
