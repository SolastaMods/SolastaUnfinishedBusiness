using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class MartialTactician : AbstractSubclass
{
    internal const string CounterStrikeTag = "CounterStrike";

    private static readonly FeatureDefinitionPower PowerPoolTacticianGambit = FeatureDefinitionPowerPoolBuilder
        .Create("PowerPoolTacticianGambit")
        .Configure(
            4,
            UsesDetermination.Fixed,
            AttributeDefinitions.Dexterity,
            RechargeRate.ShortRest)
        .SetGuiPresentation(Category.Feature)
        .AddToDB();

    internal MartialTactician()
    {
        var powerPoolTacticianGambitAdd = BuildPowerPoolTacticianGambitAdd();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("MartialTactician")
            .SetGuiPresentation(Category.Subclass, RoguishShadowCaster.GuiPresentation.SpriteReference)
            .AddFeaturesAtLevel(3,
                PowerPoolTacticianGambit,
                BuildPowerSharedPoolTacticianKnockDown(),
                BuildPowerSharedPoolTacticianInspire(),
                BuildPowerSharedPoolTacticianCounterStrike())
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

    internal override CharacterSubclassDefinition Subclass { get; set; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceFighterMartialArchetypes;

    private static FeatureDefinitionPowerSharedPool BuildPowerSharedPoolTacticianInspire()
    {
        //Create the temp hp form
        var healingEffect = new EffectForm
        {
            formType = EffectForm.EffectFormType.TemporaryHitPoints,
            temporaryHitPointsForm = new TemporaryHitPointsForm
            {
                DiceNumber = 1, DieType = DieType.D6, BonusHitPoints = 2
            }
        };

        //Add to our new effect
        var newEffectDescription = new EffectDescription()
            .Create(FeatureDefinitionPowers.PowerDomainLifePreserveLife.EffectDescription)
            .SetEffectForms(healingEffect)
            .SetHasSavingThrow(false)
            .SetDurationType(DurationType.Day)
            .SetTargetSide(Side.Ally)
            .SetTargetType(TargetType.Individuals)
            .SetTargetProximityDistance(12)
            .SetCanBePlacedOnCharacter(true)
            .SetRangeType(RangeType.Distance);

        var powerSharedPoolTacticianInspire = FeatureDefinitionPowerSharedPoolBuilder
            .Create("PowerSharedPoolTacticianInspire")
            .SetGuiPresentation(Category.Feature,
                FeatureDefinitionPowers.PowerDomainLifePreserveLife.GuiPresentation.SpriteReference)
            .Configure(
                PowerPoolTacticianGambit,
                RechargeRate.ShortRest,
                ActivationTime.BonusAction,
                1,
                true,
                true,
                AttributeDefinitions.Strength,
                newEffectDescription,
                false)
            .AddToDB();

        return powerSharedPoolTacticianInspire;
    }

    private static FeatureDefinitionPowerSharedPool BuildPowerSharedPoolTacticianKnockDown()
    {
        //Create the damage form
        var damageEffect = new EffectForm
        {
            DamageForm = new DamageForm
            {
                DiceNumber = 1, DieType = DieType.D6, BonusDamage = 2, DamageType = DamageTypeBludgeoning
            },
            SavingThrowAffinity = EffectSavingThrowType.None
        };

        //Create the prone effect - Weirdly enough the motion form seems to also automatically apply the prone condition
        var proneMotionEffect = new EffectForm
        {
            formType = EffectForm.EffectFormType.Motion,
            motionForm = new MotionForm { type = MotionForm.MotionType.FallProne, distance = 1 },
            savingThrowAffinity = EffectSavingThrowType.Negates
        };

        //Add to our new effect
        var newEffectDescription = FeatureDefinitionPowers.PowerFighterActionSurge.EffectDescription.Copy();

        newEffectDescription.SetEffectForms(damageEffect, proneMotionEffect);
        newEffectDescription.SetSavingThrowDifficultyAbility(AttributeDefinitions.Strength);
        newEffectDescription.SetDifficultyClassComputation(EffectDifficultyClassComputation.AbilityScoreAndProficiency);
        newEffectDescription.SetSavingThrowAbility(AttributeDefinitions.Strength);
        newEffectDescription.SetHasSavingThrow(true);
        newEffectDescription.SetDurationType(DurationType.Round);

        var powerSharedPoolTacticianKnockDown = FeatureDefinitionPowerSharedPoolBuilder
            .Create("PowerSharedPoolTacticianKnockDown")
            .SetGuiPresentation(Category.Feature,
                FeatureDefinitionPowers.PowerFighterActionSurge.GuiPresentation.SpriteReference)
            .Configure(
                PowerPoolTacticianGambit,
                RechargeRate.ShortRest,
                ActivationTime.OnAttackHit,
                1,
                true,
                true,
                AttributeDefinitions.Strength,
                newEffectDescription,
                false)
            .AddToDB();

        return powerSharedPoolTacticianKnockDown;
    }

    private static FeatureDefinitionPowerSharedPool BuildPowerSharedPoolTacticianCounterStrike()
    {
        // TODO: make it do the same damage as the wielded weapon (seems impossible with current tools, would need to use the AdditionalDamage feature but I'm not sure how to combine that with this to make it a reaction ability)
        var damageEffect = new EffectForm
        {
            damageForm = new DamageForm
            {
                DiceNumber = 1, DieType = DieType.D6, BonusDamage = 2, DamageType = DamageTypeBludgeoning
            },
            savingThrowAffinity = EffectSavingThrowType.None
        };

        //Add to our new effect
        var newEffectDescription = new EffectDescription()
            .Create(FeatureDefinitionPowers.PowerDomainLawHolyRetribution.EffectDescription)
            .SetEffectForms(damageEffect);

        var powerSharedPoolTacticianCounterStrike = FeatureDefinitionPowerSharedPoolBuilder
            .Create("PowerSharedPoolTacticianCounterStrike")
            .SetGuiPresentation(Category.Feature, FeatureDefinitionPowers.PowerDomainLawHolyRetribution.GuiPresentation
                .SpriteReference)
            .Configure(
                PowerPoolTacticianGambit,
                RechargeRate.ShortRest,
                ActivationTime.Reaction,
                1,
                true,
                true,
                AttributeDefinitions.Strength,
                newEffectDescription,
                false)
            .AddToDB();

        return powerSharedPoolTacticianCounterStrike;
    }

    private static FeatureDefinitionPowerPoolModifier BuildPowerPoolTacticianGambitAdd()
    {
        return FeatureDefinitionPowerPoolModifierBuilder
            .Create("PowerPoolTacticianGambitAdd")
            .SetGuiPresentation(Category.Feature)
            .Configure(
                1,
                UsesDetermination.Fixed,
                AttributeDefinitions.Dexterity,
                PowerPoolTacticianGambit)
            .AddToDB();
    }
}
