// Copyright < 2021 > Narria (github user Cabarius) - License: MIT

using System;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Api.ModKit;

public class NamedAction(string name, Action action, Func<bool> canPerform = null)
{
    [UsedImplicitly] public string Name { [UsedImplicitly] get; } = name;
    [UsedImplicitly] public Action Action { [UsedImplicitly] get; } = action;
    [UsedImplicitly] public Func<bool> CanPerform { [UsedImplicitly] get; } = canPerform ?? (() => true);
}

public class NamedAction<T>(string name, Action<T> action, Func<T, bool> canPerform = null)
{
    [UsedImplicitly] public string Name { [UsedImplicitly] get; } = name;
    [UsedImplicitly] public Action<T> Action { [UsedImplicitly] get; } = action;
    [UsedImplicitly] public Func<T, bool> CanPerform { [UsedImplicitly] get; } = canPerform ?? (_ => true);
}

public class NamedFunc<T>(string name, Func<T> func, Func<bool> canPerform = null)
{
    [UsedImplicitly] public string Name { [UsedImplicitly] get; } = name;
    [UsedImplicitly] public Func<T> Func { [UsedImplicitly] get; } = func;
    [UsedImplicitly] public Func<bool> CanPerform { [UsedImplicitly] get; } = canPerform ?? (() => true);
}

public class NamedMutator<TV, T>(
    string name,
    Action<TV, T, int> action,
    Func<TV, T, bool> canPerform = null,
    bool isRepeatable = false)
{
    [UsedImplicitly] public string Name { [UsedImplicitly] get; } = name;
    [UsedImplicitly] public Action<TV, T, int> Action { [UsedImplicitly] get; } = action;
    [UsedImplicitly] public Func<TV, T, bool> CanPerform { [UsedImplicitly] get; } = canPerform ?? ((_, _) => true);
    [UsedImplicitly] public bool IsRepeatable { [UsedImplicitly] get; } = isRepeatable;
}
