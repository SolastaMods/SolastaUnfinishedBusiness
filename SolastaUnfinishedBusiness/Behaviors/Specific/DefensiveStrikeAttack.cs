using System.Collections;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Subclasses;
using static RuleDefinitions;
using static ActionDefinitions;

namespace SolastaUnfinishedBusiness.Behaviors.Specific;

//Old Attack of Opportunity before it became too narrow to use
internal static class DefensiveStrikeAttack
{
    internal static IEnumerator ProcessOnCharacterAttackFinished(
        GameLocationBattleManager battleManager,
        GameLocationCharacter attacker,
        GameLocationCharacter defender)
    {
        if (Gui.Battle == null)
        {
            yield break;
        }

        var units = Gui.Battle.AllContenders
            .Where(u => u.RulesetCharacter is { IsDeadOrUnconscious: false })
            .ToArray(); // avoid changing enumerator

        //Process other participants of the battle
        foreach (var unit in units
                     .Where(unit => attacker != unit && defender != unit))
        {
            yield return ActiveDefensiveStrike(unit, attacker, defender, battleManager);
        }
    }

    private static IEnumerator ActiveDefensiveStrike(
        [NotNull] GameLocationCharacter unit,
        [NotNull] GameLocationCharacter attacker,
        GameLocationCharacter defender,
        GameLocationBattleManager battleManager)
    {
        var unitCharacter = unit.RulesetCharacter;

        if (!attacker.IsOppositeSide(unit.Side) ||
            defender.IsOppositeSide(unit.Side) ||
            unit == defender ||
            !unitCharacter.HasSubFeatureOfType<DefensiveStrikeMarker>())
        {
            yield break;
        }

        //Is this unit able to react (not paralyzed, prone etc.)?
        if (!unit.CanReact(true))
        {
            yield break;
        }

        //Is defender able to use reaction (has reaction available and not paralyzed, prone etc.)?
        if (!defender.CanReact())
        {
            yield break;
        }

        //Can this unit see defender?
        if (!unit.CanPerceiveTarget(defender))
        {
            yield break;
        }

        //Does unit has enough Channel Divinity uses left?
        var maxUses = unitCharacter.TryGetAttributeValue(AttributeDefinitions.ChannelDivinityNumber);

        if (unitCharacter.UsedChannelDivinity >= maxUses)
        {
            yield break;
        }

        //Find defender's attack mode
        var (opportunityAttackMode, actionModifier) = defender.GetFirstMeleeModeThatCanAttack(attacker, battleManager);

        if (opportunityAttackMode == null || actionModifier == null)
        {
            yield break;
        }

        //Calculate bonus
        var charisma = unitCharacter.TryGetAttributeValue(AttributeDefinitions.Charisma);
        var bonus = AttributeDefinitions.ComputeAbilityScoreModifier(charisma);

        yield return unit.MyReactToDoNothing(
            ExtraActionId.DoNothingReaction,
            attacker,
            OathOfAltruism.DefensiveStrike,
            $"CustomReaction{OathOfAltruism.DefensiveStrike}Description"
                .Formatted(Category.Reaction, defender.Name, attacker.Name, bonus),
            ReactionValidated,
            battleManager: battleManager,
            resource: ReactionResourceChannelDivinity.Instance);

        yield break;

        void ReactionValidated()
        {
            //spend resources
            unitCharacter.UsedChannelDivinity++;

            //create attack mode copy so we won't affect real one
            var attackMode = RulesetAttackMode.AttackModesPool.Get();

            attackMode.Copy(opportunityAttackMode);

            //Apply bonus to hit and damage of the attack mode
            attackMode.EffectDescription.FindFirstDamageForm().BonusDamage += bonus;
            attackMode.ToHitBonus += bonus;
            attackMode.ToHitBonusTrends.Add(
                new TrendInfo(bonus, FeatureSourceType.CharacterFeature, OathOfAltruism.DefensiveStrike, unit));

            //Execute attack
            defender.MyExecuteActionAttack(Id.AttackOpportunity, attacker, attackMode, actionModifier);
        }
    }
}

internal sealed class DefensiveStrikeMarker
{
    private DefensiveStrikeMarker()
    {
    }

    public static DefensiveStrikeMarker Mark { get; } = new();
}
