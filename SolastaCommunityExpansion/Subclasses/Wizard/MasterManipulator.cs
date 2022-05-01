using System;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaModApi.Extensions;
using static SolastaModApi.DatabaseHelper;
using static SolastaModApi.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaModApi.DatabaseHelper.SpellDefinitions;

namespace SolastaCommunityExpansion.Subclasses.Wizard
{
    internal class MasterManipulator : AbstractSubclass
    {
        private static readonly Guid SubclassNamespace = new("af7255d2-8ce2-4398-8999-f1ef536001f6");
        private readonly CharacterSubclassDefinition Subclass;

        #region DcIncreaseAffinity
        private static FeatureDefinitionMagicAffinity _dcIncreaseAffinity;
        private static FeatureDefinitionMagicAffinity DcIncreaseAffinity =>
            _dcIncreaseAffinity ??= BuildMagicAffinityModifiers(0, RuleDefinitions.SpellParamsModifierType.FlatValue, Main.Settings.OverrideWizardMasterManipulatorArcaneManipulationSpellDc, "MagicAffinityMasterManipulatorDC", GetSpellDCPresentation().Build());
        #endregion

        internal override FeatureDefinitionSubclassChoice GetSubclassChoiceList()
        {
            return FeatureDefinitionSubclassChoices.SubclassChoiceWizardArcaneTraditions;
        }

        internal override CharacterSubclassDefinition GetSubclass()
        {
            return Subclass;
        }

        internal MasterManipulator()
        {
            // Make Control Master subclass
            var arcaneControlAffinity = FeatureDefinitionMagicAffinityBuilder
                .Create("MagicAffinityControlHeightened", SubclassNamespace)
                .SetWarList(1,
                    CharmPerson, // enchantment
                    Sleep, // enchantment
                    ColorSpray, // illusion
                    HoldPerson, // enchantment,
                    Invisibility, // illusion
                    Counterspell, // abjuration
                    DispelMagic, // abjuration
                    Banishment, // abjuration
                    Confusion, // enchantment
                    PhantasmalKiller, // illusion
                    DominatePerson, // Enchantment
                    HoldMonster) // Enchantment           
                .SetGuiPresentation("MagicAffinityMasterManipulatorList", Category.Subclass)
                .AddToDB();

            FeatureDefinitionProficiency proficiency = FeatureDefinitionProficiencyBuilder
                .Create("ManipulatorMentalSavingThrows", SubclassNamespace)
                .SetGuiPresentation(Category.Subclass)
                .SetProficiencies(RuleDefinitions.ProficiencyType.SavingThrow, AttributeDefinitions.Charisma, AttributeDefinitions.Constitution)
                .AddToDB();

            FeatureDefinitionPower powerDominate = FeatureDefinitionPowerBuilder
                .Create("PowerManipulatorDominatePerson", SubclassNamespace)
                .SetGuiPresentation(Category.Subclass, DominatePerson.GuiPresentation.SpriteReference)
                .Configure(0,
                    RuleDefinitions.UsesDetermination.AbilityBonusPlusFixed,
                    AttributeDefinitions.Intelligence,
                    RuleDefinitions.ActivationTime.BonusAction,
                    1, RuleDefinitions.RechargeRate.LongRest,
                    false, false, AttributeDefinitions.Intelligence,
                    DominatePerson.EffectDescription, false /* unique instance */)
                .AddToDB();

            Subclass = CharacterSubclassDefinitionBuilder
                .Create("MasterManipulator", SubclassNamespace)
                .SetGuiPresentation("TraditionMasterManipulator", Category.Subclass, RoguishShadowCaster.GuiPresentation.SpriteReference)
                .AddFeatureAtLevel(arcaneControlAffinity, 2)
                .AddFeatureAtLevel(DcIncreaseAffinity, 6)
                .AddFeatureAtLevel(proficiency, 10)
                .AddFeatureAtLevel(powerDominate, 14).AddToDB();
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

        private static FeatureDefinitionMagicAffinity BuildMagicAffinityModifiers(int attackModifier, RuleDefinitions.SpellParamsModifierType attackModifierType, int dcModifier, string name, GuiPresentation guiPresentation)
        {
            return FeatureDefinitionMagicAffinityBuilder
                .Create(name, SubclassNamespace)
                .SetGuiPresentation(guiPresentation)
                .SetCastingModifiers(attackModifier, attackModifierType, dcModifier, false, false, false)
                .AddToDB();
        }
    }
}
