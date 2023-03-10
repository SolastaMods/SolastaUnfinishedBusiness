// Copyright < 2021 > Narria (github user Cabarius) - License: MIT

using System;

namespace SolastaUnfinishedBusiness.Api.ModKit;

public class NamedAction
{
    public NamedAction(string name, Action action, Func<bool> canPerform = null)
    {
        Name = name;
        Action = action;
        CanPerform = canPerform ?? (() => true);
    }

    public string Name { get; }
    public Action Action { get; }
    public Func<bool> CanPerform { get; }
}

public class NamedAction<T>
{
    public NamedAction(string name, Action<T> action, Func<T, bool> canPerform = null)
    {
        Name = name;
        Action = action;
        CanPerform = canPerform ?? (T => true);
    }

    public string Name { get; }
    public Action<T> Action { get; }
    public Func<T, bool> CanPerform { get; }
}

public class NamedFunc<T>
{
    public NamedFunc(string name, Func<T> func, Func<bool> canPerform = null)
    {
        Name = name;
        Func = func;
        CanPerform = canPerform ?? (() => true);
    }

    public string Name { get; }

    private Func<T> Func { get; }

    private Func<bool> CanPerform { get; }
}

public class NamedMutator<Target, T>
{
    public NamedMutator(
        string name,
        Action<Target, T, int> action,
        Func<Target, T, bool> canPerform = null,
        bool isRepeatable = false
    )
    {
        this.name = name;
        this.action = action;
        this.canPerform = canPerform ?? ((target, value) => true);
        this.isRepeatable = isRepeatable;
    }

    public string name { get; }
    public Action<Target, T, int> action { get; }
    public Func<Target, T, bool> canPerform { get; }
    public bool isRepeatable { get; }
}
