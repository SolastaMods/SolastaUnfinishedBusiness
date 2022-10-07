using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using static FeatureDefinitionAttributeModifier;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaUnfinishedBusiness.Classes.Inventor.Subclasses;

public static class InnovationWeapon
{
    public static CharacterSubclassDefinition Build()
    {
        return CharacterSubclassDefinitionBuilder
            .Create("InnovationWeapon")
            .SetGuiPresentation(Category.Subclass, CharacterSubclassDefinitions.OathOfJugement)
            .AddFeaturesAtLevel(3, BuildBattleReady(), BuildAutoPreparedSpells())
            .AddFeaturesAtLevel(5, BuildExtraAttack())
            .AddToDB();
    }

    private static FeatureDefinition BuildBattleReady()
    {
        return FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyInnovationWeaponBattleReady")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(RuleDefinitions.ProficiencyType.Weapon, EquipmentDefinitions.MartialWeaponCategory)
            .SetCustomSubFeatures(new CanUseAttributeForWeapon(AttributeDefinitions.Intelligence,
                ValidatorsWeapon.IsMagic))
            .AddToDB();
    }

    private static FeatureDefinition BuildAutoPreparedSpells()
    {Main.Log2($"BuildAutoPreparedSpells class: <{InventorClass.Class}>");
        return FeatureDefinitionAutoPreparedSpellsBuilder
            .Create("AutoPreparedSpellsInnovationWeapon")
            .SetGuiPresentation(Category.Feature)
            .SetSpellcastingClass(InventorClass.Class)
            .SetAutoTag("InventorWeaponsmith")
            .AddPreparedSpellGroup(3, Heroism, Shield)
            .AddPreparedSpellGroup(5, BrandingSmite, SpiritualWeapon)
            .AddPreparedSpellGroup(9, RemoveCurse, BeaconOfHope)
            .AddPreparedSpellGroup(13, FireShield, DeathWard)
            .AddPreparedSpellGroup(17, MassCureWounds, WallOfForce)
            .AddToDB();
    }

    private static FeatureDefinition BuildExtraAttack()
    {
        return FeatureDefinitionAttributeModifierBuilder
            .Create("ProficiencyInnovationWeaponExtraAttack")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(AttributeModifierOperation.ForceIfBetter, AttributeDefinitions.AttacksNumber, 2)
            .AddToDB();
    }
}
