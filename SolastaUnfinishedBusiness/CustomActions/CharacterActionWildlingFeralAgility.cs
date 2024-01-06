using System.Collections;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Races;

//This should have default namespace so that it can be properly created by `CharacterActionPatcher`
// ReSharper disable once CheckNamespace
[UsedImplicitly]
#pragma warning disable CA1050
internal class CharacterActionWildlingFeralAgility : CharacterAction
#pragma warning restore CA1050
{
    public CharacterActionWildlingFeralAgility(CharacterActionParams actionParams) : base(actionParams)
    {
    }

    public override IEnumerator ExecuteImpl()
    {
        var rulesetCharacter = ActingCharacter.RulesetCharacter;

        if (rulesetCharacter.HasConditionOfType(RaceWildlingBuilder.ConditionWildlingAgileName))
        {
            yield break;
        }

        rulesetCharacter.InflictCondition(
            RaceWildlingBuilder.ConditionWildlingAgileName,
            RuleDefinitions.DurationType.Round,
            0,
            RuleDefinitions.TurnOccurenceType.EndOfTurn,
            AttributeDefinitions.TagCombat,
            rulesetCharacter.Guid,
            rulesetCharacter.CurrentFaction.Name,
            1,
            RaceWildlingBuilder.ConditionWildlingAgileName,
            0,
            0,
            0);

        rulesetCharacter.InflictCondition(
            RaceWildlingBuilder.ConditionWildlingTiredName,
            RuleDefinitions.DurationType.Irrelevant,
            0,
            RuleDefinitions.TurnOccurenceType.EndOfTurn,
            AttributeDefinitions.TagCombat,
            rulesetCharacter.Guid,
            rulesetCharacter.CurrentFaction.Name,
            1,
            RaceWildlingBuilder.ConditionWildlingTiredName,
            0,
            0,
            0);

        yield return null;
    }
}
