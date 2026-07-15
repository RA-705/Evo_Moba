using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Linq;

namespace Evo.Editor
{
    public static class DemoVerifier
    {
        [MenuItem("Evo/Verify AI Battle Demo")]
        public static void Verify()
        {
            Debug.Log("=== AI BATTLE DEMO VERIFICATION ===");

            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            EditorApplication.EnterPlaymode();

            EditorApplication.delayCall += () =>
            {
                CheckDemo();
            };
        }

        private static void CheckDemo()
        {
            var demo = Object.FindObjectOfType<Evo.Demo.AIBattleDemo>();
            if (demo != null)
            {
                Debug.Log("[VERIFY] AIBattleDemo instance found");
            }
            else
            {
                Debug.LogError("[VERIFY] AIBattleDemo instance NOT found");
            }

            var matchMgr = Evo.Core.MatchSceneManager.Instance;
            if (matchMgr != null)
            {
                int heroCount = matchMgr.GetHeroes().Count;
                Debug.Log($"[VERIFY] MatchSceneManager active with {heroCount} heroes");
            }
            else
            {
                Debug.LogError("[VERIFY] MatchSceneManager NOT found");
            }

            var aiMgr = Evo.AI.AIManager.Instance;
            if (aiMgr != null)
            {
                Debug.Log("[VERIFY] AIManager active");
            }
            else
            {
                Debug.LogError("[VERIFY] AIManager NOT found");
            }

            Debug.Log("=== VERIFICATION COMPLETE ===");
        }
    }
}
