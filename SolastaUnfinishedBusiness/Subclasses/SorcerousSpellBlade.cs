using System;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Validators;
using static RuleDefinitions;
using static FeatureDefinitionAttributeModifier;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Subclasses.CommonBuilders;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class SorcerousSpellBlade : AbstractSubclass
{
    private const string Name = "SorcerousSpellBlade";

    public SorcerousSpellBlade()
    {
        // LEVEL 01

        // Auto Prepared Spells

        var autoPreparedSpells = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create($"AutoPreparedSpells{Name}")
            .SetGuiPresentation("ExpandedSpells", Category.Feature)
            .SetAutoTag("Origin")
            .SetSpellcastingClass(CharacterClassDefinitions.Sorcerer)
            .AddPreparedSpellGroup(1, SpellsContext.ThunderousSmite)
            .AddPreparedSpellGroup(3, MistyStep)
            .AddPreparedSpellGroup(5, Haste)
            .AddPreparedSpellGroup(7, FireShield)
            .AddPreparedSpellGroup(9, MindTwist)
            .AddToDB();

        // Enchant Weapon

        // kept name for backward compatibility
        var attackModifierEnchantWeapon = FeatureDefinitionBuilder
            .Create($"AttackModifier{Name}EnchantWeapon")
            .SetGuiPresentation("AttackModifierEnchantWeapon", Category.Feature)
            .AddCustomSubFeatures(
                new CanUseAttribute(AttributeDefinitions.Charisma, CanWeaponBeEnchanted),
                new AddTagToWeaponWeaponAttack(TagsDefinitions.MagicalWeapon, CanWeaponBeEnchanted))
            .AddToDB();

        // Martial Training

        var featureSetMartialTraining = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}MartialTraining")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                FeatureDefinitionProficiencyBuilder
                    .Create($"Proficiency{Name}MartialTrainingArmor")
                    .SetGuiPresentationNoContent(true)
                    .SetProficiencies(ProficiencyType.Armor,
                        EquipmentDefinitions.LightArmorCategory,
                        EquipmentDefinitions.MediumArmorCategory)
                    .AddToDB(),
                FeatureDefinitionProficiencyBuilder
                    .Create($"Proficiency{Name}MartialTrainingWeapon")
                    .SetGuiPresentationNoContent(true)
                    .SetProficiencies(ProficiencyType.Weapon,
                        EquipmentDefinitions.SimpleWeaponCategory,
                        EquipmentDefinitions.MartialWeaponCategory)
                    .AddToDB())
            .AddToDB();

        // Mana Shield

        const string MANA_SHIELD_NAME = $"FeatureSet{Name}ManaShield";

        var powerManaShieldFixed = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ManaShield")
            .SetGuiPresentation(MANA_SHIELD_NAME, Category.Feature,
                Sprites.GetSprite("PowerManaShield", Resources.PowerManaShield, 256, 128))
            .SetUsesProficiencyBonus(ActivationTime.BonusAction)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.UntilAnyRest)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetParticleEffectParameters(FeatureDefinitionPowers.PowerTraditionCourtMageSpellShield)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetTempHpForm()
                            .Build())
                    .Build())
            .AddToDB();

        powerManaShieldFixed.AddCustomSubFeatures(
            new ValidatorsValidatePowerUse(c => c.GetRemainingPowerUses(powerManaShieldFixed) > 0),
            new ModifyEffectDescriptionManaShield(powerManaShieldFixed));

        var powerManaShieldPoints = FeatureDefinitionPowerBuilder
            .Create(powerManaShieldFixed, $"Power{Name}ManaShieldPoints")
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.SorceryPoints, 2, 0)
            .AddToDB();

        powerManaShieldPoints.AddCustomSubFeatures(
            new ValidatorsValidatePowerUse(c => c.GetRemainingPowerUses(powerManaShieldFixed) == 0),
            new ModifyEffectDescriptionManaShield(powerManaShieldPoints));

        var featureSetManaShield = FeatureDefinitionFeatureSetBuilder
            .Create(MANA_SHIELD_NAME)
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(powerManaShieldFixed, powerManaShieldPoints)
            .AddToDB();

        // LEVEL 14

        // War Sorcerer

        var additionalDamageWarSorcererMagic = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}WarSorcererMagic")
            .SetGuiPresentationNoContent(true)
            .SetNotificationTag("WarSorcerer")
            .SetTriggerCondition(AdditionalDamageTriggerCondition.SpellDamagesTarget)
            .SetRequiredProperty(RestrictedContextRequiredProperty.SpellWithAttackRoll)
            .SetDamageValueDetermination(AdditionalDamageValueDetermination.SpellcastingBonus)
            .SetSpecificDamageType(DamageTypeForce)
            .SetIgnoreCriticalDoubleDice(true)
            .AddToDB();

        var additionalDamageWarSorcererWeapon = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}WarSorcererWeapon")
            .SetGuiPresentationNoContent(true)
            .SetNotificationTag("WarSorcerer")
            .SetRequiredProperty(RestrictedContextRequiredProperty.Weapon)
            .SetAttackModeOnly()
            .SetDamageValueDetermination(AdditionalDamageValueDetermination.SpellcastingBonus)
            .SetSpecificDamageType(DamageTypeForce)
            .SetIgnoreCriticalDoubleDice(true)
            .AddToDB();

        // kept name for backward compatibility
        var additionalDamageWarSorcerer = FeatureDefinitionFeatureSetBuilder
            .Create($"AdditionalDamage{Name}WarSorcerer")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(additionalDamageWarSorcererMagic, additionalDamageWarSorcererWeapon)
            .AddToDB();

        // LEVEL 18

        const string BATTLE_REFLEXES_NAME = $"FeatureSet{Name}BattleReflexes";

        var attributeModifierBattleReflexes = FeatureDefinitionAttributeModifierBuilder
            .Create($"AttributeModifier{Name}BattleReflexes")
            .SetGuiPresentation(BATTLE_REFLEXES_NAME, Category.Feature)
            .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.ArmorClass, 2)
            .AddToDB();

        var proficiencyBattleReflexes = FeatureDefinitionProficiencyBuilder
            .Create($"Proficiency{Name}BattleReflexes")
            .SetGuiPresentation(BATTLE_REFLEXES_NAME, Category.Feature)
            .SetProficiencies(ProficiencyType.SavingThrow, AttributeDefinitions.Dexterity)
            .AddToDB();

        var featureSetBattleReflexes = FeatureDefinitionFeatureSetBuilder
            .Create(BATTLE_REFLEXES_NAME)
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(attributeModifierBattleReflexes, proficiencyBattleReflexes)
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.SorcererSpellBlade, 256))
            .AddFeaturesAtLevel(1,
                attackModifierEnchantWeapon,
                featureSetMartialTraining,
                autoPreparedSpells)
            .AddFeaturesAtLevel(2,
                featureSetManaShield)
            .AddFeaturesAtLevel(6,
                AttributeModifierCasterFightingExtraAttack,
                AttackReplaceWithCantripCasterFighting)
            .AddFeaturesAtLevel(14,
                additionalDamageWarSorcerer)
            .AddFeaturesAtLevel(18,
                featureSetBattleReflexes)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Sorcerer;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceSorcerousOrigin;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    //
    // Mana Shield
    //

    private sealed class ModifyEffectDescriptionManaShield(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinitionPower powerManaShield) : IModifyEffectDescription
    {
        public bool IsValid(
            BaseDefinition definition,
            RulesetCharacter character,
            EffectDescription effectDescription)
        {
            return definition == powerManaShield;
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            var classLevel = character.GetClassLevel(CharacterClassDefinitions.Sorcerer);
            var charisma = character.TryGetAttributeValue(AttributeDefinitions.Charisma);
            var charismaModifier = AttributeDefinitions.ComputeAbilityScoreModifier(charisma);
            var healing = classLevel + Math.Max(1, charismaModifier);

            effectDescription.EffectForms[0].TemporaryHitPointsForm.bonusHitPoints = healing;

            return effectDescription;
        }
    }
}
