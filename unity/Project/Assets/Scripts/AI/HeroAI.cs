using UnityEngine;
using Evo.AI.SimpleBehaviorTree;
using Evo.Core;

namespace Evo.AI
{
    public class HeroAI : MonoBehaviour
    {
        public enum BehaviorState
        {
            Idle,
            MoveToLane,
            Patrol,
            SeekEnemy,
            Chase,
            Attack,
            Retreat,
            BackToBase,
            Dead
        }

        [Header("AI Settings")]
        public float AlertRange = 10f;
        public float AttackRangeBuffer = 1f;
        public float RetreatHealthPercent = 0.2f;
        public float PatrolRadius = 8f;
        public float ChaseMaxRange = 20f;

        [Header("State")]
        public BehaviorState CurrentState = BehaviorState.Idle;
        public HeroController ControlledHero;
        public HeroController CurrentTarget;

        private BTTree _behaviorTree;
        private float _stateTimer;
        private Vector3 _homePosition;
        private Vector3 _patrolTarget;
        private int _laneDirection = 1;

        public void Initialize(HeroController hero)
        {
            ControlledHero = hero;
            _homePosition = hero.transform.position;

            _behaviorTree = new AIBattleTree(this);
            _behaviorTree.BuildTree();

            CurrentState = BehaviorState.MoveToLane;
        }

        private void Update()
        {
            if (ControlledHero == null) return;

            if (!ControlledHero.IsAlive)
            {
                CurrentState = BehaviorState.Dead;
                return;
            }

            if (CurrentState == BehaviorState.Dead)
                CurrentState = BehaviorState.MoveToLane;

            _stateTimer += Time.deltaTime;

            if (_behaviorTree != null)
                _behaviorTree.Execute();

            ExecuteState();
        }

        private void ExecuteState()
        {
            switch (CurrentState)
            {
                case BehaviorState.Idle:
                    ControlledHero.StopMoving();
                    ControlledHero.StopAttack();
                    break;

                case BehaviorState.MoveToLane:
                    Vector3 lanePos = _homePosition + new Vector3(_laneDirection * 15f, 0, 0);
                    ControlledHero.MoveTo(lanePos);
                    if (Vector3.Distance(ControlledHero.transform.position, lanePos) < 2f)
                        CurrentState = BehaviorState.Patrol;
                    SeekEnemyInRange();
                    break;

                case BehaviorState.Patrol:
                    if (_stateTimer > 3f || Vector3.Distance(ControlledHero.transform.position, _patrolTarget) < 1f)
                    {
                        _patrolTarget = _homePosition + new Vector3(
                            Random.Range(-PatrolRadius, PatrolRadius),
                            0,
                            Random.Range(-PatrolRadius, PatrolRadius));
                        _stateTimer = 0f;
                    }
                    ControlledHero.MoveTo(_patrolTarget);
                    SeekEnemyInRange();
                    break;

                case BehaviorState.SeekEnemy:
                    var enemy = FindNearestEnemy();
                    if (enemy != null)
                    {
                        CurrentTarget = enemy;
                        CurrentState = BehaviorState.Chase;
                    }
                    else
                    {
                        CurrentState = BehaviorState.Patrol;
                    }
                    break;

                case BehaviorState.Chase:
                    if (CurrentTarget == null || !CurrentTarget.IsAlive)
                    {
                        CurrentState = BehaviorState.SeekEnemy;
                        return;
                    }
                    float dist = Vector3.Distance(ControlledHero.transform.position, CurrentTarget.transform.position);
                    if (dist > ChaseMaxRange)
                    {
                        CurrentState = BehaviorState.Patrol;
                        return;
                    }
                    if (dist <= ControlledHero.AttackRange + AttackRangeBuffer)
                    {
                        CurrentState = BehaviorState.Attack;
                        return;
                    }
                    ControlledHero.MoveTo(CurrentTarget.transform.position);
                    break;

                case BehaviorState.Attack:
                    if (CurrentTarget == null || !CurrentTarget.IsAlive)
                    {
                        ControlledHero.StopAttack();
                        CurrentState = BehaviorState.SeekEnemy;
                        return;
                    }
                    if (!ControlledHero.IsInAttackRange(CurrentTarget))
                    {
                        CurrentState = BehaviorState.Chase;
                        return;
                    }
                    ControlledHero.Attack(CurrentTarget);
                    if (ControlledHero.GetHealthPercent() < RetreatHealthPercent)
                        CurrentState = BehaviorState.Retreat;
                    break;

                case BehaviorState.Retreat:
                    ControlledHero.StopAttack();
                    Vector3 retreatPos = _homePosition + new Vector3(0, 0, _laneDirection * 5f);
                    ControlledHero.MoveTo(retreatPos);
                    if (Vector3.Distance(ControlledHero.transform.position, _homePosition) < 2f ||
                        ControlledHero.GetHealthPercent() > 0.7f)
                        CurrentState = BehaviorState.Patrol;
                    break;

                case BehaviorState.BackToBase:
                    ControlledHero.StopAttack();
                    ControlledHero.MoveTo(_homePosition);
                    if (Vector3.Distance(ControlledHero.transform.position, _homePosition) < 2f)
                    {
                        ControlledHero.CurrentHealth = ControlledHero.MaxHealth;
                        ControlledHero.CurrentMana = ControlledHero.MaxMana;
                        CurrentState = BehaviorState.Patrol;
                    }
                    break;
            }
        }

        private void SeekEnemyInRange()
        {
            var enemy = FindNearestEnemy();
            if (enemy != null)
            {
                CurrentTarget = enemy;
                CurrentState = BehaviorState.Chase;
            }
        }

        public HeroController FindNearestEnemy()
        {
            if (MatchSceneManager.Instance == null) return null;
            return MatchSceneManager.Instance.GetNearestEnemy(
                ControlledHero.transform.position, ControlledHero.TeamId, AlertRange);
        }

        private void OnDrawGizmosSelected()
        {
            if (ControlledHero == null) return;
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(ControlledHero.transform.position, AlertRange);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(ControlledHero.transform.position, ControlledHero.AttackRange);
        }
    }

    public class AIBattleTree : BTTree
    {
        private HeroAI _ai;

        public AIBattleTree(HeroAI ai) { _ai = ai; }

        public override void BuildTree()
        {
            Root = new BTSelectNode(
                new BTSequenceNode(
                    new BTConditionNode(() => _ai.ControlledHero == null || !_ai.ControlledHero.IsAlive),
                    new BTActionNode(() => { _ai.CurrentState = HeroAI.BehaviorState.Dead; return BTResult.Success; })
                ),
                new BTSequenceNode(
                    new BTConditionNode(() => _ai.ControlledHero.GetHealthPercent() < _ai.RetreatHealthPercent),
                    new BTActionNode(() => { _ai.CurrentState = HeroAI.BehaviorState.Retreat; return BTResult.Running; })
                ),
                new BTSequenceNode(
                    new BTConditionNode(() => _ai.CurrentTarget != null && _ai.CurrentTarget.IsAlive &&
                        Vector3.Distance(_ai.ControlledHero.transform.position, _ai.CurrentTarget.transform.position) <= _ai.ControlledHero.AttackRange + _ai.AttackRangeBuffer),
                    new BTActionNode(() => { _ai.CurrentState = HeroAI.BehaviorState.Attack; return BTResult.Running; })
                ),
                new BTSequenceNode(
                    new BTConditionNode(() => _ai.CurrentTarget != null && _ai.CurrentTarget.IsAlive),
                    new BTActionNode(() => { _ai.CurrentState = HeroAI.BehaviorState.Chase; return BTResult.Running; })
                ),
                new BTSequenceNode(
                    new BTConditionNode(() => _ai.CurrentState == HeroAI.BehaviorState.Idle || _ai.CurrentState == HeroAI.BehaviorState.Patrol),
                    new BTActionNode(() =>
                    {
                        var enemy = _ai.FindNearestEnemy();
                        if (enemy != null) { _ai.CurrentTarget = enemy; _ai.CurrentState = HeroAI.BehaviorState.Chase; }
                        return BTResult.Success;
                    })
                ),
                new BTActionNode(() => { _ai.CurrentState = HeroAI.BehaviorState.Patrol; return BTResult.Running; })
            );
        }
    }
}
