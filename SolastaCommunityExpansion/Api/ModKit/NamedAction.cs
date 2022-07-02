// Copyright < 2021 > Narria (github user Cabarius) - License: MIT

using System;

namespace ModKit;

public class NamedAction
{
    public NamedAction(string name, Action action)
    {
        this.name = name;
        this.action = action;
    }

    public string name { get; }
    public Action action { get; }
}
