using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.CustomValidators;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using UnityEngine.AddressableAssets;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaUnfinishedBusiness.Spells;

internal static partial class SpellBuilders
{
    #region Caustic Zap

    internal static SpellDefinition BuildCausticZap()
    {
        const string NAME = "CausticZap";

        var effectDescription = EffectDescriptionBuilder
            .Create()
            .SetTargetingData(Side.Enemy, RangeType.RangeHit, 18, TargetType.IndividualsUnique)
            .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
            .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, 1, 1)
            .SetParticleEffectParameters(ShockingGrasp)
            .SetEffectForms(
                EffectFormBuilder
                    .Create()
                    .SetDamageForm(DamageTypeAcid, 1, DieType.D4)
                    .Build(),
                EffectFormBuilder
                    .Create()
                    .SetDamageForm(DamageTypeLightning, 1, DieType.D6)
                    .Build(),
                EffectFormBuilder
                    .Create()
                    .SetConditionForm(ConditionDazzled, ConditionForm.ConditionOperation.Add)
                    .Build())
            .Build();

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.CausticZap, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetSpellLevel(1)
            .SetSomaticComponent(true)
            .SetVerboseComponent(true)
            .SetCastingTime(ActivationTime.Action)
            .SetEffectDescription(effectDescription)
            .AddToDB();

        return spell;
    }

    #endregion

    #region Chromatic Orb

    internal static SpellDefinition BuildChromaticOrb()
    {
        const string NAME = "ChromaticOrb";

        var sprite = Sprites.GetSprite(NAME, Resources.ChromaticOrb, 128);
        var subSpells = new List<SpellDefinition>();

        foreach (var (damageType, magicEffect) in DamagesAndEffects)
        {
            var effectDescription = EffectDescriptionBuilder.Create(magicEffect.EffectDescription).Build();

            if (damageType == DamageTypePoison)
            {
                effectDescription.EffectParticleParameters.impactParticleReference =
                    effectDescription.EffectParticleParameters.effectParticleReference;

                effectDescription.EffectParticleParameters.effectParticleReference = new AssetReference();
            }

            var title = Gui.Localize($"Tooltip/&Tag{damageType}Title");
            var description = Gui.Format("Spell/&SubSpellChromaticOrbDescription", title);
            var spell = SpellDefinitionBuilder
                .Create(NAME + damageType)
                .SetGuiPresentation(title, description, sprite)
                .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
                .SetSpellLevel(1)
                .SetMaterialComponent(MaterialComponentType.Specific)
                .SetSpecificMaterialComponent(TagsDefinitions.ItemTagDiamond, 50, false)
                .SetVocalSpellSameType(VocalSpellSemeType.Attack)
                .SetCastingTime(ActivationTime.Action)
                .SetEffectDescription(
                    EffectDescriptionBuilder
                        .Create()
                        .SetTargetingData(Side.Enemy, RangeType.RangeHit, 12, TargetType.IndividualsUnique)
                        .SetTargetFiltering(TargetFilteringMethod.CharacterOnly)
                        .SetEffectForms(EffectFormBuilder.DamageForm(damageType, 3, DieType.D8))
                        .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel,
                            additionalDicePerIncrement: 1)
                        .SetParticleEffectParameters(effectDescription.EffectParticleParameters)
                        .SetSpeed(SpeedType.CellsPerSeconds, 8.5f)
                        .SetupImpactOffsets(offsetImpactTimePerTarget: 0.1f)
                        .Build())
                .AddToDB();

            subSpells.Add(spell);
        }

        return SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, sprite)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetSpellLevel(1)
            .SetMaterialComponent(MaterialComponentType.Specific)
            .SetSpecificMaterialComponent(TagsDefinitions.ItemTagDiamond, 50, false)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetCastingTime(ActivationTime.Action)
            .SetSubSpells(subSpells.ToArray())
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetFiltering(TargetFilteringMethod.CharacterOnly)
                    .SetTargetingData(Side.Enemy, RangeType.RangeHit, 12, TargetType.IndividualsUnique)
                    .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel,
                        additionalDicePerIncrement: 1)
                    .SetSpeed(SpeedType.CellsPerSeconds, 8.5f)
                    .SetupImpactOffsets(offsetImpactTimePerTarget: 0.1f)
                    .Build())
            .AddToDB();
    }

    #endregion

    #region Earth Tremor

    internal static SpellDefinition BuildEarthTremor()
    {
        const string NAME = "EarthTremor";

        var spriteReference = Sprites.GetSprite(NAME, Resources.EarthTremor, 128, 128);

        var rubbleProxy = EffectProxyDefinitionBuilder
            .Create(EffectProxyDefinitions.ProxyGrease, "EarthTremorRubbleProxy")
            .AddToDB();

        var effectDescription = EffectDescriptionBuilder
            .Create()
            .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, 1, 0, 1)
            .SetSavingThrowData(
                false,
                AttributeDefinitions.Dexterity,
                false,
                EffectDifficultyClassComputation.SpellCastingFeature)
            .SetDurationData(DurationType.Minute, 1)
            .SetParticleEffectParameters(Grease)
            .SetTargetingData(Side.All, RangeType.Distance, 24, TargetType.Cylinder, 2, 1)
            .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, additionalDicePerIncrement: 1)
            .SetEffectForms(
                EffectFormBuilder
                    .Create()
                    .SetSummonEffectProxyForm(rubbleProxy)
                    .Build(),
                EffectFormBuilder
                    .Create()
                    .SetMotionForm(MotionForm.MotionType.FallProne, 1)
                    .HasSavingThrow(EffectSavingThrowType.Negates)
                    .Build(),
                EffectFormBuilder
                    .Create()
                    .SetDamageForm(DamageTypeBludgeoning, 1, DieType.D6)
                    .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                    .Build(),
                Grease.EffectDescription.EffectForms.Find(e => e.formType == EffectForm.EffectFormType.Topology))
            .Build();

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, spriteReference)
            .SetEffectDescription(effectDescription)
            .SetCastingTime(ActivationTime.Action)
            .SetSpellLevel(1)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .AddToDB();

        return spell;
    }

    #endregion

    #region Ensnaring Strike

    internal static SpellDefinition BuildEnsnaringStrike()
    {
        const string NAME = "EnsnaringStrike";

        var conditionEnsnared = ConditionDefinitionBuilder
            .Create(ConditionGrappledRestrainedRemorhaz, "ConditionGrappledRestrainedEnsnared")
            .SetOrUpdateGuiPresentation(Category.Condition)
            .SetParentCondition(ConditionRestrainedByWeb)
            .SetSpecialDuration(DurationType.Minute, 1, TurnOccurenceType.StartOfTurn)
            .SetRecurrentEffectForms(
                EffectFormBuilder
                    .Create()
                    .SetDamageForm(DamageTypePiercing, 1, DieType.D6)
                    .Build())
            .AddToDB();

        conditionEnsnared.specialInterruptions.Clear();

        conditionEnsnared.conditionStartParticleReference =
            Entangle.EffectDescription.EffectParticleParameters.conditionStartParticleReference;
        conditionEnsnared.conditionParticleReference =
            Entangle.EffectDescription.EffectParticleParameters.conditionParticleReference;
        conditionEnsnared.conditionEndParticleReference =
            Entangle.EffectDescription.EffectParticleParameters.conditionEndParticleReference;

        var additionalDamageEnsnaringStrike = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{NAME}")
            .SetGuiPresentation(NAME, Category.Spell)
            .SetNotificationTag(NAME)
            .SetCustomSubFeatures(ValidatorsRestrictedContext.IsWeaponAttack)
            .SetDamageDice(DieType.D6, 0)
            .SetSavingThrowData(
                EffectDifficultyClassComputation.SpellCastingFeature,
                EffectSavingThrowType.Negates,
                AttributeDefinitions.Strength)
            .SetConditionOperations(new ConditionOperationDescription
            {
                operation = ConditionOperationDescription.ConditionOperation.Add,
                conditionDefinition = conditionEnsnared,
                hasSavingThrow = true,
                saveAffinity = EffectSavingThrowType.Negates,
                saveOccurence = TurnOccurenceType.StartOfTurn
            })
            .SetImpactParticleReference(
                Entangle.EffectDescription.EffectParticleParameters.activeEffectSurfaceStartParticleReference)
            .AddToDB();

        var conditionEnsnaringStrike = ConditionDefinitionBuilder
            .Create($"Condition{NAME}")
            .SetGuiPresentation(NAME, Category.Spell, ConditionBrandingSmite)
            .AddFeatures(additionalDamageEnsnaringStrike)
            .SetSpecialInterruptions(ConditionInterruption.AttacksAndDamages)
            .SetPossessive()
            .AddToDB();

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite("EnsnaringStrike", Resources.EnsnaringStrike, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolConjuration)
            .SetSpellLevel(1)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
            .SetCastingTime(ActivationTime.BonusAction)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    // .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, additionalDicePerIncrement: 1)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionEnsnaringStrike))
                    // .SetParticleEffectParameters(Entangle)
                    .Build())
            .SetRequiresConcentration(true)
            .AddToDB();

        spell.EffectDescription.EffectParticleParameters.casterParticleReference =
            Entangle.EffectDescription.EffectParticleParameters.casterParticleReference;

        return spell;
    }

    #endregion

    #region Mule

    internal static SpellDefinition BuildMule()
    {
        const string NAME = "Mule";

        var effectDescription = EffectDescriptionBuilder
            .Create()
            .SetTargetingData(Side.Ally, RangeType.Touch, 1, TargetType.IndividualsUnique)
            .SetDurationData(DurationType.Hour, 8)
            .SetParticleEffectParameters(ExpeditiousRetreat)
            .SetEffectForms(
                EffectFormBuilder
                    .Create()
                    .SetConditionForm(
                        ConditionDefinitionBuilder
                            .Create($"Condition{NAME}")
                            .SetGuiPresentation(Category.Condition, Longstrider)
                            .SetFeatures(
                                FeatureDefinitionMovementAffinityBuilder
                                    .Create($"MovementAffinity{NAME}")
                                    .SetGuiPresentationNoContent(true)
                                    .SetImmunities(true, true)
                                    .AddToDB(),
                                FeatureDefinitionEquipmentAffinityBuilder
                                    .Create($"EquipmentAffinity{NAME}")
                                    .SetGuiPresentationNoContent(true)
                                    .SetAdditionalCarryingCapacity(20)
                                    .AddToDB())
                            .AddToDB(),
                        ConditionForm.ConditionOperation.Add,
                        false,
                        false,
                        ConditionJump.AdditionalCondition)
                    .Build())
            .Build();

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell,
                Sprites.GetSprite("Mule", Resources.Mule, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(1)
            .SetCastingTime(ActivationTime.Action)
            .SetConcentrationAction(ActionDefinitions.ActionParameter.None)
            .SetEffectDescription(effectDescription)
            .AddToDB();

        return spell;
    }

    #endregion

    #region Radiant Motes

    internal static SpellDefinition BuildRadiantMotes()
    {
        const string NAME = "RadiantMotes";

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.RadiantMotes, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetSpellLevel(1)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetCastingTime(ActivationTime.Action)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetFiltering(TargetFilteringMethod.AllCharacterAndGadgets)
                    .SetTargetingData(Side.Enemy, RangeType.RangeHit, 12, TargetType.Individuals, 4)
                    .SetDurationData(DurationType.Minute, 1)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeRadiant, 1, DieType.D4)
                            .Build())
                    .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, 1, 1)
                    .SetParticleEffectParameters(Sparkle)
                    .SetSpeed(SpeedType.CellsPerSeconds, 20)
                    .SetupImpactOffsets(offsetImpactTimePerTarget: 0.1f)
                    .Build())
            .AddToDB();

        return spell;
    }

    #endregion

    #region Searing Smite

    internal static SpellDefinition BuildSearingSmite()
    {
        const string NAME = "SearingSmite";

        var additionalDamageSearingSmite = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{NAME}")
            .SetGuiPresentation(NAME, Category.Spell)
            .SetNotificationTag(NAME)
            .SetCustomSubFeatures(ValidatorsRestrictedContext.IsWeaponAttack)
            .SetDamageDice(DieType.D6, 1)
            .SetSpecificDamageType(DamageTypeFire)
            .SetAdvancement(AdditionalDamageAdvancement.SlotLevel, 1)
            .SetSavingThrowData(
                EffectDifficultyClassComputation.SpellCastingFeature,
                EffectSavingThrowType.None)
            .SetConditionOperations(
                new ConditionOperationDescription
                {
                    hasSavingThrow = true,
                    canSaveToCancel = true,
                    saveAffinity = EffectSavingThrowType.Negates,
                    saveOccurence = TurnOccurenceType.StartOfTurn,
                    conditionDefinition = ConditionDefinitionBuilder
                        .Create(ConditionOnFire, $"Condition{NAME}Enemy")
                        .AddToDB(),
                    operation = ConditionOperationDescription.ConditionOperation.Add
                })
            .SetImpactParticleReference(FireBolt.EffectDescription.EffectParticleParameters.impactParticleReference)
            .AddToDB();

        var conditionSearingSmite = ConditionDefinitionBuilder
            .Create($"Condition{NAME}")
            .SetGuiPresentation(NAME, Category.Spell, ConditionBrandingSmite)
            .SetPossessive()
            .SetFeatures(additionalDamageSearingSmite)
            .SetSpecialInterruptions(ConditionInterruption.AttacksAndDamages)
            .AddToDB();

        var spell = SpellDefinitionBuilder
            .Create(BrandingSmite, NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.SearingSmite, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetSpellLevel(1)
            .SetCastingTime(ActivationTime.BonusAction)
            .SetVerboseComponent(true)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, additionalDicePerIncrement: 1)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionSearingSmite))
                    .SetParticleEffectParameters(FireBolt)
                    .Build())
            .AddToDB();

        return spell;
    }

    #endregion

    #region Wrathful Smite

    internal static SpellDefinition BuildWrathfulSmite()
    {
        const string NAME = "WrathfulSmite";

        var additionalDamageWrathfulSmite = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{NAME}")
            .SetGuiPresentation(NAME, Category.Spell)
            .SetNotificationTag(NAME)
            .SetCustomSubFeatures(ValidatorsRestrictedContext.IsWeaponAttack)
            .SetDamageDice(DieType.D6, 1)
            .SetSpecificDamageType(DamageTypePsychic)
            .SetAdvancement(AdditionalDamageAdvancement.SlotLevel, 1)
            .SetSavingThrowData(
                EffectDifficultyClassComputation.SpellCastingFeature,
                EffectSavingThrowType.None,
                AttributeDefinitions.Wisdom)
            .SetConditionOperations(
                new ConditionOperationDescription
                {
                    hasSavingThrow = true,
                    canSaveToCancel = true,
                    saveAffinity = EffectSavingThrowType.Negates,
                    saveOccurence = TurnOccurenceType.StartOfTurn,
                    conditionDefinition = ConditionDefinitionBuilder
                        .Create(ConditionDefinitions.ConditionFrightened, $"Condition{NAME}Enemy")
                        .SetSpecialDuration(DurationType.Minute, 1, TurnOccurenceType.StartOfTurn)
                        .SetParentCondition(ConditionDefinitions.ConditionFrightened)
                        .AddToDB(),
                    operation = ConditionOperationDescription.ConditionOperation.Add
                })
            .SetImpactParticleReference(Fear.EffectDescription.EffectParticleParameters.impactParticleReference)
            .AddToDB();

        var conditionWrathfulSmite = ConditionDefinitionBuilder
            .Create($"Condition{NAME}")
            .SetGuiPresentation(NAME, Category.Spell, ConditionBrandingSmite)
            .SetPossessive()
            .SetFeatures(additionalDamageWrathfulSmite)
            .SetSpecialInterruptions(ConditionInterruption.AttacksAndDamages)
            .AddToDB();

        var spell = SpellDefinitionBuilder
            .Create(BrandingSmite, NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.WrathfulSmite, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetSpellLevel(1)
            .SetCastingTime(ActivationTime.BonusAction)
            .SetVerboseComponent(true)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    // .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, additionalDicePerIncrement: 1)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionWrathfulSmite))
                    .SetParticleEffectParameters(Fear)
                    .Build())
            .AddToDB();

        return spell;
    }

    #endregion

    #region Magnify Gravity

    internal static SpellDefinition BuildMagnifyGravity()
    {
        const string NAME = "MagnifyGravity";

        var movementAffinityMagnifyGravity = FeatureDefinitionMovementAffinityBuilder
            .Create($"MovementAffinity{NAME}")
            .SetGuiPresentationNoContent(true)
            .SetBaseSpeedMultiplicativeModifier(0.5f)
            .AddToDB();

        var conditionGravity = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionEncumbered, "ConditionGravity")
            .SetOrUpdateGuiPresentation(Category.Condition)
            .SetSpecialDuration(DurationType.Round, 1)
            .SetFeatures(movementAffinityMagnifyGravity)
            .AddToDB();

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.MagnifyGravity, 128))
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.All, RangeType.Distance, 12, TargetType.Sphere, 2)
                    .SetDurationData(DurationType.Round, 1)
                    .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, additionalDicePerIncrement: 1)
                    .SetSavingThrowData(
                        false,
                        AttributeDefinitions.Constitution,
                        true,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetParticleEffectParameters(Shatter.EffectDescription.EffectParticleParameters)
                    .AddEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeForce, 2, DieType.D8)
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionGravity, ConditionForm.ConditionOperation.Add)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build())
                    .Build())
            .SetCastingTime(ActivationTime.Action)
            .SetSpellLevel(1)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .AddToDB();

        return spell;
    }

    #endregion

    #region Skin of Retribution

    internal static SpellDefinition BuildSkinOfRetribution()
    {
        const string NAME = "SkinOfRetribution";
        const int TEMP_HP_PER_LEVEL = 5;

        var spriteReferenceCondition = Sprites.GetSprite("ConditionMirrorImage", Resources.ConditionMirrorImage, 32);
        var subSpells = new List<SpellDefinition>();
        var conditions = new List<ConditionDefinition>();

        const string SUB_SPELL_DESCRIPTION = $"Spell/&SubSpell{NAME}Description";
        const string SUB_SPELL_CONDITION_DESCRIPTION = $"Condition/&Condition{NAME}Description";
        const string SUB_SPELL_CONDITION_TITLE = $"Condition/&Condition{NAME}Title";

        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach (var (damageType, magicEffect) in DamagesAndEffects)
        {
            var effectDescription = EffectDescriptionBuilder.Create(magicEffect.EffectDescription).Build();

            if (damageType == DamageTypePoison)
            {
                effectDescription.EffectParticleParameters.impactParticleReference =
                    effectDescription.EffectParticleParameters.effectParticleReference;

                effectDescription.EffectParticleParameters.effectParticleReference = new AssetReference();
            }

            var title = Gui.Localize($"Tooltip/&Tag{damageType}Title");

            var powerSkinOfRetribution = FeatureDefinitionPowerBuilder
                .Create($"Power{NAME}{damageType}")
                .SetGuiPresentationNoContent(true)
                .SetUsesFixed(ActivationTime.NoCost)
                .SetEffectDescription(
                    EffectDescriptionBuilder
                        .Create()
                        .SetEffectForms(EffectFormBuilder.DamageForm(damageType, bonusDamage: TEMP_HP_PER_LEVEL))
                        .SetParticleEffectParameters(effectDescription.EffectParticleParameters)
                        .Build())
                .AddToDB();

            var damageSkinOfRetribution = FeatureDefinitionDamageAffinityBuilder
                .Create($"DamageAffinity{NAME}{damageType}")
                .SetGuiPresentationNoContent(true)
                .SetDamageAffinityType(DamageAffinityType.None)
                .SetRetaliate(powerSkinOfRetribution, 1, true)
                .AddToDB();

            var conditionSkinOfRetribution = ConditionDefinitionBuilder
                .Create($"Condition{NAME}{damageType}")
                .SetGuiPresentation(
                    SUB_SPELL_CONDITION_TITLE,
                    Gui.Format(SUB_SPELL_CONDITION_DESCRIPTION, title),
                    spriteReferenceCondition)
                .SetSilent(Silent.WhenAdded)
                .SetPossessive()
                .SetFeatures(damageSkinOfRetribution)
                .SetCancellingConditions()
                .AddToDB();

            conditions.Add(conditionSkinOfRetribution);

            powerSkinOfRetribution.SetCustomSubFeatures(
                new ModifyEffectDescriptionSkinOfRetribution(conditionSkinOfRetribution));

            var spell = SpellDefinitionBuilder
                .Create(NAME + damageType)
                .SetGuiPresentation(title, Gui.Format(SUB_SPELL_DESCRIPTION, title),
                    Sprites.GetSprite(NAME, Resources.SkinOfRetribution, 128))
                .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolAbjuration)
                .SetVerboseComponent(false)
                .SetVocalSpellSameType(VocalSpellSemeType.Defense)
                .SetSpellLevel(1)
                .SetUniqueInstance()
                .SetEffectDescription(
                    EffectDescriptionBuilder
                        .Create()
                        .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                        .SetDurationData(DurationType.Hour, 1)
                        .SetEffectForms(
                            EffectFormBuilder
                                .Create()
                                .SetTempHpForm(TEMP_HP_PER_LEVEL)
                                .Build(),
                            EffectFormBuilder.ConditionForm(conditionSkinOfRetribution))
                        .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel,
                            additionalTempHpPerIncrement: TEMP_HP_PER_LEVEL)
                        .SetParticleEffectParameters(effectDescription.EffectParticleParameters)
                        .Build())
                .AddToDB();

            subSpells.Add(spell);
        }

        foreach (var condition in conditions)
        {
            condition.cancellingConditions = conditions.Where(x => x != condition).ToList();
        }

        return SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.SkinOfRetribution, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolAbjuration)
            .SetVerboseComponent(false)
            .SetVocalSpellSameType(VocalSpellSemeType.Defense)
            .SetSpellLevel(1)
            .SetSubSpells(subSpells.ToArray())
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetDurationData(DurationType.Hour, 1)
                    .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel,
                        additionalTempHpPerIncrement: TEMP_HP_PER_LEVEL)
                    .SetParticleEffectParameters(Blur)
                    .Build())
            .AddToDB();
    }

    private sealed class ModifyEffectDescriptionSkinOfRetribution : IModifyEffectDescription
    {
        private readonly ConditionDefinition _conditionSkinOfRetribution;

        public ModifyEffectDescriptionSkinOfRetribution(ConditionDefinition conditionSkinOfRetribution)
        {
            _conditionSkinOfRetribution = conditionSkinOfRetribution;
        }

        public bool IsValid(
            BaseDefinition definition,
            RulesetCharacter character,
            EffectDescription effectDescription)
        {
            return effectDescription.HasDamageForm() && character.HasConditionOfType(_conditionSkinOfRetribution);
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            var rulesetCondition = character.AllConditions
                .FirstOrDefault(x => x.ConditionDefinition == _conditionSkinOfRetribution);

            var effectLevel = rulesetCondition!.EffectLevel;
            var damageForm = effectDescription.FindFirstDamageForm();

            damageForm.bonusDamage *= effectLevel;

            MaybeRemoveSkinOfRetribution(character);

            return effectDescription;
        }

        private void MaybeRemoveSkinOfRetribution(RulesetCharacter character)
        {
            if (character.temporaryHitPoints > 0)
            {
                return;
            }

            var rulesetCondition = character.AllConditions
                .FirstOrDefault(x => x.ConditionDefinition == _conditionSkinOfRetribution);

            character.RemoveCondition(rulesetCondition);
        }
    }

    #endregion

    #region Sanctuary

    internal static SpellDefinition BuildSanctuary()
    {
        const string NAME = "Sanctuary";

        var conditionSanctuary = ConditionDefinitionBuilder
            .Create($"Condition{NAME}")
            .SetGuiPresentation(Category.Condition, ConditionDivineFavor)
            .AddSpecialInterruptions(ConditionInterruption.Attacks)
            .AddToDB();

        var conditionSanctuaryReduceDamage = ConditionDefinitionBuilder
            .Create($"Condition{NAME}ReduceDamage")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddFeatures(
                FeatureDefinitionReduceDamageBuilder
                    .Create($"ReduceDamage{NAME}")
                    .SetGuiPresentation(NAME, Category.Spell)
                    .SetAlwaysActiveReducedDamage((_, _) => Int32.MaxValue)
                    .AddToDB())
            .AddSpecialInterruptions(ConditionInterruption.Attacked)
            .AddToDB();

        var conditionSanctuaryDamageResistance = ConditionDefinitionBuilder
            .Create($"Condition{NAME}DamageResistance")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddFeatures(
                DamageAffinityAcidResistance,
                DamageAffinityBludgeoningResistance,
                DamageAffinityColdResistance,
                DamageAffinityFireResistance,
                DamageAffinityForceDamageResistance,
                DamageAffinityLightningResistance,
                DamageAffinityNecroticResistance,
                DamageAffinityPiercingResistance,
                DamageAffinityPoisonResistance,
                DamageAffinityPsychicResistance,
                DamageAffinityRadiantResistance,
                DamageAffinitySlashingResistance,
                DamageAffinityThunderResistance)
            .AddSpecialInterruptions(ConditionInterruption.Attacked)
            .AddToDB();

        var featureSanctuary = FeatureDefinitionBuilder
            .Create($"Feature{NAME}")
            .SetGuiPresentation(Category.Feature)
            .SetCustomSubFeatures(
                new AttackBeforeHitConfirmedOnMeSanctuary(
                    conditionSanctuary,
                    conditionSanctuaryReduceDamage,
                    conditionSanctuaryDamageResistance))
            .AddToDB();

        conditionSanctuary.Features.Add(featureSanctuary);

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell,
                Sprites.GetSprite("Sanctuary", Resources.Sanctuary, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolAbjuration)
            .SetSpellLevel(1)
            .SetCastingTime(ActivationTime.BonusAction)
            .SetVerboseComponent(true)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetDurationData(DurationType.Minute, 1)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionSanctuary))
                    .SetParticleEffectParameters(ProtectionFromEvilGood)
                    .Build())
            .AddToDB();

        return spell;
    }

    private sealed class AttackBeforeHitConfirmedOnMeSanctuary : IAttackBeforeHitConfirmedOnMe
    {
        private readonly ConditionDefinition _conditionReduceDamage;
        private readonly ConditionDefinition _conditionResistance;
        private readonly ConditionDefinition _conditionSanctuary;

        internal AttackBeforeHitConfirmedOnMeSanctuary(
            ConditionDefinition conditionSanctuary,
            ConditionDefinition conditionReduceDamage,
            ConditionDefinition conditionResistance)
        {
            _conditionSanctuary = conditionSanctuary;
            _conditionReduceDamage = conditionReduceDamage;
            _conditionResistance = conditionResistance;
        }

        public IEnumerator OnAttackBeforeHitConfirmedOnMe(
            GameLocationBattleManager battle,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            RulesetAttackMode attackMode,
            bool rangedAttack,
            AdvantageType advantageType,
            List<EffectForm> actualEffectForms,
            RulesetEffect rulesetEffect,
            bool firstTarget,
            bool criticalHit)
        {
            if (!battle.IsBattleInProgress)
            {
                yield break;
            }

            var rulesetDefender = defender.RulesetCharacter;

            if (rulesetDefender is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            var usableCondition =
                rulesetDefender.AllConditions.FirstOrDefault(x => x.ConditionDefinition == _conditionSanctuary);

            if (usableCondition == null)
            {
                yield break;
            }

            var rulesetCaster = EffectHelpers.GetCharacterByGuid(usableCondition.SourceGuid);

            if (rulesetCaster is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            var modifierTrend = attacker.RulesetCharacter.actionModifier.savingThrowModifierTrends;
            var advantageTrends = attacker.RulesetCharacter.actionModifier.savingThrowAdvantageTrends;

            var attackerWisModifier = AttributeDefinitions.ComputeAbilityScoreModifier(attacker.RulesetCharacter
                .TryGetAttributeValue(AttributeDefinitions.Wisdom));

            var casterProfBonus = AttributeDefinitions.ComputeProficiencyBonus(rulesetCaster
                .TryGetAttributeValue(AttributeDefinitions.CharacterLevel));
            var casterWisModifier = AttributeDefinitions.ComputeAbilityScoreModifier(rulesetCaster
                .TryGetAttributeValue(AttributeDefinitions.Wisdom));

            attacker.RulesetCharacter.RollSavingThrow(0, AttributeDefinitions.Wisdom, null, modifierTrend,
                advantageTrends, attackerWisModifier, 8 + casterProfBonus + casterWisModifier, false,
                out var savingOutcome,
                out _);

            if (savingOutcome is RollOutcome.Success or RollOutcome.CriticalSuccess)
            {
                yield break;
            }

            var condition = criticalHit ? _conditionResistance : _conditionReduceDamage;

            rulesetDefender.InflictCondition(
                condition.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.StartOfTurn,
                AttributeDefinitions.TagCombat,
                rulesetDefender.guid,
                rulesetDefender.CurrentFaction.Name,
                1,
                null,
                0,
                0,
                0);
        }
    }

    #endregion

    #region Owl Familiar

    internal const string OwlFamiliar = "OwlFamiliar";

    internal static SpellDefinition BuildFindFamiliar()
    {
        var familiarMonster = MonsterDefinitionBuilder
            .Create(MonsterDefinitions.Eagle_Matriarch, OwlFamiliar)
            .SetOrUpdateGuiPresentation(Category.Monster)
            .SetFeatures(
                FeatureDefinitionSenses.SenseNormalVision,
                FeatureDefinitionSenses.SenseDarkvision24,
                FeatureDefinitionMoveModes.MoveModeMove2,
                FeatureDefinitionMoveModes.MoveModeFly12,
                FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityKeenSight,
                FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityKeenHearing,
                FeatureDefinitionCombatAffinitys.CombatAffinityFlyby,
                FeatureDefinitionMovementAffinitys.MovementAffinityNoClimb,
                FeatureDefinitionMovementAffinitys.MovementAffinityNoSpecialMoves,
                FeatureDefinitionConditionAffinitys.ConditionAffinityProneImmunity)
            .SetMonsterPresentation(
                MonsterPresentationBuilder
                    .Create()
                    .SetAllPrefab(MonsterDefinitions.Eagle_Matriarch.MonsterPresentation)
                    .SetPhantom()
                    .SetModelScale(0.5f)
                    .SetHasMonsterPortraitBackground(true)
                    .SetCanGeneratePortrait(true)
                    .Build())
            .ClearAttackIterations()
            .SetSkillScores((SkillDefinitions.Perception, 3), (SkillDefinitions.Stealth, 3))
            .SetArmorClass(11)
            .SetAbilityScores(3, 13, 8, 2, 12, 7)
            .SetHitDice(DieType.D4, 1)
            .SetStandardHitPoints(5)
            .SetSizeDefinition(CharacterSizeDefinitions.Tiny)
            .SetAlignment("Neutral")
            .SetCharacterFamily("Fey")
            .SetChallengeRating(0)
            .SetDroppedLootDefinition(null)
            .SetDefaultBattleDecisionPackage(DecisionPackageDefinitions.DefaultSupportCasterWithBackupAttacksDecisions)
            .SetFullyControlledWhenAllied(true)
            .SetDefaultFaction(FactionDefinitions.Party)
            .SetBestiaryEntry(BestiaryDefinitions.BestiaryEntry.None)
            .AddFeatures(CharacterContext.FeatureDefinitionPowerHelpAction)
            .AddToDB();

        var spell = SpellDefinitionBuilder.Create(Fireball, "FindFamiliar")
            .SetGuiPresentation(Category.Spell, AnimalFriendship)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolConjuration)
            .SetMaterialComponent(MaterialComponentType.Specific)
            .SetSomaticComponent(true)
            .SetVerboseComponent(true)
            .SetSpellLevel(1)
            .SetUniqueInstance()
            .SetCastingTime(ActivationTime.Hours1)
            .SetRitualCasting(ActivationTime.Hours1)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Permanent)
                    .SetTargetingData(Side.Ally, RangeType.Distance, 2, TargetType.Position)
                    .SetParticleEffectParameters(ConjureAnimalsOneBeast)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetSummonCreatureForm(1, familiarMonster.Name)
                            .Build())
                    .Build())
            .SetCustomSubFeatures(SkipEffectRemovalOnLocationChange.Always)
            .AddToDB();

        GlobalUniqueEffects.AddToGroup(GlobalUniqueEffects.Group.Familiar, spell);

        return spell;
    }

    #endregion

    #region Thunderous Smite

    internal static SpellDefinition BuildThunderousSmite()
    {
        const string NAME = "ThunderousSmite";

        var power = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}Push")
            .SetGuiPresentation(NAME, Category.Spell, hidden: true)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Touch, 1, TargetType.IndividualsUnique)
                    .SetParticleEffectParameters(Shatter)
                    .SetEffectForms(
                        EffectFormBuilder.Create()
                            .SetMotionForm(MotionForm.MotionType.PushFromOrigin, 2)
                            .Build(),
                        EffectFormBuilder.Create()
                            .SetMotionForm(MotionForm.MotionType.FallProne)
                            .Build())
                    .Build())
            .AddToDB();

        var additionalDamageThunderousSmite = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{NAME}")
            .SetGuiPresentation(NAME, Category.Spell)
            .SetNotificationTag(NAME)
            .SetCustomSubFeatures(ValidatorsRestrictedContext.IsWeaponAttack)
            .SetDamageDice(DieType.D6, 2)
            .SetSpecificDamageType(DamageTypeThunder)
            .SetSavingThrowData(
                EffectDifficultyClassComputation.SpellCastingFeature,
                EffectSavingThrowType.None,
                AttributeDefinitions.Strength)
            .SetConditionOperations(
                new ConditionOperationDescription
                {
                    hasSavingThrow = true,
                    canSaveToCancel = true,
                    saveAffinity = EffectSavingThrowType.Negates,
                    saveOccurence = TurnOccurenceType.StartOfTurn,
                    conditionDefinition = ConditionDefinitionBuilder
                        .Create($"Condition{NAME}Enemy")
                        .SetGuiPresentationNoContent(true)
                        .SetSilent(Silent.WhenAddedOrRemoved)
                        .SetCustomSubFeatures(new ConditionUsesPowerOnTarget(power))
                        .AddToDB(),
                    operation = ConditionOperationDescription.ConditionOperation.Add
                })
            .SetImpactParticleReference(Shatter)
            .AddToDB();

        var conditionThunderousSmite = ConditionDefinitionBuilder
            .Create($"Condition{NAME}")
            .SetGuiPresentation(NAME, Category.Spell, ConditionBrandingSmite)
            .SetPossessive()
            .SetFeatures(additionalDamageThunderousSmite)
            .SetSpecialInterruptions(ConditionInterruption.AttacksAndDamages)
            .AddToDB();

        var spell = SpellDefinitionBuilder
            .Create(BrandingSmite, NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.ThunderousSmite, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetSpellLevel(1)
            .SetCastingTime(ActivationTime.BonusAction)
            .SetVerboseComponent(true)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    // .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, additionalDicePerIncrement: 1)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionThunderousSmite))
                    .SetParticleEffectParameters(Shatter)
                    .Build())
            .AddToDB();

        return spell;
    }

    private sealed class ConditionUsesPowerOnTarget : IOnConditionAddedOrRemoved
    {
        private readonly FeatureDefinitionPower _power;
        private readonly bool _removeCondition;

        public ConditionUsesPowerOnTarget(FeatureDefinitionPower power, bool removeCondition = true)
        {
            _power = power;
            _removeCondition = removeCondition;
        }

        public void OnConditionAdded(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            var defender = GameLocationCharacter.GetFromActor(target);
            var rulesetAttacker = EffectHelpers.GetCharacterByGuid(rulesetCondition.SourceGuid);

            if (rulesetAttacker is not { IsDeadOrDyingOrUnconscious: false } || defender == null)
            {
                return;
            }

            var usablePower = UsablePowersProvider.Get(_power, rulesetAttacker);
            var effectPower = ServiceRepository.GetService<IRulesetImplementationService>()
                .InstantiateEffectPower(rulesetAttacker, usablePower, false)
                .AddAsActivePowerToSource();

            effectPower.ApplyEffectOnCharacter(target, true, defender.LocationPosition);

            if (_removeCondition)
            {
                target.RemoveCondition(rulesetCondition);
            }
        }

        public void OnConditionRemoved(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            // empty
        }
    }

    #endregion

    #region GiftOfAlacrity

    internal static SpellDefinition BuildGiftOfAlacrity()
    {
        const string NAME = "GiftOfAlacrity";

        var featureGiftOfAlacrity = FeatureDefinitionBuilder
            .Create("FeatureGiftOfAlacrity")
            .SetGuiPresentation("ConditionGiftOfAlacrity", Category.Condition)
            .AddToDB();

        featureGiftOfAlacrity.SetCustomSubFeatures(new InitiativeEndListenerGiftOfAlacrity(featureGiftOfAlacrity));

        var conditionAlacrity = ConditionDefinitionBuilder
            .Create(ConditionBlessed, "ConditionGiftOfAlacrity")
            .SetOrUpdateGuiPresentation(Category.Condition)
            .SetPossessive()
            .SetFeatures(featureGiftOfAlacrity)
            .AddToDB();

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, CalmEmotions)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Hour, 8)
                    .SetTargetingData(Side.Ally, RangeType.Touch, 1, TargetType.IndividualsUnique)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionAlacrity, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .SetParticleEffectParameters(MageArmor)
                    .Build())
            .SetCastingTime(ActivationTime.Minute1)
            .SetSpellLevel(1)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolDivination)
            .AddToDB();

        return spell;
    }

    private sealed class InitiativeEndListenerGiftOfAlacrity : IInitiativeEndListener
    {
        private readonly FeatureDefinition _featureDefinition;

        public InitiativeEndListenerGiftOfAlacrity(FeatureDefinition featureDefinition)
        {
            _featureDefinition = featureDefinition;
        }

        public IEnumerator OnInitiativeEnded(GameLocationCharacter locationCharacter)
        {
            const string TEXT = "Feedback/&FeatureGiftOfAlacrityLine";

            var gameLocationScreenBattle = Gui.GuiService.GetScreen<GameLocationScreenBattle>();

            if (Gui.Battle == null || gameLocationScreenBattle == null)
            {
                yield break;
            }

            var rulesetCharacter = locationCharacter.RulesetCharacter;
            var roll = rulesetCharacter.RollDie(DieType.D8, RollContext.None, false, AdvantageType.None, out _, out _);

            gameLocationScreenBattle.initiativeTable.ContenderModified(locationCharacter,
                GameLocationBattle.ContenderModificationMode.Remove, false, false);

            locationCharacter.LastInitiative += roll;
            Gui.Battle.initiativeSortedContenders.Sort(Gui.Battle);

            gameLocationScreenBattle.initiativeTable.ContenderModified(locationCharacter,
                GameLocationBattle.ContenderModificationMode.Add, false, false);

            locationCharacter.RulesetCharacter.LogCharacterUsedFeature(
                _featureDefinition,
                TEXT,
                false,
                (ConsoleStyleDuplet.ParameterType.Initiative, roll.ToString()));
        }
    }

    #endregion
}
