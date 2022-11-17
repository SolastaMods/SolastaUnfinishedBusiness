using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class WizardManipulatorMaster : AbstractSubclass
{
    internal WizardManipulatorMaster()
    {
        var magicAffinityManipulatorMasterControlHeightened = FeatureDefinitionMagicAffinityBuilder
            .Create("MagicAffinityManipulatorMasterControlHeightened")
            .SetGuiPresentation(Category.Feature)
            .SetWarList(1,
                CharmPerson, // enchantment
                Sleep, // enchantment
                ColorSpray, // illusion
                HoldPerson, // enchantment
                Invisibility, // illusion
                Counterspell, // abjuration
                DispelMagic, // abjuration
                Banishment, // abjuration
                Confusion, // enchantment
                PhantasmalKiller, // illusion
                DominatePerson, // Enchantment
                HoldMonster) // Enchantment
            .AddToDB();

        var magicAffinityManipulatorMasterDc = FeatureDefinitionMagicAffinityBuilder
            .Create("MagicAffinityManipulatorMasterDC")
            .SetGuiPresentation(Category.Feature)
            .SetCastingModifiers(0, SpellParamsModifierType.None, 2)
            .AddToDB();

        var proficiencyManipulatorMasterMentalSavingThrows = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyManipulatorMasterMentalSavingThrows")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(
                ProficiencyType.SavingThrow,
                AttributeDefinitions.Charisma,
                AttributeDefinitions.Constitution)
            .AddToDB();

        var powerManipulatorMasterDominatePerson = FeatureDefinitionPowerBuilder
            .Create("PowerManipulatorMasterDominatePerson")
            .SetGuiPresentation(Category.Feature, DominatePerson)
            .SetUsesAbilityBonus(ActivationTime.BonusAction, RechargeRate.LongRest, AttributeDefinitions.Intelligence)
            .SetEffectDescription(DominatePerson.EffectDescription)
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("WizardManipulatorMaster")
            .SetGuiPresentation(Category.Subclass, RoguishShadowCaster)
            .AddFeaturesAtLevel(2,
                magicAffinityManipulatorMasterControlHeightened)
            .AddFeaturesAtLevel(6,
                magicAffinityManipulatorMasterDc)
            .AddFeaturesAtLevel(10,
                proficiencyManipulatorMasterMentalSavingThrows)
            .AddFeaturesAtLevel(14,
                powerManipulatorMasterDominatePerson)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceWizardArcaneTraditions;
}
