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
using SolastaUnfinishedBusiness.Properties;
using UnityEngine.AddressableAssets;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaUnfinishedBusiness.Spells;

internal static partial class SpellBuilders
{
    #region Blinding Smite

    internal static SpellDefinition BuildBlindingSmite()
    {
        const string NAME = "BlindingSmite";

        var additionalDamageBlindingSmite = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{NAME}")
            .SetGuiPresentation(NAME, Category.Spell)
            .SetNotificationTag(NAME)
            .AddCustomSubFeatures(ValidatorsRestrictedContext.IsWeaponAttack)
            .SetDamageDice(DieType.D8, 3)
            .SetSpecificDamageType(DamageTypeRadiant)
            .SetSavingThrowData(EffectDifficultyClassComputation.SpellCastingFeature, EffectSavingThrowType.None)
            .SetConditionOperations(
                new ConditionOperationDescription
                {
                    hasSavingThrow = true,
                    canSaveToCancel = true,
                    saveAffinity = EffectSavingThrowType.Negates,
                    saveOccurence = TurnOccurenceType.StartOfTurn,
                    conditionDefinition = ConditionDefinitionBuilder
                        .Create(ConditionDefinitions.ConditionBlinded, $"Condition{NAME}Enemy")
                        .SetSpecialDuration(DurationType.Minute, 1, TurnOccurenceType.StartOfTurn)
                        .AddToDB(),
                    operation = ConditionOperationDescription.ConditionOperation.Add
                })
            // doesn't follow the standard impact particle reference
            .SetImpactParticleReference(DivineFavor.EffectDescription.EffectParticleParameters.casterParticleReference)
            .AddToDB();

        var conditionBlindingSmite = ConditionDefinitionBuilder
            .Create($"Condition{NAME}")
            .SetGuiPresentation(NAME, Category.Spell, ConditionBrandingSmite)
            .SetPossessive()
            .SetFeatures(additionalDamageBlindingSmite)
            .SetSpecialInterruptions(ConditionInterruption.AttacksAndDamages)
            .AddToDB();

        var spell = SpellDefinitionBuilder
            .Create(BrandingSmite, NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.BlindingSmite, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetSpellLevel(3)
            .SetCastingTime(ActivationTime.BonusAction)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetSomaticComponent(false)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    // .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, additionalDicePerIncrement: 1)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionBlindingSmite))
                    .SetParticleEffectParameters(DivineFavor)
                    .Build())
            .AddToDB();

        return spell;
    }

    #endregion

    #region Winter Breath

    internal static SpellDefinition BuildWinterBreath()
    {
        const string NAME = "WinterBreath";

        var spriteReference = Sprites.GetSprite(NAME, Resources.WinterBreath, 128, 128);

        var effectDescription = EffectDescriptionBuilder
            .Create()
            .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Cone, 3)
            .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, 1, 0, 1)
            .SetSavingThrowData(
                false,
                AttributeDefinitions.Dexterity,
                false,
                EffectDifficultyClassComputation.SpellCastingFeature,
                AttributeDefinitions.Wisdom,
                12)
            .SetParticleEffectParameters(ConeOfCold)
            .ExcludeCaster()
            .SetEffectForms(
                EffectFormBuilder
                    .Create()
                    .SetMotionForm(MotionForm.MotionType.FallProne, 1)
                    .HasSavingThrow(EffectSavingThrowType.Negates)
                    .Build(),
                EffectFormBuilder
                    .Create()
                    .SetDamageForm(DamageTypeCold, 4, DieType.D8)
                    .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                    .Build())
            .Build();

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, spriteReference)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolConjuration)
            .SetSpellLevel(3)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetSomaticComponent(true)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetEffectDescription(effectDescription)
            .AddToDB();

        return spell;
    }

    #endregion

    #region Crusaders Mantle

    internal static SpellDefinition BuildCrusadersMantle()
    {
        const string NAME = "CrusadersMantle";

        var conditionCrusadersMantle = ConditionDefinitionBuilder
            .Create($"Condition{NAME}")
            .SetGuiPresentation(Category.Condition, ConditionDivineFavor)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddFeatures(FeatureDefinitionAdditionalDamages.AdditionalDamageDivineFavor)
            .AddToDB();

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell,
                Sprites.GetSprite("CrusadersMantle", Resources.CrusadersMantle, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetSpellLevel(3)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetSomaticComponent(false)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
            .SetRequiresConcentration(true)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetFiltering(TargetFilteringMethod.CharacterOnly)
                    .SetTargetingData(Side.Ally, RangeType.Self, 1, TargetType.Sphere, 6)
                    .SetDurationData(DurationType.Minute, 1)
                    .SetParticleEffectParameters(DivineFavor)
                    .SetRecurrentEffect(RecurrentEffect.OnActivation |
                                        RecurrentEffect.OnTurnStart |
                                        RecurrentEffect.OnEnter)
                    .AddEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionCrusadersMantle, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();

        return spell;
    }

    #endregion

    #region Pulse Wave

    internal static SpellDefinition BuildPulseWave()
    {
        const string NAME = "PulseWave";

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.PulseWave, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetSpellLevel(3)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetSomaticComponent(true)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Cone, 6)
                    .ExcludeCaster()
                    .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, 1, 0, 1)
                    .SetSavingThrowData(
                        false,
                        AttributeDefinitions.Constitution,
                        true,
                        EffectDifficultyClassComputation.SpellCastingFeature,
                        AttributeDefinitions.Wisdom,
                        12)
                    .AddEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetMotionForm(MotionForm.MotionType.PushFromOrigin, 3)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build())
                    .AddEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeForce, dieType: DieType.D6, diceNumber: 6)
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .Build())
                    .SetParticleEffectParameters(PowerFunctionWandFearCone)
                    .Build())
            .AddToDB();

        spell.EffectDescription.EffectParticleParameters.casterParticleReference =
            Darkness.EffectDescription.EffectParticleParameters.casterParticleReference;

        spell.EffectDescription.EffectParticleParameters.impactParticleReference =
            MindTwist.EffectDescription.EffectParticleParameters.impactParticleReference;

        return spell;
    }

    #endregion

    #region Adder's Fangs

    internal static SpellDefinition BuildAdderFangs()
    {
        const string Name = "AdderFangs";

        var movementAffinityAdderFangs = FeatureDefinitionMovementAffinityBuilder
            .Create($"MovementAffinity{Name}")
            .SetGuiPresentationNoContent(true)
            .SetBaseSpeedMultiplicativeModifier(0.5f)
            .AddToDB();

        var conditionAdderFangs = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionPoisoned, $"Condition{Name}")
            .SetPossessive()
            .AddFeatures(movementAffinityAdderFangs)
            .AddToDB();

        conditionAdderFangs.GuiPresentation.Description = Gui.NoLocalization;

        var spell = SpellDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(Name, Resources.AdderFangs, 128, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolConjuration)
            .SetSpellLevel(3)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 24, TargetType.IndividualsUnique)
                    .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel,
                        additionalTargetsPerIncrement: 1)
                    .SetSavingThrowData(false, AttributeDefinitions.Constitution, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .SetDamageForm(DamageTypePoison, 4, DieType.D10)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates, TurnOccurenceType.EndOfTurn, true)
                            .SetConditionForm(conditionAdderFangs, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .SetParticleEffectParameters(VenomousSpike)
                    .Build())
            .AddToDB();

        spell.EffectDescription.EffectParticleParameters.effectParticleReference =
            InflictWounds.EffectDescription.EffectParticleParameters.effectParticleReference;

        return spell;
    }

    #endregion

    #region Elemental Weapon

    internal static SpellDefinition BuildElementalWeapon()
    {
        const string NAME = "ElementalWeapon";

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite("ElementalWeapon", Resources.ElementalWeapon, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(3)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetSomaticComponent(true)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
            .SetRequiresConcentration(true)
            .SetUniqueInstance()
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, additionalDicePerIncrement: 1)
                    .Build())
            .SetSubSpells(DamagesAndEffects
                .Where(x => x.Item1 != DamageTypePoison)
                .Select(x => BuildElementalWeaponSubspell(x.Item1, x.Item2)).ToArray())
            .AddToDB();

        return spell;
    }

    private static SpellDefinition BuildElementalWeaponSubspell(string damageType, IMagicEffect magicEffect)
    {
        var effectParticleParameters = magicEffect.EffectDescription.EffectParticleParameters;

        const string NOTIFICATION_TAG = "ElementalWeapon";

        const string ELEMENTAL_WEAPON_ADDITIONAL_DESCRIPTION = "Feature/&AdditionalDamageElementalWeaponDescription";

        const string ELEMENTAL_WEAPON_ADDITIONAL_DESCRIPTION1 = "Feature/&AdditionalDamageElementalWeapon1Description";

        const string ELEMENTAL_WEAPON_ADDITIONAL_DESCRIPTION2 = "Feature/&AdditionalDamageElementalWeapon2Description";

        const string ELEMENTAL_WEAPON_MODIFIER_DESCRIPTION = "Feature/&AttackModifierElementalWeaponDescription";

        static string AdditionalDamageElementalWeaponDescription(string x)
        {
            return Gui.Format(ELEMENTAL_WEAPON_ADDITIONAL_DESCRIPTION, x);
        }

        static string AdditionalDamageElementalWeaponDescription1(string x)
        {
            return Gui.Format(ELEMENTAL_WEAPON_ADDITIONAL_DESCRIPTION1, x);
        }

        static string AdditionalDamageElementalWeaponDescription2(string x)
        {
            return Gui.Format(ELEMENTAL_WEAPON_ADDITIONAL_DESCRIPTION2, x);
        }

        static string AttackModifierElementalWeaponDescription(int x)
        {
            return Gui.Format(ELEMENTAL_WEAPON_MODIFIER_DESCRIPTION, x.ToString());
        }

        var additionalDamageElementalWeapon = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{damageType}ElementalWeapon")
            .SetGuiPresentation("AdditionalDamageElementalWeapon", Category.Feature,
                AdditionalDamageElementalWeaponDescription(damageType), MagicWeapon.guiPresentation.SpriteReference)
            .SetAdditionalDamageType(AdditionalDamageType.Specific)
            .SetSpecificDamageType(damageType)
            .SetAttackModeOnly()
            .SetDamageDice(DieType.D4, 1)
            .SetAdvancement(AdditionalDamageAdvancement.SlotLevel)
            .SetNotificationTag(NOTIFICATION_TAG)
            .SetImpactParticleReference(effectParticleParameters.impactParticleReference)
            .AddToDB();

        var additionalDamageElementalWeapon1 = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{damageType}ElementalWeapon1")
            .SetGuiPresentation("AdditionalDamageElementalWeapon", Category.Feature,
                AdditionalDamageElementalWeaponDescription1(damageType), MagicWeapon.guiPresentation.SpriteReference)
            .SetAdditionalDamageType(AdditionalDamageType.Specific)
            .SetSpecificDamageType(damageType)
            .SetAttackModeOnly()
            .SetDamageDice(DieType.D4, 2)
            .SetAdvancement(AdditionalDamageAdvancement.SlotLevel)
            .SetNotificationTag(NOTIFICATION_TAG)
            .SetImpactParticleReference(effectParticleParameters.impactParticleReference)
            .AddToDB();

        var additionalDamageElementalWeapon2 = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{damageType}ElementalWeapon2")
            .SetGuiPresentation("AdditionalDamageElementalWeapon", Category.Feature,
                AdditionalDamageElementalWeaponDescription2(damageType), MagicWeapon.guiPresentation.SpriteReference)
            .SetAdditionalDamageType(AdditionalDamageType.Specific)
            .SetSpecificDamageType(damageType)
            .SetAttackModeOnly()
            .SetDamageDice(DieType.D4, 3)
            .SetAdvancement(AdditionalDamageAdvancement.SlotLevel)
            .SetNotificationTag(NOTIFICATION_TAG)
            .SetImpactParticleReference(effectParticleParameters.impactParticleReference)
            .AddToDB();

        var attackModifierElementalWeapon = FeatureDefinitionAttackModifierBuilder
            .Create(FeatureDefinitionAttackModifiers.AttackModifierMagicWeapon,
                $"AttackModifier{damageType}ElementalWeapon")
            .SetGuiPresentation("AttackModifierElementalWeapon", Category.Feature,
                AttackModifierElementalWeaponDescription(1), MagicWeapon.guiPresentation.SpriteReference)
            .AddToDB();

        var attackModifierElementalWeapon1 = FeatureDefinitionAttackModifierBuilder
            .Create(FeatureDefinitionAttackModifiers.AttackModifierMagicWeapon2,
                $"AttackModifier{damageType}ElementalWeapon1")
            .SetGuiPresentation("AttackModifierElementalWeapon", Category.Feature,
                AttackModifierElementalWeaponDescription(2), MagicWeapon.guiPresentation.SpriteReference)
            .AddToDB();

        var attackModifierElementalWeapon2 = FeatureDefinitionAttackModifierBuilder
            .Create(FeatureDefinitionAttackModifiers.AttackModifierMagicWeapon3,
                $"AttackModifier{damageType}ElementalWeapon2")
            .SetGuiPresentation("AttackModifierElementalWeapon", Category.Feature,
                AttackModifierElementalWeaponDescription(3), MagicWeapon.guiPresentation.SpriteReference)
            .AddToDB();

        var spell = SpellDefinitionBuilder
            .Create($"ElementalWeapon{damageType}")
            .SetGuiPresentation(Category.Spell, MagicWeapon)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(3)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetSomaticComponent(true)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
            .SetRequiresConcentration(true)
            .SetUniqueInstance()
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(MagicWeapon)
                    .SetDurationData(DurationType.Minute, 1)
                    .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, additionalDicePerIncrement: 1)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDiceAdvancement(LevelSourceType.EffectLevel)
                            .SetLevelAdvancement(EffectForm.LevelApplianceType.MultiplyDice,
                                LevelSourceType.EffectLevel)
                            .SetItemPropertyForm(ItemPropertyUsage.Unlimited, 1,
                                new FeatureUnlockByLevel(attackModifierElementalWeapon, 3),
                                new FeatureUnlockByLevel(attackModifierElementalWeapon, 4),
                                new FeatureUnlockByLevel(attackModifierElementalWeapon1, 5),
                                new FeatureUnlockByLevel(attackModifierElementalWeapon1, 6),
                                new FeatureUnlockByLevel(attackModifierElementalWeapon2, 7),
                                new FeatureUnlockByLevel(attackModifierElementalWeapon2, 8),
                                new FeatureUnlockByLevel(attackModifierElementalWeapon2, 9))
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetDiceAdvancement(LevelSourceType.EffectLevel)
                            .SetLevelAdvancement(EffectForm.LevelApplianceType.MultiplyDice,
                                LevelSourceType.EffectLevel)
                            .SetItemPropertyForm(ItemPropertyUsage.Unlimited, 1,
                                new FeatureUnlockByLevel(additionalDamageElementalWeapon, 3),
                                new FeatureUnlockByLevel(additionalDamageElementalWeapon, 4),
                                new FeatureUnlockByLevel(additionalDamageElementalWeapon1, 5),
                                new FeatureUnlockByLevel(additionalDamageElementalWeapon1, 6),
                                new FeatureUnlockByLevel(additionalDamageElementalWeapon2, 7),
                                new FeatureUnlockByLevel(additionalDamageElementalWeapon2, 8),
                                new FeatureUnlockByLevel(additionalDamageElementalWeapon2, 9))
                            .Build())
                    .SetParticleEffectParameters(effectParticleParameters)
                    .Build())
            .AddCustomSubFeatures(ExtraCarefulTrackedItem.Marker)
            .AddToDB();

        return spell;
    }

    #endregion

    #region Spirit Shroud

    private const string SpiritShroudName = "SpiritShroud";

    internal static SpellDefinition BuildSpiritShroud()
    {
        var sprite = Sprites.GetSprite(SpiritShroudName, Resources.SpiritShroud, 128);

        var hinder = ConditionDefinitionBuilder
            .Create(ConditionHindered_By_Frost, $"Condition{SpiritShroudName}Hinder")
            .SetSilent(Silent.None)
            .SetConditionType(ConditionType.Detrimental)
            .SetParentCondition(ConditionHindered)
            .CopyParticleReferences(ConditionSpiritGuardians)
            .AddToDB();

        var noHeal = ConditionDefinitionBuilder
            .Create($"Condition{SpiritShroudName}NoHeal")
            .SetGuiPresentation(Category.Condition, ConditionChilledByTouch.GuiPresentation.SpriteReference)
            .SetConditionType(ConditionType.Detrimental)
            .SetFeatures(FeatureDefinitionHealingModifiers.HealingModifierChilledByTouch)
            .SetSpecialDuration(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
            .AddToDB();

        return SpellDefinitionBuilder
            .Create(SpiritShroudName)
            .SetGuiPresentation(Category.Spell, sprite)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolNecromancy)
            .SetSpellLevel(3)
            .SetCastingTime(ActivationTime.BonusAction)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetSomaticComponent(true)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Defense)
            .SetRequiresConcentration(true)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetEffectAdvancement(
                        EffectIncrementMethod.PerAdditionalSlotLevel, 2, additionalDicePerIncrement: 1)
                    .Build())
            .SetSubSpells(
                BuildSpiritShroudSubSpell(DamageTypeRadiant, hinder, noHeal, sprite),
                BuildSpiritShroudSubSpell(DamageTypeNecrotic, hinder, noHeal, sprite),
                BuildSpiritShroudSubSpell(DamageTypeCold, hinder, noHeal, sprite))
            .AddToDB();
    }

    private static SpellDefinition BuildSpiritShroudSubSpell(
        string damage,
        ConditionDefinition hinder,
        ConditionDefinition noHeal,
        AssetReferenceSprite sprite)
    {
        return SpellDefinitionBuilder
            .Create($"{SpiritShroudName}{damage}")
            .SetGuiPresentation(Category.Spell, sprite)
            .SetSpellLevel(3)
            .SetVocalSpellSameType(VocalSpellSemeType.Defense)
            .SetCastingTime(ActivationTime.BonusAction)
            .SetRequiresConcentration(true)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Self, 1, TargetType.Sphere, 2)
                    .SetDurationData(DurationType.Minute, 1)
                    .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, 2,
                        additionalDicePerIncrement: 1)
                    //RAW it should only trigger if target starts turn in the area, but game doesn't trigger on turn start for some reason without OnEnter
                    .SetRecurrentEffect(
                        RecurrentEffect.OnActivation | RecurrentEffect.OnTurnStart | RecurrentEffect.OnEnter)
                    .SetParticleEffectParameters(SpiritGuardians)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(hinder, ConditionForm.ConditionOperation.Add)
                            .Build(),
                        EffectFormBuilder.ConditionForm(
                            ConditionDefinitionBuilder
                                .Create($"Condition{SpiritShroudName}{damage}")
                                .SetGuiPresentationNoContent(true)
                                .SetSilent(Silent.WhenAddedOrRemoved)
                                .CopyParticleReferences(ConditionSpiritGuardiansSelf)
                                .SetFeatures(
                                    FeatureDefinitionAdditionalDamageBuilder
                                        .Create($"AdditionalDamage{SpiritShroudName}{damage}")
                                        .SetGuiPresentationNoContent(true)
                                        .SetNotificationTag($"{SpiritShroudName}{damage}")
                                        .SetTriggerCondition(ExtraAdditionalDamageTriggerCondition.TargetWithin10Ft)
                                        .SetAttackOnly()
                                        .SetDamageDice(DieType.D8, 1)
                                        .SetSpecificDamageType(damage)
                                        .SetAdvancement(AdditionalDamageAdvancement.SlotLevel, 0, 1, 2)
                                        .SetConditionOperations(
                                            new ConditionOperationDescription
                                            {
                                                conditionDefinition = noHeal,
                                                operation = ConditionOperationDescription.ConditionOperation.Add
                                            })
                                        .AddToDB())
                                .AddToDB(), ConditionForm.ConditionOperation.Add, true, true),
                        EffectFormBuilder
                            .Create()
                            .SetTopologyForm(TopologyForm.Type.DangerousZone, true)
                            .Build())
                    .Build())
            .AddToDB();
    }

    #endregion

    #region Booming Step

    internal static SpellDefinition BuildBoomingStep()
    {
        const string Name = "BoomingStep";

        var powerExplode = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}Explode")
            .SetGuiPresentation(Name, Category.Spell, hidden: true)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round)
                    .SetTargetingData(Side.All, RangeType.Touch, 0, TargetType.IndividualsUnique)
                    .SetSavingThrowData(false, AttributeDefinitions.Constitution, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeThunder, 3, DieType.D10)
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .Build())
                    .SetParticleEffectParameters(Thunderwave)
                    .Build())
            .AddToDB();

        var conditionExplode = ConditionDefinitionBuilder
            .Create($"Condition{Name}Explode")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
            .SetFeatures(powerExplode)
            .AddCustomSubFeatures(new AddUsablePowersFromCondition())
            .AddToDB();

        powerExplode.AddCustomSubFeatures(
            new ModifyEffectDescriptionBoomingStepExplode(powerExplode, conditionExplode));

        var spell = SpellDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(Name, Resources.ThunderStep, 128, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolConjuration)
            .SetSpellLevel(3)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetVerboseComponent(true)
            .SetSomaticComponent(false)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round)
                    .SetTargetingData(Side.Ally, RangeType.Distance, 18, TargetType.Position)
                    .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, additionalDicePerIncrement: 1)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetMotionForm(MotionForm.MotionType.TeleportToDestination)
                            .Build(),
                        EffectFormBuilder.ConditionForm(conditionExplode, ConditionForm.ConditionOperation.Add, true))
                    .InviteOptionalAlly()
                    .ExcludeCaster()
                    .SetParticleEffectParameters(Thunderwave)
                    .Build())
            .AddCustomSubFeatures(new MagicEffectInitiatedByMeBoomingStep(powerExplode))
            .AddToDB();

        return spell;
    }

    private sealed class MagicEffectInitiatedByMeBoomingStep : IMagicEffectInitiatedByMe
    {
        private readonly FeatureDefinitionPower _powerExplode;

        public MagicEffectInitiatedByMeBoomingStep(FeatureDefinitionPower powerExplode)
        {
            _powerExplode = powerExplode;
        }

        public IEnumerator OnMagicEffectInitiatedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            var gameLocationBattleService = ServiceRepository.GetService<IGameLocationBattleService>();

            if (gameLocationBattleService is not { IsBattleInProgress: true })
            {
                yield break;
            }

            var actionParams = action.ActionParams.Clone();
            var attacker = action.ActingCharacter;
            var rulesetAttacker = attacker.RulesetCharacter;
            var usablePower = UsablePowersProvider.Get(_powerExplode, rulesetAttacker);

            actionParams.ActionDefinition = DatabaseHelper.ActionDefinitions.SpendPower;
            actionParams.RulesetEffect = ServiceRepository.GetService<IRulesetImplementationService>()
                //CHECK: no need for AddAsActivePowerToSource
                .InstantiateEffectPower(rulesetAttacker, usablePower, false);
            actionParams.TargetCharacters.SetRange(gameLocationBattleService.Battle.AllContenders
                .Where(x => x.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false }
                            && x != attacker
                            && !actionParams.TargetCharacters.Contains(x)
                            && gameLocationBattleService.IsWithinXCells(attacker, x, 2))
                .ToList());

            // special case don't ExecuteAction on MagicEffectInitiated
            action.ResultingActions.Add(new CharacterActionSpendPower(actionParams));
        }
    }

    private sealed class ModifyEffectDescriptionBoomingStepExplode : IModifyEffectDescription
    {
        private readonly ConditionDefinition _conditionExplode;
        private readonly FeatureDefinitionPower _powerExplode;

        public ModifyEffectDescriptionBoomingStepExplode(
            FeatureDefinitionPower powerExplode,
            ConditionDefinition conditionExplode)
        {
            _powerExplode = powerExplode;
            _conditionExplode = conditionExplode;
        }

        public bool IsValid(BaseDefinition definition, RulesetCharacter character, EffectDescription effectDescription)
        {
            return definition == _powerExplode;
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            if (!character.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, _conditionExplode.Name, out var activeCondition))
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
    }

    #endregion

    #region Flame Arrows

    internal static SpellDefinition BuildFlameArrows()
    {
        const string Name = "FlameArrows";

        var conditionFlameArrows = ConditionDefinitionBuilder
            .Create($"Condition{Name}")
            .SetGuiPresentation(Category.Condition, ConditionFeatTakeAim)
            .SetPossessive()
            .SetFeatures(
                FeatureDefinitionAdditionalDamageBuilder
                    .Create($"AdditionalDamage{Name}")
                    .SetGuiPresentationNoContent(true)
                    .SetNotificationTag(Name)
                    .SetRequiredProperty(RestrictedContextRequiredProperty.RangeWeapon)
                    .SetAttackModeOnly()
                    .SetDamageDice(DieType.D6, 1)
                    .SetSpecificDamageType(DamageTypeFire)
                    .SetImpactParticleReference(
                        FireBolt.EffectDescription.EffectParticleParameters.impactParticleReference)
                    .AddToDB())
            .AddCustomSubFeatures(new CustomBehaviorFlameArrows())
            .AddToDB();

        var spell = SpellDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(Name, Resources.FlameArrows, 128, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(3)
            .SetCastingTime(ActivationTime.BonusAction)
            .SetMaterialComponent(MaterialComponentType.Specific)
            .SetSpecificMaterialComponent(TagsDefinitions.WeaponTagRange, 0, false)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
            .SetRequiresConcentration(true)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Hour, 1)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, additionalItemBonus: 2)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionFlameArrows))
                    .SetParticleEffectParameters(FireBolt)
                    .Build())
            .AddToDB();

        return spell;
    }

    private sealed class CustomBehaviorFlameArrows : IOnConditionAddedOrRemoved, IPhysicalAttackFinishedByMe
    {
        public void OnConditionAdded(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            rulesetCondition.Amount = 12 + (2 * (rulesetCondition.EffectLevel - 3));
        }

        public void OnConditionRemoved(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            // empty
        }

        public IEnumerator OnPhysicalAttackFinishedByMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackerAttackMode,
            RollOutcome attackRollOutcome,
            int damageAmount)
        {
            if (attackerAttackMode is not { Ranged: true } or { Thrown: true })
            {
                yield break;
            }

            var rulesetAttacker = attacker.RulesetCharacter;

            if (!rulesetAttacker.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect,
                    "ConditionFlameArrows",
                    out var activeCondition))
            {
                yield break;
            }

            activeCondition.Amount--;

            if (activeCondition.Amount <= 0)
            {
                rulesetAttacker.BreakConcentration();
            }
        }
    }

    #endregion

    #region Lightning Arrow

    internal static SpellDefinition BuildLightningArrow()
    {
        const string Name = "LightningArrow";

        var powerLightningArrowLeap = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}Leap")
            .SetGuiPresentation(Name, Category.Spell, hidden: true)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Touch, 1, TargetType.IndividualsUnique)
                    .SetSavingThrowData(false, AttributeDefinitions.Dexterity, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .SetDamageForm(DamageTypeLightning, 2, DieType.D8)
                            .Build())
                    .SetParticleEffectParameters(LightningBolt)
                    .Build())
            .AddToDB();

        var conditionLightningArrow = ConditionDefinitionBuilder
            .Create($"Condition{Name}")
            .SetGuiPresentation(Category.Condition, ConditionFeatTakeAim)
            .SetPossessive()
            .SetFeatures(powerLightningArrowLeap)
            .AddToDB();

        powerLightningArrowLeap.AddCustomSubFeatures(
            new ModifyEffectDescriptionLightningArrowLeap(powerLightningArrowLeap, conditionLightningArrow));

        conditionLightningArrow.AddCustomSubFeatures(
            new CustomBehaviorLightningArrow(powerLightningArrowLeap, conditionLightningArrow));

        var spell = SpellDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(Name, Resources.LightningArrow, 128, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(3)
            .SetCastingTime(ActivationTime.BonusAction)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
            .SetRequiresConcentration(true)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, additionalDicePerIncrement: 1)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionLightningArrow))
                    .SetParticleEffectParameters(LightningBolt)
                    .Build())
            .AddToDB();

        return spell;
    }

    private sealed class ModifyEffectDescriptionLightningArrowLeap : IModifyEffectDescription
    {
        private readonly ConditionDefinition _conditionLightningArrow;
        private readonly FeatureDefinitionPower _featureDefinitionPower;

        public ModifyEffectDescriptionLightningArrowLeap(
            FeatureDefinitionPower featureDefinitionPower,
            ConditionDefinition conditionDefinition)
        {
            _featureDefinitionPower = featureDefinitionPower;
            _conditionLightningArrow = conditionDefinition;
        }

        public bool IsValid(BaseDefinition definition, RulesetCharacter character, EffectDescription effectDescription)
        {
            return definition == _featureDefinitionPower;
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            var damageForm = effectDescription.FindFirstDamageForm();
            var glc = GameLocationCharacter.GetFromActor(character);

            if (damageForm != null
                && glc != null
                && glc.UsedSpecialFeatures.TryGetValue(_conditionLightningArrow.Name, out var additionalDice))
            {
                damageForm.diceNumber = 2 + additionalDice;
            }

            return effectDescription;
        }
    }

    private sealed class CustomBehaviorLightningArrow :
        IAttackBeforeHitConfirmedOnEnemy, IPhysicalAttackFinishedByMe
    {
        private const int MainTargetDiceNumber = 3;
        private readonly ConditionDefinition _conditionLightningArrow;
        private readonly FeatureDefinitionPower _powerLightningArrowLeap;

        public CustomBehaviorLightningArrow(
            FeatureDefinitionPower powerLightningArrowLeap,
            ConditionDefinition conditionLightningArrow)
        {
            _powerLightningArrowLeap = powerLightningArrowLeap;
            _conditionLightningArrow = conditionLightningArrow;
        }

        public IEnumerator OnAttackBeforeHitConfirmedOnEnemy(
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
            if (attackMode is not { Ranged: true } && attackMode is not { Thrown: true })
            {
                yield break;
            }

            var rulesetAttacker = attacker.RulesetCharacter;

            if (!rulesetAttacker.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect,
                    _conditionLightningArrow.Name,
                    out var activeCondition))
            {
                yield break;
            }

            var diceNumber = MainTargetDiceNumber + activeCondition.EffectLevel - 3;
            var pos = actualEffectForms.FindIndex(x => x.FormType == EffectForm.EffectFormType.Damage);

            if (pos >= 0)
            {
                actualEffectForms.Insert(
                    pos + 1,
                    EffectFormBuilder.DamageForm(DamageTypeLightning, diceNumber, DieType.D8));
            }
        }

        public IEnumerator OnPhysicalAttackFinishedByMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            RollOutcome attackRollOutcome,
            int damageAmount)
        {
            if (attackMode is not { Ranged: true } && attackMode is not { Thrown: true })
            {
                yield break;
            }

            var rulesetAttacker = attacker.RulesetCharacter;

            if (!rulesetAttacker.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect,
                    _conditionLightningArrow.Name,
                    out var activeCondition))
            {
                yield break;
            }

            // keep a tab on additionalDice for leap power later on
            var additionalDice = activeCondition.EffectLevel - 3;

            attacker.UsedSpecialFeatures.TryAdd(_conditionLightningArrow.Name, additionalDice);

            rulesetAttacker.RemoveCondition(activeCondition);

            // half damage on target on a miss
            if (attackRollOutcome is not (RollOutcome.Success or RollOutcome.CriticalSuccess))
            {
                var rolls = new List<int>();
                var damageForm = new DamageForm
                {
                    DamageType = DamageTypeLightning,
                    DieType = DieType.D8,
                    DiceNumber = (MainTargetDiceNumber + additionalDice) / 2
                };
                var damageRoll = rulesetAttacker.RollDamage(damageForm, 0, false, 0, 0, 1, false, false, false, rolls);
                var rulesetDefender = defender.RulesetCharacter;

                RulesetActor.InflictDamage(
                    damageRoll,
                    damageForm,
                    damageForm.DamageType,
                    new RulesetImplementationDefinitions.ApplyFormsParams { targetCharacter = rulesetDefender },
                    rulesetDefender,
                    false,
                    attacker.Guid,
                    false,
                    attackMode.AttackTags,
                    new RollInfo(damageForm.DieType, rolls, 0),
                    true,
                    out _);
            }

            // leap damage on enemies within 10 ft from target
            var actionParams = action.ActionParams.Clone();
            var usablePower = UsablePowersProvider.Get(_powerLightningArrowLeap, rulesetAttacker);

            actionParams.ActionDefinition = DatabaseHelper.ActionDefinitions.SpendPower;
            actionParams.RulesetEffect = ServiceRepository.GetService<IRulesetImplementationService>()
                //CHECK: no need for AddAsActivePowerToSource
                .InstantiateEffectPower(rulesetAttacker, usablePower, false);
            actionParams.TargetCharacters.SetRange(battleManager.Battle.AllContenders
                .Where(x =>
                    x.IsOppositeSide(attacker.Side)
                    && x != defender
                    && x.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false }
                    && battleManager.IsWithinXCells(defender, x, 2))
                .ToList());

            var actionService = ServiceRepository.GetService<IGameLocationActionService>();

            // must enqueue actions whenever within an attack workflow otherwise game won't consume attack
            actionService.ExecuteAction(actionParams, null, true);
        }
    }

    #endregion

    #region Corrupting Bolt

    internal static SpellDefinition BuildCorruptingBolt()
    {
        const string Name = "CorruptingBolt";

        var conditionCorruptingBolt = ConditionDefinitionBuilder
            .Create(ConditionEyebiteSickened, $"Condition{Name}")
            .SetGuiPresentation(Category.Condition, ConditionDoomLaughter)
            .SetConditionType(ConditionType.Detrimental)
            .SetSpecialDuration(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
            .SetFeatures()
            .AddToDB();

        foreach (var damageDefinition in DatabaseRepository.GetDatabase<DamageDefinition>())
        {
            var damageType = damageDefinition.Name;

            var damageAffinity =
                FeatureDefinitionDamageAffinityBuilder
                    .Create($"DamageAffinity{Name}{damageType}")
                    .SetGuiPresentationNoContent(true)
                    .SetDamageAffinityType(DamageAffinityType.Vulnerability)
                    .SetDamageType(damageType)
                    .AddToDB();

            conditionCorruptingBolt.Features.Add(damageAffinity);
        }

        var spell = SpellDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(Name, Resources.CorruptingBolt, 128, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolNecromancy)
            .SetSpellLevel(3)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                    .SetTargetingData(Side.Enemy, RangeType.RangeHit, 24, TargetType.IndividualsUnique)
                    .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, additionalDicePerIncrement: 1)
                    .SetSavingThrowData(false, AttributeDefinitions.Constitution, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder.DamageForm(DamageTypeNecrotic, 4, DieType.D8))
                    .SetParticleEffectParameters(FingerOfDeath)
                    .Build())
            .AddCustomSubFeatures(new MagicEffectFinishedByMeCorruptingBolt(conditionCorruptingBolt))
            .AddToDB();

        spell.EffectDescription.EffectParticleParameters.impactParticleReference =
            Disintegrate.EffectDescription.EffectParticleParameters.impactParticleReference;
        spell.EffectDescription.EffectParticleParameters.effectParticleReference =
            Disintegrate.EffectDescription.EffectParticleParameters.effectParticleReference;

        conditionCorruptingBolt.AddCustomSubFeatures(
            new ActionFinishedByEnemyCorruptingBolt(conditionCorruptingBolt, spell));

        return spell;
    }

    private sealed class ActionFinishedByEnemyCorruptingBolt : IActionFinishedByEnemy
    {
        private readonly ConditionDefinition _conditionCorruptingBolt;
        private readonly SpellDefinition _spellCorruptingBolt;

        public ActionFinishedByEnemyCorruptingBolt(
            ConditionDefinition conditionCorruptingBolt,
            SpellDefinition spellCorruptingBolt)
        {
            _conditionCorruptingBolt = conditionCorruptingBolt;
            _spellCorruptingBolt = spellCorruptingBolt;
        }

        public IEnumerator OnActionFinishedByEnemy(CharacterAction characterAction, GameLocationCharacter target)
        {
            if (characterAction is CharacterActionCastSpell actionCastSpell &&
                actionCastSpell.activeSpell.SpellDefinition == _spellCorruptingBolt)
            {
                yield break;
            }

            if (characterAction.ActionParams.TargetCharacters.Count == 0 ||
                characterAction.ActionParams.TargetCharacters[0] != target)
            {
                yield break;
            }

            if (characterAction.AttackRollOutcome is not (RollOutcome.Success or RollOutcome.CriticalSuccess))
            {
                yield break;
            }

            var rulesetDefender = characterAction.ActionParams.TargetCharacters[0].RulesetCharacter;

            if (rulesetDefender == null)
            {
                yield break;
            }

            if (!rulesetDefender.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagCombat,
                    _conditionCorruptingBolt.Name,
                    out var activeCondition))
            {
                yield break;
            }

            rulesetDefender.RemoveCondition(activeCondition);
        }
    }

    private sealed class MagicEffectFinishedByMeCorruptingBolt : IMagicEffectFinishedByMe
    {
        private readonly ConditionDefinition _conditionCorruptingBolt;

        public MagicEffectFinishedByMeCorruptingBolt(ConditionDefinition conditionCorruptingBolt)
        {
            _conditionCorruptingBolt = conditionCorruptingBolt;
        }

        public IEnumerator OnMagicEffectFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            var rulesetAttacker = action.ActingCharacter.RulesetCharacter;
            var rulesetDefender = action.ActionParams.TargetCharacters[0].RulesetCharacter;

            if (rulesetDefender is not { IsDeadOrDyingOrUnconscious: false }
                || (action.RolledSaveThrow && action.SaveOutcome == RollOutcome.Success))
            {
                yield break;
            }

            rulesetDefender.InflictCondition(
                _conditionCorruptingBolt.Name,
                _conditionCorruptingBolt.DurationType,
                _conditionCorruptingBolt.DurationParameter,
                _conditionCorruptingBolt.TurnOccurence,
                AttributeDefinitions.TagCombat,
                rulesetAttacker.guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                _conditionCorruptingBolt.Name,
                0,
                0,
                0);
        }
    }

    #endregion

    #region Vitality Transfer

    internal static SpellDefinition BuildVitalityTransfer()
    {
        const string Name = "VitalityTransfer";

        var spell = SpellDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(Name, Resources.VitalityTransfer, 128, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolNecromancy)
            .SetSpellLevel(3)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Distance, 12, TargetType.IndividualsUnique)
                    .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, additionalDicePerIncrement: 1)
                    .ExcludeCaster()
                    .SetParticleEffectParameters(FalseLife)
                    .Build())
            .AddCustomSubFeatures(new ModifyDiceRollVitalityTransfer())
            .AddToDB();

        spell.EffectDescription.EffectParticleParameters.effectParticleReference =
            CureWounds.EffectDescription.EffectParticleParameters.effectParticleReference;

        return spell;
    }

    private sealed class ModifyDiceRollVitalityTransfer : IMagicEffectFinishedByMe
    {
        public IEnumerator OnMagicEffectFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            if (action is not CharacterActionCastSpell actionCastSpell)
            {
                yield break;
            }

            var caster = action.ActingCharacter;
            var rulesetCaster = caster.RulesetCharacter;
            var rulesetTarget = action.ActionParams.TargetCharacters[0].RulesetCharacter;
            var rolls = new List<int>();
            var diceNumber = 4 + actionCastSpell.activeSpell.EffectLevel - 3;
            var damageForm = new DamageForm
            {
                DamageType = DamageTypeNecrotic, DiceNumber = diceNumber, DieType = DieType.D8
            };
            var totalDamage = rulesetCaster.RollDamage(damageForm, 0, false, 0, 0, 1, false, false, false, rolls);
            var totalHealing = totalDamage * 2;
            var currentHitPoints = rulesetCaster.CurrentHitPoints;

            rulesetCaster.SustainDamage(totalDamage, damageForm.DamageType, false, rulesetCaster.Guid,
                new RollInfo(damageForm.DieType, rolls, 0), out _);

            EffectHelpers.StartVisualEffect(caster, caster, PowerSorcererChildRiftOffering);

            rulesetCaster.DamageSustained?.Invoke(rulesetCaster, totalDamage, damageForm.DamageType, true,
                currentHitPoints > totalDamage, false);

            rulesetTarget.ReceiveHealing(totalHealing, true, rulesetCaster.Guid);
        }
    }

    #endregion

    #region Hunger of the Void

    internal static SpellDefinition BuildHungerOfTheVoid()
    {
        const string Name = "HungerOfTheVoid";

        var conditionHungerOfTheVoid = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionBlinded, $"Condition{Name}")
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddToDB();

        conditionHungerOfTheVoid.AddCustomSubFeatures(new CustomBehaviorHungerOfTheVoid(conditionHungerOfTheVoid));

        var spell = SpellDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(Name, Resources.HungerOfTheVoid, 128, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(3)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
            .SetRequiresConcentration(true)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.All, RangeType.Distance, 24, TargetType.Sphere, 4)
                    .SetEffectAdvancement(
                        EffectIncrementMethod.PerAdditionalSlotLevel, 2, additionalDicePerIncrement: 1)
                    .SetRecurrentEffect(RecurrentEffect.OnActivation | RecurrentEffect.OnEnter)
                    .AddEffectForms(Darkness.EffectDescription.EffectForms.ToArray())
                    .AddEffectForms(
                        EffectFormBuilder.ConditionForm(conditionHungerOfTheVoid),
                        Entangle.EffectDescription.EffectForms[1])
                    .SetParticleEffectParameters(Darkness)
                    .Build())
            .AddToDB();

        // remove original condition blinded from Darkness spell
        spell.EffectDescription.EffectForms.RemoveAll(x =>
            x.FormType == EffectForm.EffectFormType.Condition
            && x.ConditionForm.ConditionDefinition == ConditionDefinitions.ConditionBlinded);

        return spell;
    }

    private sealed class CustomBehaviorHungerOfTheVoid : ICharacterTurnStartListener, ICharacterTurnEndListener
    {
        private readonly ConditionDefinition _conditionHungerOfTheVoid;

        public CustomBehaviorHungerOfTheVoid(ConditionDefinition conditionHungerOfTheVoid)
        {
            _conditionHungerOfTheVoid = conditionHungerOfTheVoid;
        }

        public void OnCharacterTurnEnded(GameLocationCharacter locationCharacter)
        {
            InflictDamage(DamageTypeAcid, locationCharacter.RulesetCharacter, VenomousSpike, true);
        }

        public void OnCharacterTurnStarted(GameLocationCharacter locationCharacter)
        {
            InflictDamage(DamageTypeCold, locationCharacter.RulesetCharacter, ConeOfCold);
        }

        private void InflictDamage(
            string damageType, RulesetActor rulesetActor, IMagicEffect magicEffect, bool rollSaving = false)
        {
            if (rulesetActor == null)
            {
                return;
            }

            if (!rulesetActor.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect,
                    _conditionHungerOfTheVoid.Name,
                    out var activeCondition))
            {
                return;
            }

            var rulesetCaster = EffectHelpers.GetCharacterByGuid(activeCondition.SourceGuid);

            if (rulesetCaster == null)
            {
                return;
            }

            if (rollSaving)
            {
                var casterSaveDC = 8 + activeCondition.SourceAbilityBonus + activeCondition.SourceProficiencyBonus;
                var modifierTrend = rulesetActor.actionModifier.savingThrowModifierTrends;
                var advantageTrends = rulesetActor.actionModifier.savingThrowAdvantageTrends;
                var dexterityModifier = AttributeDefinitions.ComputeAbilityScoreModifier(
                    rulesetActor.TryGetAttributeValue(AttributeDefinitions.Dexterity));

                rulesetActor.RollSavingThrow(
                    0, AttributeDefinitions.Dexterity, null, modifierTrend, advantageTrends, dexterityModifier,
                    casterSaveDC,
                    false, out var savingOutcome, out _);

                if (savingOutcome == RollOutcome.Success)
                {
                    return;
                }
            }

            var diceNumber = activeCondition.EffectLevel switch
            {
                >= 9 => 5,
                >= 7 => 4,
                >= 5 => 3,
                _ => 2
            };

            var rolls = new List<int>();
            var damageForm = new DamageForm { DamageType = damageType, DiceNumber = diceNumber, DieType = DieType.D6 };
            var totalDamage = rulesetActor.RollDamage(damageForm, 0, false, 0, 0, 1, false, false, false, rolls);

            var attacker = GameLocationCharacter.GetFromActor(rulesetCaster);
            var defender = GameLocationCharacter.GetFromActor(rulesetActor);

            if (attacker != null && defender != null)
            {
                EffectHelpers.StartVisualEffect(attacker, defender, magicEffect);
            }

            RulesetActor.InflictDamage(
                totalDamage,
                damageForm,
                damageForm.DamageType,
                new RulesetImplementationDefinitions.ApplyFormsParams { targetCharacter = rulesetActor },
                rulesetActor,
                false,
                activeCondition.SourceGuid,
                false,
                new List<string>(),
                new RollInfo(damageForm.DieType, rolls, 0),
                false,
                out _);
        }
    }

    #endregion
}
