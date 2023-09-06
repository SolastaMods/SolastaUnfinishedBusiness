using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
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
            .SetCustomSubFeatures(ValidatorsRestrictedContext.IsWeaponAttack)
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
            .SetVerboseComponent(true)
            .SetEffectDescription(EffectDescriptionBuilder
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
            .SetDurationData(DurationType.Instantaneous)
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
            .SetEffectDescription(effectDescription)
            .SetCastingTime(ActivationTime.Action)
            .SetSpellLevel(3)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolConjuration)
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
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetTargetFiltering(TargetFilteringMethod.CharacterOnly)
                .SetTargetingData(Side.Ally, RangeType.Self, 1, TargetType.Sphere, 6)
                .SetDurationData(DurationType.Minute, 1)
                .SetParticleEffectParameters(DivineFavor)
                .SetRecurrentEffect(RecurrentEffect.OnActivation |
                                    RecurrentEffect.OnTurnStart |
                                    RecurrentEffect.OnEnter)
                .AddEffectForms(EffectFormBuilder
                    .Create()
                    .SetConditionForm(conditionCrusadersMantle, ConditionForm.ConditionOperation.Add)
                    .Build())
                .Build())
            .SetCastingTime(ActivationTime.Action)
            .SetRequiresConcentration(true)
            .SetSpellLevel(3)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
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
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetDurationData(DurationType.Instantaneous)
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
                .SetParticleEffectParameters(Fear.EffectDescription.EffectParticleParameters)
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
            .SetCastingTime(ActivationTime.Action)
            .SetSpellLevel(3)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
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
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetDurationData(DurationType.Minute, 1)
                .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, additionalDicePerIncrement: 1)
                .Build())
            .SetCastingTime(ActivationTime.Action)
            .SetRequiresConcentration(true)
            .SetSpellLevel(3)
            .SetUniqueInstance()
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSubSpells(
                BuildElementalWeaponSubspell(DamageTypeAcid, AcidArrow),
                BuildElementalWeaponSubspell(DamageTypeCold, ConeOfCold),
                BuildElementalWeaponSubspell(DamageTypeFire, FireBolt),
                BuildElementalWeaponSubspell(DamageTypeLightning, LightningBolt),
                BuildElementalWeaponSubspell(DamageTypeThunder, Shatter))
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
            .SetCustomSubFeatures(ExtraCarefulTrackedItem.Marker)
            .SetEffectDescription(EffectDescriptionBuilder.Create(MagicWeapon)
                .SetDurationData(DurationType.Minute, 1)
                .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, additionalDicePerIncrement: 1)
                .SetEffectForms(
                    EffectFormBuilder
                        .Create()
                        .SetDiceAdvancement(LevelSourceType.EffectLevel)
                        .SetLevelAdvancement(EffectForm.LevelApplianceType.MultiplyDice, LevelSourceType.EffectLevel)
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
                        .SetLevelAdvancement(EffectForm.LevelApplianceType.MultiplyDice, LevelSourceType.EffectLevel)
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
            .SetCastingTime(ActivationTime.Action)
            .SetRequiresConcentration(true)
            .SetUniqueInstance()
            .SetSpellLevel(3)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
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
            .SetSpellLevel(3)
            .SetVocalSpellSameType(VocalSpellSemeType.Defense)
            .SetCastingTime(ActivationTime.BonusAction)
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
                    .SetRecurrentEffect(RecurrentEffect.OnTurnStart | RecurrentEffect.OnEnter)
                    .SetParticleEffectParameters(SpiritGuardians)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(hinder, ConditionForm.ConditionOperation.Add)
                            .Build(),
                        EffectFormBuilder.ConditionForm(ConditionDefinitionBuilder
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
}
