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

public class NamedMutator<TV, T>
{
    public NamedMutator(
        string name,
        Action<TV, T, int> action,
        Func<TV, T, bool> canPerform = null,
        bool isRepeatable = false
    )
    {
        Name = name;
        Action = action;
        CanPerform = canPerform ?? ((target, value) => true);
        IsRepeatable = isRepeatable;
    }

    public string Name { get; }
    public Action<TV, T, int> Action { get; }
    public Func<TV, T, bool> CanPerform { get; }
    public bool IsRepeatable { get; }
}
