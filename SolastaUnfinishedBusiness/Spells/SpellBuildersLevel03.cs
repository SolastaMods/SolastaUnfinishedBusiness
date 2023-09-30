using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.CustomValidators;
using SolastaUnfinishedBusiness.Properties;
using UnityEngine.AddressableAssets;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static RuleDefinitions;

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
                    .SetParticleEffectParameters(FeatureDefinitionPowers.PowerFunctionWandFearCone)
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
                    .Build())
            .AddToDB();

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

        var conditionExplode = ConditionDefinitionBuilder
            .Create($"Condition{Name}Explode")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
            .AddToDB();

        var powerExplode = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}Explode")
            .SetGuiPresentation(Name, Category.Spell)
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
            .AddCustomSubFeatures(new MagicEffectFinishedByMeBoomingStep(powerExplode))
            .AddToDB();

        return spell;
    }

    private sealed class MagicEffectFinishedByMeBoomingStep : IMagicEffectInitiatedByMe
    {
        private readonly FeatureDefinitionPower _powerExplode;

        public MagicEffectFinishedByMeBoomingStep(FeatureDefinitionPower powerExplode)
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
                .InstantiateEffectPower(rulesetAttacker, usablePower, false)
                .AddAsActivePowerToSource();
            actionParams.TargetCharacters.SetRange(gameLocationBattleService.Battle.AllContenders
                .Where(x => x.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false }
                            && x != attacker
                            && !actionParams.TargetCharacters.Contains(x)
                            && gameLocationBattleService.IsWithinXCells(attacker, x, 2))
                .ToList());

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

        public IEnumerator OnAttackFinishedByMe(
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

        var conditionLightningArrow = ConditionDefinitionBuilder
            .Create($"Condition{Name}")
            .SetGuiPresentation(Category.Condition, ConditionFeatTakeAim)
            .SetPossessive()
            .AddToDB();

        var powerLightningArrowLeap = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}Leap")
            .SetGuiPresentation(Name, Category.Spell)
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
        IPhysicalAttackInitiatedByMe, IPhysicalAttackFinishedByMe
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

        public IEnumerator OnAttackFinishedByMe(
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
            var actionParamsLeap = action.ActionParams.Clone();
            var usablePowerLeap = UsablePowersProvider.Get(_powerLightningArrowLeap, rulesetAttacker);

            actionParamsLeap.ActionDefinition = DatabaseHelper.ActionDefinitions.SpendPower;
            actionParamsLeap.RulesetEffect = ServiceRepository.GetService<IRulesetImplementationService>()
                .InstantiateEffectPower(rulesetAttacker, usablePowerLeap, false)
                .AddAsActivePowerToSource();
            actionParamsLeap.TargetCharacters.SetRange(battleManager.Battle.AllContenders
                .Where(x =>
                    x.IsOppositeSide(attacker.Side)
                    && x != defender
                    && x.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false }
                    && battleManager.IsWithinXCells(defender, x, 2))
                .ToList());

            action.ResultingActions.Add(new CharacterActionSpendPower(actionParamsLeap));
        }

        public IEnumerator OnAttackInitiatedByMe(
            GameLocationBattleManager __instance,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            RulesetAttackMode attackMode)
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

            var additionalDice = activeCondition.EffectLevel - 3;

            attackMode.effectDescription = EffectDescriptionBuilder
                .Create(attackMode.EffectDescription)
                .Build();

            var pos = attackMode.EffectDescription.EffectForms.FindIndex(x =>
                x.FormType == EffectForm.EffectFormType.Damage);

            if (pos >= 0)
            {
                attackMode.effectDescription.EffectForms.Insert(
                    pos + 1,
                    EffectFormBuilder.DamageForm(DamageTypeLightning, MainTargetDiceNumber + additionalDice,
                        DieType.D8));
            }
        }
    }

    #endregion

    #region Corrupting Bolt

    internal static SpellDefinition BuildCorruptingBolt()
    {
        const string Name = "CorruptingBolt";

        var conditionCorruptingBolt = ConditionDefinitionBuilder
            .Create($"Condition{Name}")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionDiseased)
            .SetPossessive()
            .SetConditionType(ConditionType.Detrimental)
            .SetFeatures()
            .AddToDB();

        conditionCorruptingBolt.AddCustomSubFeatures(new CustomBehaviorCorruptingBolt(conditionCorruptingBolt));

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
                        EffectFormBuilder.DamageForm(DamageTypeNecrotic, 4, DieType.D8),
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetConditionForm(conditionCorruptingBolt, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .SetParticleEffectParameters(VampiricTouch)
                    .Build())
            .AddToDB();

        return spell;
    }

    private sealed class CustomBehaviorCorruptingBolt :
        IMagicalAttackBeforeHitConfirmedOnMe, IAttackBeforeHitConfirmedOnMe
    {
        private readonly ConditionDefinition _conditionCorruptingBolt;

        public CustomBehaviorCorruptingBolt(ConditionDefinition conditionCorruptingBolt)
        {
            _conditionCorruptingBolt = conditionCorruptingBolt;
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
            if (rulesetEffect != null)
            {
                yield break;
            }

            defender.RulesetCharacter.RemoveAllConditionsOfType(_conditionCorruptingBolt.Name);
            attackModifier.attackerDamageMultiplier += 1;
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
            defender.RulesetCharacter.RemoveAllConditionsOfType(_conditionCorruptingBolt.Name);
            magicModifier.attackerDamageMultiplier += 1;

            yield break;
        }
    }

    #endregion
}
