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

    internal static void Load()
    {
        StopMovement = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionRestrained, "ConditionStopMovement")
            .SetOrUpdateGuiPresentation(Category.Condition)
            .SetFeatures(
                FeatureDefinitionMovementAffinitys.MovementAffinityConditionRestrained,
                FeatureDefinitionActionAffinitys.ActionAffinityConditionRestrained)
            .AddToDB();

        InvisibilityEveryRound = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionInvisible, "ConditionInvisibilityEveryRound")
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetSpecialDuration(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
            .SetFeatures(FeatureDefinitionBuilder
                .Create("FeatureInvisibilityEveryRound")
                .SetGuiPresentationNoContent()
                .SetCustomSubFeatures(new InvisibilityEveryRoundBehavior())
                .AddToDB())
            .AddToDB();

        InvisibilityEveryRound.SpecialInterruptions.Clear();

        LightSensitivity = BuildLightSensitivity();

        Distracted = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionTrueStrike, "ConditionDistractedByAlly")
            .SetOrUpdateGuiPresentation(Category.Condition)
            .AddToDB();
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

    private sealed class InvisibilityEveryRoundBehavior : IActionFinished
    {
        public IEnumerator OnActionFinished(CharacterAction action)
        {
            if (action is not (CharacterActionUsePower or CharacterActionCastSpell or CharacterActionAttack))
            {
                yield break;
            }

            var rulesetCharacter = action.ActingCharacter.RulesetCharacter;
            var actionParams = action.ActionParams;

            var rulesetEffect = actionParams.RulesetEffect;

            if (rulesetEffect == null || !IsAllowedEffect(rulesetEffect.EffectDescription))
            {
                rulesetCharacter.RemoveAllConditionsOfCategory("ConditionInvisibilityEveryRound");
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
    }
}
