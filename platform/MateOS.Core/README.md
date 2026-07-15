# MateOS.Core

Multi-Agent Tactical Engine — symbolic AI for autonomous game agents. Zero references to Unity or any game genre.

## Architecture

```
Perception → CognitiveCore<T>
               ├── CognitiveMemory (facts, beliefs)
               ├── GoapPlanner<T> (DFS goal satisfaction)
               └── UtilitySelector<T> (scored action selection)
                     └── Intention<T> (zero-boxing, tagged union)
```

## Components

| Component | Description |
|-----------|-------------|
| `CognitiveCore<T>` | Perceive → Memorize → Plan → Intend loop with queue-based yield |
| `GoapGoal` | Condition (`Func<CognitiveMemory, bool>`) + priority |
| `GoapAction<T>` | Name, cost, preconditions, effects, exec function |
| `GoapPlanner<T>` | DFS search (MaxDepth=10) for goal-satisfying action chain |
| `UtilitySelector<T>` | `ICognitiveScorer`-based priority ranking |
