using System.Collections.Generic;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.Models;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFightingStyleChoices;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.FightingStyles;

internal sealed class Torchbearer : AbstractFightingStyle
{

    private static readonly FeatureDefinitionPower PowerFightingStyleTorchbearer = FeatureDefinitionPowerBuilder
        .Create("PowerFightingStyleTorchbearer")
        .SetGuiPresentation(Category.Feature, PowerDragonbornBreathWeaponGold)
        .SetUsesFixed(ActivationTime.BonusAction)
        .SetEffectDescription(EffectDescriptionBuilder
            .Create(SpellDefinitions.Fireball.EffectDescription)
            .SetCanBePlacedOnCharacter(false)
            .SetDurationData(DurationType.Round, 3)
            .SetSpeed(SpeedType.Instant, 11f)
            .SetTargetingData(Side.Enemy, RangeType.Touch, 1, TargetType.Individuals)
            .SetEffectForms(
                EffectFormBuilder
                    .Create()
                    .SetConditionForm(
                        ConditionDefinitions.ConditionOnFire1D4,
                        ConditionForm.ConditionOperation.Add)
                    .Build())
            .SetSavingThrowData(
                false,
                AttributeDefinitions.Dexterity,
                false,
                EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                AttributeDefinitions.Dexterity,
                8)
            .Build())
        .SetShowCasting(false)
        .SetCustomSubFeatures(new ValidatorsPowerUse(ValidatorsCharacter.OffHandHasLightSource))
        .AddToDB();

    internal override FightingStyleDefinition FightingStyle { get; } = FightingStyleBuilder
        .Create("Torchbearer")
        .SetGuiPresentation(Category.FightingStyle, DatabaseHelper.CharacterSubclassDefinitions.MartialMountaineer)
        .SetCustomSubFeatures(new OnAttackEffectTorchbearer())
        .AddToDB();

    internal override List<FeatureDefinitionFightingStyleChoice> FightingStyleChoice => new()
    {
        FightingStyleChampionAdditional, FightingStyleFighter, FightingStyleRanger
    };

    private sealed class OnAttackEffectTorchbearer : IAfterAttackEffect
    {
        public void AfterOnAttackHit(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RuleDefinitions.RollOutcome outcome,
            CharacterActionParams actionParams,
            RulesetAttackMode attackMode,
            ActionModifier attackModifier)
        {
            var rulesetAttacker = attacker.RulesetCharacter;

            if (attackMode == null || !ValidatorsCharacter.OffHandHasLightSource(rulesetAttacker))
            {
                return;
            }

            var usablePower = new RulesetUsablePower(PowerFightingStyleTorchbearer, null, null);

            usablePower.Recharge();
            rulesetAttacker.UsablePowers.Add(usablePower);
            rulesetAttacker.RefreshUsablePower(usablePower);
        }
    }
}
