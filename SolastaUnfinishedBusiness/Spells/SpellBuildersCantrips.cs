using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Models;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static ActionDefinitions;
using static RuleDefinitions;
using static FeatureDefinitionAttributeModifier;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterFamilyDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using Resources = SolastaUnfinishedBusiness.Properties.Resources;

namespace SolastaUnfinishedBusiness.Spells;

internal static partial class SpellBuilders
{
    private static readonly (string, IMagicEffect)[] DamagesAndEffects =
    [
        (DamageTypeAcid, AcidSplash), (DamageTypeCold, ConeOfCold), (DamageTypeFire, FireBolt),
        (DamageTypeLightning, LightningBolt), (DamageTypePoison, PoisonSpray), (DamageTypeThunder, Shatter)
    ];

    #region Acid Claws

    internal static SpellDefinition BuildAcidClaw()
    {
        const string NAME = "AcidClaws";

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.AcidClaws, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(0)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetVerboseComponent(false)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1, (TurnOccurenceType)ExtraTurnOccurenceType.StartOfSourceTurn)
                    .SetTargetingData(Side.Enemy, RangeType.MeleeHit, 1, TargetType.IndividualsUnique)
                    .SetEffectAdvancement(EffectIncrementMethod.CasterLevelTable, additionalDicePerIncrement: 1)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeAcid, 1, DieType.D8)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(
                                ConditionDefinitionBuilder
                                    .Create("ConditionAcidClaws")
                                    .SetGuiPresentation(Category.Condition, ConditionAcidSpit)
                                    .SetConditionType(ConditionType.Detrimental)
                                    .SetFeatures(
                                        FeatureDefinitionAttributeModifierBuilder
                                            .Create("AttributeModifierAcidClawsACDebuff")
                                            .SetGuiPresentation("ConditionAcidClaws", Category.Condition)
                                            .SetModifier(AttributeModifierOperation.Additive,
                                                AttributeDefinitions.ArmorClass, -1)
                                            .AddToDB())
                                    .AddToDB(), ConditionForm.ConditionOperation.Add)
                            .Build())
                    .SetParticleEffectParameters(AcidSplash)
                    .Build())
            .AddToDB();

        return spell;
    }

    #endregion

    #region Air Blast

    internal static SpellDefinition BuildAirBlast()
    {
        const string NAME = "AirBlast";

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.AirBlast, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(0)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetEffectAdvancement(EffectIncrementMethod.CasterLevelTable, additionalDicePerIncrement: 1)
                    .SetSavingThrowData(false, AttributeDefinitions.Strength, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeBludgeoning, 1, DieType.D6)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetMotionForm(MotionForm.MotionType.PushFromOrigin, 1)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build())
                    .SetParticleEffectParameters(WindWall)
                    .Build())
            .AddToDB();

        return spell;
    }

    #endregion

    #region Blade Ward

    internal static SpellDefinition BuildBladeWard()
    {
        const string NAME = "BladeWard";

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.BladeWard, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolAbjuration)
            .SetSpellLevel(0)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Defense)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(
                            ConditionDefinitionBuilder
                                .Create($"Condition{NAME}")
                                .SetGuiPresentation(NAME, Category.Spell, ConditionShielded)
                                .SetFeatures(
                                    DamageAffinityBludgeoningResistanceTrue,
                                    DamageAffinitySlashingResistanceTrue,
                                    DamageAffinityPiercingResistanceTrue)
                                .AddToDB()))
                    .SetParticleEffectParameters(FeatureDefinitionPowers.PowerPatronHiveReactiveCarapace)
                    .SetCasterEffectParameters(GuidingBolt)
                    .Build())
            .AddToDB();

        return spell;
    }

    #endregion

    #region Burst of Radiance

    internal static SpellDefinition BuildBurstOfRadiance()
    {
        const string NAME = "BurstOfRadiance";

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.BurstOfRadiance, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetSpellLevel(0)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetVerboseComponent(true)
            .SetSomaticComponent(false)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Cube, 3)
                    .SetEffectAdvancement(EffectIncrementMethod.CasterLevelTable, additionalDicePerIncrement: 1)
                    .SetSavingThrowData(false, AttributeDefinitions.Constitution, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeRadiant, 1, DieType.D6)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build())
                    .SetParticleEffectParameters(SacredFlame)
                    .SetImpactEffectParameters(SacredFlame
                        .EffectDescription.EffectParticleParameters.effectParticleReference)
                    .Build())
            .AddToDB();

        return spell;
    }

    #endregion

    #region Enduring Sting

    internal static SpellDefinition BuildEnduringSting()
    {
        const string NAME = "EnduringSting";

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.EnduringSting, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolNecromancy)
            .SetSpellLevel(0)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Defense)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetEffectAdvancement(EffectIncrementMethod.CasterLevelTable, additionalDicePerIncrement: 1)
                    .SetSavingThrowData(false, AttributeDefinitions.Constitution, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeNecrotic, 1, DieType.D4)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetMotionForm(MotionForm.MotionType.FallProne)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build())
                    .SetParticleEffectParameters(Bane)
                    .SetImpactEffectParameters(VenomousSpike)
                    .Build())
            .AddToDB();

        return spell;
    }

    #endregion

    #region Illuminating Sphere

    internal static SpellDefinition BuildIlluminatingSphere()
    {
        const string NAME = "IlluminatingSphere";

        var spell = SpellDefinitionBuilder
            .Create(Sparkle, NAME)
            .SetGuiPresentation(Category.Spell, Shine)
            .SetVocalSpellSameType(VocalSpellSemeType.Detection)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(Sparkle.EffectDescription)
                    .SetTargetingData(Side.All, RangeType.Distance, 18, TargetType.Sphere, 6)
                    .SetParticleEffectParameters(SacredFlame_B)
                    .Build())
            .AddToDB();

        return spell;
    }

    #endregion

    #region Infestation

    internal static SpellDefinition BuildInfestation()
    {
        const string NAME = "Infestation";

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.Infestation, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolConjuration)
            .SetSpellLevel(0)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetEffectAdvancement(EffectIncrementMethod.CasterLevelTable, additionalDicePerIncrement: 1)
                    .SetSavingThrowData(false, AttributeDefinitions.Constitution, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypePoison, 1, DieType.D6)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetMotionForm(MotionForm.MotionType.PushRandomDirection, 1)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build())
                    .SetCasterEffectParameters(PoisonSpray)
                    .SetImpactEffectParameters(
                        PoisonSpray.EffectDescription.EffectParticleParameters.effectParticleReference)
                    .Build())
            .AddToDB();

        return spell;
    }

    #endregion

    #region Lightning Lure

    internal static SpellDefinition BuildLightningLure()
    {
        const string NAME = "LightningLure";

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.LightningLure, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetSpellLevel(0)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetVerboseComponent(true)
            .SetSomaticComponent(false)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 3, TargetType.IndividualsUnique)
                    .SetEffectAdvancement(EffectIncrementMethod.CasterLevelTable, additionalDicePerIncrement: 1)
                    .SetSavingThrowData(false, AttributeDefinitions.Strength, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeLightning, 1, DieType.D8)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetMotionForm(MotionForm.MotionType.DragToOrigin, 2)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build())
                    .SetParticleEffectParameters(LightningBolt)
                    .Build())
            .AddToDB();

        return spell;
    }

    #endregion

#if false
    #region Magic Stone

    internal static SpellDefinition BuildMagicStone()
    {
        const string NAME = "MagicStone";

        var itemShadowStone = ItemDefinitionBuilder
            .Create(ItemDefinitions.Dart, $"Item{NAME}")
            //.SetGuiPresentation(Category.Item)
            .SetItemTags(TagsDefinitions.ItemTagConjured)
            .SetStaticProperties(
                ItemPropertyDescriptionBuilder.From(
                        FeatureDefinitionBuilder
                            .Create($"Feature{NAME}")
                            .SetGuiPresentation($"Feature{NAME}", Category.Feature)
                            .AddToDB(),
                        knowledgeAffinity: EquipmentDefinitions.KnowledgeAffinity.ActiveAndVisible)
                    .Build())
            .HideFromDungeonEditor()
            .AddToDB();

        itemShadowStone.activeTags.Clear();
        itemShadowStone.itemPresentation.assetReference =
            ItemDefinitions.StoneOfGoodLuck.itemPresentation.assetReference;
        itemShadowStone.weaponDefinition.EffectDescription.EffectParticleParameters.impactParticleReference =
            EffectProxyDefinitions.ProxyArcaneSword.attackImpactParticle;

        var weaponDescription = itemShadowStone.WeaponDescription;

        weaponDescription.closeRange = 12;
        weaponDescription.maxRange = 12;

        var damageForm = weaponDescription.EffectDescription.FindFirstDamageForm();

        damageForm.damageType = DamageTypeBludgeoning;
        damageForm.dieType = DieType.D6;
        damageForm.diceNumber = 1;

        var condition = ConditionDefinitionBuilder
            .Create($"Condition{NAME}")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddToDB();

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.MagicStone, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(0)
            .SetCastingTime(ActivationTime.BonusAction)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Ally, RangeType.Touch, 0, TargetType.IndividualsUnique)
                    .SetEffectAdvancement(EffectIncrementMethod.CasterLevelTable, additionalSummonsPerIncrement: 1)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetSummonItemForm(itemShadowStone, 3, true)
                            .Build(),
                        EffectFormBuilder.ConditionForm(condition, ConditionForm.ConditionOperation.Add, true))
                    .SetParticleEffectParameters(ShadowDagger)
                    .Build())
            .AddToDB();

        spell.EffectDescription.slotTypes =
        [
            EquipmentDefinitions.SlotTypeMainHand,
            EquipmentDefinitions.SlotTypeOffHand,
            EquipmentDefinitions.SlotTypeContainer
        ];

        return spell;
    }

    #endregion
#endif

    #region Mind Spike

    internal static SpellDefinition BuildMindSpike()
    {
        const string NAME = "MindSpike";

        var conditionMindSpike = ConditionDefinitionBuilder
            .Create(ConditionBaned, $"Condition{NAME}")
            .SetOrUpdateGuiPresentation(Category.Condition)
            .SetFeatures(FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinityConditionBaned)
            .SetSpecialInterruptions(ConditionInterruption.SavingThrow)
            .AddToDB();

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.MindSpike, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEnchantment)
            .SetSpellLevel(0)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetVerboseComponent(true)
            .SetSomaticComponent(false)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 12, TargetType.IndividualsUnique)
                    .SetEffectAdvancement(EffectIncrementMethod.CasterLevelTable, additionalDicePerIncrement: 1)
                    .SetSavingThrowData(false, AttributeDefinitions.Intelligence, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypePsychic, 1, DieType.D6)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionMindSpike, ConditionForm.ConditionOperation.Add)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build())
                    .SetParticleEffectParameters(ShadowDagger)
                    .Build())
            .AddToDB();

        return spell;
    }

    #endregion

    #region Minor Life Steal

    internal static SpellDefinition BuildMinorLifesteal()
    {
        var spell = SpellDefinitionBuilder
            .Create("MinorLifesteal")
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite("MinorLifesteal", Resources.MinorLifesteal, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolNecromancy)
            .SetSpellLevel(0)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Hour, 1)
                    .SetTargetingData(Side.Enemy, RangeType.MeleeHit, 1, TargetType.IndividualsUnique)
                    .SetEffectAdvancement(EffectIncrementMethod.CasterLevelTable, additionalDicePerIncrement: 1)
                    .AddImmuneCreatureFamilies(Construct, Undead)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeNecrotic, 1, DieType.D6, 0, HealFromInflictedDamage.Half)
                            .Build())
                    .SetParticleEffectParameters(VampiricTouch)
                    .Build())
            .AddToDB();

        return spell;
    }

    #endregion

    #region Primal Savagery

    internal static SpellDefinition BuildPrimalSavagery()
    {
        const string NAME = "PrimalSavagery";

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.AcidClaws, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(0)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetVerboseComponent(false)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.MeleeHit, 1, TargetType.IndividualsUnique)
                    .SetEffectAdvancement(EffectIncrementMethod.CasterLevelTable, additionalDicePerIncrement: 1)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeAcid, 1, DieType.D10)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build())
                    .SetCasterEffectParameters(AcidSplash)
                    .SetImpactEffectParameters(AcidArrow)
                    .Build())
            .AddToDB();

        return spell;
    }

    #endregion

    #region Starry Wisp

    internal static SpellDefinition BuildStarryWisp()
    {
        const string NAME = "StarryWisp";

        var lightSourceForm =
            SpellDefinitions.Light.EffectDescription.GetFirstFormOfType(EffectForm.EffectFormType.LightSource);

        var condition = ConditionDefinitionBuilder
            .Create($"Condition{NAME}")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionLightSensitive)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetConditionType(ConditionType.Detrimental)
            .AddToDB();

        ConditionInvisibleBase.cancellingConditions.Add(condition);

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.StarryWisp, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetSpellLevel(0)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(SpellDefinitions.Light)
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                    .SetTargetingData(Side.Enemy, RangeType.RangeHit, 12, TargetType.IndividualsUnique)
                    .SetEffectAdvancement(EffectIncrementMethod.CasterLevelTable, additionalDicePerIncrement: 1)
                    .SetEffectForms(
                        EffectFormBuilder.DamageForm(DamageTypeRadiant, 1, DieType.D8),
                        EffectFormBuilder.ConditionForm(condition),
                        EffectFormBuilder.ConditionForm(
                            ConditionDefinitions.ConditionInvisible, ConditionForm.ConditionOperation.Remove),
                        EffectFormBuilder
                            .Create()
                            .SetLightSourceForm(
                                LightSourceType.Basic, 0, 2,
                                lightSourceForm.lightSourceForm.color,
                                lightSourceForm.lightSourceForm.graphicsPrefabReference)
                            .Build())
                    .SetParticleEffectParameters(FaerieFire)
                    .Build())
            .AddToDB();

        return spell;
    }

    #endregion

    #region Sunlit Blade

    internal static SpellDefinition BuildSunlightBlade()
    {
        var conditionSunlightBlade = ConditionDefinitionBuilder
            .Create("ConditionSunlightBlade")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(
                FeatureDefinitionAdditionalDamageBuilder
                    .Create("AdditionalDamageSunlightBlade")
                    .SetGuiPresentationNoContent(true)
                    .SetNotificationTag("SunlightBlade")
                    .SetRequiredProperty(RestrictedContextRequiredProperty.MeleeWeapon)
                    .SetAttackModeOnly()
                    .SetDamageDice(DieType.D8, 0)
                    .SetSpecificDamageType(DamageTypeRadiant)
                    .SetAdvancement(
                        ExtraAdditionalDamageAdvancement.CharacterLevel,
                        DiceByRankBuilder.InterpolateDiceByRankTable(0, 20, (5, 1), (11, 2), (17, 3)))
                    .AddConditionOperation(
                        ConditionOperationDescription.ConditionOperation.Add,
                        ConditionDefinitionBuilder
                            .Create(ConditionHighlighted, "ConditionSunlightBladeHighlighted")
                            // don't use AfterWasAttacked here as it gets removed too soon
                            .SetSpecialInterruptions(ConditionInterruption.Attacked)
                            .SetSpecialDuration(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
                            .AddToDB())
                    .SetAddLightSource(true)
                    .SetLightSourceForm(new LightSourceForm
                    {
                        brightRange = 0,
                        dimAdditionalRange = 2,
                        lightSourceType = LightSourceType.Basic,
                        color = new Color(0.9f, 0.8f, 0.4f),
                        graphicsPrefabReference = FeatureDefinitionAdditionalDamages
                            .AdditionalDamageBrandingSmite.LightSourceForm.graphicsPrefabReference
                    })
                    .SetImpactParticleReference(DivineFavor)
                    .AddToDB())
            .SetSpecialInterruptions(ConditionInterruption.Attacks)
            .AddToDB();

        var spell = SpellDefinitionBuilder
            .Create("SunlightBlade")
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite("SunlightBlade", Resources.SunlightBlade, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetSpellLevel(0)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.Specific)
            .SetSpecificMaterialComponent(TagsDefinitions.WeaponTagMelee, 0, false)
            .SetVerboseComponent(false)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetIgnoreCover()
                    .SetEffectAdvancement(EffectIncrementMethod.CasterLevelTable, additionalDicePerIncrement: 1)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(
                            conditionSunlightBlade,
                            ConditionForm.ConditionOperation.Add, true))
                    .SetParticleEffectParameters(DivineFavor)
                    .Build())
            .AddCustomSubFeatures(SrdAndHouseRulesContext.NoTwinned.Mark, new AttackAfterMagicEffect())
            .AddToDB();

        return spell;
    }

    #endregion

    #region Sword Storm

    internal static SpellDefinition BuildSwordStorm()
    {
        const string NAME = "SwordStorm";

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.SwordStorm, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEnchantment)
            .SetSpellLevel(0)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Debuff)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Cube, 3)
                    .SetEffectAdvancement(EffectIncrementMethod.CasterLevelTable, additionalDicePerIncrement: 1)
                    .SetSavingThrowData(false, AttributeDefinitions.Dexterity, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeForce, 1, DieType.D6)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build())
                    .SetParticleEffectParameters(ShadowDagger)
                    .Build())
            .AddToDB();

        return spell;
    }

    #endregion

    #region Thorny Vines

    internal static SpellDefinition BuildThornyVines()
    {
        var spell = SpellDefinitionBuilder
            .Create("ThornyVines")
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite("ThornyVines", Resources.ThornyVines, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(0)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Debuff)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.MeleeHit, 6, TargetType.IndividualsUnique)
                    .SetEffectAdvancement(EffectIncrementMethod.CasterLevelTable, 5,
                        additionalDicePerIncrement: 1)
                    .SetEffectForms(
                        EffectFormBuilder.DamageForm(DamageTypePiercing, 1, DieType.D6),
                        EffectFormBuilder.MotionForm(MotionForm.MotionType.DragToOrigin, 2))
                    .SetParticleEffectParameters(VenomousSpike)
                    .Build())
            .AddToDB();

        return spell;
    }

    #endregion

    #region Thunder Strike

    internal static SpellDefinition BuildThunderStrike()
    {
        const string NAME = "ThunderStrike";

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Shield)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetSpellLevel(0)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Cube, 3)
                    .SetEffectAdvancement(EffectIncrementMethod.CasterLevelTable, additionalDicePerIncrement: 1)
                    .SetSavingThrowData(false, AttributeDefinitions.Constitution, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .ExcludeCaster()
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeThunder, 1, DieType.D6)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build())
                    .SetParticleEffectParameters(Shatter)
                    .Build())
            .AddToDB();

        spell.EffectDescription.EffectParticleParameters.impactParticleReference =
            spell.EffectDescription.EffectParticleParameters.zoneParticleReference;
        spell.EffectDescription.EffectParticleParameters.zoneParticleReference = new AssetReference();

        return spell;
    }

    #endregion

    #region Wrack

    internal static SpellDefinition BuildWrack()
    {
        const string NAME = "Wrack";

        var conditionWrack = ConditionDefinitionBuilder
            .Create($"Condition{NAME}")
            .SetGuiPresentation(Category.Condition, ConditionHindered)
            .SetConditionType(ConditionType.Detrimental)
            .AddFeatures(
                FeatureDefinitionActionAffinityBuilder
                    .Create($"ActionAffinity{NAME}")
                    .SetGuiPresentationNoContent(true)
                    .SetForbiddenActions(
                        Id.DisengageMain,
                        Id.DisengageBonus,
                        Id.DashMain,
                        Id.DashBonus)
                    .AddToDB())
            .AddToDB();

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.Wrack, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolNecromancy)
            .SetSpellLevel(0)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Debuff)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(InflictWounds)
                    .SetDurationData(DurationType.Round, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 12, TargetType.IndividualsUnique)
                    .SetEffectAdvancement(EffectIncrementMethod.CasterLevelTable, additionalDicePerIncrement: 1)
                    .SetSavingThrowData(
                        false,
                        AttributeDefinitions.Constitution,
                        false,
                        EffectDifficultyClassComputation.SpellCastingFeature,
                        AttributeDefinitions.Wisdom,
                        12)
                    .AddImmuneCreatureFamilies(Construct, Undead)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeNecrotic, 1, DieType.D6)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionWrack, ConditionForm.ConditionOperation.Add)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build())
                    .Build())
            .AddToDB();

        return spell;
    }

    #endregion

    #region Booming Blade

    internal static SpellDefinition BuildBoomingBlade()
    {
        var powerBoomingBladeDamage = FeatureDefinitionPowerBuilder
            .Create("PowerBoomingBladeDamage")
            .SetGuiPresentationNoContent(true)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDiceAdvancement(LevelSourceType.CharacterLevel, 1, 20, (1, 1), (5, 2), (11, 3), (17, 4))
                            .SetDamageForm(DamageTypeThunder, 0, DieType.D8)
                            .Build())
                    .SetImpactEffectParameters(Shatter)
                    .Build())
            .AddToDB();

        var conditionBoomingBladeSheathed = ConditionDefinitionBuilder
            .Create(ConditionShine, "ConditionBoomingBladeSheathed")
            .SetOrUpdateGuiPresentation(Category.Condition)
            .SetSpecialDuration(DurationType.Round, 0, (TurnOccurenceType)ExtraTurnOccurenceType.StartOfSourceTurn)
            .AddToDB();

        conditionBoomingBladeSheathed.possessive = false;
        conditionBoomingBladeSheathed.AddCustomSubFeatures(new ActionFinishedByMeConditionBoomingBladeSheathed(
            conditionBoomingBladeSheathed, powerBoomingBladeDamage));

        var additionalDamageBoomingBlade = FeatureDefinitionAdditionalDamageBuilder
            .Create("AdditionalDamageBoomingBlade")
            .SetGuiPresentationNoContent(true)
            .SetNotificationTag("BoomingBlade")
            .SetRequiredProperty(RestrictedContextRequiredProperty.MeleeWeapon)
            .SetAttackModeOnly()
            .SetDamageDice(DieType.D8, 0)
            .SetSpecificDamageType(DamageTypeThunder)
            .SetAdvancement(
                ExtraAdditionalDamageAdvancement.CharacterLevel,
                DiceByRankBuilder.InterpolateDiceByRankTable(0, 20, (5, 1), (11, 2), (17, 3)))
            .AddConditionOperation(ConditionOperationDescription.ConditionOperation.Add, conditionBoomingBladeSheathed)
            .SetImpactParticleReference(Shatter)
            .AddToDB();

        var conditionBoomingBlade = ConditionDefinitionBuilder
            .Create("ConditionBoomingBlade")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(additionalDamageBoomingBlade)
            .SetSpecialInterruptions(ConditionInterruption.Attacks)
            .AddToDB();

        var spell = SpellDefinitionBuilder
            .Create("BoomingBlade")
            .SetGuiPresentation(Category.Spell, DivineBlade)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetSpellLevel(0)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.Specific)
            .SetSpecificMaterialComponent(TagsDefinitions.WeaponTagMelee, 0, false)
            .SetVerboseComponent(false)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 0, TurnOccurenceType.EndOfSourceTurn)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetIgnoreCover()
                    .SetEffectAdvancement(EffectIncrementMethod.CasterLevelTable, additionalDicePerIncrement: 1)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(
                            conditionBoomingBlade,
                            ConditionForm.ConditionOperation.Add, true))
                    .SetParticleEffectParameters(Shatter)
                    .Build())
            .AddCustomSubFeatures(SrdAndHouseRulesContext.NoTwinned.Mark, new AttackAfterMagicEffect())
            .AddToDB();

        // need to use same spell reference so power texts update properly on AllowBladeCantripsToUseReach setting
        powerBoomingBladeDamage.GuiPresentation = spell.GuiPresentation;

        return spell;
    }

    private sealed class ActionFinishedByMeConditionBoomingBladeSheathed(
        ConditionDefinition conditionBoomingBladeSheathed,
        FeatureDefinitionPower powerBoomingBladeDamage) : IActionFinishedByMe
    {
        public IEnumerator OnActionFinishedByMe(CharacterAction action)
        {
            if (action.ActionId != Id.TacticalMove)
            {
                yield break;
            }

            var defender = action.ActingCharacter;
            var rulesetDefender = defender.RulesetCharacter;

            if (!rulesetDefender.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, conditionBoomingBladeSheathed.Name, out var activeCondition))
            {
                yield break;
            }

            rulesetDefender.RemoveCondition(activeCondition);

            var rulesetAttacker = EffectHelpers.GetCharacterByGuid(activeCondition.SourceGuid);
            var attacker = GameLocationCharacter.GetFromActor(rulesetAttacker);
            var usablePower = PowerProvider.Get(powerBoomingBladeDamage, rulesetAttacker);

            attacker.MyExecuteActionSpendPower(usablePower, false, defender);
        }
    }

    #endregion

    #region Create Bonfire

    private static readonly ConditionDefinition ConditionCreateBonfireMark = ConditionDefinitionBuilder
        .Create("ConditionCreateBonfireMark")
        .SetGuiPresentationNoContent(true)
        .SetSilent(Silent.WhenAddedOrRemoved)
        .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
        .AddToDB();

    private static readonly FeatureDefinitionPower PowerCreateBonfireDamage = FeatureDefinitionPowerBuilder
        .Create("PowerCreateBonfireDamage")
        .SetGuiPresentation(Category.Feature, hidden: true)
        .SetUsesFixed(ActivationTime.NoCost)
        .SetShowCasting(false)
        .SetEffectDescription(
            EffectDescriptionBuilder
                .Create()
                .SetDurationData(DurationType.Round)
                .SetTargetingData(Side.All, RangeType.Distance, 6, TargetType.IndividualsUnique)
                .SetEffectAdvancement(EffectIncrementMethod.CasterLevelTable, additionalDicePerIncrement: 1)
                .SetSavingThrowData(false, AttributeDefinitions.Dexterity, false,
                    EffectDifficultyClassComputation.SpellCastingFeature)
                .SetEffectForms(
                    EffectFormBuilder
                        .Create()
                        .HasSavingThrow(EffectSavingThrowType.Negates)
                        .SetDamageForm(DamageTypeFire, 1, DieType.D8)
                        .Build())
                .SetImpactEffectParameters(FireBolt)
                .Build())
        .AddCustomSubFeatures(new CustomBehaviorCreateBonfireDamage())
        .AddToDB();

    private static readonly EffectProxyDefinition EffectProxyCreateBonfire = EffectProxyDefinitionBuilder
        .Create(EffectProxyDefinitions.ProxyDancingLights, "ProxyCreateBonfire")
        .SetOrUpdateGuiPresentation(Category.Proxy)
        .SetCanMove(false, false)
        .SetAdditionalFeatures()
        .AddToDB();

    internal static SpellDefinition BuildCreateBonfire()
    {
        const string NAME = "CreateBonfire";

        EffectProxyCreateBonfire.actionId = Id.NoAction;
        EffectProxyCreateBonfire.addLightSource = false;
        EffectProxyCreateBonfire.GuiPresentation.description = Gui.NoLocalization;

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.CreateBonfire, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolConjuration)
            .SetSpellLevel(0)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetRequiresConcentration(true)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.All, RangeType.Distance, 6, TargetType.Cube)
                    .SetEffectAdvancement(EffectIncrementMethod.CasterLevelTable, additionalDicePerIncrement: 1)
                    .SetSavingThrowData(false, AttributeDefinitions.Dexterity, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .RollSaveOnlyIfRelevantForms()
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetSummonEffectProxyForm(EffectProxyCreateBonfire)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetTopologyForm(TopologyForm.Type.DangerousZone, true)
                            .Build())
                    .SetCasterEffectParameters(ProduceFlameHold)
                    .Build())
            .AddCustomSubFeatures(new PowerOrSpellFinishedByMeCreateBonfire())
            .AddToDB();

        return spell;
    }

    // increase damage die and mark enemies damaged this turn
    private sealed class CustomBehaviorCreateBonfireDamage : IModifyEffectDescription, IPowerOrSpellFinishedByMe
    {
        public bool IsValid(BaseDefinition definition, RulesetCharacter character, EffectDescription effectDescription)
        {
            return definition == PowerCreateBonfireDamage;
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            var diceNumber = character.TryGetAttributeValue(AttributeDefinitions.CharacterLevel) switch
            {
                >= 17 => 3,
                >= 11 => 2,
                >= 5 => 1,
                _ => 0
            };

            var damageForm = effectDescription.EffectForms[0].DamageForm;

            damageForm.DiceNumber = diceNumber;

            return effectDescription;
        }

        public IEnumerator OnPowerOrSpellFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            if (action.SaveOutcome is RollOutcome.Success or RollOutcome.CriticalSuccess)
            {
                yield break;
            }

            var rulesetCaster = action.ActingCharacter.RulesetCharacter;
            var rulesetTarget = action.ActionParams.TargetCharacters[0].RulesetCharacter;

            rulesetCaster.InflictCondition(
                ConditionCreateBonfireMark.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagStatus,
                rulesetTarget.Guid,
                rulesetTarget.CurrentFaction.Name,
                1,
                ConditionCreateBonfireMark.Name,
                0,
                0,
                0);
        }
    }

    // issue power damage if there is a target on cube position on spell end
    private sealed class PowerOrSpellFinishedByMeCreateBonfire : IPowerOrSpellFinishedByMe
    {
        public IEnumerator OnPowerOrSpellFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            var locationCharacterService = ServiceRepository.GetService<IGameLocationCharacterService>();
            var contenders =
                (Gui.Battle?.AllContenders ??
                 locationCharacterService.PartyCharacters.Union(locationCharacterService.GuestCharacters))
                .ToList();

            var target = contenders.FirstOrDefault(x =>
                x.LocationPosition == action.ActionParams.Positions[0]);

            if (target != null)
            {
                HandleCreateBonfireBehavior(target);
            }

            yield break;
        }
    }

    // handle on turn start, on turn end, shove, move behaviors
    internal static void HandleCreateBonfireBehavior(GameLocationCharacter character, bool checkCondition = true)
    {
        var battleManager = ServiceRepository.GetService<IGameLocationBattleService>() as GameLocationBattleManager;

        if (!battleManager ||
            (character.RulesetCharacter is RulesetCharacterEffectProxy proxy &&
             proxy.EffectProxyDefinition == EffectProxyCreateBonfire))
        {
            return;
        }

        var locationCharacterService = ServiceRepository.GetService<IGameLocationCharacterService>();
        var bonfireProxy = locationCharacterService.AllProxyCharacters
            .FirstOrDefault(u =>
                character.LocationPosition == u.LocationPosition &&
                u.RulesetCharacter is RulesetCharacterEffectProxy rulesetCharacterEffectProxy &&
                rulesetCharacterEffectProxy.EffectProxyDefinition == EffectProxyCreateBonfire);

        if (bonfireProxy == null)
        {
            return;
        }

        var rulesetProxy = bonfireProxy.RulesetCharacter as RulesetCharacterEffectProxy;
        var rulesetSource = EffectHelpers.GetCharacterByGuid(rulesetProxy!.ControllerGuid);

        if (checkCondition &&
            character.RulesetCharacter.TryGetConditionOfCategoryAndType(
                AttributeDefinitions.TagEffect, ConditionCreateBonfireMark.Name, out var activeCondition) &&
            activeCondition.SourceGuid == rulesetSource.Guid)
        {
            return;
        }

        var source = GameLocationCharacter.GetFromActor(rulesetSource);
        var usablePower = PowerProvider.Get(PowerCreateBonfireDamage, rulesetSource);

        source.MyExecuteActionSpendPower(usablePower, false, character);
    }

    #endregion

    #region Resonating Strike

    internal static SpellDefinition BuildResonatingStrike()
    {
        var powerResonatingStrikeDamage = FeatureDefinitionPowerBuilder
            .Create("PowerResonatingStrike")
            .SetGuiPresentationNoContent(true)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetEffectForms(EffectFormBuilder.DamageForm(DamageTypeFire, 0, DieType.D8))
                    .SetImpactEffectParameters(BurningHands_B)
                    .Build())
            .AddToDB();

        var additionalDamageResonatingStrike = FeatureDefinitionAdditionalDamageBuilder
            .Create("AdditionalDamageResonatingStrike")
            .SetGuiPresentationNoContent(true)
            .SetNotificationTag("ResonatingStrike")
            .SetRequiredProperty(RestrictedContextRequiredProperty.MeleeWeapon)
            .SetDamageDice(DieType.D8, 0)
            .SetSpecificDamageType(DamageTypeFire)
            .SetAdvancement(
                ExtraAdditionalDamageAdvancement.CharacterLevel,
                DiceByRankBuilder.InterpolateDiceByRankTable(0, 20, (5, 1), (11, 2), (17, 3)))
            .SetImpactParticleReference(BurningHands_B)
            .SetAttackModeOnly()
            .AddToDB();

        var conditionResonatingStrike = ConditionDefinitionBuilder
            .Create("ConditionResonatingStrike")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(additionalDamageResonatingStrike)
            .SetSpecialInterruptions(ExtraConditionInterruption.SpendPowerExecuted)
            .AddToDB();

        conditionResonatingStrike.AddCustomSubFeatures(
            new CustomBehaviorConditionResonatingStrike(conditionResonatingStrike, powerResonatingStrikeDamage));

        var spell = SpellDefinitionBuilder
            .Create("ResonatingStrike")
            .SetGuiPresentation(Category.Spell,
                Sprites.GetSprite("ResonatingStrike", Resources.BurningBlade, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetSpellLevel(0)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.Specific)
            .SetSpecificMaterialComponent(TagsDefinitions.WeaponTagMelee, 0, false)
            .SetVerboseComponent(false)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.IndividualsUnique, 2)
                    .SetIgnoreCover()
                    .SetEffectAdvancement(EffectIncrementMethod.CasterLevelTable, additionalDicePerIncrement: 1)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(
                            conditionResonatingStrike,
                            ConditionForm.ConditionOperation.Add, true))
                    .SetParticleEffectParameters(BurningHands_B)
                    .SetImpactEffectParameters(new AssetReference())
                    .Build())
            .AddCustomSubFeatures(
                SrdAndHouseRulesContext.NoTwinned.Mark,
                // order matters here as CustomBehaviorResonatingStrike.IFilterTargetingCharacter
                // should trigger before AttackAfterMagicEffect.IFilterTargetingCharacter
                new CustomBehaviorResonatingStrike(),
                new AttackAfterMagicEffect())
            .AddToDB();

        // need to use same spell reference so power texts update properly on AllowBladeCantripsToUseReach setting
        powerResonatingStrikeDamage.GuiPresentation = spell.GuiPresentation;

        return spell;
    }

    private sealed class CustomBehaviorResonatingStrike : IPowerOrSpellFinishedByMe, IFilterTargetingCharacter
    {
        internal static int SpellCastingModifier;
        internal static GameLocationCharacter SecondTarget;

        public bool EnforceFullSelection => false;

        public bool IsValid(CursorLocationSelectTarget __instance, GameLocationCharacter target)
        {
            // handle first target like AttackAfterMagicEffect
            if (__instance.SelectionService.SelectedTargets.Count == 0)
            {
                if (AttackAfterMagicEffect.CanAttack(__instance.ActionParams.ActingCharacter, target))
                {
                    return true;
                }

                var text = Main.Settings.AllowBladeCantripsToUseReach ? "Feedback/&WithinReach" : "Feedback/&Within5Ft";

                __instance.actionModifier.FailureFlags.Add(Gui.Format("Tooltip/&TargetMeleeWeaponError", text));

                return false;
            }

            if (__instance.SelectionService.SelectedTargets[0].IsWithinRange(target, 1))
            {
                return true;
            }

            __instance.actionModifier.FailureFlags.Add("Tooltip/&SecondTargetNotWithinRange");

            return false;
        }

        public IEnumerator OnPowerOrSpellFinishedByMe(CharacterActionMagicEffect action, BaseDefinition spell)
        {
            if (action.ActionParams.activeEffect is not RulesetEffectSpell rulesetEffectSpell)
            {
                yield break;
            }

            var targets = action.ActionParams.TargetCharacters;

            if (targets.Count != 2)
            {
                SecondTarget = null;
            }
            else
            {
                var rulesetCaster = action.ActingCharacter.RulesetCharacter;
                var spellCastingAbility = rulesetEffectSpell.SpellRepertoire.SpellCastingAbility;

                SecondTarget = action.ActionParams.TargetCharacters[1];
                SpellCastingModifier = AttributeDefinitions.ComputeAbilityScoreModifier(
                    rulesetCaster.TryGetAttributeValue(spellCastingAbility));
            }
        }
    }

    private sealed class CustomBehaviorConditionResonatingStrike(
        ConditionDefinition conditionResonatingStrike,
        FeatureDefinitionPower powerResonatingStrikeDamage) : IPhysicalAttackFinishedByMe, IModifyEffectDescription
    {
        public bool IsValid(BaseDefinition definition, RulesetCharacter character, EffectDescription effectDescription)
        {
            return definition == powerResonatingStrikeDamage;
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            var diceNumber = character.TryGetAttributeValue(AttributeDefinitions.CharacterLevel) switch
            {
                >= 17 => 3,
                >= 11 => 2,
                >= 5 => 1,
                _ => 0
            };

            var damageForm = effectDescription.EffectForms[0].DamageForm;

            damageForm.DiceNumber = diceNumber;
            damageForm.BonusDamage = CustomBehaviorResonatingStrike.SpellCastingModifier;

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
            var rulesetAttacker = attacker.RulesetCharacter;
            var secondDefender = CustomBehaviorResonatingStrike.SecondTarget;

            if (secondDefender == null ||
                rollOutcome is not (RollOutcome.Success or RollOutcome.CriticalSuccess))
            {
                if (rulesetAttacker.TryGetConditionOfCategoryAndType(
                        AttributeDefinitions.TagEffect, conditionResonatingStrike.Name, out var activeCondition))
                {
                    rulesetAttacker.RemoveCondition(activeCondition);
                }

                yield break;
            }

            var usablePower = PowerProvider.Get(powerResonatingStrikeDamage, rulesetAttacker);

            attacker.MyExecuteActionSpendPower(usablePower, false, secondDefender);
        }
    }

    #endregion

    #region Toll the Dead

    internal static SpellDefinition BuildTollTheDead()
    {
        const string NAME = "TollTheDead";

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Bane.GuiPresentation.SpriteReference)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolNecromancy)
            .SetSpellLevel(0)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 12, TargetType.IndividualsUnique)
                    .SetEffectAdvancement(EffectIncrementMethod.CasterLevelTable, additionalDicePerIncrement: 1)
                    .SetSavingThrowData(false, AttributeDefinitions.Wisdom, true,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetParticleEffectParameters(CircleOfDeath)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeNecrotic, 1, DieType.D6)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build())
                    .Build())
            .AddToDB();

        spell.AddCustomSubFeatures(new MagicEffectBeforeHitConfirmedOnEnemyTollTheDead(spell));

        return spell;
    }

    // ReSharper disable once SuggestBaseTypeForParameterInConstructor
    private sealed class MagicEffectBeforeHitConfirmedOnEnemyTollTheDead(SpellDefinition spellTollTheDead)
        : IMagicEffectBeforeHitConfirmedOnEnemy
    {
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
            if (rulesetEffect == null || rulesetEffect.SourceDefinition != spellTollTheDead)
            {
                yield break;
            }

            var rulesetDefender = defender.RulesetCharacter;

            actualEffectForms[0].DamageForm.dieType = rulesetDefender.MissingHitPoints == 0 ? DieType.D8 : DieType.D12;
        }
    }

    #endregion
}
