using System;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.BehaviorsGeneric;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
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

        var attackModifierEnchantWeapon = FeatureDefinitionAttackModifierBuilder
            .Create($"AttackModifier{Name}EnchantWeapon")
            .SetGuiPresentation("AttackModifierEnchantWeapon", Category.Feature)
            .AddCustomSubFeatures(
                new CanUseAttribute(AttributeDefinitions.Charisma, CanWeaponBeEnchanted),
                new AddTagToWeaponWeaponAttack(TagsDefinitions.MagicalWeapon, CanWeaponBeEnchanted))
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
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.SorceryPoints, 2, 0)
            .SetEffectDescription(effectDescriptionManaShield)
            .AddToDB();

        powerManaShield.AddCustomSubFeatures(
            new ModifyEffectDescriptionManaShield(powerManaShield, powerManaShieldPoints),
            new ModifyPowerVisibilityManaShield());

        powerManaShieldPoints.AddCustomSubFeatures(
            new ModifyEffectDescriptionManaShield(powerManaShieldPoints, powerManaShieldPoints),
            new ModifyPowerVisibilityManaShieldPoints(powerManaShield));

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

    private sealed class ModifyPowerVisibilityManaShield() : ModifyPowerVisibility((character, power, _) =>
        character.CanUsePower(power) || character.RemainingSorceryPoints < 2);

    private sealed class ModifyPowerVisibilityManaShieldPoints(FeatureDefinitionPower powerManaShield)
        : ModifyPowerVisibility((character, _, _) =>
            !character.CanUsePower(powerManaShield) && character.RemainingSorceryPoints >= 2);

    private sealed class ModifyEffectDescriptionManaShield(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinitionPower baseDefinition,
        FeatureDefinitionPower powerManaShieldPoints)
        : IModifyEffectDescription
    {
        public bool IsValid(
            BaseDefinition definition,
            RulesetCharacter character,
            EffectDescription effectDescription)
        {
            return definition == baseDefinition;
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

            //TODO: refactor this - we should not do actions when modifying effect descriptions
            if (baseDefinition != powerManaShieldPoints)
            {
                return effectDescription;
            }

            var usablePower = PowerProvider.Get(powerManaShieldPoints, character);

            character.RepayPowerUse(usablePower);

            return effectDescription;
        }
    }
}
