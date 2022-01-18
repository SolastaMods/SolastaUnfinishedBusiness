using SolastaCommunityExpansion.CustomFeatureDefinitions;
using SolastaModApi;
using SolastaModApi.Extensions;
using System.Collections.Generic;
using static RuleDefinitions;

namespace SolastaArtificerMod
{
    public class BattleSmithBuilder
    {
        public static CharacterSubclassDefinition Build(CharacterClassDefinition artificer)
        {
            // Make Battle Smith subclass
            CharacterSubclassDefinitionBuilder battleSmith = new CharacterSubclassDefinitionBuilder("BattleSmith", GuidHelper.Create(Main.ModGuidNamespace, "BattleSmith").ToString());
            GuiPresentationBuilder battleSmithPresentation = new GuiPresentationBuilder(
                "Subclass/&ArtificerBattleSmithDescription",
                "Subclass/&ArtificerBattleSmithTitle");
            battleSmithPresentation.SetSpriteReference(DatabaseHelper.CharacterSubclassDefinitions.MartialSpellblade.GuiPresentation.SpriteReference);
            battleSmith.SetGuiPresentation(battleSmithPresentation.Build());

            FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup battleSmithSpells1 = FeatureHelpers.BuildAutoPreparedSpellGroup(3,
                new List<SpellDefinition>() { DatabaseHelper.SpellDefinitions.Heroism, DatabaseHelper.SpellDefinitions.Shield, DatabaseHelper.SpellDefinitions.HuntersMark });

            FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup battleSmithSpells2 = FeatureHelpers.BuildAutoPreparedSpellGroup(5,
                new List<SpellDefinition>() { DatabaseHelper.SpellDefinitions.BrandingSmite, DatabaseHelper.SpellDefinitions.SpiritualWeapon });

            FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup battleSmithSpells3 = FeatureHelpers.BuildAutoPreparedSpellGroup(9,
                new List<SpellDefinition>() { DatabaseHelper.SpellDefinitions.RemoveCurse, DatabaseHelper.SpellDefinitions.BeaconOfHope });

            FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup battleSmithSpells4 = FeatureHelpers.BuildAutoPreparedSpellGroup(13,
                new List<SpellDefinition>() { DatabaseHelper.SpellDefinitions.FireShield, DatabaseHelper.SpellDefinitions.DeathWard });

            FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup battleSmithSpells5 = FeatureHelpers.BuildAutoPreparedSpellGroup(17,
                new List<SpellDefinition>() { DatabaseHelper.SpellDefinitions.MassCureWounds, DatabaseHelper.SpellDefinitions.WallOfForce });

            GuiPresentationBuilder battleSmithSpellsPresentation = new GuiPresentationBuilder(
                "Feat/&BattleSmithSubclassSpellsDescription",
                "Feat/&BattleSmithSubclassSpellsTitle");
            FeatureDefinitionAutoPreparedSpells battleSmithPrepSpells = FeatureHelpers.BuildAutoPreparedSpells(
                new List<FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup>() {
                    battleSmithSpells1, battleSmithSpells2, battleSmithSpells3, battleSmithSpells4, battleSmithSpells5 },
                artificer, "ArtificerBattleSmithAutoPrepSpells", battleSmithSpellsPresentation.Build());
            battleSmith.AddFeatureAtLevel(battleSmithPrepSpells, 3);

            GuiPresentationBuilder weaponProfPresentation = new GuiPresentationBuilder(
                "Subclass/&WeaponProfArtificerBattleSmithDescription",
                "Subclass/&WeaponProfArtificerBattleSmithTitle");
            FeatureDefinitionProficiency weaponProf = FeatureHelpers.BuildProficiency(RuleDefinitions.ProficiencyType.Weapon,
                new List<string>() { EquipmentDefinitions.MartialWeaponCategory },
                "ProficiencyWeaponArtificerBattleSmith", weaponProfPresentation.Build());
            battleSmith.AddFeatureAtLevel(weaponProf, 3);

            GuiPresentationBuilder InfusionPoolIncreaseGui = new GuiPresentationBuilder(
                "Subclass/&HealingPoolArtificerBattleSmithInfusionsIncreaseDescription",
                "Subclass/&HealingPoolArtificerBattleSmithInfusionsIncreaseTitle");
            FeatureDefinitionPowerPoolModifier InfusionPoolIncrease = new FeatureDefinitionPowerPoolModifierBuilder("AttributeModiferArtificerBattleSmithInfusionHealingPool",
                GuidHelper.Create(Main.ModGuidNamespace, "AttributeModiferArtificerBattleSmithInfusionHealingPool").ToString(),
                2, RuleDefinitions.UsesDetermination.Fixed, AttributeDefinitions.Intelligence, TinkererClass.InfusionPool, InfusionPoolIncreaseGui.Build()).AddToDB();
            battleSmith.AddFeatureAtLevel(InfusionPoolIncrease, 3);

            GuiPresentationBuilder attackModGui = new GuiPresentationBuilder(
                "Subclass/&AttackModifierArtificerBattleSmithWeaponDescription",
                "Subclass/&AttackModifierArtificerBattleSmithWeaponTitle");
            attackModGui.SetSpriteReference(DatabaseHelper.FeatureDefinitionAttackModifiers.AttackModifierMagicWeapon.GuiPresentation.SpriteReference);
            FeatureDefinitionAttackModifier battleSmithInfusedWeapon = new FeatureDefinitionAttackModifierBuilder("AttackModifierArtificerBattleSmithWeapon",
                 GuidHelper.Create(Main.ModGuidNamespace, "AttackModifierArtificerBattleSmithWeapon").ToString(),
                // Note this is not magical because that causes a conflict with the enhanced weapon effect.
                AbilityScoreReplacement.SpellcastingAbility, "", attackModGui.Build()).AddToDB();

            GuiPresentationBuilder infuseWeaponGui = new GuiPresentationBuilder(
                "Subclass/&PowerArtificerBattleSmithInfuseWeaponDescription",
                "Subclass/&PowerArtificerBattleSmithInfuseWeaponTitle");
            infuseWeaponGui.SetSpriteReference(DatabaseHelper.FeatureDefinitionPowers.PowerDomainElementalLightningBlade.GuiPresentation.SpriteReference);

            FeatureDefinitionPowerSharedPool enchantWeapon = InfusionHelpers.BuildItemModifierInfusion(battleSmithInfusedWeapon,
               ActionDefinitions.ItemSelectionType.Weapon, "PowerBattleSmithWeapon", infuseWeaponGui.Build()).AddToDB();
            battleSmith.AddFeatureAtLevel(enchantWeapon, 3);

            battleSmith.AddFeatureAtLevel(ProtectorConstructFeatureSetBuilder.ProtectorConstructFeatureSet, 3);

            // Level 5: Extra attack
            GuiPresentationBuilder extraAttackGui = new GuiPresentationBuilder(
                "Subclass/&AttributeModifierArtificerBattleSmithExtraAttackDescription",
                "Subclass/&AttributeModifierArtificerBattleSmithExtraAttackTitle");
            FeatureDefinitionAttributeModifier extraAttack = FeatureHelpers.BuildAttributeModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
                AttributeDefinitions.AttacksNumber, 1,
                "AttributeModifierBattleSmithExtraAttack", extraAttackGui.Build());
            battleSmith.AddFeatureAtLevel(extraAttack, 5);

            GuiPresentationBuilder joltAttackGui = new GuiPresentationBuilder(
                "Feat/&AttackModifierArtificerBattleSmithJoltDescription",
                "Feat/&AttackModifierArtificerBattleSmithJoltTitle");
            FeatureDefinitionAttackModifier joltAttack = FeatureHelpers.BuildAttackModifier(
                // Note ability score bonus only works if it's applied to a weapon, not a character.
                RuleDefinitions.AttackModifierMethod.None, 0, AttributeDefinitions.Intelligence,
                RuleDefinitions.AttackModifierMethod.FlatValue, 3, AttributeDefinitions.Intelligence, false, "Magical",
                "AttackModifierArtificerBattleSmithJolt", joltAttackGui.Build());
            battleSmith.AddFeatureAtLevel(joltAttack, 9);

            GuiPresentationBuilder improvedInfuseWeaponGui = new GuiPresentationBuilder(
                "Subclass/&PowerArtificerBattleSmithImprovedInfuseWeaponDescription",
                "Subclass/&PowerArtificerBattleSmithImprovedInfuseWeaponTitle");
            improvedInfuseWeaponGui.SetSpriteReference(DatabaseHelper.FeatureDefinitionPowers.PowerDomainElementalLightningBlade.GuiPresentation.SpriteReference);

            GuiPresentationBuilder attackImprovedModGui = new GuiPresentationBuilder(
                "Subclass/&AttackModifierImprovedArtificerBattleSmithWeaponDescription",
                "Subclass/&AttackModifierImprovedArtificerBattleSmithWeaponTitle");
            attackImprovedModGui.SetSpriteReference(DatabaseHelper.FeatureDefinitionAttackModifiers.AttackModifierMagicWeapon.GuiPresentation.SpriteReference);

            GuiPresentationBuilder jolt2AttackGui = new GuiPresentationBuilder(
                "Feat/&AttackModifierArtificerBattleSmithJolt2Description",
                "Feat/&AttackModifierArtificerBattleSmithJolt2Title");
            FeatureDefinitionAttackModifier jolt2Attack = FeatureHelpers.BuildAttackModifier(
                // Note ability score bonus only works if it's applied to a weapon, not a character.
                RuleDefinitions.AttackModifierMethod.None, 0, AttributeDefinitions.Intelligence,
                RuleDefinitions.AttackModifierMethod.FlatValue, 3, AttributeDefinitions.Intelligence, false, "Magical",
                "AttackModifierArtificerBattleSmithJolt2", jolt2AttackGui.Build());
            battleSmith.AddFeatureAtLevel(jolt2Attack, 15);
            battleSmith.AddFeatureAtLevel(ProtectorConstructUpgradeFeatureSetBuilder.ProtectorConstructUpgradeFeatureSet, 15);

            // build the subclass and add tot he db
            return battleSmith.AddToDB();
        }

        private class FeatureDefinitionAttackModifierBuilder : BaseDefinitionBuilder<FeatureDefinitionAttackModifier>
        {
            public FeatureDefinitionAttackModifierBuilder(string name, string guid,
            RuleDefinitions.AbilityScoreReplacement abilityReplacement, string additionalAttackTag,
            GuiPresentation guiPresentation) : base(name, guid)
            {
                Definition.SetAbilityScoreReplacement(abilityReplacement);
                Definition.SetAdditionalAttackTag(additionalAttackTag);
                Definition.SetGuiPresentation(guiPresentation);
            }
        }
    }
}
