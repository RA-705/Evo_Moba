using System.Collections.Generic;

namespace MateOS.Core;

public enum IntentionKind : byte
{
    Move,
    Attack,
    Stop,
}

public readonly record struct IntentionMove(float X, float Y, float Z);

public readonly record struct IntentionAttack(int TargetId);

public readonly record struct IntentionStop;

public readonly struct Intention<TPerception>
{
    private readonly IntentionMove _moveData;
    private readonly IntentionAttack _attackData;

    public IntentionKind Kind { get; }

    private Intention(IntentionKind kind, IntentionMove data)
    {
        Kind = kind;
        _moveData = data;
        _attackData = default;
    }

    private Intention(IntentionKind kind, IntentionAttack data)
    {
        Kind = kind;
        _moveData = default;
        _attackData = data;
    }

    private Intention(IntentionKind kind, IntentionStop data)
    {
        Kind = kind;
        _moveData = default;
        _attackData = default;
    }

    public static Intention<TPerception> Move(float x, float y, float z) =>
        new(IntentionKind.Move, new IntentionMove(x, y, z));

    public static Intention<TPerception> Attack(int targetId) =>
        new(IntentionKind.Attack, new IntentionAttack(targetId));

    public static Intention<TPerception> Stop() =>
        new(IntentionKind.Stop, new IntentionStop());

    public IntentionMove GetMoveData()
    {
        if (Kind != IntentionKind.Move)
            throw new InvalidOperationException($"Intention is {Kind}, not Move.");

        return _moveData;
    }

    public IntentionAttack GetAttackData()
    {
        if (Kind != IntentionKind.Attack)
            throw new InvalidOperationException($"Intention is {Kind}, not Attack.");

        return _attackData;
    }

    public bool IsStop() => Kind == IntentionKind.Stop;
}

public abstract class CognitiveCore<TPerception>
{
    private readonly Queue<Intention<TPerception>> _intentQueue = new();

    public WorldModel<TPerception> World { get; } = new();

    public void Think(TPerception perception, float dt)
    {
        ProcessPerception(perception, dt);
        World.Update(perception);
        UpdateInternalMemory(dt);
        GenerateDecisions();
    }

    public IEnumerable<Intention<TPerception>> ExecuteIntentions()
    {
        while (_intentQueue.Count > 0)
            yield return _intentQueue.Dequeue();
    }

    protected void EnqueueIntention(Intention<TPerception> intention) =>
        _intentQueue.Enqueue(intention);

    protected abstract void ProcessPerception(TPerception perception, float dt);

    protected abstract void UpdateInternalMemory(float dt);

    protected abstract void GenerateDecisions();
}
