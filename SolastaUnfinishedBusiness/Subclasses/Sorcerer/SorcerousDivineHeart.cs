using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using static SolastaUnfinishedBusiness.Builders.Features.AutoPreparedSpellsGroupBuilder;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses.Sorcerer;

internal sealed class SorcerousDivineHeart : AbstractSubclass
{
    private readonly CharacterSubclassDefinition Subclass;

    internal SorcerousDivineHeart()
    {
        var divineHeartArun = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create("AutoPreparedSpellsDivineHeartArun", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Feature)
            .SetCastingClass(CharacterClassDefinitions.Sorcerer)
            .SetPreparedSpellGroups(
                BuildSpellGroup(1, ProtectionFromEvilGood))
            .AddToDB();

        var divineHeartEinar = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create("AutoPreparedSpellsDivineHeartEinar", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Feature)
            .SetCastingClass(CharacterClassDefinitions.Sorcerer)
            .SetPreparedSpellGroups(
                BuildSpellGroup(1, InflictWounds))
            .AddToDB();

        var divineHeartMariake = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create("AutoPreparedSpellsDivineHeartMariake", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Feature)
            .SetCastingClass(CharacterClassDefinitions.Sorcerer)
            .SetPreparedSpellGroups(
                BuildSpellGroup(1, CureWounds))
            .AddToDB();

        var divineHeartMisaye = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create("AutoPreparedSpellsDivineHeartMisaye", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Feature)
            .SetCastingClass(CharacterClassDefinitions.Sorcerer)
            .SetPreparedSpellGroups(
                BuildSpellGroup(1, Bane))
            .AddToDB();

        var divineHeartPakri = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create("AutoPreparedSpellsDivineHeartPakri", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Feature)
            .SetCastingClass(CharacterClassDefinitions.Sorcerer)
            .SetPreparedSpellGroups(
                BuildSpellGroup(1, Bless))
            .AddToDB();

        var divineHeartDeityChoice = FeatureDefinitionFeatureSetBuilder
            .Create(FeatureDefinitionFeatureSets.FeatureSetSorcererDraconicChoice, "FeatureSetDivineHeartDeityChoice",
                DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Feature)
            .SetFeatureSet(
                divineHeartArun,
                divineHeartEinar,
                divineHeartMariake,
                divineHeartMisaye,
                divineHeartPakri)
            .AddToDB();

        var divineHeartDivineFortitude = FeatureDefinitionAttributeModifierBuilder
            .Create(FeatureDefinitionAttributeModifiers.AttributeModifierDwarfHillToughness,
                "AttributeModifierDivineHeartDivineFortitude", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        var magicAffinityDivineHeartClericSpellsList = FeatureDefinitionMagicAffinityBuilder
            .Create(FeatureDefinitionMagicAffinitys.MagicAffinityGreenmageGreenMagicList,
                "MagicAffinityDivineHeartClericSpellsList", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Feature)
            .SetExtendedSpellList(SpellListDefinitions.SpellListCleric)
            .AddToDB();

        var divineHeartEmpoweredHealingModifier = FeatureDefinitionDieRollModifierBuilder
            .Create(FeatureDefinitionDieRollModifiers.DieRollModifierEmpoweredSpell,
                "DieRollModifierDivineHeartEmpoweredHealing", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation("Feature/&PowerDivineHeartEmpoweredHealingTitle",
                "Feature/&PowerDivineHeartEmpoweredHealingDescription")
            .AddToDB();

        divineHeartEmpoweredHealingModifier.validityContext = RuleDefinitions.RollContext.HealValueRoll;
        divineHeartEmpoweredHealingModifier.rerollLocalizationKey =
            "Feature/&PowerDivineHeartEmpoweredHealingRerollDescription";

        var divineHeartEmpoweredHealingCondition = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionSorcererChildRiftDeflection, "ConditionDivineHeartEmpoweredHealing",
                DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Condition)
            .SetFeatures(divineHeartEmpoweredHealingModifier)
            .AddToDB();

        var divineHeartEmpoweredHealingPower = FeatureDefinitionPowerBuilder
            .Create(FeatureDefinitionPowers.PowerSorcererChildRiftDeflection, "PowerDivineHeartEmpoweredHealing",
                DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Feature, HealingWord.GuiPresentation.SpriteReference)
            .AddToDB();
        divineHeartEmpoweredHealingPower.EffectDescription.EffectForms[0].ConditionForm.conditionDefinition =
            divineHeartEmpoweredHealingCondition;

        var divineHeartPlanarPortalPower = FeatureDefinitionPowerBuilder
            .Create("PowerDivineHeartPlanarPortal", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Feature, DimensionDoor.GuiPresentation.SpriteReference)
            .SetEffectDescription(DimensionDoor.EffectDescription.Copy())
            .SetActivationTime(RuleDefinitions.ActivationTime.Action)
            .SetFixedUsesPerRecharge(1)
            .SetRechargeRate(RuleDefinitions.RechargeRate.AtWill)
            .SetCostPerUse(1)
            .SetShowCasting(true)
            .AddToDB();

        var divineHeartDivineRecoveryPower = FeatureDefinitionPowerBuilder
            .Create("PowerDivineHeartDivineRecovery", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Feature, Heal.GuiPresentation.SpriteReference)
            .SetEffectDescription(Heal.EffectDescription.Copy())
            .SetActivationTime(RuleDefinitions.ActivationTime.BonusAction)
            .SetFixedUsesPerRecharge(1)
            .SetRechargeRate(RuleDefinitions.RechargeRate.LongRest)
            .SetCostPerUse(1)
            .SetShowCasting(true)
            .AddToDB();
        divineHeartDivineRecoveryPower.EffectDescription.rangeType = RuleDefinitions.RangeType.Self;
        divineHeartDivineRecoveryPower.EffectDescription.targetType = RuleDefinitions.TargetType.Self;

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("SorcerousDivineHeart", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Subclass,
                DomainLife.GuiPresentation.SpriteReference)
            .AddFeatureAtLevel(divineHeartDeityChoice, 1)
            .AddFeatureAtLevel(divineHeartDivineFortitude, 1)
            .AddFeatureAtLevel(magicAffinityDivineHeartClericSpellsList, 1)
            .AddFeatureAtLevel(divineHeartEmpoweredHealingPower, 6)
            .AddFeatureAtLevel(divineHeartPlanarPortalPower, 14)
            .AddFeatureAtLevel(divineHeartDivineRecoveryPower, 18)
            .AddToDB();
    }

    internal override FeatureDefinitionSubclassChoice GetSubclassChoiceList()
    {
        return FeatureDefinitionSubclassChoices.SubclassChoiceSorcerousOrigin;
    }

    internal override CharacterSubclassDefinition GetSubclass()
    {
        return Subclass;
    }
}
