using System;
using SolastaCommunityExpansion.Api.Extensions;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using static SolastaCommunityExpansion.Builders.Features.AutoPreparedSpellsGroupBuilder;
using static SolastaCommunityExpansion.Api.DatabaseHelper;
using static SolastaCommunityExpansion.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaCommunityExpansion.Subclasses.Sorcerer;

internal sealed class DivineHeart : AbstractSubclass
{
    private static readonly Guid SubclassNamespace = new("58ce31d8-6a37-4d6e-adbd-b9b60658a3ef");
    private readonly CharacterSubclassDefinition Subclass;

    internal DivineHeart()
    {
        var divineHeartArun = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create("AutoPreparedSpellsSorcererDivineHeartArun", SubclassNamespace)
            .SetGuiPresentation(Category.Feature)
            .SetCastingClass(CharacterClassDefinitions.Sorcerer)
            .SetPreparedSpellGroups(
                BuildSpellGroup(1, ProtectionFromEvilGood))
            .AddToDB();

        var divineHeartEinar = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create("AutoPreparedSpellsSorcererDivineHeartEinar", SubclassNamespace)
            .SetGuiPresentation(Category.Feature)
            .SetCastingClass(CharacterClassDefinitions.Sorcerer)
            .SetPreparedSpellGroups(
                BuildSpellGroup(1, InflictWounds))
            .AddToDB();

        var divineHeartMariake = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create("AutoPreparedSpellsSorcererDivineHeartMariake", SubclassNamespace)
            .SetGuiPresentation(Category.Feature)
            .SetCastingClass(CharacterClassDefinitions.Sorcerer)
            .SetPreparedSpellGroups(
                BuildSpellGroup(1, CureWounds))
            .AddToDB();

        var divineHeartMisaye = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create("AutoPreparedSpellsSorcererDivineHeartMisaye", SubclassNamespace)
            .SetGuiPresentation(Category.Feature)
            .SetCastingClass(CharacterClassDefinitions.Sorcerer)
            .SetPreparedSpellGroups(
                BuildSpellGroup(1, Bane))
            .AddToDB();

        var divineHeartPakri = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create("AutoPreparedSpellsSorcererDivineHeartPakri", SubclassNamespace)
            .SetGuiPresentation(Category.Feature)
            .SetCastingClass(CharacterClassDefinitions.Sorcerer)
            .SetPreparedSpellGroups(
                BuildSpellGroup(1, Bless))
            .AddToDB();

        var divineHeartDeityChoice = FeatureDefinitionFeatureSetBuilder
            .Create(FeatureDefinitionFeatureSets.FeatureSetSorcererDraconicChoice, "FeatureSetDivineHeartDeityChoice",
                SubclassNamespace)
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
                "AttributeModifierDivineHeartDivineFortitude", SubclassNamespace)
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        var magicAffinityDivineHeartClericSpellsList = FeatureDefinitionMagicAffinityBuilder
            .Create(FeatureDefinitionMagicAffinitys.MagicAffinityGreenmageGreenMagicList,
                "MagicAffinityDivineHeartClericSpellsList", SubclassNamespace)
            .SetGuiPresentation(Category.Feature)
            .SetExtendedSpellList(SpellListDefinitions.SpellListCleric)
            .AddToDB();

        var divineHeartEmpoweredHealingModifier = FeatureDefinitionDieRollModifierBuilder
            .Create(FeatureDefinitionDieRollModifiers.DieRollModifierEmpoweredSpell,
                "DieRollModifierDivineHeartEmpoweredHealing", SubclassNamespace)
            .SetGuiPresentation("Feature/&PowerDivineHeartEmpoweredHealingTitle",
                "Feature/&PowerDivineHeartEmpoweredHealingDescription")
            .AddToDB();

        divineHeartEmpoweredHealingModifier.validityContext = RuleDefinitions.RollContext.HealValueRoll;
        divineHeartEmpoweredHealingModifier.rerollLocalizationKey =
            "Feature/&PowerDivineHeartEmpoweredHealingRerollDescription";

        var divineHeartEmpoweredHealingCondition = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionSorcererChildRiftDeflection, "ConditionDivineHeartEmpoweredHealing",
                SubclassNamespace)
            .SetGuiPresentation(Category.Condition)
            .SetFeatures(divineHeartEmpoweredHealingModifier)
            .AddToDB();

        var divineHeartEmpoweredHealingPower = FeatureDefinitionPowerBuilder
            .Create(FeatureDefinitionPowers.PowerSorcererChildRiftDeflection, "PowerDivineHeartEmpoweredHealing",
                SubclassNamespace)
            .SetGuiPresentation(Category.Feature, HealingWord.GuiPresentation.SpriteReference)
            .AddToDB();
        divineHeartEmpoweredHealingPower.EffectDescription.EffectForms[0].ConditionForm.conditionDefinition =
            divineHeartEmpoweredHealingCondition;

        var divineHeartPlanarPortalPower = FeatureDefinitionPowerBuilder
            .Create("PowerDivineHeartPlanarPortal", SubclassNamespace)
            .SetGuiPresentation(Category.Feature, DimensionDoor.GuiPresentation.SpriteReference)
            .SetEffectDescription(DimensionDoor.EffectDescription.Copy())
            .SetActivationTime(RuleDefinitions.ActivationTime.Action)
            .SetFixedUsesPerRecharge(1)
            .SetRechargeRate(RuleDefinitions.RechargeRate.AtWill)
            .SetCostPerUse(1)
            .SetShowCasting(true)
            .AddToDB();

        var divineHeartDivineRecoveryPower = FeatureDefinitionPowerBuilder
            .Create("PowerDivineHeartDivineRecovery", SubclassNamespace)
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
            .Create("DivineHeart", SubclassNamespace)
            .SetGuiPresentation("SorcerousDivineHeart", Category.Subclass,
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
