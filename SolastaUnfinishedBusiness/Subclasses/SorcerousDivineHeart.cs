using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using static SolastaUnfinishedBusiness.Builders.Features.AutoPreparedSpellsGroupBuilder;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class SorcerousDivineHeart : AbstractSubclass
{
    internal SorcerousDivineHeart()
    {
        var autoPreparedSpellsDivineHeartArun = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create("AutoPreparedSpellsDivineHeartArun")
            .SetGuiPresentation(Category.Feature)
            .SetCastingClass(CharacterClassDefinitions.Sorcerer)
            .SetPreparedSpellGroups(
                BuildSpellGroup(1, ProtectionFromEvilGood))
            .AddToDB();

        var autoPreparedSpellsDivineHeartEinar = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create("AutoPreparedSpellsDivineHeartEinar")
            .SetGuiPresentation(Category.Feature)
            .SetCastingClass(CharacterClassDefinitions.Sorcerer)
            .SetPreparedSpellGroups(
                BuildSpellGroup(1, InflictWounds))
            .AddToDB();

        var autoPreparedSpellsDivineHeartMariake = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create("AutoPreparedSpellsDivineHeartMariake")
            .SetGuiPresentation(Category.Feature)
            .SetCastingClass(CharacterClassDefinitions.Sorcerer)
            .SetPreparedSpellGroups(
                BuildSpellGroup(1, CureWounds))
            .AddToDB();

        var autoPreparedSpellsDivineHeartMisaye = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create("AutoPreparedSpellsDivineHeartMisaye")
            .SetGuiPresentation(Category.Feature)
            .SetCastingClass(CharacterClassDefinitions.Sorcerer)
            .SetPreparedSpellGroups(
                BuildSpellGroup(1, Bane))
            .AddToDB();

        var autoPreparedSpellsDivineHeartPakri = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create("AutoPreparedSpellsDivineHeartPakri")
            .SetGuiPresentation(Category.Feature)
            .SetCastingClass(CharacterClassDefinitions.Sorcerer)
            .SetPreparedSpellGroups(
                BuildSpellGroup(1, Bless))
            .AddToDB();

        var featureSetDivineHeartDeityChoice = FeatureDefinitionFeatureSetBuilder
            .Create(FeatureDefinitionFeatureSets.FeatureSetSorcererDraconicChoice, "FeatureSetDivineHeartDeityChoice")
            .SetGuiPresentation(Category.Feature)
            .SetFeatureSet(
                autoPreparedSpellsDivineHeartArun,
                autoPreparedSpellsDivineHeartEinar,
                autoPreparedSpellsDivineHeartMariake,
                autoPreparedSpellsDivineHeartMisaye,
                autoPreparedSpellsDivineHeartPakri)
            .AddToDB();

        var attributeModifierDivineHeartDivineFortitude = FeatureDefinitionAttributeModifierBuilder
            .Create(FeatureDefinitionAttributeModifiers.AttributeModifierDwarfHillToughness,
                "AttributeModifierDivineHeartDivineFortitude")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        var magicAffinityDivineHeartClericSpellsList = FeatureDefinitionMagicAffinityBuilder
            .Create(FeatureDefinitionMagicAffinitys.MagicAffinityGreenmageGreenMagicList,
                "MagicAffinityDivineHeartClericSpellsList")
            .SetGuiPresentation(Category.Feature)
            .SetExtendedSpellList(SpellListDefinitions.SpellListCleric)
            .AddToDB();

        var conditionDivineHeartEmpoweredHealing = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionSorcererChildRiftDeflection, "ConditionDivineHeartEmpoweredHealing")
            .SetOrUpdateGuiPresentation(Category.Condition)
            .SetFeatures(
                FeatureDefinitionDieRollModifierBuilder
                    .Create(FeatureDefinitionDieRollModifiers.DieRollModifierEmpoweredSpell,
                        "DieRollModifierDivineHeartEmpoweredHealing")
                    .SetGuiPresentation("PowerDivineHeartEmpoweredHealing", Category.Feature)
                    .SetModifiers(
                        RollContext.HealValueRoll,
                        1,
                        0,
                        2,
                        "Feature/&PowerDivineHeartEmpoweredHealingRerollDescription")
                    .AddToDB())
            .AddToDB();

        var powerDivineHeartEmpoweredHealing = FeatureDefinitionPowerBuilder
            .Create(FeatureDefinitionPowers.PowerSorcererChildRiftDeflection, "PowerDivineHeartEmpoweredHealing")
            .SetGuiPresentation(Category.Feature, HealingWord.GuiPresentation.SpriteReference)
            .AddToDB();

        powerDivineHeartEmpoweredHealing.EffectDescription.EffectForms[0].ConditionForm.conditionDefinition =
            conditionDivineHeartEmpoweredHealing;

        var powerDivineHeartPlanarPortal = FeatureDefinitionPowerBuilder
            .Create("PowerDivineHeartPlanarPortal")
            .SetGuiPresentation(Category.Feature, DimensionDoor.GuiPresentation.SpriteReference)
            .SetEffectDescription(DimensionDoor.EffectDescription.Copy())
            .SetActivationTime(ActivationTime.Action)
            .SetFixedUsesPerRecharge(1)
            .SetRechargeRate(RechargeRate.AtWill)
            .SetCostPerUse(1)
            .SetShowCasting(true)
            .AddToDB();

        var powerDivineHeartDivineRecovery = FeatureDefinitionPowerBuilder
            .Create("PowerDivineHeartDivineRecovery")
            .SetGuiPresentation(Category.Feature, Heal.GuiPresentation.SpriteReference)
            .SetEffectDescription(Heal.EffectDescription.Copy())
            .SetActivationTime(ActivationTime.BonusAction)
            .SetFixedUsesPerRecharge(1)
            .SetRechargeRate(RechargeRate.LongRest)
            .SetCostPerUse(1)
            .SetShowCasting(true)
            .AddToDB();

        powerDivineHeartDivineRecovery.EffectDescription.rangeType = RangeType.Self;
        powerDivineHeartDivineRecovery.EffectDescription.targetType = TargetType.Self;

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("SorcerousDivineHeart")
            .SetGuiPresentation(Category.Subclass, DomainLife.GuiPresentation.SpriteReference)
            .AddFeaturesAtLevel(1,
                featureSetDivineHeartDeityChoice,
                attributeModifierDivineHeartDivineFortitude,
                magicAffinityDivineHeartClericSpellsList)
            .AddFeaturesAtLevel(6,
                powerDivineHeartEmpoweredHealing)
            .AddFeaturesAtLevel(14,
                powerDivineHeartPlanarPortal)
            .AddFeaturesAtLevel(18,
                powerDivineHeartDivineRecovery)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceSorcerousOrigin;
}
