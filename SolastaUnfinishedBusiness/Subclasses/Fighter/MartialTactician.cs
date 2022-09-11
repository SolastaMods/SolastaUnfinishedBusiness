using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses.Fighter;

internal sealed class MartialTactician : AbstractSubclass
{
    // ReSharper disable once InconsistentNaming
    private CharacterSubclassDefinition Subclass;

    internal override FeatureDefinitionSubclassChoice GetSubclassChoiceList()
    {
        return FeatureDefinitionSubclassChoices.SubclassChoiceFighterMartialArchetypes;
    }

    internal override CharacterSubclassDefinition GetSubclass()
    {
        return Subclass ??= TacticianFighterSubclassBuilder.BuildAndAddSubclass();
    }
}

internal static class PowerSharedPoolKnockDownBuilder
{
    private const string PowerSharedPoolKnockDownName = "PowerSharedPoolKnockDown";
    private const string PowerSharedPoolKnockDownNameGuid = "90dd5e81-40d7-4824-89b4-45bcf4c05218";

    private static FeatureDefinitionPowerSharedPool Build(string name, string guid)
    {
        //Create the damage form - TODO make it do the same damage as the wielded weapon?  This doesn't seem possible
        var damageEffect = new EffectForm
        {
            DamageForm = new DamageForm
            {
                DiceNumber = 1,
                DieType = RuleDefinitions.DieType.D6,
                BonusDamage = 2,
                DamageType = RuleDefinitions.DamageTypeBludgeoning
            },
            SavingThrowAffinity = RuleDefinitions.EffectSavingThrowType.None
        };

        //Create the prone effect - Weirdly enough the motion form seems to also automatically apply the prone condition
        var proneMotionEffect = new EffectForm
        {
            formType = EffectForm.EffectFormType.Motion,
            motionForm = new MotionForm { type = MotionForm.MotionType.FallProne, distance = 1 },
            savingThrowAffinity = RuleDefinitions.EffectSavingThrowType.Negates
        };

        //Add to our new effect
        var newEffectDescription = FeatureDefinitionPowers.PowerFighterActionSurge.EffectDescription.Copy();

        newEffectDescription.SetEffectForms(damageEffect, proneMotionEffect);
        newEffectDescription.SetSavingThrowDifficultyAbility(AttributeDefinitions.Strength);
        newEffectDescription.SetDifficultyClassComputation(RuleDefinitions.EffectDifficultyClassComputation
            .AbilityScoreAndProficiency);
        newEffectDescription.SavingThrowAbility = AttributeDefinitions.Strength;
        newEffectDescription.HasSavingThrow = true;
        newEffectDescription.DurationType = RuleDefinitions.DurationType.Round;

        var builder = new FeatureDefinitionPowerSharedPoolBuilder(
            name, guid,
            TacticianFighterSubclassBuilder.PowerPoolTacticianGambit,
            RuleDefinitions.RechargeRate.ShortRest,
            RuleDefinitions.ActivationTime.OnAttackHit,
            1,
            true,
            true,
            AttributeDefinitions.Strength,
            newEffectDescription,
            new GuiPresentationBuilder("Feature/&PowerSharedPoolKnockDownTitle",
                    "Feature/&PowerSharedPoolKnockDownDescription")
                .SetSpriteReference(FeatureDefinitionPowers.PowerFighterActionSurge.GuiPresentation.SpriteReference)
                .Build(),
            false);

        builder.SetShortTitle("Feature/&PowerSharedPoolKnockDownTitle");
        
        return builder.AddToDB();
    }

    internal static FeatureDefinitionPowerSharedPool CreateAndAddToDB()
    {
        return Build(PowerSharedPoolKnockDownName, PowerSharedPoolKnockDownNameGuid);
    }
}

internal static class PowerSharedPoolInspirePowerBuilder
{
    private const string PowerSharedPoolInspirePowerName = "PowerSharedPoolInspirePower";
    private const string PowerSharedPoolInspirePowerNameGuid = "163c28de-48e5-4f75-bdd0-d42374a75ef8";

    private static FeatureDefinitionPowerSharedPool Build(string name, string guid)
    {
        //Create the temp hp form
        var healingEffect = new EffectForm
        {
            formType = EffectForm.EffectFormType.TemporaryHitPoints,
            temporaryHitPointsForm = new TemporaryHitPointsForm
            {
                DiceNumber = 1, DieType = RuleDefinitions.DieType.D6, BonusHitPoints = 2
            }
        };

        //Add to our new effect
        var newEffectDescription = new EffectDescription();

        newEffectDescription.Copy(FeatureDefinitionPowers.PowerDomainLifePreserveLife.EffectDescription);
        newEffectDescription.EffectForms.Clear();
        newEffectDescription.EffectForms.Add(healingEffect);
        newEffectDescription.HasSavingThrow = false;
        newEffectDescription.DurationType = RuleDefinitions.DurationType.Day;
        newEffectDescription.SetTargetSide(RuleDefinitions.Side.Ally);
        newEffectDescription.SetTargetType(RuleDefinitions.TargetType.Individuals);
        newEffectDescription.SetTargetProximityDistance(12);
        newEffectDescription.SetCanBePlacedOnCharacter(true);
        newEffectDescription.SetRangeType(RuleDefinitions.RangeType.Distance);

        var builder = new FeatureDefinitionPowerSharedPoolBuilder(
            name, guid,
            TacticianFighterSubclassBuilder.PowerPoolTacticianGambit,
            RuleDefinitions.RechargeRate.ShortRest,
            RuleDefinitions.ActivationTime.BonusAction,
            1,
            true,
            true,
            AttributeDefinitions.Strength,
            newEffectDescription,
            new GuiPresentationBuilder("Feature/&PowerSharedPoolInspirePowerTitle",
                    "Feature/&PowerSharedPoolInspirePowerDescription")
                .SetSpriteReference(FeatureDefinitionPowers.PowerDomainLifePreserveLife.GuiPresentation
                    .SpriteReference).Build(),
            false);

        builder.SetShortTitle("Feature/&PowerSharedPoolInspirePowerTitle");

        return builder.AddToDB();
    }

    internal static FeatureDefinitionPowerSharedPool CreateAndAddToDB()
    {
        return Build(PowerSharedPoolInspirePowerName, PowerSharedPoolInspirePowerNameGuid);
    }
}

internal static class PowerSharedPoolTacticianCounterStrikeBuilder
{
    private const string PowerSharedPoolTacticianCounterStrikeName = "PowerSharedPoolTacticianCounterStrike";
    private const string PowerSharedPoolTacticianCounterStrikeNameGuid = "88c294ce-14fa-4f7e-8b81-ea4d289e3d8b";

    private static FeatureDefinitionPowerSharedPool Build(string name, string guid)
    {
        // TODO: make it do the same damage as the wielded weapon (seems impossible with current tools, would need to use the AdditionalDamage feature but I'm not sure how to combine that with this to make it a reaction ability).
        var damageEffect = new EffectForm
        {
            damageForm = new DamageForm
            {
                DiceNumber = 1,
                DieType = RuleDefinitions.DieType.D6,
                BonusDamage = 2,
                DamageType = RuleDefinitions.DamageTypeBludgeoning
            },
            savingThrowAffinity = RuleDefinitions.EffectSavingThrowType.None
        };

        //Add to our new effect
        var newEffectDescription = new EffectDescription();

        newEffectDescription.Copy(FeatureDefinitionPowers.PowerDomainLawHolyRetribution.EffectDescription);
        newEffectDescription.EffectForms.SetRange(damageEffect);

        var builder = new FeatureDefinitionPowerSharedPoolBuilder(
            name, guid,
            TacticianFighterSubclassBuilder.PowerPoolTacticianGambit,
            RuleDefinitions.RechargeRate.ShortRest,
            RuleDefinitions.ActivationTime.Reaction,
            1,
            true,
            true,
            AttributeDefinitions.Strength,
            newEffectDescription,
            new GuiPresentationBuilder("Feature/&PowerSharedPoolTacticianCounterStrikeTitle",
                    "Feature/&PowerSharedPoolTacticianCounterStrikeDescription")
                .SetSpriteReference(FeatureDefinitionPowers.PowerDomainLawHolyRetribution.GuiPresentation
                    .SpriteReference).Build(),
            false);

        builder.SetReaction(RuleDefinitions.ReactionTriggerContext.HitByMelee, "CounterStrike");

        return builder.AddToDB();
    }

    internal static FeatureDefinitionPowerSharedPool CreateAndAddToDB()
    {
        return Build(PowerSharedPoolTacticianCounterStrikeName, PowerSharedPoolTacticianCounterStrikeNameGuid);
    }
}

internal static class PowerPoolTacticianGambitAddBuilder
{
    private const string PowerPoolModifierTacticianGambitAdd10Name = "PowerPoolModifierTacticianGambitAdd10";
    private const string PowerPoolModifierTacticianGambitAdd10Guid = "52b74360-eecf-407c-9445-4515cbb372f3";

    private const string PowerPoolModifierTacticianGambitAdd15Name = "PowerPoolModifierTacticianGambitAdd15";
    private const string PowerPoolModifierTacticianGambitAdd15Guid = "b4307074-cd80-4376-96f0-46f7a3a79b5a";

    private const string PowerPoolModifierTacticianGambitAdd18Name = "PowerPoolModifierTacticianGambitAdd18";
    private const string PowerPoolModifierTacticianGambitAdd18Guid = "c7ced45a-572f-4af0-8ec5-2add074dd7c3";

    private static FeatureDefinitionPower CreateAndAddToDB(string name, string guid)
    {
        return FeatureDefinitionPowerPoolModifierBuilder.Create(name, guid)
            .Configure(
                1,
                RuleDefinitions.UsesDetermination.Fixed,
                AttributeDefinitions.Dexterity,
                TacticianFighterSubclassBuilder.PowerPoolTacticianGambit)
            .SetGuiPresentation(new GuiPresentationBuilder("Feature/&PowerPoolTacticianGambitAddTitle",
                "Feature/&PowerPoolTacticianGambitAddDescription").Build())
            .AddToDB();
    }

    internal static FeatureDefinitionPower PowerPoolModifierTacticianGambitAdd10()
    {
        return CreateAndAddToDB(PowerPoolModifierTacticianGambitAdd10Name,
            PowerPoolModifierTacticianGambitAdd10Guid);
    }

    internal static FeatureDefinitionPower PowerPoolModifierTacticianGambitAdd15()
    {
        return CreateAndAddToDB(PowerPoolModifierTacticianGambitAdd15Name,
            PowerPoolModifierTacticianGambitAdd15Guid);
    }

    internal static FeatureDefinitionPower PowerPoolModifierTacticianGambitAdd18()
    {
        return CreateAndAddToDB(PowerPoolModifierTacticianGambitAdd18Name,
            PowerPoolModifierTacticianGambitAdd18Guid);
    }
}

internal static class TacticianFighterSubclassBuilder
{
    private const string TacticianFighterSubclassName = "MartialTactician";
    private const string TacticianFighterSubclassNameGuid = "9d32577d-d3ec-4859-b66d-451d071bb117";

    internal static readonly FeatureDefinitionPower PowerPoolTacticianGambit = FeatureDefinitionPowerPoolBuilder
        .Create("PowerPoolTacticianGambit", "00da2b27-139a-4ca0-a285-aaa70d108bc8")
        .Configure(
            4,
            RuleDefinitions.UsesDetermination.Fixed,
            AttributeDefinitions.Dexterity,
            RuleDefinitions.RechargeRate.ShortRest)
        .SetGuiPresentation(Category.Feature)
        .AddToDB();

    private static readonly FeatureDefinitionPower PowerPoolModifierTacticianGambitAdd10 =
        PowerPoolTacticianGambitAddBuilder.PowerPoolModifierTacticianGambitAdd10();

    private static readonly FeatureDefinitionPower PowerPoolModifierTacticianGambitAdd15 =
        PowerPoolTacticianGambitAddBuilder.PowerPoolModifierTacticianGambitAdd15();

    private static readonly FeatureDefinitionPower PowerPoolModifierTacticianGambitAdd18 =
        PowerPoolTacticianGambitAddBuilder.PowerPoolModifierTacticianGambitAdd18();

    private static readonly FeatureDefinitionPowerSharedPool PowerSharedPoolKnockDown =
        PowerSharedPoolKnockDownBuilder.CreateAndAddToDB();

    private static readonly FeatureDefinitionPowerSharedPool PowerSharedPoolInspirePower =
        PowerSharedPoolInspirePowerBuilder.CreateAndAddToDB();

    private static readonly FeatureDefinitionPowerSharedPool PowerSharedPoolTacticianCounterStrike =
        PowerSharedPoolTacticianCounterStrikeBuilder.CreateAndAddToDB();

    internal static CharacterSubclassDefinition BuildAndAddSubclass()
    {
        return CharacterSubclassDefinitionBuilder
            .Create(TacticianFighterSubclassName, TacticianFighterSubclassNameGuid)
            .SetGuiPresentation(Category.Subclass,
                RoguishShadowCaster.GuiPresentation.SpriteReference)
            .AddFeatureAtLevel(PowerPoolTacticianGambit, 3)
            .AddFeatureAtLevel(PowerSharedPoolKnockDown, 3)
            .AddFeatureAtLevel(PowerSharedPoolInspirePower, 3)
            .AddFeatureAtLevel(PowerSharedPoolTacticianCounterStrike, 3)
            .AddFeatureAtLevel(FeatureDefinitionFeatureSets.FeatureSetChampionRemarkableAthlete, 7)
            .AddFeatureAtLevel(PowerPoolModifierTacticianGambitAdd10, 10)
            .AddFeatureAtLevel(PowerPoolModifierTacticianGambitAdd15, 15)
            .AddFeatureAtLevel(PowerPoolModifierTacticianGambitAdd18, 18)
            .AddToDB();
    }
}
