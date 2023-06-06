using System.Collections.Generic;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Subclasses;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFightingStyleChoices;

namespace SolastaUnfinishedBusiness.FightingStyles;

internal sealed class Executioner : AbstractFightingStyle
{
    private const string ExecutionerName = "Executioner";

    private static readonly FeatureDefinition FeatureFightingStyleExecutioner = FeatureDefinitionBuilder
        .Create("FeatureFightingStyleExecutioner")
        .SetGuiPresentationNoContent(true)
        .SetCustomSubFeatures(new CustomAdditionalDamageExecutioner(
            FeatureDefinitionAdditionalDamageBuilder
                .Create("AdditionalDamageFightingStyleExecutioner")
                .SetGuiPresentationNoContent(true)
                .SetNotificationTag(ExecutionerName)
                .SetDamageValueDetermination(AdditionalDamageValueDetermination.ProficiencyBonus)
                .AddToDB()))
        .AddToDB();

    internal override FightingStyleDefinition FightingStyle { get; } = FightingStyleBuilder
        .Create(ExecutionerName)
        .SetGuiPresentation(Category.FightingStyle, Sprites.GetSprite(ExecutionerName, Resources.Executioner, 256))
        .SetFeatures(FeatureFightingStyleExecutioner)
        .AddToDB();

    internal override List<FeatureDefinitionFightingStyleChoice> FightingStyleChoice => new()
    {
        FightingStyleChampionAdditional, FightingStyleFighter, FightingStylePaladin, FightingStyleRanger
    };

    private sealed class CustomAdditionalDamageExecutioner : CustomAdditionalDamage
    {
        public CustomAdditionalDamageExecutioner(IAdditionalDamageProvider provider) : base(provider)
        {
        }

        internal override bool IsValid(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            RulesetAttackMode attackMode,
            bool rangedAttack,
            AdvantageType advantageType,
            List<EffectForm> actualEffectForms,
            RulesetEffect rulesetEffect,
            bool criticalHit,
            bool firstTarget,
            out CharacterActionParams reactionParams)
        {
            reactionParams = null;

            var rulesetDefender = defender.RulesetCharacter;

            if (attackMode == null || rulesetDefender == null || rulesetDefender.IsDeadOrDying)
            {
                return false;
            }

            var survivalistLevel = attacker.RulesetCharacter
                .GetSubclassLevel(DatabaseHelper.CharacterClassDefinitions.Ranger, RangerSurvivalist.Name);

            if (survivalistLevel >= 11)
            {
                return rulesetDefender.HasAnyConditionOfType(
                    ConditionBlinded,
                    ConditionFrightened,
                    ConditionRestrained,
                    ConditionGrappled,
                    ConditionIncapacitated,
                    ConditionParalyzed,
                    ConditionProne,
                    ConditionStunned,
                    "ConditionHindered");
            }

            return rulesetDefender.HasAnyConditionOfType(
                ConditionBlinded,
                ConditionFrightened,
                ConditionRestrained,
                ConditionGrappled,
                ConditionIncapacitated,
                ConditionParalyzed,
                ConditionProne,
                ConditionStunned);
        }
    }
}
