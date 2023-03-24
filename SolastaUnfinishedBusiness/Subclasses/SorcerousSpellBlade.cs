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
        // LEVEL 03

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

        // Mana Shield

        const string MANA_SHIELD_NAME = $"FeatureSet{Name}ManaShield";

        var effectDescriptionManaShield = EffectDescriptionBuilder
            .Create()
            .SetDurationData(DurationType.Minute, 1)
            .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
            .SetEffectForms(
                EffectFormBuilder
                    .Create()
                    .SetTempHpForm(0, DieType.D1, 0, true)
                    .Build())
            .Build();

        var spriteManaShield = Sprites.GetSprite("PowerManaShield", Resources.PowerMedKit, 256, 128);

        var powerManaShield = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ManaShield")
            .SetGuiPresentation(MANA_SHIELD_NAME, Category.Feature, spriteManaShield)
            .SetUsesProficiencyBonus(ActivationTime.BonusAction)
            .SetEffectDescription(effectDescriptionManaShield)
            .SetCustomSubFeatures(
                new ModifyMagicEffectManaShield(false),
                new PowerVisibilityModifierManaShield())
            .AddToDB();

        var powerManaShieldPoints = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ManaShieldPoints")
            .SetGuiPresentation(MANA_SHIELD_NAME, Category.Feature, spriteManaShield)
            .SetUsesFixed(ActivationTime.BonusAction)
            .SetEffectDescription(effectDescriptionManaShield)
            .SetCustomSubFeatures(
                new ModifyMagicEffectManaShield(true),
                new PowerVisibilityModifierManaShieldPoints(powerManaShield))
            .AddToDB();

        var featureSetManaShield = FeatureDefinitionFeatureSetBuilder
            .Create(MANA_SHIELD_NAME)
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(powerManaShield, powerManaShieldPoints)
            .AddToDB();

        // LEVEL 06

        // all from common builders

        // LEVEL 14

        var additionalDamageWarSorcerer = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}WarSorcerer")
            .SetGuiPresentation(Category.Feature)
            .SetNotificationTag("WarSorcerer")
            .SetTriggerCondition(AdditionalDamageTriggerCondition.SpellDamagesTarget)
            .SetRequiredProperty(RestrictedContextRequiredProperty.SpellWithAttackRoll)
            .SetDamageValueDetermination(AdditionalDamageValueDetermination.SpellcastingBonus)
            .SetSpecificDamageType(DamageTypeForce)
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
                FeatureSetCasterFightingProficiency,
                PowerArcaneFighterEnchantWeapon,
                autoPreparedSpells)
            .AddFeaturesAtLevel(2,
                featureSetManaShield)
            .AddFeaturesAtLevel(6,
                AttributeModifierCasterFightingExtraAttack,
                ReplaceAttackWithCantripCasterFighting)
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

    private sealed class PowerVisibilityModifierManaShield : PowerVisibilityModifier
    {
        public PowerVisibilityModifierManaShield() : base((character, power, _) =>
        {
            var rulesetManaShield = character.UsablePowers.First(x => x.PowerDefinition == power);

            return rulesetManaShield.RemainingUses > 0 || character.RemainingSorceryPoints < 2;
        })
        {
        }
    }

    private sealed class PowerVisibilityModifierManaShieldPoints : PowerVisibilityModifier
    {
        public PowerVisibilityModifierManaShieldPoints(BaseDefinition powerManaShield) : base((character, _, _) =>
        {
            var rulesetManaShield = character.UsablePowers.First(x => x.PowerDefinition == powerManaShield);

            return rulesetManaShield.RemainingUses == 0 && character.RemainingSorceryPoints >= 2;
        })
        {
        }
    }

    private sealed class ModifyMagicEffectManaShield : IModifyMagicEffect
    {
        private readonly bool _consumeSlots;

        public ModifyMagicEffectManaShield(bool consumeSlots)
        {
            _consumeSlots = consumeSlots;
        }

        public EffectDescription ModifyEffect(
            BaseDefinition definition,
            EffectDescription effect,
            RulesetCharacter character)
        {
            var classLevel = character.GetClassLevel(CharacterClassDefinitions.Sorcerer);
            var charisma = character.TryGetAttributeValue(AttributeDefinitions.Charisma);
            var charismaModifier = AttributeDefinitions.ComputeAbilityScoreModifier(charisma);
            var healing = classLevel + Math.Min(1, charismaModifier);

            effect.EffectForms[0].TemporaryHitPointsForm.bonusHitPoints = healing;

            if (_consumeSlots)
            {
                character.SpendSorceryPoints(2);
            }

            return effect;
        }
    }
}
