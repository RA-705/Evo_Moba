using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Evo.MOBA.Update;

public sealed class UpdateService
{
    private readonly string _currentExePath;
    private readonly string _updateDirectory;
    private readonly string _versionUrl;
    private readonly HttpClient _http = new();

    public UpdateService(string updateUrl)
    {
        _currentExePath = Process.GetCurrentProcess().MainModule?.FileName ?? "";
        _updateDirectory = Path.Combine(AppContext.BaseDirectory, "updates");
        _versionUrl = updateUrl;
    }

    public async Task<bool> CheckAndUpdateAsync()
    {
        try
        {
            Console.WriteLine("[Updater] Checking for updates...");

            var remoteVersion = await DownloadVersionJsonAsync();
            if (remoteVersion == null)
            {
                Console.WriteLine("[Updater] Could not fetch version info.");
                return false;
            }

            var localVersion = LoadLocalVersion();
            if (localVersion == null)
            {
                Console.WriteLine("[Updater] No local version found. Treating as fresh install.");
                return await DownloadAndApplyUpdateAsync(remoteVersion);
            }

            if (remoteVersion.Build <= localVersion.Build)
            {
                Console.WriteLine($"[Updater] Already up to date (v{localVersion.Version} build {localVersion.Build}).");
                return false;
            }

            Console.WriteLine($"[Updater] New version available: v{remoteVersion.Version} build {remoteVersion.Build}");
            Console.WriteLine($"[Updater] Current: v{localVersion.Version} build {localVersion.Build}");

            return await DownloadAndApplyUpdateAsync(remoteVersion);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Updater] Error: {ex.Message}");
            return false;
        }
    }

    private async Task<VersionInfo?> DownloadVersionJsonAsync()
    {
        try
        {
            var url = $"{_versionUrl}/version.json";
            var json = await _http.GetStringAsync(url);
            return JsonSerializer.Deserialize<VersionInfo>(json);
        }
        catch
        {
            return null;
        }
    }

    private VersionInfo? LoadLocalVersion()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "version.json");
        if (!File.Exists(path)) return null;

        try
        {
            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<VersionInfo>(json);
        }
        catch
        {
            return null;
        }
    }

    private async Task<bool> DownloadAndApplyUpdateAsync(VersionInfo remote)
    {
        try
        {
            Directory.CreateDirectory(_updateDirectory);

            var zipPath = Path.Combine(_updateDirectory, $"update_build{remote.Build}.zip");
            var downloadUrl = $"{_versionUrl}/builds/build_{remote.Build}.zip";

            Console.WriteLine($"[Updater] Downloading from {downloadUrl}...");
            var bytes = await _http.GetByteArrayAsync(downloadUrl);
            await File.WriteAllBytesAsync(zipPath, bytes);

            Console.WriteLine("[Updater] Extracting...");
            var extractDir = Path.Combine(_updateDirectory, $"build_{remote.Build}");
            if (Directory.Exists(extractDir))
                Directory.Delete(extractDir, true);

            ZipFile.ExtractToDirectory(zipPath, extractDir);

            var backupDir = Path.Combine(_updateDirectory, "backup");
            Directory.CreateDirectory(backupDir);

            var currentDir = AppContext.BaseDirectory;
            foreach (var file in Directory.GetFiles(currentDir))
            {
                var fileName = Path.GetFileName(file);
                if (fileName == "Evo.MOBA.exe" || fileName == "Evo.MOBA.dll")
                {
                    var backupPath = Path.Combine(backupDir, fileName);
                    File.Copy(file, backupPath, true);
                }
            }

            var newExe = Path.Combine(extractDir, "Evo.MOBA.exe");
            if (File.Exists(newExe))
            {
                Console.WriteLine("[Updater] Update ready. Restarting...");
                Process.Start(newExe);
                Process.GetCurrentProcess().Kill();
                return true;
            }

            Console.WriteLine("[Updater] Update package incomplete.");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Updater] Download failed: {ex.Message}");
            return false;
        }
    }

    public void CleanupOldUpdates()
    {
        try
        {
            if (Directory.Exists(_updateDirectory))
            {
                var dirs = Directory.GetDirectories(_updateDirectory, "build_*");
                foreach (var dir in dirs)
                {
                    try { Directory.Delete(dir, true); } catch { }
                }

                var zips = Directory.GetFiles(_updateDirectory, "update_*.zip");
                foreach (var zip in zips)
                {
                    try { File.Delete(zip); } catch { }
                }
            }
        }
        catch { }
    }
}

public class VersionInfo
{
    public string Version { get; set; } = "1.0.0";
    public int Build { get; set; } = 1;
    public int MinClientVersion { get; set; } = 1;
    public string UpdateUrl { get; set; } = "";
    public string Changelog { get; set; } = "";
}
