using SolastaCommunityExpansion.Api.Extensions;
using SolastaCommunityExpansion.Api.Infrastructure;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.CustomDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper;
using static SolastaCommunityExpansion.Api.DatabaseHelper.CharacterSubclassDefinitions;

namespace SolastaCommunityExpansion.Subclasses.Fighter;

internal sealed class Tactician : AbstractSubclass
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
                DamageType = "DamageBludgeoning"
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
            TacticianFighterSubclassBuilder.GambitResourcePool,
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
            TacticianFighterSubclassBuilder.GambitResourcePool,
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

internal static class PowerSharedPoolCounterStrikeBuilder
{
    private const string PowerSharedPoolCounterStrikeName = "PowerSharedPoolCounterStrike";
    private const string PowerSharedPoolCounterStrikeNameGuid = "88c294ce-14fa-4f7e-8b81-ea4d289e3d8b";

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
                DamageType = "DamageBludgeoning"
            },
            savingThrowAffinity = RuleDefinitions.EffectSavingThrowType.None
        };

        //Add to our new effect
        var newEffectDescription = new EffectDescription();

        newEffectDescription.Copy(FeatureDefinitionPowers.PowerDomainLawHolyRetribution.EffectDescription);
        newEffectDescription.EffectForms.SetRange(damageEffect);

        var builder = new FeatureDefinitionPowerSharedPoolBuilder(
            name, guid,
            TacticianFighterSubclassBuilder.GambitResourcePool,
            RuleDefinitions.RechargeRate.ShortRest,
            RuleDefinitions.ActivationTime.Reaction,
            1,
            true,
            true,
            AttributeDefinitions.Strength,
            newEffectDescription,
            new GuiPresentationBuilder("Feature/&PowerSharedPoolCounterStrikeTitle",
                    "Feature/&PowerSharedPoolCounterStrikeDescription")
                .SetSpriteReference(FeatureDefinitionPowers.PowerDomainLawHolyRetribution.GuiPresentation
                    .SpriteReference).Build(),
            false);

        builder.SetReaction(RuleDefinitions.ReactionTriggerContext.HitByMelee, "CounterStrike");

        return builder.AddToDB();
    }

    internal static FeatureDefinitionPowerSharedPool CreateAndAddToDB()
    {
        return Build(PowerSharedPoolCounterStrikeName, PowerSharedPoolCounterStrikeNameGuid);
    }
}

internal static class GambitResourcePoolAddBuilder
{
    private const string PowerPoolModifierGambitResourcePoolAdd10Name = "PowerPoolModifierGambitResourcePoolAdd10";
    private const string PowerPoolModifierGambitResourcePoolAdd10Guid = "52b74360-eecf-407c-9445-4515cbb372f3";

    private const string PowerPoolModifierGambitResourcePoolAdd15Name = "PowerPoolModifierGambitResourcePoolAdd15";
    private const string PowerPoolModifierGambitResourcePoolAdd15Guid = "b4307074-cd80-4376-96f0-46f7a3a79b5a";

    private const string PowerPoolModifierGambitResourcePoolAdd18Name = "PowerPoolModifierGambitResourcePoolAdd18";
    private const string PowerPoolModifierGambitResourcePoolAdd18Guid = "c7ced45a-572f-4af0-8ec5-2add074dd7c3";

    private static FeatureDefinitionPower CreateAndAddToDB(string name, string guid)
    {
        return FeatureDefinitionPowerPoolModifierBuilder.Create(name, guid)
            .Configure(
                1,
                RuleDefinitions.UsesDetermination.Fixed,
                AttributeDefinitions.Dexterity,
                TacticianFighterSubclassBuilder.GambitResourcePool)
            .SetGuiPresentation(new GuiPresentationBuilder("Feature/&GambitResourcePoolAddTitle",
                "Feature/&GambitResourcePoolAddDescription").Build())
            .AddToDB();
    }

    internal static FeatureDefinitionPower PowerPoolModifierGambitResourcePoolAdd10()
    {
        return CreateAndAddToDB(PowerPoolModifierGambitResourcePoolAdd10Name,
            PowerPoolModifierGambitResourcePoolAdd10Guid);
    }

    internal static FeatureDefinitionPower PowerPoolModifierGambitResourcePoolAdd15()
    {
        return CreateAndAddToDB(PowerPoolModifierGambitResourcePoolAdd15Name,
            PowerPoolModifierGambitResourcePoolAdd15Guid);
    }

    internal static FeatureDefinitionPower PowerPoolModifierGambitResourcePoolAdd18()
    {
        return CreateAndAddToDB(PowerPoolModifierGambitResourcePoolAdd18Name,
            PowerPoolModifierGambitResourcePoolAdd18Guid);
    }
}

internal static class TacticianFighterSubclassBuilder
{
    private const string TacticianFighterSubclassName = "FighterTactician";
    private const string TacticianFighterSubclassNameGuid = "9d32577d-d3ec-4859-b66d-451d071bb117";

    internal static readonly FeatureDefinitionPower GambitResourcePool = FeatureDefinitionPowerPoolBuilder
        .Create("GambitResourcePool", "00da2b27-139a-4ca0-a285-aaa70d108bc8")
        .Configure(
            4,
            RuleDefinitions.UsesDetermination.Fixed,
            AttributeDefinitions.Dexterity,
            RuleDefinitions.RechargeRate.ShortRest)
        .SetGuiPresentation(Category.Feature)
        .AddToDB();

    private static readonly FeatureDefinitionPower PowerPoolModifierGambitResourcePoolAdd10 =
        GambitResourcePoolAddBuilder.PowerPoolModifierGambitResourcePoolAdd10();

    private static readonly FeatureDefinitionPower PowerPoolModifierGambitResourcePoolAdd15 =
        GambitResourcePoolAddBuilder.PowerPoolModifierGambitResourcePoolAdd15();

    private static readonly FeatureDefinitionPower PowerPoolModifierGambitResourcePoolAdd18 =
        GambitResourcePoolAddBuilder.PowerPoolModifierGambitResourcePoolAdd18();

    private static readonly FeatureDefinitionPowerSharedPool PowerSharedPoolKnockDown =
        PowerSharedPoolKnockDownBuilder.CreateAndAddToDB();

    private static readonly FeatureDefinitionPowerSharedPool PowerSharedPoolInspirePower =
        PowerSharedPoolInspirePowerBuilder.CreateAndAddToDB();

    private static readonly FeatureDefinitionPowerSharedPool PowerSharedPoolCounterStrike =
        PowerSharedPoolCounterStrikeBuilder.CreateAndAddToDB();

    internal static CharacterSubclassDefinition BuildAndAddSubclass()
    {
        return CharacterSubclassDefinitionBuilder
            .Create(TacticianFighterSubclassName, TacticianFighterSubclassNameGuid)
            .SetGuiPresentation("TactitionFighterSubclass", Category.Subclass,
                RoguishShadowCaster.GuiPresentation.SpriteReference)
            .AddFeatureAtLevel(GambitResourcePool, 3)
            .AddFeatureAtLevel(PowerSharedPoolKnockDown, 3)
            .AddFeatureAtLevel(PowerSharedPoolInspirePower, 3)
            .AddFeatureAtLevel(PowerSharedPoolCounterStrike, 3)
            .AddFeatureAtLevel(FeatureDefinitionFeatureSets.FeatureSetChampionRemarkableAthlete, 7)
            .AddFeatureAtLevel(PowerPoolModifierGambitResourcePoolAdd10, 10)
            .AddFeatureAtLevel(PowerPoolModifierGambitResourcePoolAdd15, 15)
            .AddFeatureAtLevel(PowerPoolModifierGambitResourcePoolAdd18, 18)
            .AddToDB();
    }
}
