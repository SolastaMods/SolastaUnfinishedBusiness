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
        var powerPoolTacticianGambit = FeatureDefinitionPowerBuilder
            .Create("PowerPoolTacticianGambit")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.Permanent, RechargeRate.ShortRest, 1, 4)
            .SetIsPowerPool()
            .AddToDB();

        var powerSharedPoolTacticianKnockDown = FeatureDefinitionPowerSharedPoolBuilder
            .Create("PowerSharedPoolTacticianKnockDown")
            .SetGuiPresentation(Category.Feature, PowerFighterActionSurge)
            .SetUsesFixed(ActivationTime.OnAttackHit, RechargeRate.ShortRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(PowerFighterActionSurge.EffectDescription)
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeBludgeoning, 1, DieType.D6, 2)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetMotionForm(MotionForm.MotionType.FallProne, 1)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build())
                    .SetSavingThrowData(
                        false,
                        AttributeDefinitions.Strength,
                        false,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                        AttributeDefinitions.Strength,
                        15)
                    .Build())
            .SetSharedPool(powerPoolTacticianGambit)
            .SetBonusToAttack(true, true, AttributeDefinitions.Strength)
            .AddToDB();

        var powerSharedPoolTacticianInspire = FeatureDefinitionPowerSharedPoolBuilder
            .Create("PowerSharedPoolTacticianInspire")
            .SetGuiPresentation(Category.Feature, PowerDomainLifePreserveLife)
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.ShortRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(PowerDomainLifePreserveLife.EffectDescription)
                    .SetCanBePlacedOnCharacter()
                    .SetDurationData(DurationType.Day, 1)
                    .SetTargetProximityData(false, 12)
                    .SetTargetingData(Side.Ally, RangeType.Distance, 30, TargetType.Individuals)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetTempHpForm(2, DieType.D6, 1)
                            .Build())
                    .Build())
            .SetSharedPool(powerPoolTacticianGambit)
            .SetBonusToAttack(true, true, AttributeDefinitions.Strength)
            .AddToDB();

        // TODO: make it do the same damage as the wielded weapon
        // (seems impossible with current tools, would need to use the AdditionalDamage feature but I'm not sure how to combine that with this to make it a reaction ability)
        var powerSharedPoolTacticianCounterStrike = FeatureDefinitionPowerSharedPoolBuilder
            .Create("PowerSharedPoolTacticianCounterStrike")
            .SetGuiPresentation(Category.Feature, PowerDomainLawHolyRetribution)
            .SetUsesFixed(ActivationTime.Reaction, RechargeRate.ShortRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(PowerDomainLawHolyRetribution.EffectDescription)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeBludgeoning, 1, DieType.D6, 2)
                            .Build())
                    .Build())
            .SetSharedPool(powerPoolTacticianGambit)
            .SetBonusToAttack(true, true, AttributeDefinitions.Strength)
            .AddToDB();

        var powerPoolTacticianGambitAdd = FeatureDefinitionPowerPoolModifierBuilder
            .Create("PowerPoolTacticianGambitAdd")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.Permanent)
            .SetPoolModifier(powerPoolTacticianGambit, 1)
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("MartialTactician")
            .SetGuiPresentation(Category.Subclass, RoguishShadowCaster)
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
