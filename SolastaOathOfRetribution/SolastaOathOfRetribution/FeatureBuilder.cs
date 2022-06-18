// Decompiled with JetBrains decompiler
// Type: SolastaModApi.FeatureBuilder
// Assembly: OathOfRetribution_springupdate, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E0C31DE3-014A-4A7A-8428-F8DEFE001310
// Assembly location: C:\Users\paulo\Downloads\OathOfRetribution.dll

using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SolastaOathOfRetribution
{
  internal class FeatureBuilder
  {
    public static FeatureDefinitionProficiency BuildProficiency(
      RuleDefinitions.ProficiencyType type,
      List<string> proficiencies,
      string name,
      GuiPresentation guiPresentation)
    {
      FeatureDefinitionProficiency instance = ScriptableObject.CreateInstance<FeatureDefinitionProficiency>();
      Traverse.Create((object) instance).Field("proficiencyType").SetValue((object) type);
      foreach (string proficiency in proficiencies)
        instance.Proficiencies.Add(proficiency);
      Traverse.Create((object) instance).Field(nameof (name)).SetValue((object) name);
      instance.name = name;
      Traverse.Create((object) instance).Field(nameof (guiPresentation)).SetValue((object) guiPresentation);
      Traverse.Create((object) instance).Field("guid").SetValue((object) GuidHelper.Create(Main.ModGuidNamespace, name).ToString());
      DatabaseRepository.GetDatabase<FeatureDefinition>().Add((FeatureDefinition) instance);
      return instance;
    }

    public static FeatureDefinitionPointPool BuildPointPool(
      HeroDefinitions.PointsPoolType poolType,
      int poolAmount,
      List<string> choices,
      string name,
      GuiPresentation guiPresentation)
    {
      FeatureDefinitionPointPool instance = ScriptableObject.CreateInstance<FeatureDefinitionPointPool>();
      Traverse.Create((object) instance).Field(nameof (poolType)).SetValue((object) poolType);
      Traverse.Create((object) instance).Field(nameof (poolAmount)).SetValue((object) poolAmount);
      foreach (string choice in choices)
        instance.RestrictedChoices.Add(choice);
      Traverse.Create((object) instance).Field(nameof (name)).SetValue((object) name);
      instance.name = name;
      Traverse.Create((object) instance).Field(nameof (guiPresentation)).SetValue((object) guiPresentation);
      Traverse.Create((object) instance).Field("guid").SetValue((object) GuidHelper.Create(Main.ModGuidNamespace, name).ToString());
      DatabaseRepository.GetDatabase<FeatureDefinition>().Add((FeatureDefinition) instance);
      return instance;
    }

    public static FeatureDefinitionAttributeModifier BuildAttributeModifier(
      FeatureDefinitionAttributeModifier.AttributeModifierOperation modifierType,
      string attribute,
      int amount,
      string name)
    {
      FeatureDefinitionAttributeModifier instance = ScriptableObject.CreateInstance<FeatureDefinitionAttributeModifier>();
      Traverse.Create((object) instance).Field("modifierType2").SetValue((object) modifierType);
      Traverse.Create((object) instance).Field("modifiedAttribute").SetValue((object) attribute);
      Traverse.Create((object) instance).Field("modifierValue").SetValue((object) amount);
      Traverse.Create((object) instance).Field(nameof (name)).SetValue((object) name);
      instance.name = name;
      GuiPresentationBuilder presentationBuilder = new GuiPresentationBuilder("Feature/&AttributeIncreaseDescription", "Feature/&AbilityScoreIncreaseTitle");
      Traverse.Create((object) instance).Field("guiPresentation").SetValue((object) presentationBuilder.Build());
      Traverse.Create((object) instance).Field("guid").SetValue((object) GuidHelper.Create(Main.ModGuidNamespace, name).ToString());
      DatabaseRepository.GetDatabase<FeatureDefinition>().Add((FeatureDefinition) instance);
      return instance;
    }

    public static FeatureDefinitionAttributeModifier BuildAttributeModifier(
      FeatureDefinitionAttributeModifier.AttributeModifierOperation modifierType,
      string attribute,
      int amount,
      string name,
      GuiPresentation guiPresentation)
    {
      FeatureDefinitionAttributeModifier instance = ScriptableObject.CreateInstance<FeatureDefinitionAttributeModifier>();
      Traverse.Create((object) instance).Field("modifierType2").SetValue((object) modifierType);
      Traverse.Create((object) instance).Field("modifiedAttribute").SetValue((object) attribute);
      Traverse.Create((object) instance).Field("modifierValue").SetValue((object) amount);
      Traverse.Create((object) instance).Field(nameof (name)).SetValue((object) name);
      instance.name = name;
      Traverse.Create((object) instance).Field(nameof (guiPresentation)).SetValue((object) guiPresentation);
      Traverse.Create((object) instance).Field("guid").SetValue((object) GuidHelper.Create(Main.ModGuidNamespace, name).ToString());
      DatabaseRepository.GetDatabase<FeatureDefinition>().Add((FeatureDefinition) instance);
      return instance;
    }

    public static FeatureDefinitionAttackModifier BuildAttackModifier(
      RuleDefinitions.AttackModifierMethod attackRollModifierMethod,
      int attackRollModifier,
      string attackRollAbilityScore,
      RuleDefinitions.AttackModifierMethod damageRollModifierMethod,
      int damageRollModifier,
      string damageRollAbilityScore,
      bool canAddAbilityBonusToSecondary,
      string additionalAttackTag,
      string name,
      GuiPresentation guiPresentation)
    {
      FeatureDefinitionAttackModifier instance = ScriptableObject.CreateInstance<FeatureDefinitionAttackModifier>();
      Traverse.Create((object) instance).Field(nameof (attackRollModifierMethod)).SetValue((object) attackRollModifierMethod);
      Traverse.Create((object) instance).Field(nameof (attackRollModifier)).SetValue((object) attackRollModifier);
      Traverse.Create((object) instance).Field(nameof (attackRollAbilityScore)).SetValue((object) attackRollAbilityScore);
      Traverse.Create((object) instance).Field(nameof (damageRollModifierMethod)).SetValue((object) damageRollModifierMethod);
      Traverse.Create((object) instance).Field(nameof (damageRollModifier)).SetValue((object) damageRollModifier);
      Traverse.Create((object) instance).Field(nameof (damageRollAbilityScore)).SetValue((object) damageRollAbilityScore);
      Traverse.Create((object) instance).Field(nameof (canAddAbilityBonusToSecondary)).SetValue((object) canAddAbilityBonusToSecondary);
      Traverse.Create((object) instance).Field(nameof (additionalAttackTag)).SetValue((object) additionalAttackTag);
      Traverse.Create((object) instance).Field(nameof (name)).SetValue((object) name);
      instance.name = name;
      Traverse.Create((object) instance).Field(nameof (guiPresentation)).SetValue((object) guiPresentation);
      Traverse.Create((object) instance).Field("guid").SetValue((object) GuidHelper.Create(Main.ModGuidNamespace, name).ToString());
      DatabaseRepository.GetDatabase<FeatureDefinition>().Add((FeatureDefinition) instance);
      return instance;
    }

    public static FeatureDefinitionSavingThrowAffinity BuildSavingThrowAffinity(
      List<string> abilityScores,
      RuleDefinitions.CharacterSavingThrowAffinity affinityType,
      bool againstMagic,
      string name,
      GuiPresentation guiPresentation)
    {
      FeatureDefinitionSavingThrowAffinity instance = ScriptableObject.CreateInstance<FeatureDefinitionSavingThrowAffinity>();
      foreach (string abilityScore in abilityScores)
      {
        FeatureDefinitionSavingThrowAffinity.SavingThrowAffinityGroup throwAffinityGroup = new FeatureDefinitionSavingThrowAffinity.SavingThrowAffinityGroup();
        throwAffinityGroup.abilityScoreName = abilityScore;
        throwAffinityGroup.affinity = affinityType;
        if (againstMagic)
        {
          throwAffinityGroup.restrictedSchools.Add(DatabaseHelper.SchoolOfMagicDefinitions.SchoolAbjuration.Name);
          throwAffinityGroup.restrictedSchools.Add(DatabaseHelper.SchoolOfMagicDefinitions.SchoolConjuration.Name);
          throwAffinityGroup.restrictedSchools.Add(DatabaseHelper.SchoolOfMagicDefinitions.SchoolDivination.Name);
          throwAffinityGroup.restrictedSchools.Add(DatabaseHelper.SchoolOfMagicDefinitions.SchoolEnchantment.Name);
          throwAffinityGroup.restrictedSchools.Add(DatabaseHelper.SchoolOfMagicDefinitions.SchoolEvocation.Name);
          throwAffinityGroup.restrictedSchools.Add(DatabaseHelper.SchoolOfMagicDefinitions.SchoolIllusion.Name);
          throwAffinityGroup.restrictedSchools.Add(DatabaseHelper.SchoolOfMagicDefinitions.SchoolNecromancy.Name);
          throwAffinityGroup.restrictedSchools.Add(DatabaseHelper.SchoolOfMagicDefinitions.SchoolTransmutation.Name);
        }
        instance.AffinityGroups.Add(throwAffinityGroup);
      }
      Traverse.Create((object) instance).Field(nameof (name)).SetValue((object) name);
      instance.name = name;
      Traverse.Create((object) instance).Field(nameof (guiPresentation)).SetValue((object) guiPresentation);
      Traverse.Create((object) instance).Field("guid").SetValue((object) GuidHelper.Create(Main.ModGuidNamespace, name).ToString());
      DatabaseRepository.GetDatabase<FeatureDefinition>().Add((FeatureDefinition) instance);
      return instance;
    }

    public static FeatureDefinitionAbilityCheckAffinity BuildAbilityAffinity(
      List<Tuple<string, string>> abilityProficiencyPairs,
      RuleDefinitions.CharacterAbilityCheckAffinity affinityType,
      string name,
      GuiPresentation guiPresentation)
    {
      FeatureDefinitionAbilityCheckAffinity instance = ScriptableObject.CreateInstance<FeatureDefinitionAbilityCheckAffinity>();
      foreach (Tuple<string, string> abilityProficiencyPair in abilityProficiencyPairs)
      {
        FeatureDefinitionAbilityCheckAffinity.AbilityCheckAffinityGroup checkAffinityGroup = new FeatureDefinitionAbilityCheckAffinity.AbilityCheckAffinityGroup();
        checkAffinityGroup.abilityScoreName = abilityProficiencyPair.Item1;
        if (!string.IsNullOrEmpty(abilityProficiencyPair.Item2))
          checkAffinityGroup.proficiencyName = abilityProficiencyPair.Item2;
        checkAffinityGroup.affinity = affinityType;
        instance.AffinityGroups.Add(checkAffinityGroup);
      }
      Traverse.Create((object) instance).Field(nameof (name)).SetValue((object) name);
      instance.name = name;
      Traverse.Create((object) instance).Field(nameof (guiPresentation)).SetValue((object) guiPresentation);
      Traverse.Create((object) instance).Field("guid").SetValue((object) GuidHelper.Create(Main.ModGuidNamespace, name).ToString());
      DatabaseRepository.GetDatabase<FeatureDefinition>().Add((FeatureDefinition) instance);
      return instance;
    }

    public static FeatureDefinitionMagicAffinity BuildMagicAffinityConcentration(
      RuleDefinitions.AdvantageType advantageType,
      int threshold,
      string name,
      GuiPresentation guiPresentation)
    {
      FeatureDefinitionMagicAffinity instance = ScriptableObject.CreateInstance<FeatureDefinitionMagicAffinity>();
      Traverse.Create((object) instance).Field("concentrationAdvantage").SetValue((object) advantageType);
      if (threshold > 0)
        Traverse.Create((object) instance).Field("overConcentrationThreshold").SetValue((object) threshold);
      Traverse.Create((object) instance).Field(nameof (name)).SetValue((object) name);
      instance.name = name;
      Traverse.Create((object) instance).Field(nameof (guiPresentation)).SetValue((object) guiPresentation);
      Traverse.Create((object) instance).Field("guid").SetValue((object) GuidHelper.Create(Main.ModGuidNamespace, name).ToString());
      DatabaseRepository.GetDatabase<FeatureDefinition>().Add((FeatureDefinition) instance);
      return instance;
    }

    public static FeatureDefinitionMagicAffinity BuildMagicAffinitySpecialtySpell(
      SpellDefinition spell,
      RuleDefinitions.AdvantageType advantageType,
      string name,
      GuiPresentation guiPresentation)
    {
      FeatureDefinitionMagicAffinity instance = ScriptableObject.CreateInstance<FeatureDefinitionMagicAffinity>();
      Traverse.Create((object) instance).Field("forcedSpellDefinition").SetValue((object) spell);
      Traverse.Create((object) instance).Field("forcedSavingThrowAffinity").SetValue((object) advantageType);
      Traverse.Create((object) instance).Field(nameof (name)).SetValue((object) name);
      instance.name = name;
      Traverse.Create((object) instance).Field(nameof (guiPresentation)).SetValue((object) guiPresentation);
      Traverse.Create((object) instance).Field("guid").SetValue((object) GuidHelper.Create(Main.ModGuidNamespace, name).ToString());
      DatabaseRepository.GetDatabase<FeatureDefinition>().Add((FeatureDefinition) instance);
      return instance;
    }

    public static FeatureDefinitionMagicAffinity BuildMagicAffinityModifiers(
      int attackModifier,
      int dcModifier,
      int preparedModifier,
      string name,
      GuiPresentation guiPresentation)
    {
      FeatureDefinitionMagicAffinity instance = ScriptableObject.CreateInstance<FeatureDefinitionMagicAffinity>();
      if ((uint) attackModifier > 0U)
        Traverse.Create((object) instance).Field("spellAttackModifier").SetValue((object) attackModifier);
      if ((uint) dcModifier > 0U)
        Traverse.Create((object) instance).Field("saveDCModifier").SetValue((object) dcModifier);
      if ((uint) preparedModifier > 0U)
        Traverse.Create((object) instance).Field("maxPreparedSpellsModifier").SetValue((object) preparedModifier);
      Traverse.Create((object) instance).Field(nameof (name)).SetValue((object) name);
      instance.name = name;
      Traverse.Create((object) instance).Field(nameof (guiPresentation)).SetValue((object) guiPresentation);
      Traverse.Create((object) instance).Field("guid").SetValue((object) GuidHelper.Create(Main.ModGuidNamespace, name).ToString());
      DatabaseRepository.GetDatabase<FeatureDefinition>().Add((FeatureDefinition) instance);
      return instance;
    }

    public static FeatureDefinitionMagicAffinity BuildMagicAffinityScribing(
      float scribeDurationMultiplier,
      float scribeCostMultiplier,
      int additionalScribedSpells,
      RuleDefinitions.AdvantageType scribeAdvantage,
      string name,
      GuiPresentation guiPresentation)
    {
      FeatureDefinitionMagicAffinity instance = ScriptableObject.CreateInstance<FeatureDefinitionMagicAffinity>();
      Traverse.Create((object) instance).Field(nameof (scribeDurationMultiplier)).SetValue((object) scribeDurationMultiplier);
      Traverse.Create((object) instance).Field(nameof (scribeCostMultiplier)).SetValue((object) scribeCostMultiplier);
      Traverse.Create((object) instance).Field(nameof (additionalScribedSpells)).SetValue((object) additionalScribedSpells);
      Traverse.Create((object) instance).Field("scribeAdvantageType").SetValue((object) scribeAdvantage);
      Traverse.Create((object) instance).Field(nameof (name)).SetValue((object) name);
      instance.name = name;
      Traverse.Create((object) instance).Field(nameof (guiPresentation)).SetValue((object) guiPresentation);
      Traverse.Create((object) instance).Field("guid").SetValue((object) GuidHelper.Create(Main.ModGuidNamespace, name).ToString());
      DatabaseRepository.GetDatabase<FeatureDefinition>().Add((FeatureDefinition) instance);
      return instance;
    }

    public static FeatureDefinitionMagicAffinity BuildMagicAffinityHeightenedList(
      List<string> spellNames,
      int levelBonus,
      string name,
      GuiPresentation guiPresentation)
    {
      FeatureDefinitionMagicAffinity instance = ScriptableObject.CreateInstance<FeatureDefinitionMagicAffinity>();
      Traverse.Create((object) instance).Field("warListSlotBonus").SetValue((object) levelBonus);
      Traverse.Create((object) instance).Field("usesWarList").SetValue((object) true);
      foreach (string spellName in spellNames)
        instance.WarListSpells.Add(spellName);
      Traverse.Create((object) instance).Field(nameof (name)).SetValue((object) name);
      instance.name = name;
      Traverse.Create((object) instance).Field(nameof (guiPresentation)).SetValue((object) guiPresentation);
      Traverse.Create((object) instance).Field("guid").SetValue((object) GuidHelper.Create(Main.ModGuidNamespace, name).ToString());
      DatabaseRepository.GetDatabase<FeatureDefinition>().Add((FeatureDefinition) instance);
      return instance;
    }

    public static FeatureDefinitionMagicAffinity BuildMagicAffinityRitualCasting(
      RuleDefinitions.RitualCasting ritualCasting,
      string name,
      GuiPresentation guiPresentation)
    {
      FeatureDefinitionMagicAffinity instance = ScriptableObject.CreateInstance<FeatureDefinitionMagicAffinity>();
      Traverse.Create((object) instance).Field(nameof (ritualCasting)).SetValue((object) ritualCasting);
      Traverse.Create((object) instance).Field(nameof (name)).SetValue((object) name);
      instance.name = name;
      Traverse.Create((object) instance).Field(nameof (guiPresentation)).SetValue((object) guiPresentation);
      Traverse.Create((object) instance).Field("guid").SetValue((object) GuidHelper.Create(Main.ModGuidNamespace, name).ToString());
      DatabaseRepository.GetDatabase<FeatureDefinition>().Add((FeatureDefinition) instance);
      return instance;
    }

    public static FeatureDefinitionMagicAffinity BuildMagicAffinityExtendedSpellList(
      SpellListDefinition spellListDefinition,
      string name,
      GuiPresentation guiPresentation)
    {
      FeatureDefinitionMagicAffinity instance = ScriptableObject.CreateInstance<FeatureDefinitionMagicAffinity>();
      Traverse.Create((object) instance).Field("extendedSpellList").SetValue((object) spellListDefinition);
      Traverse.Create((object) instance).Field(nameof (name)).SetValue((object) name);
      instance.name = name;
      Traverse.Create((object) instance).Field(nameof (guiPresentation)).SetValue((object) guiPresentation);
      Traverse.Create((object) instance).Field("guid").SetValue((object) GuidHelper.Create(Main.ModGuidNamespace, name).ToString());
      DatabaseRepository.GetDatabase<FeatureDefinition>().Add((FeatureDefinition) instance);
      return instance;
    }

    public static FeatureDefinitionMagicAffinity BuildMagicAffinityImmunities(
      List<string> spellNames,
      string name,
      GuiPresentation guiPresentation)
    {
      FeatureDefinitionMagicAffinity instance = ScriptableObject.CreateInstance<FeatureDefinitionMagicAffinity>();
      foreach (string spellName in spellNames)
        instance.SpellImmunities.Add(spellName);
      Traverse.Create((object) instance).Field(nameof (name)).SetValue((object) name);
      instance.name = name;
      Traverse.Create((object) instance).Field(nameof (guiPresentation)).SetValue((object) guiPresentation);
      Traverse.Create((object) instance).Field("guid").SetValue((object) GuidHelper.Create(Main.ModGuidNamespace, name).ToString());
      DatabaseRepository.GetDatabase<FeatureDefinition>().Add((FeatureDefinition) instance);
      return instance;
    }

    public static FeatureDefinitionMagicAffinity BuildMagicAffinityAutoIdentify(
      List<string> autoIdentify,
      string name,
      GuiPresentation guiPresentation)
    {
      FeatureDefinitionMagicAffinity instance = ScriptableObject.CreateInstance<FeatureDefinitionMagicAffinity>();
      foreach (string str in autoIdentify)
        instance.DeviceTagsAutoIdentifying.Add(str);
      Traverse.Create((object) instance).Field(nameof (name)).SetValue((object) name);
      instance.name = name;
      Traverse.Create((object) instance).Field(nameof (guiPresentation)).SetValue((object) guiPresentation);
      Traverse.Create((object) instance).Field("guid").SetValue((object) GuidHelper.Create(Main.ModGuidNamespace, name).ToString());
      DatabaseRepository.GetDatabase<FeatureDefinition>().Add((FeatureDefinition) instance);
      return instance;
    }

    public static FeatureDefinitionPower BuildSpellFormPower(
      int usesPerRecharge,
      RuleDefinitions.UsesDetermination usesDetermination,
      RuleDefinitions.ActivationTime activationTime,
      int costPerUse,
      RuleDefinitions.RechargeRate recharge,
      string name,
      GuiPresentation guiPresentation)
    {
      FeatureDefinitionPower instance = ScriptableObject.CreateInstance<FeatureDefinitionPower>();
      Traverse.Create((object) instance).Field("fixedUsesPerRecharge").SetValue((object) usesPerRecharge);
      Traverse.Create((object) instance).Field(nameof (usesDetermination)).SetValue((object) usesDetermination);
      Traverse.Create((object) instance).Field(nameof (activationTime)).SetValue((object) activationTime);
      Traverse.Create((object) instance).Field(nameof (costPerUse)).SetValue((object) costPerUse);
      Traverse.Create((object) instance).Field("rechargeRate").SetValue((object) recharge);
      EffectDescription root1 = new EffectDescription();
      Traverse.Create((object) root1).Field("targetSide").SetValue((object) RuleDefinitions.Side.Ally);
      Traverse.Create((object) root1).Field("createdByCharacter").SetValue((object) true);
      EffectForm root2 = new EffectForm();
      root2.FormType = EffectForm.EffectFormType.SpellSlots;
      SpellSlotsForm root3 = new SpellSlotsForm();
      Traverse.Create((object) root3).Field("type").SetValue((object) SpellSlotsForm.EffectType.RecoverHalfLevelUp);
      Traverse.Create((object) root2).Field("spellSlotsForm").SetValue((object) root3);
      root1.EffectForms.Add(root2);
      EffectAdvancement root4 = new EffectAdvancement();
      Traverse.Create((object) root4).Field("incrementMultiplier").SetValue((object) 1);
      Traverse.Create((object) root1).Field("effectAdvancement").SetValue((object) root4);
      EffectParticleParameters particleParameters = new EffectParticleParameters();
      particleParameters.Copy(DatabaseHelper.FeatureDefinitionPowers.PowerWizardArcaneRecovery.EffectDescription.EffectParticleParameters);
      Traverse.Create((object) root1).Field("effectParticleParameters").SetValue((object) particleParameters);
      Traverse.Create((object) instance).Field("effectDescription").SetValue((object) root1);
      Traverse.Create((object) instance).Field(nameof (name)).SetValue((object) name);
      instance.name = name;
      Traverse.Create((object) instance).Field(nameof (guiPresentation)).SetValue((object) guiPresentation);
      Traverse.Create((object) instance).Field("guid").SetValue((object) GuidHelper.Create(Main.ModGuidNamespace, name).ToString());
      DatabaseRepository.GetDatabase<FeatureDefinition>().Add((FeatureDefinition) instance);
      return instance;
    }

    public static FeatureDefinitionPower BuildActionItemPower(
      int usesPerRecharge,
      RuleDefinitions.UsesDetermination usesDetermination,
      string usesAbilityScoreName,
      RuleDefinitions.ActivationTime activationTime,
      int costPerUse,
      RuleDefinitions.RechargeRate recharge,
      RuleDefinitions.RangeType rangeType,
      int rangeParameter,
      global::ActionDefinitions.ItemSelectionType itemSelectionType,
      RuleDefinitions.DurationType durationType,
      int durationParameter,
      RuleDefinitions.TurnOccurenceType endOfEffect,
      FeatureDefinition itemFeature,
      string name,
      GuiPresentation guiPresentation)
    {
      FeatureDefinitionPower instance = ScriptableObject.CreateInstance<FeatureDefinitionPower>();
      Traverse.Create((object) instance).Field("fixedUsesPerRecharge").SetValue((object) usesPerRecharge);
      Traverse.Create((object) instance).Field(nameof (usesDetermination)).SetValue((object) usesDetermination);
      Traverse.Create((object) instance).Field(nameof (activationTime)).SetValue((object) activationTime);
      Traverse.Create((object) instance).Field(nameof (costPerUse)).SetValue((object) costPerUse);
      Traverse.Create((object) instance).Field("rechargeRate").SetValue((object) recharge);
      Traverse.Create((object) instance).Field(nameof (usesAbilityScoreName)).SetValue((object) usesAbilityScoreName);
      Traverse.Create((object) instance).Field("uniqueInstance").SetValue((object) true);
      EffectDescription root1 = new EffectDescription();
      Traverse.Create((object) root1).Field("targetSide").SetValue((object) RuleDefinitions.Side.Ally);
      Traverse.Create((object) root1).Field("createdByCharacter").SetValue((object) true);
      Traverse.Create((object) root1).Field(nameof (rangeType)).SetValue((object) rangeType);
      Traverse.Create((object) root1).Field(nameof (rangeParameter)).SetValue((object) rangeParameter);
      Traverse.Create((object) root1).Field("targetType").SetValue((object) RuleDefinitions.TargetType.Item);
      Traverse.Create((object) root1).Field(nameof (itemSelectionType)).SetValue((object) itemSelectionType);
      Traverse.Create((object) root1).Field("canBePlacedOnCharacter").SetValue((object) true);
      Traverse.Create((object) root1).Field(nameof (durationType)).SetValue((object) durationType);
      Traverse.Create((object) root1).Field(nameof (durationParameter)).SetValue((object) durationParameter);
      Traverse.Create((object) root1).Field(nameof (endOfEffect)).SetValue((object) endOfEffect);
      EffectForm root2 = new EffectForm();
      root2.FormType = EffectForm.EffectFormType.ItemProperty;
      Traverse.Create((object) root2).Field("createdByCharacter").SetValue((object) true);
      ItemPropertyForm root3 = new ItemPropertyForm();
      Traverse.Create((object) root3).Field("usageLimitation").SetValue((object) RuleDefinitions.ItemPropertyUsage.Unlimited);
      root3.FeatureBySlotLevel.Add(new FeatureUnlockByLevel(itemFeature, 0));
      Traverse.Create((object) root2).Field("itemPropertyForm").SetValue((object) root3);
      root1.EffectForms.Add(root2);
      EffectAdvancement root4 = new EffectAdvancement();
      Traverse.Create((object) root4).Field("incrementMultiplier").SetValue((object) 1);
      Traverse.Create((object) root1).Field("effectAdvancement").SetValue((object) root4);
      EffectParticleParameters particleParameters = new EffectParticleParameters();
      particleParameters.Copy(DatabaseHelper.SpellDefinitions.MagicWeapon.EffectDescription.EffectParticleParameters);
      Traverse.Create((object) root1).Field("effectParticleParameters").SetValue((object) particleParameters);
      Traverse.Create((object) instance).Field("effectDescription").SetValue((object) root1);
      Traverse.Create((object) instance).Field(nameof (name)).SetValue((object) name);
      instance.name = name;
      Traverse.Create((object) instance).Field(nameof (guiPresentation)).SetValue((object) guiPresentation);
      Traverse.Create((object) instance).Field("guid").SetValue((object) GuidHelper.Create(Main.ModGuidNamespace, name).ToString());
      DatabaseRepository.GetDatabase<FeatureDefinition>().Add((FeatureDefinition) instance);
      return instance;
    }

    public static SaveAffinityBySenseDescription BuildSaveAffinityBySense(
      SenseMode.Type senseType,
      RuleDefinitions.AdvantageType advantageType)
    {
      SaveAffinityBySenseDescription root = new SaveAffinityBySenseDescription();
      Traverse.Create((object) root).Field(nameof (senseType)).SetValue((object) senseType);
      Traverse.Create((object) root).Field(nameof (advantageType)).SetValue((object) advantageType);
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
      Traverse.Create((object) instance).Field("fixedUsesPerRecharge").SetValue((object) usesPerRecharge);
      Traverse.Create((object) instance).Field(nameof (usesDetermination)).SetValue((object) usesDetermination);
      Traverse.Create((object) instance).Field(nameof (activationTime)).SetValue((object) activationTime);
      Traverse.Create((object) instance).Field(nameof (costPerUse)).SetValue((object) costPerUse);
      Traverse.Create((object) instance).Field("rechargeRate").SetValue((object) recharge);
      Traverse.Create((object) instance).Field(nameof (usesAbilityScoreName)).SetValue((object) usesAbilityScoreName);
      Traverse.Create((object) instance).Field("uniqueInstance").SetValue((object) true);
      EffectDescription root1 = new EffectDescription();
      Traverse.Create((object) root1).Field("targetSide").SetValue((object) RuleDefinitions.Side.Enemy);
      Traverse.Create((object) root1).Field("createdByCharacter").SetValue((object) true);
      Traverse.Create((object) root1).Field(nameof (rangeType)).SetValue((object) rangeType);
      Traverse.Create((object) root1).Field(nameof (rangeParameter)).SetValue((object) rangeParameter);
      Traverse.Create((object) root1).Field(nameof (targetType)).SetValue((object) targetType);
      Traverse.Create((object) root1).Field(nameof (itemSelectionType)).SetValue((object) itemSelectionType);
      Traverse.Create((object) root1).Field("canBePlacedOnCharacter").SetValue((object) true);
      Traverse.Create((object) root1).Field(nameof (durationType)).SetValue((object) durationType);
      Traverse.Create((object) root1).Field(nameof (durationParameter)).SetValue((object) durationParameter);
      Traverse.Create((object) root1).Field(nameof (endOfEffect)).SetValue((object) endOfEffect);
      Traverse.Create((object) root1).Field("hasSavingThrow").SetValue((object) true);
      Traverse.Create((object) root1).Field("savingThrowAbility").SetValue((object) savethrowtype);
      Traverse.Create((object) root1).Field("createdByCharacter").SetValue((object) true);
      Traverse.Create((object) root1).Field("savingThrowDifficultyAbility").SetValue((object) savethrowDA);
      Traverse.Create((object) root1).Field("fixedSavingThrowDifficultyClass").SetValue((object) savethrowDC);
      foreach (SaveAffinityBySenseDescription senseDescription in saveAffinityBySenseDescriptions)
        root1.SavingThrowAffinitiesBySense.Add(senseDescription);
      EffectForm root2 = new EffectForm();
      root2.FormType = EffectForm.EffectFormType.Condition;
      Traverse.Create((object) root2).Field("formType").SetValue((object) 2);
      Traverse.Create((object) root2).Field("createdByCharacter").SetValue((object) true);
      ConditionForm root3 = new ConditionForm();
      Traverse.Create((object) root3).Field("operation").SetValue((object) ConditionForm.ConditionOperation.Add);
      Traverse.Create((object) root3).Field("conditionDefinition").SetValue((object) condition1);
      Traverse.Create((object) root3).Field("conditionDefinitionName").SetValue((object) condition1.Name);
      Traverse.Create((object) root2).Field("conditionForm").SetValue((object) root3);
      root1.EffectForms.Add(root2);
      EffectAdvancement effectAdvancement = new EffectAdvancement();
      Traverse.Create((object) root1).Field("effectAdvancement").SetValue((object) effectAdvancement);
      EffectParticleParameters particleParameters = new EffectParticleParameters();
      particleParameters.Copy(DatabaseHelper.SpellDefinitions.Fear.EffectDescription.EffectParticleParameters);
      Traverse.Create((object) root1).Field("effectParticleParameters").SetValue((object) particleParameters);
      Traverse.Create((object) instance).Field("effectDescription").SetValue((object) root1);
      Traverse.Create((object) instance).Field(nameof (name)).SetValue((object) name);
      instance.name = name;
      Traverse.Create((object) instance).Field(nameof (guiPresentation)).SetValue((object) guiPresentation);
      Traverse.Create((object) instance).Field("guid").SetValue((object) GuidHelper.Create(Main.ModGuidNamespace, name).ToString());
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
      Traverse.Create((object) instance).Field("fixedUsesPerRecharge").SetValue((object) usesPerRecharge);
      Traverse.Create((object) instance).Field(nameof (usesDetermination)).SetValue((object) usesDetermination);
      Traverse.Create((object) instance).Field(nameof (activationTime)).SetValue((object) activationTime);
      Traverse.Create((object) instance).Field(nameof (costPerUse)).SetValue((object) costPerUse);
      Traverse.Create((object) instance).Field("rechargeRate").SetValue((object) recharge);
      Traverse.Create((object) instance).Field(nameof (usesAbilityScoreName)).SetValue((object) usesAbilityScoreName);
      Traverse.Create((object) instance).Field("uniqueInstance").SetValue((object) true);
      EffectDescription root1 = new EffectDescription();
      Traverse.Create((object) root1).Field("targetSide").SetValue((object) RuleDefinitions.Side.Enemy);
      Traverse.Create((object) root1).Field("createdByCharacter").SetValue((object) true);
      Traverse.Create((object) root1).Field(nameof (rangeType)).SetValue((object) rangeType);
      Traverse.Create((object) root1).Field(nameof (rangeParameter)).SetValue((object) rangeParameter);
      Traverse.Create((object) root1).Field(nameof (targetType)).SetValue((object) targetType);
      Traverse.Create((object) root1).Field(nameof (itemSelectionType)).SetValue((object) itemSelectionType);
      Traverse.Create((object) root1).Field("canBePlacedOnCharacter").SetValue((object) true);
      Traverse.Create((object) root1).Field(nameof (durationType)).SetValue((object) durationType);
      Traverse.Create((object) root1).Field(nameof (durationParameter)).SetValue((object) durationParameter);
      Traverse.Create((object) root1).Field(nameof (endOfEffect)).SetValue((object) endOfEffect);
      EffectForm root2 = new EffectForm();
      root2.FormType = EffectForm.EffectFormType.Condition;
      Traverse.Create((object) root2).Field("formType").SetValue((object) 2);
      Traverse.Create((object) root2).Field("createdByCharacter").SetValue((object) true);
      ConditionForm root3 = new ConditionForm();
      Traverse.Create((object) root3).Field("operation").SetValue((object) ConditionForm.ConditionOperation.Add);
      Traverse.Create((object) root3).Field("conditionDefinition").SetValue((object) condition1);
      Traverse.Create((object) root3).Field("conditionDefinitionName").SetValue((object) condition1.Name);
      Traverse.Create((object) root2).Field("conditionForm").SetValue((object) root3);
      root1.EffectForms.Add(root2);
      EffectAdvancement effectAdvancement = new EffectAdvancement();
      Traverse.Create((object) root1).Field("effectAdvancement").SetValue((object) effectAdvancement);
      EffectParticleParameters particleParameters = new EffectParticleParameters();
      particleParameters.Copy(DatabaseHelper.SpellDefinitions.TrueStrike.EffectDescription.EffectParticleParameters);
      Traverse.Create((object) root1).Field("effectParticleParameters").SetValue((object) particleParameters);
      Traverse.Create((object) instance).Field("effectDescription").SetValue((object) root1);
      Traverse.Create((object) instance).Field(nameof (name)).SetValue((object) name);
      instance.name = name;
      Traverse.Create((object) instance).Field(nameof (guiPresentation)).SetValue((object) guiPresentation);
      Traverse.Create((object) instance).Field("guid").SetValue((object) GuidHelper.Create(Main.ModGuidNamespace, name).ToString());
      DatabaseRepository.GetDatabase<FeatureDefinition>().Add((FeatureDefinition) instance);
      return instance;
    }

    public static FeatureDefinitionPower BuildActionConditionPower(
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
      ConditionDefinition condition,
      string name,
      GuiPresentation guiPresentation)
    {
      FeatureDefinitionPower instance = ScriptableObject.CreateInstance<FeatureDefinitionPower>();
      Traverse.Create((object) instance).Field("fixedUsesPerRecharge").SetValue((object) usesPerRecharge);
      Traverse.Create((object) instance).Field(nameof (usesDetermination)).SetValue((object) usesDetermination);
      Traverse.Create((object) instance).Field(nameof (activationTime)).SetValue((object) activationTime);
      Traverse.Create((object) instance).Field(nameof (costPerUse)).SetValue((object) costPerUse);
      Traverse.Create((object) instance).Field("rechargeRate").SetValue((object) recharge);
      Traverse.Create((object) instance).Field(nameof (usesAbilityScoreName)).SetValue((object) usesAbilityScoreName);
      Traverse.Create((object) instance).Field("uniqueInstance").SetValue((object) true);
      EffectDescription root1 = new EffectDescription();
      Traverse.Create((object) root1).Field("targetSide").SetValue((object) RuleDefinitions.Side.Ally);
      Traverse.Create((object) root1).Field("createdByCharacter").SetValue((object) true);
      Traverse.Create((object) root1).Field(nameof (rangeType)).SetValue((object) rangeType);
      Traverse.Create((object) root1).Field(nameof (rangeParameter)).SetValue((object) rangeParameter);
      Traverse.Create((object) root1).Field(nameof (targetType)).SetValue((object) targetType);
      Traverse.Create((object) root1).Field(nameof (itemSelectionType)).SetValue((object) itemSelectionType);
      Traverse.Create((object) root1).Field("canBePlacedOnCharacter").SetValue((object) true);
      Traverse.Create((object) root1).Field(nameof (durationType)).SetValue((object) durationType);
      Traverse.Create((object) root1).Field(nameof (durationParameter)).SetValue((object) durationParameter);
      Traverse.Create((object) root1).Field(nameof (endOfEffect)).SetValue((object) endOfEffect);
      EffectForm root2 = new EffectForm();
      root2.FormType = EffectForm.EffectFormType.Condition;
      Traverse.Create((object) root2).Field("createdByCharacter").SetValue((object) true);
      ConditionForm root3 = new ConditionForm();
      Traverse.Create((object) root3).Field("operation").SetValue((object) ConditionForm.ConditionOperation.Add);
      Traverse.Create((object) root3).Field("conditionDefinition").SetValue((object) condition);
      Traverse.Create((object) root3).Field("conditionDefinitionName").SetValue((object) condition.Name);
      Traverse.Create((object) root2).Field("conditionForm").SetValue((object) root3);
      root1.EffectForms.Add(root2);
      EffectAdvancement root4 = new EffectAdvancement();
      Traverse.Create((object) root4).Field("incrementMultiplier").SetValue((object) 1);
      Traverse.Create((object) root1).Field("effectAdvancement").SetValue((object) root4);
      EffectParticleParameters particleParameters = new EffectParticleParameters();
      particleParameters.Copy(DatabaseHelper.SpellDefinitions.MagicWeapon.EffectDescription.EffectParticleParameters);
      Traverse.Create((object) root1).Field("effectParticleParameters").SetValue((object) particleParameters);
      Traverse.Create((object) instance).Field("effectDescription").SetValue((object) root1);
      Traverse.Create((object) instance).Field(nameof (name)).SetValue((object) name);
      instance.name = name;
      Traverse.Create((object) instance).Field(nameof (guiPresentation)).SetValue((object) guiPresentation);
      Traverse.Create((object) instance).Field("guid").SetValue((object) GuidHelper.Create(Main.ModGuidNamespace, name).ToString());
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
      Traverse.Create((object) instance).Field(nameof (activationTime)).SetValue((object) activationTime);
      Traverse.Create((object) instance).Field("rechargeRate").SetValue((object) rechargerate);
      Traverse.Create((object) instance).Field(nameof (costPerUse)).SetValue((object) costPerUse);
      Traverse.Create((object) instance).Field(nameof (usesAbilityScoreName)).SetValue((object) usesAbilityScoreName);
      Traverse.Create((object) instance).Field("fixedUsesPerRecharge").SetValue((object) fixedusesPerRecharge);
      Traverse.Create((object) instance).Field("abilityScore").SetValue((object) abilityScoreName);
      Traverse.Create((object) instance).Field("showCasting").SetValue((object) true);
      Traverse.Create((object) instance).Field("uniqueInstance").SetValue((object) true);
      Traverse.Create((object) instance).Field("shortTitleOverride").SetValue((object) shorttitle);
      EffectDescription root1 = new EffectDescription();
      Traverse.Create((object) root1).Field(nameof (targetType)).SetValue((object) targetType);
      Traverse.Create((object) root1).Field(nameof (itemSelectionType)).SetValue((object) itemSelectionType);
      Traverse.Create((object) root1).Field("targetSide").SetValue((object) targetside);
      Traverse.Create((object) root1).Field("createdByCharacter").SetValue((object) true);
      Traverse.Create((object) root1).Field("rangeType").SetValue((object) rangetype);
      Traverse.Create((object) root1).Field("canBePlacedOnCharacter").SetValue((object) true);
      Traverse.Create((object) root1).Field(nameof (durationType)).SetValue((object) durationType);
      Traverse.Create((object) root1).Field(nameof (durationParameter)).SetValue((object) durationParameter);
      Traverse.Create((object) root1).Field("endOfEffect").SetValue((object) turnoccurence);
      EffectForm root2 = new EffectForm();
      root2.FormType = EffectForm.EffectFormType.Condition;
      Traverse.Create((object) root2).Field("createdByCharacter").SetValue((object) true);
      ConditionForm root3 = new ConditionForm();
      Traverse.Create((object) root3).Field("operation").SetValue((object) ConditionForm.ConditionOperation.Add);
      Traverse.Create((object) root3).Field("conditionDefinition").SetValue((object) condition);
      Traverse.Create((object) root3).Field("conditionDefinitionName").SetValue((object) condition.Name);
      Traverse.Create((object) root3).Field("applyToSelf").SetValue((object) true);
      Traverse.Create((object) root2).Field("conditionForm").SetValue((object) root3);
      root1.EffectForms.Add(root2);
      EffectAdvancement root4 = new EffectAdvancement();
      Traverse.Create((object) root4).Field("incrementMultiplier").SetValue((object) 1);
      Traverse.Create((object) root1).Field("effectAdvancement").SetValue((object) root4);
      EffectParticleParameters particleParameters = new EffectParticleParameters();
      particleParameters.Copy(DatabaseHelper.FeatureDefinitionPowers.PowerDomainLawHolyRetribution.EffectDescription.EffectParticleParameters);
      Traverse.Create((object) root1).Field("effectParticleParameters").SetValue((object) particleParameters);
      Traverse.Create((object) instance).Field("effectDescription").SetValue((object) root1);
      Traverse.Create((object) instance).Field(nameof (name)).SetValue((object) name);
      instance.name = name;
      Traverse.Create((object) instance).Field(nameof (guiPresentation)).SetValue((object) guiPresentation);
      Traverse.Create((object) instance).Field("guid").SetValue((object) GuidHelper.Create(Main.ModGuidNamespace, name).ToString());
      DatabaseRepository.GetDatabase<FeatureDefinition>().Add((FeatureDefinition) instance);
      return instance;
    }

    public static FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup BuildAutoPreparedSpellGroup(
      int classLevel,
      List<SpellDefinition> spellnames)
    {
      FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup root = new FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup();
      Traverse.Create((object) root).Field(nameof (classLevel)).SetValue((object) classLevel);
      Traverse.Create((object) root).Field("spellsList").SetValue((object) new List<SpellDefinition>());
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
      Traverse.Create((object) instance).Field("autoPreparedSpellsGroups").SetValue((object) new List<FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup>());
      foreach (FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup autospelllist in autospelllists)
        instance.AutoPreparedSpellsGroups.Add(autospelllist);
      Traverse.Create((object) instance).Field("spellcastingClass").SetValue((object) characterclass);
      Traverse.Create((object) instance).Field("contentCopyright").SetValue((object) 4);
      Traverse.Create((object) instance).Field(nameof (name)).SetValue((object) name);
      instance.name = name;
      Traverse.Create((object) instance).Field(nameof (guiPresentation)).SetValue((object) guiPresentation);
      Traverse.Create((object) instance).Field("guid").SetValue((object) GuidHelper.Create(Main.ModGuidNamespace, name).ToString());
      DatabaseRepository.GetDatabase<FeatureDefinition>().Add((FeatureDefinition) instance);
      return instance;
    }

    public static FeatureDefinitionMovementAffinity BuildMovementAffinity(
      int minbasespeed,
      double basespeedmultiplier,
      string name,
      GuiPresentation guiPresentation)
    {
      FeatureDefinitionMovementAffinity instance = ScriptableObject.CreateInstance<FeatureDefinitionMovementAffinity>();
      Traverse.Create((object) instance).Field("appliesToAllModes").SetValue((object) true);
      Traverse.Create((object) instance).Field("minimalBaseSpeed").SetValue((object) minbasespeed);
      float num = (float) basespeedmultiplier;
      Traverse.Create((object) instance).Field("baseSpeedMultiplicativeModifier").SetValue((object) num);
      Traverse.Create((object) instance).Field(nameof (name)).SetValue((object) name);
      instance.name = name;
      Traverse.Create((object) instance).Field(nameof (guiPresentation)).SetValue((object) guiPresentation);
      Traverse.Create((object) instance).Field("guid").SetValue((object) GuidHelper.Create(Main.ModGuidNamespace, name).ToString());
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
      Traverse.Create((object) instance).Field("conditionType").SetValue((object) RuleDefinitions.ConditionType.Detrimental);
      Traverse.Create((object) instance).Field("allowMultipleInstances").SetValue((object) false);
      Traverse.Create((object) instance).Field(nameof (durationType)).SetValue((object) durationType);
      Traverse.Create((object) instance).Field(nameof (durationParameter)).SetValue((object) durationParameter);
      foreach (RuleDefinitions.ConditionInterruption conditionInterruption in conditionInterruptions)
        instance.SpecialInterruptions.Add(conditionInterruption);
      AssetReference assetReference = new AssetReference();
      Traverse.Create((object) instance).Field("conditionStartParticleReference").SetValue((object) assetReference);
      Traverse.Create((object) instance).Field("conditionParticleReference").SetValue((object) assetReference);
      Traverse.Create((object) instance).Field("conditionEndParticleReference").SetValue((object) assetReference);
      Traverse.Create((object) instance).Field("characterShaderReference").SetValue((object) assetReference);
      Traverse.Create((object) instance).Field("recurrentEffectForms").SetValue((object) new List<EffectForm>());
      Traverse.Create((object) instance).Field("cancellingConditions").SetValue((object) new List<ConditionDefinition>());
      Traverse.Create((object) instance).Field(nameof (guiPresentation)).SetValue((object) guiPresentation);
      Traverse.Create((object) instance).Field(nameof (name)).SetValue((object) name);
      instance.name = name;
      Traverse.Create((object) instance).Field("guid").SetValue((object) GuidHelper.Create(Main.ModGuidNamespace, name).ToString());
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
      Traverse.Create((object) instance).Field("conditionType").SetValue((object) RuleDefinitions.ConditionType.Beneficial);
      Traverse.Create((object) instance).Field("allowMultipleInstances").SetValue((object) false);
      Traverse.Create((object) instance).Field(nameof (durationType)).SetValue((object) durationType);
      Traverse.Create((object) instance).Field(nameof (durationParameter)).SetValue((object) durationParameter);
      Traverse.Create((object) instance).Field("turnOccurence").SetValue((object) turnoccurence);
      foreach (RuleDefinitions.ConditionInterruption conditionInterruption in conditionInterruptions)
        instance.SpecialInterruptions.Add(conditionInterruption);
      AssetReference assetReference = new AssetReference();
      Traverse.Create((object) instance).Field("conditionStartParticleReference").SetValue((object) assetReference);
      Traverse.Create((object) instance).Field("conditionParticleReference").SetValue((object) assetReference);
      Traverse.Create((object) instance).Field("conditionEndParticleReference").SetValue((object) assetReference);
      Traverse.Create((object) instance).Field("characterShaderReference").SetValue((object) assetReference);
      Traverse.Create((object) instance).Field("recurrentEffectForms").SetValue((object) new List<EffectForm>());
      Traverse.Create((object) instance).Field("cancellingConditions").SetValue((object) new List<ConditionDefinition>());
      Traverse.Create((object) instance).Field(nameof (guiPresentation)).SetValue((object) guiPresentation);
      Traverse.Create((object) instance).Field(nameof (name)).SetValue((object) name);
      instance.name = name;
      Traverse.Create((object) instance).Field("guid").SetValue((object) GuidHelper.Create(Main.ModGuidNamespace, name).ToString());
      DatabaseRepository.GetDatabase<ConditionDefinition>().Add(instance);
      return instance;
    }
  }
}
