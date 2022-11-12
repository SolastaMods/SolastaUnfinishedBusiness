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

    private sealed class OnAttackHitEffectFightingStyleExecutioner : IOnAttackDamageEffect
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
            var i = effectDescription.EffectForms.FindIndex(x => x.damageForm == damage);

            if (i < 0 || damage == null)
            {
                return;
            }

            var proficiencyBonus =
                attacker.RulesetCharacter.GetAttribute(AttributeDefinitions.ProficiencyBonus).CurrentValue;
            var additionalDice = EffectFormBuilder
                .Create()
                .SetDamageForm(damage.damageType, 0, RuleDefinitions.DieType.D4, proficiencyBonus)
                .Build();

            effectDescription.EffectForms.Insert(i + 1, additionalDice);
        }

        public void AfterOnAttackDamage(
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
        }
    }
}
