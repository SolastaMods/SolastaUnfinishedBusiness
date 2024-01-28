using System.Collections;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Behaviors.Specific;

//This should have default namespace so that it can be properly created by `CharacterActionPatcher`
// ReSharper disable once CheckNamespace
[UsedImplicitly]
#pragma warning disable CA1050
public class CharacterActionPrioritizeAction(CharacterActionParams actionParams) : CharacterAction(actionParams)
#pragma warning restore CA1050
{
    public override IEnumerator ExecuteImpl()
    {
        ActionSwitching.DoPrioritizeAction(actionParams.ActingCharacter,
            (ActionDefinitions.ActionType)actionParams.IntParameter,
            actionParams.IntParameter2);

        yield break;
    }
}
