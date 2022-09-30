using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class WizardMasterManipulator : AbstractSubclass
{
    internal WizardMasterManipulator()
    {
        // Make Control Master subclass
        var magicAffinityMasterManipulatorControlHeightened = FeatureDefinitionMagicAffinityBuilder
            .Create("MagicAffinityMasterManipulatorControlHeightened")
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
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        var magicAffinityMasterManipulatorDc = FeatureDefinitionMagicAffinityBuilder
            .Create("MagicAffinityMasterManipulatorDC")
            .SetGuiPresentation(Category.Feature)
            .SetCastingModifiers(
                0,
                RuleDefinitions.SpellParamsModifierType.None,
                2)
            .AddToDB();

        var proficiencyMasterManipulatorMentalSavingThrows = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyMasterManipulatorMentalSavingThrows")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(
                RuleDefinitions.ProficiencyType.SavingThrow,
                AttributeDefinitions.Charisma,
                AttributeDefinitions.Constitution)
            .AddToDB();

        var powerMasterManipulatorDominatePerson = FeatureDefinitionPowerBuilder
            .Create("PowerMasterManipulatorDominatePerson")
            .SetGuiPresentation(Category.Feature, DominatePerson.GuiPresentation.SpriteReference)
            .Configure(0,
                RuleDefinitions.UsesDetermination.AbilityBonusPlusFixed,
                AttributeDefinitions.Intelligence,
                RuleDefinitions.ActivationTime.BonusAction,
                1,
                RuleDefinitions.RechargeRate.LongRest,
                false,
                false,
                AttributeDefinitions.Intelligence,
                DominatePerson.EffectDescription)
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("WizardMasterManipulator")
            .SetGuiPresentation(Category.Subclass, RoguishShadowCaster.GuiPresentation.SpriteReference)
            .AddFeaturesAtLevel(2,
                magicAffinityMasterManipulatorControlHeightened)
            .AddFeaturesAtLevel(6,
                magicAffinityMasterManipulatorDc)
            .AddFeaturesAtLevel(10,
                proficiencyMasterManipulatorMentalSavingThrows)
            .AddFeaturesAtLevel(14,
                powerMasterManipulatorDominatePerson)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; set; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceWizardArcaneTraditions;
}
