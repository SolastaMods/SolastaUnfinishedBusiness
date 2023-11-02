using System.Collections;
using SolastaUnfinishedBusiness.Races;

internal class CharacterActionWildlingFeralAgility : CharacterAction
{
    public CharacterActionWildlingFeralAgility(CharacterActionParams actionParams) : base(actionParams)
    {
    }

    public override IEnumerator ExecuteImpl()
    {
        var rulesetCharacter = ActingCharacter.RulesetCharacter;

        if (!rulesetCharacter.HasConditionOfCategoryAndType(
                AttributeDefinitions.TagCombat, RaceWildlingBuilder.ConditionWildlingAgileName))
        {
            rulesetCharacter.InflictCondition(
                RaceWildlingBuilder.ConditionWildlingAgileName,
                RuleDefinitions.DurationType.Round,
                0,
                RuleDefinitions.TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagCombat,
                rulesetCharacter.Guid,
                rulesetCharacter.CurrentFaction.Name,
                0,
                string.Empty,
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
                0,
                string.Empty,
                0,
                0,
                0);
            yield return null;
        }
    }
}
