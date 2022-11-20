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
                .SetCustomSubFeatures(new OnAttackDamageEffectFightingStyleExecutioner())
                .AddToDB())
        .AddToDB();

    internal override List<FeatureDefinitionFightingStyleChoice> FightingStyleChoice => new()
    {
        FightingStyleChampionAdditional, FightingStyleFighter, FightingStylePaladin, FightingStyleRanger
    };

    private sealed class OnAttackDamageEffectFightingStyleExecutioner : IOnAttackDamageEffect
    {
        public void BeforeOnAttackDamage(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            RulesetAttackMode attackMode,
            bool rangedAttack,
            RuleDefinitions.AdvantageType advantageType,
            List<EffectForm> actualEffectForms,
            RulesetEffect rulesetEffect,
            bool criticalHit,
            bool firstTarget)
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
                RuleDefinitions.FeatureSourceType.FightingStyle, "Executioner", null));
        }
    }
}
