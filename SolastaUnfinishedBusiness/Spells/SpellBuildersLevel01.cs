using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
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
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionActionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionMovementAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
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
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
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
                .SetCastingTime(ActivationTime.Action)
                .SetMaterialComponent(MaterialComponentType.Specific)
                .SetSpecificMaterialComponent(TagsDefinitions.ItemTagDiamond, 50, false)
                .SetVerboseComponent(true)
                .SetSomaticComponent(true)
                .SetVocalSpellSameType(VocalSpellSemeType.Attack)
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
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetSpellLevel(1)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetEffectDescription(effectDescription)
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
            .AddCustomSubFeatures(ValidatorsRestrictedContext.IsWeaponAttack)
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
            .SetCastingTime(ActivationTime.BonusAction)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetVerboseComponent(true)
            .SetSomaticComponent(false)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
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
                                    .SetGuiPresentation($"Condition{NAME}", Category.Condition, Gui.NoLocalization)
                                    .SetImmunities(true, true)
                                    .AddToDB(),
                                FeatureDefinitionEquipmentAffinityBuilder
                                    .Create($"EquipmentAffinity{NAME}")
                                    .SetGuiPresentation($"Condition{NAME}", Category.Condition, Gui.NoLocalization)
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
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite("Mule", Resources.Mule, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(1)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
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
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
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
            .AddCustomSubFeatures(ValidatorsRestrictedContext.IsWeaponAttack)
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
            .SetMaterialComponent(MaterialComponentType.None)
            .SetVerboseComponent(true)
            .SetSomaticComponent(false)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
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
            .AddCustomSubFeatures(ValidatorsRestrictedContext.IsWeaponAttack)
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
            .SetMaterialComponent(MaterialComponentType.None)
            .SetVerboseComponent(true)
            .SetSomaticComponent(false)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
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
            .SetGuiPresentation("ConditionGravity", Category.Condition, Gui.NoLocalization)
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
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(1)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Debuff)
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
            .AddToDB();

        return spell;
    }

    #endregion

    #region Void's Grasp

    internal static SpellDefinition BuildVoidGrasp()
    {
        const string NAME = "VoidGrasp";

        var actionAffinityVoidGrasp = FeatureDefinitionActionAffinityBuilder
            .Create($"ActionAffinity{NAME}")
            .SetGuiPresentationNoContent(true)
            .SetAllowedActionTypes(reaction: false)
            .AddToDB();

        var conditionVoidGrasp = ConditionDefinitionBuilder
            .Create($"Condition{NAME}")
            .SetGuiPresentation(NAME, Category.Spell, ConditionBaned)
            .SetPossessive()
            .SetConditionType(ConditionType.Detrimental)
            .SetFeatures(actionAffinityVoidGrasp)
            .AddToDB();

        conditionVoidGrasp.GuiPresentation.Description = Gui.NoLocalization;

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.VoidGrasp, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetSpellLevel(1)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
                    .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Sphere, 2)
                    .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, additionalDicePerIncrement: 1)
                    .SetSavingThrowData(false, AttributeDefinitions.Strength, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .ExcludeCaster()
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .SetDamageForm(DamageTypeNecrotic, 2, DieType.D6)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetConditionForm(conditionVoidGrasp, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .SetParticleEffectParameters(ChillTouch)
                    .Build())
            .AddToDB();

        return spell;
    }

    #endregion

    #region Vile Brew

    internal static SpellDefinition BuildVileBrew()
    {
        const string NAME = "VileBrew";

        var conditionVileBrew = ConditionDefinitionBuilder
            .Create(ConditionOnAcidPilgrim, $"Condition{NAME}")
            .SetGuiPresentation(Category.Condition, ConditionAcidArrowed)
            .SetConditionType(ConditionType.Detrimental)
            .SetFeatures(MovementAffinityConditionRestrained, ActionAffinityConditionRestrained, ActionAffinityGrappled)
            .SetRecurrentEffectForms(EffectFormBuilder.DamageForm(DamageTypeAcid, 2, DieType.D4))
            .AddToDB();

        conditionVileBrew.possessive = false;
        conditionVileBrew.specialDuration = false;

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.VileBrew, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetSpellLevel(1)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetRequiresConcentration(true)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Line, 6)
                .SetDurationData(DurationType.Minute, 1, TurnOccurenceType.StartOfTurn)
                .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, additionalDicePerIncrement: 2)
                .SetSavingThrowData(false, AttributeDefinitions.Dexterity, false,
                    EffectDifficultyClassComputation.SpellCastingFeature)
                .SetEffectForms(
                    EffectFormBuilder
                        .Create()
                        .HasSavingThrow(EffectSavingThrowType.Negates, TurnOccurenceType.StartOfTurn)
                        .SetConditionForm(conditionVileBrew, ConditionForm.ConditionOperation.Add)
                        .Build())
                .SetParticleEffectParameters(AcidSplash)
                .Build())
            .AddToDB();

        return spell;
    }

    #endregion

    #region Elemental Infusion

    internal static SpellDefinition BuildElementalInfusion()
    {
        const string NAME = "ElementalInfusion";

        foreach (var (damageType, magicEffect) in DamagesAndEffects
                     .Where(x => x.Item1 != DamageTypePoison))
        {
            var damageTitle = Gui.Localize($"Tooltip/&Tag{damageType}Title");

            var shortDamageType = damageType.Substring(6);

            var additionalDamage = FeatureDefinitionAdditionalDamageBuilder
                .Create($"AdditionalDamage{NAME}{shortDamageType}")
                .SetGuiPresentationNoContent(true)
                .SetNotificationTag($"{NAME}{shortDamageType}")
                .SetDamageDice(DieType.D6, 1)
                .SetAdvancement((AdditionalDamageAdvancement)ExtraAdditionalDamageAdvancement.ConditionAmount, 1)
                .SetSpecificDamageType(damageType)
                .SetImpactParticleReference(
                    magicEffect.EffectDescription.EffectParticleParameters.impactParticleReference)
                .AddCustomSubFeatures(ValidatorsRestrictedContext.IsMeleeAttack)
                .AddToDB();

            var title = $"Condition{NAME}Title".Formatted(Category.Condition, damageTitle);

            var description = $"Condition{NAME}DamageDescription".Formatted(Category.Condition, damageTitle);
            var conditionElementalInfusionAdditionalDamage = ConditionDefinitionBuilder
                .Create($"Condition{NAME}{shortDamageType}Damage")
                .SetGuiPresentation(title, description, ConditionDivineFavor)
                .SetPossessive()
                .SetSilent(Silent.WhenAdded)
                .SetFixedAmount(1)
                .SetFeatures(additionalDamage)
                .AddToDB();

            conditionElementalInfusionAdditionalDamage.AddCustomSubFeatures(
                new CustomBehaviorConditionElementalInfusion(conditionElementalInfusionAdditionalDamage));

            description = $"Condition{NAME}ResistanceDescription".Formatted(Category.Condition, damageTitle);
            var conditionElementalInfusionResistance = ConditionDefinitionBuilder
                .Create($"Condition{NAME}{shortDamageType}Resistance")
                .SetGuiPresentation(title, description, ConditionProtectedInsideMagicCircle)
                .SetPossessive()
                .SetSilent(Silent.WhenRemoved)
                .SetFixedAmount(1)
                .SetFeatures(
                    additionalDamage,
                    GetDefinition<FeatureDefinitionDamageAffinity>($"DamageAffinity{shortDamageType}Resistance"))
                .AddToDB();

            conditionElementalInfusionResistance.AddCustomSubFeatures(
                new OnConditionAddedOrRemovedElementalInfusionResistance(),
                new CustomBehaviorConditionElementalInfusion(conditionElementalInfusionResistance));
        }

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.ElementalInfusion, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolAbjuration)
            .SetSpellLevel(1)
            .SetCastingTime(ActivationTime.Reaction)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetVerboseComponent(false)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Self)
                    .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, 1, 1)
                    .SetParticleEffectParameters(ConjureElemental)
                    .Build())
            .AddToDB();

        spell.AddCustomSubFeatures(new AttackBeforeHitPossibleOnMeOrAllyElementalInfusion(spell));

        return spell;
    }

    private sealed class CustomBehaviorConditionElementalInfusion :
        IPhysicalAttackFinishedByMe, IMagicalAttackFinishedByMe
    {
        private readonly ConditionDefinition _conditionElementalInfusion;

        public CustomBehaviorConditionElementalInfusion(ConditionDefinition conditionElementalInfusion)
        {
            _conditionElementalInfusion = conditionElementalInfusion;
        }

        public IEnumerator OnMagicalAttackFinishedByMe(
            CharacterActionMagicEffect action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender)
        {
            if (action.ActionParams.activeEffect.EffectDescription.RangeType is RangeType.Touch or RangeType.MeleeHit
                && action.AttackRollOutcome is RollOutcome.Success or RollOutcome.CriticalSuccess)
            {
                attacker.RulesetCharacter.RemoveAllConditionsOfType(_conditionElementalInfusion.Name);
            }

            yield break;
        }

        public IEnumerator OnAttackFinishedByMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackerAttackMode,
            RollOutcome attackRollOutcome,
            int damageAmount)
        {
            if (attackRollOutcome is RollOutcome.Success or RollOutcome.CriticalSuccess)
            {
                attacker.RulesetCharacter.RemoveAllConditionsOfType(_conditionElementalInfusion.Name);
            }

            yield break;
        }
    }

    private sealed class OnConditionAddedOrRemovedElementalInfusionResistance : IOnConditionAddedOrRemoved
    {
        public void OnConditionAdded(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            // empty
        }

        public void OnConditionRemoved(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            // only add the damage condition if in my own turn
            if (Gui.Battle == null || Gui.Battle.ActiveContenderIgnoringLegendary.RulesetCharacter != target)
            {
                return;
            }

            var name = rulesetCondition.ConditionDefinition.Name.Replace("Resistance", "Damage");

            target.InflictCondition(
                name,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagCombat,
                target.guid,
                target.CurrentFaction.Name,
                1,
                name,
                rulesetCondition.Amount,
                0,
                0);
        }
    }

    private sealed class AttackBeforeHitPossibleOnMeOrAllyElementalInfusion :
        IAttackBeforeHitConfirmedOnMe, IMagicalAttackBeforeHitConfirmedOnMe
    {
        private static readonly IEnumerable<string> AllowedDamageTypes = DamagesAndEffects
            .Where(x => x.Item1 != DamageTypePoison)
            .Select(x => x.Item1);

        private readonly SpellDefinition _spellDefinition;

        public AttackBeforeHitPossibleOnMeOrAllyElementalInfusion(SpellDefinition spellDefinition)
        {
            _spellDefinition = spellDefinition;
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
            if (attackMode != null)
            {
                yield return HandleReaction(defender, actualEffectForms);
            }
        }

        public IEnumerator OnMagicalAttackBeforeHitConfirmedOnMe(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier magicModifier,
            RulesetEffect rulesetEffect,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            yield return HandleReaction(defender, actualEffectForms);
        }

        private IEnumerator HandleReaction(
            GameLocationCharacter defender,
            IEnumerable<EffectForm> actualEffectForms)
        {
            var battleManager = ServiceRepository.GetService<IGameLocationBattleService>() as GameLocationBattleManager;

            if (battleManager is not { IsBattleInProgress: true })
            {
                yield break;
            }

            if (!defender.CanReact())
            {
                yield break;
            }

            var attackDamageTypes = actualEffectForms
                .Where(x => x.FormType == EffectForm.EffectFormType.Damage)
                .Select(x => x.DamageForm.DamageType)
                .Distinct()
                .ToList();

            var resistanceDamageTypes = AllowedDamageTypes.Intersect(attackDamageTypes).ToList();

            if (!resistanceDamageTypes.Any())
            {
                yield break;
            }

            var rulesetDefender = defender.RulesetCharacter;
            var slotLevel = rulesetDefender.GetLowestSlotLevelAndRepertoireToCastSpell(
                _spellDefinition, out var spellRepertoire);

            var reactionParams = new CharacterActionParams(defender, ActionDefinitions.Id.SpendSpellSlot)
            {
                IntParameter = slotLevel, StringParameter = _spellDefinition.Name, SpellRepertoire = spellRepertoire
            };
            var actionService = ServiceRepository.GetService<IGameLocationActionService>();
            var count = actionService.PendingReactionRequestGroups.Count;

            actionService.ReactToSpendSpellSlot(reactionParams);

            yield return battleManager.WaitForReactions(defender, actionService, count);

            if (!reactionParams.ReactionValidated)
            {
                yield break;
            }

            var slotUsed = reactionParams.IntParameter;

            spellRepertoire.SpendSpellSlot(slotUsed);
            defender.SpendActionType(ActionDefinitions.ActionType.Reaction);
            EffectHelpers.StartVisualEffect(defender, defender, ShadowArmor, EffectHelpers.EffectType.Caster);
            EffectHelpers.StartVisualEffect(defender, defender, ShadowArmor, EffectHelpers.EffectType.Effect);

            foreach (var condition in resistanceDamageTypes
                         .Select(damageType =>
                             GetDefinition<ConditionDefinition>(
                                 $"Condition{_spellDefinition.Name}{damageType.Substring(6)}Resistance")))
            {
                rulesetDefender.InflictCondition(
                    condition.Name,
                    DurationType.Round,
                    0,
                    TurnOccurenceType.StartOfTurn,
                    AttributeDefinitions.TagCombat,
                    rulesetDefender.guid,
                    rulesetDefender.CurrentFaction.Name,
                    1,
                    condition.Name,
                    slotUsed,
                    0,
                    0);
            }
        }
    }

    #endregion

    #region Gone With The Wind

    internal static SpellDefinition BuildGoneWithTheWind()
    {
        const string NAME = "StrikeWithTheWind";

        var movementAffinityStrikeWithTheWind = FeatureDefinitionMovementAffinityBuilder
            .Create($"MovementAffinity{NAME}")
            .SetGuiPresentation($"Condition{NAME}Movement", Category.Condition, Gui.NoLocalization)
            .SetBaseSpeedAdditiveModifier(5)
            .AddToDB();

        var conditionStrikeWithTheWindAttackMovement = ConditionDefinitionBuilder
            .Create($"Condition{NAME}Movement")
            .SetGuiPresentation(Category.Condition, Gui.NoLocalization, ConditionDefinitions.ConditionDisengaging)
            .SetPossessive()
            .SetSpecialDuration()
            .SetFeatures(movementAffinityStrikeWithTheWind)
            .SetConditionParticleReference(ConditionSpellbladeArcaneEscape.conditionParticleReference)
            .AddToDB();

        var additionalDamageStrikeWithTheWind = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{NAME}")
            .SetGuiPresentationNoContent(true)
            .SetNotificationTag(NAME)
            .SetDamageDice(DieType.D8, 1)
            .SetSpecificDamageType(DamageTypeForce)
            .AddToDB();

        var combatAffinityStrikeWithTheWind = FeatureDefinitionCombatAffinityBuilder
            .Create($"CombatAffinity{NAME}")
            .SetGuiPresentation($"Condition{NAME}", Category.Condition, Gui.NoLocalization)
            .SetMyAttackAdvantage(AdvantageType.Advantage)
            .AddToDB();

        var conditionStrikeWithTheWindAttack = ConditionDefinitionBuilder
            .Create($"Condition{NAME}Attack")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionDisengaging)
            .SetPossessive()
            .SetFeatures(additionalDamageStrikeWithTheWind, combatAffinityStrikeWithTheWind)
            .SetSpecialInterruptions(ConditionInterruption.Attacks, ConditionInterruption.AnyBattleTurnEnd)
            .AddCustomSubFeatures(
                new OnConditionAddedOrRemovedStrikeWithTheWindAttack(conditionStrikeWithTheWindAttackMovement))
            .SetConditionParticleReference(ConditionStrikeOfChaosAttackAdvantage.conditionParticleReference)
            .AddToDB();

        var powerStrikeWithTheWind = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}")
            .SetGuiPresentation(Category.Feature, PowerShadowcasterShadowDodge)
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.None)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionStrikeWithTheWindAttack))
                    .Build())
            .AddToDB();

        var conditionStrikeWithTheWind = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionDisengaging, $"Condition{NAME}")
            .SetOrUpdateGuiPresentation(Category.Condition)
            .SetPossessive()
            .AddFeatures(powerStrikeWithTheWind)
            .AddCustomSubFeatures(new AddUsablePowersFromCondition())
            .SetConditionParticleReference(ConditionStrikeOfChaosAttackAdvantage.conditionParticleReference)
            .AddToDB();

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.StrikeWithTheWind, 128))
            .SetCastingTime(ActivationTime.BonusAction)
            .SetSpellLevel(1)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetSomaticComponent(false)
            .SetVerboseComponent(true)
            .SetRequiresConcentration(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionStrikeWithTheWind))
                    .Build())
            .AddToDB();

        return spell;
    }

    private sealed class OnConditionAddedOrRemovedStrikeWithTheWindAttack : IOnConditionAddedOrRemoved
    {
        private readonly ConditionDefinition _conditionStrikeWithTheWindMovement;

        public OnConditionAddedOrRemovedStrikeWithTheWindAttack(ConditionDefinition conditionStrikeWithTheWindMovement)
        {
            _conditionStrikeWithTheWindMovement = conditionStrikeWithTheWindMovement;
        }

        public void OnConditionAdded(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            // empty
        }

        public void OnConditionRemoved(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            target.InflictCondition(
                _conditionStrikeWithTheWindMovement.Name,
                _conditionStrikeWithTheWindMovement.DurationType,
                _conditionStrikeWithTheWindMovement.DurationParameter,
                _conditionStrikeWithTheWindMovement.TurnOccurence,
                AttributeDefinitions.TagEffect,
                target.guid,
                target.CurrentFaction.Name,
                0,
                null,
                0,
                0,
                0);
        }
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

            powerSkinOfRetribution.AddCustomSubFeatures(
                new ModifyEffectDescriptionSkinOfRetribution(conditionSkinOfRetribution));

            var spell = SpellDefinitionBuilder
                .Create(NAME + damageType)
                .SetGuiPresentation(title, Gui.Format(SUB_SPELL_DESCRIPTION, title),
                    Sprites.GetSprite(NAME, Resources.SkinOfRetribution, 128))
                .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolAbjuration)
                .SetSpellLevel(1)
                .SetCastingTime(ActivationTime.Action)
                .SetMaterialComponent(MaterialComponentType.Mundane)
                .SetVerboseComponent(true)
                .SetSomaticComponent(true)
                .SetVocalSpellSameType(VocalSpellSemeType.Defense)
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
            .SetSpellLevel(1)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Defense)
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
            .AddSpecialInterruptions(ConditionInterruption.Attacks, ConditionInterruption.CastSpell)
            .AddToDB();

        var conditionSanctuaryReduceDamage = ConditionDefinitionBuilder
            .Create($"Condition{NAME}ReduceDamage")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddFeatures(
                FeatureDefinitionReduceDamageBuilder
                    .Create($"ReduceDamage{NAME}")
                    .SetGuiPresentation(NAME, Category.Spell)
                    .SetAlwaysActiveReducedDamage((_, _) => 999)
                    .AddToDB())
            .AddSpecialInterruptions(
                ConditionInterruption.Attacked, ConditionInterruption.Attacks, ConditionInterruption.CastSpell)
            .AddToDB();

        conditionSanctuary.AddCustomSubFeatures(
            new AttackBeforeHitConfirmedOnMeSanctuary(conditionSanctuary, conditionSanctuaryReduceDamage));

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite("Sanctuary", Resources.Sanctuary, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolAbjuration)
            .SetSpellLevel(1)
            .SetCastingTime(ActivationTime.BonusAction)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetDurationData(DurationType.Minute, 1)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionSanctuary))
                    .SetParticleEffectParameters(ProtectionFromEvilGood)
                    .Build())
            .AddCustomSubFeatures(new MagicEffectFinishedByMeSanctuary(conditionSanctuary))
            .AddToDB();

        return spell;
    }

    // store the caster Save DC on condition amount
    private sealed class MagicEffectFinishedByMeSanctuary : IMagicEffectFinishedByMe
    {
        private readonly ConditionDefinition _conditionSanctuary;

        public MagicEffectFinishedByMeSanctuary(ConditionDefinition conditionSanctuary)
        {
            _conditionSanctuary = conditionSanctuary;
        }

        public IEnumerator OnMagicEffectFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            if (action is not CharacterActionCastSpell actionCastSpell)
            {
                yield break;
            }

            var rulesetCaster = action.ActingCharacter.RulesetCharacter;
            var rulesetTarget = action.ActionParams.TargetCharacters[0].RulesetCharacter;

            if (!rulesetTarget.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect,
                    _conditionSanctuary.Name,
                    out var activeCondition))
            {
                yield break;
            }

            rulesetTarget.EnumerateFeaturesToBrowse<ISpellCastingAffinityProvider>(
                rulesetCaster.FeaturesToBrowse, rulesetCaster.FeaturesOrigin);
            activeCondition.Amount = rulesetCaster.ComputeSaveDC(actionCastSpell.activeSpell.SpellRepertoire);
        }
    }

    // force the attacker to roll a WIS saving throw or lose the attack
    private sealed class AttackBeforeHitConfirmedOnMeSanctuary : IAttackBeforeHitConfirmedOnMe
    {
        private readonly ConditionDefinition _conditionReduceDamage;
        private readonly ConditionDefinition _conditionSanctuary;

        internal AttackBeforeHitConfirmedOnMeSanctuary(
            ConditionDefinition conditionSanctuary,
            ConditionDefinition conditionReduceDamage)
        {
            _conditionSanctuary = conditionSanctuary;
            _conditionReduceDamage = conditionReduceDamage;
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
            var rulesetDefender = defender.RulesetCharacter;

            if (rulesetDefender is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            if (!rulesetDefender.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect,
                    _conditionSanctuary.Name,
                    out var activeCondition))
            {
                yield break;
            }

            var casterSaveDC = activeCondition.Amount;
            var modifierTrend = attacker.RulesetCharacter.actionModifier.savingThrowModifierTrends;
            var advantageTrends = attacker.RulesetCharacter.actionModifier.savingThrowAdvantageTrends;
            var attackerWisModifier = AttributeDefinitions.ComputeAbilityScoreModifier(
                attacker.RulesetCharacter.TryGetAttributeValue(AttributeDefinitions.Wisdom));

            attacker.RulesetCharacter.RollSavingThrow(
                0, AttributeDefinitions.Wisdom, null, modifierTrend, advantageTrends, attackerWisModifier, casterSaveDC,
                false, out var savingOutcome, out _);

            if (savingOutcome is RollOutcome.Success or RollOutcome.CriticalSuccess)
            {
                yield break;
            }

            rulesetDefender.InflictCondition(
                _conditionReduceDamage.Name,
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
                MovementAffinityNoClimb,
                MovementAffinityNoSpecialMoves,
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
            .SetSpellLevel(1)
            .SetCastingTime(ActivationTime.Hours1)
            .SetRitualCasting(ActivationTime.Hours1)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetSomaticComponent(true)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
            .SetUniqueInstance()
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
            .AddCustomSubFeatures(SkipEffectRemovalOnLocationChange.Always)
            .AddToDB();

        GlobalUniqueEffects.AddToGroup(GlobalUniqueEffects.Group.Familiar, spell);

        return spell;
    }

    #endregion

    #region Thunderous Smite

    internal static SpellDefinition BuildThunderousSmite()
    {
        const string NAME = "ThunderousSmite";

        var powerThunderousSmite = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}ThunderousSmite")
            .SetGuiPresentation(NAME, Category.Spell, hidden: true)
            .SetUsesFixed(ActivationTime.OnAttackHitAuto)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Touch, 1, TargetType.IndividualsUnique)
                    .SetSavingThrowData(false, AttributeDefinitions.Strength, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder.DamageForm(DamageTypeThunder, 2, DieType.D6),
                        EffectFormBuilder.Create()
                            .SetMotionForm(MotionForm.MotionType.PushFromOrigin, 2)
                            .HasSavingThrow(EffectSavingThrowType.Negates, TurnOccurenceType.StartOfTurn, true)
                            .Build(),
                        EffectFormBuilder.Create()
                            .SetMotionForm(MotionForm.MotionType.FallProne)
                            .HasSavingThrow(EffectSavingThrowType.Negates, TurnOccurenceType.StartOfTurn, true)
                            .Build())
                    .SetParticleEffectParameters(Shatter)
                    .Build())
            .AddToDB();

        var conditionThunderousSmite = ConditionDefinitionBuilder
            .Create($"Condition{NAME}")
            .SetGuiPresentation($"{NAME}Title".Formatted(Category.Spell), Gui.NoLocalization,
                ConditionBrandingSmite)
            .SetPossessive()
            .SetSpecialInterruptions(ConditionInterruption.AttacksAndDamages)
            .SetFeatures(powerThunderousSmite)
            .AddCustomSubFeatures(
                new AddUsablePowersFromCondition(),
                new PhysicalAttackFinishedByMeThunderousSmite(powerThunderousSmite))
            .AddToDB();

        var spell = SpellDefinitionBuilder
            .Create(BrandingSmite, NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.ThunderousSmite, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetSpellLevel(1)
            .SetCastingTime(ActivationTime.BonusAction)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetVerboseComponent(true)
            .SetSomaticComponent(false)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
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

    private sealed class PhysicalAttackFinishedByMeThunderousSmite : IPhysicalAttackFinishedByMe
    {
        private readonly FeatureDefinitionPower _powerThunderousSmite;

        public PhysicalAttackFinishedByMeThunderousSmite(FeatureDefinitionPower powerThunderousSmite)
        {
            _powerThunderousSmite = powerThunderousSmite;
        }

        public IEnumerator OnAttackFinishedByMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackerAttackMode,
            RollOutcome attackRollOutcome,
            int damageAmount)
        {
            if (attackRollOutcome is not (RollOutcome.Success or RollOutcome.CriticalSuccess))
            {
                yield break;
            }

            var rulesetAttacker = attacker.RulesetCharacter;
            var rulesetDefender = defender.RulesetCharacter;

            if (rulesetAttacker is not { IsDeadOrDyingOrUnconscious: false }
                || rulesetDefender is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            var actionParams = action.ActionParams.Clone();
            var usablePower = UsablePowersProvider.Get(_powerThunderousSmite, rulesetAttacker);

            actionParams.ActionDefinition = DatabaseHelper.ActionDefinitions.SpendPower;
            actionParams.RulesetEffect = ServiceRepository.GetService<IRulesetImplementationService>()
                //CHECK: no need for AddAsActivePowerToSource
                .InstantiateEffectPower(rulesetAttacker, usablePower, false);

            var actionService = ServiceRepository.GetService<IGameLocationActionService>();

            // must enqueue actions whenever within an attack workflow otherwise game won't consume attack
            actionService.ExecuteAction(actionParams, null, true);
        }
    }

    #endregion

    #region GiftOfAlacrity

    internal static SpellDefinition BuildGiftOfAlacrity()
    {
        const string NAME = "GiftOfAlacrity";

        var conditionAlacrity = ConditionDefinitionBuilder
            .Create(ConditionBlessed, "ConditionGiftOfAlacrity")
            .SetOrUpdateGuiPresentation(Category.Condition)
            .SetPossessive()
            .SetFeatures()
            .AddToDB();

        conditionAlacrity.AddCustomSubFeatures(new InitiativeEndListenerGiftOfAlacrity(conditionAlacrity));

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, CalmEmotions)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolDivination)
            .SetSpellLevel(1)
            .SetCastingTime(ActivationTime.Minute1)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
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
            .AddToDB();

        return spell;
    }

    private sealed class InitiativeEndListenerGiftOfAlacrity : IInitiativeEndListener
    {
        private readonly BaseDefinition _featureDefinition;

        public InitiativeEndListenerGiftOfAlacrity(BaseDefinition featureDefinition)
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

    #region Spike Barrage

    internal static SpellDefinition BuildSpikeBarrage()
    {
        const string NAME = "SpikeBarrage";

        var powerSpikeBarrage = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}")
            .SetGuiPresentation(NAME, Category.Spell, hidden: true)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Touch, 0, TargetType.IndividualsUnique)
                    .SetSavingThrowData(false, AttributeDefinitions.Dexterity, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(EffectFormBuilder
                        .Create()
                        .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                        .SetDamageForm(DamageTypePiercing, 1, DieType.D10)
                        .Build())
                    .SetParticleEffectParameters(ShadowDagger)
                    .Build())
            .AddToDB();

        var conditionSpikeBarrage = ConditionDefinitionBuilder
            .Create($"Condition{NAME}")
            .SetGuiPresentation(Category.Condition, ConditionTrueStrike)
            .SetPossessive()
            .SetConditionType(ConditionType.Beneficial)
            .SetSpecialInterruptions(ConditionInterruption.AttacksAndDamages)
            .AddToDB();

        conditionSpikeBarrage.AddCustomSubFeatures(
            new AddUsablePowersFromCondition(),
            new PhysicalAttackFinishedByMeSpikeBarrage(powerSpikeBarrage, conditionSpikeBarrage));

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.SpikeBarrage, 128))
            .SetCastingTime(ActivationTime.BonusAction)
            .SetSpellLevel(1)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolConjuration)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetSomaticComponent(false)
            .SetVerboseComponent(true)
            .SetRequiresConcentration(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, additionalDicePerIncrement: 1)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionSpikeBarrage))
                    .Build())
            .AddToDB();

        spell.EffectDescription.EffectParticleParameters.casterParticleReference =
            Entangle.EffectDescription.EffectParticleParameters.casterParticleReference;
        spell.EffectDescription.EffectParticleParameters.zoneParticleReference =
            Entangle.EffectDescription.EffectParticleParameters.zoneParticleReference;

        return spell;
    }

    private sealed class PhysicalAttackFinishedByMeSpikeBarrage : IPhysicalAttackFinishedByMe, IModifyEffectDescription
    {
        private readonly ConditionDefinition _conditionSpikeBarrage;
        private readonly FeatureDefinitionPower _powerSpikeBarrage;

        public PhysicalAttackFinishedByMeSpikeBarrage(
            FeatureDefinitionPower powerSpikeBarrage,
            ConditionDefinition conditionSpikeBarrage)
        {
            _powerSpikeBarrage = powerSpikeBarrage;
            _conditionSpikeBarrage = conditionSpikeBarrage;
        }

        public bool IsValid(BaseDefinition definition, RulesetCharacter character, EffectDescription effectDescription)
        {
            return definition == _powerSpikeBarrage;
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            if (!character.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, _conditionSpikeBarrage.Name, out var activeCondition))
            {
                return effectDescription;
            }

            var damageForm = effectDescription.FindFirstDamageForm();

            if (damageForm != null)
            {
                damageForm.diceNumber = activeCondition.EffectLevel;
            }

            return effectDescription;
        }

        public IEnumerator OnAttackFinishedByMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            RollOutcome outcome,
            int damageAmount)
        {
            if (outcome is not (RollOutcome.Success or RollOutcome.CriticalSuccess))
            {
                yield break;
            }

            if (!attackMode.Ranged && !attackMode.Thrown)
            {
                yield break;
            }

            var targets = new List<GameLocationCharacter>();

            targets.SetRange(Gui.Battle.AllContenders
                .Where(x =>
                    x.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false }
                    && battleManager.IsWithin1Cell(x, defender)));

            if (targets.Empty())
            {
                yield break;
            }

            var rulesetImplementationService = ServiceRepository.GetService<IRulesetImplementationService>();
            var rulesetCharacter = action.ActingCharacter.RulesetCharacter;

            var actionParams = action.ActionParams.Clone();
            var usablePower = UsablePowersProvider.Get(_powerSpikeBarrage, rulesetCharacter);

            actionParams.ActionDefinition = DatabaseHelper.ActionDefinitions.SpendPower;
            actionParams.RulesetEffect = rulesetImplementationService
                //CHECK: no need for AddAsActivePowerToSource
                .InstantiateEffectPower(rulesetCharacter, usablePower, false);
            actionParams.TargetCharacters.SetRange(targets);

            var actionService = ServiceRepository.GetService<IGameLocationActionService>();

            // must enqueue actions whenever within an attack workflow otherwise game won't consume attack
            actionService.ExecuteAction(actionParams, null, true);
        }
    }

    #endregion
}
