using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.CustomFeatureDefinitions;
using SolastaModApi;
using SolastaModApi.Extensions;
using static RuleDefinitions;
using static SolastaModApi.DatabaseHelper.CharacterSubclassDefinitions;

namespace SolastaCommunityExpansion.Classes.Tinkerer.Subclasses
{
    public static class BattleSmithBuilder
    {
        public static CharacterSubclassDefinition Build(CharacterClassDefinition artificer)
        {
            // Make Battle Smith subclass
            CharacterSubclassDefinitionBuilder battleSmith = CharacterSubclassDefinitionBuilder
                .Create("BattleSmith", TinkererClass.GuidNamespace)
                .SetGuiPresentation("ArtificerBattleSmith", Category.Subclass, MartialSpellblade.GuiPresentation.SpriteReference);

            FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup battleSmithSpells1 = FeatureHelpers.BuildAutoPreparedSpellGroup(
                3, DatabaseHelper.SpellDefinitions.Heroism, DatabaseHelper.SpellDefinitions.Shield, DatabaseHelper.SpellDefinitions.HuntersMark);

            FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup battleSmithSpells2 = FeatureHelpers.BuildAutoPreparedSpellGroup(
                5, DatabaseHelper.SpellDefinitions.BrandingSmite, DatabaseHelper.SpellDefinitions.SpiritualWeapon);

            FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup battleSmithSpells3 = FeatureHelpers.BuildAutoPreparedSpellGroup(
                9, DatabaseHelper.SpellDefinitions.RemoveCurse, DatabaseHelper.SpellDefinitions.BeaconOfHope);

            FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup battleSmithSpells4 = FeatureHelpers.BuildAutoPreparedSpellGroup(
                13, DatabaseHelper.SpellDefinitions.FireShield, DatabaseHelper.SpellDefinitions.DeathWard);

            FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup battleSmithSpells5 = FeatureHelpers.BuildAutoPreparedSpellGroup(
                17, DatabaseHelper.SpellDefinitions.MassCureWounds, DatabaseHelper.SpellDefinitions.WallOfForce);

            GuiPresentationBuilder battleSmithSpellsPresentation = new GuiPresentationBuilder(
                "Feat/&BattleSmithSubclassSpellsTitle",
                "Feat/&BattleSmithSubclassSpellsDescription");
            FeatureDefinitionAutoPreparedSpells battleSmithPrepSpells = FeatureHelpers.BuildAutoPreparedSpells(
                new List<FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup>() {
                    battleSmithSpells1, battleSmithSpells2, battleSmithSpells3, battleSmithSpells4, battleSmithSpells5 },
                artificer, "ArtificerBattleSmithAutoPrepSpells", battleSmithSpellsPresentation.Build());
            battleSmith.AddFeatureAtLevel(battleSmithPrepSpells, 3);

            GuiPresentationBuilder weaponProfPresentation = new GuiPresentationBuilder(
                "Subclass/&WeaponProfArtificerBattleSmithTitle",
                "Subclass/&WeaponProfArtificerBattleSmithDescription");
            FeatureDefinitionProficiency weaponProf = FeatureHelpers.BuildProficiency(ProficiencyType.Weapon,
                new List<string>() { EquipmentDefinitions.MartialWeaponCategory },
                "ProficiencyWeaponArtificerBattleSmith", weaponProfPresentation.Build());
            battleSmith.AddFeatureAtLevel(weaponProf, 3);

            GuiPresentationBuilder InfusionPoolIncreaseGui = new GuiPresentationBuilder(
                "Subclass/&HealingPoolArtificerBattleSmithInfusionsIncreaseTitle",
                "Subclass/&HealingPoolArtificerBattleSmithInfusionsIncreaseDescription");
            FeatureDefinitionPowerPoolModifier InfusionPoolIncrease = new FeatureDefinitionPowerPoolModifierBuilder("AttributeModiferArtificerBattleSmithInfusionHealingPool",
                GuidHelper.Create(TinkererClass.GuidNamespace, "AttributeModiferArtificerBattleSmithInfusionHealingPool").ToString(),
                2, UsesDetermination.Fixed, AttributeDefinitions.Intelligence, TinkererClass.InfusionPool, InfusionPoolIncreaseGui.Build()).AddToDB();
            battleSmith.AddFeatureAtLevel(InfusionPoolIncrease, 3);

            GuiPresentationBuilder attackModGui = new GuiPresentationBuilder(
                "Subclass/&AttackModifierArtificerBattleSmithWeaponTitle",
                "Subclass/&AttackModifierArtificerBattleSmithWeaponDescription");
            attackModGui.SetSpriteReference(DatabaseHelper.FeatureDefinitionAttackModifiers.AttackModifierMagicWeapon.GuiPresentation.SpriteReference);
            FeatureDefinitionAttackModifier battleSmithInfusedWeapon = new FeatureDefinitionAttackModifierBuilder("AttackModifierArtificerBattleSmithWeapon",
                 GuidHelper.Create(TinkererClass.GuidNamespace, "AttackModifierArtificerBattleSmithWeapon").ToString(),
                // Note this is not magical because that causes a conflict with the enhanced weapon effect.
                AbilityScoreReplacement.SpellcastingAbility, "", attackModGui.Build()).AddToDB();

            GuiPresentationBuilder infuseWeaponGui = new GuiPresentationBuilder(
                "Subclass/&PowerArtificerBattleSmithInfuseWeaponTitle",
                "Subclass/&PowerArtificerBattleSmithInfuseWeaponDescription");
            infuseWeaponGui.SetSpriteReference(DatabaseHelper.FeatureDefinitionPowers.PowerDomainElementalLightningBlade.GuiPresentation.SpriteReference);

            FeatureDefinitionPowerSharedPool enchantWeapon = InfusionHelpers.BuildItemModifierInfusion(battleSmithInfusedWeapon,
               ActionDefinitions.ItemSelectionType.Weapon, "PowerBattleSmithWeapon", infuseWeaponGui.Build()).AddToDB();
            battleSmith.AddFeatureAtLevel(enchantWeapon, 3);

            battleSmith.AddFeatureAtLevel(ProtectorConstructFeatureSetBuilder.ProtectorConstructFeatureSet, 3);

            // Level 5: Extra attack
            GuiPresentationBuilder extraAttackGui = new GuiPresentationBuilder(
                "Subclass/&AttributeModifierArtificerBattleSmithExtraAttackTitle",
                "Subclass/&AttributeModifierArtificerBattleSmithExtraAttackDescription");
            FeatureDefinitionAttributeModifier extraAttack = FeatureHelpers.BuildAttributeModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
                AttributeDefinitions.AttacksNumber, 1,
                "AttributeModifierBattleSmithExtraAttack", extraAttackGui.Build());
            battleSmith.AddFeatureAtLevel(extraAttack, 5);

            GuiPresentationBuilder joltAttackGui = new GuiPresentationBuilder(
                "Feat/&AttackModifierArtificerBattleSmithJoltTitle",
                "Feat/&AttackModifierArtificerBattleSmithJoltDescription");
            FeatureDefinitionAttackModifier joltAttack = FeatureHelpers.BuildAttackModifier(
                // Note ability score bonus only works if it's applied to a weapon, not a character.
                AttackModifierMethod.None, 0, AttributeDefinitions.Intelligence,
                AttackModifierMethod.FlatValue, 3, AttributeDefinitions.Intelligence, false, "Magical",
                "AttackModifierArtificerBattleSmithJolt", joltAttackGui.Build());
            battleSmith.AddFeatureAtLevel(joltAttack, 9);

            GuiPresentationBuilder improvedInfuseWeaponGui = new GuiPresentationBuilder(
                "Subclass/&PowerArtificerBattleSmithImprovedInfuseWeaponTitle",
                "Subclass/&PowerArtificerBattleSmithImprovedInfuseWeaponDescription");
            improvedInfuseWeaponGui.SetSpriteReference(DatabaseHelper.FeatureDefinitionPowers.PowerDomainElementalLightningBlade.GuiPresentation.SpriteReference);

            GuiPresentationBuilder attackImprovedModGui = new GuiPresentationBuilder(
                "Subclass/&AttackModifierImprovedArtificerBattleSmithWeaponTitle",
                "Subclass/&AttackModifierImprovedArtificerBattleSmithWeaponDescription");
            attackImprovedModGui.SetSpriteReference(DatabaseHelper.FeatureDefinitionAttackModifiers.AttackModifierMagicWeapon.GuiPresentation.SpriteReference);

            GuiPresentationBuilder jolt2AttackGui = new GuiPresentationBuilder(
                "Feat/&AttackModifierArtificerBattleSmithJolt2Title",
                "Feat/&AttackModifierArtificerBattleSmithJolt2Description");
            FeatureDefinitionAttackModifier jolt2Attack = FeatureHelpers.BuildAttackModifier(
                // Note ability score bonus only works if it's applied to a weapon, not a character.
                AttackModifierMethod.None, 0, AttributeDefinitions.Intelligence,
                AttackModifierMethod.FlatValue, 3, AttributeDefinitions.Intelligence, false, "Magical",
                "AttackModifierArtificerBattleSmithJolt2", jolt2AttackGui.Build());
            battleSmith.AddFeatureAtLevel(jolt2Attack, 15);
            battleSmith.AddFeatureAtLevel(ProtectorConstructUpgradeFeatureSetBuilder.ProtectorConstructUpgradeFeatureSet, 15);

            // build the subclass and add tot he db
            return battleSmith.AddToDB();
        }

        private sealed class FeatureDefinitionAttackModifierBuilder : Builders.Features.FeatureDefinitionAttackModifierBuilder
        {
            public FeatureDefinitionAttackModifierBuilder(string name, string guid,
                AbilityScoreReplacement abilityReplacement, string additionalAttackTag,
                GuiPresentation guiPresentation) : base(name, guid)
            {
                Definition.SetAbilityScoreReplacement(abilityReplacement);
                Definition.SetAdditionalAttackTag(additionalAttackTag);
                Definition.SetGuiPresentation(guiPresentation);
            }
        }
    }
}
