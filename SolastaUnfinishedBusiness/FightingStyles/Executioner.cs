using System.Collections.Generic;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFightingStyleChoices;

namespace SolastaUnfinishedBusiness.FightingStyles;

internal sealed class Executioner : AbstractFightingStyle
{
    private const string ExecutionerName = "Executioner";

    internal static readonly FeatureDefinition FeatureFightingStyleExecutioner = FeatureDefinitionBuilder
        .Create("FeatureFightingStyleExecutioner")
        .SetGuiPresentationNoContent(true)
        .SetCustomSubFeatures(new ExecutionerDamage(FeatureDefinitionAdditionalDamageBuilder
            .Create("AdditionalDamageFightingStyleExecutioner")
            .SetGuiPresentation(ExecutionerName, Category.FightingStyle)
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

    private sealed class ExecutionerDamage : CustomAdditionalDamage
    {
        public ExecutionerDamage(IAdditionalDamageProvider provider) : base(provider)
        {
        }

        internal override bool IsValid(GameLocationBattleManager battleManager, GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier, RulesetAttackMode attackMode, bool rangedAttack, AdvantageType advantageType,
            List<EffectForm> actualEffectForms, RulesetEffect rulesetEffect, bool criticalHit, bool firstTarget,
            out CharacterActionParams reactionParams)
        {
            reactionParams = null;

            if (attackMode == null)
            {
                return false;
            }

            return defender?.RulesetCharacter.HasAnyConditionOfType(
                ConditionBlinded,
                ConditionFrightened,
                ConditionRestrained,
                ConditionIncapacitated,
                ConditionParalyzed,
                ConditionProne,
                ConditionStunned
            ) == true;
        }
    }
}
