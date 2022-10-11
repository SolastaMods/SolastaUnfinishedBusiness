using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class MartialTactician : AbstractSubclass
{
    internal const string CounterStrikeTag = "CounterStrike";

    internal MartialTactician()
    {
        var powerPoolTacticianGambit = FeatureDefinitionPowerPoolBuilder
            .Create("PowerPoolTacticianGambit")
            .SetGuiPresentation(Category.Feature)
            .SetActivationTime(ActivationTime.Permanent)
            .Configure(
                4,
                UsesDetermination.Fixed,
                AttributeDefinitions.Dexterity,
                RechargeRate.ShortRest)
            .AddToDB();

        var powerSharedPoolTacticianKnockDown = FeatureDefinitionPowerSharedPoolBuilder
            .Create("PowerSharedPoolTacticianKnockDown")
            .SetGuiPresentation(Category.Feature, PowerFighterActionSurge.GuiPresentation.SpriteReference)
            .Configure(
                powerPoolTacticianGambit,
                RechargeRate.ShortRest,
                ActivationTime.OnAttackHit,
                1,
                true,
                true,
                AttributeDefinitions.Strength,
                EffectDescriptionBuilder
                    .Create(PowerFighterActionSurge.EffectDescription)
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(
                                false,
                                DieType.D1,
                                DamageTypeBludgeoning,
                                2,
                                DieType.D6,
                                1)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetMotionForm(MotionForm.MotionType.FallProne, 1)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build())
                    .SetSavingThrowData(
                        true,
                        false,
                        AttributeDefinitions.Strength,
                        false,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                        AttributeDefinitions.Strength,
                        15)
                    .Build(),
                false)
            .AddToDB();

        var powerSharedPoolTacticianInspire = FeatureDefinitionPowerSharedPoolBuilder
            .Create("PowerSharedPoolTacticianInspire")
            .SetGuiPresentation(Category.Feature, PowerDomainLifePreserveLife.GuiPresentation.SpriteReference)
            .Configure(
                powerPoolTacticianGambit,
                RechargeRate.ShortRest,
                ActivationTime.BonusAction,
                1,
                true,
                true,
                AttributeDefinitions.Strength,
                EffectDescriptionBuilder
                    .Create(PowerDomainLifePreserveLife.EffectDescription)
                    .SetCanBePlacedOnCharacter()
                    .SetDurationData(DurationType.Day, 1)
                    .SetTargetProximityData(false, 12)
                    .SetTargetingData(
                        Side.Ally,
                        RangeType.Distance,
                        30,
                        TargetType.Individuals,
                        1,
                        2,
                        ActionDefinitions.ItemSelectionType.Equiped)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetTempHpForm(2, DieType.D6, 1)
                            .Build())
                    .Build(),
                false)
            .AddToDB();

        // TODO: make it do the same damage as the wielded weapon
        // (seems impossible with current tools, would need to use the AdditionalDamage feature but I'm not sure how to combine that with this to make it a reaction ability)
        var powerSharedPoolTacticianCounterStrike = FeatureDefinitionPowerSharedPoolBuilder
            .Create("PowerSharedPoolTacticianCounterStrike")
            .SetGuiPresentation(Category.Feature, PowerDomainLawHolyRetribution.GuiPresentation.SpriteReference)
            .Configure(
                powerPoolTacticianGambit,
                RechargeRate.ShortRest,
                ActivationTime.Reaction,
                1,
                true,
                true,
                AttributeDefinitions.Strength,
                EffectDescriptionBuilder
                    .Create(PowerDomainLawHolyRetribution.EffectDescription)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(
                                false,
                                DieType.D1,
                                DamageTypeBludgeoning,
                                2,
                                DieType.D6,
                                1)
                            .Build())
                    .Build(),
                false)
            .AddToDB();

        var powerPoolTacticianGambitAdd = FeatureDefinitionPowerPoolModifierBuilder
            .Create("PowerPoolTacticianGambitAdd")
            .SetGuiPresentation(Category.Feature)
            .SetActivationTime(ActivationTime.Permanent)
            .Configure(
                1,
                UsesDetermination.Fixed,
                AttributeDefinitions.Dexterity,
                powerPoolTacticianGambit)
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("MartialTactician")
            .SetGuiPresentation(Category.Subclass, RoguishShadowCaster.GuiPresentation.SpriteReference)
            .AddFeaturesAtLevel(3,
                powerPoolTacticianGambit,
                powerSharedPoolTacticianKnockDown,
                powerSharedPoolTacticianInspire,
                powerSharedPoolTacticianCounterStrike)
            .AddFeaturesAtLevel(7,
                FeatureDefinitionFeatureSets.FeatureSetChampionRemarkableAthlete)
            .AddFeaturesAtLevel(10,
                powerPoolTacticianGambitAdd)
            .AddFeaturesAtLevel(15,
                powerPoolTacticianGambitAdd)
            .AddFeaturesAtLevel(18,
                powerPoolTacticianGambitAdd)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceFighterMartialArchetypes;
}
