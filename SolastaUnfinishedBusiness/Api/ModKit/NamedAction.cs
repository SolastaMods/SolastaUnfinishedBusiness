// Copyright < 2021 > Narria (github user Cabarius) - License: MIT

using System;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Api.ModKit;

public class NamedAction
{
    public NamedAction(string name, Action action, Func<bool> canPerform = null)
    {
        Name = name;
        Action = action;
        CanPerform = canPerform ?? (() => true);
    }

    [UsedImplicitly] public string Name { [UsedImplicitly] get; }
    [UsedImplicitly] public Action Action { [UsedImplicitly] get; }
    [UsedImplicitly] public Func<bool> CanPerform { [UsedImplicitly] get; }
}

public class NamedAction<T>
{
    public NamedAction(string name, Action<T> action, Func<T, bool> canPerform = null)
    {
        Name = name;
        Action = action;
        CanPerform = canPerform ?? (_ => true);
    }

    [UsedImplicitly] public string Name { [UsedImplicitly] get; }
    [UsedImplicitly] public Action<T> Action { [UsedImplicitly] get; }
    [UsedImplicitly] public Func<T, bool> CanPerform { [UsedImplicitly] get; }
}

public class NamedFunc<T>
{
    public NamedFunc(string name, Func<T> func, Func<bool> canPerform = null)
    {
        Name = name;
        Func = func;
        CanPerform = canPerform ?? (() => true);
    }

    [UsedImplicitly] public string Name { [UsedImplicitly] get; }
    [UsedImplicitly] public Func<T> Func { [UsedImplicitly] get; }
    [UsedImplicitly] public Func<bool> CanPerform { [UsedImplicitly] get; }
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
        CanPerform = canPerform ?? ((_, _) => true);
        IsRepeatable = isRepeatable;
    }

    [UsedImplicitly] public string Name { [UsedImplicitly] get; }
    [UsedImplicitly] public Action<TV, T, int> Action { [UsedImplicitly] get; }
    [UsedImplicitly] public Func<TV, T, bool> CanPerform { [UsedImplicitly] get; }
    [UsedImplicitly] public bool IsRepeatable { [UsedImplicitly] get; }
}
