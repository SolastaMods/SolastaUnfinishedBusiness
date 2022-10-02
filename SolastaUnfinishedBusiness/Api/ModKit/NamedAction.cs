// Copyright < 2021 > Narria (github user Cabarius) - License: MIT

using System;

namespace SolastaUnfinishedBusiness.Api.ModKit;

internal sealed class NamedAction
{
    internal NamedAction(string name, Action action)
    {
        Name = name;
        Action = action;
    }

    internal string Name { get; }
    internal Action Action { get; }
}
