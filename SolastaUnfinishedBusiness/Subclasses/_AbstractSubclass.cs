using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;

namespace SolastaUnfinishedBusiness.Subclasses;

internal abstract class AbstractSubclass
{
    internal static readonly FeatureDefinitionAttributeModifier AttributeModifierCasterFightingExtraAttack =
        FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierCasterFightingExtraAttack")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(
                FeatureDefinitionAttributeModifier.AttributeModifierOperation.ForceIfBetter,
                AttributeDefinitions.AttacksNumber, 2)
            .AddToDB();

    internal static readonly FeatureDefinitionFeatureSet FeatureSetCasterFighting = FeatureDefinitionFeatureSetBuilder
        .Create("FeatureSetCasterFighting")
        .SetGuiPresentation(Category.Feature)
        .AddFeatureSet(
            FeatureDefinitionProficiencyBuilder
                .Create("ProficiencyCasterFightingArmor")
                .SetGuiPresentationNoContent(true)
                .SetProficiencies(RuleDefinitions.ProficiencyType.Armor,
                    EquipmentDefinitions.LightArmorCategory,
                    EquipmentDefinitions.MediumArmorCategory,
                    EquipmentDefinitions.ShieldCategory)
                .AddToDB(),
            FeatureDefinitionProficiencyBuilder
                .Create("ProficiencyCasterFightingWeapon")
                .SetGuiPresentationNoContent(true)
                .SetProficiencies(RuleDefinitions.ProficiencyType.Weapon,
                    EquipmentDefinitions.SimpleWeaponCategory,
                    EquipmentDefinitions.MartialWeaponCategory)
                .AddToDB())
        .AddToDB();

    internal abstract CharacterSubclassDefinition Subclass { get; }
    internal abstract FeatureDefinitionSubclassChoice SubclassChoice { get; }
}
