using System;
using System.Collections.Generic;
using HarmonyLib;
using SolastaCommunityExpansion.Api;
using SolastaCommunityExpansion.Builders;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SolastaCommunityExpansion.Subclasses.Paladin;

internal class SubClassBuilder
{
    private readonly CharacterSubclassDefinition MyClass;

    public SubClassBuilder()
    {
        MyClass = ScriptableObject.CreateInstance<CharacterSubclassDefinition>();
    }

    public void SetName(string name)
    {
        Traverse.Create(MyClass).Field(nameof(name)).SetValue(name);
        MyClass.name = name;
        Traverse.Create(MyClass).Field("guid").SetValue(GuidHelper.Create(OathOfRetribution.ModGuidNamespace, name).ToString());
    }

    public void SetGuiPresentation(GuiPresentation gui)
    {
        Traverse.Create(MyClass).Field("guiPresentation").SetValue(gui);
    }

    public void AddFeatureAtLevel(FeatureDefinition feature, int level)
    {
        MyClass.FeatureUnlocks.Add(new FeatureUnlockByLevel(feature, level));
    }

    public CharacterSubclassDefinition AddToDB()
    {
        DatabaseRepository.GetDatabase<CharacterSubclassDefinition>().Add(MyClass);
        return MyClass;
    }
}

internal static class OathOfRetribution
{
    public static Guid ModGuidNamespace = new("1e13fc7e-08ab-42d1-ba98-f7854b3f58ea");

    private static void ModifyDatabase()
    {
        var subClassBuilder = new SubClassBuilder();
        subClassBuilder.SetName("OathOfRetribution");
        var presentationBuilder1 = new GuiPresentationBuilder(
            "Subclass/&OathOfRetributionTitle", "Subclass/&OathOfRetributionDescription");
        presentationBuilder1.SetSpriteReference(DatabaseHelper.CharacterSubclassDefinitions.DomainBattle.GuiPresentation
            .SpriteReference);
        subClassBuilder.SetGuiPresentation(presentationBuilder1.Build());
        var feature1 = FeatureBuilder.BuildPaladinAutoPreparedSpellGroup(
            new List<FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup>
            {
                FeatureBuilder.BuildAutoPreparedSpellGroup(3, new List<SpellDefinition>
                {
                    DatabaseHelper.SpellDefinitions.Bane,
                    DatabaseHelper.SpellDefinitions.HuntersMark
                }),
                FeatureBuilder.BuildAutoPreparedSpellGroup(5, new List<SpellDefinition>
                {
                    DatabaseHelper.SpellDefinitions.HoldPerson,
                    DatabaseHelper.SpellDefinitions.MistyStep
                }),
                FeatureBuilder.BuildAutoPreparedSpellGroup(6, new List<SpellDefinition>
                {
                    DatabaseHelper.SpellDefinitions.Haste,
                    DatabaseHelper.SpellDefinitions.ProtectionFromEnergy
                })
            }, DatabaseHelper.CharacterClassDefinitions.Paladin,
            "AutoPreparedSpellsOathOfRetribution",
            new GuiPresentationBuilder("Feature/&DomainSpellsTitle", "Feature/&DomainSpellsDescription").Build());
        subClassBuilder.AddFeatureAtLevel(feature1, 3);
        var presentationBuilder2 = new GuiPresentationBuilder(
            "Rules/&ConditionFrightenedZealousAccusationTitle",
            "Rules/&ConditionFrightenedZealousAccusationDescription");
        presentationBuilder2.SetSpriteReference(DatabaseHelper.ConditionDefinitions.ConditionFrightened.GuiPresentation
            .SpriteReference);
        var condition1_1 = FeatureBuilder.BuildCondition(new List<FeatureDefinition>
        {
            DatabaseHelper.FeatureDefinitionCombatAffinitys.CombatAffinityFrightened,
            DatabaseHelper.FeatureDefinitionMovementAffinitys.MovementAffinityConditionRestrained
        }, RuleDefinitions.DurationType.Minute, 1, new List<RuleDefinitions.ConditionInterruption>
        {
            RuleDefinitions.ConditionInterruption.Damaged,
            RuleDefinitions.ConditionInterruption.DamagedByFriendly
        }, "ConditionFrightenedZealousAccusation", presentationBuilder2.Build());
        var presentationBuilder3 = new GuiPresentationBuilder(
            "Feature/&PowerOathOfRetributionZealousAccusationTitle",
            "Feature/&PowerOathOfRetributionZealousAccusationDescription");
        presentationBuilder3.SetSpriteReference(DatabaseHelper.FeatureDefinitionPowers.PowerDomainLawHolyRetribution
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
        subClassBuilder.AddFeatureAtLevel(feature2, 3);
        var presentationBuilder4 = new GuiPresentationBuilder(
            "Rules/&ConditionTSZealousCondemnationTitle", "Rules/&ConditionTSZealousCondemnationDescription");
        presentationBuilder4.SetSpriteReference(DatabaseHelper.ConditionDefinitions.ConditionGuided.GuiPresentation
            .SpriteReference);
        var condition1_2 = FeatureBuilder.BuildCondition(new List<FeatureDefinition>
        {
            DatabaseHelper.FeatureDefinitionCombatAffinitys.CombatAffinityTrueStrike
        }, RuleDefinitions.DurationType.Minute, 1, new List<RuleDefinitions.ConditionInterruption>
        {
            RuleDefinitions.ConditionInterruption.None
        }, "ConditionTrueStrikeZealousCondemnation", presentationBuilder4.Build());
        var presentationBuilder5 = new GuiPresentationBuilder(
            "Feature/&PowerOathOfRetributionZealousCondemnationTitle",
            "Feature/&PowerOathOfRetributionZealousCondemnationDescription");
        presentationBuilder5.SetSpriteReference(DatabaseHelper.FeatureDefinitionPowers.PowerOathOfTirmarSmiteTheHidden
            .GuiPresentation.SpriteReference);
        var feature3 = FeatureBuilder.BuildActionConditionPowerPaladinCD2(1, RuleDefinitions.UsesDetermination.Fixed,
            "Wisdom", RuleDefinitions.ActivationTime.BonusAction, 1, RuleDefinitions.RechargeRate.ChannelDivinity,
            RuleDefinitions.RangeType.Distance, 2, RuleDefinitions.TargetType.Individuals,
            ActionDefinitions.ItemSelectionType.None, RuleDefinitions.DurationType.Minute, 1,
            RuleDefinitions.TurnOccurenceType.EndOfTurn, condition1_2, "PowerOathOfRetributionZealousCondemnation",
            presentationBuilder5.Build());
        subClassBuilder.AddFeatureAtLevel(feature3, 3);
        var presentationBuilder6 = new GuiPresentationBuilder(
            "Rules/&ConditionBonusRushTenaciousPursuitTitle", "Rules/&ConditionBonusRushTenaciousPursuitDescription");
        presentationBuilder6.SetSpriteReference(DatabaseHelper.ConditionDefinitions.ConditionHasted.GuiPresentation
            .SpriteReference);
        var condition = FeatureBuilder.BuildBuffCondition(new List<FeatureDefinition>
            {
                DatabaseHelper.FeatureDefinitionCombatAffinitys.CombatAffinityDisengaging,
                DatabaseHelper.FeatureDefinitionActionAffinitys.ActionAffinityExpeditiousRetreat,
                DatabaseHelper.FeatureDefinitionAdditionalActions.AdditionalActionExpeditiousRetreat
            }, RuleDefinitions.DurationType.Round, 1, RuleDefinitions.TurnOccurenceType.StartOfTurn,
            new List<RuleDefinitions.ConditionInterruption>
            {
                RuleDefinitions.ConditionInterruption.None
            }, "ConditionBonusRushTenaciousPursuitGui", presentationBuilder6.Build());
        var presentationBuilder7 = new GuiPresentationBuilder(
            "Feature/&PowerOathOfRetributionTenaciousPursuitTitle",
            "Feature/&PowerOathOfRetributionTenaciousPursuitDescription");
        presentationBuilder7.SetSpriteReference(DatabaseHelper.FeatureDefinitionPowers.PowerDomainBattleDecisiveStrike
            .GuiPresentation.SpriteReference);

        var feature4 = FeatureBuilder.BuildBonusMoveAfterHitPower(RuleDefinitions.ActivationTime.OnAttackHit,
            RuleDefinitions.RechargeRate.LongRest, 1, "Wisdom", 5, "Charisma",
            "Feature/&PowerOathOfRetributionTenaciousPursuitShortTitle", RuleDefinitions.TargetType.Self,
            ActionDefinitions.ItemSelectionType.Equiped, RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Self,
            RuleDefinitions.DurationType.Round, 1, RuleDefinitions.TurnOccurenceType.StartOfTurn, condition,
            "PowerOathOfRetributionTenaciousPursuit", presentationBuilder7.Build());
        subClassBuilder.AddFeatureAtLevel(feature4, 7);
        DatabaseHelper.FeatureDefinitionSubclassChoices.SubclassChoicePaladinSacredOaths.Subclasses.Add(subClassBuilder
            .AddToDB().Name);
    }
}

internal class FeatureBuilder
{
    public static SaveAffinityBySenseDescription BuildSaveAffinityBySense(
        SenseMode.Type senseType,
        RuleDefinitions.AdvantageType advantageType)
    {
        var root = new SaveAffinityBySenseDescription();
        Traverse.Create(root).Field(nameof(senseType)).SetValue(senseType);
        Traverse.Create(root).Field(nameof(advantageType)).SetValue(advantageType);
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
        Traverse.Create(instance).Field("fixedUsesPerRecharge").SetValue(usesPerRecharge);
        Traverse.Create(instance).Field(nameof(usesDetermination)).SetValue(usesDetermination);
        Traverse.Create(instance).Field(nameof(activationTime)).SetValue(activationTime);
        Traverse.Create(instance).Field(nameof(costPerUse)).SetValue(costPerUse);
        Traverse.Create(instance).Field("rechargeRate").SetValue(recharge);
        Traverse.Create(instance).Field(nameof(usesAbilityScoreName)).SetValue(usesAbilityScoreName);
        Traverse.Create(instance).Field("uniqueInstance").SetValue(true);
        var root1 = new EffectDescription();
        Traverse.Create(root1).Field("targetSide").SetValue(RuleDefinitions.Side.Enemy);
        Traverse.Create(root1).Field("createdByCharacter").SetValue(true);
        Traverse.Create(root1).Field(nameof(rangeType)).SetValue(rangeType);
        Traverse.Create(root1).Field(nameof(rangeParameter)).SetValue(rangeParameter);
        Traverse.Create(root1).Field(nameof(targetType)).SetValue(targetType);
        Traverse.Create(root1).Field(nameof(itemSelectionType)).SetValue(itemSelectionType);
        Traverse.Create(root1).Field("canBePlacedOnCharacter").SetValue(true);
        Traverse.Create(root1).Field(nameof(durationType)).SetValue(durationType);
        Traverse.Create(root1).Field(nameof(durationParameter)).SetValue(durationParameter);
        Traverse.Create(root1).Field(nameof(endOfEffect)).SetValue(endOfEffect);
        Traverse.Create(root1).Field("hasSavingThrow").SetValue(true);
        Traverse.Create(root1).Field("savingThrowAbility").SetValue(savethrowtype);
        Traverse.Create(root1).Field("createdByCharacter").SetValue(true);
        Traverse.Create(root1).Field("savingThrowDifficultyAbility").SetValue(savethrowDA);
        Traverse.Create(root1).Field("fixedSavingThrowDifficultyClass").SetValue(savethrowDC);
        foreach (var senseDescription in saveAffinityBySenseDescriptions)
            root1.SavingThrowAffinitiesBySense.Add(senseDescription);
        var root2 = new EffectForm();
        root2.FormType = EffectForm.EffectFormType.Condition;
        Traverse.Create(root2).Field("formType").SetValue(2);
        Traverse.Create(root2).Field("createdByCharacter").SetValue(true);
        var root3 = new ConditionForm();
        Traverse.Create(root3).Field("operation").SetValue(ConditionForm.ConditionOperation.Add);
        Traverse.Create(root3).Field("conditionDefinition").SetValue(condition1);
        Traverse.Create(root3).Field("conditionDefinitionName").SetValue(condition1.Name);
        Traverse.Create(root2).Field("conditionForm").SetValue(root3);
        root1.EffectForms.Add(root2);
        var effectAdvancement = new EffectAdvancement();
        Traverse.Create(root1).Field("effectAdvancement").SetValue(effectAdvancement);
        var particleParameters = new EffectParticleParameters();
        particleParameters.Copy(DatabaseHelper.SpellDefinitions.Fear.EffectDescription.EffectParticleParameters);
        Traverse.Create(root1).Field("effectParticleParameters").SetValue(particleParameters);
        Traverse.Create(instance).Field("effectDescription").SetValue(root1);
        Traverse.Create(instance).Field(nameof(name)).SetValue(name);
        instance.name = name;
        Traverse.Create(instance).Field(nameof(guiPresentation)).SetValue(guiPresentation);
        Traverse.Create(instance).Field("guid").SetValue(GuidHelper.Create(OathOfRetribution.ModGuidNamespace, name).ToString());
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
        Traverse.Create(instance).Field("fixedUsesPerRecharge").SetValue(usesPerRecharge);
        Traverse.Create(instance).Field(nameof(usesDetermination)).SetValue(usesDetermination);
        Traverse.Create(instance).Field(nameof(activationTime)).SetValue(activationTime);
        Traverse.Create(instance).Field(nameof(costPerUse)).SetValue(costPerUse);
        Traverse.Create(instance).Field("rechargeRate").SetValue(recharge);
        Traverse.Create(instance).Field(nameof(usesAbilityScoreName)).SetValue(usesAbilityScoreName);
        Traverse.Create(instance).Field("uniqueInstance").SetValue(true);
        var root1 = new EffectDescription();
        Traverse.Create(root1).Field("targetSide").SetValue(RuleDefinitions.Side.Enemy);
        Traverse.Create(root1).Field("createdByCharacter").SetValue(true);
        Traverse.Create(root1).Field(nameof(rangeType)).SetValue(rangeType);
        Traverse.Create(root1).Field(nameof(rangeParameter)).SetValue(rangeParameter);
        Traverse.Create(root1).Field(nameof(targetType)).SetValue(targetType);
        Traverse.Create(root1).Field(nameof(itemSelectionType)).SetValue(itemSelectionType);
        Traverse.Create(root1).Field("canBePlacedOnCharacter").SetValue(true);
        Traverse.Create(root1).Field(nameof(durationType)).SetValue(durationType);
        Traverse.Create(root1).Field(nameof(durationParameter)).SetValue(durationParameter);
        Traverse.Create(root1).Field(nameof(endOfEffect)).SetValue(endOfEffect);
        var root2 = new EffectForm();
        root2.FormType = EffectForm.EffectFormType.Condition;
        Traverse.Create(root2).Field("formType").SetValue(2);
        Traverse.Create(root2).Field("createdByCharacter").SetValue(true);
        var root3 = new ConditionForm();
        Traverse.Create(root3).Field("operation").SetValue(ConditionForm.ConditionOperation.Add);
        Traverse.Create(root3).Field("conditionDefinition").SetValue(condition1);
        Traverse.Create(root3).Field("conditionDefinitionName").SetValue(condition1.Name);
        Traverse.Create(root2).Field("conditionForm").SetValue(root3);
        root1.EffectForms.Add(root2);
        var effectAdvancement = new EffectAdvancement();
        Traverse.Create(root1).Field("effectAdvancement").SetValue(effectAdvancement);
        var particleParameters = new EffectParticleParameters();
        particleParameters.Copy(DatabaseHelper.SpellDefinitions.TrueStrike.EffectDescription.EffectParticleParameters);
        Traverse.Create(root1).Field("effectParticleParameters").SetValue(particleParameters);
        Traverse.Create(instance).Field("effectDescription").SetValue(root1);
        Traverse.Create(instance).Field(nameof(name)).SetValue(name);
        instance.name = name;
        Traverse.Create(instance).Field(nameof(guiPresentation)).SetValue(guiPresentation);
        Traverse.Create(instance).Field("guid").SetValue(GuidHelper.Create(OathOfRetribution.ModGuidNamespace, name).ToString());
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
        Traverse.Create(instance).Field(nameof(activationTime)).SetValue(activationTime);
        Traverse.Create(instance).Field("rechargeRate").SetValue(rechargerate);
        Traverse.Create(instance).Field(nameof(costPerUse)).SetValue(costPerUse);
        Traverse.Create(instance).Field(nameof(usesAbilityScoreName)).SetValue(usesAbilityScoreName);
        Traverse.Create(instance).Field("fixedUsesPerRecharge").SetValue(fixedusesPerRecharge);
        Traverse.Create(instance).Field("abilityScore").SetValue(abilityScoreName);
        Traverse.Create(instance).Field("showCasting").SetValue(true);
        Traverse.Create(instance).Field("uniqueInstance").SetValue(true);
        Traverse.Create(instance).Field("shortTitleOverride").SetValue(shorttitle);
        var root1 = new EffectDescription();
        Traverse.Create(root1).Field(nameof(targetType)).SetValue(targetType);
        Traverse.Create(root1).Field(nameof(itemSelectionType)).SetValue(itemSelectionType);
        Traverse.Create(root1).Field("targetSide").SetValue(targetside);
        Traverse.Create(root1).Field("createdByCharacter").SetValue(true);
        Traverse.Create(root1).Field("rangeType").SetValue(rangetype);
        Traverse.Create(root1).Field("canBePlacedOnCharacter").SetValue(true);
        Traverse.Create(root1).Field(nameof(durationType)).SetValue(durationType);
        Traverse.Create(root1).Field(nameof(durationParameter)).SetValue(durationParameter);
        Traverse.Create(root1).Field("endOfEffect").SetValue(turnoccurence);
        var root2 = new EffectForm();
        root2.FormType = EffectForm.EffectFormType.Condition;
        Traverse.Create(root2).Field("createdByCharacter").SetValue(true);
        var root3 = new ConditionForm();
        Traverse.Create(root3).Field("operation").SetValue(ConditionForm.ConditionOperation.Add);
        Traverse.Create(root3).Field("conditionDefinition").SetValue(condition);
        Traverse.Create(root3).Field("conditionDefinitionName").SetValue(condition.Name);
        Traverse.Create(root3).Field("applyToSelf").SetValue(true);
        Traverse.Create(root2).Field("conditionForm").SetValue(root3);
        root1.EffectForms.Add(root2);
        var root4 = new EffectAdvancement();
        Traverse.Create(root4).Field("incrementMultiplier").SetValue(1);
        Traverse.Create(root1).Field("effectAdvancement").SetValue(root4);
        var particleParameters = new EffectParticleParameters();
        particleParameters.Copy(DatabaseHelper.FeatureDefinitionPowers.PowerDomainLawHolyRetribution.EffectDescription
            .EffectParticleParameters);
        Traverse.Create(root1).Field("effectParticleParameters").SetValue(particleParameters);
        Traverse.Create(instance).Field("effectDescription").SetValue(root1);
        Traverse.Create(instance).Field(nameof(name)).SetValue(name);
        instance.name = name;
        Traverse.Create(instance).Field(nameof(guiPresentation)).SetValue(guiPresentation);
        Traverse.Create(instance).Field("guid").SetValue(GuidHelper.Create(OathOfRetribution.ModGuidNamespace, name).ToString());
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
            root.SpellsList.Add(spellname);
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
            instance.AutoPreparedSpellsGroups.Add(autospelllist);
        Traverse.Create(instance).Field("spellcastingClass").SetValue(characterclass);
        Traverse.Create(instance).Field("contentCopyright").SetValue(4);
        Traverse.Create(instance).Field(nameof(name)).SetValue(name);
        instance.name = name;
        Traverse.Create(instance).Field(nameof(guiPresentation)).SetValue(guiPresentation);
        Traverse.Create(instance).Field("guid").SetValue(GuidHelper.Create(OathOfRetribution.ModGuidNamespace, name).ToString());
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
            instance.Features.Add(conditionFeature);
        Traverse.Create(instance).Field("conditionType").SetValue(RuleDefinitions.ConditionType.Detrimental);
        Traverse.Create(instance).Field("allowMultipleInstances").SetValue(false);
        Traverse.Create(instance).Field(nameof(durationType)).SetValue(durationType);
        Traverse.Create(instance).Field(nameof(durationParameter)).SetValue(durationParameter);
        foreach (var conditionInterruption in conditionInterruptions)
            instance.SpecialInterruptions.Add(conditionInterruption);
        var assetReference = new AssetReference();
        Traverse.Create(instance).Field("conditionStartParticleReference").SetValue(assetReference);
        Traverse.Create(instance).Field("conditionParticleReference").SetValue(assetReference);
        Traverse.Create(instance).Field("conditionEndParticleReference").SetValue(assetReference);
        Traverse.Create(instance).Field("characterShaderReference").SetValue(assetReference);
        Traverse.Create(instance).Field("recurrentEffectForms").SetValue(new List<EffectForm>());
        Traverse.Create(instance).Field("cancellingConditions").SetValue(new List<ConditionDefinition>());
        Traverse.Create(instance).Field(nameof(guiPresentation)).SetValue(guiPresentation);
        Traverse.Create(instance).Field(nameof(name)).SetValue(name);
        instance.name = name;
        Traverse.Create(instance).Field("guid").SetValue(GuidHelper.Create(OathOfRetribution.ModGuidNamespace, name).ToString());
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
            instance.Features.Add(conditionFeature);
        Traverse.Create(instance).Field("conditionType").SetValue(RuleDefinitions.ConditionType.Beneficial);
        Traverse.Create(instance).Field("allowMultipleInstances").SetValue(false);
        Traverse.Create(instance).Field(nameof(durationType)).SetValue(durationType);
        Traverse.Create(instance).Field(nameof(durationParameter)).SetValue(durationParameter);
        Traverse.Create(instance).Field("turnOccurence").SetValue(turnoccurence);
        foreach (var conditionInterruption in conditionInterruptions)
            instance.SpecialInterruptions.Add(conditionInterruption);
        var assetReference = new AssetReference();
        Traverse.Create(instance).Field("conditionStartParticleReference").SetValue(assetReference);
        Traverse.Create(instance).Field("conditionParticleReference").SetValue(assetReference);
        Traverse.Create(instance).Field("conditionEndParticleReference").SetValue(assetReference);
        Traverse.Create(instance).Field("characterShaderReference").SetValue(assetReference);
        Traverse.Create(instance).Field("recurrentEffectForms").SetValue(new List<EffectForm>());
        Traverse.Create(instance).Field("cancellingConditions").SetValue(new List<ConditionDefinition>());
        Traverse.Create(instance).Field(nameof(guiPresentation)).SetValue(guiPresentation);
        Traverse.Create(instance).Field(nameof(name)).SetValue(name);
        instance.name = name;
        Traverse.Create(instance).Field("guid").SetValue(GuidHelper.Create(OathOfRetribution.ModGuidNamespace, name).ToString());
        DatabaseRepository.GetDatabase<ConditionDefinition>().Add(instance);
        return instance;
    }
}
