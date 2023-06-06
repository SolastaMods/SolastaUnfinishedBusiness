using System;
using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using static FeatureDefinitionAttributeModifier;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Subclasses.CommonBuilders;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class SorcerousSpellBlade : AbstractSubclass
{
    private const string Name = "SorcerousSpellBlade";

    internal SorcerousSpellBlade()
    {
        // LEVEL 01

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
            .AddPreparedSpellGroup(11, GlobeOfInvulnerability)
            .AddToDB();

        var featureEnchantWeapon = FeatureDefinitionBuilder
            .Create($"Feature{Name}EnchantWeapon")
            .SetGuiPresentation("PowerArcaneFighterEnchantWeapon", Category.Feature)
            .SetCustomSubFeatures(
                new CanUseAttribute(AttributeDefinitions.Charisma, CanWeaponBeEmpowered))
            .AddToDB();

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

        var effectDescriptionManaShield = EffectDescriptionBuilder
            .Create()
            .SetDurationData(DurationType.Minute, 1)
            .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
            .SetParticleEffectParameters(FeatureDefinitionPowers.PowerTraditionCourtMageSpellShield)
            .SetEffectForms(
                EffectFormBuilder
                    .Create()
                    .SetTempHpForm(0, DieType.D1, 0, true)
                    .Build())
            .Build();

        var spriteManaShield = Sprites.GetSprite("PowerManaShield", Resources.PowerManaShield, 256, 128);

        var powerManaShield = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ManaShield")
            .SetGuiPresentation(MANA_SHIELD_NAME, Category.Feature, spriteManaShield)
            .SetUsesProficiencyBonus(ActivationTime.BonusAction)
            .SetEffectDescription(effectDescriptionManaShield)
            .AddToDB();

        var powerManaShieldPoints = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ManaShieldPoints")
            .SetGuiPresentation(MANA_SHIELD_NAME, Category.Feature, spriteManaShield)
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.SorceryPoints, 2)
            .SetEffectDescription(effectDescriptionManaShield)
            .AddToDB();

        powerManaShield.SetCustomSubFeatures(
            new ModifyMagicEffectManaShield(false, powerManaShieldPoints),
            new PowerVisibilityModifierManaShield());

        powerManaShieldPoints.SetCustomSubFeatures(
            new ModifyMagicEffectManaShield(true, powerManaShieldPoints),
            new PowerVisibilityModifierManaShieldPoints(powerManaShield));

        var featureSetManaShield = FeatureDefinitionFeatureSetBuilder
            .Create(MANA_SHIELD_NAME)
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(powerManaShield, powerManaShieldPoints)
            .AddToDB();

        // LEVEL 06

        // all from common builders

        // LEVEL 14

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

        // had to keep this name off standard for reasons
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
            .SetProficiencies(
                ProficiencyType.SavingThrow,
                AttributeDefinitions.Dexterity)
            .AddToDB();

        var featureSetBattleReflexes = FeatureDefinitionFeatureSetBuilder
            .Create(BATTLE_REFLEXES_NAME)
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(attributeModifierBattleReflexes, proficiencyBattleReflexes)
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass,
                Sprites.GetSprite("SorcererSpellBlade", Resources.SorcererSpellBlade, 256))
            .AddFeaturesAtLevel(1,
                featureEnchantWeapon,
                featureSetMartialTraining,
                autoPreparedSpells,
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

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceSorcerousOrigin;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    //
    // Empower Weapon
    //

    private static bool CanWeaponBeEmpowered(RulesetAttackMode mode, RulesetItem item, RulesetCharacter character)
    {
        if (item == null)
        {
            return false;
        }

        var definition = item.ItemDefinition;

        if (definition == null ||
            !definition.IsWeapon ||
            !character.IsProficientWithItem(definition))
        {
            return false;
        }

        if (character is not RulesetCharacterHero hero)
        {
            return false;
        }

        if (mode.ActionType == ActionDefinitions.ActionType.Bonus &&
            !hero.TrainedFightingStyles.Contains(GetDefinition<FightingStyleDefinition>("TwoWeapon")))
        {
            return false;
        }

        return !definition.WeaponDescription.WeaponTags.Contains(TagsDefinitions.WeaponTagTwoHanded);
    }

    //
    // Mana Shield
    //

    private sealed class PowerVisibilityModifierManaShield : PowerVisibilityModifier
    {
        public PowerVisibilityModifierManaShield() : base((character, power, _) =>
            character.CanUsePower(power) || character.RemainingSorceryPoints < 2)
        {
        }
    }

    private sealed class PowerVisibilityModifierManaShieldPoints : PowerVisibilityModifier
    {
        public PowerVisibilityModifierManaShieldPoints(FeatureDefinitionPower powerManaShield) :
            base((character, _, _) =>
                !character.CanUsePower(powerManaShield) && character.RemainingSorceryPoints >= 2)
        {
        }
    }

    private sealed class ModifyMagicEffectManaShield : IModifyMagicEffect
    {
        private readonly bool _consumedSlots;
        private readonly FeatureDefinitionPower _featureDefinitionPower;

        public ModifyMagicEffectManaShield(bool consumedSlots, FeatureDefinitionPower featureDefinitionPower)
        {
            _consumedSlots = consumedSlots;
            _featureDefinitionPower = featureDefinitionPower;
        }

        public EffectDescription ModifyEffect(
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

            if (_consumedSlots)
            {
                character.UsablePowers
                    .FirstOrDefault(x => x.PowerDefinition == _featureDefinitionPower)
                    ?.RepayUse();
            }

            return effectDescription;
        }
    }
}
