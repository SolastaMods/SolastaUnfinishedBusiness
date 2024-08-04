using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Validators;
using UnityEngine.AddressableAssets;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionActionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionMovementAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using Resources = SolastaUnfinishedBusiness.Properties.Resources;

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
            .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, additionalTargetsPerIncrement: 1)
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

        var proxyEarthTremor = EffectProxyDefinitionBuilder
            .Create(EffectProxyDefinitions.ProxyGrease, "ProxyEarthTremor")
            .SetOrUpdateGuiPresentation(NAME, Category.Spell)
            .AddToDB();

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.EarthTremor, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetSpellLevel(1)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, additionalDicePerIncrement: 1)
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
                            .SetSummonEffectProxyForm(proxyEarthTremor)
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
                        EffectFormBuilder.TopologyForm(TopologyForm.Type.DangerousZone, false),
                        EffectFormBuilder.TopologyForm(TopologyForm.Type.DifficultThrough, false))
                    .Build())
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
                    .SetCreatedBy()
                    .Build())
            .CopyParticleReferences(Entangle)
            .AddToDB();

        conditionEnsnared.specialInterruptions.Clear();

        var additionalDamageEnsnaringStrike = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{NAME}")
            .SetGuiPresentation(NAME, Category.Spell)
            .SetNotificationTag(NAME)
            .SetAttackModeOnly()
            .SetDamageDice(DieType.D6, 0)
            .SetSavingThrowData(
                EffectDifficultyClassComputation.SpellCastingFeature,
                EffectSavingThrowType.Negates,
                AttributeDefinitions.Strength)
            .AddConditionOperation(new ConditionOperationDescription
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
                    .SetCasterEffectParameters(Entangle)
                    .Build())
            .SetRequiresConcentration(true)
            .AddToDB();

        return spell;
    }

    #endregion

    #region Mule

    internal static SpellDefinition BuildMule()
    {
        const string NAME = "Mule";

        var effectDescription = EffectDescriptionBuilder
            .Create()
            .SetTargetingData(Side.Ally, RangeType.Touch, 0, TargetType.IndividualsUnique)
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
                        ConditionForm.ConditionOperation.Add, false, false,
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
                    .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel,
                        additionalTargetsPerIncrement: 1)
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
            .SetAttackModeOnly()
            .SetDamageDice(DieType.D6, 1)
            .SetSpecificDamageType(DamageTypeFire)
            .SetAdvancement(AdditionalDamageAdvancement.SlotLevel)
            .SetSavingThrowData(
                EffectDifficultyClassComputation.SpellCastingFeature,
                EffectSavingThrowType.None)
            .AddConditionOperation(
                new ConditionOperationDescription
                {
                    operation = ConditionOperationDescription.ConditionOperation.Add,
                    conditionDefinition = ConditionOnFire,
                    hasSavingThrow = true,
                    canSaveToCancel = true,
                    saveAffinity = EffectSavingThrowType.Negates,
                    saveOccurence = TurnOccurenceType.StartOfTurn
                })
            .SetImpactParticleReference(FireBolt)
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
            .SetAttackModeOnly()
            .SetDamageDice(DieType.D6, 1)
            .SetSpecificDamageType(DamageTypePsychic)
            .SetAdvancement(AdditionalDamageAdvancement.SlotLevel)
            .SetSavingThrowData(
                EffectDifficultyClassComputation.SpellCastingFeature,
                EffectSavingThrowType.None,
                AttributeDefinitions.Wisdom)
            .AddConditionOperation(
                new ConditionOperationDescription
                {
                    operation = ConditionOperationDescription.ConditionOperation.Add,
                    conditionDefinition = ConditionDefinitionBuilder
                        .Create(ConditionDefinitions.ConditionFrightened, $"Condition{NAME}Enemy")
                        .SetSpecialDuration(DurationType.Minute, 1, TurnOccurenceType.StartOfTurn)
                        .SetParentCondition(ConditionDefinitions.ConditionFrightened)
                        .AddToDB(),
                    hasSavingThrow = true,
                    canSaveToCancel = true,
                    saveAffinity = EffectSavingThrowType.Negates,
                    saveOccurence = TurnOccurenceType.StartOfTurn
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
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                    .SetTargetingData(Side.All, RangeType.Distance, 12, TargetType.Sphere, 2)
                    .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, additionalDicePerIncrement: 1)
                    .SetSavingThrowData(
                        false,
                        AttributeDefinitions.Constitution,
                        true,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetParticleEffectParameters(Shatter)
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

        conditionVoidGrasp.GuiPresentation.description = Gui.EmptyContent;

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
            .SetRecurrentEffectForms(
                EffectFormBuilder
                    .Create()
                    .SetDamageForm(DamageTypeAcid, 2, DieType.D4)
                    .SetCreatedBy()
                    .Build())
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
                    .SetTargetingData(Side.Enemy, RangeType.Touch, 0, TargetType.IndividualsUnique)
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
            .AddCustomSubFeatures(AddUsablePowersFromCondition.Marker)
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

    #endregion

    #region Chaos Bolt

    private static readonly (string, IMagicEffect)[] ChaosBoltDamagesAndEffects =
    [
        (DamageTypeAcid, AcidSplash), (DamageTypeCold, ConeOfCold),
        (DamageTypeFire, FireBolt), (DamageTypeForce, EldritchBlast),
        (DamageTypeLightning, LightningBolt), (DamageTypePoison, PoisonSpray),
        (DamageTypePsychic, Fear), (DamageTypeThunder, Shatter)
    ];

    internal static SpellDefinition BuildChaosBolt()
    {
        const string NAME = "ChaosBolt";

        var sprite = Sprites.GetSprite(NAME, Resources.ChaosBolt, 128);

        var powerPool = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}Pool")
            .SetGuiPresentation(NAME, Category.Spell, hidden: true)
            .SetUsesFixed(ActivationTime.NoCost)
            .AddToDB();

        var powers = new List<FeatureDefinitionPower>();

        foreach (var (damageType, _) in ChaosBoltDamagesAndEffects)
        {
            var title = Gui.Localize($"Tooltip/&Tag{damageType}Title");
            var description = Gui.Format($"Feature/&Power{NAME}Description", title);
            var power = FeatureDefinitionPowerSharedPoolBuilder
                .Create($"Power{NAME}{damageType}")
                .SetGuiPresentation(title, description)
                .SetSharedPool(ActivationTime.NoCost, powerPool)
                .AddToDB();

            powers.Add(power);
        }

        PowerBundle.RegisterPowerBundle(powerPool, false, powers);

        var conditionMark = ConditionDefinitionBuilder
            .Create($"Condition{NAME}Mark")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddToDB();

        var powerLeap = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}Leap")
            .SetGuiPresentation(NAME, Category.Spell, sprite)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetUseSpellAttack()
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(GuidingBolt)
                    .SetDurationData(DurationType.Instantaneous)
                    .SetTargetingData(Side.Enemy, RangeType.RangeHit, 24, TargetType.IndividualsUnique)
                    .SetTargetFiltering(TargetFilteringMethod.CharacterOnly)
                    .SetEffectAdvancement(EffectIncrementMethod.None)
                    .SetEffectForms(
                        EffectFormBuilder.DamageForm(DamageTypeBludgeoning, 2, DieType.D8),
                        EffectFormBuilder.DamageForm(DamageTypeBludgeoning, 1, DieType.D6))
                    .SetCasterEffectParameters(PrismaticSpray)
                    .SetImpactEffectParameters(new AssetReference())
                    .Build())
            .AddCustomSubFeatures(new ValidatorsValidatePowerUse(c =>
            {
                var glc = GameLocationCharacter.GetFromActor(c);

                return glc != null &&
                       glc.UsedSpecialFeatures.TryGetValue("ChaosBoltLeap", out var chaosBoltLeap) &&
                       chaosBoltLeap != 0;
            }))
            .AddToDB();

        var conditionLeap = ConditionDefinitionBuilder
            .Create($"Condition{NAME}Leap")
            .SetGuiPresentation(NAME, Category.Spell, Gui.NoLocalization)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetAmountOrigin(ConditionDefinition.OriginOfAmount.Fixed)
            .SetFeatures(powerLeap)
            .AddCustomSubFeatures(AddUsablePowersFromCondition.Marker)
            .AddToDB();

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, sprite)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetSpellLevel(1)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(GuidingBolt)
                    .SetDurationData(DurationType.Instantaneous)
                    .SetTargetingData(Side.Enemy, RangeType.RangeHit, 24, TargetType.IndividualsUnique)
                    .SetTargetFiltering(TargetFilteringMethod.CharacterOnly)
                    .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel)
                    .SetEffectForms(
                        EffectFormBuilder.DamageForm(DamageTypeBludgeoning, 2, DieType.D8),
                        EffectFormBuilder.DamageForm(DamageTypeBludgeoning, 1, DieType.D6))
                    .SetCasterEffectParameters(PrismaticSpray)
                    .SetImpactEffectParameters(new AssetReference())
                    .Build())
            .AddToDB();

        var damageDeterminationBehavior =
            new CustomBehaviorChaosBolt(spell, powerLeap, conditionLeap, conditionMark, powerPool, [.. powers]);
        var initAndFinishBehavior =
            new PowerOrSpellInitiatedAndFinishedByMeChaosBolt(conditionLeap, damageDeterminationBehavior);
        var filterTargetBehavior =
            new FilterTargetingCharacterChaosBolt(conditionMark);

        spell.AddCustomSubFeatures(
            initAndFinishBehavior,
            filterTargetBehavior);
        powerLeap.AddCustomSubFeatures(
            initAndFinishBehavior,
            filterTargetBehavior);
        conditionLeap.AddCustomSubFeatures(
            damageDeterminationBehavior);

        return spell;
    }

    private sealed class PowerOrSpellInitiatedAndFinishedByMeChaosBolt(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionLeap,
        CustomBehaviorChaosBolt damageDeterminationBehavior) : IPowerOrSpellInitiatedByMe, IPowerOrSpellFinishedByMe
    {
        public IEnumerator OnPowerOrSpellFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            var attacker = action.ActingCharacter;

            foreach (var (defender, effect) in damageDeterminationBehavior.MagicEffect)
            {
                EffectHelpers.StartVisualEffect(attacker, defender, effect);
            }

            if (!damageDeterminationBehavior.HasLeap())
            {
                yield break;
            }

            action.ActingCharacter.RulesetCharacter.LogCharacterActivatesAbility(
                "Spell/&ChaosBoltTitle", "Feedback/&ChaosBoltGainLeap");
        }

        public IEnumerator OnPowerOrSpellInitiatedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            var attacker = action.ActingCharacter;

            damageDeterminationBehavior.Reset(attacker);

            if (action is not CharacterActionCastSpell actionCastSpell)
            {
                yield break;
            }

            var rulesetAttacker = action.ActingCharacter.RulesetCharacter;

            rulesetAttacker.InflictCondition(
                conditionLeap.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetAttacker.guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                conditionLeap.Name,
                actionCastSpell.ActiveSpell.EffectLevel,
                0,
                0);
        }
    }

    private sealed class FilterTargetingCharacterChaosBolt(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionMark) : IFilterTargetingCharacter
    {
        public bool EnforceFullSelection => false;

        public bool IsValid(CursorLocationSelectTarget __instance, GameLocationCharacter target)
        {
            if (target.RulesetCharacter == null)
            {
                return false;
            }

            var isValid = !target.RulesetCharacter.HasConditionOfCategoryAndType(
                AttributeDefinitions.TagEffect, conditionMark.Name);

            if (!isValid)
            {
                __instance.actionModifier.FailureFlags.Add("Tooltip/&MustNotHaveChaosBoltMark");
            }

            return isValid;
        }
    }

    private sealed class CustomBehaviorChaosBolt(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        SpellDefinition spell,
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinitionPower powerLeap,
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionLeap,
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionMark,
        FeatureDefinitionPower powerPool,
        params FeatureDefinitionPower[] powers)
        : IMagicEffectBeforeHitConfirmedOnEnemy, IModifyEffectDescription, IModifyDiceRoll
    {
        private readonly List<int> _rolls = [];
        private int _rollIndex;
        private int _usedDamageDice;
        internal List<(GameLocationCharacter, IMagicEffect)> MagicEffect { get; } = [];

        public IEnumerator OnMagicEffectBeforeHitConfirmedOnEnemy(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier actionModifier,
            RulesetEffect rulesetEffect,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            switch (rulesetEffect)
            {
                case RulesetEffectSpell rulesetEffectSpell when
                    rulesetEffectSpell.SpellDefinition != spell:
                case RulesetEffectPower rulesetEffectPower when
                    rulesetEffectPower.PowerDefinition != powerLeap:
                    yield break;
            }

            if (rulesetEffect is not (RulesetEffectSpell or RulesetEffectPower))
            {
                yield break;
            }

            var rulesetAttacker = attacker.RulesetCharacter;
            var rulesetDefender = defender.RulesetActor;

            rulesetDefender.InflictCondition(
                conditionMark.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetAttacker.guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                conditionMark.Name,
                0,
                0,
                0);

            var hasEmpowered = rulesetEffect.MetamagicOption == MetamagicOptionDefinitions.MetamagicEmpoweredSpell;
            var firstRoll = RollDie(DieType.D8, AdvantageType.None, out _, out _);
            var secondRoll = RollDie(DieType.D8, AdvantageType.None, out _, out _);

            if (hasEmpowered)
            {
                while (firstRoll <= 2)
                {
                    firstRoll = RollDie(DieType.D8, AdvantageType.None, out _, out _);
                }

                while (secondRoll <= 2)
                {
                    secondRoll = RollDie(DieType.D8, AdvantageType.None, out _, out _);
                }
            }

            _rolls.AddRange(firstRoll, secondRoll);

            if (firstRoll == secondRoll)
            {
                attacker.UsedSpecialFeatures["ChaosBoltLeap"] += 1;

                rulesetAttacker.LogCharacterActivatesAbility(
                    "Spell/&ChaosBoltTitle",
                    "Feedback/&ChaosBoltRolledEqualDice",
                    extra:
                    [
                        (ConsoleStyleDuplet.ParameterType.Positive, firstRoll.ToString())
                    ]);

                var (damageType, effect) = ChaosBoltDamagesAndEffects.ElementAt(firstRoll - 1);

                MagicEffect.Add((defender, effect));

                foreach (var effectForm in actualEffectForms
                             .Where(x =>
                                 x.FormType == EffectForm.EffectFormType.Damage &&
                                 x.DamageForm.DamageType == DamageTypeBludgeoning))
                {
                    effectForm.DamageForm.DamageType = damageType;
                }
            }
            else
            {
                rulesetAttacker.LogCharacterActivatesAbility(
                    "Spell/&ChaosBoltTitle",
                    "Feedback/&ChaosBoltRolledDifferentDice",
                    extra:
                    [
                        (ConsoleStyleDuplet.ParameterType.Positive, firstRoll.ToString()),
                        (ConsoleStyleDuplet.ParameterType.Positive, secondRoll.ToString())
                    ]);

                var actionManager =
                    ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;

                if (!actionManager)
                {
                    yield break;
                }

                var usablePowerPool = PowerProvider.Get(powerPool, rulesetAttacker);
                var usablePowerFirst = PowerProvider.Get(powers[firstRoll - 1], rulesetAttacker);
                var usablePowerSecond = PowerProvider.Get(powers[secondRoll - 1], rulesetAttacker);

                rulesetAttacker.UsablePowers.Add(usablePowerPool);
                rulesetAttacker.UsablePowers.Add(usablePowerFirst);
                rulesetAttacker.UsablePowers.Add(usablePowerSecond);

                var implementationManager =
                    ServiceRepository.GetService<IRulesetImplementationService>() as RulesetImplementationManager;

                var usablePower = PowerProvider.Get(powerPool, rulesetAttacker);
                var actionParams = new CharacterActionParams(attacker, ActionDefinitions.Id.SpendPower)
                {
                    StringParameter = "ChaosBolt",
                    RulesetEffect = implementationManager
                        .MyInstantiateEffectPower(rulesetAttacker, usablePower, false),
                    UsablePower = usablePower,
                    TargetCharacters = { defender }
                };

                var count = actionManager.PendingReactionRequestGroups.Count;
                var reactionRequest = new ReactionRequestSpendBundlePower(actionParams);

                actionManager.AddInterruptRequest(reactionRequest);

                yield return battleManager.WaitForReactions(attacker, actionManager, count);

                rulesetAttacker.UsablePowers.Remove(usablePowerPool);
                rulesetAttacker.UsablePowers.Remove(usablePowerFirst);
                rulesetAttacker.UsablePowers.Remove(usablePowerSecond);

                string damageType;
                IMagicEffect effect;

                if (!reactionRequest.Validated)
                {
                    var choiceRoll = RollDie(DieType.D2, AdvantageType.None, out _, out _);

                    var option = choiceRoll == 1 ? firstRoll : secondRoll;

                    (damageType, effect) = ChaosBoltDamagesAndEffects.ElementAt(option - 1);

                    var damageTitle = Gui.Localize($"Tooltip/&Tag{damageType}Title");

                    rulesetAttacker.LogCharacterActivatesAbility(
                        "Spell/&ChaosBoltTitle", "Feedback/&ChaosBoltRandomChoice",
                        extra:
                        [
                            (ConsoleStyleDuplet.ParameterType.Base, damageTitle)
                        ]);
                }
                else
                {
                    (damageType, effect) = ChaosBoltDamagesAndEffects.ElementAt(reactionRequest.SelectedSubOption);
                }

                MagicEffect.Add((defender, effect));

                foreach (var effectForm in actualEffectForms
                             .Where(x =>
                                 x.FormType == EffectForm.EffectFormType.Damage &&
                                 x.DamageForm.DamageType == DamageTypeBludgeoning))
                {
                    effectForm.DamageForm.DamageType = damageType;
                }
            }
        }

        public void BeforeRoll(
            RollContext rollContext,
            RulesetCharacter rulesetCharacter,
            ref DieType dieType,
            ref AdvantageType advantageType)
        {
            // empty
        }

        public void AfterRoll(
            DieType dieType,
            AdvantageType advantageType,
            RollContext rollContext,
            RulesetCharacter rulesetCharacter,
            ref int firstRoll,
            ref int secondRoll,
            ref int result)
        {
            if (rollContext == RollContext.AttackRoll ||
                (dieType == DieType.D6 &&
                 rollContext == RollContext.MagicDamageValueRoll))
            {
                _usedDamageDice = 0;

                return;
            }

            if (dieType != DieType.D8 ||
                rollContext != RollContext.MagicDamageValueRoll ||
                _rollIndex >= _rolls.Count ||
                _usedDamageDice >= 2)
            {
                return;
            }

            _usedDamageDice++;

            var roll = _rolls[_rollIndex++];

            firstRoll = roll;
            secondRoll = -1;
            result = roll;
        }

        public bool IsValid(BaseDefinition definition, RulesetCharacter character, EffectDescription effectDescription)
        {
            return definition == spell || definition == powerLeap;
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            if (character.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, conditionLeap.Name, out var activeCondition))
            {
                effectDescription.EffectForms[1].DamageForm.DiceNumber = activeCondition.Amount;
            }

            return effectDescription;
        }

        public void Reset(GameLocationCharacter attacker)
        {
            attacker.UsedSpecialFeatures.TryAdd("ChaosBoltLeap", 0);
            attacker.UsedSpecialFeatures["ChaosBoltLeap"] = 0;
            _rollIndex = 0;
            _usedDamageDice = 0;
            _rolls.Clear();
            MagicEffect.Clear();
        }

        public bool HasLeap()
        {
            for (var i = 0; i < _rolls.Count; i += 2)
            {
                if (i + 1 < _rolls.Count && _rolls[i] == _rolls[i + 1])
                {
                    return true;
                }
            }

            return false;
        }
    }

    #endregion

    #region Ice Blade

    internal static SpellDefinition BuildIceBlade()
    {
        const string NAME = "IceBlade";

        var power = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}")
            .SetGuiPresentation(NAME, Category.Spell, hidden: true)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetSavingThrowData(false, AttributeDefinitions.Dexterity, false,
                        EffectDifficultyClassComputation.FixedValue)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetDamageForm(DamageTypeCold, 1, DieType.D6)
                            .Build())
                    .SetImpactEffectParameters(ConeOfCold)
                    .Build())
            .AddToDB();

        power.AddCustomSubFeatures(new ModifyEffectDescriptionIceBlade(power));

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.IceBlade, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolConjuration)
            .SetSpellLevel(1)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetVerboseComponent(false)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.RangeHit, 12, TargetType.IndividualsUnique)
                    .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, additionalDicePerIncrement: 1)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypePiercing, 1, DieType.D10)
                            .Build())
                    .SetParticleEffectParameters(RayOfFrost)
                    .SetImpactEffectParameters(ShadowDagger)
                    .SetEffectEffectParameters(ShadowDagger)
                    .Build())
            .AddCustomSubFeatures(new PowerOrSpellFinishedByMeIceBlade(power))
            .AddToDB();

        return spell;
    }

    private sealed class ModifyEffectDescriptionIceBlade(FeatureDefinitionPower powerDamage)
        : IModifyEffectDescription
    {
        public bool IsValid(BaseDefinition definition, RulesetCharacter character, EffectDescription effectDescription)
        {
            return definition == powerDamage;
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            effectDescription.EffectForms[0].DamageForm.DiceNumber =
                2 + (PowerOrSpellFinishedByMeIceBlade.EffectLevel - 1);

            return effectDescription;
        }
    }

    private sealed class PowerOrSpellFinishedByMeIceBlade(FeatureDefinitionPower powerIceBlade)
        : IPowerOrSpellFinishedByMe
    {
        internal static int EffectLevel;

        public IEnumerator OnPowerOrSpellFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            if (Gui.Battle == null)
            {
                yield break;
            }

            if (action is not CharacterActionCastSpell actionCastSpell || action.Countered)
            {
                yield break;
            }

            var caster = actionCastSpell.ActingCharacter;
            var rulesetCaster = caster.RulesetCharacter;

            EffectLevel = actionCastSpell.ActionParams.activeEffect.EffectLevel;

            var implementationManager =
                ServiceRepository.GetService<IRulesetImplementationService>() as RulesetImplementationManager;

            // need to loop over target characters to support twinned metamagic scenarios
            foreach (var target in actionCastSpell.ActionParams.TargetCharacters)
            {
                var contenders = Gui.Battle.AllContenders
                    .Where(x =>
                        x.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false } &&
                        x.IsWithinRange(target, 1))
                    .ToList();

                var usablePower = PowerProvider.Get(powerIceBlade, rulesetCaster);

                usablePower.saveDC = 8 + actionCastSpell.ActiveSpell.MagicAttackBonus;

                var actionModifiers = new List<ActionModifier>();

                for (var i = 0; i < contenders.Count; i++)
                {
                    actionModifiers.Add(new ActionModifier());
                }

                var actionParams = new CharacterActionParams(caster, ActionDefinitions.Id.PowerNoCost)
                {
                    ActionModifiers = actionModifiers,
                    RulesetEffect = implementationManager
                        .MyInstantiateEffectPower(rulesetCaster, usablePower, false),
                    UsablePower = usablePower,
                    targetCharacters = contenders
                };

                ServiceRepository.GetService<IGameLocationActionService>()?
                    .ExecuteAction(actionParams, null, true);
            }
        }
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
                .SetAdvancement((AdditionalDamageAdvancement)ExtraAdditionalDamageAdvancement.ConditionAmount)
                .SetSpecificDamageType(damageType)
                .SetImpactParticleReference(
                    magicEffect.EffectDescription.EffectParticleParameters.impactParticleReference)
                .AddCustomSubFeatures(ValidatorsRestrictedContext.IsMeleeOrUnarmedAttack)
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
                    .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, additionalDicePerIncrement: 1)
                    .SetParticleEffectParameters(ConjureElemental)
                    .Build())
            .AddToDB();

        spell.AddCustomSubFeatures(new CustomBehaviorElementalInfusion(spell));

        return spell;
    }

    // ReSharper disable once SuggestBaseTypeForParameterInConstructor
    private sealed class CustomBehaviorConditionElementalInfusion(ConditionDefinition conditionElementalInfusion) :
        IPhysicalAttackFinishedByMe, IMagicEffectFinishedByMe
    {
        public IEnumerator OnMagicEffectFinishedByMe(
            CharacterActionMagicEffect action,
            GameLocationCharacter attacker,
            List<GameLocationCharacter> targets)
        {
            if (action.AttackRollOutcome is RollOutcome.Success or RollOutcome.CriticalSuccess &&
                action.ActionParams.RulesetEffect.EffectDescription.RangeType is RangeType.Touch or RangeType.MeleeHit)
            {
                attacker.RulesetCharacter.RemoveAllConditionsOfCategoryAndType(
                    AttributeDefinitions.TagEffect, conditionElementalInfusion.Name);
            }

            yield break;
        }

        public IEnumerator OnPhysicalAttackFinishedByMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            RollOutcome rollOutcome,
            int damageAmount)
        {
            if (rollOutcome is RollOutcome.Success or RollOutcome.CriticalSuccess)
            {
                attacker.RulesetCharacter.RemoveAllConditionsOfCategoryAndType(AttributeDefinitions.TagEffect,
                    conditionElementalInfusion.Name);
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
            var glc = GameLocationCharacter.GetFromActor(target);

            if (glc == null || !glc.IsMyTurn())
            {
                return;
            }

            var name = rulesetCondition.ConditionDefinition.Name.Replace("Resistance", "Damage");

            target.InflictCondition(
                name,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagEffect,
                target.guid,
                target.CurrentFaction.Name,
                1,
                name,
                rulesetCondition.Amount,
                0,
                0);
        }
    }

    private sealed class CustomBehaviorElementalInfusion(SpellDefinition spellDefinition) :
        ITryAlterOutcomeAttack, IMagicEffectBeforeHitConfirmedOnMe
    {
        private static readonly IEnumerable<string> AllowedDamageTypes = DamagesAndEffects
            .Where(x => x.Item1 != DamageTypePoison)
            .Select(x => x.Item1);

        public IEnumerator OnMagicEffectBeforeHitConfirmedOnMe(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier actionModifier,
            RulesetEffect rulesetEffect,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            if (rulesetEffect is RulesetEffectSpell &&
                rulesetEffect.EffectDescription.RangeType is not (RangeType.MeleeHit or RangeType.RangeHit))
            {
                yield return HandleReaction(battleManager, attacker, defender, actualEffectForms);
            }
        }

        public int HandlerPriority => 10;

        public IEnumerator OnTryAlterOutcomeAttack(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter helper,
            ActionModifier actionModifier,
            RulesetAttackMode attackMode,
            RulesetEffect rulesetEffect)
        {
            if (helper != defender ||
                !helper.CanReact() ||
                !helper.RulesetCharacter.AreSpellComponentsValid(spellDefinition))
            {
                yield break;
            }

            var actualEffectForms =
                attackMode?.EffectDescription.EffectForms ?? rulesetEffect?.EffectDescription.EffectForms ?? [];

            yield return HandleReaction(battleManager, attacker, helper, actualEffectForms);
        }

        private IEnumerator HandleReaction(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter helper,
            IEnumerable<EffectForm> actualEffectForms)
        {
            var attackDamageTypes = actualEffectForms
                .Where(x => x.FormType == EffectForm.EffectFormType.Damage)
                .Select(x => x.DamageForm.DamageType)
                .Distinct()
                .ToList();

            var resistanceDamageTypes = AllowedDamageTypes.Intersect(attackDamageTypes).ToList();

            if (resistanceDamageTypes.Count == 0)
            {
                yield break;
            }

            var rulesetHelper = helper.RulesetCharacter;
            var slotLevel = rulesetHelper.GetLowestSlotLevelAndRepertoireToCastSpell(
                spellDefinition, out var spellRepertoire);

            if (slotLevel < 1 ||
                spellRepertoire == null)
            {
                yield break;
            }

            var actionService = ServiceRepository.GetService<IGameLocationActionService>();
            var effectSpell = ServiceRepository.GetService<IRulesetImplementationService>()
                .InstantiateEffectSpell(rulesetHelper, spellRepertoire, SpellsContext.ElementalInfusion, slotLevel,
                    false);
            var reactionParams = new CharacterActionParams(helper, ActionDefinitions.Id.CastReaction)
            {
                IntParameter = slotLevel,
                StringParameter = spellDefinition.Name,
                SpellRepertoire = spellRepertoire,
                RulesetEffect = effectSpell
            };
            var count = actionService.PendingReactionRequestGroups.Count;

            actionService.ReactToSpendSpellSlot(reactionParams);

            yield return battleManager.WaitForReactions(attacker, actionService, count);

            if (!reactionParams.ReactionValidated)
            {
                yield break;
            }

            var slotUsed = reactionParams.IntParameter;

            EffectHelpers.StartVisualEffect(helper, helper, ShadowArmor, EffectHelpers.EffectType.Caster);
            EffectHelpers.StartVisualEffect(helper, helper, ShadowArmor, EffectHelpers.EffectType.Effect);

            foreach (var condition in resistanceDamageTypes
                         .Select(damageType =>
                             GetDefinition<ConditionDefinition>(
                                 $"Condition{spellDefinition.Name}{damageType.Substring(6)}Resistance")))
            {
                rulesetHelper.InflictCondition(
                    condition.Name,
                    DurationType.Round,
                    0,
                    TurnOccurenceType.StartOfTurn,
                    AttributeDefinitions.TagEffect,
                    rulesetHelper.guid,
                    rulesetHelper.CurrentFaction.Name,
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
            .SetFeatures(movementAffinityStrikeWithTheWind)
            .SetConditionParticleReference(ConditionSpellbladeArcaneEscape)
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
            .SetSpecialInterruptions(ConditionInterruption.Attacks)
            .AddCustomSubFeatures(
                new OnConditionAddedOrRemovedStrikeWithTheWindAttack(conditionStrikeWithTheWindAttackMovement))
            .SetConditionParticleReference(ConditionStrikeOfChaosAttackAdvantage)
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
            .AddCustomSubFeatures(AddUsablePowersFromCondition.Marker)
            .SetConditionParticleReference(ConditionStrikeOfChaosAttackAdvantage)
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

    private sealed class OnConditionAddedOrRemovedStrikeWithTheWindAttack(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionStrikeWithTheWindMovement) : IOnConditionAddedOrRemoved
    {
        public void OnConditionAdded(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            // empty
        }

        public void OnConditionRemoved(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            target.InflictCondition(
                conditionStrikeWithTheWindMovement.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagEffect,
                target.guid,
                target.CurrentFaction.Name,
                1,
                conditionStrikeWithTheWindMovement.Name,
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

        var powerSkinOfRetribution = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}")
            .SetGuiPresentationNoContent(true)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetEffectForms(EffectFormBuilder.DamageForm(DamageTypeCold, bonusDamage: TEMP_HP_PER_LEVEL))
                    .SetParticleEffectParameters(ConeOfCold)
                    .Build())
            .AddToDB();

        var damageAffinitySkinOfRetribution = FeatureDefinitionDamageAffinityBuilder
            .Create($"DamageAffinity{NAME}")
            .SetGuiPresentationNoContent(true)
            .SetRetaliate(powerSkinOfRetribution, 1)
            .AddToDB();

        var conditionSkinOfRetribution = ConditionDefinitionBuilder
            .Create($"Condition{NAME}")
            .SetGuiPresentation(Category.Condition,
                Sprites.GetSprite("ConditionMirrorImage", Resources.ConditionMirrorImage, 32))
            .SetSilent(Silent.WhenAdded)
            .SetPossessive()
            .SetFeatures(damageAffinitySkinOfRetribution)
            .AddCustomSubFeatures(new OnConditionAddedOrRemovedSkinOfRetribution())
            .CopyParticleReferences(PowerDomainElementalHeraldOfTheElementsCold)
            .AddToDB();

        powerSkinOfRetribution.AddCustomSubFeatures(
            new ModifyEffectDescriptionSkinOfRetribution(conditionSkinOfRetribution));

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
            .SetUniqueInstance()
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Hour, 1)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetTempHpForm(TEMP_HP_PER_LEVEL)
                            .Build(),
                        EffectFormBuilder.ConditionForm(conditionSkinOfRetribution))
                    .SetEffectAdvancement(
                        EffectIncrementMethod.PerAdditionalSlotLevel,
                        additionalTempHpPerIncrement: TEMP_HP_PER_LEVEL)
                    .SetParticleEffectParameters(ConeOfCold)
                    .Build())
            .AddToDB();
    }

    private sealed class OnConditionAddedOrRemovedSkinOfRetribution
        : IOnConditionAddedOrRemoved, ICharacterTurnStartListener
    {
        // required to ensure the behavior will still work after loading a save
        public void OnCharacterTurnStarted(GameLocationCharacter locationCharacter)
        {
            var rulesetCharacter = locationCharacter.RulesetCharacter;

            rulesetCharacter.DamageReceived -= DamageReceivedHandler;
            rulesetCharacter.DamageReceived += DamageReceivedHandler;
        }

        public void OnConditionAdded(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            target.DamageReceived += DamageReceivedHandler;
        }

        public void OnConditionRemoved(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            target.DamageReceived -= DamageReceivedHandler;
        }

        private static void DamageReceivedHandler(
            RulesetActor target,
            int damage,
            string damageType,
            ulong sourceGuid,
            RollInfo rollInfo)
        {
            if (target is RulesetCharacter rulesetCharacter &&
                rulesetCharacter.TemporaryHitPoints <= damage)
            {
                rulesetCharacter.RemoveAllConditionsOfCategoryAndType(
                    AttributeDefinitions.TagEffect, "ConditionSkinOfRetribution");
            }
        }
    }

    private sealed class ModifyEffectDescriptionSkinOfRetribution(ConditionDefinition conditionSkinOfRetribution)
        : IModifyEffectDescription
    {
        public bool IsValid(
            BaseDefinition definition,
            RulesetCharacter character,
            EffectDescription effectDescription)
        {
            return effectDescription.HasDamageForm() && character.HasConditionOfType(conditionSkinOfRetribution);
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            var rulesetCondition =
                character.AllConditions.FirstOrDefault(x => x.ConditionDefinition == conditionSkinOfRetribution);
            var effectLevel = rulesetCondition!.EffectLevel;

            var damageForm = effectDescription.FindFirstDamageForm();

            damageForm.bonusDamage *= effectLevel;

            return effectDescription;
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
                (ConditionInterruption)ExtraConditionInterruption.AfterWasAttacked,
                ConditionInterruption.Attacks,
                ConditionInterruption.CastSpell)
            .AddToDB();

        conditionSanctuary.AddCustomSubFeatures(
            new CustomBehaviorSanctuary(conditionSanctuary, conditionSanctuaryReduceDamage));

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
            .AddCustomSubFeatures(new PowerOrSpellFinishedByMeSanctuary(conditionSanctuary))
            .AddToDB();

        return spell;
    }

    // store the caster Save DC on condition amount
    // ReSharper disable once SuggestBaseTypeForParameterInConstructor
    private sealed class PowerOrSpellFinishedByMeSanctuary(ConditionDefinition conditionSanctuary)
        : IPowerOrSpellFinishedByMe
    {
        public IEnumerator OnPowerOrSpellFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            if (action is not CharacterActionCastSpell actionCastSpell)
            {
                yield break;
            }

            var rulesetCaster = action.ActingCharacter.RulesetCharacter;

            // need to loop over target characters to support twinned metamagic scenarios
            foreach (var rulesetTarget in action.ActionParams.TargetCharacters
                         .Select(target => target.RulesetCharacter))
            {
                if (!rulesetTarget.TryGetConditionOfCategoryAndType(
                        AttributeDefinitions.TagEffect,
                        conditionSanctuary.Name,
                        out var activeCondition))
                {
                    continue;
                }

                rulesetTarget.EnumerateFeaturesToBrowse<ISpellCastingAffinityProvider>(
                    rulesetCaster.FeaturesToBrowse, rulesetCaster.FeaturesOrigin);
                activeCondition.Amount = rulesetCaster.ComputeSaveDC(actionCastSpell.activeSpell.SpellRepertoire);
            }
        }
    }

    // force the attacker to roll a WIS saving throw or lose the attack
    private sealed class CustomBehaviorSanctuary : ITryAlterOutcomeAttack
    {
        private readonly ConditionDefinition _conditionReduceDamage;
        private readonly ConditionDefinition _conditionSanctuary;

        internal CustomBehaviorSanctuary(
            ConditionDefinition conditionSanctuary,
            ConditionDefinition conditionReduceDamage)
        {
            _conditionSanctuary = conditionSanctuary;
            _conditionReduceDamage = conditionReduceDamage;
        }

        public int HandlerPriority => 20;

        public IEnumerator OnTryAlterOutcomeAttack(
            GameLocationBattleManager instance,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter helper,
            ActionModifier actionModifier,
            RulesetAttackMode attackMode,
            RulesetEffect rulesetEffect)
        {
            var rulesetDefender = defender.RulesetCharacter;

            if (helper.IsOppositeSide(defender.Side))
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

            if (savingOutcome == RollOutcome.Success)
            {
                yield break;
            }

            rulesetDefender.InflictCondition(
                _conditionReduceDamage.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.StartOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetDefender.guid,
                rulesetDefender.CurrentFaction.Name,
                1,
                _conditionReduceDamage.Name,
                0,
                0,
                0);
        }
    }

    #endregion

    #region Owl Familiar

    private const string OwlFamiliar = "OwlFamiliar";

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
                FeatureDefinitionConditionAffinitys.ConditionAffinityProneImmunity,
                CharacterContext.FeatureDefinitionPowerHelpAction,
                CharacterContext.PowerTeleportSummon,
                CharacterContext.PowerVanishSummon)
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
            .SetDungeonMakerPresence(MonsterDefinition.DungeonMaker.None)
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

        ForceGlobalUniqueEffects.AddToGroup(ForceGlobalUniqueEffects.Group.Familiar, spell);

        return spell;
    }

    #endregion

    #region Gift of Alacrity

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
                    .SetTargetingData(Side.Ally, RangeType.Touch, 0, TargetType.IndividualsUnique)
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

    private sealed class InitiativeEndListenerGiftOfAlacrity(BaseDefinition featureDefinition) : IInitiativeEndListener
    {
        public IEnumerator OnInitiativeEnded(GameLocationCharacter locationCharacter)
        {
            const string TEXT = "Feedback/&FeatureGiftOfAlacrityLine";

            var gameLocationScreenBattle = Gui.GuiService.GetScreen<GameLocationScreenBattle>();

            if (Gui.Battle == null || !gameLocationScreenBattle)
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
                featureDefinition,
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
            AddUsablePowersFromCondition.Marker,
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
                    .SetCasterEffectParameters(Entangle)
                    .Build())
            .AddToDB();

        spell.EffectDescription.EffectParticleParameters.zoneParticleReference =
            Entangle.EffectDescription.EffectParticleParameters.zoneParticleReference;

        return spell;
    }

    private sealed class PhysicalAttackFinishedByMeSpikeBarrage(
        FeatureDefinitionPower powerSpikeBarrage,
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionSpikeBarrage)
        : IPhysicalAttackFinishedByMe, IModifyEffectDescription
    {
        public bool IsValid(BaseDefinition definition, RulesetCharacter character, EffectDescription effectDescription)
        {
            return definition == powerSpikeBarrage;
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            if (!character.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, conditionSpikeBarrage.Name, out var activeCondition))
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

        public IEnumerator OnPhysicalAttackFinishedByMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            RollOutcome rollOutcome,
            int damageAmount)
        {
            if (rollOutcome is not (RollOutcome.Success or RollOutcome.CriticalSuccess))
            {
                yield break;
            }

            if (!attackMode.Ranged && !attackMode.Thrown)
            {
                yield break;
            }

            var targets = Gui.Battle.AllContenders
                .Where(x =>
                    x.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false } && x.IsWithinRange(defender, 1))
                .ToList();

            if (targets.Count == 0)
            {
                yield break;
            }

            var rulesetAttacker = attacker.RulesetCharacter;

            var implementationManager =
                ServiceRepository.GetService<IRulesetImplementationService>() as RulesetImplementationManager;

            var usablePower = PowerProvider.Get(powerSpikeBarrage, rulesetAttacker);
            var actionModifiers = new List<ActionModifier>();

            for (var i = 0; i < targets.Count; i++)
            {
                actionModifiers.Add(new ActionModifier());
            }

            var actionParams = new CharacterActionParams(attacker, ActionDefinitions.Id.PowerNoCost)
            {
                ActionModifiers = actionModifiers,
                RulesetEffect = implementationManager
                    .MyInstantiateEffectPower(rulesetAttacker, usablePower, false),
                UsablePower = usablePower,
                targetCharacters = targets
            };

            // must enqueue actions whenever within an attack workflow otherwise game won't consume attack
            ServiceRepository.GetService<IGameLocationActionService>()?
                .ExecuteAction(actionParams, null, true);
        }
    }

    #endregion

    #region Witch Bolt

    internal static SpellDefinition BuildWitchBolt()
    {
        const string NAME = "WitchBolt";

        var conditionWitchBolt = ConditionDefinitionBuilder
            .Create($"Condition{NAME}")
            .SetGuiPresentation(Category.Condition, ConditionShocked)
            .SetPossessive()
            .SetConditionType(ConditionType.Detrimental)
            .AddToDB();

        var powerWitchBolt = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}")
            .SetGuiPresentation(NAME, Category.Spell, LightningBolt)
            .SetUsesFixed(ActivationTime.Action)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetEffectForms(EffectFormBuilder.DamageForm(DamageTypeLightning, 1, DieType.D12))
                    .SetParticleEffectParameters(LightningBolt)
                    .Build())
            .AddToDB();

        var conditionWitchBoltSelf = ConditionDefinitionBuilder
            .Create($"Condition{NAME}Self")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(powerWitchBolt)
            .AddToDB();

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.WitchBolt, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetSpellLevel(1)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetSomaticComponent(true)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetRequiresConcentration(true)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Enemy, RangeType.RangeHit, 6, TargetType.IndividualsUnique)
                    .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, additionalDicePerIncrement: 1)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeLightning, 1, DieType.D12)
                            .Build(),
                        EffectFormBuilder.ConditionForm(conditionWitchBolt),
                        EffectFormBuilder.ConditionForm(
                            conditionWitchBoltSelf,
                            ConditionForm.ConditionOperation.Add, true))
                    .SetParticleEffectParameters(LightningBolt)
                    .Build())
            .AddToDB();

        powerWitchBolt.AddCustomSubFeatures(
            new CustomBehaviorWitchBolt(spell, powerWitchBolt, conditionWitchBolt));

        conditionWitchBolt.AddCustomSubFeatures(
            new ActionFinishedByMeWitchBoltEnemy(spell, conditionWitchBolt));

        conditionWitchBoltSelf.AddCustomSubFeatures(
            AddUsablePowersFromCondition.Marker,
            new ActionFinishedByMeWitchBolt(spell, powerWitchBolt, conditionWitchBolt));

        return spell;
    }

    private sealed class CustomBehaviorWitchBolt(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        SpellDefinition spellWitchBolt,
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinitionPower powerWitchBolt,
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionWitchBolt) : IFilterTargetingCharacter, IModifyEffectDescription
    {
        public bool EnforceFullSelection => false;

        public bool IsValid(CursorLocationSelectTarget __instance, GameLocationCharacter target)
        {
            if (target.RulesetCharacter == null)
            {
                return false;
            }

            var isValid = target.RulesetCharacter.HasConditionOfCategoryAndType(
                AttributeDefinitions.TagEffect, conditionWitchBolt.Name);

            if (!isValid)
            {
                __instance.actionModifier.FailureFlags.Add("Tooltip/&MustBeWitchBolt");
            }

            return isValid;
        }

        public bool IsValid(BaseDefinition definition, RulesetCharacter character, EffectDescription effectDescription)
        {
            return definition == powerWitchBolt;
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            var rulesetSpell = character.SpellsCastByMe.FirstOrDefault(x => x.SpellDefinition == spellWitchBolt);

            if (rulesetSpell == null)
            {
                return effectDescription;
            }

            var effectLevel = rulesetSpell.EffectLevel;

            effectDescription.EffectForms[0].DamageForm.DiceNumber = effectLevel;

            return effectDescription;
        }
    }

    private sealed class ActionFinishedByMeWitchBolt(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        SpellDefinition spellWitchBolt,
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinitionPower powerWitchBolt,
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionWitchBolt) : IActionFinishedByMe
    {
        public IEnumerator OnActionFinishedByMe(CharacterAction action)
        {
            // handle initial cases
            switch (action)
            {
                case CharacterActionUsePower actionUsePower when
                    actionUsePower.activePower.PowerDefinition == powerWitchBolt:
                    yield break;
                case CharacterActionSpendPower actionSpendPower when
                    actionSpendPower.activePower.PowerDefinition.ActivationTime
                        is ActivationTime.OnSpellNoCantripDamageAuto
                        or ActivationTime.OnAttackOrSpellHitAuto
                        or ActivationTime.OnKillCreatureWithSpell1OrMoreCR1OrMoreAuto:
                    yield break;
                case CharacterActionCastSpell actionCastSpell when
                    actionCastSpell.activeSpell.SpellDefinition == spellWitchBolt:
                    action.ActingCharacter.UsedSpecialFeatures.TryAdd(powerWitchBolt.Name, 0);
                    yield break;
            }

            var actingCharacter = action.ActingCharacter;
            var rulesetCharacter = actingCharacter.RulesetCharacter;

            if (action.ActionType
                is ActionDefinitions.ActionType.Move
                // these although allowed could potentially move both contenders off range
                or ActionDefinitions.ActionType.Bonus
                or ActionDefinitions.ActionType.Reaction
                or ActionDefinitions.ActionType.NoCost)
            {
                if (Gui.Battle == null)
                {
                    yield break;
                }

                var stillInRange = Gui.Battle
                    .GetContenders(actingCharacter, withinRange: 6)
                    .Any(x =>
                        x.RulesetCharacter.TryGetConditionOfCategoryAndType(
                            AttributeDefinitions.TagEffect, conditionWitchBolt.Name, out var activeCondition) &&
                        rulesetCharacter.Guid == activeCondition.SourceGuid);

                if (stillInRange)
                {
                    yield break;
                }
            }

            var rulesetSpell = rulesetCharacter.SpellsCastByMe.FirstOrDefault(x => x.SpellDefinition == spellWitchBolt);

            if (rulesetSpell != null)
            {
                rulesetCharacter.TerminateSpell(rulesetSpell);
            }
        }
    }

    private sealed class ActionFinishedByMeWitchBoltEnemy(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        SpellDefinition spellWitchBolt,
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionWitchBolt) : IActionFinishedByMe, IOnConditionAddedOrRemoved
    {
        public IEnumerator OnActionFinishedByMe(CharacterAction action)
        {
            if (Gui.Battle == null)
            {
                yield break;
            }

            var actingCharacter = action.ActingCharacter;
            var rulesetCharacter = actingCharacter.RulesetCharacter;

            if (!rulesetCharacter.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, conditionWitchBolt.Name, out var activeCondition))
            {
                yield break;
            }

            var stillInRange = Gui.Battle.GetContenders(actingCharacter, withinRange: 6).Any(x =>
                x.RulesetCharacter.Guid == activeCondition.SourceGuid);

            if (stillInRange)
            {
                yield break;
            }

            var rulesetCaster = EffectHelpers.GetCharacterByGuid(activeCondition.SourceGuid);

            if (rulesetCaster == null)
            {
                yield break;
            }

            var rulesetSpell = rulesetCharacter.SpellsCastByMe.FirstOrDefault(x => x.SpellDefinition == spellWitchBolt);

            if (rulesetSpell != null)
            {
                rulesetCaster.TerminateSpell(rulesetSpell);
            }
        }

        public void OnConditionAdded(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            // empty
        }

        public void OnConditionRemoved(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            var rulesetCaster = EffectHelpers.GetCharacterByGuid(rulesetCondition.SourceGuid);

            var rulesetSpell = rulesetCaster?.SpellsCastByMe.FirstOrDefault(x => x.SpellDefinition == spellWitchBolt);

            if (rulesetSpell != null)
            {
                rulesetCaster.TerminateSpell(rulesetSpell);
            }
        }
    }

    #endregion
}
