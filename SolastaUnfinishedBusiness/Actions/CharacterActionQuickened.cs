using System.Collections;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using static RuleDefinitions;

//This should have default namespace so that it can be properly created by `CharacterActionPatcher`
// ReSharper disable once CheckNamespace
[UsedImplicitly]
#pragma warning disable CA1050
public class CharacterActionQuickened(CharacterActionParams actionParams) : CharacterAction(actionParams)
#pragma warning restore CA1050
{
    public override IEnumerator ExecuteImpl()
    {
        var rulesetCharacter = ActingCharacter.RulesetCharacter;
        var hero = rulesetCharacter.GetOriginalHero();

        if (hero == null)
        {
            yield break;
        }

        ActingCharacter.SpendActionType(ActionDefinitions.ActionType.Bonus);
        hero.SpendSorceryPoints(2);
        hero.InflictCondition(
            "ConditionSorcererQuickenedCastMain",
            DurationType.Round,
            0,
            TurnOccurenceType.EndOfTurn,
            AttributeDefinitions.TagEffect,
            hero.guid,
            hero.CurrentFaction.Name,
            1,
            "ConditionSorcererQuickenedCastMain",
            0,
            0,
            0);
    }
}
