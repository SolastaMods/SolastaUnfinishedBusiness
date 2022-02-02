using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaModApi;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Subclasses.Wizard
{
    internal class MasterManipulator : AbstractSubclass
    {
        private static readonly Guid SubclassNamespace = new("af7255d2-8ce2-4398-8999-f1ef536001f6");
        private readonly CharacterSubclassDefinition Subclass;

        #region DcIncreaseAffinity
        private static FeatureDefinitionMagicAffinity _dcIncreaseAffinity;
        private static FeatureDefinitionMagicAffinity DcIncreaseAffinity =>
            _dcIncreaseAffinity ??= BuildMagicAffinityModifiers(0, Main.Settings.OverrideWizardMasterManipulatorArcaneManipulationSpellDc, "MagicAffinityMasterManipulatorDC", GetSpellDCPresentation().Build());
        #endregion

        internal override FeatureDefinitionSubclassChoice GetSubclassChoiceList()
        {
            return DatabaseHelper.FeatureDefinitionSubclassChoices.SubclassChoiceWizardArcaneTraditions;
        }
        internal override CharacterSubclassDefinition GetSubclass()
        {
            return Subclass;
        }

        internal MasterManipulator()
        {
            // Make Control Master subclass
            CharacterSubclassDefinitionBuilder controlMaster = new CharacterSubclassDefinitionBuilder("MasterManipulator", GuidHelper.Create(SubclassNamespace, "MasterManipulator").ToString());
            GuiPresentationBuilder controlPresentation = new GuiPresentationBuilder(
                "Subclass/&TraditionMasterManipulatorTitle",
                "Subclass/&TraditionMasterManipulatorDescription");
            controlPresentation.SetSpriteReference(DatabaseHelper.CharacterSubclassDefinitions.RoguishShadowCaster.GuiPresentation.SpriteReference);
            controlMaster.SetGuiPresentation(controlPresentation.Build());

            GuiPresentationBuilder arcaneControlAffinityGui = new GuiPresentationBuilder(
                "Subclass/&MagicAffinityMasterManipulatorListTitle",
                "Subclass/&MagicAffinityMasterManipulatorListDescription");
            FeatureDefinitionMagicAffinity arcaneControlAffinity = BuildMagicAffinityHeightenedList(new List<string>() {
                DatabaseHelper.SpellDefinitions.CharmPerson.Name, // enchantment
                DatabaseHelper.SpellDefinitions.Sleep.Name, // enchantment
                DatabaseHelper.SpellDefinitions.ColorSpray.Name, // illusion
                DatabaseHelper.SpellDefinitions.HoldPerson.Name, // enchantment,
                DatabaseHelper.SpellDefinitions.Invisibility.Name, // illusion
                DatabaseHelper.SpellDefinitions.Counterspell.Name, // abjuration
                DatabaseHelper.SpellDefinitions.DispelMagic.Name, // abjuration
                DatabaseHelper.SpellDefinitions.Banishment.Name, // abjuration
                DatabaseHelper.SpellDefinitions.Confusion.Name, // enchantment
                DatabaseHelper.SpellDefinitions.PhantasmalKiller.Name, // illusion
                DatabaseHelper.SpellDefinitions.DominatePerson.Name, // Enchantment
                DatabaseHelper.SpellDefinitions.HoldMonster.Name // Enchantment
            }, 1,
                "MagicAffinityControlHeightened", arcaneControlAffinityGui.Build());
            controlMaster.AddFeatureAtLevel(arcaneControlAffinity, 2);
            controlMaster.AddFeatureAtLevel(DcIncreaseAffinity, 6);

            FeatureDefinitionProficiency proficiency = new FeatureDefinitionProficiencyBuilder("ManipulatorMentalSavingThrows", GuidHelper.Create(SubclassNamespace, "ManipulatorMentalSavingThrows").ToString(),
                RuleDefinitions.ProficiencyType.SavingThrow,
                new List<string>() { AttributeDefinitions.Charisma, AttributeDefinitions.Constitution },
                new GuiPresentationBuilder(
                    "Subclass/&ManipulatorMentalSavingThrowsTitle",
                    "Subclass/&ManipulatorMentalSavingThrowsDescription").Build()).AddToDB();
            controlMaster.AddFeatureAtLevel(proficiency, 10);

            GuiPresentationBuilder DominatePower = new GuiPresentationBuilder(
                "Subclass/&PowerManipulatorDominatePersonTitle",
                "Subclass/&PowerManipulatorDominatePersonDescription");
            DominatePower.SetSpriteReference(DatabaseHelper.SpellDefinitions.DominatePerson.GuiPresentation.SpriteReference);
            FeatureDefinitionPower PowerDominate = new FeatureDefinitionPowerBuilder("PowerManipulatorDominatePerson", GuidHelper.Create(SubclassNamespace, "PowerManipulatorDominatePerson").ToString(),
                0, RuleDefinitions.UsesDetermination.AbilityBonusPlusFixed, AttributeDefinitions.Intelligence, RuleDefinitions.ActivationTime.BonusAction, 1, RuleDefinitions.RechargeRate.LongRest,
                false, false, AttributeDefinitions.Intelligence,
                DatabaseHelper.SpellDefinitions.DominatePerson.EffectDescription, DominatePower.Build(), false /* unique instance */).AddToDB();
            controlMaster.AddFeatureAtLevel(PowerDominate, 14);

            Subclass = controlMaster.AddToDB();
        }

        private static GuiPresentationBuilder GetSpellDCPresentation()
        {
            return new GuiPresentationBuilder("Subclass/&MagicAffinityMasterManipulatorDCTitle", "Subclass/&MagicAffinityMasterManipulatorDC" + Main.Settings.OverrideWizardMasterManipulatorArcaneManipulationSpellDc + "Description");
        }

        public static void UpdateSpellDCBoost()
        {
            if (DcIncreaseAffinity)
            {
                DcIncreaseAffinity.SetSaveDCModifier(Main.Settings.OverrideWizardMasterManipulatorArcaneManipulationSpellDc);
                DcIncreaseAffinity.SetGuiPresentation(GetSpellDCPresentation().Build());
            }
        }

        public static FeatureDefinitionMagicAffinity BuildMagicAffinityModifiers(int attackModifier, int dcModifier, string name, GuiPresentation guiPresentation)
        {
            FeatureDefinitionMagicAffinityBuilder builder = new FeatureDefinitionMagicAffinityBuilder(name, GuidHelper.Create(SubclassNamespace, name).ToString(),
                guiPresentation).SetCastingModifiers(attackModifier, dcModifier, false, false, false);
            return builder.AddToDB();
        }

        public static FeatureDefinitionMagicAffinity BuildMagicAffinityHeightenedList(List<string> spellNames, int levelBonus, string name, GuiPresentation guiPresentation)
        {
            FeatureDefinitionMagicAffinityBuilder builder = new FeatureDefinitionMagicAffinityBuilder(name, GuidHelper.Create(SubclassNamespace, name).ToString(),
                guiPresentation).SetWarList(spellNames, levelBonus);
            return builder.AddToDB();
        }
    }
}
