using System;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.CustomFeatureDefinitions;
using SolastaModApi.Extensions;
using static RuleDefinitions;
using static SolastaCommunityExpansion.Builders.Features.AutoPreparedSpellsGroupBuilder;
using static SolastaModApi.DatabaseHelper;
using static SolastaModApi.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaModApi.DatabaseHelper.SpellDefinitions;

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

            FeatureDefinitionAutoPreparedSpells battleSmithPrepSpells = FeatureDefinitionAutoPreparedSpellsBuilder
                .Create("ArtificerBattleSmithAutoPrepSpells", TinkererClass.GuidNamespace)
                .SetGuiPresentation("BattleSmithSubclassSpells", Category.Feat)
                .SetCastingClass(artificer)
                .SetPreparedSpellGroups(
                    BuildSpellGroup(3, Heroism, Shield, HuntersMark),
                    BuildSpellGroup(5, BrandingSmite, SpiritualWeapon),
                    BuildSpellGroup(9, RemoveCurse, BeaconOfHope),
                    BuildSpellGroup(13, FireShield, DeathWard),
                    BuildSpellGroup(17, MassCureWounds, WallOfForce))
                .AddToDB();

            battleSmith.AddFeatureAtLevel(battleSmithPrepSpells, 3);

            var weaponProf = FeatureHelpers
                .BuildProficiency("ProficiencyWeaponArtificerBattleSmith", ProficiencyType.Weapon, EquipmentDefinitions.MartialWeaponCategory)
                .SetGuiPresentation("WeaponProfArtificerBattleSmith", Category.Subclass)
                .AddToDB();

            var infusionPoolIncrease = FeatureDefinitionPowerPoolModifierBuilder
                .Create("AttributeModiferArtificerBattleSmithInfusionHealingPool", TinkererClass.GuidNamespace)
                .Configure(2, UsesDetermination.Fixed, AttributeDefinitions.Intelligence, TinkererClass.InfusionPool)
                .SetGuiPresentation("HealingPoolArtificerBattleSmithInfusionsIncrease", Category.Subclass)
                .AddToDB();

            battleSmith.AddFeatureAtLevel(weaponProf, 3);
            battleSmith.AddFeatureAtLevel(infusionPoolIncrease, 3);

            var battleSmithInfusedWeapon = new FeatureDefinitionAttackModifierBuilder(
                    "AttackModifierArtificerBattleSmithWeapon", TinkererClass.GuidNamespace,
                    // Note this is not magical because that causes a conflict with the enhanced weapon effect.
                    AbilityScoreReplacement.SpellcastingAbility, "")
                .SetGuiPresentation("AttackModifierArtificerBattleSmithWeapon", Category.Subclass, FeatureDefinitionAttackModifiers.AttackModifierMagicWeapon.GuiPresentation.SpriteReference)
                .AddToDB();

            GuiPresentationBuilder infuseWeaponGui = new GuiPresentationBuilder(
                "Subclass/&PowerArtificerBattleSmithInfuseWeaponTitle",
                "Subclass/&PowerArtificerBattleSmithInfuseWeaponDescription");
            infuseWeaponGui.SetSpriteReference(FeatureDefinitionPowers.PowerDomainElementalLightningBlade.GuiPresentation.SpriteReference);

            FeatureDefinitionPowerSharedPool enchantWeapon = InfusionHelpers.BuildItemModifierInfusion(battleSmithInfusedWeapon,
               ActionDefinitions.ItemSelectionType.Weapon, "PowerBattleSmithWeapon", infuseWeaponGui.Build()).AddToDB();
            battleSmith.AddFeatureAtLevel(enchantWeapon, 3);

            battleSmith.AddFeatureAtLevel(ProtectorConstructFeatureSetBuilder.ProtectorConstructFeatureSet, 3);

            // Level 5: Extra attack
            GuiPresentationBuilder extraAttackGui = new GuiPresentationBuilder(
                "Subclass/&AttributeModifierArtificerBattleSmithExtraAttackTitle",
                "Subclass/&AttributeModifierArtificerBattleSmithExtraAttackDescription");
            FeatureDefinitionAttributeModifier extraAttack = FeatureHelpers.BuildAttributeModifier("AttributeModifierBattleSmithExtraAttack",
                FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive, AttributeDefinitions.AttacksNumber,
                1, extraAttackGui.Build());
            battleSmith.AddFeatureAtLevel(extraAttack, 5);

            GuiPresentationBuilder joltAttackGui = new GuiPresentationBuilder(
                "Feat/&AttackModifierArtificerBattleSmithJoltTitle",
                "Feat/&AttackModifierArtificerBattleSmithJoltDescription");
            FeatureDefinitionAttackModifier joltAttack = FeatureHelpers.BuildAttackModifier(
                "AttackModifierArtificerBattleSmithJolt",                 // Note ability score bonus only works if it's applied to a weapon, not a character.
                AttackModifierMethod.None, 0,
                AttributeDefinitions.Intelligence, AttackModifierMethod.FlatValue, 3, AttributeDefinitions.Intelligence, false,
                "Magical", joltAttackGui.Build());
            battleSmith.AddFeatureAtLevel(joltAttack, 9);

            GuiPresentationBuilder improvedInfuseWeaponGui = new GuiPresentationBuilder(
                "Subclass/&PowerArtificerBattleSmithImprovedInfuseWeaponTitle",
                "Subclass/&PowerArtificerBattleSmithImprovedInfuseWeaponDescription");
            improvedInfuseWeaponGui.SetSpriteReference(FeatureDefinitionPowers.PowerDomainElementalLightningBlade.GuiPresentation.SpriteReference);

            GuiPresentationBuilder attackImprovedModGui = new GuiPresentationBuilder(
                "Subclass/&AttackModifierImprovedArtificerBattleSmithWeaponTitle",
                "Subclass/&AttackModifierImprovedArtificerBattleSmithWeaponDescription");
            attackImprovedModGui.SetSpriteReference(FeatureDefinitionAttackModifiers.AttackModifierMagicWeapon.GuiPresentation.SpriteReference);

            GuiPresentationBuilder jolt2AttackGui = new GuiPresentationBuilder(
                "Feat/&AttackModifierArtificerBattleSmithJolt2Title",
                "Feat/&AttackModifierArtificerBattleSmithJolt2Description");
            FeatureDefinitionAttackModifier jolt2Attack = FeatureHelpers.BuildAttackModifier(
                "AttackModifierArtificerBattleSmithJolt2",                 // Note ability score bonus only works if it's applied to a weapon, not a character.
                AttackModifierMethod.None, 0,
                AttributeDefinitions.Intelligence, AttackModifierMethod.FlatValue, 3, AttributeDefinitions.Intelligence, false,
                "Magical", jolt2AttackGui.Build());
            battleSmith.AddFeatureAtLevel(jolt2Attack, 15);
            battleSmith.AddFeatureAtLevel(ProtectorConstructUpgradeFeatureSetBuilder.ProtectorConstructUpgradeFeatureSet, 15);

            // build the subclass and add tot he db
            return battleSmith.AddToDB();
        }

        private sealed class FeatureDefinitionAttackModifierBuilder : Builders.Features.FeatureDefinitionAttackModifierBuilder
        {
            public FeatureDefinitionAttackModifierBuilder(string name, Guid guidNamespace, AbilityScoreReplacement abilityReplacement, string additionalAttackTag) : base(name, guidNamespace)
            {
                Definition.SetAbilityScoreReplacement(abilityReplacement);
                Definition.SetAdditionalAttackTag(additionalAttackTag);
            }
        }
    }
}
