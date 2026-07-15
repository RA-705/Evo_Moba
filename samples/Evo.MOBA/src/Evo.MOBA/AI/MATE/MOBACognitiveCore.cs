using System.Collections.Generic;
using MateOS.Core;

namespace Evo.MOBA.AI.MATE;

public sealed class MOBACognitiveCore : CognitiveCore<MOBAPerception>
{
    private readonly GoapPlanner<MOBAPerception> _goapPlanner = new();
    private readonly UtilitySelector<MOBAPerception> _utilitySelector = new();
    private readonly MOBAMemory _memory = new();

    private List<GoapAction<MOBAPerception>>? _currentPlan;
    private int _planIndex;
    private float _thinkCooldown;
    private const float ThinkInterval = 0.3f;

    private MOBAPerception _lastPerception;
    private bool _perceptionDirty;
    private List<GoapGoal> _cachedGoals = new();
    private List<GoapAction<MOBAPerception>> _cachedActions = new();

    public MOBAMemory Memory => _memory;

    public MOBACognitiveCore()
    {
        SetupUtilityScorers();
    }

    private void SetupUtilityScorers()
    {
        var retreatAction = new ScoredAction<MOBAPerception>
        {
            Weight = 1.2f,
            ActionFactory = () => Intention<MOBAPerception>.Move(0, 0, 0),
        };
        retreatAction.AddScorer(new LowHpRetreatScorer());
        retreatAction.AddScorer(new DangerScorer());
        _utilitySelector.AddAction(retreatAction);

        var attackAction = new ScoredAction<MOBAPerception>
        {
            Weight = 1.0f,
            ActionFactory = () => Intention<MOBAPerception>.Stop(),
        };
        attackAction.AddScorer(new KillOpportunityScorer());
        attackAction.AddScorer(new HealthScorer());
        _utilitySelector.AddAction(attackAction);

        var castAction = new ScoredAction<MOBAPerception>
        {
            Weight = 0.9f,
            ActionFactory = () => Intention<MOBAPerception>.Stop(),
        };
        castAction.AddScorer(new KillOpportunityScorer());
        castAction.AddScorer(new ManaScorer());
        _utilitySelector.AddAction(castAction);

        var farmAction = new ScoredAction<MOBAPerception>
        {
            Weight = 0.7f,
            ActionFactory = () => Intention<MOBAPerception>.Stop(),
        };
        farmAction.AddScorer(new FarmValueScorer());
        _utilitySelector.AddAction(farmAction);

        var pushAction = new ScoredAction<MOBAPerception>
        {
            Weight = 0.6f,
            ActionFactory = () => Intention<MOBAPerception>.Move(0, 0, 0),
        };
        pushAction.AddScorer(new ObjectiveScorer());
        _utilitySelector.AddAction(pushAction);
    }

    protected override void ProcessPerception(MOBAPerception perception, float dt)
    {
        _lastPerception = perception;
        _perceptionDirty = true;
    }

    protected override void UpdateInternalMemory(float dt) { }

    protected override void GenerateDecisions()
    {
        _thinkCooldown -= 0.1f;
        if (_thinkCooldown > 0f) return;
        _thinkCooldown = ThinkInterval;

        if (_currentPlan != null && _planIndex < _currentPlan.Count)
        {
            var currentAction = _currentPlan[_planIndex];
            if (currentAction.CanExecute())
            {
                var intention = currentAction.ActionFactory();
                EnqueueIntention(intention);
                return;
            }
            _currentPlan = null;
        }

        if (_perceptionDirty)
        {
            MOBAGoals.PopulateGoals(_lastPerception, _memory, _cachedGoals);
            MOBAActions.PopulateActions(_lastPerception, _memory, _cachedActions);
            _perceptionDirty = false;
        }

        var plan = _goapPlanner.Plan(_cachedGoals, _cachedActions);
        if (plan != null && plan.Count > 0)
        {
            _currentPlan = plan;
            _planIndex = 0;
            var intention = _currentPlan[0].ActionFactory();
            EnqueueIntention(intention);
            return;
        }

        var fallback = _utilitySelector.SelectBestAction(World);
        EnqueueIntention(fallback);
    }

    public new void Think(MOBAPerception perception, float dt)
    {
        base.Think(perception, dt);
    }
}
