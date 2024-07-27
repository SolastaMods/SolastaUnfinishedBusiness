using System.Collections;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Subclasses;

//This should have default namespace so that it can be properly created by `CharacterActionPatcher`
// ReSharper disable once CheckNamespace
[UsedImplicitly]
#pragma warning disable CA1050
public class CharacterActionTidesOfChaosRecharge(CharacterActionParams actionParams) : CharacterAction(actionParams)
#pragma warning restore CA1050
{
    public override IEnumerator ExecuteImpl()
    {
        yield return SorcerousWildMagic.HandleRechargeTidesOfChaos(actionParams.ActingCharacter.RulesetCharacter);
    }
}
