using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static RuleDefinitions;

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
            .SetGuiPresentation(Category.Feature,
                FeatureDefinitionPowers.PowerFighterActionSurge.GuiPresentation.SpriteReference)
            .Configure(
                powerPoolTacticianGambit,
                RechargeRate.ShortRest,
                ActivationTime.OnAttackHit,
                1,
                true,
                true,
                AttributeDefinitions.Strength,
                FeatureDefinitionPowers.PowerFighterActionSurge.EffectDescription
                    .Copy()
                    .SetEffectForms(
                        new EffectForm
                        {
                            DamageForm = new DamageForm
                            {
                                DiceNumber = 1,
                                DieType = DieType.D6,
                                BonusDamage = 2,
                                DamageType = DamageTypeBludgeoning
                            },
                            SavingThrowAffinity = EffectSavingThrowType.None
                        },
                        new EffectForm
                        {
                            formType = EffectForm.EffectFormType.Motion,
                            motionForm = new MotionForm { type = MotionForm.MotionType.FallProne, distance = 1 },
                            savingThrowAffinity = EffectSavingThrowType.Negates
                        })
                    .SetSavingThrowDifficultyAbility(AttributeDefinitions.Strength)
                    .SetDifficultyClassComputation(EffectDifficultyClassComputation.AbilityScoreAndProficiency)
                    .SetSavingThrowAbility(AttributeDefinitions.Strength)
                    .SetHasSavingThrow(true)
                    .SetDurationType(DurationType.Round),
                false)
            .AddToDB();

        var powerSharedPoolTacticianInspire = FeatureDefinitionPowerSharedPoolBuilder
            .Create("PowerSharedPoolTacticianInspire")
            .SetGuiPresentation(Category.Feature,
                FeatureDefinitionPowers.PowerDomainLifePreserveLife.GuiPresentation.SpriteReference)
            .Configure(
                powerPoolTacticianGambit,
                RechargeRate.ShortRest,
                ActivationTime.BonusAction,
                1,
                true,
                true,
                AttributeDefinitions.Strength,
                FeatureDefinitionPowers.PowerDomainLifePreserveLife.EffectDescription
                    .Copy()
                    .SetEffectForms(
                        new EffectForm
                        {
                            formType = EffectForm.EffectFormType.TemporaryHitPoints,
                            temporaryHitPointsForm = new TemporaryHitPointsForm
                            {
                                DiceNumber = 1, DieType = DieType.D6, BonusHitPoints = 2
                            }
                        })
                    .SetHasSavingThrow(false)
                    .SetDurationType(DurationType.Day)
                    .SetTargetSide(Side.Ally)
                    .SetTargetType(TargetType.Individuals)
                    .SetTargetProximityDistance(12)
                    .SetCanBePlacedOnCharacter(true)
                    .SetRangeType(RangeType.Distance),
                false)
            .AddToDB();

        // TODO: make it do the same damage as the wielded weapon
        // (seems impossible with current tools, would need to use the AdditionalDamage feature but I'm not sure how to combine that with this to make it a reaction ability)
        var powerSharedPoolTacticianCounterStrike = FeatureDefinitionPowerSharedPoolBuilder
            .Create("PowerSharedPoolTacticianCounterStrike")
            .SetGuiPresentation(Category.Feature, FeatureDefinitionPowers.PowerDomainLawHolyRetribution.GuiPresentation
                .SpriteReference)
            .Configure(
                powerPoolTacticianGambit,
                RechargeRate.ShortRest,
                ActivationTime.Reaction,
                1,
                true,
                true,
                AttributeDefinitions.Strength,
                FeatureDefinitionPowers.PowerDomainLawHolyRetribution.EffectDescription
                    .Copy()
                    .SetEffectForms(
                        new EffectForm
                        {
                            damageForm = new DamageForm
                            {
                                DiceNumber = 1,
                                DieType = DieType.D6,
                                BonusDamage = 2,
                                DamageType = DamageTypeBludgeoning
                            },
                            savingThrowAffinity = EffectSavingThrowType.None
                        }),
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
