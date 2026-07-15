using Evo.MOBA.Configuration;
using Evo.MOBA.Match;
using Evo.MOBA.Update;

namespace Evo.MOBA;

public static class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("=== DotaX Zero Friction Server ===");
        Console.WriteLine();

        var config = EvoConfig.Load("./evoconfig.json");

        if (!string.IsNullOrEmpty(config.UpdateUrl))
        {
            var updater = new UpdateService(config.UpdateUrl);
            var updated = await updater.CheckAndUpdateAsync();
            if (updated) return;
            updater.CleanupOldUpdates();
        }

        if (args.Length > 0 && int.TryParse(args[0], out var port))
            config.ListenPort = port;

        if (args.Length > 1 && int.TryParse(args[1], out var heroesPerTeam))
            config.HeroesPerTeam = heroesPerTeam;

        var matchConfig = config.ToMatchConfig();
        var orchestrator = new MatchOrchestrator(matchConfig);
        orchestrator.Run();
    }
}
