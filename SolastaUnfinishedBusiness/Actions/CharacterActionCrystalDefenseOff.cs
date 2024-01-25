using System.Collections;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Races;

//This should have default namespace so that it can be properly created by `CharacterActionPatcher`
// ReSharper disable once CheckNamespace
[UsedImplicitly]
#pragma warning disable CA1050
public class CharacterActionCrystalDefenseOff(CharacterActionParams actionParams) : CharacterAction(actionParams)
#pragma warning restore CA1050
{
    public override IEnumerator ExecuteImpl()
    {
        var rulesetCharacter = ActingCharacter.RulesetCharacter;

        rulesetCharacter.RemoveAllConditionsOfCategoryAndType(
            AttributeDefinitions.TagStatus, RaceWyrmkinBuilder.ConditionCrystalDefenseName);

        yield return CharacterActionStandUp.StandUp(ActingCharacter, false, true);
    }
}
