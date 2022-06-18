// Decompiled with JetBrains decompiler
// Type: SolastaModApi.FeatureBuilder
// Assembly: OathOfRetribution_springupdate, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E0C31DE3-014A-4A7A-8428-F8DEFE001310
// Assembly location: C:\Users\paulo\Downloads\OathOfRetribution.dll

using HarmonyLib;
using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.Api;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SolastaOathOfRetribution
{
  internal class FeatureBuilder
  {
    public static SaveAffinityBySenseDescription BuildSaveAffinityBySense(
      SenseMode.Type senseType,
      RuleDefinitions.AdvantageType advantageType)
    {
      SaveAffinityBySenseDescription root = new SaveAffinityBySenseDescription();
      Traverse.Create(root).Field(nameof (senseType)).SetValue(senseType);
      Traverse.Create(root).Field(nameof (advantageType)).SetValue(advantageType);
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
      global::ActionDefinitions.ItemSelectionType itemSelectionType,
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
      FeatureDefinitionPower instance = ScriptableObject.CreateInstance<FeatureDefinitionPower>();
      Traverse.Create(instance).Field("fixedUsesPerRecharge").SetValue(usesPerRecharge);
      Traverse.Create(instance).Field(nameof (usesDetermination)).SetValue(usesDetermination);
      Traverse.Create(instance).Field(nameof (activationTime)).SetValue(activationTime);
      Traverse.Create(instance).Field(nameof (costPerUse)).SetValue(costPerUse);
      Traverse.Create(instance).Field("rechargeRate").SetValue(recharge);
      Traverse.Create(instance).Field(nameof (usesAbilityScoreName)).SetValue(usesAbilityScoreName);
      Traverse.Create(instance).Field("uniqueInstance").SetValue(true);
      EffectDescription root1 = new EffectDescription();
      Traverse.Create(root1).Field("targetSide").SetValue(RuleDefinitions.Side.Enemy);
      Traverse.Create(root1).Field("createdByCharacter").SetValue(true);
      Traverse.Create(root1).Field(nameof (rangeType)).SetValue(rangeType);
      Traverse.Create(root1).Field(nameof (rangeParameter)).SetValue(rangeParameter);
      Traverse.Create(root1).Field(nameof (targetType)).SetValue(targetType);
      Traverse.Create(root1).Field(nameof (itemSelectionType)).SetValue(itemSelectionType);
      Traverse.Create(root1).Field("canBePlacedOnCharacter").SetValue(true);
      Traverse.Create(root1).Field(nameof (durationType)).SetValue(durationType);
      Traverse.Create(root1).Field(nameof (durationParameter)).SetValue(durationParameter);
      Traverse.Create(root1).Field(nameof (endOfEffect)).SetValue(endOfEffect);
      Traverse.Create(root1).Field("hasSavingThrow").SetValue(true);
      Traverse.Create(root1).Field("savingThrowAbility").SetValue(savethrowtype);
      Traverse.Create(root1).Field("createdByCharacter").SetValue(true);
      Traverse.Create(root1).Field("savingThrowDifficultyAbility").SetValue(savethrowDA);
      Traverse.Create(root1).Field("fixedSavingThrowDifficultyClass").SetValue(savethrowDC);
      foreach (SaveAffinityBySenseDescription senseDescription in saveAffinityBySenseDescriptions)
        root1.SavingThrowAffinitiesBySense.Add(senseDescription);
      EffectForm root2 = new EffectForm();
      root2.FormType = EffectForm.EffectFormType.Condition;
      Traverse.Create(root2).Field("formType").SetValue(2);
      Traverse.Create(root2).Field("createdByCharacter").SetValue(true);
      ConditionForm root3 = new ConditionForm();
      Traverse.Create(root3).Field("operation").SetValue(ConditionForm.ConditionOperation.Add);
      Traverse.Create(root3).Field("conditionDefinition").SetValue(condition1);
      Traverse.Create(root3).Field("conditionDefinitionName").SetValue(condition1.Name);
      Traverse.Create(root2).Field("conditionForm").SetValue(root3);
      root1.EffectForms.Add(root2);
      EffectAdvancement effectAdvancement = new EffectAdvancement();
      Traverse.Create(root1).Field("effectAdvancement").SetValue(effectAdvancement);
      EffectParticleParameters particleParameters = new EffectParticleParameters();
      particleParameters.Copy(DatabaseHelper.SpellDefinitions.Fear.EffectDescription.EffectParticleParameters);
      Traverse.Create(root1).Field("effectParticleParameters").SetValue(particleParameters);
      Traverse.Create(instance).Field("effectDescription").SetValue(root1);
      Traverse.Create(instance).Field(nameof (name)).SetValue(name);
      instance.name = name;
      Traverse.Create(instance).Field(nameof (guiPresentation)).SetValue(guiPresentation);
      Traverse.Create(instance).Field("guid").SetValue(GuidHelper.Create(Main.ModGuidNamespace, name).ToString());
      DatabaseRepository.GetDatabase<FeatureDefinition>().Add((FeatureDefinition) instance);
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
      global::ActionDefinitions.ItemSelectionType itemSelectionType,
      RuleDefinitions.DurationType durationType,
      int durationParameter,
      RuleDefinitions.TurnOccurenceType endOfEffect,
      ConditionDefinition condition1,
      string name,
      GuiPresentation guiPresentation)
    {
      FeatureDefinitionPower instance = ScriptableObject.CreateInstance<FeatureDefinitionPower>();
      Traverse.Create(instance).Field("fixedUsesPerRecharge").SetValue(usesPerRecharge);
      Traverse.Create(instance).Field(nameof (usesDetermination)).SetValue(usesDetermination);
      Traverse.Create(instance).Field(nameof (activationTime)).SetValue(activationTime);
      Traverse.Create(instance).Field(nameof (costPerUse)).SetValue(costPerUse);
      Traverse.Create(instance).Field("rechargeRate").SetValue(recharge);
      Traverse.Create(instance).Field(nameof (usesAbilityScoreName)).SetValue(usesAbilityScoreName);
      Traverse.Create(instance).Field("uniqueInstance").SetValue(true);
      EffectDescription root1 = new EffectDescription();
      Traverse.Create(root1).Field("targetSide").SetValue(RuleDefinitions.Side.Enemy);
      Traverse.Create(root1).Field("createdByCharacter").SetValue(true);
      Traverse.Create(root1).Field(nameof (rangeType)).SetValue(rangeType);
      Traverse.Create(root1).Field(nameof (rangeParameter)).SetValue(rangeParameter);
      Traverse.Create(root1).Field(nameof (targetType)).SetValue(targetType);
      Traverse.Create(root1).Field(nameof (itemSelectionType)).SetValue(itemSelectionType);
      Traverse.Create(root1).Field("canBePlacedOnCharacter").SetValue(true);
      Traverse.Create(root1).Field(nameof (durationType)).SetValue(durationType);
      Traverse.Create(root1).Field(nameof (durationParameter)).SetValue(durationParameter);
      Traverse.Create(root1).Field(nameof (endOfEffect)).SetValue(endOfEffect);
      EffectForm root2 = new EffectForm();
      root2.FormType = EffectForm.EffectFormType.Condition;
      Traverse.Create(root2).Field("formType").SetValue(2);
      Traverse.Create(root2).Field("createdByCharacter").SetValue(true);
      ConditionForm root3 = new ConditionForm();
      Traverse.Create(root3).Field("operation").SetValue(ConditionForm.ConditionOperation.Add);
      Traverse.Create(root3).Field("conditionDefinition").SetValue(condition1);
      Traverse.Create(root3).Field("conditionDefinitionName").SetValue(condition1.Name);
      Traverse.Create(root2).Field("conditionForm").SetValue(root3);
      root1.EffectForms.Add(root2);
      EffectAdvancement effectAdvancement = new EffectAdvancement();
      Traverse.Create(root1).Field("effectAdvancement").SetValue(effectAdvancement);
      EffectParticleParameters particleParameters = new EffectParticleParameters();
      particleParameters.Copy(DatabaseHelper.SpellDefinitions.TrueStrike.EffectDescription.EffectParticleParameters);
      Traverse.Create(root1).Field("effectParticleParameters").SetValue(particleParameters);
      Traverse.Create(instance).Field("effectDescription").SetValue(root1);
      Traverse.Create(instance).Field(nameof (name)).SetValue(name);
      instance.name = name;
      Traverse.Create(instance).Field(nameof (guiPresentation)).SetValue(guiPresentation);
      Traverse.Create(instance).Field("guid").SetValue(GuidHelper.Create(Main.ModGuidNamespace, name).ToString());
      DatabaseRepository.GetDatabase<FeatureDefinition>().Add((FeatureDefinition) instance);
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
      global::ActionDefinitions.ItemSelectionType itemSelectionType,
      RuleDefinitions.Side targetside,
      RuleDefinitions.RangeType rangetype,
      RuleDefinitions.DurationType durationType,
      int durationParameter,
      RuleDefinitions.TurnOccurenceType turnoccurence,
      ConditionDefinition condition,
      string name,
      GuiPresentation guiPresentation)
    {
      FeatureDefinitionPower instance = ScriptableObject.CreateInstance<FeatureDefinitionPower>();
      Traverse.Create(instance).Field(nameof (activationTime)).SetValue(activationTime);
      Traverse.Create(instance).Field("rechargeRate").SetValue(rechargerate);
      Traverse.Create(instance).Field(nameof (costPerUse)).SetValue(costPerUse);
      Traverse.Create(instance).Field(nameof (usesAbilityScoreName)).SetValue(usesAbilityScoreName);
      Traverse.Create(instance).Field("fixedUsesPerRecharge").SetValue(fixedusesPerRecharge);
      Traverse.Create(instance).Field("abilityScore").SetValue(abilityScoreName);
      Traverse.Create(instance).Field("showCasting").SetValue(true);
      Traverse.Create(instance).Field("uniqueInstance").SetValue(true);
      Traverse.Create(instance).Field("shortTitleOverride").SetValue(shorttitle);
      EffectDescription root1 = new EffectDescription();
      Traverse.Create(root1).Field(nameof (targetType)).SetValue(targetType);
      Traverse.Create(root1).Field(nameof (itemSelectionType)).SetValue(itemSelectionType);
      Traverse.Create(root1).Field("targetSide").SetValue(targetside);
      Traverse.Create(root1).Field("createdByCharacter").SetValue(true);
      Traverse.Create(root1).Field("rangeType").SetValue(rangetype);
      Traverse.Create(root1).Field("canBePlacedOnCharacter").SetValue(true);
      Traverse.Create(root1).Field(nameof (durationType)).SetValue(durationType);
      Traverse.Create(root1).Field(nameof (durationParameter)).SetValue(durationParameter);
      Traverse.Create(root1).Field("endOfEffect").SetValue(turnoccurence);
      EffectForm root2 = new EffectForm();
      root2.FormType = EffectForm.EffectFormType.Condition;
      Traverse.Create(root2).Field("createdByCharacter").SetValue(true);
      ConditionForm root3 = new ConditionForm();
      Traverse.Create(root3).Field("operation").SetValue(ConditionForm.ConditionOperation.Add);
      Traverse.Create(root3).Field("conditionDefinition").SetValue(condition);
      Traverse.Create(root3).Field("conditionDefinitionName").SetValue(condition.Name);
      Traverse.Create(root3).Field("applyToSelf").SetValue(true);
      Traverse.Create(root2).Field("conditionForm").SetValue(root3);
      root1.EffectForms.Add(root2);
      EffectAdvancement root4 = new EffectAdvancement();
      Traverse.Create(root4).Field("incrementMultiplier").SetValue(1);
      Traverse.Create(root1).Field("effectAdvancement").SetValue(root4);
      EffectParticleParameters particleParameters = new EffectParticleParameters();
      particleParameters.Copy(DatabaseHelper.FeatureDefinitionPowers.PowerDomainLawHolyRetribution.EffectDescription.EffectParticleParameters);
      Traverse.Create(root1).Field("effectParticleParameters").SetValue(particleParameters);
      Traverse.Create(instance).Field("effectDescription").SetValue(root1);
      Traverse.Create(instance).Field(nameof (name)).SetValue(name);
      instance.name = name;
      Traverse.Create(instance).Field(nameof (guiPresentation)).SetValue(guiPresentation);
      Traverse.Create(instance).Field("guid").SetValue(GuidHelper.Create(Main.ModGuidNamespace, name).ToString());
      DatabaseRepository.GetDatabase<FeatureDefinition>().Add((FeatureDefinition) instance);
      return instance;
    }

    public static FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup BuildAutoPreparedSpellGroup(
      int classLevel,
      List<SpellDefinition> spellnames)
    {
      FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup root = new FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup();
      Traverse.Create(root).Field(nameof (classLevel)).SetValue(classLevel);
      Traverse.Create(root).Field("spellsList").SetValue(new List<SpellDefinition>());
      foreach (SpellDefinition spellname in spellnames)
        root.SpellsList.Add(spellname);
      return root;
    }

    public static FeatureDefinitionAutoPreparedSpells BuildPaladinAutoPreparedSpellGroup(
      List<FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup> autospelllists,
      CharacterClassDefinition characterclass,
      string name,
      GuiPresentation guiPresentation)
    {
      FeatureDefinitionAutoPreparedSpells instance = ScriptableObject.CreateInstance<FeatureDefinitionAutoPreparedSpells>();
      Traverse.Create(instance).Field("autoPreparedSpellsGroups").SetValue(new List<FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup>());
      foreach (FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup autospelllist in autospelllists)
        instance.AutoPreparedSpellsGroups.Add(autospelllist);
      Traverse.Create(instance).Field("spellcastingClass").SetValue(characterclass);
      Traverse.Create(instance).Field("contentCopyright").SetValue(4);
      Traverse.Create(instance).Field(nameof (name)).SetValue(name);
      instance.name = name;
      Traverse.Create(instance).Field(nameof (guiPresentation)).SetValue(guiPresentation);
      Traverse.Create(instance).Field("guid").SetValue(GuidHelper.Create(Main.ModGuidNamespace, name).ToString());
      DatabaseRepository.GetDatabase<FeatureDefinition>().Add((FeatureDefinition) instance);
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
      ConditionDefinition instance = ScriptableObject.CreateInstance<ConditionDefinition>();
      foreach (FeatureDefinition conditionFeature in conditionFeatures)
        instance.Features.Add(conditionFeature);
      Traverse.Create(instance).Field("conditionType").SetValue(RuleDefinitions.ConditionType.Detrimental);
      Traverse.Create(instance).Field("allowMultipleInstances").SetValue(false);
      Traverse.Create(instance).Field(nameof (durationType)).SetValue(durationType);
      Traverse.Create(instance).Field(nameof (durationParameter)).SetValue(durationParameter);
      foreach (RuleDefinitions.ConditionInterruption conditionInterruption in conditionInterruptions)
        instance.SpecialInterruptions.Add(conditionInterruption);
      AssetReference assetReference = new AssetReference();
      Traverse.Create(instance).Field("conditionStartParticleReference").SetValue(assetReference);
      Traverse.Create(instance).Field("conditionParticleReference").SetValue(assetReference);
      Traverse.Create(instance).Field("conditionEndParticleReference").SetValue(assetReference);
      Traverse.Create(instance).Field("characterShaderReference").SetValue(assetReference);
      Traverse.Create(instance).Field("recurrentEffectForms").SetValue(new List<EffectForm>());
      Traverse.Create(instance).Field("cancellingConditions").SetValue(new List<ConditionDefinition>());
      Traverse.Create(instance).Field(nameof (guiPresentation)).SetValue(guiPresentation);
      Traverse.Create(instance).Field(nameof (name)).SetValue(name);
      instance.name = name;
      Traverse.Create(instance).Field("guid").SetValue(GuidHelper.Create(Main.ModGuidNamespace, name).ToString());
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
      ConditionDefinition instance = ScriptableObject.CreateInstance<ConditionDefinition>();
      foreach (FeatureDefinition conditionFeature in conditionFeatures)
        instance.Features.Add(conditionFeature);
      Traverse.Create(instance).Field("conditionType").SetValue(RuleDefinitions.ConditionType.Beneficial);
      Traverse.Create(instance).Field("allowMultipleInstances").SetValue(false);
      Traverse.Create(instance).Field(nameof (durationType)).SetValue(durationType);
      Traverse.Create(instance).Field(nameof (durationParameter)).SetValue(durationParameter);
      Traverse.Create(instance).Field("turnOccurence").SetValue(turnoccurence);
      foreach (RuleDefinitions.ConditionInterruption conditionInterruption in conditionInterruptions)
        instance.SpecialInterruptions.Add(conditionInterruption);
      AssetReference assetReference = new AssetReference();
      Traverse.Create(instance).Field("conditionStartParticleReference").SetValue(assetReference);
      Traverse.Create(instance).Field("conditionParticleReference").SetValue(assetReference);
      Traverse.Create(instance).Field("conditionEndParticleReference").SetValue(assetReference);
      Traverse.Create(instance).Field("characterShaderReference").SetValue(assetReference);
      Traverse.Create(instance).Field("recurrentEffectForms").SetValue(new List<EffectForm>());
      Traverse.Create(instance).Field("cancellingConditions").SetValue(new List<ConditionDefinition>());
      Traverse.Create(instance).Field(nameof (guiPresentation)).SetValue(guiPresentation);
      Traverse.Create(instance).Field(nameof (name)).SetValue(name);
      instance.name = name;
      Traverse.Create(instance).Field("guid").SetValue(GuidHelper.Create(Main.ModGuidNamespace, name).ToString());
      DatabaseRepository.GetDatabase<ConditionDefinition>().Add(instance);
      return instance;
    }
  }
}
