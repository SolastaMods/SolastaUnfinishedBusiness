using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomDefinitions;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.Utils;
using UnityEngine;
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
    //
    // CANTRIPS
    //

    internal static SpellDefinition BuildAcidClaw()
    {
        const string NAME = "AcidClaws";
        const string CONDITION = "ConditionAcidClaws";
        const string MODIFIER = "AttributeModifierAcidClawsACDebuff";

        var spriteReference =
            CustomIcons.CreateAssetReferenceSprite(NAME, Resources.AcidClaws, 128, 128);

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, spriteReference)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetEffectAdvancement(EffectIncrementMethod.CasterLevelTable, 5, additionalDicePerIncrement: 1)
                .SetDurationData(DurationType.Instantaneous)
                .SetTargetingData(Side.Enemy, RangeType.MeleeHit, 1, TargetType.Individuals)
                .AddEffectForm(EffectFormBuilder.Create()
                    .SetDamageForm(dieType: DieType.D8, diceNumber: 1, damageType: DamageTypeNecrotic)
                    .HasSavingThrow(EffectSavingThrowType.None)
                    .Build())
                .AddEffectForm(EffectFormBuilder.Create()
                    .SetConditionForm(ConditionDefinitionBuilder
                        .Create(CONDITION)
                        .SetGuiPresentation(Category.Condition,
                            ConditionAcidSpit.GuiPresentation.SpriteReference)
                        .SetDuration(DurationType.Round, 1)
                        .SetSpecialDuration(true)
                        .SetFeatures(FeatureDefinitionAttributeModifierBuilder
                            .Create(MODIFIER)
                            .SetGuiPresentation(Category.Feature)
                            .SetModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
                                AttributeDefinitions.ArmorClass, -1)
                            .AddToDB())
                        .AddToDB(), ConditionForm.ConditionOperation.Add)
                    .HasSavingThrow(EffectSavingThrowType.None)
                    .Build())
                .Build())
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

        var spriteReference =
            CustomIcons.CreateAssetReferenceSprite(NAME, Resources.AirBlast, 128, 128);

        var effectDescription = EffectDescriptionBuilder
            .Create()
            .SetEffectAdvancement(EffectIncrementMethod.CasterLevelTable, 1, 0, 1)
            .SetSavingThrowData(true, false, AttributeDefinitions.Strength, false,
                EffectDifficultyClassComputation.SpellCastingFeature, AttributeDefinitions.Wisdom, 15)
            .SetDurationData(DurationType.Instantaneous)
            .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.Individuals, 1, 2)
            .AddEffectForm(
                EffectFormBuilder
                    .Create()
                    .SetMotionForm(MotionForm.MotionType.PushFromOrigin, 1)
                    .HasSavingThrow(EffectSavingThrowType.Negates).Build())
            .AddEffectForm(
                EffectFormBuilder
                    .Create()
                    .SetDamageForm(false, DieType.D1, DamageTypeBludgeoning, 0, DieType.D6, 1)
                    .HasSavingThrow(EffectSavingThrowType.Negates).Build()
            ).Build();

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

        var spriteReference =
            CustomIcons.CreateAssetReferenceSprite(NAME, Resources.BurstOfRadiance, 128, 128);

        var effectDescription = EffectDescriptionBuilder
            .Create()
            .SetEffectAdvancement(EffectIncrementMethod.CasterLevelTable, 1, 0, 1)
            .SetSavingThrowData(true, true, AttributeDefinitions.Constitution, false,
                EffectDifficultyClassComputation.SpellCastingFeature, AttributeDefinitions.Wisdom, 13)
            .SetDurationData(DurationType.Instantaneous)
            .SetParticleEffectParameters(BurningHands)
            .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Sphere, 1, 2)
            .AddEffectForm(
                EffectFormBuilder
                    .Create()
                    .SetDamageForm(false, DieType.D1, DamageTypeRadiant, 0, DieType.D6, 1)
                    .HasSavingThrow(EffectSavingThrowType.Negates).Build()
            ).Build();

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
            .SetGuiPresentation(Category.Spell, Shine.GuiPresentation.SpriteReference)
            .AddToDB();

        spell.EffectDescription.rangeType = RangeType.Distance;
        spell.EffectDescription.rangeParameter = 18;
        spell.EffectDescription.targetType = TargetType.Sphere;
        spell.EffectDescription.targetParameter = 5;
        spell.EffectDescription.effectParticleParameters = SacredFlame_B.EffectDescription.EffectParticleParameters;

        return spell;
    }

    internal static SpellDefinition BuildMinorLifesteal()
    {
        return SpellDefinitionBuilder
            .Create("MinorLifesteal")
            .SetGuiPresentation(Category.Spell,
                CustomIcons.CreateAssetReferenceSprite("MinorLifesteal", Resources.MinorLifesteal, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolNecromancy)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetSomaticComponent(true)
            .SetVerboseComponent(false)
            .SetSpellLevel(0)
            .SetRequiresConcentration(false)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetTargetingData(Side.Enemy, RangeType.RangeHit, 12, TargetType.Individuals)
                .AddImmuneCreatureFamilies(CharacterFamilyDefinitions.Construct, CharacterFamilyDefinitions.Undead)
                .SetEffectAdvancement(EffectIncrementMethod.CasterLevelTable, 5, additionalDicePerIncrement: 1)
                .AddEffectForm(EffectFormBuilder
                    .Create()
                    .SetDamageForm(dieType: DieType.D8, diceNumber: 1, damageType: DamageTypeNecrotic,
                        healFromInflictedDamage: HealFromInflictedDamage.Half)
                    .HasSavingThrow(EffectSavingThrowType.None)
                    .Build())
                .AddEffectForm(EffectFormBuilder
                    .Create()
                    .SetTempHpForm(dieType: DieType.D4, diceNumber: 1, applyToSelf: true)
                    .HasSavingThrow(EffectSavingThrowType.None)
                    .Build())
                .SetParticleEffectParameters(VampiricTouch)
                .Build())
            .AddToDB();
    }

    internal static SpellDefinition BuildResonatingStrike()
    {
        var resonanceHighLevel = EffectDescriptionBuilder.Create()
            .SetParticleEffectParameters(AcidSplash)
            .SetTargetFiltering(TargetFilteringMethod.CharacterOnly)
            .SetTargetingData(Side.Enemy, RangeType.Touch, 1,
                TargetType.Individuals)
            .SetEffectForms(EffectFormBuilder
                .Create()
                .SetBonusMode(AddBonusMode.AbilityBonus)
                .SetDamageForm(
                    dieType: DieType.D8,
                    diceNumber: 0,
                    bonusDamage: 0,
                    damageType: DamageTypeThunder
                )
                .Build()
            )
            .SetEffectAdvancement(
                EffectIncrementMethod.PerAdditionalSlotLevel,
                5, additionalDicePerIncrement: 1)
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
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetParticleEffectParameters(AcidSplash)
                .SetTargetFiltering(TargetFilteringMethod.CharacterOnly)
                .SetTargetingData(Side.Enemy, RangeType.Touch, 1,
                    TargetType.Individuals)
                .SetEffectForms(EffectFormBuilder
                    .Create()
                    .SetBonusMode(AddBonusMode.AbilityBonus)
                    .SetDamageForm(
                        dieType: DieType.D1,
                        diceNumber: 0,
                        bonusDamage: 0,
                        damageType: DamageTypeThunder
                    )
                    .Build()
                )
                .Build()
            )
            .AddToDB();

        return SpellDefinitionBuilder
            .Create("ResonatingStrike")
            .SetGuiPresentation(Category.Spell,
                CustomIcons.CreateAssetReferenceSprite("ResonatingStrike", Resources.ResonatingStrike, 128, 128))
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
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetParticleEffectParameters(ScorchingRay)
                .SetTargetingData(
                    Side.Enemy,
                    RangeType.Distance,
                    5,
                    TargetType.IndividualsUnique,
                    2
                )
                .SetTargetProximityData(true, 1)
                .SetSavingThrowData(
                    false,
                    false,
                    AttributeDefinitions.Dexterity,
                    true,
                    EffectDifficultyClassComputation.SpellCastingFeature,
                    AttributeDefinitions.Charisma
                )
                .SetEffectAdvancement(
                    EffectIncrementMethod.CasterLevelTable,
                    additionalDicePerIncrement: 1,
                    incrementMultiplier: 1
                )
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
                                .Configure(
                                    "ResonatingStrike",
                                    FeatureLimitedUsage.None,
                                    AdditionalDamageValueDetermination.Die,
                                    AdditionalDamageTriggerCondition.AlwaysActive,
                                    RestrictedContextRequiredProperty.MeleeWeapon,
                                    true,
                                    DieType.D8,
                                    1,
                                    AdditionalDamageType.Specific,
                                    DamageTypeThunder,
                                    AdditionalDamageAdvancement.SlotLevel,
                                    DiceByRankBuilder.BuildDiceByRankTable(0, step: 5, increment: 1),
                                    true
                                )
                                .AddToDB()
                            )
                            .AddToDB(),
                        ConditionForm.ConditionOperation.Add,
                        true,
                        false
                    )
                    .Build()
                )
                .Build()
            )
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
                CustomIcons.CreateAssetReferenceSprite("SunlightBlade", Resources.SunlightBlade, 128,
                    128))
            .SetSpellLevel(0)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetSomaticComponent(true)
            .SetVerboseComponent(false)
            .SetMaterialComponent(MaterialComponentType.Specific)
            .SetSpecificMaterialComponent(TagsDefinitions.WeaponTagMelee, 0, false)
            .SetCustomSubFeatures(
                PerformAttackAfterMagicEffectUse.MeleeAttack,
                new UpgradeRangeBasedOnWeaponReach(),
                CustomSpellEffectLevel.ByCasterLevel
            )
            .SetCastingTime(ActivationTime.Action)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetParticleEffectParameters(ScorchingRay)
                .SetTargetingData(
                    Side.Enemy,
                    RangeType.Touch,
                    1,
                    TargetType.IndividualsUnique
                )
                .SetSavingThrowData(
                    false,
                    false,
                    AttributeDefinitions.Dexterity,
                    true,
                    EffectDifficultyClassComputation.SpellCastingFeature,
                    AttributeDefinitions.Charisma
                )
                .SetEffectAdvancement(
                    EffectIncrementMethod.CasterLevelTable,
                    additionalDicePerIncrement: 1,
                    incrementMultiplier: 1
                )
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
                                    .Configure(
                                        "SunlightBlade",
                                        FeatureLimitedUsage.None,
                                        AdditionalDamageValueDetermination.Die,
                                        AdditionalDamageTriggerCondition.AlwaysActive,
                                        RestrictedContextRequiredProperty.MeleeWeapon,
                                        true,
                                        DieType.D8,
                                        1,
                                        AdditionalDamageType.Specific,
                                        DamageTypeRadiant,
                                        AdditionalDamageAdvancement.SlotLevel,
                                        DiceByRankBuilder.BuildDiceByRankTable(0, step: 5, increment: 1)
                                    )
                                    .SetTargetCondition(sunlitMark,
                                        AdditionalDamageTriggerCondition.TargetHasCondition)
                                    .SetConditionOperations(highlight)
                                    .SetAddLightSource(true)
                                    .SetLightSourceForm(dimLight)
                                    .AddToDB()
                                )
                                .AddToDB(),
                            ConditionForm.ConditionOperation.Add,
                            true,
                            false
                        ).Build(),
                    EffectFormBuilder.Create()
                        .HasSavingThrow(EffectSavingThrowType.None)
                        .SetConditionForm(sunlitMark, ConditionForm.ConditionOperation.Add)
                        .Build()
                )
                .Build()
            )
            .AddToDB();
    }

    internal static SpellDefinition BuildThornyVines()
    {
        return SpellDefinitionBuilder
            .Create("ThornyVines")
            .SetGuiPresentation(Category.Spell,
                CustomIcons.CreateAssetReferenceSprite("ThornyVines", Resources.ThornyVines, 128))
            .SetSpellLevel(0)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSomaticComponent(true)
            .SetVerboseComponent(true)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetCastingTime(ActivationTime.Action)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetTargetingData(Side.Enemy, RangeType.RangeHit, 6, TargetType.Individuals)
                .SetParticleEffectParameters(VenomousSpike)
                .SetEffectAdvancement(EffectIncrementMethod.CasterLevelTable, 5,
                    additionalDicePerIncrement: 1)
                .AddEffectForm(EffectFormBuilder.Create()
                    .SetDamageForm(diceNumber: 1, dieType: DieType.D6, damageType: DamageTypePiercing)
                    .Build())
                .AddEffectForm(EffectFormBuilder.Create()
                    .SetMotionForm(MotionForm.MotionType.DragToOrigin, 2)
                    .Build())
                .Build())
            .AddToDB();
    }

    internal static SpellDefinition BuildThunderStrike()
    {
        const string NAME = "ThunderStrike";

        var spriteReference = Shield.GuiPresentation.SpriteReference;

        var effectDescription = EffectDescriptionBuilder
            .Create()
            .SetEffectAdvancement(
                EffectIncrementMethod.CasterLevelTable, 1, 0, 1)
            .SetSavingThrowData(true, true, AttributeDefinitions.Constitution, false,
                EffectDifficultyClassComputation.SpellCastingFeature, AttributeDefinitions.Wisdom, 15)
            .SetDurationData(DurationType.Instantaneous)
            .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Sphere)
            .AddEffectForm(
                EffectFormBuilder
                    .Create()
                    .SetDamageForm(false, DieType.D1, DamageTypeThunder, 0, DieType.D6, 1)
                    .HasSavingThrow(EffectSavingThrowType.Negates).Build()
            ).Build();

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

    //
    // LEVEL 01
    //

    internal static SpellDefinition BuildFindFamiliar()
    {
        var owlFamiliar = MonsterDefinitionBuilder
            .Create(Eagle_Matriarch, "OwlFamiliar")
            .SetGuiPresentation("OwlFamiliar", Category.Monster, Eagle_Matriarch.GuiPresentation.SpriteReference)
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
            .ClearAttackIterations()
            .SetSkillScores(
                (DatabaseHelper.SkillDefinitions.Perception.Name, 3),
                (DatabaseHelper.SkillDefinitions.Stealth.Name, 3)
            )
            .SetArmorClass(11)
            .SetAbilityScores(3, 13, 8, 2, 12, 7)
            .SetHitDiceNumber(1)
            .SetHitDiceType(DieType.D4)
            .SetHitPointsBonus(-1)
            .SetStandardHitPoints(1)
            .SetSizeDefinition(CharacterSizeDefinitions.Tiny)
            .SetAlignment("Neutral")
            .SetCharacterFamily(CharacterFamilyDefinitions.Fey.name)
            .SetChallengeRating(0)
            .SetDroppedLootDefinition(null)
            .SetDefaultBattleDecisionPackage(DecisionPackageDefinitions
                .DefaultSupportCasterWithBackupAttacksDecisions)
            .SetFullyControlledWhenAllied(true)
            .SetDefaultFaction("Party")
            .SetBestiaryEntry(BestiaryDefinitions.BestiaryEntry.None)
            .AddFeatures(CharacterContext.FeatureDefinitionPowerHelpAction)
            .AddToDB();

        var spell = SpellDefinitionBuilder.Create(Fireball, "FindFamiliar")
            .SetGuiPresentation(Category.Spell, AnimalFriendship.GuiPresentation.SpriteReference)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolConjuration)
            .SetMaterialComponent(MaterialComponentType.Specific)
            .SetSomaticComponent(true)
            .SetVerboseComponent(true)
            .SetSpellLevel(1)
            .SetUniqueInstance(true)
            .SetCastingTime(ActivationTime.Hours1)
            // BUG: Unable to have 70 minutes ritual casting time... if set to 10 minutes, it really only takes 10 minutes, instead of 70
            .SetRitualCasting(ActivationTime.Minute10)
            .AddToDB();

        spell.EffectDescription.Copy(ConjureAnimalsOneBeast.EffectDescription);
        spell.EffectDescription.rangeType = RangeType.Distance;
        spell.EffectDescription.rangeParameter = 2;
        spell.EffectDescription.durationType = DurationType.Permanent;
        spell.EffectDescription.targetSide = Side.Ally;
        spell.EffectDescription.EffectForms.SetRange(
            EffectFormBuilder
                .Create()
                .SetSummonCreatureForm(1, owlFamiliar.name)
                .CreatedByCharacter()
                .Build());

        GlobalUniqueEffects.AddToGroup(GlobalUniqueEffects.Group.Familiar, spell);

        return spell;
    }

    internal static SpellDefinition BuildMule()
    {
        const string NAME = "Mule";

        var movementAffinity = FeatureDefinitionMovementAffinityBuilder
            .Create("MovementAffinityConditionMule")
            .SetGuiPresentationNoContent(true)
            .AddToDB();
        movementAffinity.heavyArmorImmunity = true;
        movementAffinity.encumbranceImmunity = true;

        var equipmentAffinity = FeatureDefinitionEquipmentAffinityBuilder
            .Create("EquipmentAffinityConditionMule")
            .SetGuiPresentationNoContent(true)
            .AddToDB();
        equipmentAffinity.additionalCarryingCapacity = 20;

        var effectDescription = EffectDescriptionBuilder
            .Create()
            .SetTargetingData(
                Side.Ally,
                RangeType.Touch,
                0,
                TargetType.Individuals,
                1,
                2, ActionDefinitions.ItemSelectionType.Equiped)
            .SetDurationData(
                DurationType.Hour,
                8)
            .SetParticleEffectParameters(ExpeditiousRetreat)
            .AddEffectForm(
                EffectFormBuilder
                    .Create()
                    .SetConditionForm(
                        ConditionDefinitionBuilder
                            .Create("ConditionMule")
                            .SetGuiPresentation(Category.Condition, Longstrider.GuiPresentation.SpriteReference)
                            .SetConditionType(ConditionType.Beneficial)
                            .SetFeatures(movementAffinity, equipmentAffinity)
                            .AddToDB(),
                        ConditionForm.ConditionOperation.Add,
                        false,
                        false,
                        ConditionJump.AdditionalCondition)
                    .Build())
            .Build();

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Longstrider.GuiPresentation.SpriteReference)
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
            .SetGuiPresentation(Category.Spell, Sparkle.GuiPresentation.SpriteReference)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(MagicMissile.EffectDescription)
                    .SetTargetingData(
                        Side.Enemy,
                        RangeType.Distance,
                        18,
                        TargetType.Individuals,
                        5,
                        2,
                        ActionDefinitions.ItemSelectionType.Equiped)
                    .AddEffectForm(Shine.EffectDescription.EffectForms[0])
                    .SetEffectAdvancement(
                        EffectIncrementMethod.PerAdditionalSlotLevel,
                        1, 2)
                    .Build())
            .AddToDB();

        spell.EffectDescription.EffectForms[0].DamageForm.dieType = DieType.D1;
        spell.EffectDescription.EffectForms[0].DamageForm.damageType = DamageTypeRadiant;

        return spell;
    }

    //
    // LEVEL 02
    //

    internal static SpellDefinition BuildPetalStorm()
    {
        const string ProxyPetalStormName = "ProxyPetalStorm";

        _ = EffectProxyDefinitionBuilder
            .Create(EffectProxyDefinitions.ProxyInsectPlague, ProxyPetalStormName)
            .SetGuiPresentation("PetalStorm", Category.Spell, WindWall.GuiPresentation.SpriteReference)
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
            .SetGuiPresentation(Category.Spell, WindWall.GuiPresentation.SpriteReference)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolConjuration)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetSomaticComponent(true)
            .SetVerboseComponent(true)
            .SetSpellLevel(2)
            .SetRequiresConcentration(true)
            .AddToDB();

        spell.EffectDescription.rangeType = RangeType.Distance;
        spell.EffectDescription.rangeParameter = 12;
        spell.EffectDescription.durationType = DurationType.Minute;
        spell.EffectDescription.durationParameter = 1;
        spell.EffectDescription.targetType = TargetType.Cube;
        spell.EffectDescription.targetParameter = 3;
        spell.EffectDescription.hasSavingThrow = true;
        spell.EffectDescription.savingThrowAbility = AttributeDefinitions.Strength;
        spell.EffectDescription.recurrentEffect = (RecurrentEffect)20;

        spell.EffectDescription.EffectAdvancement.additionalDicePerIncrement = 2;
        spell.EffectDescription.EffectAdvancement.incrementMultiplier = 1;
        spell.EffectDescription.EffectAdvancement.effectIncrementMethod = EffectIncrementMethod.PerAdditionalSlotLevel;

        spell.EffectDescription.EffectForms[0].hasSavingThrow = true;
        spell.EffectDescription.EffectForms[0].savingThrowAffinity = EffectSavingThrowType.Negates;
        spell.EffectDescription.EffectForms[0].DamageForm.diceNumber = 3;
        spell.EffectDescription.EffectForms[0].DamageForm.dieType = DieType.D4;
        spell.EffectDescription.EffectForms[0].DamageForm.damageType = DamageTypeSlashing;
        spell.EffectDescription.EffectForms[0].levelMultiplier = 1;

        spell.EffectDescription.EffectForms[2].SummonForm.effectProxyDefinitionName = ProxyPetalStormName;

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
            .SetGuiPresentation(Category.Spell, Bane.GuiPresentation.SpriteReference)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolAbjuration)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetSomaticComponent(true)
            .SetVerboseComponent(true)
            .SetSpellLevel(2)
            .SetRequiresConcentration(false)
            .SetRitualCasting(ActivationTime.Minute10)
            .AddToDB();

        spell.EffectDescription.difficultyClassComputation =
            EffectDifficultyClassComputation.AbilityScoreAndProficiency;
        spell.EffectDescription.durationParameter = 10;
        spell.EffectDescription.durationType = DurationType.Minute;
        spell.EffectDescription.hasSavingThrow = true;
        spell.EffectDescription.rangeParameter = 1;
        spell.EffectDescription.rangeType = RangeType.Distance;
        spell.EffectDescription.recurrentEffect = RecurrentEffect.OnEnter;
        spell.EffectDescription.savingThrowAbility = AttributeDefinitions.Wisdom;
        spell.EffectDescription.targetParameter = 0;
        spell.EffectDescription.targetType = TargetType.Sphere;

        spell.EffectDescription.EffectAdvancement.additionalDicePerIncrement = 1;
        spell.EffectDescription.EffectAdvancement.incrementMultiplier = 1;
        spell.EffectDescription.EffectAdvancement.effectIncrementMethod = EffectIncrementMethod.PerAdditionalSlotLevel;

        spell.EffectDescription.EffectForms[0].SummonForm.effectProxyDefinitionName = ProxyPetalStormName;

        spell.EffectDescription.EffectForms[1].hasSavingThrow = true;
        spell.EffectDescription.EffectForms[1].savingThrowAffinity = EffectSavingThrowType.HalfDamage;
        spell.EffectDescription.EffectForms[1].DamageForm.diceNumber = 4;
        spell.EffectDescription.EffectForms[1].DamageForm.dieType = DieType.D6;
        spell.EffectDescription.EffectForms[1].DamageForm.damageType = DamageTypePsychic;
        spell.EffectDescription.EffectForms[1].levelMultiplier = 1;

        return spell;
    }

    //
    // LEVEL 03
    //

    internal static SpellDefinition BuildEarthTremor()
    {
        const string NAME = "EarthTremor";

        var spriteReference =
            CustomIcons.CreateAssetReferenceSprite(NAME, Resources.EarthTremor, 128, 128);

        //var rubbleProxy = EffectProxyDefinitionBuilder
        //    .Create(EffectProxyDefinitions.ProxyGrease, "RubbleProxy", "")
        //    .SetGuiPresentation(Category.EffectProxy, spriteReference)
        //    .AddToDB();

        var effectDescription = EffectDescriptionBuilder
            .Create()
            .SetEffectAdvancement(
                EffectIncrementMethod.PerAdditionalSlotLevel, 1, 0, 1)
            .SetSavingThrowData(true, true, AttributeDefinitions.Dexterity, false,
                EffectDifficultyClassComputation.AbilityScoreAndProficiency, AttributeDefinitions.Wisdom, 12)
            .SetDurationData(DurationType.Minute, 10)
            .SetParticleEffectParameters(Grease)
            .SetTargetingData(Side.All, RangeType.Distance, 24, TargetType.Cylinder)
            .AddEffectForm(
                EffectFormBuilder
                    .Create()
                    .SetMotionForm(MotionForm.MotionType.FallProne, 1)
                    .CreatedByCharacter()
                    .HasSavingThrow(EffectSavingThrowType.Negates).Build())
            .AddEffectForm(
                EffectFormBuilder
                    .Create()
                    .SetDamageForm(false, DieType.D1, DamageTypeBludgeoning, 0, DieType.D12, 3)
                    .HasSavingThrow(EffectSavingThrowType.HalfDamage).Build()
            ).Build();

        effectDescription.EffectForms.AddRange(
            Grease.EffectDescription.EffectForms.Find(e =>
                e.formType == EffectFormType.Topology));

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

        var spriteReference =
            CustomIcons.CreateAssetReferenceSprite(NAME, Resources.WinterBreath, 128, 128);

        var effectDescription = EffectDescriptionBuilder
            .Create()
            .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, 1, 0, 1)
            .SetSavingThrowData(true, true, AttributeDefinitions.Dexterity, false,
                EffectDifficultyClassComputation.AbilityScoreAndProficiency, AttributeDefinitions.Wisdom, 12)
            .SetDurationData(DurationType.Minute, 1)
            .SetParticleEffectParameters(ConeOfCold)
            .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Cone, 3, 2)
            .AddEffectForm(
                EffectFormBuilder
                    .Create()
                    .SetMotionForm(MotionForm.MotionType.FallProne, 1)
                    .HasSavingThrow(EffectSavingThrowType.Negates).Build())
            .AddEffectForm(
                EffectFormBuilder
                    .Create()
                    .SetDamageForm(damageType: DamageTypeCold, dieType: DieType.D8, diceNumber: 4)
                    .HasSavingThrow(EffectSavingThrowType.HalfDamage).Build()
            ).Build();

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

    //
    // LEVEL 07
    //

    internal static SpellDefinition BuildReverseGravity()
    {
        const string ReverseGravityName = "ReverseGravity";

        var effectDescription = EffectDescriptionBuilder.Create()
            .SetDurationData(
                DurationType.Minute,
                1)
            .SetTargetingData(
                Side.All,
                RangeType.Distance,
                12,
                TargetType.Cylinder,
                10,
                10)
            .SetSavingThrowData(
                true,
                false,
                AttributeDefinitions.Dexterity,
                true,
                EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                AttributeDefinitions.Dexterity,
                20)
            .AddEffectForm(EffectFormBuilder
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
                .Build())
            .AddEffectForm(EffectFormBuilder
                .Create()
                .SetMotionForm(
                    MotionForm.MotionType.Levitate,
                    10)
                .HasSavingThrow(EffectSavingThrowType.Negates)
                .Build())
            .SetRecurrentEffect(Entangle.EffectDescription.RecurrentEffect);

        return SpellDefinitionBuilder
            .Create(ReverseGravityName)
            .SetGuiPresentation(Category.Spell, Thunderwave.GuiPresentation.SpriteReference)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(7)
            .SetCastingTime(ActivationTime.Action)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetEffectDescription(effectDescription.Build())
            .SetAiParameters(new SpellAIParameters())
            .SetRequiresConcentration(true)
            .AddToDB();
    }

    //
    // LEVEL 08
    //

    internal static SpellDefinition BuildMindBlank()
    {
        var effectDescription = EffectDescriptionBuilder.Create();
        effectDescription.SetDurationData(
            DurationType.Hour,
            24
        );
        effectDescription.SetTargetingData(
            Side.Ally,
            RangeType.Touch,
            1,
            TargetType.Individuals
        );
        effectDescription.AddEffectForm(
            EffectFormBuilder
                .Create()
                .SetConditionForm(
                    ConditionDefinitionBuilder
                        .Create(ConditionBearsEndurance, "ConditionMindBlank")
                        .SetOrUpdateGuiPresentation(Category.Condition)
                        .SetFeatures(
                            FeatureDefinitionConditionAffinitys.ConditionAffinityCharmImmunity,
                            FeatureDefinitionConditionAffinitys.ConditionAffinityCharmImmunityHypnoticPattern,
                            FeatureDefinitionConditionAffinitys.ConditionAffinityCalmEmotionCharmedImmunity,
                            FeatureDefinitionDamageAffinitys.DamageAffinityPsychicImmunity
                        )
                        .AddToDB(),
                    ConditionForm.ConditionOperation.Add,
                    false,
                    false)
                .Build()
        );

        return SpellDefinitionBuilder
            .Create("MindBlank")
            .SetGuiPresentation(Category.Spell, MindTwist.GuiPresentation.SpriteReference)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(8)
            .SetCastingTime(ActivationTime.Action)
            .SetVerboseComponent(true)
            .SetSomaticComponent(false)
            .SetEffectDescription(effectDescription.Build())
            .SetAiParameters(new SpellAIParameters())
            .AddToDB();
    }

    //
    // LEVEL 09
    //

    internal static SpellDefinition BuildForesight()
    {
        var effectDescription = EffectDescriptionBuilder.Create()
            .SetDurationData(
                DurationType.Hour,
                8)
            .SetTargetingData(
                Side.Ally,
                RangeType.Touch,
                1,
                TargetType.Individuals)
            .AddEffectForm(EffectFormBuilder
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
                .Build());

        return SpellDefinitionBuilder
            .Create("Foresight")
            .SetGuiPresentation(Category.Spell, TrueSeeing.GuiPresentation.SpriteReference)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(9)
            .SetCastingTime(ActivationTime.Minute1)
            .SetVerboseComponent(true)
            .SetSomaticComponent(false)
            .SetEffectDescription(effectDescription.Build())
            .SetAiParameters(new SpellAIParameters())
            .AddToDB();
    }

    internal static SpellDefinition BuildMassHeal()
    {
        var effectDescription = EffectDescriptionBuilder.Create()
            .SetDurationData(DurationType.Instantaneous)
            .SetTargetingData(
                Side.All,
                RangeType.Distance,
                12,
                TargetType.Individuals,
                6)
            .AddEffectForm(EffectFormBuilder
                .Create()
                .SetHealingForm(
                    HealingComputation.Dice,
                    120,
                    DieType.D1,
                    0,
                    false,
                    HealingCap.MaximumHitPoints)
                .Build());

        return SpellDefinitionBuilder
            .Create("MassHeal")
            .SetGuiPresentation(Category.Spell, Heal.GuiPresentation.SpriteReference)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(9)
            .SetCastingTime(ActivationTime.Action)
            .SetVerboseComponent(true)
            .SetSomaticComponent(false)
            .SetEffectDescription(effectDescription.Build())
            .SetAiParameters(new SpellAIParameters())
            .AddToDB();
    }

    internal static SpellDefinition BuildMeteorSwarmSingleTarget()
    {
        var effectDescription = EffectDescriptionBuilder.Create()
            .SetDurationData(DurationType.Instantaneous)
            .SetTargetingData(
                Side.All,
                RangeType.Distance,
                200,
                TargetType.Sphere,
                8,
                8)
            .AddEffectForm(
                EffectFormBuilder
                    .Create()
                    .SetDamageForm(
                        false,
                        DieType.D6,
                        DamageTypeFire,
                        0,
                        DieType.D6,
                        20) // 20 because hits dont stack even on single target
                    .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                    .Build())
            .AddEffectForm(EffectFormBuilder
                .Create()
                .SetDamageForm(
                    false,
                    DieType.D6,
                    DamageTypeBludgeoning,
                    0,
                    DieType.D6,
                    20) // 20 because hits dont stack even on single target)
                .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                .Build())
            .SetSavingThrowData(
                true,
                false,
                AttributeDefinitions.Dexterity,
                true,
                EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                AttributeDefinitions.Dexterity,
                20)
            .SetParticleEffectParameters(FlameStrike);

        return SpellDefinitionBuilder
            .Create("MeteorSwarmSingleTarget")
            .SetGuiPresentation(Category.Spell, FlamingSphere.GuiPresentation.SpriteReference)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(9)
            .SetCastingTime(ActivationTime.Action)
            .SetVerboseComponent(true)
            .SetSomaticComponent(false)
            .SetEffectDescription(effectDescription.Build())
            .SetAiParameters(new SpellAIParameters())
            .AddToDB();
    }

    internal static SpellDefinition BuildPowerWordHeal()
    {
        var effectDescription = EffectDescriptionBuilder.Create()
            .SetDurationData(DurationType.Instantaneous)
            .SetTargetingData(
                Side.Ally,
                RangeType.Distance,
                12,
                TargetType.Individuals)
            .AddEffectForm(EffectFormBuilder
                .Create()
                .SetHealingForm(
                    HealingComputation.Dice,
                    700,
                    DieType.D1,
                    0,
                    false,
                    HealingCap.MaximumHitPoints)
                .Build())
            .AddEffectForm(EffectFormBuilder
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
                .Build());

        return SpellDefinitionBuilder
            .Create("PowerWordHeal")
            .SetGuiPresentation(Category.Spell, HealingWord.GuiPresentation.SpriteReference)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(9)
            .SetCastingTime(ActivationTime.Action)
            .SetVerboseComponent(true)
            .SetSomaticComponent(false)
            .SetEffectDescription(effectDescription.Build())
            .SetAiParameters(new SpellAIParameters())
            .AddToDB();
    }

    internal static SpellDefinition BuildPowerWordKill()
    {
        var effectDescription = EffectDescriptionBuilder.Create()
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
                    .Build());

        return SpellDefinitionBuilder
            .Create("PowerWordKill")
            .SetGuiPresentation(Category.Spell, Disintegrate.GuiPresentation.SpriteReference)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(9)
            .SetCastingTime(ActivationTime.Action)
            .SetVerboseComponent(true)
            .SetSomaticComponent(false)
            .SetEffectDescription(effectDescription.Build())
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
            .AddEffectForm(
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
                            new ShapeOptionDescription()
                                .SetRequiredLevel(1)
                                .SetSubstituteMonster(GoldDragon_AerElai),
                            new ShapeOptionDescription()
                                .SetRequiredLevel(1)
                                .SetSubstituteMonster(Divine_Avatar),
                            new ShapeOptionDescription()
                                .SetRequiredLevel(1)
                                .SetSubstituteMonster(Sorr_Akkath_Tshar_Boss),
                            new ShapeOptionDescription()
                                .SetRequiredLevel(1)
                                .SetSubstituteMonster(GreenDragon_MasterOfConjuration),
                            new ShapeOptionDescription()
                                .SetRequiredLevel(1)
                                .SetSubstituteMonster(BlackDragon_MasterOfNecromancy),
                            new ShapeOptionDescription()
                                .SetRequiredLevel(1)
                                .SetSubstituteMonster(Remorhaz),
                            new ShapeOptionDescription()
                                .SetRequiredLevel(1)
                                .SetSubstituteMonster(Emperor_Laethar),
                            new ShapeOptionDescription()
                                .SetRequiredLevel(1)
                                .SetSubstituteMonster(Giant_Ape),
                            new ShapeOptionDescription()
                                .SetRequiredLevel(1)
                                .SetSubstituteMonster(Spider_Queen),
                            new ShapeOptionDescription()
                                .SetRequiredLevel(1)
                                .SetSubstituteMonster(Sorr_Akkath_Shikkath)
                        }
                    }
                });

        return SpellDefinitionBuilder
            .Create("Shapechange")
            .SetGuiPresentation(Category.Spell, PowerDruidWildShape.GuiPresentation.SpriteReference)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(9)
            .SetCastingTime(ActivationTime.Action)
            .SetVerboseComponent(true)
            .SetSomaticComponent(false)
            .SetEffectDescription(effectDescription.Build())
            .SetAiParameters(new SpellAIParameters())
            .SetRequiresConcentration(true)
            .AddToDB();
    }

    internal static SpellDefinition BuildTimeStop()
    {
        var effectDescription = EffectDescriptionBuilder.Create()
            .SetDurationData(
                DurationType.Round,
                3)
            .SetTargetingData(
                Side.All,
                RangeType.Self,
                0,
                TargetType.Cylinder,
                20,
                10)
            .AddEffectForm(EffectFormBuilder
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
            .ExcludeCaster();

        return SpellDefinitionBuilder
            .Create("TimeStop")
            .SetGuiPresentation(Category.Spell, PowerDomainLawWordOfLaw.GuiPresentation.SpriteReference)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(9)
            .SetCastingTime(ActivationTime.Action)
            .SetVerboseComponent(true)
            .SetSomaticComponent(false)
            .SetEffectDescription(effectDescription.Build())
            .SetAiParameters(new SpellAIParameters())
            .AddToDB();
    }

    internal static SpellDefinition BuildWeird()
    {
        var effectDescription = EffectDescriptionBuilder.Create()
            .SetDurationData(
                DurationType.Minute,
                1)
            .SetTargetingData(
                Side.Enemy,
                RangeType.Distance,
                12,
                TargetType.Sphere,
                6,
                6)
            .SetSavingThrowData(
                true,
                false,
                AttributeDefinitions.Wisdom,
                true,
                EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                AttributeDefinitions.Constitution,
                20)
            .AddEffectForm(EffectFormBuilder
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
                .Build());

        return SpellDefinitionBuilder
            .Create("Weird")
            .SetGuiPresentation(Category.Spell, PhantasmalKiller.GuiPresentation.SpriteReference)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(9)
            .SetCastingTime(ActivationTime.Action)
            .SetVerboseComponent(true)
            .SetSomaticComponent(false)
            .SetEffectDescription(effectDescription.Build())
            .SetAiParameters(new SpellAIParameters())
            .SetRequiresConcentration(true)
            .AddToDB();
    }
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
    public CharacterActionMagicEffect GetNextMagicEffect([CanBeNull] CharacterActionMagicEffect baseEffect,
        CharacterActionAttack triggeredAttack, RollOutcome attackOutcome)
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
