using System;
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
    public static CharacterSubclassDefinition Build(CharacterClassDefinition artisan)
    {
        // Make Battle Smith subclass
        var battleSmith = CharacterSubclassDefinitionBuilder
            .Create("BattleSmith", ArtisanClass.GuidNamespace)
            .SetGuiPresentation("ArtisanBattleSmith", Category.Subclass,
                MartialSpellblade.GuiPresentation.SpriteReference);

        var battleSmithPrepSpells = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create("ArtisanBattleSmithAutoPrepSpells", ArtisanClass.GuidNamespace)
            .SetGuiPresentation("BattleSmithSubclassSpells", Category.Feat)
            .SetCastingClass(artisan)
            .SetPreparedSpellGroups(
                BuildSpellGroup(3, Heroism, Shield, HuntersMark),
                BuildSpellGroup(5, BrandingSmite, SpiritualWeapon),
                BuildSpellGroup(9, RemoveCurse, BeaconOfHope),
                BuildSpellGroup(13, FireShield, DeathWard),
                BuildSpellGroup(17, MassCureWounds, WallOfForce))
            .AddToDB();

        battleSmith.AddFeatureAtLevel(battleSmithPrepSpells, 3);

        var weaponProf = ArtisanHelpers
            .BuildProficiency("ProficiencyWeaponArtisanBattleSmith", ProficiencyType.Weapon,
                EquipmentDefinitions.MartialWeaponCategory)
            .SetGuiPresentation("WeaponProfArtisanBattleSmith", Category.Subclass)
            .AddToDB();

        var infusionPoolIncrease = FeatureDefinitionPowerPoolModifierBuilder
            .Create("AttributeModiferArtisanBattleSmithInfusionHealingPool", ArtisanClass.GuidNamespace)
            .Configure(2, UsesDetermination.Fixed, AttributeDefinitions.Intelligence,
                ArtisanClass.PowerPoolArtisanInfusion)
            .SetGuiPresentation("HealingPoolArtisanBattleSmithInfusionsIncrease", Category.Subclass)
            .AddToDB();

        battleSmith.AddFeatureAtLevel(weaponProf, 3);
        battleSmith.AddFeatureAtLevel(infusionPoolIncrease, 3);

        var battleSmithInfusedWeapon = new FeatureDefinitionAttackModifierBuilder(
                "AttackModifierArtisanBattleSmithWeapon", ArtisanClass.GuidNamespace,
                // Note this is not magical because that causes a conflict with the enhanced weapon effect.
                AbilityScoreReplacement.SpellcastingAbility, "")
            .SetGuiPresentation("AttackModifierArtisanBattleSmithWeapon", Category.Subclass,
                FeatureDefinitionAttackModifiers.AttackModifierMagicWeapon.GuiPresentation.SpriteReference)
            .AddToDB();

        var infuseWeaponGui = new GuiPresentationBuilder(
            "Subclass/&PowerArtisanBattleSmithInfuseWeaponTitle",
            "Subclass/&PowerArtisanBattleSmithInfuseWeaponDescription");
        infuseWeaponGui.SetSpriteReference(FeatureDefinitionPowers.PowerDomainElementalLightningBlade
            .GuiPresentation.SpriteReference);

        var enchantWeapon = ArtisanInfusionHelper.BuildItemModifierInfusion(battleSmithInfusedWeapon,
                ActionDefinitions.ItemSelectionType.Weapon, "PowerBattleSmithWeapon", infuseWeaponGui.Build())
            .AddToDB();
        battleSmith.AddFeatureAtLevel(enchantWeapon, 3);

        battleSmith.AddFeatureAtLevel(ProtectorConstructFeatureSetBuilder.ProtectorConstructFeatureSet, 3);

        // Level 5: Extra attack
        var extraAttackGui = new GuiPresentationBuilder(
            "Subclass/&AttributeModifierArtisanBattleSmithExtraAttackTitle",
            "Subclass/&AttributeModifierArtisanBattleSmithExtraAttackDescription");
        var extraAttack = ArtisanHelpers.BuildAttributeModifier("AttributeModifierBattleSmithExtraAttack",
            FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
            AttributeDefinitions.AttacksNumber,
            1, extraAttackGui.Build());
        battleSmith.AddFeatureAtLevel(extraAttack, 5);

        var joltAttackGui = new GuiPresentationBuilder(
            "Feat/&AttackModifierArtisanBattleSmithJoltTitle",
            "Feat/&AttackModifierArtisanBattleSmithJoltDescription");
        var joltAttack = ArtisanHelpers.BuildAttackModifier(
            "AttackModifierArtisanBattleSmithJolt", // Note ability score bonus only works if it's applied to a weapon, not a character.
            AttackModifierMethod.None, 0,
            AttributeDefinitions.Intelligence, AttackModifierMethod.FlatValue, 3, AttributeDefinitions.Intelligence,
            false,
            TagsDefinitions.Magical, joltAttackGui.Build());
        battleSmith.AddFeatureAtLevel(joltAttack, 9);

        var improvedInfuseWeaponGui = new GuiPresentationBuilder(
            "Subclass/&PowerArtisanBattleSmithImprovedInfuseWeaponTitle",
            "Subclass/&PowerArtisanBattleSmithImprovedInfuseWeaponDescription");
        improvedInfuseWeaponGui.SetSpriteReference(FeatureDefinitionPowers.PowerDomainElementalLightningBlade
            .GuiPresentation.SpriteReference);

        // var attackImprovedModGui = new GuiPresentationBuilder(
        //     "Subclass/&AttackModifierImprovedArtisanBattleSmithWeaponTitle",
        //     "Subclass/&AttackModifierImprovedArtisanBattleSmithWeaponDescription");
        // attackImprovedModGui.SetSpriteReference(FeatureDefinitionAttackModifiers.AttackModifierMagicWeapon
        //     .GuiPresentation.SpriteReference);

        var jolt2AttackGui = new GuiPresentationBuilder(
            "Feat/&AttackModifierArtisanBattleSmithJolt2Title",
            "Feat/&AttackModifierArtisanBattleSmithJolt2Description");
        var jolt2Attack = ArtisanHelpers.BuildAttackModifier(
            "AttackModifierArtisanBattleSmithJolt2", // Note ability score bonus only works if it's applied to a weapon, not a character.
            AttackModifierMethod.None, 0,
            AttributeDefinitions.Intelligence, AttackModifierMethod.FlatValue, 3, AttributeDefinitions.Intelligence,
            false,
            TagsDefinitions.Magical, jolt2AttackGui.Build());
        battleSmith.AddFeatureAtLevel(jolt2Attack, 15);
        battleSmith.AddFeatureAtLevel(
            ProtectorConstructUpgradeFeatureSetBuilder.ProtectorConstructUpgradeFeatureSet, 15);

        // build the subclass and add tot he db
        return battleSmith.AddToDB();
    }

    private sealed class
        FeatureDefinitionAttackModifierBuilder : Builders.Features.FeatureDefinitionAttackModifierBuilder
    {
        public FeatureDefinitionAttackModifierBuilder(string name, Guid guidNamespace,
            AbilityScoreReplacement abilityReplacement, string additionalAttackTag) : base(name, guidNamespace)
        {
            Definition.abilityScoreReplacement = abilityReplacement;
            Definition.additionalBonusUnarmedStrikeAttacksTag = additionalAttackTag;
            Definition.attackRollModifierMethod = AttackModifierMethod.FlatValue;
            Definition.damageRollModifierMethod = AttackModifierMethod.FlatValue;
        }
    }
}
