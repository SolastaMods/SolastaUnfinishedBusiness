// Copyright < 2021 > Narria (github user Cabarius) - License: MIT

using System;

namespace SolastaUnfinishedBusiness.Api.ModKit;

public sealed class NamedAction
{
    public NamedAction(string name, Action action)
    {
        Name = name;
        Action = action;
    }

    public string Name { get; }
    public Action Action { get; }
}
