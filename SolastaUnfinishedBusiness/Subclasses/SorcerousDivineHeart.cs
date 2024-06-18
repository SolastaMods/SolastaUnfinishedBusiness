using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static FeatureDefinitionAttributeModifier;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class SorcerousDivineHeart : AbstractSubclass
{
    private const string Name = "DivineHeart";
    private const string OriginTag = "Origin";

    public SorcerousDivineHeart()
    {
        // LEVEL 01

        // Deity Choice

        var autoPreparedSpellsDivineHeartArun = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create($"AutoPreparedSpells{Name}Arun")
            .SetGuiPresentation(Category.Feature)
            .SetAutoTag(OriginTag)
            .SetSpellcastingClass(CharacterClassDefinitions.Sorcerer)
            .AddPreparedSpellGroup(1, ProtectionFromEvilGood)
            .AddToDB();

        var autoPreparedSpellsDivineHeartEinar = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create($"AutoPreparedSpells{Name}Einar")
            .SetGuiPresentation(Category.Feature)
            .SetAutoTag(OriginTag)
            .SetSpellcastingClass(CharacterClassDefinitions.Sorcerer)
            .AddPreparedSpellGroup(1, InflictWounds)
            .AddToDB();

        var autoPreparedSpellsDivineHeartMariake = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create($"AutoPreparedSpells{Name}Mariake")
            .SetGuiPresentation(Category.Feature)
            .SetAutoTag(OriginTag)
            .SetSpellcastingClass(CharacterClassDefinitions.Sorcerer)
            .AddPreparedSpellGroup(1, CureWounds)
            .AddToDB();

        var autoPreparedSpellsDivineHeartMisaye = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create($"AutoPreparedSpells{Name}Misaye")
            .SetGuiPresentation(Category.Feature)
            .SetAutoTag(OriginTag)
            .SetSpellcastingClass(CharacterClassDefinitions.Sorcerer)
            .AddPreparedSpellGroup(1, Bane)
            .AddToDB();

        var autoPreparedSpellsDivineHeartPakri = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create($"AutoPreparedSpells{Name}Pakri")
            .SetGuiPresentation(Category.Feature)
            .SetAutoTag(OriginTag)
            .SetSpellcastingClass(CharacterClassDefinitions.Sorcerer)
            .AddPreparedSpellGroup(1, Bless)
            .AddToDB();

        var featureSetDivineHeartDeityChoice = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}DeityChoice")
            .SetGuiPresentation(Category.Feature)
            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Exclusion)
            .AddFeatureSet(
                autoPreparedSpellsDivineHeartArun,
                autoPreparedSpellsDivineHeartEinar,
                autoPreparedSpellsDivineHeartMariake,
                autoPreparedSpellsDivineHeartMisaye,
                autoPreparedSpellsDivineHeartPakri)
            .AddToDB();

        // Divine Fortitude

        var attributeModifierDivineHeartDivineFortitude = FeatureDefinitionAttributeModifierBuilder
            .Create($"AttributeModifier{Name}DivineFortitude")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.HitPointBonusPerLevel, 1)
            .AddToDB();

        // Extended Spells

        var magicAffinityDivineHeartClericSpellsList = FeatureDefinitionMagicAffinityBuilder
            .Create($"MagicAffinity{Name}ClericSpellsList")
            .SetGuiPresentation(Category.Feature)
            .SetExtendedSpellList(SpellListDefinitions.SpellListCleric)
            .AddToDB();

        // LEVEL 06

        // Empowered Healing

        var dieRollModifierEmpoweredHealing = FeatureDefinitionDieRollModifierBuilder
            .Create($"DieRollModifier{Name}EmpoweredHealing")
            .SetGuiPresentation($"Power{Name}EmpoweredHealing", Category.Feature, Global.Empty)
            .SetModifiers(
                RollContext.HealValueRoll,
                1,
                1,
                2,
                $"Feature/&Power{Name}EmpoweredHealingReroll")
            .AddToDB();

        var conditionDivineHeartEmpoweredHealing = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionSorcererChildRiftDeflection, $"Condition{Name}EmpoweredHealing")
            .SetOrUpdateGuiPresentation(Category.Condition)
            .SetFeatures(dieRollModifierEmpoweredHealing)
            .AddToDB();

        var powerDivineHeartEmpoweredHealing = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}EmpoweredHealing")
            .SetGuiPresentation(Category.Feature, HealingWord)
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.SorceryPoints, 1, 0)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(
                                conditionDivineHeartEmpoweredHealing,
                                ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();

        // Divine Fount

        var powerDivineHeartDivineFount = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}DivineFount")
            .SetGuiPresentation(Category.Feature, BeaconOfHope)
            .SetUsesAbilityBonus(
                ActivationTime.BonusAction, RechargeRate.LongRest, AttributeDefinitions.Wisdom)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(PowerSorcererManaPainterDrain.EffectDescription.EffectForms[1])
                    .Build())
            .AddToDB();

        // LEVEL 14

        // Planar Portal

        var powerDivineHeartPlanarPortal = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}PlanarPortal")
            .SetGuiPresentation(Category.Feature, DimensionDoor)
            .SetUsesFixed(ActivationTime.Action)
            .SetEffectDescription(DimensionDoor.EffectDescription)
            .AddToDB();

        // LEVEL 18

        // Divine Recovery

        var powerDivineHeartDivineRecovery = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}DivineRecovery")
            .SetGuiPresentation(Category.Feature, Heal)
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.LongRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(Heal.EffectDescription)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .Build())
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create($"Sorcerous{Name}")
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.SorcererDivineHeart, 256))
            .AddFeaturesAtLevel(1,
                featureSetDivineHeartDeityChoice,
                attributeModifierDivineHeartDivineFortitude,
                magicAffinityDivineHeartClericSpellsList)
            .AddFeaturesAtLevel(6,
                powerDivineHeartEmpoweredHealing,
                powerDivineHeartDivineFount)
            .AddFeaturesAtLevel(14,
                powerDivineHeartPlanarPortal)
            .AddFeaturesAtLevel(18,
                powerDivineHeartDivineRecovery)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Sorcerer;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceSorcerousOrigin;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }
}
