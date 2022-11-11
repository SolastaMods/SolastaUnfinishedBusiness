using System.Collections.Generic;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomInterfaces;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFightingStyleChoices;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;

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
                .SetCustomSubFeatures(new OnAttackHitEffectFightingStyleExecutioner())
                .AddToDB())
        .AddToDB();

    internal override List<FeatureDefinitionFightingStyleChoice> FightingStyleChoice => new()
    {
        FightingStyleChampionAdditional, FightingStyleFighter, FightingStylePaladin, FightingStyleRanger
    };

    private sealed class OnAttackHitEffectFightingStyleExecutioner : IOnAttackHitEffect
    {
        public void AfterOnAttackHit(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RuleDefinitions.RollOutcome outcome,
            CharacterActionParams actionParams,
            RulesetAttackMode attackMode,
            ActionModifier attackModifier)
        {
            // melee attack only
            if (attackMode == null || defender == null)
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

            var damage = attackMode.EffectDescription?.FindFirstDamageForm();

            if (damage == null)
            {
                return;
            }

            var proficiencyBonus = 
                attacker.RulesetCharacter.GetAttribute(AttributeDefinitions.ProficiencyBonus).CurrentValue;

            damage.BonusDamage += proficiencyBonus;
            damage.DamageBonusTrends.Add(new RuleDefinitions.TrendInfo(proficiencyBonus,
                RuleDefinitions.FeatureSourceType.Power, ExecutionerName, null));
        }
    }
}
