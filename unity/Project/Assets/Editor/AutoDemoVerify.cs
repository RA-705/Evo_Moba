using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Evo.Editor
{
    public static class AutoDemoVerify
    {
        private static float _startTime;
        private static int _phase;

        public static void RunVerification()
        {
            _phase = 0;
            _startTime = Time.realtimeSinceStartup;
            EditorApplication.update += VerificationUpdate;
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            EditorApplication.EnterPlaymode();
            Debug.Log("[AutoVerify] Entering Play Mode...");
        }

        private static void VerificationUpdate()
        {
            float elapsed = Time.realtimeSinceStartup - _startTime;

            if (_phase == 0 && EditorApplication.isPlaying)
            {
                _phase = 1;
                _startTime = Time.realtimeSinceStartup;
                Debug.Log("[AutoVerify] Play Mode entered, waiting for initialization...");
                return;
            }

            if (_phase == 1 && elapsed > 3.0f)
            {
                _phase = 2;
                Debug.Log("[AutoVerify] === CHECKING DEMO STATE ===");

                var demo = Object.FindObjectOfType<Evo.Demo.AIBattleDemo>();
                var matchMgr = Evo.Core.MatchSceneManager.Instance;
                var aiMgr = Evo.AI.AIManager.Instance;

                bool allOk = true;

                if (demo != null) { Debug.Log("[AutoVerify] OK: AIBattleDemo found"); }
                else { Debug.LogError("[AutoVerify] FAIL: AIBattleDemo not found"); allOk = false; }

                if (matchMgr != null) {
                    int count = matchMgr.GetHeroes().Count;
                    Debug.Log($"[AutoVerify] OK: MatchSceneManager with {count} heroes");
                    if (count < 6) { Debug.LogError($"[AutoVerify] FAIL: Expected 6 heroes, got {count}"); allOk = false; }
                } else { Debug.LogError("[AutoVerify] FAIL: MatchSceneManager not found"); allOk = false; }

                if (aiMgr != null) { Debug.Log("[AutoVerify] OK: AIManager found"); }
                else { Debug.LogError("[AutoVerify] FAIL: AIManager not found"); allOk = false; }

                var heroes = Resources.FindObjectsOfTypeAll<Evo.AI.HeroController>();
                Debug.Log($"[AutoVerify] HeroController instances in scene: {heroes.Length}");

                if (allOk) { Debug.Log("[AutoVerify] === VERIFICATION PASSED ==="); }
                else { Debug.LogError("[AutoVerify] === VERIFICATION FAILED ==="); }

                EditorApplication.ExitPlaymode();
                EditorApplication.update -= VerificationUpdate;
                Debug.Log("[AutoVerify] Exited Play Mode. Verification complete.");
            }
        }
    }
}
