using System.Collections;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Races;

//This should have default namespace so that it can be properly created by `CharacterActionPatcher`
// ReSharper disable once CheckNamespace
[UsedImplicitly]
#pragma warning disable CA1050
public class CharacterActionCrystalDefenseOn : CharacterAction
#pragma warning restore CA1050
{
    public CharacterActionCrystalDefenseOn(CharacterActionParams actionParams) : base(actionParams)
    {
    }

    public override IEnumerator ExecuteImpl()
    {
        var rulesetCharacter = ActingCharacter.RulesetCharacter;

        if (rulesetCharacter.HasConditionOfCategoryAndType(
                AttributeDefinitions.TagStatus, RaceWyrmkinBuilder.ConditionCrystalDefenseName))
        {
            yield break;
        }

        rulesetCharacter.InflictCondition(
            RaceWyrmkinBuilder.ConditionCrystalDefenseName,
            RuleDefinitions.DurationType.Irrelevant,
            0,
            RuleDefinitions.TurnOccurenceType.StartOfTurn,
            AttributeDefinitions.TagStatus,
            rulesetCharacter.Guid,
            rulesetCharacter.CurrentFaction.Name,
            0,
            string.Empty,
            0,
            0,
            0);

        if (ActingCharacter.SetProne(true))
        {
            yield return ActingCharacter.EventSystem.WaitForEvent(
                GameLocationCharacterEventSystem.Event.ProneInAnimationEnd);
        }
    }
}
