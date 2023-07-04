using System.Collections;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomInterfaces;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionCombatAffinitys;

namespace SolastaUnfinishedBusiness.Models;

/**
 * Place for generic conditions that may be reused between several features
 */
internal static class CustomConditionsContext
{
    internal static ConditionDefinition Distracted;

    internal static ConditionDefinition InvisibilityEveryRound;

    internal static ConditionDefinition LightSensitivity;

    internal static ConditionDefinition StopMovement;

    private static ConditionDefinition ConditionInvisibilityEveryRoundRevealed { get; set; }

    private static ConditionDefinition ConditionInvisibilityEveryRoundHidden { get; set; }

    internal static void Load()
    {
        StopMovement = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionRestrained, "ConditionStopMovement")
            .SetOrUpdateGuiPresentation(Category.Condition)
            .SetFeatures(
                FeatureDefinitionMovementAffinitys.MovementAffinityConditionRestrained,
                FeatureDefinitionActionAffinitys.ActionAffinityConditionRestrained)
            .AddToDB();

        InvisibilityEveryRound = BuildInvisibilityEveryRound();

        LightSensitivity = BuildLightSensitivity();

        Distracted = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionTrueStrike, "ConditionDistractedByAlly")
            .SetOrUpdateGuiPresentation(Category.Condition)
            .SetSpecialDuration(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
            .SetSpecialInterruptions(ConditionInterruption.Attacked)
            .SetFeatures(
                FeatureDefinitionCombatAffinityBuilder
                    .Create("CombatAffinityDistractedByAlly")
                    .SetGuiPresentation("ConditionDistractedByAlly", Category.Condition)
                    .SetAttackOnMeAdvantage(AdvantageType.Advantage)
                    .AddToDB())
            .AddToDB();
    }

    private static ConditionDefinition BuildInvisibilityEveryRound()
    {
        ConditionInvisibilityEveryRoundRevealed = ConditionDefinitionBuilder
            .Create("ConditionInvisibilityEveryRoundRevealed")
            .SetGuiPresentationNoContent()
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddToDB();

        ConditionInvisibilityEveryRoundHidden = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionInvisible, "ConditionInvisibilityEveryRoundHidden")
            .SetCancellingConditions(ConditionInvisibilityEveryRoundRevealed)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetSpecialDuration(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
            .SetSpecialInterruptions(
                ConditionInterruption.Attacks,
                ConditionInterruption.CastSpell,
                ConditionInterruption.UsePower)
            .AddToDB();

        var conditionInvisibilityEveryRound = ConditionDefinitionBuilder
            .Create("ConditionInvisibilityEveryRound")
            .SetGuiPresentationNoContent()
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetSpecialDuration(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
            .SetFeatures(FeatureDefinitionBuilder
                .Create("FeatureInvisibilityEveryRound")
                .SetGuiPresentationNoContent()
                .SetCustomSubFeatures(new InvisibilityEveryRoundBehavior())
                .AddToDB())
            .AddToDB();

        return conditionInvisibilityEveryRound;
    }

    private static ConditionDefinition BuildLightSensitivity()
    {
        var abilityCheckAffinityLightSensitivity = FeatureDefinitionAbilityCheckAffinityBuilder
            .Create("AbilityCheckAffinityLightSensitivity")
            .SetGuiPresentation(CombatAffinitySensitiveToLight.GuiPresentation)
            .BuildAndSetAffinityGroups(CharacterAbilityCheckAffinity.Disadvantage, DieType.D1, 0,
                (AttributeDefinitions.Wisdom, SkillDefinitions.Perception))
            .AddToDB();

        abilityCheckAffinityLightSensitivity.AffinityGroups[0].lightingContext = LightingContext.BrightLight;

        var combatAffinityDarkelfLightSensitivity = FeatureDefinitionCombatAffinityBuilder
            .Create(CombatAffinitySensitiveToLight, "CombatAffinityLightSensitivity")
            .SetMyAttackAdvantage(AdvantageType.None)
            .SetMyAttackModifierSign(AttackModifierSign.Substract)
            .SetMyAttackModifierDieType(DieType.D4)
            .AddToDB();

        var conditionLightSensitive = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionLightSensitive, "ConditionLightSensitivity")
            .SetOrUpdateGuiPresentation(Category.Condition)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetPossessive()
            .SetConditionType(ConditionType.Detrimental)
            .SetFeatures(abilityCheckAffinityLightSensitivity, combatAffinityDarkelfLightSensitivity)
            .AddToDB();

        return conditionLightSensitive;
    }

    private sealed class InvisibilityEveryRoundBehavior : IActionFinishedByMe, ICustomConditionFeature
    {
        private const string CategoryRevealed = "InvisibilityEveryRoundRevealed";
        private const string CategoryHidden = "InvisibilityEveryRoundHidden";

        public IEnumerator OnActionFinishedByMe(CharacterAction action)
        {
            if (action is not (CharacterActionUsePower or CharacterActionCastSpell or CharacterActionAttack))
            {
                yield break;
            }

            var actingCharacter = action.ActingCharacter;
            var rulesetCharacter = actingCharacter.RulesetCharacter;
            var actionParams = action.ActionParams;
            var rulesetEffect = actionParams.RulesetEffect;

            if (rulesetEffect == null || !IsAllowedEffect(rulesetEffect.EffectDescription))
            {
                BecomeRevealed(rulesetCharacter);
            }
        }

        public void ApplyFeature(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            if (target is not RulesetCharacterMonster &&
                !target.HasConditionOfType(ConditionInvisibilityEveryRoundRevealed))
            {
                BecomeHidden(target);
            }
        }

        public void RemoveFeature(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            if (target is not RulesetCharacterMonster)
            {
                target.RemoveAllConditionsOfCategory(CategoryHidden, false);
            }
        }

        // returns true if effect is self teleport or any self targeting spell that is self-buff
        private static bool IsAllowedEffect(EffectDescription effect)
        {
            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (effect.TargetType)
            {
                case TargetType.Position:
                {
                    foreach (var form in effect.EffectForms)
                    {
                        if (form.FormType != EffectForm.EffectFormType.Motion)
                        {
                            return false;
                        }

                        if (form.MotionForm.Type != MotionForm.MotionType.TeleportToDestination)
                        {
                            return false;
                        }
                    }

                    break;
                }
                case TargetType.Self:
                {
                    foreach (var form in effect.EffectForms)
                    {
                        // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
                        switch (form.FormType)
                        {
                            case EffectForm.EffectFormType.Damage:
                            case EffectForm.EffectFormType.Healing:
                            case EffectForm.EffectFormType.ShapeChange:
                            case EffectForm.EffectFormType.Summon:
                            case EffectForm.EffectFormType.Counter:
                            case EffectForm.EffectFormType.Motion:
                                return false;
                        }
                    }

                    break;
                }
                default:
                    return false;
            }

            return true;
        }

        private static void BecomeRevealed(RulesetCharacter hero)
        {
            hero.AddConditionOfCategory(CategoryRevealed,
                RulesetCondition.CreateActiveCondition(
                    hero.Guid,
                    ConditionInvisibilityEveryRoundRevealed,
                    DurationType.Round,
                    1,
                    TurnOccurenceType.StartOfTurn,
                    hero.Guid,
                    hero.CurrentFaction.Name
                ));
        }

        private static void BecomeHidden(RulesetCharacter hero)
        {
            hero.AddConditionOfCategory(CategoryHidden,
                RulesetCondition.CreateActiveCondition(
                    hero.Guid,
                    ConditionInvisibilityEveryRoundHidden,
                    DurationType.Permanent,
                    0,
                    TurnOccurenceType.EndOfTurn,
                    hero.Guid,
                    hero.CurrentFaction.Name),
                false);
        }
    }
}
