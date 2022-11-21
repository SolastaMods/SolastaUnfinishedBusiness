using System.Collections.Generic;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomInterfaces;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFightingStyleChoices;

namespace SolastaUnfinishedBusiness.FightingStyles;

internal sealed class Executioner : AbstractFightingStyle
{
    private const string ExecutionerName = "Executioner";

    internal override FightingStyleDefinition FightingStyle { get; } = FightingStyleBuilder
        .Create(ExecutionerName)
        .SetGuiPresentation(Category.FightingStyle, PathMagebane)
        .SetFeatures(
            FeatureDefinitionBuilder
                .Create("OnComputeAttackModifierFightingStyleExecutioner")
                .SetGuiPresentationNoContent(true)
                .SetCustomSubFeatures(new OnAttackDamageEffectFightingStyleExecutioner())
                .AddToDB())
        .AddToDB();

    internal override List<FeatureDefinitionFightingStyleChoice> FightingStyleChoice => new()
    {
        FightingStyleChampionAdditional, FightingStyleFighter, FightingStylePaladin, FightingStyleRanger
    };

    private sealed class OnAttackDamageEffectFightingStyleExecutioner : IBeforeAttackEffect
    {
        public void BeforeOnAttackHit(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RollOutcome outcome,
            CharacterActionParams actionParams,
            RulesetAttackMode attackMode,
            ActionModifier attackModifier)
        {
            if (attackMode == null || outcome is RollOutcome.Failure or RollOutcome.CriticalFailure)
            {
                return;
            }

            var rulesetDefender = defender.RulesetCharacter;

            if (!rulesetDefender.HasConditionOfType(ConditionBlinded)
                && !rulesetDefender.HasConditionOfType(ConditionFrightened)
                && !rulesetDefender.HasConditionOfType(ConditionRestrained)
                && !rulesetDefender.HasConditionOfType(ConditionIncapacitated)
                && !rulesetDefender.HasConditionOfType(ConditionParalyzed)
                && !rulesetDefender.HasConditionOfType(ConditionProne)
                && !rulesetDefender.HasConditionOfType(ConditionStunned))
            {
                return;
            }

            var effectDescription = attackMode.EffectDescription;
            var damage = effectDescription.FindFirstDamageForm();

            if (damage == null)
            {
                return;
            }

            var proficiencyBonus =
                attacker.RulesetCharacter.GetAttribute(AttributeDefinitions.ProficiencyBonus).CurrentValue;

            damage.BonusDamage += proficiencyBonus;
            damage.DamageBonusTrends.Add(new RuleDefinitions.TrendInfo(proficiencyBonus,
                FeatureSourceType.FightingStyle, "Executioner", null));
        }
    }
}
