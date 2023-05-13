using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomUI;
using UnityEngine;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static RuleDefinitions;
using Resources = SolastaUnfinishedBusiness.Properties.Resources;

namespace SolastaUnfinishedBusiness.Spells;

internal static partial class SpellBuilders
{
    #region CANTRIPS

    private static ConditionDefinition _acidClawCondition;
    internal static ConditionDefinition AcidClawCondition => _acidClawCondition ??= BuildAcidClawCondition();

    private static ConditionDefinition BuildAcidClawCondition()
    {
        return ConditionDefinitionBuilder
            .Create("ConditionAcidClaws")
            .SetGuiPresentation(Category.Condition, ConditionAcidSpit)
            .SetConditionType(ConditionType.Detrimental)
            .SetSpecialDuration(DurationType.Round, 1)
            .SetFeatures(FeatureDefinitionAttributeModifierBuilder
                .Create("AttributeModifierAcidClawsACDebuff")
                .SetGuiPresentation("ConditionAcidClaws", Category.Condition)
                .SetModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
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
            .SetDurationData(DurationType.Instantaneous)
            .SetTargetingData(Side.Enemy, RangeType.MeleeHit, 1, TargetType.Individuals)
            .SetEffectForms(
                EffectFormBuilder
                    .Create()
                    .SetDamageForm(DamageTypeAcid, 1, DieType.D8)
                    .Build(),
                EffectFormBuilder
                    .Create()
                    .SetConditionForm(AcidClawCondition, ConditionForm.ConditionOperation.Add)
                    .Build())
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
            .SetDurationData(DurationType.Instantaneous)
            .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.Individuals)
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

    internal static SpellDefinition BuildBladeWard()
    {
        const string NAME = "BladeWard";

        var spriteReference = Sprites.GetSprite(NAME, Resources.BladeWard, 128, 128);

        var effectDescription = EffectDescriptionBuilder
            .Create()
            .SetDurationData(DurationType.Round, 1)
            .SetTargetingData(Side.Ally, RangeType.Touch, 0, TargetType.Self)
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

    internal static SpellDefinition BuildBurstOfRadiance()
    {
        const string NAME = "BurstOfRadiance";

        var spriteReference = Sprites.GetSprite(NAME, Resources.BurstOfRadiance, 128, 128);

        var effectDescription = EffectDescriptionBuilder
            .Create()
            .SetEffectAdvancement(EffectIncrementMethod.CasterLevelTable, 1, 0, 1)
            .SetSavingThrowData(
                false,
                AttributeDefinitions.Constitution,
                false,
                EffectDifficultyClassComputation.SpellCastingFeature,
                AttributeDefinitions.Wisdom,
                12)
            .SetDurationData(DurationType.Instantaneous)
            .SetParticleEffectParameters(BurningHands)
            .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Cube, 3)
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
            .SetSomaticComponent(false)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .AddToDB();

        return spell;
    }

    internal static SpellDefinition BuildEnduringSting()
    {
        const string NAME = "EnduringSting";

        var spriteReference = Sprites.GetSprite(NAME, Resources.EnduringSting, 128, 128);

        var effectDescription = EffectDescriptionBuilder
            .Create()
            .SetDurationData(DurationType.Instantaneous)
            .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.Individuals)
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

    internal static SpellDefinition BuildIlluminatingSphere()
    {
        const string NAME = "IlluminatingSphere";

        var spell = SpellDefinitionBuilder
            .Create(Sparkle, NAME)
            .SetGuiPresentation(Category.Spell, Shine)
            .SetVocalSpellSameType(VocalSpellSemeType.Detection)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create(Sparkle.EffectDescription)
                .SetTargetingData(Side.All, RangeType.Distance, 18, TargetType.Sphere, 6)
                .SetParticleEffectParameters(SacredFlame_B.EffectDescription.EffectParticleParameters)
                .Build())
            .AddToDB();

        return spell;
    }

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
            .SetDurationData(DurationType.Instantaneous)
            .SetTargetingData(Side.Enemy, RangeType.Distance, 12, TargetType.Individuals)
            .SetParticleEffectParameters(Bane)
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
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.Enemy, RangeType.RangeHit, 12, TargetType.Individuals)
                .AddImmuneCreatureFamilies(CharacterFamilyDefinitions.Construct, CharacterFamilyDefinitions.Undead)
                .SetDurationData(DurationType.Hour, 1)
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
        var resonanceLeap = SpellDefinitionBuilder
            .Create("ResonatingStrikeLeap")
            .SetGuiPresentationNoContent()
            .SetSpellLevel(1)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetSomaticComponent(false)
            .SetVerboseComponent(false)
            .SetCustomSubFeatures(new BonusSlotLevelsByClassLevel())
            .SetCastingTime(ActivationTime.Action)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetTargetFiltering(TargetFilteringMethod.CharacterOnly)
                .SetTargetingData(Side.Enemy, RangeType.Touch, 1, TargetType.Individuals)
                .SetParticleEffectParameters(Shatter)
                .SetEffectForms(EffectFormBuilder.Create()
                    .SetBonusMode(AddBonusMode.AbilityBonus)
                    .SetDamageForm(DamageTypeThunder, 0, DieType.D8)
                    .Build())
                .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, additionalDicePerIncrement: 1)
                .Build())
            .AddToDB();

        var additionalDamageResonatingStrike = FeatureDefinitionAdditionalDamageBuilder
            .Create("AdditionalDamageResonatingStrike")
            .SetGuiPresentationNoContent(true)
            .SetNotificationTag("ResonatingStrike")
            .SetRequiredProperty(RestrictedContextRequiredProperty.MeleeWeapon)
            .SetDamageDice(DieType.D8, 0)
            .SetSpecificDamageType(DamageTypeThunder)
            .SetAdvancement(ExtraAdditionalDamageAdvancement.CharacterLevel, 1, 1, 6, 5)
            .SetImpactParticleReference(Shatter.EffectDescription.EffectParticleParameters.impactParticleReference)
            .SetAttackModeOnly()
            .AddToDB();

        return SpellDefinitionBuilder
            .Create("ResonatingStrike")
            .SetGuiPresentation(Category.Spell,
                Sprites.GetSprite("ResonatingStrike", Resources.ResonatingStrike, 128, 128))
            .SetSpellLevel(0)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetVerboseComponent(false)
            .SetMaterialComponent(MaterialComponentType.Specific)
            .SetSpecificMaterialComponent(TagsDefinitions.WeaponTagMelee, 0, false)
            .SetCustomSubFeatures(
                AttackAfterMagicEffect.MeleeAttack,
                CustomSpellEffectLevel.ByCasterLevel,
                new ChainSpellEffectOnAttackHit(resonanceLeap, "ResonatingStrike")
            )
            .SetCastingTime(ActivationTime.Action)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetParticleEffectParameters(Shatter)
                .SetTargetProximityData(true, 1)
                .SetTargetingData(Side.Enemy, RangeType.Distance, 5, TargetType.IndividualsUnique, 2)
                .SetIgnoreCover()
                .SetEffectAdvancement(EffectIncrementMethod.CasterLevelTable, additionalDicePerIncrement: 1,
                    incrementMultiplier: 1)
                .SetDurationData(DurationType.Round, 1)
                .SetEffectForms(EffectFormBuilder.Create()
                    .HasSavingThrow(EffectSavingThrowType.None)
                    .SetConditionForm(ConditionDefinitionBuilder
                            .Create("ConditionResonatingStrike")
                            .SetGuiPresentation(Category.Condition)
                            .SetSpecialInterruptions(ConditionInterruption.Attacks)
                            .SetSilent(Silent.WhenAddedOrRemoved)
                            .SetFeatures(additionalDamageResonatingStrike)
                            .AddToDB(),
                        ConditionForm.ConditionOperation.Add,
                        true)
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
                .SetSpecialDuration(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
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
            .SetSpecialDuration(DurationType.Round, 1)
            .AddToDB();

        return SpellDefinitionBuilder
            .Create("SunlightBlade")
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite("SunlightBlade", Resources.SunlightBlade, 128, 128))
            .SetSpellLevel(0)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetVerboseComponent(false)
            .SetMaterialComponent(MaterialComponentType.Specific)
            .SetSpecificMaterialComponent(TagsDefinitions.WeaponTagMelee, 0, false)
            .SetCustomSubFeatures(
                AttackAfterMagicEffect.MeleeAttackCanTwin,
                new UpgradeRangeBasedOnWeaponReach())
            .SetCastingTime(ActivationTime.Action)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetParticleEffectParameters(ScorchingRay)
                .SetTargetingData(Side.Enemy, RangeType.Touch, 1, TargetType.Individuals)
                .SetIgnoreCover()
                .SetEffectAdvancement( //this is needed for tooltip
                    EffectIncrementMethod.CasterLevelTable,
                    additionalDicePerIncrement: 1,
                    incrementMultiplier: 1)
                .SetDurationData(DurationType.Round, 1)
                .SetEffectForms(EffectFormBuilder.Create()
                        .HasSavingThrow(EffectSavingThrowType.None)
                        .SetConditionForm(
                            ConditionDefinitionBuilder
                                .Create("ConditionSunlightBlade")
                                .SetGuiPresentation(Category.Condition)
                                .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
                                .SetSilent(Silent.WhenAddedOrRemoved)
                                .SetFeatures(FeatureDefinitionAdditionalDamageBuilder
                                    .Create("AdditionalDamageSunlightBlade")
                                    .SetGuiPresentationNoContent(true)
                                    .SetNotificationTag("SunlightBlade")
                                    .SetRequiredProperty(RestrictedContextRequiredProperty.MeleeWeapon)
                                    .SetAttackModeOnly()
                                    .SetDamageDice(DieType.D8, 1)
                                    .SetSpecificDamageType(DamageTypeRadiant)
                                    .SetAdvancement(ExtraAdditionalDamageAdvancement.CharacterLevel, 1, 1, 6, 5)
                                    .SetTargetCondition(sunlitMark, AdditionalDamageTriggerCondition.TargetHasCondition)
                                    .SetConditionOperations(highlight)
                                    .SetAddLightSource(true)
                                    .SetLightSourceForm(dimLight)
                                    .AddToDB())
                                .AddToDB(),
                            ConditionForm.ConditionOperation.Add,
                            true)
                        .Build(),
                    EffectFormBuilder
                        .Create()
                        .SetConditionForm(sunlitMark, ConditionForm.ConditionOperation.Add)
                        .Build())
                .Build())
            .AddToDB();
    }

    internal static SpellDefinition BuildSwordStorm()
    {
        const string NAME = "SwordStorm";

        var spriteReference = Sprites.GetSprite(NAME, Resources.SwordStorm, 128, 128);

        var effectDescription = EffectDescriptionBuilder
            .Create()
            .SetEffectAdvancement(EffectIncrementMethod.CasterLevelTable, 5, additionalDicePerIncrement: 1)
            .SetDurationData(DurationType.Instantaneous)
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
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.Enemy, RangeType.RangeHit, 6, TargetType.Individuals)
                .SetParticleEffectParameters(VenomousSpike)
                .SetEffectAdvancement(EffectIncrementMethod.CasterLevelTable, 5,
                    additionalDicePerIncrement: 1)
                .SetEffectForms(
                    EffectFormBuilder
                        .Create()
                        .SetDamageForm(DamageTypePiercing, 1, DieType.D6)
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
                false,
                AttributeDefinitions.Constitution,
                false,
                EffectDifficultyClassComputation.SpellCastingFeature,
                AttributeDefinitions.Wisdom,
                12)
            .SetDurationData(DurationType.Instantaneous)
            .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Cube, 3)
            .ExcludeCaster()
            .SetEffectForms(
                EffectFormBuilder
                    .Create()
                    .SetDamageForm(DamageTypeThunder, 1, DieType.D6)
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
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .AddToDB();

        return spell;
    }

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
            .SetTargetingData(Side.Enemy, RangeType.Distance, 12, TargetType.Individuals)
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

    internal static SpellDefinition BuildTollTheDead()
    {
        const string NAME = "TollTheDead";

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Bane.GuiPresentation.SpriteReference)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.Enemy, RangeType.Distance, 12, TargetType.Individuals)
                .SetDurationData(DurationType.Instantaneous)
                .SetEffectAdvancement(EffectIncrementMethod.CasterLevelTable, additionalDicePerIncrement: 1)
                .SetSavingThrowData(
                    false,
                    AttributeDefinitions.Wisdom,
                    true,
                    EffectDifficultyClassComputation.SpellCastingFeature)
                .SetParticleEffectParameters(Bane.EffectDescription.EffectParticleParameters)
                .SetEffectForms(
                    EffectFormBuilder
                        .Create()
                        .SetDamageForm(DamageTypeNecrotic, 1, DieType.D12)
                        .HasSavingThrow(EffectSavingThrowType.Negates)
                        .Build())
                .Build())
            .SetCastingTime(ActivationTime.Action)
            .SetSpellLevel(0)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolNecromancy)
            .AddToDB();

        return spell;
    }

    #endregion
}
