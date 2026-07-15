using UnityEngine;
using Evo.AI;
using Evo.Core;

namespace Evo.Demo
{
    public class AIBattleDemo : MonoBehaviour
    {
        [Header("Battle Settings")]
        public int HeroesPerTeam = 3;
        public bool SpawnAtRuntime = true;

        private void Start()
        {
            if (SpawnAtRuntime)
                SetupBattleScene();
        }

        private void SetupBattleScene()
        {
            var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Arena_Floor";
            ground.transform.localScale = new Vector3(10, 1, 10);

            var lightGo = new GameObject("DirectionalLight", typeof(Light));
            lightGo.GetComponent<Light>().type = LightType.Directional;
            lightGo.transform.rotation = Quaternion.Euler(50, -30, 0);

            var cameraGo = Camera.main?.gameObject;
            if (cameraGo == null)
            {
                cameraGo = new GameObject("MainCamera", typeof(Camera), typeof(AudioListener));
                cameraGo.tag = "MainCamera";
            }
            cameraGo.transform.position = new Vector3(0, 25, -20);
            cameraGo.transform.rotation = Quaternion.Euler(45, 0, 0);

            var t1Parent = new GameObject("Team1_Spawns").transform;
            var spawns1 = new Transform[HeroesPerTeam];
            for (int i = 0; i < HeroesPerTeam; i++)
            {
                var go = new GameObject($"Spawn_T1_{i}");
                go.transform.SetParent(t1Parent);
                go.transform.position = new Vector3(-15 - i * 2, 0, -3 + i * 3);
                spawns1[i] = go.transform;
            }

            var t2Parent = new GameObject("Team2_Spawns").transform;
            var spawns2 = new Transform[HeroesPerTeam];
            for (int i = 0; i < HeroesPerTeam; i++)
            {
                var go = new GameObject($"Spawn_T2_{i}");
                go.transform.SetParent(t2Parent);
                go.transform.position = new Vector3(15 + i * 2, 0, -3 + i * 3);
                spawns2[i] = go.transform;
            }

            var matchGo = new GameObject("MatchSceneManager", typeof(MatchSceneManager));
            var matchMgr = matchGo.GetComponent<MatchSceneManager>();
            matchMgr.Team1Spawns = spawns1;
            matchMgr.Team2Spawns = spawns2;

            new GameObject("AIManager", typeof(AIManager));

            matchMgr.StartAIBattle(HeroesPerTeam);

            Debug.Log($"[AIBattleDemo] Battle started: {HeroesPerTeam}v{HeroesPerTeam}");
        }
    }
}
