using System.Collections.Generic;
using UnityEngine;

namespace Evo.AI
{
    public class AIManager : MonoBehaviour
    {
        public static AIManager Instance { get; private set; }

        public enum AIDifficulty { Easy, Normal, Hard }
        public enum TeamStrategy { Push, Defend, Jungle, Group }

        [Header("AI Settings")]
        public AIDifficulty Difficulty = AIDifficulty.Normal;
        public float DecisionInterval = 1f;

        private List<HeroAI> _allAI = new List<HeroAI>();
        private Dictionary<int, TeamStrategy> _teamStrategies = new Dictionary<int, TeamStrategy>();
        private float _nextDecisionTime;

        private void Awake()
        {
            Instance = this;
        }

        public void RegisterAI(HeroAI ai)
        {
            if (!_allAI.Contains(ai))
                _allAI.Add(ai);
        }

        public void UnregisterAI(HeroAI ai)
        {
            _allAI.Remove(ai);
        }

        private void Update()
        {
            if (Time.time < _nextDecisionTime) return;
            _nextDecisionTime = Time.time + DecisionInterval;
            MakeTeamDecisions();
        }

        private void MakeTeamDecisions()
        {
            foreach (var ai in _allAI)
            {
                if (ai == null || ai.ControlledHero == null || !ai.ControlledHero.IsAlive) continue;
                EvaluateSituation(ai);
            }
        }

        private void EvaluateSituation(HeroAI ai)
        {
            var hero = ai.ControlledHero;
            if (hero.GetHealthPercent() < 0.15f)
            {
                ai.CurrentState = HeroAI.BehaviorState.BackToBase;
                return;
            }

            int allyCount = CountAlliesInRange(hero.transform.position, hero.TeamId, 15f);
            int enemyCount = CountEnemiesInRange(hero.transform.position, hero.TeamId, 15f);

            if (enemyCount > allyCount && hero.GetHealthPercent() < 0.5f)
            {
                ai.CurrentState = HeroAI.BehaviorState.Retreat;
            }
        }

        private int CountAlliesInRange(Vector3 pos, int teamId, float range)
        {
            int count = 0;
            foreach (var ai in _allAI)
            {
                if (ai == null || ai.ControlledHero == null || !ai.ControlledHero.IsAlive) continue;
                if (ai.ControlledHero.TeamId == teamId &&
                    Vector3.Distance(pos, ai.ControlledHero.transform.position) <= range)
                    count++;
            }
            return count;
        }

        private int CountEnemiesInRange(Vector3 pos, int teamId, float range)
        {
            int count = 0;
            foreach (var ai in _allAI)
            {
                if (ai == null || ai.ControlledHero == null || !ai.ControlledHero.IsAlive) continue;
                if (ai.ControlledHero.TeamId != teamId &&
                    Vector3.Distance(pos, ai.ControlledHero.transform.position) <= range)
                    count++;
            }
            return count;
        }

        public void SetDifficulty(AIDifficulty diff)
        {
            Difficulty = diff;
            foreach (var ai in _allAI)
            {
                if (ai == null) continue;
                switch (diff)
                {
                    case AIDifficulty.Easy:
                        ai.AlertRange = 8f;
                        ai.RetreatHealthPercent = 0.15f;
                        break;
                    case AIDifficulty.Normal:
                        ai.AlertRange = 12f;
                        ai.RetreatHealthPercent = 0.25f;
                        break;
                    case AIDifficulty.Hard:
                        ai.AlertRange = 18f;
                        ai.RetreatHealthPercent = 0.35f;
                        break;
                }
            }
        }
    }
}
