using Evo.MOBA.Configuration;
using Evo.MOBA.Admin;
using Evo.MOBA.Match;

var config = EvoConfig.Load("./evoconfig.json").ToMatchConfig();

var orchestrator = new MatchOrchestrator(config);
orchestrator.Run();
