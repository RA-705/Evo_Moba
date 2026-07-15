using System.Collections.Generic;
using UnityEngine;
using Evo.AI;

namespace Evo.Core
{
    public class MatchSceneManager : MonoBehaviour
    {
        public static MatchSceneManager Instance { get; private set; }

        [Header("Spawn Points")]
        public Transform[] Team1Spawns;
        public Transform[] Team2Spawns;

        private List<HeroController> _heroes = new List<HeroController>();
        private bool _battleActive;

        private void Awake()
        {
            Instance = this;
        }

        public void StartAIBattle(int teamSize = 3)
        {
            _heroes.Clear();

            int[] team1HeroIds = { 1, 28, 11 };
            int[] team2HeroIds = { 4, 24, 36 };

            for (int i = 0; i < teamSize && i < team1HeroIds.Length; i++)
            {
                var pos = Team1Spawns != null && i < Team1Spawns.Length
                    ? Team1Spawns[i].position
                    : new Vector3(-15 - i * 3, 0, -5 + i * 5);
                var hero = SpawnHero(team1HeroIds[i], 0, pos, true);
                if (hero != null) _heroes.Add(hero);
            }

            for (int i = 0; i < teamSize && i < team2HeroIds.Length; i++)
            {
                var pos = Team2Spawns != null && i < Team2Spawns.Length
                    ? Team2Spawns[i].position
                    : new Vector3(15 + i * 3, 0, -5 + i * 5);
                var hero = SpawnHero(team2HeroIds[i], 1, pos, true);
                if (hero != null) _heroes.Add(hero);
            }

            var camera = Camera.main ?? FindObjectOfType<Camera>();
            if (camera != null && _heroes.Count > 0)
            {
                var center = Vector3.zero;
                foreach (var h in _heroes) center += h.transform.position;
                center /= _heroes.Count;
                camera.transform.position = new Vector3(center.x, 25, center.z - 25);
                camera.transform.LookAt(center);
            }

            _battleActive = true;
            Debug.Log($"[MatchScene] AI battle started with {_heroes.Count} heroes");
        }

        private HeroController SpawnHero(int heroId, int teamId, Vector3 position, bool isAI)
        {
            var prefab = LoadHeroPrefab(heroId);
            GameObject go;
            if (prefab != null)
            {
                go = Instantiate(prefab, position, Quaternion.identity);
                go.AddComponent<CharacterController>();
            }
            else
            {
                go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                go.transform.position = position;
                go.transform.localScale = new Vector3(0.8f, 1, 0.8f);
                go.AddComponent<CharacterController>();
            }

            go.SetActive(true);
            go.name = $"Hero_{heroId}_Team{teamId}";

            var heroController = go.GetComponent<HeroController>();
            if (heroController == null)
                heroController = go.AddComponent<HeroController>();

            heroController.Initialize(heroId, teamId);

            CreateHealthBar(go);

            if (isAI)
            {
                var ai = go.GetComponent<HeroAI>();
                if (ai == null) ai = go.AddComponent<HeroAI>();
                ai.Initialize(heroController);
            }

            return heroController;
        }

        private GameObject LoadHeroPrefab(int heroId)
        {
            string[] heroNames = {
                "001_Keth", "004_Mikal", "008_Tigria", "011_Eida", "017_Reisa",
                "024_Kyouya", "026_Norah", "027_Salome", "028_Wulfric", "030_Adele", "036_Lars"
            };
            string dirName = null;
            foreach (var n in heroNames)
            {
                if (n.StartsWith(heroId.ToString("D3"))) { dirName = n; break; }
            }
            if (dirName == null) return null;
            return Resources.Load<GameObject>($"Heroes/{dirName}");
        }

        private void CreateHealthBar(GameObject hero)
        {
            var barGo = GameObject.CreatePrimitive(PrimitiveType.Quad);
            barGo.name = "HealthBarBg";
            barGo.transform.SetParent(hero.transform);
            barGo.transform.localPosition = new Vector3(0, 2.5f, 0);
            barGo.transform.localScale = new Vector3(2f, 0.2f, 1f);
            var barMat = barGo.GetComponent<Renderer>().material;
            barMat.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);

            var fillGo = GameObject.CreatePrimitive(PrimitiveType.Quad);
            fillGo.name = "HealthBarFill";
            fillGo.transform.SetParent(barGo.transform);
            fillGo.transform.localPosition = new Vector3(0.5f, 0, -0.01f);
            fillGo.transform.localScale = new Vector3(0.98f, 0.9f, 1f);
            var fillMat = fillGo.GetComponent<Renderer>().material;
            fillMat.color = Color.green;
        }

        public void Tick()
        {
            if (!_battleActive) return;
        }

        public List<HeroController> GetHeroes() => _heroes;

        public List<HeroController> GetEnemyHeroes(int teamId)
        {
            var enemies = new List<HeroController>();
            foreach (var h in _heroes)
                if (h.TeamId != teamId && h.IsAlive)
                    enemies.Add(h);
            return enemies;
        }

        public List<HeroController> GetTeamHeroes(int teamId)
        {
            var allies = new List<HeroController>();
            foreach (var h in _heroes)
                if (h.TeamId == teamId && h.IsAlive)
                    allies.Add(h);
            return allies;
        }

        public HeroController GetNearestEnemy(Vector3 pos, int teamId, float maxRange = float.MaxValue)
        {
            HeroController nearest = null;
            float minDist = maxRange;
            foreach (var h in _heroes)
            {
                if (h.TeamId == teamId || !h.IsAlive) continue;
                float d = Vector3.Distance(pos, h.transform.position);
                if (d < minDist) { minDist = d; nearest = h; }
            }
            return nearest;
        }
    }
}
