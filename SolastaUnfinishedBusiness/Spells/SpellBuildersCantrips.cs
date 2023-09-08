using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static RuleDefinitions;
using static FeatureDefinitionAttributeModifier;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using Resources = SolastaUnfinishedBusiness.Properties.Resources;

namespace SolastaUnfinishedBusiness.Spells;

internal static partial class SpellBuilders
{
    #region Air Blast

    internal static SpellDefinition BuildAirBlast()
    {
        const string NAME = "AirBlast";

        var spriteReference = Sprites.GetSprite(NAME, Resources.AirBlast, 128, 128);

        var effectDescription = EffectDescriptionBuilder
            .Create()
            .SetEffectAdvancement(EffectIncrementMethod.CasterLevelTable, 1, 0, 1)
            .SetSavingThrowData(
                false,
                AttributeDefinitions.Strength,
                false,
                EffectDifficultyClassComputation.SpellCastingFeature,
                AttributeDefinitions.Wisdom,
                12)
            .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.IndividualsUnique)
            .SetEffectForms(
                EffectFormBuilder
                    .Create()
                    .SetMotionForm(MotionForm.MotionType.PushFromOrigin, 1)
                    .HasSavingThrow(EffectSavingThrowType.Negates)
                    .Build(),
                EffectFormBuilder
                    .Create()
                    .SetDamageForm(DamageTypeBludgeoning, 1, DieType.D6)
                    .HasSavingThrow(EffectSavingThrowType.Negates)
                    .Build())
            .SetParticleEffectParameters(WindWall)
            .Build();

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, spriteReference)
            .SetEffectDescription(effectDescription)
            .SetCastingTime(ActivationTime.Action)
            .SetSpellLevel(0)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .AddToDB();

        return spell;
    }

    #endregion

    #region Blade Ward

    internal static SpellDefinition BuildBladeWard()
    {
        const string NAME = "BladeWard";

        var spriteReference = Sprites.GetSprite(NAME, Resources.BladeWard, 128, 128);

        var effectDescription = EffectDescriptionBuilder
            .Create()
            .SetDurationData(DurationType.Round, 1)
            .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
            .SetParticleEffectParameters(FeatureDefinitionPowers.PowerPatronHiveReactiveCarapace)
            .SetEffectForms(
                EffectFormBuilder
                    .Create()
                    .SetConditionForm(
                        ConditionDefinitionBuilder
                            .Create("ConditionBladeWard")
                            .SetGuiPresentation(NAME, Category.Spell, ConditionShielded)
                            .SetFeatures(
                                DamageAffinityBludgeoningResistance,
                                DamageAffinitySlashingResistance,
                                DamageAffinityPiercingResistance)
                            .AddToDB(),
                        ConditionForm.ConditionOperation.Add)
                    .Build())
            .Build();

        effectDescription.EffectParticleParameters.casterParticleReference =
            GuidingBolt.effectDescription.EffectParticleParameters.casterParticleReference;

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, spriteReference)
            .SetEffectDescription(effectDescription)
            .SetCastingTime(ActivationTime.Action)
            .SetSpellLevel(0)
            .SetVocalSpellSameType(VocalSpellSemeType.Defense)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolAbjuration)
            .AddToDB();

        return spell;
    }

    #endregion

    #region Burst of Radiance

    internal static SpellDefinition BuildBurstOfRadiance()
    {
        const string NAME = "BurstOfRadiance";

        var spriteReference = Sprites.GetSprite(NAME, Resources.BurstOfRadiance, 128, 128);

        var effectDescription = EffectDescriptionBuilder
            .Create()
            .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Cube, 3)
            .SetEffectAdvancement(EffectIncrementMethod.CasterLevelTable, 1, 0, 1)
            .SetSavingThrowData(
                false,
                AttributeDefinitions.Constitution,
                false,
                EffectDifficultyClassComputation.SpellCastingFeature,
                AttributeDefinitions.Wisdom,
                12)
            .SetEffectForms(
                EffectFormBuilder
                    .Create()
                    .SetDamageForm(DamageTypeRadiant, 1, DieType.D6)
                    .HasSavingThrow(EffectSavingThrowType.Negates)
                    .Build())
            .SetParticleEffectParameters(SacredFlame)
            .Build();

        effectDescription.EffectParticleParameters.impactParticleReference =
            effectDescription.EffectParticleParameters.effectParticleReference;

        effectDescription.EffectParticleParameters.effectParticleReference = new AssetReference();

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, spriteReference)
            .SetEffectDescription(effectDescription)
            .SetCastingTime(ActivationTime.Action)
            .SetSpellLevel(0)
            .SetSomaticComponent(false)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .AddToDB();

        return spell;
    }

    #endregion

    #region Enduring Sting

    internal static SpellDefinition BuildEnduringSting()
    {
        const string NAME = "EnduringSting";

        var spriteReference = Sprites.GetSprite(NAME, Resources.EnduringSting, 128, 128);

        var effectDescription = EffectDescriptionBuilder
            .Create()
            .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.IndividualsUnique)
            .SetTargetFiltering(TargetFilteringMethod.CharacterOnly)
            .SetEffectAdvancement(EffectIncrementMethod.CasterLevelTable, 5, additionalDicePerIncrement: 1)
            .SetParticleEffectParameters(Bane)
            .SetSavingThrowData(
                false,
                AttributeDefinitions.Constitution,
                true,
                EffectDifficultyClassComputation.SpellCastingFeature,
                AttributeDefinitions.Wisdom,
                12)
            .SetEffectForms(
                EffectFormBuilder
                    .Create()
                    .HasSavingThrow(EffectSavingThrowType.Negates)
                    .SetMotionForm(MotionForm.MotionType.FallProne)
                    .Build(),
                EffectFormBuilder
                    .Create()
                    .HasSavingThrow(EffectSavingThrowType.Negates)
                    .SetDamageForm(DamageTypeNecrotic, 1, DieType.D4)
                    .Build())
            .Build();

        effectDescription.EffectParticleParameters.impactParticleReference =
            VenomousSpike.EffectDescription.EffectParticleParameters.impactParticleReference;

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, spriteReference)
            .SetEffectDescription(effectDescription)
            .SetCastingTime(ActivationTime.Action)
            .SetSpellLevel(0)
            .SetVocalSpellSameType(VocalSpellSemeType.Defense)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolNecromancy)
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
                    .SetParticleEffectParameters(SacredFlame_B.EffectDescription.EffectParticleParameters)
                    .Build())
            .AddToDB();

        return spell;
    }

    #endregion

    #region Mind Spike

    internal static SpellDefinition BuildMindSpike()
    {
        const string NAME = "MindSpike";

        var spriteReference = Sprites.GetSprite(NAME, Resources.MindSpike, 128, 128);

        var conditionMindSpike = ConditionDefinitionBuilder
            .Create(ConditionBaned, $"Condition{NAME}")
            .SetOrUpdateGuiPresentation(Category.Condition)
            .SetSpecialDuration(DurationType.Round, 1)
            .SetFeatures(FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinityConditionBaned)
            .SetSpecialInterruptions(ConditionInterruption.SavingThrow)
            .AddToDB();

        var effectDescription = EffectDescriptionBuilder
            .Create()
            .SetEffectAdvancement(EffectIncrementMethod.CasterLevelTable, 5, additionalDicePerIncrement: 1)
            .SetTargetingData(Side.Enemy, RangeType.Distance, 12, TargetType.IndividualsUnique)
            .SetParticleEffectParameters(ShadowDagger)
            .SetSavingThrowData(
                false,
                AttributeDefinitions.Intelligence,
                false,
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
            .Build();

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, spriteReference)
            .SetEffectDescription(effectDescription)
            .SetCastingTime(ActivationTime.Action)
            .SetSpellLevel(0)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEnchantment)
            .AddToDB();

        return spell;
    }

    #endregion

    #region Minor Life Steal

    internal static SpellDefinition BuildMinorLifesteal()
    {
        var spriteReference = Sprites.GetSprite("MinorLifesteal", Resources.MinorLifesteal, 128);

        return SpellDefinitionBuilder
            .Create("MinorLifesteal")
            .SetGuiPresentation(Category.Spell, spriteReference)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolNecromancy)
            .SetVerboseComponent(false)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetSpellLevel(0)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Hour, 1)
                    .SetTargetingData(Side.Enemy, RangeType.RangeHit, 6, TargetType.IndividualsUnique)
                    .AddImmuneCreatureFamilies(CharacterFamilyDefinitions.Construct, CharacterFamilyDefinitions.Undead)
                    .SetEffectAdvancement(EffectIncrementMethod.CasterLevelTable, 5, additionalDicePerIncrement: 1)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeNecrotic, 1, DieType.D8, 0, HealFromInflictedDamage.Half)
                            .HasSavingThrow(EffectSavingThrowType.None)
                            .Build())
                    .SetParticleEffectParameters(VampiricTouch)
                    .Build())
            .AddToDB();
    }

    #endregion

    #region Sword Storm

    internal static SpellDefinition BuildSwordStorm()
    {
        const string NAME = "SwordStorm";

        var spriteReference = Sprites.GetSprite(NAME, Resources.SwordStorm, 128, 128);

        var effectDescription = EffectDescriptionBuilder
            .Create()
            .SetEffectAdvancement(EffectIncrementMethod.CasterLevelTable, 5, additionalDicePerIncrement: 1)
            .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Cube, 3)
            .SetParticleEffectParameters(ShadowDagger)
            .SetSavingThrowData(
                false,
                AttributeDefinitions.Dexterity,
                false,
                EffectDifficultyClassComputation.SpellCastingFeature)
            .SetEffectForms(
                EffectFormBuilder
                    .Create()
                    .SetDamageForm(DamageTypeForce, 1, DieType.D6)
                    .HasSavingThrow(EffectSavingThrowType.Negates)
                    .Build())
            .Build();

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, spriteReference)
            .SetEffectDescription(effectDescription)
            .SetCastingTime(ActivationTime.Action)
            .SetSpellLevel(0)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEnchantment)
            .AddToDB();

        return spell;
    }

    #endregion

    #region Thorny Vines

    internal static SpellDefinition BuildThornyVines()
    {
        var spriteReference = Sprites.GetSprite("ThornyVines", Resources.ThornyVines, 128);

        return SpellDefinitionBuilder
            .Create("ThornyVines")
            .SetGuiPresentation(Category.Spell, spriteReference)
            .SetSpellLevel(0)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetCastingTime(ActivationTime.Action)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.MeleeHit, 6, TargetType.IndividualsUnique)
                    .SetParticleEffectParameters(VenomousSpike)
                    .SetEffectAdvancement(EffectIncrementMethod.CasterLevelTable, 5,
                        additionalDicePerIncrement: 1)
                    .SetEffectForms(
                        EffectFormBuilder.DamageForm(DamageTypePiercing, 1, DieType.D6),
                        EffectFormBuilder.MotionForm(MotionForm.MotionType.DragToOrigin, 2)
                    ).Build())
            .AddToDB();
    }

    #endregion

    #region Thunder Strike

    internal static SpellDefinition BuildThunderStrike()
    {
        const string NAME = "ThunderStrike";

        var spriteReference = Shield;

        var effectDescription = EffectDescriptionBuilder
            .Create()
            .SetEffectAdvancement(
                EffectIncrementMethod.CasterLevelTable, 1, 0, 1)
            .SetSavingThrowData(
                false,
                AttributeDefinitions.Constitution,
                false,
                EffectDifficultyClassComputation.SpellCastingFeature)
            .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Cube, 3)
            .ExcludeCaster()
            .SetEffectForms(
                EffectFormBuilder
                    .Create()
                    .SetDamageForm(DamageTypeThunder, 1, DieType.D6)
                    .HasSavingThrow(EffectSavingThrowType.Negates)
                    .Build())
            .SetParticleEffectParameters(Shatter)
            .Build();

        effectDescription.EffectParticleParameters.impactParticleReference =
            effectDescription.EffectParticleParameters.zoneParticleReference;
        effectDescription.EffectParticleParameters.zoneParticleReference = new AssetReference();

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, spriteReference)
            .SetEffectDescription(effectDescription)
            .SetCastingTime(ActivationTime.Action)
            .SetSpellLevel(0)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .AddToDB();

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
            .AddFeatures(
                FeatureDefinitionActionAffinityBuilder
                    .Create($"ActionAffinity{NAME}")
                    .SetGuiPresentationNoContent(true)
                    .SetForbiddenActions(
                        ActionDefinitions.Id.DisengageMain,
                        ActionDefinitions.Id.DisengageBonus,
                        ActionDefinitions.Id.DashMain,
                        ActionDefinitions.Id.DashBonus)
                    .AddToDB())
            .AddToDB();

        var effectDescription = EffectDescriptionBuilder
            .Create(InflictWounds)
            .SetEffectAdvancement(
                EffectIncrementMethod.CasterLevelTable, 1, 0, 1)
            .SetDurationData(DurationType.Round, 1)
            .SetSavingThrowData(
                false,
                AttributeDefinitions.Constitution,
                false,
                EffectDifficultyClassComputation.SpellCastingFeature,
                AttributeDefinitions.Wisdom,
                12)
            .SetTargetingData(Side.Enemy, RangeType.Distance, 12, TargetType.IndividualsUnique)
            .AddImmuneCreatureFamilies(CharacterFamilyDefinitions.Construct, CharacterFamilyDefinitions.Undead)
            .SetEffectForms(
                EffectFormBuilder
                    .Create()
                    .SetConditionForm(conditionWrack, ConditionForm.ConditionOperation.Add)
                    .HasSavingThrow(EffectSavingThrowType.Negates)
                    .Build(),
                EffectFormBuilder
                    .Create()
                    .SetDamageForm(DamageTypeNecrotic, 1, DieType.D8)
                    .HasSavingThrow(EffectSavingThrowType.Negates)
                    .Build())
            .Build();

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite("Wrack", Resources.Wrack, 128))
            .SetEffectDescription(effectDescription)
            .SetCastingTime(ActivationTime.Action)
            .SetSpellLevel(0)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolNecromancy)
            .AddToDB();

        return spell;
    }

    #endregion

    #region Sunlit Blade

    internal static SpellDefinition BuildSunlightBlade()
    {
        var conditionMarked = ConditionDefinitionBuilder
            .Create("ConditionSunlightBladeMarked")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetSpecialDuration(DurationType.Round, 1)
            .SetSpecialInterruptions(ExtraConditionInterruption.AfterWasAttacked)
            .AddSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
            .AddToDB();

        var conditionSunlightBlade = ConditionDefinitionBuilder
            .Create("ConditionSunlightBlade")
            .SetGuiPresentation(Category.Condition)
            .SetSpecialInterruptions(ConditionInterruption.Attacks, ConditionInterruption.AnyBattleTurnEnd)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(
                FeatureDefinitionAdditionalDamageBuilder
                    .Create("AdditionalDamageSunlightBlade")
                    .SetGuiPresentationNoContent(true)
                    .SetNotificationTag("SunlightBlade")
                    .SetRequiredProperty(RestrictedContextRequiredProperty.MeleeWeapon)
                    .SetAttackModeOnly()
                    .SetDamageDice(DieType.D8, 1)
                    .SetSpecificDamageType(DamageTypeRadiant)
                    .SetAdvancement(ExtraAdditionalDamageAdvancement.CharacterLevel,
                        DiceByRankBuilder.InterpolateDiceByRankTable(0, 20, (5, 1), (11, 2), (17, 3)))
                    .SetTargetCondition(conditionMarked,
                        AdditionalDamageTriggerCondition.TargetHasCondition)
                    .SetConditionOperations(new ConditionOperationDescription
                    {
                        hasSavingThrow = false,
                        operation = ConditionOperationDescription.ConditionOperation.Add,
                        conditionDefinition = ConditionDefinitionBuilder
                            .Create(ConditionHighlighted, "ConditionSunlightBladeHighlighted")
                            .SetSpecialInterruptions(ConditionInterruption.Attacked)
                            .SetSpecialDuration(DurationType.Round, 1,
                                TurnOccurenceType.StartOfTurn)
                            .AddToDB()
                    })
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
            .AddToDB();

        var spell = SpellDefinitionBuilder
            .Create("SunlightBlade")
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite("SunlightBlade", Resources.SunlightBlade, 128, 128))
            .SetSpellLevel(0)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetVerboseComponent(false)
            .SetMaterialComponent(MaterialComponentType.Specific)
            .SetSpecificMaterialComponent(TagsDefinitions.WeaponTagMelee, 0, false)
            .SetCastingTime(ActivationTime.Action)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Touch, 1, TargetType.IndividualsUnique)
                    .SetIgnoreCover()
                    .SetEffectAdvancement( // this is needed for tooltip
                        EffectIncrementMethod.CasterLevelTable, additionalDicePerIncrement: 1, incrementMultiplier: 1)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(
                            conditionSunlightBlade, ConditionForm.ConditionOperation.Add, true),
                        EffectFormBuilder.ConditionForm(conditionMarked))
                    .SetParticleEffectParameters(DivineFavor)
                    .Build())
            .AddToDB();

        spell.SetCustomSubFeatures(
            AttackAfterMagicEffect.SunlitBladeAttack,
            new UpgradeRangeBasedOnWeaponReach(spell));

        return spell;
    }

    #endregion

    #region Acid Claws

    internal static ConditionDefinition AcidClawCondition => _acidClawCondition ??= BuildAcidClawCondition();

    private static ConditionDefinition _acidClawCondition;

    private static ConditionDefinition BuildAcidClawCondition()
    {
        return ConditionDefinitionBuilder
            .Create("ConditionAcidClaws")
            .SetGuiPresentation(Category.Condition, ConditionAcidSpit)
            .SetConditionType(ConditionType.Detrimental)
            .SetSpecialDuration(DurationType.Round, 1)
            .SetFeatures(
                FeatureDefinitionAttributeModifierBuilder
                    .Create("AttributeModifierAcidClawsACDebuff")
                    .SetGuiPresentation("ConditionAcidClaws", Category.Condition)
                    .SetModifier(AttributeModifierOperation.Additive,
                        AttributeDefinitions.ArmorClass, -1)
                    .AddToDB())
            .AddToDB();
    }

    internal static SpellDefinition BuildAcidClaw()
    {
        const string NAME = "AcidClaws";

        var spriteReference = Sprites.GetSprite(NAME, Resources.AcidClaws, 128, 128);

        var effectDescription = EffectDescriptionBuilder
            .Create()
            .SetEffectAdvancement(EffectIncrementMethod.CasterLevelTable, 5, additionalDicePerIncrement: 1)
            .SetTargetingData(Side.Enemy, RangeType.MeleeHit, 1, TargetType.IndividualsUnique)
            .SetEffectForms(
                EffectFormBuilder
                    .Create()
                    .SetDamageForm(DamageTypeAcid, 1, DieType.D8)
                    .Build(),
                EffectFormBuilder
                    .Create()
                    .SetConditionForm(AcidClawCondition, ConditionForm.ConditionOperation.Add)
                    .Build())
            .SetParticleEffectParameters(AcidSplash)
            .Build();

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, spriteReference)
            .SetEffectDescription(effectDescription)
            .SetCastingTime(ActivationTime.Action)
            .SetSpellLevel(0)
            .SetVerboseComponent(false)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .AddToDB();

        return spell;
    }

    #endregion

    #region Booming Blade

    internal static SpellDefinition BuildBoomingBlade()
    {
        var conditionMarked = ConditionDefinitionBuilder
            .Create("ConditionBoomingBladeMarked")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetSpecialDuration(DurationType.Round, 1)
            .SetSpecialInterruptions(ExtraConditionInterruption.AfterWasAttacked)
            .AddSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
            .AddToDB();

        var featureSheathed = FeatureDefinitionBuilder
            .Create("FeatureBoomingBladeSheathed")
            .SetGuiPresentationNoContent(true)
            .SetCustomSubFeatures(new ActionFinishedByMeBoomingBladeSheathed())
            .AddToDB();

        var conditionBoomingBladeSheathed = ConditionDefinitionBuilder
            .Create(ConditionHighlighted, "ConditionBoomingBladeSheathed")
            .SetOrUpdateGuiPresentation(Category.Condition)
            .SetSpecialDuration(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
            .SetFeatures(featureSheathed)
            .AddToDB();

        var conditionBoomingBlade = ConditionDefinitionBuilder
            .Create("ConditionBoomingBlade")
            .SetGuiPresentation(Category.Condition)
            .SetSpecialInterruptions(ConditionInterruption.Attacks, ConditionInterruption.AnyBattleTurnEnd)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(
                FeatureDefinitionAdditionalDamageBuilder
                    .Create("AdditionalDamageBoomingBlade")
                    .SetGuiPresentationNoContent(true)
                    .SetNotificationTag("BoomingBlade")
                    .SetRequiredProperty(RestrictedContextRequiredProperty.MeleeWeapon)
                    .SetAttackModeOnly()
                    .SetDamageDice(DieType.D8, 1)
                    .SetSpecificDamageType(DamageTypeThunder)
                    .SetAdvancement(ExtraAdditionalDamageAdvancement.CharacterLevel,
                        DiceByRankBuilder.InterpolateDiceByRankTable(0, 20, (5, 1), (11, 2), (17, 3)))
                    .SetConditionOperations(new ConditionOperationDescription
                    {
                        hasSavingThrow = false,
                        operation = ConditionOperationDescription.ConditionOperation.Add,
                        conditionDefinition = conditionBoomingBladeSheathed
                    })
                    .SetTargetCondition(conditionMarked, AdditionalDamageTriggerCondition.TargetHasCondition)
                    .SetImpactParticleReference(Shatter)
                    .AddToDB())
            .AddToDB();

        var spell = SpellDefinitionBuilder
            .Create("BoomingBlade")
            .SetGuiPresentation(Category.Spell, DivineBlade)
            .SetSpellLevel(0)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetVerboseComponent(false)
            .SetMaterialComponent(MaterialComponentType.Specific)
            .SetSpecificMaterialComponent(TagsDefinitions.WeaponTagMelee, 0, false)
            .SetCastingTime(ActivationTime.Action)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Touch, 1, TargetType.IndividualsUnique)
                    .SetIgnoreCover()
                    .SetEffectAdvancement( // this is needed for tooltip
                        EffectIncrementMethod.CasterLevelTable, additionalDicePerIncrement: 1, incrementMultiplier: 1)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(
                            conditionBoomingBlade, ConditionForm.ConditionOperation.Add, true),
                        EffectFormBuilder.ConditionForm(conditionMarked))
                    .SetParticleEffectParameters(Shatter)
                    .Build())
            .AddToDB();

        spell.SetCustomSubFeatures(
            AttackAfterMagicEffect.BoomingBladeAttack,
            new UpgradeRangeBasedOnWeaponReach(spell));

        return spell;
    }

    private sealed class ActionFinishedByMeBoomingBladeSheathed : IActionFinishedByMe
    {
        public IEnumerator OnActionFinishedByMe(CharacterAction characterAction)
        {
            if (characterAction is not CharacterActionMove)
            {
                yield break;
            }

            var defender = characterAction.ActingCharacter;
            var rulesetDefender = characterAction.ActingCharacter.RulesetCharacter;

            if (rulesetDefender is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            var usableCondition =
                rulesetDefender.AllConditions.FirstOrDefault(x =>
                    x.ConditionDefinition.Name == "ConditionBoomingBladeSheathed");

            if (usableCondition == null)
            {
                yield break;
            }

            var rulesetAttacker = EffectHelpers.GetCharacterByGuid(usableCondition.SourceGuid);

            if (rulesetAttacker is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            var attacker = GameLocationCharacter.GetFromActor(rulesetAttacker);

            if (attacker == null)
            {
                yield break;
            }

            var dice = new List<int>();
            var characterLevel = rulesetAttacker.TryGetAttributeValue(AttributeDefinitions.CharacterLevel);
            var diceNumber = characterLevel switch
            {
                >= 17 => 4,
                >= 11 => 3,
                >= 5 => 2,
                _ => 1
            };

            for (var i = 0; i < diceNumber; i++)
            {
                var damageRoll = RollDie(DieType.D8, AdvantageType.None, out _, out _);

                dice.Add(damageRoll);
            }

            var damageForm = new DamageForm
            {
                DamageType = DamageTypeThunder, DieType = DieType.D8, DiceNumber = diceNumber, BonusDamage = 0
            };

            EffectHelpers.StartVisualEffect(attacker, defender, Shatter);
            RulesetActor.InflictDamage(
                dice.Sum(),
                damageForm,
                DamageTypeThunder,
                new RulesetImplementationDefinitions.ApplyFormsParams { targetCharacter = rulesetDefender },
                rulesetDefender,
                false,
                attacker.Guid,
                false,
                new List<string>(),
                new RollInfo(DieType.D8, dice, 0),
                false,
                out _);
            rulesetDefender.RemoveCondition(usableCondition);
        }
    }

    #endregion

    #region Burning Blade (Resonating Strike)

    internal static SpellDefinition BuildResonatingStrike()
    {
        // this is the leap damage to second target
        var powerResonatingStrike = FeatureDefinitionPowerBuilder
            .Create("PowerResonatingStrike")
            .SetGuiPresentationNoContent(true)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetFiltering(TargetFilteringMethod.CharacterOnly)
                    .SetTargetingData(Side.Enemy, RangeType.Touch, 1, TargetType.IndividualsUnique)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetBonusMode(AddBonusMode.AbilityBonus)
                            .SetDamageForm(DamageTypeFire, 0, DieType.D8)
                            .SetDiceAdvancement(LevelSourceType.CharacterLevel, 0, 20, (5, 1), (11, 2), (17, 3))
                            .Build())
                    .SetParticleEffectParameters(FireBolt)
                    .Build())
            .AddToDB();

        powerResonatingStrike.SetCustomSubFeatures(new ModifyEffectDescriptionResonatingStrike(powerResonatingStrike));

        // this is the main damage to first target
        var additionalDamageResonatingStrike = FeatureDefinitionAdditionalDamageBuilder
            .Create("AdditionalDamageResonatingStrike")
            .SetGuiPresentationNoContent(true)
            .SetNotificationTag("ResonatingStrike")
            .SetRequiredProperty(RestrictedContextRequiredProperty.MeleeWeapon)
            .SetDamageDice(DieType.D8, 1)
            .SetSpecificDamageType(DamageTypeFire)
            .SetAdvancement(
                ExtraAdditionalDamageAdvancement.CharacterLevel,
                DiceByRankBuilder.InterpolateDiceByRankTable(0, 20, (5, 1), (11, 2), (17, 3)))
            .SetImpactParticleReference(FireBolt)
            .SetAttackModeOnly()
            .AddToDB();

        return SpellDefinitionBuilder
            .Create("ResonatingStrike")
            .SetGuiPresentation(Category.Spell, FlameBlade)
            .SetSpellLevel(0)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetVerboseComponent(false)
            .SetMaterialComponent(MaterialComponentType.Specific)
            .SetSpecificMaterialComponent(TagsDefinitions.WeaponTagMelee, 0, false)
            .SetCustomSubFeatures(
                AttackAfterMagicEffect.ResonatingStrikeAttack,
                new ChainActionAfterMagicEffectResonatingStrike(powerResonatingStrike))
            .SetCastingTime(ActivationTime.Action)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 5, TargetType.IndividualsUnique, 2)
                    .SetTargetProximityData(true, 1)
                    .SetIgnoreCover()
                    .SetEffectAdvancement(
                        EffectIncrementMethod.CasterLevelTable, additionalDicePerIncrement: 1, incrementMultiplier: 1)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(
                                ConditionDefinitionBuilder
                                    .Create("ConditionResonatingStrike")
                                    .SetGuiPresentation(Category.Condition)
                                    .SetSpecialInterruptions(ConditionInterruption.Attacks,
                                        ConditionInterruption.AnyBattleTurnEnd)
                                    .SetSilent(Silent.WhenAddedOrRemoved)
                                    .SetFeatures(additionalDamageResonatingStrike)
                                    .AddToDB(),
                                ConditionForm.ConditionOperation.Add,
                                true)
                            .Build())
                    .SetParticleEffectParameters(FireBolt)
                    .Build())
            .AddToDB();
    }

    // remove effect forms from resonating strike leap damage if not a hit
    private sealed class ModifyEffectDescriptionResonatingStrike : IModifyEffectDescription
    {
        private readonly BaseDefinition _baseDefinition;

        public ModifyEffectDescriptionResonatingStrike(BaseDefinition baseDefinition)
        {
            _baseDefinition = baseDefinition;
        }

        public bool IsValid(BaseDefinition definition, RulesetCharacter character, EffectDescription effectDescription)
        {
            return definition == _baseDefinition;
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            if (!Global.LastAttackWasCantripWeaponAttackHit)
            {
                effectDescription.EffectForms.Clear();
            }

            return effectDescription;
        }
    }

    // chain resonating strike leap damage power
    private sealed class ChainActionAfterMagicEffectResonatingStrike : IChainActionAfterMagicEffect
    {
        private readonly FeatureDefinitionPower _powerResonatingStrike;

        internal ChainActionAfterMagicEffectResonatingStrike(FeatureDefinitionPower powerResonatingStrike)
        {
            _powerResonatingStrike = powerResonatingStrike;
        }

        [CanBeNull]
        public CharacterAction GetNextAction(CharacterActionMagicEffect baseEffect)
        {
            var targets = baseEffect.ActionParams.TargetCharacters;

            if (targets.Count != 2)
            {
                return null;
            }

            var rulesetImplementationService = ServiceRepository.GetService<IRulesetImplementationService>();
            var actionParams = baseEffect.ActionParams.Clone();
            var rulesetCharacter = actionParams.ActingCharacter.RulesetCharacter;
            var usablePower = UsablePowersProvider.Get(_powerResonatingStrike, rulesetCharacter);

            actionParams.ActionDefinition = DatabaseHelper.ActionDefinitions.PowerNoCost;
            actionParams.RulesetEffect = rulesetImplementationService
                .InstantiateEffectPower(rulesetCharacter, usablePower, false)
                .AddAsActivePowerToSource();
            actionParams.TargetCharacters.SetRange(targets[1]);

            return new CharacterActionSpendPower(actionParams);
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
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 12, TargetType.IndividualsUnique)
                    .SetEffectAdvancement(EffectIncrementMethod.CasterLevelTable, additionalDicePerIncrement: 1)
                    .SetSavingThrowData(
                        false,
                        AttributeDefinitions.Wisdom,
                        true,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetParticleEffectParameters(CircleOfDeath.EffectDescription.EffectParticleParameters)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeNecrotic, 1, DieType.D8)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build())
                    .Build())
            .SetCastingTime(ActivationTime.Action)
            .SetSpellLevel(0)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolNecromancy)
            .AddToDB();

        spell.SetCustomSubFeatures(new MagicalAttackBeforeHitConfirmedOnEnemyTollTheDead(spell));

        return spell;
    }

    private sealed class MagicalAttackBeforeHitConfirmedOnEnemyTollTheDead : IMagicalAttackBeforeHitConfirmedOnEnemy
    {
        private readonly SpellDefinition _spellTollTheDead;

        public MagicalAttackBeforeHitConfirmedOnEnemyTollTheDead(SpellDefinition spellTollTheDead)
        {
            _spellTollTheDead = spellTollTheDead;
        }

        public IEnumerator OnMagicalAttackBeforeHitConfirmedOnEnemy(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier magicModifier,
            RulesetEffect rulesetEffect,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            if (rulesetEffect == null || rulesetEffect.SourceDefinition != _spellTollTheDead)
            {
                yield break;
            }

            var rulesetDefender = defender.RulesetCharacter;

            actualEffectForms[0].DamageForm.dieType = rulesetDefender.MissingHitPoints == 0 ? DieType.D8 : DieType.D12;
        }
    }

    #endregion
}
