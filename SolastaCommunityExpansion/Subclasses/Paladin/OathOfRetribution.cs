using System;
using System.Collections.Generic;
using HarmonyLib;
using SolastaCommunityExpansion.Api;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static SolastaCommunityExpansion.Api.DatabaseHelper;

namespace SolastaCommunityExpansion.Subclasses.Paladin;

internal class OathOfRetribution : AbstractSubclass
{
    public static Guid ModGuidNamespace = new("1e13fc7e-08ab-42d1-ba98-f7854b3f58ea");
    private static readonly Guid SubclassNamespace = new("f5efd735-ff95-4256-ad17-dde585aeb5f3");
    private readonly CharacterSubclassDefinition Subclass;

    internal OathOfRetribution()
    {
        var paladinOathOfRetributionAutoPreparedSpells = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create("PaladinOathOfRetributionAutoPreparedSpells", SubclassNamespace)
            .SetGuiPresentation(Category.Feature)
            .SetCastingClass(CharacterClassDefinitions.Paladin)
            .SetPreparedSpellGroups(
                AutoPreparedSpellsGroupBuilder.BuildSpellGroup(3, SpellDefinitions.Bane, SpellDefinitions.HuntersMark),
                AutoPreparedSpellsGroupBuilder.BuildSpellGroup(5, SpellDefinitions.HoldPerson,
                    SpellDefinitions.MistyStep),
                AutoPreparedSpellsGroupBuilder.BuildSpellGroup(6, SpellDefinitions.Haste,
                    SpellDefinitions.ProtectionFromEnergy))
            .AddToDB();

        // var conditionFrightenedZealousAccusation = ConditionDefinitionBuilder
        //     .Create("ConditionFrightenedZealousAccusation", SubclassNamespace)
        //     .SetConditionType(RuleDefinitions.ConditionType.Detrimental)
        //     .SetAllowMultipleInstances(false)
        //     .SetSpecialInterruptions(
        //         RuleDefinitions.ConditionInterruption.Damaged,
        //         RuleDefinitions.ConditionInterruption.DamagedByFriendly
        //         )
        //     .Configure(RuleDefinitions.DurationType.Minute, 1, false,
        //         FeatureDefinitionCombatAffinitys.CombatAffinityFrightened,
        //         FeatureDefinitionMovementAffinitys.MovementAffinityConditionRestrained)
        //     .AddToDB();


        var presentationBuilder2 = new GuiPresentationBuilder(
            "Rules/&ConditionFrightenedZealousAccusationTitle",
            "Rules/&ConditionFrightenedZealousAccusationDescription",
            ConditionDefinitions.ConditionFrightened.GuiPresentation.SpriteReference);

        var condition1_1 = FeatureBuilder.BuildCondition(
            new List<FeatureDefinition>
            {
                FeatureDefinitionCombatAffinitys.CombatAffinityFrightened,
                FeatureDefinitionMovementAffinitys.MovementAffinityConditionRestrained
            }, RuleDefinitions.DurationType.Minute, 1,
            new List<RuleDefinitions.ConditionInterruption>
            {
                RuleDefinitions.ConditionInterruption.Damaged,
                RuleDefinitions.ConditionInterruption.DamagedByFriendly
            }, "ConditionFrightenedZealousAccusation", presentationBuilder2.Build());

        var presentationBuilder3 = new GuiPresentationBuilder(
            "Feature/&PowerOathOfRetributionZealousAccusationTitle",
            "Feature/&PowerOathOfRetributionZealousAccusationDescription",
            FeatureDefinitionPowers.PowerDomainLawHolyRetribution
                .GuiPresentation.SpriteReference);

        var feature2 = FeatureBuilder.BuildActionConditionPowerPaladinCD1(1, RuleDefinitions.UsesDetermination.Fixed,
            "Wisdom", RuleDefinitions.ActivationTime.Action, 1, RuleDefinitions.RechargeRate.ChannelDivinity,
            RuleDefinitions.RangeType.Distance, 12, RuleDefinitions.TargetType.Individuals,
            ActionDefinitions.ItemSelectionType.None, RuleDefinitions.DurationType.Minute, 1,
            RuleDefinitions.TurnOccurenceType.EndOfTurn, "Wisdom", "Wisdom", 15,
            new List<SaveAffinityBySenseDescription>
            {
                FeatureBuilder.BuildSaveAffinityBySense(SenseMode.Type.Darkvision,
                    RuleDefinitions.AdvantageType.Disadvantage),
                FeatureBuilder.BuildSaveAffinityBySense(SenseMode.Type.SuperiorDarkvision,
                    RuleDefinitions.AdvantageType.Disadvantage)
            }, condition1_1, "PowerOathOfRetributionZealousAccusation", presentationBuilder3.Build());


        var presentationBuilder4 = new GuiPresentationBuilder(
            "Rules/&ConditionTSZealousCondemnationTitle", "Rules/&ConditionTSZealousCondemnationDescription",
            ConditionDefinitions.ConditionGuided.GuiPresentation
                .SpriteReference);

        var condition1_2 = FeatureBuilder.BuildCondition(
            new List<FeatureDefinition> {FeatureDefinitionCombatAffinitys.CombatAffinityTrueStrike},
            RuleDefinitions.DurationType.Minute, 1,
            new List<RuleDefinitions.ConditionInterruption> {RuleDefinitions.ConditionInterruption.None},
            "ConditionTrueStrikeZealousCondemnation", presentationBuilder4.Build());

        var presentationBuilder5 = new GuiPresentationBuilder(
            "Feature/&PowerOathOfRetributionZealousCondemnationTitle",
            "Feature/&PowerOathOfRetributionZealousCondemnationDescription",
            FeatureDefinitionPowers.PowerOathOfTirmarSmiteTheHidden
                .GuiPresentation.SpriteReference);

        var feature3 = FeatureBuilder.BuildActionConditionPowerPaladinCD2(1, RuleDefinitions.UsesDetermination.Fixed,
            "Wisdom", RuleDefinitions.ActivationTime.BonusAction, 1, RuleDefinitions.RechargeRate.ChannelDivinity,
            RuleDefinitions.RangeType.Distance, 2, RuleDefinitions.TargetType.Individuals,
            ActionDefinitions.ItemSelectionType.None, RuleDefinitions.DurationType.Minute, 1,
            RuleDefinitions.TurnOccurenceType.EndOfTurn, condition1_2, "PowerOathOfRetributionZealousCondemnation",
            presentationBuilder5.Build());

        var presentationBuilder6 = new GuiPresentationBuilder(
            "Rules/&ConditionBonusRushTenaciousPursuitTitle", "Rules/&ConditionBonusRushTenaciousPursuitDescription",
            ConditionDefinitions.ConditionHasted.GuiPresentation
                .SpriteReference);

        var condition = FeatureBuilder.BuildBuffCondition(
            new List<FeatureDefinition>
            {
                FeatureDefinitionCombatAffinitys.CombatAffinityDisengaging,
                FeatureDefinitionActionAffinitys.ActionAffinityExpeditiousRetreat,
                FeatureDefinitionAdditionalActions.AdditionalActionExpeditiousRetreat
            }, RuleDefinitions.DurationType.Round, 1, RuleDefinitions.TurnOccurenceType.StartOfTurn,
            new List<RuleDefinitions.ConditionInterruption> {RuleDefinitions.ConditionInterruption.None},
            "ConditionBonusRushTenaciousPursuitGui", presentationBuilder6.Build());

        var presentationBuilder7 = new GuiPresentationBuilder(
            "Feature/&PowerOathOfRetributionTenaciousPursuitTitle",
            "Feature/&PowerOathOfRetributionTenaciousPursuitDescription",
            FeatureDefinitionPowers.PowerDomainBattleDecisiveStrike
                .GuiPresentation.SpriteReference);


        var feature4 = FeatureBuilder.BuildBonusMoveAfterHitPower(RuleDefinitions.ActivationTime.OnAttackHit,
            RuleDefinitions.RechargeRate.LongRest, 1, "Wisdom", 5, "Charisma",
            "Feature/&PowerOathOfRetributionTenaciousPursuitShortTitle", RuleDefinitions.TargetType.Self,
            ActionDefinitions.ItemSelectionType.Equiped, RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Self,
            RuleDefinitions.DurationType.Round, 1, RuleDefinitions.TurnOccurenceType.StartOfTurn, condition,
            "PowerOathOfRetributionTenaciousPursuit", presentationBuilder7.Build());

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("PaladinOathOfRetribution", SubclassNamespace)
            .SetGuiPresentation(Category.Subclass,
                CharacterSubclassDefinitions.DomainBattle.GuiPresentation.SpriteReference)
            .AddFeaturesAtLevel(3,
                paladinOathOfRetributionAutoPreparedSpells,
                feature2,
                feature3)
            .AddFeaturesAtLevel(7, feature4)
            .AddToDB();
    }

    internal override FeatureDefinitionSubclassChoice GetSubclassChoiceList()
    {
        return FeatureDefinitionSubclassChoices.SubclassChoicePaladinSacredOaths;
    }

    internal override CharacterSubclassDefinition GetSubclass()
    {
        return Subclass;
    }
}

internal class FeatureBuilder
{
    public static SaveAffinityBySenseDescription BuildSaveAffinityBySense(
        SenseMode.Type senseType,
        RuleDefinitions.AdvantageType advantageType)
    {
        var root = new SaveAffinityBySenseDescription {senseType = senseType, advantageType = advantageType};
        
        return root;
    }

    public static FeatureDefinitionPower BuildActionConditionPowerPaladinCD1(
        int usesPerRecharge,
        RuleDefinitions.UsesDetermination usesDetermination,
        string usesAbilityScoreName,
        RuleDefinitions.ActivationTime activationTime,
        int costPerUse,
        RuleDefinitions.RechargeRate recharge,
        RuleDefinitions.RangeType rangeType,
        int rangeParameter,
        RuleDefinitions.TargetType targetType,
        ActionDefinitions.ItemSelectionType itemSelectionType,
        RuleDefinitions.DurationType durationType,
        int durationParameter,
        RuleDefinitions.TurnOccurenceType endOfEffect,
        string savethrowtype,
        string savethrowDA,
        int savethrowDC,
        List<SaveAffinityBySenseDescription> saveAffinityBySenseDescriptions,
        ConditionDefinition condition1,
        string name,
        GuiPresentation guiPresentation)
    {
        var instance = ScriptableObject.CreateInstance<FeatureDefinitionPower>();
        instance.fixedUsesPerRecharge = usesPerRecharge;
        instance.usesDetermination = usesDetermination;
        instance.activationTime = activationTime;
        instance.costPerUse = costPerUse;
        instance.rechargeRate = recharge;
        instance.usesAbilityScoreName = usesAbilityScoreName;
        instance.uniqueInstance = true;
        var root1 = new EffectDescription();
        root1.targetSide = RuleDefinitions.Side.Enemy;
        root1.createdByCharacter = true;
        root1.rangeType = rangeType;
        root1.rangeParameter = rangeParameter;
        root1.targetType = targetType;
        root1.itemSelectionType = itemSelectionType;
        root1.canBePlacedOnCharacter = true;
        root1.durationType = durationType;
        root1.durationParameter = durationParameter;
        root1.endOfEffect = endOfEffect;
        root1.hasSavingThrow = true;
        root1.savingThrowAbility = savethrowtype;
        root1.createdByCharacter = true;
        root1.savingThrowDifficultyAbility = savethrowDA;
        root1.fixedSavingThrowDifficultyClass = savethrowDC;
        foreach (var senseDescription in saveAffinityBySenseDescriptions)
        {
            root1.SavingThrowAffinitiesBySense.Add(senseDescription);
        }

        var root2 = new EffectForm {FormType = EffectForm.EffectFormType.Condition};
        root2.formType = (EffectForm.EffectFormType)2;
        root2.createdByCharacter = true;
        var root3 = new ConditionForm();
        root3.operation = ConditionForm.ConditionOperation.Add;
        root3.conditionDefinition = condition1;
        root3.conditionDefinitionName = condition1.Name;
        root2.conditionForm = root3;
        root1.EffectForms.Add(root2);
        var effectAdvancement = new EffectAdvancement();
        root1.effectAdvancement = effectAdvancement;
        var particleParameters = new EffectParticleParameters();
        particleParameters.Copy(SpellDefinitions.Fear.EffectDescription.EffectParticleParameters);
        root1.effectParticleParameters = particleParameters;
        instance.effectDescription = root1;
        instance.name = name;
        instance.name = name;
        instance.guiPresentation = guiPresentation;
        Traverse.Create(instance).Field("guid")
            .SetValue(GuidHelper.Create(OathOfRetribution.ModGuidNamespace, name).ToString());
        DatabaseRepository.GetDatabase<FeatureDefinition>().Add(instance);
        return instance;
    }

    public static FeatureDefinitionPower BuildActionConditionPowerPaladinCD2(
        int usesPerRecharge,
        RuleDefinitions.UsesDetermination usesDetermination,
        string usesAbilityScoreName,
        RuleDefinitions.ActivationTime activationTime,
        int costPerUse,
        RuleDefinitions.RechargeRate recharge,
        RuleDefinitions.RangeType rangeType,
        int rangeParameter,
        RuleDefinitions.TargetType targetType,
        ActionDefinitions.ItemSelectionType itemSelectionType,
        RuleDefinitions.DurationType durationType,
        int durationParameter,
        RuleDefinitions.TurnOccurenceType endOfEffect,
        ConditionDefinition condition1,
        string name,
        GuiPresentation guiPresentation)
    {
        var instance = ScriptableObject.CreateInstance<FeatureDefinitionPower>();
        instance.fixedUsesPerRecharge = usesPerRecharge;
        instance.usesDetermination = usesDetermination;
        instance.activationTime = activationTime;
        instance.costPerUse = costPerUse;
        instance.rechargeRate = recharge;
        instance.usesAbilityScoreName = usesAbilityScoreName;
        instance.uniqueInstance = true;
        var root1 = new EffectDescription();
        root1.targetSide = RuleDefinitions.Side.Enemy;
        root1.createdByCharacter = true;
        root1.rangeType = rangeType;
        root1.rangeParameter = rangeParameter;
        root1.targetType = targetType;
        root1.itemSelectionType = itemSelectionType;
        root1.canBePlacedOnCharacter = true;
        root1.durationType = durationType;
        root1.durationParameter = durationParameter;
        root1.endOfEffect = endOfEffect;
        var root2 = new EffectForm {FormType = EffectForm.EffectFormType.Condition};
        root2.formType = (EffectForm.EffectFormType)2;
        root2.createdByCharacter = true;
        var root3 = new ConditionForm();
        root3.operation = ConditionForm.ConditionOperation.Add;
        root3.conditionDefinition = condition1;
        root3.conditionDefinitionName = condition1.Name;
        root2.conditionForm = root3;
        root1.EffectForms.Add(root2);
        var effectAdvancement = new EffectAdvancement();
        root1.effectAdvancement = effectAdvancement;
        var particleParameters = new EffectParticleParameters();
        particleParameters.Copy(SpellDefinitions.TrueStrike.EffectDescription.EffectParticleParameters);
        root1.effectParticleParameters = particleParameters;
        instance.effectDescription = root1;
        instance.name = name;
        instance.name = name;
        instance.guiPresentation = guiPresentation;
        Traverse.Create(instance).Field("guid")
            .SetValue(GuidHelper.Create(OathOfRetribution.ModGuidNamespace, name).ToString());
        DatabaseRepository.GetDatabase<FeatureDefinition>().Add(instance);
        return instance;
    }

    public static FeatureDefinitionPower BuildBonusMoveAfterHitPower(
        RuleDefinitions.ActivationTime activationTime,
        RuleDefinitions.RechargeRate rechargerate,
        int costPerUse,
        string usesAbilityScoreName,
        int fixedusesPerRecharge,
        string abilityScoreName,
        string shorttitle,
        RuleDefinitions.TargetType targetType,
        ActionDefinitions.ItemSelectionType itemSelectionType,
        RuleDefinitions.Side targetside,
        RuleDefinitions.RangeType rangetype,
        RuleDefinitions.DurationType durationType,
        int durationParameter,
        RuleDefinitions.TurnOccurenceType turnoccurence,
        ConditionDefinition condition,
        string name,
        GuiPresentation guiPresentation)
    {
        var instance = ScriptableObject.CreateInstance<FeatureDefinitionPower>();
        instance.activationTime = activationTime;
        instance.rechargeRate = rechargerate;
        instance.costPerUse = costPerUse;
        instance.usesAbilityScoreName = usesAbilityScoreName;
        instance.fixedUsesPerRecharge = fixedusesPerRecharge;
        instance.abilityScore = abilityScoreName;
        instance.showCasting = true;
        instance.uniqueInstance = true;
        instance.shortTitleOverride = shorttitle;
        var root1 = new EffectDescription();
        root1.targetType = targetType;
        root1.itemSelectionType = itemSelectionType;
        root1.targetSide = targetside;
        root1.createdByCharacter = true;
        root1.rangeType = rangetype;
        root1.canBePlacedOnCharacter = true;
        root1.durationType = durationType;
        root1.durationParameter = durationParameter;
        root1.endOfEffect = turnoccurence;
        var root2 = new EffectForm {FormType = EffectForm.EffectFormType.Condition};
        root2.createdByCharacter = true;
        var root3 = new ConditionForm();
        root3.operation = ConditionForm.ConditionOperation.Add;
        root3.conditionDefinition = condition;
        root3.conditionDefinitionName = condition.Name;
        root3.applyToSelf = true;
        root2.conditionForm = root3;
        root1.EffectForms.Add(root2);
        var root4 = new EffectAdvancement();
        Traverse.Create(root4).Field("incrementMultiplier").SetValue(1);
        root1.effectAdvancement = root4;
        var particleParameters = new EffectParticleParameters();
        particleParameters.Copy(FeatureDefinitionPowers.PowerDomainLawHolyRetribution.EffectDescription
            .EffectParticleParameters);
        root1.effectParticleParameters = particleParameters;
        instance.effectDescription = root1;
        instance.name = name;
        instance.name = name;
        instance.guiPresentation = guiPresentation;
        Traverse.Create(instance).Field("guid")
            .SetValue(GuidHelper.Create(OathOfRetribution.ModGuidNamespace, name).ToString());
        DatabaseRepository.GetDatabase<FeatureDefinition>().Add(instance);
        return instance;
    }

    public static FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup BuildAutoPreparedSpellGroup(
        int classLevel,
        List<SpellDefinition> spellnames)
    {
        var root = new FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup();
        Traverse.Create(root).Field(nameof(classLevel)).SetValue(classLevel);
        Traverse.Create(root).Field("spellsList").SetValue(new List<SpellDefinition>());
        foreach (var spellname in spellnames)
        {
            root.SpellsList.Add(spellname);
        }

        return root;
    }

    public static FeatureDefinitionAutoPreparedSpells BuildPaladinAutoPreparedSpellGroup(
        List<FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup> autospelllists,
        CharacterClassDefinition characterclass,
        string name,
        GuiPresentation guiPresentation)
    {
        var instance = ScriptableObject.CreateInstance<FeatureDefinitionAutoPreparedSpells>();
        Traverse.Create(instance).Field("autoPreparedSpellsGroups")
            .SetValue(new List<FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup>());
        foreach (var autospelllist in autospelllists)
        {
            instance.AutoPreparedSpellsGroups.Add(autospelllist);
        }

        instance.spellcastingClass = characterclass;
        instance.contentCopyright = (BaseDefinition.Copyright)4;
        instance.name = name;
        instance.name = name;
        instance.guiPresentation = guiPresentation;
        Traverse.Create(instance).Field("guid")
            .SetValue(GuidHelper.Create(OathOfRetribution.ModGuidNamespace, name).ToString());
        DatabaseRepository.GetDatabase<FeatureDefinition>().Add(instance);
        return instance;
    }

    public static ConditionDefinition BuildCondition(
        List<FeatureDefinition> conditionFeatures,
        RuleDefinitions.DurationType durationType,
        int durationParameter,
        List<RuleDefinitions.ConditionInterruption> conditionInterruptions,
        string name,
        GuiPresentation guiPresentation)
    {
        var instance = ScriptableObject.CreateInstance<ConditionDefinition>();
        foreach (var conditionFeature in conditionFeatures)
        {
            instance.Features.Add(conditionFeature);
        }

        instance.conditionType = RuleDefinitions.ConditionType.Detrimental;
        instance.allowMultipleInstances = false;
        instance.durationType = durationType;
        instance.durationParameter = durationParameter;
        foreach (var conditionInterruption in conditionInterruptions)
        {
            instance.SpecialInterruptions.Add(conditionInterruption);
        }

        var assetReference = new AssetReference();
        instance.conditionStartParticleReference = assetReference;
        instance.conditionParticleReference = assetReference;
        instance.conditionEndParticleReference = assetReference;
        instance.characterShaderReference = assetReference;
        instance.recurrentEffectForms = new List<EffectForm>();
        instance.cancellingConditions = new List<ConditionDefinition>();
        instance.guiPresentation = guiPresentation;
        instance.name = name;
        instance.name = name;
        Traverse.Create(instance).Field("guid")
            .SetValue(GuidHelper.Create(OathOfRetribution.ModGuidNamespace, name).ToString());
        DatabaseRepository.GetDatabase<ConditionDefinition>().Add(instance);
        return instance;
    }

    public static ConditionDefinition BuildBuffCondition(
        List<FeatureDefinition> conditionFeatures,
        RuleDefinitions.DurationType durationType,
        int durationParameter,
        RuleDefinitions.TurnOccurenceType turnoccurence,
        List<RuleDefinitions.ConditionInterruption> conditionInterruptions,
        string name,
        GuiPresentation guiPresentation)
    {
        var instance = ScriptableObject.CreateInstance<ConditionDefinition>();
        foreach (var conditionFeature in conditionFeatures)
        {
            instance.Features.Add(conditionFeature);
        }

        instance.conditionType = RuleDefinitions.ConditionType.Beneficial;
        instance.allowMultipleInstances = false;
        instance.durationType = durationType;
        instance.durationParameter = durationParameter;
        instance.turnOccurence = turnoccurence;
        foreach (var conditionInterruption in conditionInterruptions)
        {
            instance.SpecialInterruptions.Add(conditionInterruption);
        }

        var assetReference = new AssetReference();
        instance.conditionStartParticleReference = assetReference;
        instance.conditionParticleReference = assetReference;
        instance.conditionEndParticleReference = assetReference;
        instance.characterShaderReference = assetReference;
        instance.recurrentEffectForms = new List<EffectForm>();
        instance.cancellingConditions = new List<ConditionDefinition>();
        instance.guiPresentation = guiPresentation;
        instance.name = name;
        instance.name = name;
        Traverse.Create(instance).Field("guid")
            .SetValue(GuidHelper.Create(OathOfRetribution.ModGuidNamespace, name).ToString());
        DatabaseRepository.GetDatabase<ConditionDefinition>().Add(instance);
        return instance;
    }
}
