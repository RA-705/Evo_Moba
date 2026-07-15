using UnityEngine;
using Evo.Demo;

namespace Evo.Core
{
    public static class DemoEntryPoint
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            var demoGo = new GameObject("AIBattleDemo", typeof(AIBattleDemo));
            Object.DontDestroyOnLoad(demoGo);
            Debug.Log("[DemoEntryPoint] AI Battle Demo initialized. Press Play to start.");
        }
    }
}
