using System.Collections.Generic;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Validators;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFightingStyleChoices;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.FightingStyles;

internal sealed class Torchbearer : AbstractFightingStyle
{
    private static readonly FeatureDefinitionPower PowerFightingStyleTorchbearer = FeatureDefinitionPowerBuilder
        .Create("PowerFightingStyleTorchbearer")
        .SetGuiPresentation(Category.Feature,
            Sprites.GetSprite("PowerTorchBearer", Resources.PowerTorchBearer, 256, 128))
        .SetUsesFixed(ActivationTime.BonusAction)
        .SetShowCasting(false)
        .SetEffectDescription(
            EffectDescriptionBuilder
                .Create()
                .SetDurationData(DurationType.Minute, 1)
                .SetTargetingData(Side.Enemy, RangeType.Touch, 0, TargetType.IndividualsUnique)
                .SetSavingThrowData(
                    false,
                    AttributeDefinitions.Dexterity,
                    false,
                    EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                    AttributeDefinitions.Dexterity,
                    8)
                .SetParticleEffectParameters(SpellDefinitions.FireBolt)
                .SetEffectForms(
                    EffectFormBuilder
                        .Create()
                        .SetConditionForm(ConditionDefinitions.ConditionOnFire1D4, ConditionForm.ConditionOperation.Add)
                        .HasSavingThrow(EffectSavingThrowType.Negates, TurnOccurenceType.StartOfTurn)
                        .Build())
                .Build())
        .AddCustomSubFeatures(new ValidatorsValidatePowerUse(ValidatorsCharacter.HasLightSourceOffHand))
        .AddToDB();

    internal override FightingStyleDefinition FightingStyle { get; } = FightingStyleBuilder
        .Create("Torchbearer")
        .SetGuiPresentation(Category.FightingStyle, Sprites.GetSprite("Torchbearer", Resources.Torchbearer, 256))
        .SetFeatures(
            FeatureDefinitionBuilder
                .Create("AddExtraAttackTorchbearer")
                .SetGuiPresentationNoContent(true)
                .AddCustomSubFeatures(new AddBonusTorchAttack(PowerFightingStyleTorchbearer))
                .AddToDB())
        .AddToDB();

    internal override List<FeatureDefinitionFightingStyleChoice> FightingStyleChoice =>
    [
        CharacterContext.FightingStyleChoiceBarbarian,
        CharacterContext.FightingStyleChoiceMonk,
        CharacterContext.FightingStyleChoiceRogue,
        FightingStyleChampionAdditional,
        FightingStyleFighter,
        FightingStyleRanger
    ];
}
