using System.Collections;
using SolastaUnfinishedBusiness.Races;

internal class CharacterActionWildlingDash : CharacterAction
{
    public CharacterActionWildlingDash(CharacterActionParams actionParams) : base(actionParams)
    {
    }

    public override IEnumerator ExecuteImpl()
    {
        var rulesetCharacter = ActingCharacter.RulesetCharacter;

        if (!rulesetCharacter.HasConditionOfCategoryAndType(
                AttributeDefinitions.TagCombat, RaceWildlingBuilder.ConditionWildlingFeralDashName))
        {
            rulesetCharacter.InflictCondition(
                RaceWildlingBuilder.ConditionWildlingFeralDashName,
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
