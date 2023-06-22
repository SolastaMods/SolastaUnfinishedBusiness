using System.Collections;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.CustomBehaviors;

//This should have default namespace so that it can be properly created by `CharacterActionPatcher`
// ReSharper disable once CheckNamespace
[UsedImplicitly]
#pragma warning disable CA1050
public class CharacterActionPrioritizeAction : CharacterAction
#pragma warning restore CA1050
{
    public CharacterActionPrioritizeAction(CharacterActionParams actionParams) : base(actionParams)
    {
    }

    public override IEnumerator ExecuteImpl()
    {
        ActionSwitching.DoPrioritizeAction(actionParams.ActingCharacter,
            (ActionDefinitions.ActionType)actionParams.IntParameter,
            actionParams.IntParameter2);
        yield break;
    }
}
