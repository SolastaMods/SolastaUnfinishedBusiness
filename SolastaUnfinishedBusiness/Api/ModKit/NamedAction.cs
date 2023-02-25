// Copyright < 2021 > Narria (github user Cabarius) - License: MIT

using System;

namespace SolastaUnfinishedBusiness.Api.ModKit;

public class NamedAction
{
    public NamedAction(string name, Action action, Func<bool> canPerform = null)
    {
        Name = name;
        Action = action;
        CanPerform = canPerform ?? (() => { return true; });
    }

    public string Name { get; }
    public Action Action { get; }
    public Func<bool> CanPerform { get; }
}

public class NamedAction<T>
{
    public NamedAction(string name, Action<T> action, Func<T, bool> canPerform = null)
    {
        this.name = name;
        this.action = action;
        this.canPerform = canPerform ?? (T => { return true; });
    }

    public string name { get; }
    public Action<T> action { get; }
    public Func<T, bool> canPerform { get; }
}

public class NamedFunc<T>
{
    public NamedFunc(string name, Func<T> func, Func<bool> canPerform = null)
    {
        this.name = name;
        this.func = func;
        this.canPerform = canPerform ?? (() => { return true; });
    }

    public string name { get; }
    public Func<T> func { get; }
    public Func<bool> canPerform { get; }
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
