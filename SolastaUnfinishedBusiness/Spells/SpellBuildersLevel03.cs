using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Validators;
using TA;
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

        var conditionBlindedByBlindingSmite = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionBlinded, $"ConditionBlindedBy{NAME}")
            .SetOrUpdateGuiPresentation(Category.Condition)
            .SetParentCondition(ConditionDefinitions.ConditionBlinded)
            .SetSpecialDuration(DurationType.Minute, 1, TurnOccurenceType.StartOfTurn)
            .SetFeatures()
            .AddToDB();

        conditionBlindedByBlindingSmite.GuiPresentation.description = "Rules/&ConditionBlindedDescription";

        var additionalDamageBlindingSmite = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{NAME}")
            .SetGuiPresentation(NAME, Category.Spell)
            .SetNotificationTag(NAME)
            .AddCustomSubFeatures(ValidatorsRestrictedContext.IsWeaponOrUnarmedAttack)
            .SetDamageDice(DieType.D8, 3)
            .SetSpecificDamageType(DamageTypeRadiant)
            .SetSavingThrowData(EffectDifficultyClassComputation.SpellCastingFeature, EffectSavingThrowType.None)
            .AddConditionOperation(
                new ConditionOperationDescription
                {
                    operation = ConditionOperationDescription.ConditionOperation.Add,
                    conditionDefinition = conditionBlindedByBlindingSmite,
                    hasSavingThrow = true,
                    canSaveToCancel = true,
                    saveAffinity = EffectSavingThrowType.Negates,
                    saveOccurence = TurnOccurenceType.StartOfTurn
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

        var spriteReference = Sprites.GetSprite(NAME, Resources.WinterBreath, 128);

        var effectDescription = EffectDescriptionBuilder
            .Create()
            .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Cone, 3)
            .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, additionalDicePerIncrement: 1)
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
                Sprites.GetSprite(NAME, Resources.CrusadersMantle, 128))
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
                    //.SetParticleEffectParameters(DivineFavor)
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
                    .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, additionalDicePerIncrement: 1)
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
                            .SetDamageForm(DamageTypeForce, dieType: DieType.D6, diceNumber: 6)
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetMotionForm(MotionForm.MotionType.PushFromOrigin, 3)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build())
                    .SetParticleEffectParameters(PowerFunctionWandFearCone)
                    .SetCasterEffectParameters(Darkness)
                    .SetImpactEffectParameters(MindTwist)
                    .Build())
            .AddToDB();

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
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(Name, Resources.AdderFangs, 128))
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
                    .SetEffectEffectParameters(InflictWounds)
                    .Build())
            .AddToDB();

        return spell;
    }

    #endregion

    #region Aura of Vitality

    internal static SpellDefinition BuildAuraOfVitality()
    {
        // kept this name for backward compatibility reasons
        const string NAME = "AuraOfLife";

        var sprite = Sprites.GetSprite(NAME, Resources.AuraOfVitality, 128);

        var powerAuraOfLife = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}")
            .SetGuiPresentation(NAME, Category.Spell, sprite)
            .SetUsesFixed(ActivationTime.BonusAction)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetHealingForm(HealingComputation.Dice, 0, DieType.D6, 2, false,
                                HealingCap.MaximumHitPoints)
                            .Build())
                    .SetParticleEffectParameters(HealingWord)
                    .Build())
            .AddToDB();

        var conditionAuraOfLifeSelf = ConditionDefinitionBuilder
            .Create($"Condition{NAME}Self")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(powerAuraOfLife)
            .AddCustomSubFeatures(AddUsablePowersFromCondition.Marker)
            .AddToDB();

        var conditionAuraOfLife = ConditionDefinitionBuilder
            .Create($"Condition{NAME}")
            .SetGuiPresentation(Category.Condition, ConditionHeroism)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddToDB();

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, sprite)
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
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Sphere, 6)
                    .SetRecurrentEffect(
                        RecurrentEffect.OnActivation | RecurrentEffect.OnEnter | RecurrentEffect.OnTurnStart)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(conditionAuraOfLife),
                        EffectFormBuilder.ConditionForm(
                            conditionAuraOfLifeSelf,
                            ConditionForm.ConditionOperation.Add, true, true))
                    .SetParticleEffectParameters(DivineWord)
                    .Build())
            .AddCustomSubFeatures(new FilterTargetingCharacterAuraOfVitality(conditionAuraOfLife))
            .AddToDB();

        return spell;
    }

    private sealed class FilterTargetingCharacterAuraOfVitality(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionAuraOfVitality) : IFilterTargetingCharacter
    {
        public bool EnforceFullSelection => false;

        public bool IsValid(CursorLocationSelectTarget __instance, GameLocationCharacter target)
        {
            if (target.RulesetCharacter == null)
            {
                return false;
            }

            var isValid = target.RulesetCharacter.HasConditionOfCategoryAndType(
                AttributeDefinitions.TagEffect, conditionAuraOfVitality.Name);

            if (!isValid)
            {
                __instance.actionModifier.FailureFlags.Add("Tooltip/&MustBeAuraOfLife");
            }

            return isValid;
        }
    }

    #endregion

    #region Elemental Weapon

    internal static SpellDefinition BuildElementalWeapon()
    {
        const string NAME = "ElementalWeapon";

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.ElementalWeapon, 128))
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
            .AddCustomSubFeatures(TrackItemsCarefully.Marker)
            .AddToDB();

        return spell;

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
                                        .SetTriggerCondition(ExtraAdditionalDamageTriggerCondition.TargetIsWithin10Ft)
                                        .SetAttackOnly()
                                        .SetDamageDice(DieType.D8, 1)
                                        .SetSpecificDamageType(damage)
                                        .SetAdvancement(AdditionalDamageAdvancement.SlotLevel, 0, 1, 2)
                                        .AddConditionOperation(
                                            ConditionOperationDescription.ConditionOperation.Add, noHeal)
                                        .AddToDB())
                                .AddToDB(),
                            ConditionForm.ConditionOperation.Add, true, true),
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
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round)
                    .SetTargetingData(Side.All, RangeType.Distance, 18, TargetType.IndividualsUnique)
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
            .SetFeatures(powerExplode)
            .AddCustomSubFeatures(AddUsablePowersFromCondition.Marker)
            .AddToDB();

        powerExplode.AddCustomSubFeatures(
            new ModifyEffectDescriptionBoomingStepExplode(powerExplode, conditionExplode));

        var spell = SpellDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(Name, Resources.ThunderStep, 128))
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
                        EffectFormBuilder.ConditionForm(
                            conditionExplode,
                            ConditionForm.ConditionOperation.Add, true, true))
                    .InviteOptionalAlly()
                    .ExcludeCaster()
                    .SetParticleEffectParameters(Thunderwave)
                    .Build())
            .AddCustomSubFeatures(new CustomBehaviorBoomingStep(powerExplode))
            .AddToDB();

        return spell;
    }

    private sealed class CustomBehaviorBoomingStep(FeatureDefinitionPower powerExplode)
        : IMagicEffectInitiatedByMe, IMagicEffectFinishedByMe, IFilterTargetingCharacter
    {
        private readonly List<GameLocationCharacter> _targets = [];

        public bool EnforceFullSelection => false;

        public bool IsValid(CursorLocationSelectTarget __instance, GameLocationCharacter target)
        {
            if (target.RulesetCharacter == null)
            {
                return false;
            }

            var isValid =
                target.RulesetCharacter is not RulesetCharacterEffectProxy &&
                __instance.ActionParams.ActingCharacter.IsWithinRange(target, 1);

            if (!isValid)
            {
                __instance.actionModifier.FailureFlags.Add("Tooltip/&MustBeWithin5ft");
            }

            return isValid;
        }

        public IEnumerator OnMagicEffectFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            var attacker = action.ActingCharacter;
            var rulesetAttacker = attacker.RulesetCharacter;

            var implementationManager =
                ServiceRepository.GetService<IRulesetImplementationService>() as RulesetImplementationManager;

            var usablePower = PowerProvider.Get(powerExplode, rulesetAttacker);
            var actionModifiers = new List<ActionModifier>();

            for (var i = 0; i < _targets.Count; i++)
            {
                actionModifiers.Add(new ActionModifier());
            }

            // don't use PowerNoCost here as it breaks the spell under MP
            var actionParams = new CharacterActionParams(attacker, ActionDefinitions.Id.SpendPower)
            {
                ActionModifiers = actionModifiers,
                RulesetEffect = implementationManager
                    .MyInstantiateEffectPower(rulesetAttacker, usablePower, false),
                UsablePower = usablePower,
                targetCharacters = _targets
            };

            ServiceRepository.GetService<IGameLocationActionService>()?
                .ExecuteAction(actionParams, null, true);

            yield break;
        }

        public IEnumerator OnMagicEffectInitiatedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            var attacker = action.ActingCharacter;
            var locationCharacterService = ServiceRepository.GetService<IGameLocationCharacterService>();
            var contenders =
                (Gui.Battle?.AllContenders ??
                 locationCharacterService.PartyCharacters.Union(locationCharacterService.GuestCharacters))
                .ToList();

            _targets.SetRange(contenders
                .Where(x =>
                    x.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false } &&
                    x != attacker &&
                    !action.ActionParams.TargetCharacters.Contains(x) &&
                    attacker.IsWithinRange(x, 2)));

            yield break;
        }
    }

    private sealed class ModifyEffectDescriptionBoomingStepExplode(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinitionPower powerExplode,
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionExplode)
        : IModifyEffectDescription
    {
        public bool IsValid(BaseDefinition definition, RulesetCharacter character, EffectDescription effectDescription)
        {
            return definition == powerExplode;
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            if (!character.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, conditionExplode.Name, out var activeCondition))
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
                    .SetImpactParticleReference(FireBolt)
                    .AddToDB())
            .AddCustomSubFeatures(new CustomBehaviorFlameArrows())
            .AddToDB();

        var spell = SpellDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(Name, Resources.FlameArrows, 128))
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
            RulesetAttackMode attackMode,
            RollOutcome rollOutcome,
            int damageAmount)
        {
            if (attackMode is not { Ranged: true } or { Thrown: true })
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
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 12, TargetType.IndividualsUnique)
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
            AddUsablePowersFromCondition.Marker,
            new CustomBehaviorLightningArrow(powerLightningArrowLeap, conditionLightningArrow));

        var spell = SpellDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(Name, Resources.LightningArrow, 128))
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

    private sealed class ModifyEffectDescriptionLightningArrowLeap(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinitionPower featureDefinitionPower,
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionDefinition)
        : IModifyEffectDescription
    {
        public bool IsValid(BaseDefinition definition, RulesetCharacter character, EffectDescription effectDescription)
        {
            return definition == featureDefinitionPower;
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
                && glc.UsedSpecialFeatures.TryGetValue(conditionDefinition.Name, out var additionalDice))
            {
                damageForm.diceNumber = 2 + additionalDice;
            }

            return effectDescription;
        }
    }

    private sealed class CustomBehaviorLightningArrow(
        FeatureDefinitionPower powerLightningArrowLeap,
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionLightningArrow)
        : IPhysicalAttackBeforeHitConfirmedOnEnemy, IPhysicalAttackFinishedByMe
    {
        private const int MainTargetDiceNumber = 3;

        public IEnumerator OnPhysicalAttackBeforeHitConfirmedOnEnemy(
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
            if (attackMode is not { Ranged: true } && attackMode is not { Thrown: true })
            {
                yield break;
            }

            var rulesetAttacker = attacker.RulesetCharacter;

            if (!rulesetAttacker.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect,
                    conditionLightningArrow.Name,
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
            RollOutcome rollOutcome,
            int damageAmount)
        {
            if (attackMode is not { Ranged: true } && attackMode is not { Thrown: true })
            {
                yield break;
            }

            var rulesetAttacker = attacker.RulesetCharacter;

            if (!rulesetAttacker.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect,
                    conditionLightningArrow.Name,
                    out var activeCondition))
            {
                yield break;
            }

            // keep a tab on additionalDice for leap power later on
            var additionalDice = activeCondition.EffectLevel - 3;

            attacker.UsedSpecialFeatures.TryAdd(conditionLightningArrow.Name, additionalDice);

            rulesetAttacker.RemoveCondition(activeCondition);

            // half damage on target on a miss
            if (rollOutcome is not (RollOutcome.Success or RollOutcome.CriticalSuccess))
            {
                var rolls = new List<int>();
                var damageForm = new DamageForm
                {
                    DamageType = DamageTypeLightning,
                    DieType = DieType.D8,
                    DiceNumber = MainTargetDiceNumber + additionalDice
                };
                var damageRoll = rulesetAttacker.RollDamage(damageForm, 0, false, 0, 0, 1, false, false, false, rolls);
                var rulesetDefender = defender.RulesetActor;
                var applyFormsParams = new RulesetImplementationDefinitions.ApplyFormsParams
                {
                    sourceCharacter = rulesetAttacker,
                    targetCharacter = rulesetDefender,
                    position = defender.LocationPosition
                };

                RulesetActor.InflictDamage(
                    damageRoll / 2,
                    damageForm,
                    damageForm.DamageType,
                    applyFormsParams,
                    rulesetDefender,
                    false,
                    rulesetAttacker.Guid,
                    false,
                    attackMode.AttackTags,
                    new RollInfo(damageForm.DieType, rolls, 0),
                    false,
                    out _);
            }

            // leap damage on enemies within 10 ft from target
            var implementationManager =
                ServiceRepository.GetService<IRulesetImplementationService>() as RulesetImplementationManager;
            var usablePower = PowerProvider.Get(powerLightningArrowLeap, rulesetAttacker);
            var targets = battleManager.Battle
                .GetContenders(defender, isOppositeSide: false, withinRange: 2);
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

    #region Corrupting Bolt

    internal static SpellDefinition BuildCorruptingBolt()
    {
        const string Name = "CorruptingBolt";

        var conditionCorruptingBolt = ConditionDefinitionBuilder
            .Create(ConditionEyebiteSickened, $"Condition{Name}")
            .SetGuiPresentation(Category.Condition, ConditionDoomLaughter)
            .SetConditionType(ConditionType.Detrimental)
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
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(Name, Resources.CorruptingBolt, 128))
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
                    .SetImpactEffectParameters(Disintegrate)
                    .SetEffectEffectParameters(Disintegrate)
                    .Build())
            .AddCustomSubFeatures(new MagicEffectFinishedByMeCorruptingBolt(conditionCorruptingBolt))
            .AddToDB();

        conditionCorruptingBolt.AddCustomSubFeatures(
            new ActionFinishedByEnemyCorruptingBolt(conditionCorruptingBolt, spell));

        return spell;
    }

    private sealed class ActionFinishedByEnemyCorruptingBolt(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionCorruptingBolt,
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        SpellDefinition spellCorruptingBolt)
        : IActionFinishedByEnemy
    {
        public IEnumerator OnActionFinishedByEnemy(CharacterAction characterAction, GameLocationCharacter target)
        {
            if (characterAction is CharacterActionCastSpell actionCastSpell &&
                actionCastSpell.activeSpell.SpellDefinition == spellCorruptingBolt)
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
                    AttributeDefinitions.TagEffect,
                    conditionCorruptingBolt.Name,
                    out var activeCondition))
            {
                yield break;
            }

            rulesetDefender.RemoveCondition(activeCondition);
        }
    }

    private sealed class MagicEffectFinishedByMeCorruptingBolt(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionCorruptingBolt) : IMagicEffectFinishedByMe
    {
        public IEnumerator OnMagicEffectFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            if (!action.RolledSaveThrow || action.SaveOutcome == RollOutcome.Success)
            {
                yield break;
            }

            var rulesetAttacker = action.ActingCharacter.RulesetCharacter;

            // need to loop over target characters to support twinned metamagic scenarios
            foreach (var rulesetDefender in action.ActionParams.TargetCharacters
                         .Select(target => target.RulesetCharacter)
                         .Where(rulesetDefender =>
                             rulesetDefender is { IsDeadOrDyingOrUnconscious: false }))
            {
                rulesetDefender.InflictCondition(
                    conditionCorruptingBolt.Name,
                    DurationType.Round,
                    1,
                    TurnOccurenceType.EndOfSourceTurn,
                    AttributeDefinitions.TagEffect,
                    rulesetAttacker.guid,
                    rulesetAttacker.CurrentFaction.Name,
                    1,
                    conditionCorruptingBolt.Name,
                    0,
                    0,
                    0);
            }
        }
    }

    #endregion

    #region Vitality Transfer

    internal static SpellDefinition BuildVitalityTransfer()
    {
        const string Name = "VitalityTransfer";

        var spell = SpellDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(Name, Resources.VitalityTransfer, 128))
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
                    .SetEffectEffectParameters(CureWounds)
                    .Build())
            .AddCustomSubFeatures(new ModifyDiceRollVitalityTransfer())
            .AddToDB();

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
            var diceNumber = 4 + actionCastSpell.activeSpell.EffectLevel - 3;

            // need to loop over target characters to support twinned metamagic scenarios
            foreach (var target in action.ActionParams.TargetCharacters)
            {
                var rulesetTarget = target.RulesetCharacter;
                var rolls = new List<int>();
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
    }

    #endregion

    #region Hunger of the Void

    internal static SpellDefinition BuildHungerOfTheVoid()
    {
        const string Name = "HungerOfTheVoid";

        var conditionHungerOfTheVoid = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionBlinded, $"ConditionBlindedBy{Name}")
            .SetOrUpdateGuiPresentation(Category.Condition)
            .SetParentCondition(ConditionDefinitions.ConditionBlinded)
            .SetFeatures()
            .AddToDB();

        conditionHungerOfTheVoid.GuiPresentation.description = "Rules/&ConditionBlindedDescription";

        conditionHungerOfTheVoid.AddCustomSubFeatures(new CustomBehaviorHungerOfTheVoid(conditionHungerOfTheVoid));

        var spell = SpellDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(Name, Resources.HungerOfTheVoid, 128))
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
                    .SetRecurrentEffect(
                        RecurrentEffect.OnActivation | RecurrentEffect.OnEnter | RecurrentEffect.OnTurnStart)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(conditionHungerOfTheVoid),
                        EffectFormBuilder.TopologyForm(TopologyForm.Type.DangerousZone, true),
                        Darkness.EffectDescription.EffectForms[2],
                        Darkness.EffectDescription.EffectForms[3])
                    .SetParticleEffectParameters(Darkness)
                    .Build())
            .AddToDB();

        return spell;
    }

    private sealed class CustomBehaviorHungerOfTheVoid(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionHungerOfTheVoid) : ICharacterTurnStartListener, ICharacterBeforeTurnEndListener
    {
        public void OnCharacterBeforeTurnEnded(GameLocationCharacter locationCharacter)
        {
            InflictDamage(DamageTypeAcid, locationCharacter.RulesetCharacter, VenomousSpike, true);
        }

        public void OnCharacterTurnStarted(GameLocationCharacter locationCharacter)
        {
            InflictDamage(DamageTypeCold, locationCharacter.RulesetCharacter, ConeOfCold);
        }

        // ReSharper disable once SuggestBaseTypeForParameter
        private void InflictDamage(
            string damageType, RulesetCharacter rulesetCharacter, IMagicEffect magicEffect, bool rollSaving = false)
        {
            if (rulesetCharacter == null)
            {
                return;
            }

            if (!rulesetCharacter.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect,
                    conditionHungerOfTheVoid.Name,
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
                var modifierTrend = rulesetCharacter.actionModifier.savingThrowModifierTrends;
                var advantageTrends = rulesetCharacter.actionModifier.savingThrowAdvantageTrends;
                var dexterityModifier = AttributeDefinitions.ComputeAbilityScoreModifier(
                    rulesetCharacter.TryGetAttributeValue(AttributeDefinitions.Dexterity));

                rulesetCharacter.RollSavingThrow(
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
            var totalDamage = rulesetCaster.RollDamage(damageForm, 0, false, 0, 0, 1, false, false, false, rolls);

            var attacker = GameLocationCharacter.GetFromActor(rulesetCaster);
            var defender = GameLocationCharacter.GetFromActor(rulesetCharacter);

            if (attacker != null && defender != null)
            {
                EffectHelpers.StartVisualEffect(attacker, defender, magicEffect);
            }

            var applyFormsParams = new RulesetImplementationDefinitions.ApplyFormsParams
            {
                sourceCharacter = rulesetCaster,
                targetCharacter = rulesetCharacter,
                position = defender?.LocationPosition ?? int3.zero
            };

            RulesetActor.InflictDamage(
                totalDamage,
                damageForm,
                damageForm.DamageType,
                applyFormsParams,
                rulesetCharacter,
                false,
                activeCondition.SourceGuid,
                false,
                [],
                new RollInfo(damageForm.DieType, rolls, 0),
                false,
                out _);
        }
    }

    #endregion
}
