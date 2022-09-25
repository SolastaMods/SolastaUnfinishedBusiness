using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Builders.Features.AutoPreparedSpellsGroupBuilder;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaUnfinishedBusiness.Classes.Artisan.Subclasses;

public static class BattleSmithBuilder
{
    public static CharacterSubclassDefinition Build(CharacterClassDefinition castingClass)
    {
        var autoPreparedSpellsArtisanBattleSmith = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create("AutoPreparedSpellsArtisanBattleSmith")
            .SetGuiPresentation(Category.Feature)
            .SetCastingClass(castingClass)
            .SetPreparedSpellGroups(
                BuildSpellGroup(3, Heroism, Shield, HuntersMark),
                BuildSpellGroup(5, BrandingSmite, SpiritualWeapon),
                BuildSpellGroup(9, RemoveCurse, BeaconOfHope),
                BuildSpellGroup(13, FireShield, DeathWard),
                BuildSpellGroup(17, MassCureWounds, WallOfForce))
            .AddToDB();

        var proficiencyArtisanBattleSmithWeapon = ArtisanHelpers
            .BuildProficiency("ProficiencyArtisanBattleSmithWeapon", ProficiencyType.Weapon,
                EquipmentDefinitions.MartialWeaponCategory)
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        var powerPoolModifierArtisanBattleSmithInfusionPool = FeatureDefinitionPowerPoolModifierBuilder
            .Create("PowerPoolModifierArtisanBattleSmithInfusionPool")
            .Configure(
                2,
                UsesDetermination.Fixed,
                AttributeDefinitions.Intelligence,
                ArtisanClass.PowerPoolArtisanInfusion)
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        // note this is not magical because that causes a conflict with the enhanced weapon effect
        var attackModifierArtisanBattleSmithWeapon = FeatureDefinitionAttackModifierBuilder
            .Create("AttackModifierArtisanBattleSmithWeapon")
            .SetGuiPresentation(Category.Feature,
                FeatureDefinitionAttackModifiers.AttackModifierMagicWeapon.GuiPresentation.SpriteReference)
            .SetAbilityScoreReplacement(AbilityScoreReplacement.SpellcastingAbility)
            .AddToDB();

        var enchantWeapon = ArtisanInfusionHelper.BuildItemModifierInfusion(
                attackModifierArtisanBattleSmithWeapon,
                ActionDefinitions.ItemSelectionType.Weapon,
                "BattleSmithWeapon",
                FeatureDefinitionPowers.PowerDomainElementalLightningBlade.GuiPresentation.SpriteReference)
            .AddToDB();

        var attributeModifierArtisanBattleSmithExtraAttack = ArtisanHelpers.BuildAttributeModifier(
            "AttributeModifierArtisanBattleSmithExtraAttack",
            FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
            AttributeDefinitions.AttacksNumber,
            1,
            "AttributeModifierArtisanBattleSmithExtraAttack",
            FeatureDefinitionAttributeModifiers.AttributeModifierFighterExtraAttack.GuiPresentation.SpriteReference);

        // note ability score bonus only works if it's applied to a weapon, not a character
        var attackModifierArtisanBattleSmithJolt = ArtisanHelpers.BuildAttackModifier(
            "AttackModifierArtisanBattleSmithJolt",
            AttackModifierMethod.None,
            0,
            AttributeDefinitions.Intelligence,
            AttackModifierMethod.FlatValue,
            3,
            AttributeDefinitions.Intelligence,
            false,
            TagsDefinitions.Magical);

        // note ability score bonus only works if it's applied to a weapon, not a character
        var attackModifierArtisanBattleSmithJolt2 = ArtisanHelpers.BuildAttackModifier(
            "AttackModifierArtisanBattleSmithJolt2",
            AttackModifierMethod.None,
            0,
            AttributeDefinitions.Intelligence,
            AttackModifierMethod.FlatValue,
            3,
            AttributeDefinitions.Intelligence,
            false,
            TagsDefinitions.Magical);

        // Make Battle Smith subclass
        return CharacterSubclassDefinitionBuilder
            .Create("ArtisanBattleSmith")
            .SetGuiPresentation(Category.Subclass, MartialSpellblade.GuiPresentation.SpriteReference)
            .AddFeatureAtLevel(autoPreparedSpellsArtisanBattleSmith, 3)
            .AddFeatureAtLevel(proficiencyArtisanBattleSmithWeapon, 3)
            .AddFeatureAtLevel(powerPoolModifierArtisanBattleSmithInfusionPool, 3)
            .AddFeatureAtLevel(enchantWeapon, 3)
            .AddFeatureAtLevel(ProtectorConstructFeatureSetBuilder.ProtectorConstructFeatureSet, 3)
            .AddFeatureAtLevel(attributeModifierArtisanBattleSmithExtraAttack, 5)
            .AddFeatureAtLevel(attackModifierArtisanBattleSmithJolt, 9)
            .AddFeatureAtLevel(attackModifierArtisanBattleSmithJolt2, 15)
            .AddFeatureAtLevel(ProtectorConstructUpgradeFeatureSetBuilder.ProtectorConstructUpgradeFeatureSet, 15)
            .AddToDB();
    }
}
