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
using TA.AI;
using TA.AI.Activities;
using UnityEngine.AddressableAssets;
using static ActionDefinitions;
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
            .SetImpactEffectParameters(AcidSplash)
            .SetSpeedAndImpactOffset(SpeedType.CellsPerSeconds, 12)
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
                        .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel,
                            additionalDicePerIncrement: 1)
                        .SetEffectForms(EffectFormBuilder.DamageForm(damageType, 3, DieType.D8))
                        .SetParticleEffectParameters(effectDescription.EffectParticleParameters)
                        .SetSpeedAndImpactOffset(SpeedType.CellsPerSeconds, 8.5f, offsetImpactTimePerTarget: 0.1f)
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
            .SetSubSpells([.. subSpells])
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.RangeHit, 12, TargetType.IndividualsUnique)
                    .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel,
                        additionalDicePerIncrement: 1)
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

        var battlePackage = AiHelpers.BuildDecisionPackageBreakFree("ConditionGrappledRestrainedEnsnared");

        var conditionEnsnared = ConditionDefinitionBuilder
            .Create("ConditionGrappledRestrainedEnsnared")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionRestrained)
            .SetConditionType(ConditionType.Detrimental)
            .SetParentCondition(ConditionDefinitions.ConditionRestrained)
            .SetFixedAmount((int)AiHelpers.BreakFreeType.DoStrengthCheckAgainstCasterDC)
            .SetBrain(battlePackage, true)
            .SetFeatures(ActionAffinityGrappled)
            .CopyParticleReferences(Entangle)
            .SetRecurrentEffectForms(
                EffectFormBuilder
                    .Create()
                    .SetDamageForm(DamageTypePiercing, 1, DieType.D6)
                    .SetCreatedBy()
                    .Build())
            .AddToDB();

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
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Enemy, RangeType.RangeHit, 12, TargetType.Individuals, 4)
                    .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel,
                        additionalTargetsPerIncrement: 1)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeRadiant, 1, DieType.D4)
                            .Build())
                    .SetParticleEffectParameters(Sparkle)
                    .SetSpeedAndImpactOffset(SpeedType.CellsPerSeconds, 20, offsetImpactTimePerTarget: 0.1f)
                    .Build())
            .AddToDB();

        return spell;
    }

    #endregion

    #region Ray of Sickness

    internal static SpellDefinition BuildRayOfSickness()
    {
        const string NAME = "RayOfSickness";

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.RayOfSickness, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolNecromancy)
            .SetSpellLevel(1)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                    .SetTargetingData(Side.Enemy, RangeType.RangeHit, 12, TargetType.Individuals)
                    .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel,
                        additionalDicePerIncrement: 1)
                    .SetEffectForms(
                        EffectFormBuilder.DamageForm(DamageTypePoison, 2, DieType.D8),
                        EffectFormBuilder.ConditionForm(ConditionDefinitions.ConditionPoisoned))
                    .SetParticleEffectParameters(PoisonSpray)
                    .SetEffectEffectParameters(Disintegrate)
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
            .SetRequiredProperty(RestrictedContextRequiredProperty.MeleeWeapon)
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
            .SetSpecialInterruptions(ExtraConditionInterruption.AttacksWithMeleeAndDamages)
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
            .SetRequiredProperty(RestrictedContextRequiredProperty.MeleeWeapon)
            .SetDamageDice(DieType.D6, 1)
            .SetSpecificDamageType(DamageTypeNecrotic)
            .SetAdvancement(AdditionalDamageAdvancement.SlotLevel)
            .SetSavingThrowData(
                EffectDifficultyClassComputation.SpellCastingFeature,
                EffectSavingThrowType.Negates,
                AttributeDefinitions.Wisdom)
            .AddConditionOperation(
                new ConditionOperationDescription
                {
                    operation = ConditionOperationDescription.ConditionOperation.Add,
                    conditionDefinition = ConditionDefinitionBuilder
                        .Create(ConditionDefinitions.ConditionFrightened, $"Condition{NAME}Frightened")
                        .SetParentCondition(ConditionDefinitions.ConditionFrightened)
                        .SetSpecialDuration(DurationType.Minute, 1)
                        .SetFeatures()
                        .AddToDB(),
                    hasSavingThrow = true,
                    canSaveToCancel = true,
                    saveAffinity = EffectSavingThrowType.Negates,
                    saveOccurence = TurnOccurenceType.EndOfTurn
                })
            .SetImpactParticleReference(Fear.EffectDescription.EffectParticleParameters.impactParticleReference)
            .AddToDB();

        var conditionWrathfulSmite = ConditionDefinitionBuilder
            .Create($"Condition{NAME}")
            .SetGuiPresentation(NAME, Category.Spell, ConditionBrandingSmite)
            .SetPossessive()
            .SetFeatures(additionalDamageWrathfulSmite)
            .SetSpecialInterruptions(ExtraConditionInterruption.AttacksWithMeleeAndDamages)
            .AddToDB();

        var spell = SpellDefinitionBuilder
            .Create(BrandingSmite, NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.WrathfulSmite, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolNecromancy)
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
                    .SetEffectForms(
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

        var battlePackage = AiHelpers.BuildDecisionPackageBreakFree(
            $"Condition{NAME}", AiHelpers.RandomType.RandomMedium);

        var conditionVileBrew = ConditionDefinitionBuilder
            .Create($"Condition{NAME}")
            .SetGuiPresentation(Category.Condition, ConditionAcidArrowed)
            .SetConditionType(ConditionType.Detrimental)
            .SetFixedAmount((int)AiHelpers.BreakFreeType.DoNoCheckAndRemoveCondition)
            .SetBrain(battlePackage, true)
            .SetFeatures(ActionAffinityGrappled)
            // need special duration here to enforce the recurrent damage at start of turn
            .SetSpecialDuration(DurationType.Minute, 1, TurnOccurenceType.StartOfTurn)
            .SetConditionParticleReference(ConditionOnAcidPilgrim)
            .SetRecurrentEffectForms(
                EffectFormBuilder
                    .Create()
                    .SetDamageForm(DamageTypeAcid, 2, DieType.D4)
                    .SetCreatedBy()
                    .Build())
            .AddToDB();

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
                .SetDurationData(DurationType.Minute, 1)
                .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Line, 6)
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
            .SetUsesFixed(ActivationTime.OnAttackHitMeleeAuto)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.IndividualsUnique)
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
            .SetGuiPresentation(
                $"{NAME}Title".Formatted(Category.Spell), Gui.EmptyContent, ConditionBrandingSmite)
            .SetPossessive()
            .SetFeatures(powerThunderousSmite)
            .AddCustomSubFeatures(AddUsablePowersFromCondition.Marker)
            .SetSpecialInterruptions(ExtraConditionInterruption.AttacksWithMeleeAndDamages)
            .AddToDB();

        conditionThunderousSmite.terminateWhenRemoved = true;

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

    private const string DamageTypeChaosBolt = "DamageChaosBolt";

    private static readonly (string, IMagicEffect)[] ChaosBoltDamagesAndEffects =
    [
        (DamageTypeAcid, AcidSplash), (DamageTypeCold, ConeOfCold),
        (DamageTypeFire, FireBolt), (DamageTypeForce, EldritchBlast),
        (DamageTypeLightning, LightningBolt), (DamageTypePoison, PoisonSpray),
        (DamageTypePsychic, Fear), (DamageTypeThunder, Shatter)
    ];

    internal static SpellDefinition BuildChaosBolt()
    {
        var formattedDamages = "";
        for (var i = 0; i < ChaosBoltDamagesAndEffects.Length; i++)
        {
            if (i > 0)
            {
                if (i % 2 == 1)
                {
                    formattedDamages += "  \t";
                }
                else
                {
                    formattedDamages += "\n";
                }
            }

            formattedDamages += $"{i + 1}: {Gui.FormatDamageType(ChaosBoltDamagesAndEffects[i].Item1, true)}";
        }

        var spellDescription = Gui.Format("Spell/&ChaosBoltDescription", formattedDamages);
        //267B = ♻
        var damageGui = GuiPresentationBuilder.Build(Gui.NoLocalization, Gui.NoLocalization, symbol: "267B");
        DamageDefinitionBuilder.Create(DamageTypeChaosBolt)
            .SetGuiPresentation(damageGui)
            .AddToDB();

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
            .SetGuiPresentation(NAME, Category.Spell, spellDescription, sprite)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetUseSpellAttack()
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.RangeHit, 24, TargetType.IndividualsUnique)
                    .SetEffectForms(
                        EffectFormBuilder.DamageForm(DamageTypeChaosBolt, 2, DieType.D8),
                        EffectFormBuilder.DamageForm(DamageTypeChaosBolt, 1, DieType.D6))
                    .SetParticleEffectParameters(GuidingBolt)
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
            .SetGuiPresentation(NAME, Category.Spell, Gui.EmptyContent)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetAmountOrigin(ConditionDefinition.OriginOfAmount.Fixed)
            .SetFeatures(powerLeap)
            .AddCustomSubFeatures(AddUsablePowersFromCondition.Marker)
            .AddToDB();

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, spellDescription, sprite)
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
                    .SetTargetingData(Side.Enemy, RangeType.RangeHit, 24, TargetType.IndividualsUnique)
                    .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel)
                    .SetEffectForms(
                        EffectFormBuilder.DamageForm(DamageTypeChaosBolt, 2, DieType.D8),
                        EffectFormBuilder.DamageForm(DamageTypeChaosBolt, 1, DieType.D6))
                    .SetParticleEffectParameters(GuidingBolt)
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
            CustomSpellAdvancementTooltip.ExtraDie(DieType.D6),
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
                __instance.actionModifier.FailureFlags.Add("Failure/&MustNotHaveChaosBoltMark");
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

            var firstRoll = RollDie(DieType.D8, AdvantageType.None, out _, out _);
            var secondRoll = RollDie(DieType.D8, AdvantageType.None, out _, out _);

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

                ModifyChaosBoltForms(actualEffectForms, damageType);
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

                var usablePowerPool = PowerProvider.Get(powerPool, rulesetAttacker);
                var usablePowerFirst = PowerProvider.Get(powers[firstRoll - 1], rulesetAttacker);
                var usablePowerSecond = PowerProvider.Get(powers[secondRoll - 1], rulesetAttacker);

                rulesetAttacker.UsablePowers.Add(usablePowerPool);
                rulesetAttacker.UsablePowers.Add(usablePowerFirst);
                rulesetAttacker.UsablePowers.Add(usablePowerSecond);

                yield return attacker.MyReactToSpendPowerBundle(
                    usablePowerPool,
                    [defender],
                    attacker,
                    "ChaosBolt",
                    string.Empty,
                    ReactionValidated,
                    ReactionNotValidated,
                    battleManager);

                rulesetAttacker.UsablePowers.Remove(usablePowerPool);
                rulesetAttacker.UsablePowers.Remove(usablePowerFirst);
                rulesetAttacker.UsablePowers.Remove(usablePowerSecond);

                void ReactionValidated(ReactionRequestSpendBundlePower reactionRequest)
                {
                    var (damageType, effect) = ChaosBoltDamagesAndEffects.ElementAt(reactionRequest.SelectedSubOption);

                    MagicEffect.Add((defender, effect));

                    ModifyChaosBoltForms(actualEffectForms, damageType);
                }

                void ReactionNotValidated(ReactionRequestSpendBundlePower reactionRequest)
                {
                    var choiceRoll = RollDie(DieType.D2, AdvantageType.None, out _, out _);
                    var option = choiceRoll == 1 ? firstRoll : secondRoll;
                    var (damageType, effect) = ChaosBoltDamagesAndEffects.ElementAt(option - 1);
                    var damageTitle = Gui.Localize($"Tooltip/&Tag{damageType}Title");

                    rulesetAttacker.LogCharacterActivatesAbility(
                        "Spell/&ChaosBoltTitle", "Feedback/&ChaosBoltRandomChoice",
                        extra:
                        [
                            (ConsoleStyleDuplet.ParameterType.Base, damageTitle)
                        ]);

                    MagicEffect.Add((defender, effect));

                    ModifyChaosBoltForms(actualEffectForms, damageType);
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

        private static void ModifyChaosBoltForms(List<EffectForm> actualEffectForms, string damageType)
        {
            foreach (var effectForm in actualEffectForms
                         .Where(x => x.FormType == EffectForm.EffectFormType.Damage &&
                                     x.DamageForm.DamageType == DamageTypeChaosBolt))
            {
                effectForm.DamageForm.DamageType = damageType;
            }
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

    #region Command

    internal static SpellDefinition BuildCommand()
    {
        const string NAME = "CommandSpell";

        // Approach

        #region Approach AI Behavior

        const string ConditionApproachName = $"Condition{NAME}Approach";

        var scorerApproach = AiHelpers.CreateActivityScorer(FixesContext.DecisionMoveAfraid, "MoveScorer_Approach");

        // invert PenalizeFearSourceProximityAtPosition if brain character has condition approach and enemy is condition source
        scorerApproach.scorer.WeightedConsiderations[2].Consideration.stringParameter = ConditionApproachName;
        // invert PenalizeVeryCloseEnemyProximityAtPosition if brain character has condition approach and enemy is condition source
        scorerApproach.scorer.WeightedConsiderations[1].Consideration.stringParameter = ConditionApproachName;

        var decisionApproach = DecisionDefinitionBuilder
            .Create("Move_Approach")
            .SetGuiPresentationNoContent(true)
            .SetDecisionDescription(
                "Go as close as possible to enemies.",
                nameof(Move),
                scorerApproach)
            .AddToDB();

        var packageApproach = DecisionPackageDefinitionBuilder
            .Create("Approach")
            .SetGuiPresentationNoContent(true)
            .SetWeightedDecisions(new WeightedDecisionDescription { decision = decisionApproach, weight = 9 })
            .AddToDB();

        #endregion

        var conditionApproach = ConditionDefinitionBuilder
            .Create(ConditionApproachName)
            .SetGuiPresentation($"{NAME}Approach", Category.Spell, ConditionPossessed)
            .SetConditionType(ConditionType.Detrimental)
            .SetPossessive()
            .SetBrain(packageApproach, forceBehavior: true, fearSource: true)
            .AddToDB();

        conditionApproach.AddCustomSubFeatures(new ActionFinishedByMeApproach(conditionApproach));

        var spellApproach = SpellDefinitionBuilder
            .Create($"{NAME}Approach")
            .SetGuiPresentation(Category.Spell, Command)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEnchantment)
            .SetSpellLevel(1)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetVerboseComponent(true)
            .SetSomaticComponent(false)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 12, TargetType.IndividualsUnique)
                    .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel,
                        additionalTargetsPerIncrement: 1)
                    .SetSavingThrowData(false, AttributeDefinitions.Wisdom, true,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetConditionForm(conditionApproach, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .SetParticleEffectParameters(Command)
                    .SetEffectEffectParameters(SpareTheDying)
                    .SetConditionEffectParameters()
                    .Build())
            .AddToDB();

        // Flee

        var conditionFlee = ConditionDefinitionBuilder
            .Create($"Condition{NAME}Flee")
            .SetGuiPresentation($"{NAME}Flee", Category.Spell, ConditionPossessed)
            .SetConditionType(ConditionType.Detrimental)
            .SetPossessive()
            .SetBrain(DecisionPackageDefinitions.Fear, forceBehavior: true, fearSource: true)
            .SetFeatures(MovementAffinityConditionDashing)
            .AddToDB();

        var spellFlee = SpellDefinitionBuilder
            .Create($"{NAME}Flee")
            .SetGuiPresentation(Category.Spell, Command)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEnchantment)
            .SetSpellLevel(1)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetVerboseComponent(true)
            .SetSomaticComponent(false)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 12, TargetType.IndividualsUnique)
                    .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel,
                        additionalTargetsPerIncrement: 1)
                    .SetSavingThrowData(false, AttributeDefinitions.Wisdom, true,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetConditionForm(conditionFlee, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .SetParticleEffectParameters(Command)
                    .SetEffectEffectParameters(SpareTheDying)
                    .SetConditionEffectParameters()
                    .Build())
            .AddToDB();

        // Grovel

        var conditionGrovel = ConditionDefinitionBuilder
            .Create($"Condition{NAME}Grovel")
            .SetGuiPresentation($"{NAME}Grovel", Category.Spell, ConditionPossessed)
            .SetConditionType(ConditionType.Detrimental)
            .SetPossessive()
            .SetSpecialDuration()
            .AddCustomSubFeatures(new CharacterBeforeTurnStartListenerCommandGrovel())
            .AddToDB();

        var spellGrovel = SpellDefinitionBuilder
            .Create($"{NAME}Grovel")
            .SetGuiPresentation(Category.Spell, Command)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEnchantment)
            .SetSpellLevel(1)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetVerboseComponent(true)
            .SetSomaticComponent(false)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 12, TargetType.IndividualsUnique)
                    .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel,
                        additionalTargetsPerIncrement: 1)
                    .SetSavingThrowData(false, AttributeDefinitions.Wisdom, true,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetConditionForm(conditionGrovel, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .SetParticleEffectParameters(Command)
                    .SetEffectEffectParameters(SpareTheDying)
                    .SetConditionEffectParameters()
                    .Build())
            .AddToDB();

        // Halt

        var conditionHalt = ConditionDefinitionBuilder
            .Create($"Condition{NAME}Halt")
            .SetGuiPresentation($"{NAME}Halt", Category.Spell, ConditionPossessed)
            .SetConditionType(ConditionType.Detrimental)
            .SetPossessive()
            .SetSpecialDuration()
            .SetFeatures(
                FeatureDefinitionActionAffinityBuilder
                    .Create($"ActionAffinity{NAME}Halt")
                    .SetGuiPresentationNoContent(true)
                    .SetAllowedActionTypes(false, false, false, false, false, false)
                    .AddToDB())
            .AddToDB();

        var spellHalt = SpellDefinitionBuilder
            .Create($"{NAME}Halt")
            .SetGuiPresentation(Category.Spell, Command)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEnchantment)
            .SetSpellLevel(1)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetVerboseComponent(true)
            .SetSomaticComponent(false)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 12, TargetType.IndividualsUnique)
                    .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel,
                        additionalTargetsPerIncrement: 1)
                    .SetSavingThrowData(false, AttributeDefinitions.Wisdom, true,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetConditionForm(conditionHalt, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .SetParticleEffectParameters(Command)
                    .SetEffectEffectParameters(SpareTheDying)
                    .SetConditionEffectParameters()
                    .Build())
            .AddToDB();

        // Command Spell

        // MAIN

        var behavior =
            new PowerOrSpellFinishedByMeCommand(conditionApproach, conditionFlee, conditionGrovel, conditionHalt);

        spellApproach.AddCustomSubFeatures(behavior);
        spellFlee.AddCustomSubFeatures(behavior);
        spellGrovel.AddCustomSubFeatures(behavior);
        spellHalt.AddCustomSubFeatures(behavior);

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Command)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEnchantment)
            .SetSpellLevel(1)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetVerboseComponent(true)
            .SetSomaticComponent(false)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetSubSpells(spellApproach, spellFlee, spellGrovel, spellHalt)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 12, TargetType.IndividualsUnique)
                    .SetEffectAdvancement(
                        EffectIncrementMethod.PerAdditionalSlotLevel, additionalTargetsPerIncrement: 1)
                    .SetSavingThrowData(false, AttributeDefinitions.Wisdom, true,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .Build())
            .AddToDB();

        return spell;
    }

    private sealed class PowerOrSpellFinishedByMeCommand(params ConditionDefinition[] conditions)
        : IPowerOrSpellFinishedByMe, IFilterTargetingCharacter
    {
        private static readonly Dictionary<string, string> FamilyLanguages = new()
        {
            //TODO: ideally we need to use proper LanguageDefinitions here instead of partial names
            { CharacterFamilyDefinitions.Dragon.Name, "Draconic" },
            { CharacterFamilyDefinitions.Elemental.Name, "Terran" },
            { CharacterFamilyDefinitions.Fey.Name, "Elvish" },
            { CharacterFamilyDefinitions.Fiend.Name, "Infernal" },
            { CharacterFamilyDefinitions.Giant.Name, "Giant" },
            { CharacterFamilyDefinitions.Humanoid.Name, "Common" }
        };

        public bool EnforceFullSelection => true;

        public bool IsValid(CursorLocationSelectTarget __instance, GameLocationCharacter target)
        {
            var selectedTargets = __instance.SelectionService.SelectedTargets;

            var failureFlags = __instance.actionModifier.FailureFlags;
            if (selectedTargets.Any(selectedTarget => !target.IsWithinRange(selectedTarget, 6)))
            {
                failureFlags.Add("Failure/&SecondTargetNotWithinRange");
                return false;
            }

            var rulesetTarget = target.RulesetCharacter;

            if (rulesetTarget.HasConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, RuleDefinitions.ConditionSurprised))
            {
                failureFlags.Add("Failure/&FailureFlagTargetMustNotBeSurprised");
                return false;
            }

            if (rulesetTarget.CharacterFamily == CharacterFamilyDefinitions.Undead.Name)
            {
                failureFlags.Add("Failure/&FailureFlagCannotTargetUndead");
                return false;
            }

            var rulesetCaster = __instance.ActionParams.ActingCharacter.RulesetCharacter.GetOriginalHero();

            if (rulesetCaster == null || !FamilyLanguages.TryGetValue(rulesetTarget.CharacterFamily, out var language))
            {
                failureFlags.Add("Failure/&FailureFlagTargetMustUnderstandYou");
                return false;
            }

            if (rulesetCaster.LanguageProficiencies.Contains($"Language_{language}"))
            {
                return true;
            }

            failureFlags.Add(Gui.Format("Failure/&FailureFlagMustKnowLanguage", $"Language/&{language}Title"));
            return false;
        }

        public IEnumerator OnPowerOrSpellFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            if (action.Countered || action.ExecutionFailed)
            {
                yield break;
            }

            var caster = action.ActingCharacter;

            foreach (var target in action.ActionParams.TargetCharacters)
            {
                var rulesetTarget = target.RulesetCharacter;
                var conditionsToRemove = new List<RulesetCondition>();
                var shouldRemove = false;

                foreach (var activeCondition in rulesetTarget.ConditionsByCategory
                             .SelectMany(x => x.Value)
                             .Where(activeCondition => conditions.Contains(activeCondition.ConditionDefinition)))
                {
                    if (activeCondition.SourceGuid == caster.Guid)
                    {
                        shouldRemove = true;
                    }
                    else
                    {
                        conditionsToRemove.Add(activeCondition);
                    }
                }

                if (!shouldRemove)
                {
                    yield break;
                }

                foreach (var condition in conditionsToRemove)
                {
                    rulesetTarget.RemoveCondition(condition);
                }
            }
        }
    }

    private sealed class ActionFinishedByMeApproach(ConditionDefinition conditionApproach) : IActionFinishedByMe
    {
        public IEnumerator OnActionFinishedByMe(CharacterAction action)
        {
            var actingCharacter = action.ActingCharacter;
            var rulesetCharacter = actingCharacter.RulesetCharacter;

            if (action.ActionId != Id.TacticalMove ||
                actingCharacter.MovingToDestination ||
                !actingCharacter.IsMyTurn() ||
                !rulesetCharacter.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, conditionApproach.Name, out var activeCondition))
            {
                yield break;
            }

            actingCharacter.UsedTacticalMoves = actingCharacter.MaxTacticalMoves;
            actingCharacter.UsedTacticalMovesChanged?.Invoke(actingCharacter);

            var rulesetCaster = EffectHelpers.GetCharacterByGuid(activeCondition.SourceGuid);
            var caster = GameLocationCharacter.GetFromActor(rulesetCaster);

            if (!caster.IsWithinRange(actingCharacter, 1))
            {
                rulesetCharacter.RemoveCondition(activeCondition);
            }
        }
    }

    private sealed class CharacterBeforeTurnStartListenerCommandGrovel : ICharacterTurnStartListener
    {
        public void OnCharacterTurnStarted(GameLocationCharacter locationCharacter)
        {
            var actionService = ServiceRepository.GetService<IGameLocationActionService>();
            var commandService = ServiceRepository.GetService<ICommandService>();
            var actionParams = new CharacterActionParams(locationCharacter, Id.DropProne)
            {
                CanBeAborted = false, CanBeCancelled = false
            };

            actionService.ExecuteAction(actionParams, null, false);
            commandService.EndTurn();
        }
    }

    #endregion

    #region Dissonant Whispers

    internal static SpellDefinition BuildDissonantWhispers()
    {
        const string NAME = "DissonantWhispers";

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.DissonantWhispers, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEnchantment)
            .SetSpellLevel(1)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetVerboseComponent(true)
            .SetSomaticComponent(false)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 12, TargetType.IndividualsUnique)
                    .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, additionalDicePerIncrement: 1)
                    .SetSavingThrowData(false, AttributeDefinitions.Wisdom, true,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .SetDamageForm(DamageTypePsychic, 3, DieType.D6)
                            .Build())
                    .SetCasterEffectParameters(Feeblemind)
                    .SetEffectEffectParameters(PowerBardTraditionVerbalOnslaught)
                    .Build())
            .AddCustomSubFeatures(new PowerOrSpellFinishedByMeDissonantWhispers())
            .AddToDB();

        return spell;
    }

    private sealed class PowerOrSpellFinishedByMeDissonantWhispers : IPowerOrSpellFinishedByMe
    {
        public IEnumerator OnPowerOrSpellFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            if (action.Countered || action.ExecutionFailed)
            {
                yield break;
            }

            var target = action.ActionParams.TargetCharacters[0];
            var actionManager = ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;

            if (!actionManager ||
                action.SaveOutcome == RollOutcome.Success ||
                !target.CanReact())
            {
                yield break;
            }

            target.SpendActionType(ActionType.Reaction);
            target.UsedTacticalMoves = 0;
            target.UsedTacticalMovesChanged?.Invoke(target);

            var pathfindingService = ServiceRepository.GetService<IGameLocationPathfindingService>();

            yield return pathfindingService
                .ComputeValidDestinationsAsync(target, target.LocationPosition, target.MaxTacticalMoves * 2);

            var positioningService = ServiceRepository.GetService<IGameLocationPositioningService>();
            var casterPosition = action.ActingCharacter.LocationPosition;
            var destinationPosition = target.LocationPosition;
            var candidatePositions = pathfindingService.ValidDestinations
                .Select(x => x.position)
                .Where(x =>
                    !positioningService.IsDangerousPosition(target, x) &&
                    !positioningService.IsDifficultGroundOrThroughForCharacter(target, x));

            foreach (var candidatePosition in candidatePositions)
            {
                var currentMagnitude = (destinationPosition - casterPosition).magnitude2DSqr;
                var candidateMagnitude = (candidatePosition - casterPosition).magnitude2DSqr;

                if (candidateMagnitude > currentMagnitude)
                {
                    destinationPosition = candidatePosition;
                }
            }

            target.MyExecuteActionTacticalMove(destinationPosition);
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
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 1, TargetType.IndividualsUnique)
                    .SetSavingThrowData(false, AttributeDefinitions.Dexterity, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
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
            if (rulesetEffect is RulesetEffectPower rulesetEffectPower)
            {
                effectDescription.EffectForms[0].DamageForm.DiceNumber =
                    2 + (rulesetEffectPower.usablePower.spentPoints - 1);
            }

            return effectDescription;
        }
    }

    private sealed class PowerOrSpellFinishedByMeIceBlade(FeatureDefinitionPower powerIceBlade)
        : IPowerOrSpellFinishedByMe
    {
        public IEnumerator OnPowerOrSpellFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            if (Gui.Battle == null)
            {
                yield break;
            }

            if (action is not CharacterActionCastSpell actionCastSpell || action.Countered || action.ExecutionFailed)
            {
                yield break;
            }

            var caster = actionCastSpell.ActingCharacter;
            var rulesetCaster = caster.RulesetCharacter;
            var usablePower = PowerProvider.Get(powerIceBlade, rulesetCaster);

            // use spentPoints to store effect level to be used later by power
            usablePower.spentPoints = action.ActionParams.RulesetEffect.EffectLevel;

            // need to loop over target characters to support twinned metamagic scenarios
            foreach (var targets in actionCastSpell.ActionParams.TargetCharacters
                         .Select(target => Gui.Battle.AllContenders
                             .Where(x =>
                                 x.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false } &&
                                 x.IsWithinRange(target, 1))
                             .ToArray()))
            {
                caster.MyExecuteActionSpendPower(usablePower, targets);
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
            CharacterAction action,
            GameLocationCharacter attacker,
            List<GameLocationCharacter> targets)
        {
            if (action.AttackRollOutcome is not (RollOutcome.Success or RollOutcome.CriticalSuccess) ||
                action.Countered ||
                action is CharacterActionCastSpell { ExecutionFailed: true } ||
                action.ActionParams.RulesetEffect.EffectDescription.RangeType
                    is not (RangeType.Touch or RangeType.MeleeHit))
            {
                yield break;
            }

            attacker.RulesetCharacter.RemoveAllConditionsOfCategoryAndType(
                AttributeDefinitions.TagEffect, conditionElementalInfusion.Name);
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
        IPhysicalAttackBeforeHitConfirmedOnMe, IMagicEffectBeforeHitConfirmedOnMe
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
            yield return HandleReaction(battleManager, attacker, defender, actualEffectForms);
        }

        public IEnumerator OnPhysicalAttackBeforeHitConfirmedOnMe(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier actionModifier,
            RulesetAttackMode attackMode,
            bool rangedAttack,
            AdvantageType advantageType,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            yield return HandleReaction(battleManager, attacker, defender, actualEffectForms);
        }

        private IEnumerator HandleReaction(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            IEnumerable<EffectForm> actualEffectForms)
        {
            if (!defender.CanReact())
            {
                yield break;
            }

            var attackDamageTypes = actualEffectForms
                .Where(x => x.FormType == EffectForm.EffectFormType.Damage)
                .Select(x => x.DamageForm.DamageType)
                .Distinct()
                .ToArray();

            var resistanceDamageTypes = AllowedDamageTypes.Intersect(attackDamageTypes).ToArray();

            if (resistanceDamageTypes.Length == 0)
            {
                yield break;
            }

            var rulesetDefender = defender.RulesetCharacter;

            yield return defender.MyReactToCastSpell(
                SpellsContext.ElementalInfusion, defender, attacker, ReactionValidated, battleManager);

            yield break;

            void ReactionValidated(CharacterActionParams actionParams)
            {
                var slotUsed = actionParams.IntParameter;

                EffectHelpers.StartVisualEffect(defender, defender, ShadowArmor, EffectHelpers.EffectType.Caster);
                EffectHelpers.StartVisualEffect(defender, defender, ShadowArmor, EffectHelpers.EffectType.Effect);

                foreach (var condition in resistanceDamageTypes
                             .Select(damageType =>
                                 GetDefinition<ConditionDefinition>(
                                     $"Condition{spellDefinition.Name}{damageType.Substring(6)}Resistance")))
                {
                    rulesetDefender.InflictCondition(
                        condition.Name,
                        DurationType.Round,
                        0,
                        TurnOccurenceType.StartOfTurn,
                        AttributeDefinitions.TagEffect,
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
            .SetGuiPresentation(Category.Condition, Gui.EmptyContent, ConditionDefinitions.ConditionDisengaging)
            .SetPossessive()
            .SetFeatures(movementAffinityStrikeWithTheWind)
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
            .SetConditionParticleReference(PowerWindGuidingWinds)
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

    private const int TempHpPerLevelSkinOfRetribution = 5;

    private const string SkinOfRetributionName = "SkinOfRetribution";

    internal static SpellDefinition BuildSkinOfRetribution()
    {
        var powerSkinOfRetribution = FeatureDefinitionPowerBuilder
            .Create($"Power{SkinOfRetributionName}")
            .SetGuiPresentationNoContent(true)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetEffectForms(EffectFormBuilder.DamageForm(DamageTypeCold, 0, DieType.D1,
                        TempHpPerLevelSkinOfRetribution))
                    .SetParticleEffectParameters(ConeOfCold)
                    .Build())
            .AddToDB();

        powerSkinOfRetribution.AddCustomSubFeatures(
            new ModifyEffectDescriptionSkinOfRetribution(powerSkinOfRetribution));

        var damageAffinitySkinOfRetribution = FeatureDefinitionDamageAffinityBuilder
            .Create($"DamageAffinity{SkinOfRetributionName}")
            .SetGuiPresentationNoContent(true)
            // max possible reach in game is 15 ft
            .SetRetaliate(powerSkinOfRetribution, 3)
            .AddToDB();

        var conditionSkinOfRetribution = ConditionDefinitionBuilder
            .Create($"Condition{SkinOfRetributionName}")
            .SetGuiPresentation(Category.Condition,
                Sprites.GetSprite("ConditionMirrorImage", Resources.ConditionMirrorImage, 32))
            .SetSilent(Silent.WhenAdded)
            .SetPossessive()
            .SetFeatures(damageAffinitySkinOfRetribution)
            .CopyParticleReferences(PowerDomainElementalHeraldOfTheElementsCold)
            .AddToDB();

        conditionSkinOfRetribution.AddCustomSubFeatures(
            new CustomBehaviorSkinOfRetribution(conditionSkinOfRetribution));

        return SpellDefinitionBuilder
            .Create(SkinOfRetributionName)
            .SetGuiPresentation(Category.Spell,
                Sprites.GetSprite(SkinOfRetributionName, Resources.SkinOfRetribution, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolAbjuration)
            .SetSpellLevel(1)
            .SetCastingTime(ActivationTime.BonusAction)
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
                            .SetTempHpForm(TempHpPerLevelSkinOfRetribution)
                            .Build(),
                        EffectFormBuilder.ConditionForm(conditionSkinOfRetribution))
                    .SetEffectAdvancement(
                        EffectIncrementMethod.PerAdditionalSlotLevel,
                        additionalTempHpPerIncrement: TempHpPerLevelSkinOfRetribution)
                    .SetParticleEffectParameters(ConeOfCold)
                    .Build())
            .AddToDB();
    }

    private sealed class ModifyEffectDescriptionSkinOfRetribution(FeatureDefinitionPower powerSkinOfRetribution)
        : IModifyEffectDescription
    {
        public bool IsValid(
            BaseDefinition definition,
            RulesetCharacter character,
            EffectDescription effectDescription)
        {
            return definition == powerSkinOfRetribution;
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            var glc = GameLocationCharacter.GetFromActor(character);

            if (glc == null)
            {
                return effectDescription;
            }

            var effectLevel = glc.GetSpecialFeatureUses(SkinOfRetributionName, 1);
            var damageForm = effectDescription.FindFirstDamageForm();

            damageForm.BonusDamage = TempHpPerLevelSkinOfRetribution * effectLevel;

            return effectDescription;
        }
    }

    private sealed class CustomBehaviorSkinOfRetribution(ConditionDefinition conditionSkinOfRetribution)
        : IMagicEffectBeforeHitConfirmedOnMe, IMagicEffectFinishedOnMe,
            IPhysicalAttackBeforeHitConfirmedOnMe, IPhysicalAttackFinishedOnMe
    {
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
            Set(defender);

            yield break;
        }

        public IEnumerator OnMagicEffectFinishedOnMe(
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            List<GameLocationCharacter> targets)
        {
            Reset(defender.RulesetCharacter);

            yield break;
        }

        public IEnumerator OnPhysicalAttackBeforeHitConfirmedOnMe(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier actionModifier,
            RulesetAttackMode attackMode,
            bool rangedAttack,
            AdvantageType advantageType,
            List<EffectForm> actualEffectForms,
            bool firstTarget, bool criticalHit)
        {
            Set(defender);

            yield break;
        }


        public IEnumerator OnPhysicalAttackFinishedOnMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            RollOutcome rollOutcome,
            int damageAmount)
        {
            Reset(defender.RulesetCharacter);

            yield break;
        }

        private void Set(GameLocationCharacter defender)
        {
            var rulesetDefender = defender.RulesetCharacter;

            if (rulesetDefender.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, conditionSkinOfRetribution.Name, out var activeCondition))
            {
                defender.SetSpecialFeatureUses(SkinOfRetributionName, activeCondition.EffectLevel);
            }
        }

        private void Reset(RulesetCharacter rulesetDefender)
        {
            if (rulesetDefender.TemporaryHitPoints == 0 &&
                rulesetDefender.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, conditionSkinOfRetribution.Name, out var activeCondition))
            {
                rulesetDefender.RemoveCondition(activeCondition);
            }
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
                Tabletop2014Context.FeatureDefinitionPowerHelpAction,
                RulesContext.PowerTeleportSummon,
                RulesContext.PowerVanishSummon)
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

            Gui.Battle.ContenderModified(
                locationCharacter, GameLocationBattle.ContenderModificationMode.Remove, false, false);
            locationCharacter.LastInitiative += roll;
            Gui.Battle.initiativeSortedContenders.Sort(Gui.Battle);
            Gui.Battle.ContenderModified(
                locationCharacter, GameLocationBattle.ContenderModificationMode.Add, false, false);

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
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 1, TargetType.IndividualsUnique)
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
            if (character.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, conditionSpikeBarrage.Name, out var activeCondition))
            {
                effectDescription.EffectForms[0].DamageForm.DiceNumber = activeCondition.EffectLevel;
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
                .ToArray();
            var rulesetAttacker = attacker.RulesetCharacter;
            var usablePower = PowerProvider.Get(powerSpikeBarrage, rulesetAttacker);

            attacker.MyExecuteActionSpendPower(usablePower, targets);
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
            .SetConditionParticleReference(PowerTraditionShockArcanistArcaneFury)
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
                    .SetParticleEffectParameters(ChainLightning)
                    .SetImpactEffectParameters(LightningBolt)
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
                    .SetParticleEffectParameters(ChainLightning)
                    .SetImpactEffectParameters(LightningBolt)
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
                __instance.actionModifier.FailureFlags.Add("Failure/&MustBeWitchBolt");
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
            if (character.ConcentratedSpell != null &&
                character.ConcentratedSpell.SpellDefinition == spellWitchBolt)
            {
                effectDescription.EffectForms[0].DamageForm.DiceNumber =
                    1 + (character.ConcentratedSpell.EffectLevel - 1);
            }

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
                case CharacterActionUsePower when
                    action.ActionParams.RulesetEffect.SourceDefinition == powerWitchBolt:
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
                is ActionType.Move
                // these although allowed could potentially move both contenders off range
                or ActionType.Bonus
                or ActionType.Reaction
                or ActionType.NoCost)
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
