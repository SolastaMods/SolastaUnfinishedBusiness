using System.Collections;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;

//This should have default namespace so that it can be properly created by `CharacterActionPatcher`
// ReSharper disable once CheckNamespace
[UsedImplicitly]
#pragma warning disable CA1050
public class CharacterActionTraditionOpenHandQuiveringPalmToggle(CharacterActionParams actionParams)
    : CharacterAction(actionParams)
#pragma warning restore CA1050
{
    public override IEnumerator ExecuteImpl()
    {
        yield return ActingCharacter.RulesetCharacter.FlipToggle(ExtraActionId.QuiveringPalmToggle);
    }
}
