using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomDefinitions;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.Utils;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.MonsterDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static EffectForm;
using static RuleDefinitions;
using Resources = SolastaUnfinishedBusiness.Properties.Resources;

namespace SolastaUnfinishedBusiness.Models;

internal static class SpellsBuildersContext
{
    #region LEVEL 07

    internal static SpellDefinition BuildReverseGravity()
    {
        const string ReverseGravityName = "ReverseGravity";

        var effectDescription = EffectDescriptionBuilder.Create()
            .SetDurationData(DurationType.Minute, 1)
            .SetTargetingData(Side.All, RangeType.Distance, 12, TargetType.Cylinder, 10, 10)
            .SetSavingThrowData(
                false,
                AttributeDefinitions.Dexterity,
                true,
                EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                AttributeDefinitions.Dexterity,
                20)
            .SetEffectForms(
                EffectFormBuilder
                    .Create()
                    .SetConditionForm(
                        ConditionDefinitionBuilder
                            .Create(ConditionDefinitions.ConditionLevitate, "ConditionReverseGravity")
                            .SetOrUpdateGuiPresentation(Category.Condition)
                            .SetConditionType(ConditionType.Neutral)
                            .SetFeatures(
                                FeatureDefinitionMovementAffinitys.MovementAffinityConditionLevitate,
                                FeatureDefinitionMoveModes.MoveModeFly2
                            )
                            .AddToDB(),
                        ConditionForm.ConditionOperation.Add,
                        false,
                        false)
                    .HasSavingThrow(EffectSavingThrowType.Negates)
                    .Build(),
                EffectFormBuilder
                    .Create()
                    .SetMotionForm(
                        MotionForm.MotionType.Levitate,
                        10)
                    .HasSavingThrow(EffectSavingThrowType.Negates)
                    .Build())
            .SetRecurrentEffect(Entangle.EffectDescription.RecurrentEffect)
            .Build();

        return SpellDefinitionBuilder
            .Create(ReverseGravityName)
            .SetGuiPresentation(Category.Spell, Thunderwave)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(7)
            .SetCastingTime(ActivationTime.Action)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetEffectDescription(effectDescription)
            .SetAiParameters(new SpellAIParameters())
            .SetRequiresConcentration(true)
            .AddToDB();
    }

    #endregion

    #region LEVEL 08

    internal static SpellDefinition BuildMindBlank()
    {
        var effectDescription = EffectDescriptionBuilder
            .Create()
            .SetDurationData(
                DurationType.Hour,
                24)
            .SetTargetingData(
                Side.Ally,
                RangeType.Touch,
                1,
                TargetType.Individuals
            )
            .SetEffectForms(EffectFormBuilder
                .Create()
                .SetConditionForm(
                    ConditionDefinitionBuilder
                        .Create(ConditionBearsEndurance, "ConditionMindBlank")
                        .SetOrUpdateGuiPresentation(Category.Condition)
                        .SetFeatures(
                            FeatureDefinitionConditionAffinitys.ConditionAffinityCharmImmunity,
                            FeatureDefinitionConditionAffinitys.ConditionAffinityCharmImmunityHypnoticPattern,
                            FeatureDefinitionConditionAffinitys.ConditionAffinityCalmEmotionCharmedImmunity,
                            FeatureDefinitionDamageAffinitys.DamageAffinityPsychicImmunity)
                        .AddToDB(),
                    ConditionForm.ConditionOperation.Add,
                    false,
                    false)
                .Build())
            .Build();

        return SpellDefinitionBuilder
            .Create("MindBlank")
            .SetGuiPresentation(Category.Spell, MindTwist)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(8)
            .SetCastingTime(ActivationTime.Action)
            .SetVerboseComponent(true)
            .SetSomaticComponent(false)
            .SetEffectDescription(effectDescription)
            .SetAiParameters(new SpellAIParameters())
            .AddToDB();
    }

    #endregion

    #region CANTRIPS

    private static ConditionDefinition _acidClawCondition;
    internal static ConditionDefinition AcidClawCondition => _acidClawCondition ??= BuildAcidClawCondition();

    private static ConditionDefinition BuildAcidClawCondition()
    {
        return ConditionDefinitionBuilder
            .Create("ConditionAcidClaws")
            .SetGuiPresentation(Category.Condition, ConditionAcidSpit)
            .SetConditionType(ConditionType.Detrimental)
            .SetDuration(DurationType.Round, 1)
            .SetSpecialDuration(true)
            .SetFeatures(FeatureDefinitionAttributeModifierBuilder
                .Create("AttributeModifierAcidClawsACDebuff")
                .SetGuiPresentation(Category.Feature)
                .SetModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
                    AttributeDefinitions.ArmorClass, -1)
                .AddToDB())
            .AddToDB();
    }

    internal static SpellDefinition BuildAcidClaw()
    {
        const string NAME = "AcidClaws";

        var spriteReference = CustomIcons.GetSprite(NAME, Resources.AcidClaws, 128, 128);

        var effectDescription = EffectDescriptionBuilder
            .Create()
            .SetEffectAdvancement(EffectIncrementMethod.CasterLevelTable, 5, additionalDicePerIncrement: 1)
            .SetDurationData(DurationType.Instantaneous)
            .SetTargetingData(Side.Enemy, RangeType.MeleeHit, 1, TargetType.Individuals)
            .SetEffectForms(
                EffectFormBuilder
                    .Create()
                    .SetDamageForm(dieType: DieType.D8, diceNumber: 1, damageType: DamageTypeAcid)
                    .HasSavingThrow(EffectSavingThrowType.None)
                    .Build(),
                EffectFormBuilder
                    .Create()
                    .SetConditionForm(AcidClawCondition, ConditionForm.ConditionOperation.Add)
                    .HasSavingThrow(EffectSavingThrowType.None)
                    .Build())
            .Build();

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, spriteReference)
            .SetEffectDescription(effectDescription)
            .SetCastingTime(ActivationTime.Action)
            .SetSpellLevel(0)
            .SetRequiresConcentration(false)
            .SetVerboseComponent(false)
            .SetSomaticComponent(true)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .AddToDB();

        return spell;
    }

    internal static SpellDefinition BuildAirBlast()
    {
        const string NAME = "AirBlast";

        var spriteReference = CustomIcons.GetSprite(NAME, Resources.AirBlast, 128, 128);

        var effectDescription = EffectDescriptionBuilder
            .Create()
            .SetEffectAdvancement(EffectIncrementMethod.CasterLevelTable, 1, 0, 1)
            .SetSavingThrowData(
                false,
                AttributeDefinitions.Strength,
                false,
                EffectDifficultyClassComputation.SpellCastingFeature,
                AttributeDefinitions.Wisdom,
                15)
            .SetDurationData(DurationType.Instantaneous)
            .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.Individuals)
            .SetEffectForms(
                EffectFormBuilder
                    .Create()
                    .SetMotionForm(MotionForm.MotionType.PushFromOrigin, 1)
                    .HasSavingThrow(EffectSavingThrowType.Negates).Build(),
                EffectFormBuilder
                    .Create()
                    .SetDamageForm(DamageTypeBludgeoning, 1, DieType.D6)
                    .HasSavingThrow(EffectSavingThrowType.Negates)
                    .Build())
            .Build();

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, spriteReference)
            .SetEffectDescription(effectDescription)
            .SetCastingTime(ActivationTime.Action)
            .SetSpellLevel(0)
            .SetRequiresConcentration(false)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .AddToDB();

        return spell;
    }

    internal static SpellDefinition BuildBurstOfRadiance()
    {
        const string NAME = "BurstOfRadiance";

        var spriteReference = CustomIcons.GetSprite(NAME, Resources.BurstOfRadiance, 128, 128);

        var effectDescription = EffectDescriptionBuilder
            .Create()
            .SetEffectAdvancement(EffectIncrementMethod.CasterLevelTable, 1, 0, 1)
            .SetSavingThrowData(
                true,
                AttributeDefinitions.Constitution,
                false,
                EffectDifficultyClassComputation.SpellCastingFeature,
                AttributeDefinitions.Wisdom,
                13)
            .SetDurationData(DurationType.Instantaneous)
            .SetParticleEffectParameters(BurningHands)
            .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Sphere)
            .SetEffectForms(
                EffectFormBuilder
                    .Create()
                    .SetDamageForm(DamageTypeRadiant, 1, DieType.D6)
                    .HasSavingThrow(EffectSavingThrowType.Negates)
                    .Build())
            .Build();

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, spriteReference)
            .SetEffectDescription(effectDescription)
            .SetCastingTime(ActivationTime.Action)
            .SetSpellLevel(0)
            .SetRequiresConcentration(false)
            .SetVerboseComponent(true)
            .SetSomaticComponent(false)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .AddToDB();

        return spell;
    }

    internal static SpellDefinition BuildIlluminatingSphere()
    {
        const string NAME = "IlluminatingSphere";

        var spell = SpellDefinitionBuilder
            .Create(Sparkle, NAME)
            .SetGuiPresentation(Category.Spell, Shine)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create(Sparkle.EffectDescription)
                .SetTargetingData(Side.All, RangeType.Distance, 18, TargetType.Sphere, 5)
                .SetParticleEffectParameters(SacredFlame_B.EffectDescription.EffectParticleParameters)
                .Build())
            .AddToDB();

        return spell;
    }

    internal static SpellDefinition BuildMinorLifesteal()
    {
        var spriteReference = CustomIcons.GetSprite("MinorLifesteal", Resources.MinorLifesteal, 128);

        return SpellDefinitionBuilder
            .Create("MinorLifesteal")
            .SetGuiPresentation(Category.Spell, spriteReference)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolNecromancy)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetSomaticComponent(true)
            .SetVerboseComponent(false)
            .SetSpellLevel(0)
            .SetRequiresConcentration(false)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.Enemy, RangeType.RangeHit, 12, TargetType.Individuals)
                .AddImmuneCreatureFamilies(CharacterFamilyDefinitions.Construct, CharacterFamilyDefinitions.Undead)
                .SetEffectAdvancement(EffectIncrementMethod.CasterLevelTable, 5, additionalDicePerIncrement: 1)
                .SetEffectForms(
                    EffectFormBuilder
                        .Create()
                        .SetDamageForm(DamageTypeNecrotic, 1, DieType.D8, 0, HealFromInflictedDamage.Half)
                        .HasSavingThrow(EffectSavingThrowType.None)
                        .Build(),
                    EffectFormBuilder
                        .Create()
                        .SetTempHpForm(0, DieType.D4, 1, true)
                        .HasSavingThrow(EffectSavingThrowType.None)
                        .Build())
                .SetParticleEffectParameters(VampiricTouch)
                .Build())
            .AddToDB();
    }

    internal static SpellDefinition BuildResonatingStrike()
    {
        var resonanceHighLevel = EffectDescriptionBuilder
            .Create()
            .SetParticleEffectParameters(AcidSplash)
            .SetTargetFiltering(TargetFilteringMethod.CharacterOnly)
            .SetTargetingData(Side.Enemy, RangeType.Touch, 1, TargetType.Individuals)
            .SetEffectForms(
                EffectFormBuilder
                    .Create()
                    .SetBonusMode(AddBonusMode.AbilityBonus)
                    .SetDamageForm(DamageTypeThunder, 1, DieType.D8)
                    .Build())
            .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, 5, additionalDicePerIncrement: 1)
            .Build();

        var resonanceLeap = SpellDefinitionBuilder
            .Create("ResonatingStrikeLeap")
            .SetGuiPresentationNoContent()
            .SetSpellLevel(1)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetSomaticComponent(false)
            .SetVerboseComponent(false)
            .SetCustomSubFeatures(
                new BonusSlotLevelsByClassLevel(),
                new UpgradeEffectFromLevel(resonanceHighLevel, 5)
            )
            .SetCastingTime(ActivationTime.Action)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetParticleEffectParameters(AcidSplash)
                .SetTargetFiltering(TargetFilteringMethod.CharacterOnly)
                .SetTargetingData(Side.Enemy, RangeType.Touch, 1, TargetType.Individuals)
                .SetEffectForms(EffectFormBuilder
                    .Create()
                    .SetBonusMode(AddBonusMode.AbilityBonus)
                    .SetDamageForm(DamageTypeThunder)
                    .Build())
                .Build())
            .AddToDB();

        return SpellDefinitionBuilder
            .Create("ResonatingStrike")
            .SetGuiPresentation(Category.Spell,
                CustomIcons.GetSprite("ResonatingStrike", Resources.ResonatingStrike, 128, 128))
            .SetSpellLevel(0)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetSomaticComponent(true)
            .SetVerboseComponent(false)
            .SetMaterialComponent(MaterialComponentType.Specific)
            .SetSpecificMaterialComponent(TagsDefinitions.WeaponTagMelee, 0, false)
            .SetCustomSubFeatures(
                PerformAttackAfterMagicEffectUse.MeleeAttack,
                CustomSpellEffectLevel.ByCasterLevel,
                new ChainSpellEffectOnAttackHit(resonanceLeap, "ResonatingStrike")
            )
            .SetCastingTime(ActivationTime.Action)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetParticleEffectParameters(ScorchingRay)
                .SetTargetProximityData(true, 1)
                .SetTargetingData(Side.Enemy, RangeType.Distance, 5, TargetType.IndividualsUnique, 2)
                .SetIgnoreCover()
                .SetEffectAdvancement(EffectIncrementMethod.CasterLevelTable, additionalDicePerIncrement: 1,
                    incrementMultiplier: 1)
                .SetDurationData(DurationType.Round, 1)
                .SetEffectForms(EffectFormBuilder
                    .Create()
                    .HasSavingThrow(EffectSavingThrowType.None)
                    .SetConditionForm(ConditionDefinitionBuilder
                            .Create("ConditionResonatingStrike")
                            .SetGuiPresentation(Category.Condition)
                            .SetSpecialInterruptions(ConditionInterruption.Attacks)
                            .SetSilent(Silent.WhenAddedOrRemoved)
                            .SetTurnOccurence(TurnOccurenceType.EndOfTurn)
                            .SetDuration(DurationType.Round, 1)
                            .SetFeatures(FeatureDefinitionAdditionalDamageBuilder
                                .Create("AdditionalDamageResonatingStrike")
                                .SetGuiPresentationNoContent(true)
                                .SetNotificationTag("ResonatingStrike")
                                .SetDamageDice(DieType.D8, 1)
                                .SetRequiredProperty(RestrictedContextRequiredProperty.MeleeWeapon)
                                .SetAttackModeOnly()
                                .SetSpecificDamageType(DamageTypeThunder)
                                .SetAdvancement(
                                    AdditionalDamageAdvancement.ClassLevel,
                                    DiceByRankBuilder.BuildDiceByRankTable(0, step: 5, increment: 1))
                                .SetIgnoreCriticalDoubleDice(true)
                                .AddToDB()
                            )
                            .AddToDB(),
                        ConditionForm.ConditionOperation.Add,
                        true,
                        false)
                    .Build())
                .Build())
            .AddToDB();
    }

    internal static SpellDefinition BuildSunlightBlade()
    {
        var highlight = new ConditionOperationDescription
        {
            hasSavingThrow = false,
            operation = ConditionOperationDescription.ConditionOperation.Add,
            conditionDefinition = ConditionDefinitionBuilder
                .Create(ConditionHighlighted, "ConditionSunlightBladeHighlighted")
                .SetSpecialInterruptions(ConditionInterruption.Attacked)
                .SetDuration(DurationType.Round, 1)
                .SetTurnOccurence(TurnOccurenceType.StartOfTurn)
                .SetSpecialDuration(true)
                .AddToDB()
        };

        var dimLight = new LightSourceForm
        {
            brightRange = 0,
            dimAdditionalRange = 2,
            lightSourceType = LightSourceType.Basic,
            color = new Color(0.9f, 0.8f, 0.4f),
            graphicsPrefabReference = FeatureDefinitionAdditionalDamages
                .AdditionalDamageBrandingSmite.LightSourceForm.graphicsPrefabReference
        };

        var sunlitMark = ConditionDefinitionBuilder
            .Create("ConditionSunlightBladeMarked")
            .SetGuiPresentationNoContent()
            .SetSpecialInterruptions(ExtraConditionInterruption.AfterWasAttacked)
            .AddSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetTurnOccurence(TurnOccurenceType.EndOfTurn)
            .SetDuration(DurationType.Round, 1)
            .AddToDB();

        return SpellDefinitionBuilder
            .Create("SunlightBlade")
            .SetGuiPresentation(Category.Spell,
                CustomIcons.GetSprite("SunlightBlade", Resources.SunlightBlade, 128, 128))
            .SetSpellLevel(0)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetSomaticComponent(true)
            .SetVerboseComponent(false)
            .SetMaterialComponent(MaterialComponentType.Specific)
            .SetSpecificMaterialComponent(TagsDefinitions.WeaponTagMelee, 0, false)
            .SetCustomSubFeatures(
                PerformAttackAfterMagicEffectUse.MeleeAttack,
                new UpgradeRangeBasedOnWeaponReach(),
                CustomSpellEffectLevel.ByCasterLevel)
            .SetCastingTime(ActivationTime.Action)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetParticleEffectParameters(ScorchingRay)
                .SetTargetingData(Side.Enemy, RangeType.Touch, 1, TargetType.IndividualsUnique)
                .SetIgnoreCover()
                .SetEffectAdvancement(
                    EffectIncrementMethod.CasterLevelTable,
                    additionalDicePerIncrement: 1,
                    incrementMultiplier: 1)
                .SetDurationData(DurationType.Round, 1)
                .SetEffectForms(EffectFormBuilder
                        .Create()
                        .HasSavingThrow(EffectSavingThrowType.None)
                        .SetConditionForm(ConditionDefinitionBuilder
                                .Create("ConditionSunlightBlade")
                                .SetGuiPresentation(Category.Condition)
                                .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
                                .SetSilent(Silent.WhenAddedOrRemoved)
                                .SetTurnOccurence(TurnOccurenceType.EndOfTurn)
                                .SetDuration(DurationType.Round, 1)
                                .SetFeatures(FeatureDefinitionAdditionalDamageBuilder
                                    .Create("AdditionalDamageSunlightBlade")
                                    .SetGuiPresentationNoContent(true)
                                    .SetNotificationTag("SunlightBlade")
                                    .SetRequiredProperty(RestrictedContextRequiredProperty.MeleeWeapon)
                                    .SetAttackModeOnly()
                                    .SetDamageDice(DieType.D8, 1)
                                    .SetSpecificDamageType(DamageTypeRadiant)
                                    .SetAdvancement(
                                        AdditionalDamageAdvancement.ClassLevel,
                                        DiceByRankBuilder.BuildDiceByRankTable(0, step: 5, increment: 1))
                                    .SetTargetCondition(sunlitMark,
                                        AdditionalDamageTriggerCondition.TargetHasCondition)
                                    .SetConditionOperations(highlight)
                                    .SetAddLightSource(true)
                                    .SetLightSourceForm(dimLight)
                                    .AddToDB())
                                .AddToDB(),
                            ConditionForm.ConditionOperation.Add,
                            true,
                            false)
                        .Build(),
                    EffectFormBuilder
                        .Create()
                        .HasSavingThrow(EffectSavingThrowType.None)
                        .SetConditionForm(sunlitMark, ConditionForm.ConditionOperation.Add)
                        .Build())
                .Build())
            .AddToDB();
    }

    internal static SpellDefinition BuildThornyVines()
    {
        var spriteReference = CustomIcons.GetSprite("ThornyVines", Resources.ThornyVines, 128);

        return SpellDefinitionBuilder
            .Create("ThornyVines")
            .SetGuiPresentation(Category.Spell, spriteReference)
            .SetSpellLevel(0)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSomaticComponent(true)
            .SetVerboseComponent(true)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetCastingTime(ActivationTime.Action)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.Enemy, RangeType.RangeHit, 6, TargetType.Individuals)
                .SetParticleEffectParameters(VenomousSpike)
                .SetEffectAdvancement(EffectIncrementMethod.CasterLevelTable, 5,
                    additionalDicePerIncrement: 1)
                .SetEffectForms(
                    EffectFormBuilder
                        .Create()
                        .SetDamageForm(diceNumber: 1, dieType: DieType.D6, damageType: DamageTypePiercing)
                        .Build(),
                    EffectFormBuilder
                        .Create()
                        .SetMotionForm(MotionForm.MotionType.DragToOrigin, 2)
                        .Build())
                .Build())
            .AddToDB();
    }

    internal static SpellDefinition BuildThunderStrike()
    {
        const string NAME = "ThunderStrike";

        var spriteReference = Shield;

        var effectDescription = EffectDescriptionBuilder
            .Create()
            .SetEffectAdvancement(
                EffectIncrementMethod.CasterLevelTable, 1, 0, 1)
            .SetSavingThrowData(
                true,
                AttributeDefinitions.Constitution,
                false,
                EffectDifficultyClassComputation.SpellCastingFeature,
                AttributeDefinitions.Wisdom,
                15)
            .SetDurationData(DurationType.Instantaneous)
            .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Sphere)
            .SetEffectForms(
                EffectFormBuilder
                    .Create()
                    .SetDamageForm(DamageTypeThunder, 1, DieType.D6)
                    .HasSavingThrow(EffectSavingThrowType.Negates).Build())
            .Build();

        effectDescription.targetExcludeCaster = true;

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, spriteReference)
            .SetEffectDescription(effectDescription)
            .SetCastingTime(ActivationTime.Action)
            .SetSpellLevel(0)
            .SetRequiresConcentration(false)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .AddToDB();

        return spell;
    }

    #endregion

    #region LEVEL 01

    internal static SpellDefinition BuildFindFamiliar()
    {
        var owlFamiliar = MonsterDefinitionBuilder
            .Create(Eagle_Matriarch, "OwlFamiliar")
            .SetGuiPresentation("OwlFamiliar", Category.Monster, Eagle_Matriarch)
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
                FeatureDefinitionConditionAffinitys.ConditionAffinityProneImmunity,
                CharacterContext.FeatureDefinitionPowerHelpAction)
            .ClearAttackIterations()
            .SetSkillScores(
                (DatabaseHelper.SkillDefinitions.Perception.Name, 3),
                (DatabaseHelper.SkillDefinitions.Stealth.Name, 3))
            .SetArmorClass(11)
            .SetAbilityScores(3, 13, 8, 2, 12, 7)
            .SetHitDiceNumber(1)
            .SetHitDiceType(DieType.D4)
            .SetHitPointsBonus(-1)
            .SetStandardHitPoints(1)
            .SetSizeDefinition(CharacterSizeDefinitions.Tiny)
            .SetAlignment("Neutral")
            .SetCharacterFamily(CharacterFamilyDefinitions.Fey)
            .SetChallengeRating(0)
            .SetDroppedLootDefinition(null)
            .SetDefaultBattleDecisionPackage(DecisionPackageDefinitions.DefaultSupportCasterWithBackupAttacksDecisions)
            .SetFullyControlledWhenAllied(true)
            .SetDefaultFaction(FactionDefinitions.Party)
            .SetBestiaryEntry(BestiaryDefinitions.BestiaryEntry.None)
            .AddToDB();

        var spell = SpellDefinitionBuilder.Create(Fireball, "FindFamiliar")
            .SetGuiPresentation(Category.Spell, AnimalFriendship)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolConjuration)
            .SetMaterialComponent(MaterialComponentType.Specific)
            .SetSomaticComponent(true)
            .SetVerboseComponent(true)
            .SetSpellLevel(1)
            .SetUniqueInstance(true)
            .SetCastingTime(ActivationTime.Minute10)
            .SetRitualCasting(ActivationTime.Minute10)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create(ConjureAnimalsOneBeast.EffectDescription)
                .SetTargetingData(Side.Ally, RangeType.Distance, 2, TargetType.Position)
                .SetDurationData(DurationType.Permanent)
                .SetEffectForms(
                    EffectFormBuilder
                        .Create()
                        .SetSummonCreatureForm(1, owlFamiliar.name)
                        .CreatedByCharacter()
                        .Build())
                .Build())
            .AddToDB();

        GlobalUniqueEffects.AddToGroup(GlobalUniqueEffects.Group.Familiar, spell);

        return spell;
    }

    internal static SpellDefinition BuildMule()
    {
        const string NAME = "Mule";

        var effectDescription = EffectDescriptionBuilder
            .Create()
            .SetTargetingData(Side.Ally, RangeType.Touch, 0, TargetType.Individuals)
            .SetDurationData(DurationType.Hour, 8)
            .SetParticleEffectParameters(ExpeditiousRetreat)
            .SetEffectForms(
                EffectFormBuilder
                    .Create()
                    .SetConditionForm(
                        ConditionDefinitionBuilder
                            .Create("ConditionMule")
                            .SetGuiPresentation(Category.Condition, Longstrider)
                            .SetConditionType(ConditionType.Beneficial)
                            .SetFeatures(
                                FeatureDefinitionMovementAffinityBuilder
                                    .Create("MovementAffinityConditionMule")
                                    .SetGuiPresentationNoContent(true)
                                    .SetImmunities(true, true)
                                    .AddToDB(),
                                FeatureDefinitionEquipmentAffinityBuilder
                                    .Create("EquipmentAffinityConditionMule")
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
            .SetGuiPresentation(Category.Spell, Longstrider)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(1)
            .SetCastingTime(ActivationTime.Action)
            .SetConcentrationAction(ActionDefinitions.ActionParameter.None)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetRequiresConcentration(false)
            .SetSomaticComponent(true)
            .SetVerboseComponent(true)
            .SetEffectDescription(effectDescription)
            .AddToDB();

        return spell;
    }

    internal static SpellDefinition BuildRadiantMotes()
    {
        const string NAME = "RadiantMotes";

        var spell = SpellDefinitionBuilder
            .Create(MagicMissile, NAME)
            .SetGuiPresentation(Category.Spell, Sparkle)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.Enemy, RangeType.Distance, 18, TargetType.Individuals)
                .SetEffectForms(
                    Shine.EffectDescription.EffectForms[0],
                    EffectFormBuilder
                        .Create()
                        .SetDamageForm(diceNumber: 1, dieType: DieType.D1, damageType: DamageTypeRadiant)
                        .Build())
                .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, 1, 2)
                .Build())
            .AddToDB();

        return spell;
    }

    #endregion

    #region LEVEL 02

    internal static SpellDefinition BuildPetalStorm()
    {
        const string ProxyPetalStormName = "ProxyPetalStorm";

        _ = EffectProxyDefinitionBuilder
            .Create(EffectProxyDefinitions.ProxyInsectPlague, ProxyPetalStormName)
            .SetGuiPresentation("PetalStorm", Category.Spell, WindWall)
            .SetCanMove()
            .SetIsEmptyPresentation(false)
            .SetCanMoveOnCharacters()
            .SetAttackMethod(ProxyAttackMethod.ReproduceDamageForms)
            .SetActionId(ActionDefinitions.Id.ProxyFlamingSphere)
            .SetPortrait(WindWall.GuiPresentation.SpriteReference)
            .AddAdditionalFeatures(FeatureDefinitionMoveModes.MoveModeMove6)
            .AddToDB();

        var spell = SpellDefinitionBuilder
            .Create(InsectPlague, "PetalStorm")
            .SetGuiPresentation(Category.Spell, WindWall)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolConjuration)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetSomaticComponent(true)
            .SetVerboseComponent(true)
            .SetSpellLevel(2)
            .SetRequiresConcentration(true)
            .AddToDB();

        //TODO: move this into a builder
        var effectDescription = spell.EffectDescription;

        effectDescription.rangeType = RangeType.Distance;
        effectDescription.rangeParameter = 12;
        effectDescription.durationType = DurationType.Minute;
        effectDescription.durationParameter = 1;
        effectDescription.targetType = TargetType.Cube;
        effectDescription.targetParameter = 3;
        effectDescription.hasSavingThrow = true;
        effectDescription.savingThrowAbility = AttributeDefinitions.Strength;
        effectDescription.recurrentEffect = (RecurrentEffect)20;

        effectDescription.EffectAdvancement.additionalDicePerIncrement = 2;
        effectDescription.EffectAdvancement.incrementMultiplier = 1;
        effectDescription.EffectAdvancement.effectIncrementMethod = EffectIncrementMethod.PerAdditionalSlotLevel;

        effectDescription.EffectForms[0].hasSavingThrow = true;
        effectDescription.EffectForms[0].savingThrowAffinity = EffectSavingThrowType.Negates;
        effectDescription.EffectForms[0].DamageForm.diceNumber = 3;
        effectDescription.EffectForms[0].DamageForm.dieType = DieType.D4;
        effectDescription.EffectForms[0].DamageForm.damageType = DamageTypeSlashing;
        effectDescription.EffectForms[0].levelMultiplier = 1;

        effectDescription.EffectForms[2].SummonForm.effectProxyDefinitionName = ProxyPetalStormName;

        return spell;
    }

    [NotNull]
    internal static SpellDefinition BuildProtectThreshold()
    {
        const string ProxyPetalStormName = "ProxyProtectThreshold";

        EffectProxyDefinitionBuilder
            .Create(EffectProxyDefinitions.ProxySpikeGrowth, ProxyPetalStormName)
            .SetOrUpdateGuiPresentation("ProtectThreshold", Category.Spell)
            .AddToDB();

        var spell = SpellDefinitionBuilder
            .Create(SpikeGrowth, "ProtectThreshold")
            .SetGuiPresentation(Category.Spell, Bane)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolAbjuration)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetSomaticComponent(true)
            .SetVerboseComponent(true)
            .SetSpellLevel(2)
            .SetRequiresConcentration(false)
            .SetRitualCasting(ActivationTime.Minute10)
            .AddToDB();

        //TODO: move this into a builder
        var effectDescription = spell.EffectDescription;

        effectDescription.difficultyClassComputation = EffectDifficultyClassComputation.AbilityScoreAndProficiency;
        effectDescription.durationParameter = 10;
        effectDescription.durationType = DurationType.Minute;
        effectDescription.hasSavingThrow = true;
        effectDescription.rangeParameter = 1;
        effectDescription.rangeType = RangeType.Distance;
        effectDescription.recurrentEffect = RecurrentEffect.OnEnter;
        effectDescription.savingThrowAbility = AttributeDefinitions.Wisdom;
        effectDescription.targetParameter = 0;
        effectDescription.targetType = TargetType.Sphere;

        effectDescription.EffectAdvancement.additionalDicePerIncrement = 1;
        effectDescription.EffectAdvancement.incrementMultiplier = 1;
        effectDescription.EffectAdvancement.effectIncrementMethod = EffectIncrementMethod.PerAdditionalSlotLevel;

        effectDescription.EffectForms[0].SummonForm.effectProxyDefinitionName = ProxyPetalStormName;

        effectDescription.EffectForms[1].hasSavingThrow = true;
        effectDescription.EffectForms[1].savingThrowAffinity = EffectSavingThrowType.HalfDamage;
        effectDescription.EffectForms[1].DamageForm.diceNumber = 4;
        effectDescription.EffectForms[1].DamageForm.dieType = DieType.D6;
        effectDescription.EffectForms[1].DamageForm.damageType = DamageTypePsychic;
        effectDescription.EffectForms[1].levelMultiplier = 1;

        return spell;
    }

    #endregion

    #region LEVEL 03

    internal static SpellDefinition BuildEarthTremor()
    {
        const string NAME = "EarthTremor";

        var spriteReference = CustomIcons.GetSprite(NAME, Resources.EarthTremor, 128, 128);

        // var rubbleProxy = EffectProxyDefinitionBuilder
        //     .Create(EffectProxyDefinitions.ProxyGrease, "RubbleProxy")
        //     .AddToDB();

        var effectDescription = EffectDescriptionBuilder
            .Create()
            .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, 1, 0, 1)
            .SetSavingThrowData(
                true,
                AttributeDefinitions.Dexterity,
                false,
                EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                AttributeDefinitions.Wisdom,
                12)
            .SetDurationData(DurationType.Minute, 10)
            .SetParticleEffectParameters(Grease)
            .SetTargetingData(Side.All, RangeType.Distance, 24, TargetType.Cylinder, 2, 1)
            .SetEffectForms(
                EffectFormBuilder
                    .Create()
                    .SetMotionForm(MotionForm.MotionType.FallProne, 1)
                    .CreatedByCharacter()
                    .HasSavingThrow(EffectSavingThrowType.Negates).Build(),
                EffectFormBuilder
                    .Create()
                    .SetDamageForm(DamageTypeBludgeoning, 3, DieType.D12)
                    .HasSavingThrow(EffectSavingThrowType.HalfDamage).Build(),
                Grease.EffectDescription.EffectForms.Find(e => e.formType == EffectFormType.Topology))
            .Build();

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, spriteReference)
            .SetEffectDescription(effectDescription)
            .SetCastingTime(ActivationTime.Action)
            .SetSpellLevel(3)
            .SetRequiresConcentration(false)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .AddToDB();

        return spell;
    }

    internal static SpellDefinition BuildWinterBreath()
    {
        const string NAME = "WinterBreath";

        var spriteReference = CustomIcons.GetSprite(NAME, Resources.WinterBreath, 128, 128);

        var effectDescription = EffectDescriptionBuilder
            .Create()
            .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, 1, 0, 1)
            .SetSavingThrowData(
                true,
                AttributeDefinitions.Dexterity,
                false,
                EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                AttributeDefinitions.Wisdom,
                12)
            .SetDurationData(DurationType.Minute, 1)
            .SetParticleEffectParameters(ConeOfCold)
            .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Cone, 3)
            .SetEffectForms(
                EffectFormBuilder
                    .Create()
                    .SetMotionForm(MotionForm.MotionType.FallProne, 1)
                    .HasSavingThrow(EffectSavingThrowType.Negates)
                    .Build(),
                EffectFormBuilder
                    .Create()
                    .SetDamageForm(DamageTypeCold, dieType: DieType.D8, diceNumber: 4)
                    .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                    .Build())
            .Build();

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, spriteReference)
            .SetEffectDescription(effectDescription)
            .SetCastingTime(ActivationTime.Action)
            .SetSpellLevel(3)
            .SetRequiresConcentration(false)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolConjuration)
            .AddToDB();

        return spell;
    }

    #region Spirit Shroud

    internal static SpellDefinition BuildSpiritShroud()
    {
        var hinder = ConditionDefinitionBuilder
            .Create("ConditionSpiritShroudHinder")
            .SetGuiPresentation(ConditionHindered_By_Frost.GuiPresentation)
            .SetSilent(Silent.None)
            .SetConditionType(ConditionType.Detrimental)
            .SetFeatures(ConditionHindered_By_Frost.features)
            .SetSpecialDuration(true)
            .SetDuration(DurationType.Round, 1)
            .SetTurnOccurence(TurnOccurenceType.StartOfTurn)
            .CopyParticleReferences(ConditionSpiritGuardians)
            .AddToDB();

        var noHeal = ConditionDefinitionBuilder
            .Create("ConditionSpiritShroudNoHeal")
            .SetGuiPresentation(Category.Condition,
                FeatureDefinitionHealingModifiers.HealingModifierChilledByTouch.GuiPresentation.Description,
                ConditionChilledByTouch.GuiPresentation.SpriteReference)
            .SetConditionType(ConditionType.Detrimental)
            .SetFeatures(
                FeatureDefinitionHealingModifiers.HealingModifierChilledByTouch
            )
            .SetSpecialDuration(true)
            .SetDuration(DurationType.Round, 1)
            .SetTurnOccurence(TurnOccurenceType.StartOfTurn)
            .AddToDB();

        var sprite = CustomIcons.GetSprite("SpiritShroud", Resources.SpiritShroud, 128);

        return SpellDefinitionBuilder
            .Create("SpiritShroud")
            .SetGuiPresentation(Category.Spell, sprite)
            .SetSpellLevel(3)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetCastingTime(ActivationTime.BonusAction)
            .SetRequiresConcentration(true)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetDurationData(DurationType.Minute, 1)
                .Build())
            .SetSubSpells(
                BuildSpriritShroudSubSpell(DamageTypeRadiant, hinder, noHeal, sprite),
                BuildSpriritShroudSubSpell(DamageTypeNecrotic, hinder, noHeal, sprite),
                BuildSpriritShroudSubSpell(DamageTypeCold, hinder, noHeal, sprite)
            )
            .AddToDB();
    }

    private static SpellDefinition BuildSpriritShroudSubSpell(string damage, ConditionDefinition hinder,
        ConditionDefinition noHeal, AssetReferenceSprite sprite)
    {
        return SpellDefinitionBuilder
            .Create($"SpiritShroud{damage}")
            .SetGuiPresentation(Category.Spell, sprite)
            .SetSpellLevel(3)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetCastingTime(ActivationTime.BonusAction)
            .SetRequiresConcentration(true)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetTargetFiltering(TargetFilteringMethod.CharacterOnly)
                .SetTargetingData(Side.Enemy, RangeType.Self, 1, TargetType.Cube, 5)
                .SetDurationData(DurationType.Minute, 1)
                //RAW it should only trigger if target starts turn in the area, but game doesn't trigger on turn start for some reason without other flags
                .SetRecurrentEffect(RecurrentEffect.OnActivation
                                    | RecurrentEffect.OnTurnStart
                                    | RecurrentEffect.OnEnter)
                .SetParticleEffectParameters(SpiritGuardians)
                .SetEffectForms(
                    EffectFormBuilder.Create()
                        .SetConditionForm(hinder, ConditionForm.ConditionOperation.Add)
                        .Build(),
                    EffectFormBuilder.Create()
                        .SetConditionForm(ConditionDefinitionBuilder
                            .Create($"ConditionSpiritShroud{damage}")
                            // .SetGuiPresentation(Category.Condition, ConditionSpiritGuardiansSelf)
                            .SetGuiPresentationNoContent()
                            .SetSilent(Silent.WhenAddedOrRemoved)
                            .CopyParticleReferences(ConditionSpiritGuardiansSelf)
                            .SetFeatures(FeatureDefinitionAdditionalDamageBuilder
                                .Create($"AdditionalDamageSpiritShroud{damage}")
                                .SetGuiPresentationNoContent(true)
                                .SetNotificationTag($"SpiritShroud{damage}")
                                .SetTriggerCondition(ExtraAdditionalDamageTriggerCondition.TargetWithin10Ft)
                                .SetFrequencyLimit(FeatureLimitedUsage.None)
                                .SetAttackOnly()
                                .SetConditionOperations(new ConditionOperationDescription
                                {
                                    operation = ConditionOperationDescription.ConditionOperation.Add,
                                    conditionDefinition = noHeal,
                                    hasSavingThrow = false
                                })
                                .SetDamageDice(DieType.D8, 1)
                                .SetSpecificDamageType(damage)
                                .SetAdvancement(AdditionalDamageAdvancement.SlotLevel,
                                    (3, 1),
                                    (4, 1),
                                    (5, 2),
                                    (6, 2),
                                    (7, 3),
                                    (8, 3),
                                    (9, 4))
                                .AddToDB())
                            .AddToDB(), ConditionForm.ConditionOperation.Add, true, true)
                        .Build(),
                    EffectFormBuilder.Create()
                        .SetTopologyForm(TopologyForm.Type.DangerousZone, true)
                        .Build())
                .Build())
            .AddToDB();
    }

    #endregion

    #endregion

    #region LEVEL 09

    internal static SpellDefinition BuildForesight()
    {
        var effectDescription = EffectDescriptionBuilder
            .Create()
            .SetDurationData(
                DurationType.Hour,
                8)
            .SetTargetingData(
                Side.Ally,
                RangeType.Touch,
                1,
                TargetType.Individuals)
            .SetEffectForms(EffectFormBuilder
                .Create()
                .SetConditionForm(
                    ConditionDefinitionBuilder
                        .Create(ConditionBearsEndurance, "ConditionForesight")
                        .SetOrUpdateGuiPresentation(Category.Condition)
                        .SetFeatures(
                            FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityConditionBearsEndurance,
                            FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityConditionBullsStrength,
                            FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityConditionCatsGrace,
                            FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityConditionEaglesSplendor,
                            FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityConditionFoxsCunning,
                            FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityConditionOwlsWisdom,
                            FeatureDefinitionCombatAffinitys.CombatAffinityStealthy,
                            FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinityShelteringBreeze
                        )
                        .AddToDB(),
                    ConditionForm.ConditionOperation.Add,
                    false,
                    false)
                .Build())
            .Build();

        return SpellDefinitionBuilder
            .Create("Foresight")
            .SetGuiPresentation(Category.Spell, TrueSeeing)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(9)
            .SetCastingTime(ActivationTime.Minute1)
            .SetVerboseComponent(true)
            .SetSomaticComponent(false)
            .SetEffectDescription(effectDescription)
            .SetAiParameters(new SpellAIParameters())
            .AddToDB();
    }

    internal static SpellDefinition BuildMassHeal()
    {
        var effectDescription = EffectDescriptionBuilder.Create()
            .SetDurationData(DurationType.Instantaneous)
            .SetTargetingData(Side.All, RangeType.Distance, 12, TargetType.Individuals, 6)
            .SetEffectForms(EffectFormBuilder
                .Create()
                .SetHealingForm(
                    HealingComputation.Dice,
                    120,
                    DieType.D1,
                    0,
                    false,
                    HealingCap.MaximumHitPoints)
                .Build())
            .Build();

        return SpellDefinitionBuilder
            .Create("MassHeal")
            .SetGuiPresentation(Category.Spell, Heal)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(9)
            .SetCastingTime(ActivationTime.Action)
            .SetVerboseComponent(true)
            .SetSomaticComponent(false)
            .SetEffectDescription(effectDescription)
            .SetAiParameters(new SpellAIParameters())
            .AddToDB();
    }

    internal static SpellDefinition BuildMeteorSwarmSingleTarget()
    {
        var effectDescription = EffectDescriptionBuilder
            .Create()
            .SetDurationData(DurationType.Instantaneous)
            .SetTargetingData(
                Side.All,
                RangeType.Distance,
                200,
                TargetType.Sphere,
                8,
                8)
            // 20 dice number because hits dont stack even on single target
            .SetEffectForms(
                EffectFormBuilder
                    .Create()
                    .SetDamageForm(DamageTypeFire, 20, DieType.D6)
                    .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                    .Build(),
                EffectFormBuilder
                    .Create()
                    .SetDamageForm(DamageTypeBludgeoning, 0, DieType.D6)
                    .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                    .Build())
            .SetSavingThrowData(
                false,
                AttributeDefinitions.Dexterity,
                true,
                EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                AttributeDefinitions.Dexterity,
                20)
            .SetParticleEffectParameters(FlameStrike)
            .Build();

        return SpellDefinitionBuilder
            .Create("MeteorSwarmSingleTarget")
            .SetGuiPresentation(Category.Spell, FlamingSphere)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(9)
            .SetCastingTime(ActivationTime.Action)
            .SetVerboseComponent(true)
            .SetSomaticComponent(false)
            .SetEffectDescription(effectDescription)
            .SetAiParameters(new SpellAIParameters())
            .AddToDB();
    }

    internal static SpellDefinition BuildPowerWordHeal()
    {
        var effectDescription = EffectDescriptionBuilder
            .Create()
            .SetDurationData(DurationType.Instantaneous)
            .SetTargetingData(
                Side.Ally,
                RangeType.Distance,
                12,
                TargetType.Individuals)
            .SetEffectForms(
                EffectFormBuilder
                    .Create()
                    .SetHealingForm(
                        HealingComputation.Dice,
                        700,
                        DieType.D1,
                        0,
                        false,
                        HealingCap.MaximumHitPoints)
                    .Build(),
                EffectFormBuilder
                    .Create()
                    .SetConditionForm(
                        ConditionDefinitions.ConditionParalyzed,
                        ConditionForm.ConditionOperation.RemoveDetrimentalAll,
                        false,
                        false,
                        ConditionDefinitions.ConditionCharmed,
                        ConditionCharmedByHypnoticPattern,
                        ConditionDefinitions.ConditionFrightened,
                        ConditionDefinitions.ConditionFrightenedFear,
                        ConditionFrightenedPhantasmalKiller,
                        ConditionDefinitions.ConditionParalyzed,
                        ConditionParalyzed_CrimsonSpiderVenom,
                        ConditionParalyzed_GhoulsCaress,
                        ConditionDefinitions.ConditionStunned,
                        ConditionStunned_MutantApeSlam,
                        ConditionStunnedConjuredDeath,
                        ConditionDefinitions.ConditionProne)
                    .Build())
            .Build();

        return SpellDefinitionBuilder
            .Create("PowerWordHeal")
            .SetGuiPresentation(Category.Spell, HealingWord)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(9)
            .SetCastingTime(ActivationTime.Action)
            .SetVerboseComponent(true)
            .SetSomaticComponent(false)
            .SetEffectDescription(effectDescription)
            .SetAiParameters(new SpellAIParameters())
            .AddToDB();
    }

    internal static SpellDefinition BuildPowerWordKill()
    {
        var effectDescription = EffectDescriptionBuilder
            .Create()
            .SetDurationData(DurationType.Instantaneous)
            .SetTargetingData(
                Side.Enemy,
                RangeType.Distance,
                12,
                TargetType.Individuals)
            .SetEffectForms(
                EffectFormBuilder
                    .Create()
                    .SetKillForm(
                        KillCondition.UnderHitPoints,
                        0F,
                        100)
                    .SetLevelAdvancement(
                        LevelApplianceType.No,
                        LevelSourceType.ClassLevel)
                    .CreatedByCharacter()
                    .Build())
            .Build();

        return SpellDefinitionBuilder
            .Create("PowerWordKill")
            .SetGuiPresentation(Category.Spell, Disintegrate)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(9)
            .SetCastingTime(ActivationTime.Action)
            .SetVerboseComponent(true)
            .SetSomaticComponent(false)
            .SetEffectDescription(effectDescription)
            .SetAiParameters(new SpellAIParameters())
            .AddToDB();
    }

    internal static SpellDefinition BuildShapechange()
    {
        var effectDescription = EffectDescriptionBuilder
            .Create()
            .SetCreatedByCharacter()
            .SetParticleEffectParameters(PowerDruidWildShape)
            .SetDurationData(
                DurationType.Hour,
                1)
            .SetTargetingData(
                Side.Ally,
                RangeType.Distance,
                12,
                TargetType.Self)
            .SetEffectForms(
                new EffectForm
                {
                    addBonusMode = AddBonusMode.None,
                    applyLevel = LevelApplianceType.No,
                    canSaveToCancel = false,
                    createdByCharacter = true,
                    formType = EffectFormType.ShapeChange,
                    shapeChangeForm = new ShapeChangeForm
                    {
                        keepMentalAbilityScores = true,
                        shapeChangeType = ShapeChangeForm.Type.FreeListSelection,
                        specialSubstituteCondition = ConditionDefinitions.ConditionWildShapeSubstituteForm,
                        shapeOptions = new List<ShapeOptionDescription>
                        {
                            new() { requiredLevel = 1, substituteMonster = GoldDragon_AerElai },
                            new() { requiredLevel = 1, substituteMonster = Divine_Avatar },
                            new() { requiredLevel = 1, substituteMonster = Sorr_Akkath_Tshar_Boss },
                            new()
                            {
                                requiredLevel = 1, substituteMonster = GreenDragon_MasterOfConjuration
                            },
                            new() { requiredLevel = 1, substituteMonster = BlackDragon_MasterOfNecromancy },
                            new() { requiredLevel = 1, substituteMonster = Remorhaz },
                            new() { requiredLevel = 1, substituteMonster = Emperor_Laethar },
                            new() { requiredLevel = 1, substituteMonster = Giant_Ape },
                            new() { requiredLevel = 1, substituteMonster = Spider_Queen },
                            new() { requiredLevel = 1, substituteMonster = Sorr_Akkath_Shikkath }
                        }
                    }
                })
            .Build();

        return SpellDefinitionBuilder
            .Create("Shapechange")
            .SetGuiPresentation(Category.Spell, PowerDruidWildShape)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(9)
            .SetCastingTime(ActivationTime.Action)
            .SetVerboseComponent(true)
            .SetSomaticComponent(false)
            .SetEffectDescription(effectDescription)
            .SetAiParameters(new SpellAIParameters())
            .SetRequiresConcentration(true)
            .AddToDB();
    }

    internal static SpellDefinition BuildTimeStop()
    {
        var effectDescription = EffectDescriptionBuilder
            .Create()
            .SetDurationData(DurationType.Round, 3)
            .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Cylinder, 20, 10)
            .SetEffectForms(EffectFormBuilder
                .Create()
                .SetConditionForm(
                    ConditionDefinitionBuilder
                        .Create(ConditionDefinitions.ConditionIncapacitated, "ConditionTimeStop")
                        .SetOrUpdateGuiPresentation(Category.Condition)
                        .SetInterruptionDamageThreshold(1)
                        .SetSpecialInterruptions(
                            ConditionInterruption.Attacked,
                            ConditionInterruption.Damaged)
                        .AddToDB(),
                    ConditionForm.ConditionOperation.Add,
                    false,
                    false)
                .Build())
            .ExcludeCaster()
            .Build();

        return SpellDefinitionBuilder
            .Create("TimeStop")
            .SetGuiPresentation(Category.Spell, PowerDomainLawWordOfLaw)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(9)
            .SetCastingTime(ActivationTime.Action)
            .SetVerboseComponent(true)
            .SetSomaticComponent(false)
            .SetEffectDescription(effectDescription)
            .SetAiParameters(new SpellAIParameters())
            .AddToDB();
    }

    internal static SpellDefinition BuildWeird()
    {
        var effectDescription = EffectDescriptionBuilder.Create()
            .SetDurationData(DurationType.Minute, 1)
            .SetTargetingData(Side.Enemy, RangeType.Distance, 12, TargetType.Sphere, 6, 6)
            .SetSavingThrowData(
                false,
                AttributeDefinitions.Wisdom,
                true,
                EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                AttributeDefinitions.Constitution,
                20)
            .SetEffectForms(EffectFormBuilder
                .Create()
                .SetConditionForm(
                    ConditionDefinitionBuilder
                        .Create(ConditionFrightenedPhantasmalKiller, "ConditionWeird")
                        .SetOrUpdateGuiPresentation(Category.Condition)
                        .AddToDB(),
                    ConditionForm.ConditionOperation.Add,
                    false,
                    false)
                .HasSavingThrow(EffectSavingThrowType.Negates)
                .CanSaveToCancel(TurnOccurenceType.EndOfTurn)
                .Build())
            .Build();

        return SpellDefinitionBuilder
            .Create("Weird")
            .SetGuiPresentation(Category.Spell, PhantasmalKiller)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(9)
            .SetCastingTime(ActivationTime.Action)
            .SetVerboseComponent(true)
            .SetSomaticComponent(false)
            .SetEffectDescription(effectDescription)
            .SetAiParameters(new SpellAIParameters())
            .SetRequiresConcentration(true)
            .AddToDB();
    }

    #endregion
}

internal sealed class ChainSpellEffectOnAttackHit : IChainMagicEffect
{
    private readonly string _notificationTag;
    private readonly SpellDefinition _spell;

    internal ChainSpellEffectOnAttackHit(SpellDefinition spell, [CanBeNull] string notificationTag = null)
    {
        _spell = spell;
        _notificationTag = notificationTag;
    }

    [CanBeNull]
    public CharacterActionMagicEffect GetNextMagicEffect(
        [CanBeNull] CharacterActionMagicEffect baseEffect,
        CharacterActionAttack triggeredAttack,
        RollOutcome attackOutcome)
    {
        if (baseEffect == null)
        {
            return null;
        }

        var spellEffect = baseEffect as CharacterActionCastSpell;
        var repertoire = spellEffect?.ActiveSpell.SpellRepertoire;
        var actionParams = baseEffect.actionParams;

        if (actionParams == null)
        {
            return null;
        }

        if (baseEffect.Countered || baseEffect.ExecutionFailed)
        {
            return null;
        }

        if (attackOutcome != RollOutcome.Success
            && attackOutcome != RollOutcome.CriticalSuccess)
        {
            return null;
        }

        var caster = actionParams.ActingCharacter;
        var targets = actionParams.TargetCharacters;

        if (caster == null || targets.Count < 2)
        {
            return null;
        }

        var rulesetCaster = caster.RulesetCharacter;
        var rules = ServiceRepository.GetService<IRulesetImplementationService>();
        var bonusLevelProvider = _spell.GetFirstSubFeatureOfType<IBonusSlotLevels>();
        var slotLevel = _spell.SpellLevel;

        if (bonusLevelProvider != null)
        {
            slotLevel += bonusLevelProvider.GetBonusSlotLevels(rulesetCaster);
        }

        var effectSpell = rules.InstantiateEffectSpell(rulesetCaster, repertoire, _spell, slotLevel, false);

        for (var i = 1; i < targets.Count; i++)
        {
            var rulesetTarget = targets[i].RulesetCharacter;

            if (!string.IsNullOrEmpty(_notificationTag))
            {
                GameConsoleHelper.LogCharacterAffectsTarget(rulesetCaster, rulesetTarget, _notificationTag, true);
            }

            effectSpell.ApplyEffectOnCharacter(rulesetTarget, true, targets[i].LocationPosition);
        }

        effectSpell.Terminate(true);

        return null;
    }
}

internal sealed class BonusSlotLevelsByClassLevel : IBonusSlotLevels
{
    public int GetBonusSlotLevels([NotNull] RulesetCharacter caster)
    {
        return caster.GetAttribute(AttributeDefinitions.CharacterLevel).CurrentValue;
    }
}
