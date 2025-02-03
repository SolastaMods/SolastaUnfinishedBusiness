﻿using System.Collections.Generic;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Interfaces;

public interface IPreventEnemySenseMode
{
    [UsedImplicitly]
    public List<SenseMode.Type> PreventedSenseModes(GameLocationCharacter attacker, RulesetCharacter defender);
}

public interface IAddAttackerSenseMode
{
    [UsedImplicitly]
    public List<SenseMode> AddedSenseModes(GameLocationCharacter attacker, RulesetCharacter defender);
}
