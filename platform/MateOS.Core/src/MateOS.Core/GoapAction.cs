using System;
using System.Collections.Generic;

namespace MateOS.Core;

public sealed class GoapAction<TPerception>
{
    public string Name { get; }
    public float Cost { get; }
    public List<Func<bool>> Preconditions { get; }
    public List<Action> Effects { get; }
    public Func<Intention<TPerception>> ActionFactory { get; set; }

    public GoapAction(string name, float cost = 1f)
    {
        Name = name;
        Cost = cost;
        Preconditions = new List<Func<bool>>();
        Effects = new List<Action>();
        ActionFactory = () => Intention<TPerception>.Stop();
    }

    public bool CanExecute() => Preconditions.TrueForAll(p => p());
    public void ApplyEffects() => Effects.ForEach(e => e());

    public static GoapAction<TPerception> Create(
        string name, float cost,
        Func<bool> precondition,
        Func<Intention<TPerception>> factory)
    {
        var action = new GoapAction<TPerception>(name, cost);
        action.Preconditions.Add(precondition);
        action.ActionFactory = factory;
        return action;
    }
}
