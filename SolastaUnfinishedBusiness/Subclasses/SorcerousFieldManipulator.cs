using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class SorcerousFieldManipulator : AbstractSubclass
{
    internal SorcerousFieldManipulator()
    {
        var autoPreparedSpellsFieldManipulator = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create("AutoPreparedSpellsFieldManipulator")
            .SetGuiPresentation(Category.Feature)
            .SetSpellcastingClass(CharacterClassDefinitions.Sorcerer)
            .AddPreparedSpellGroup(1, CharmPerson, Sleep)
            .AddPreparedSpellGroup(2, HoldPerson, Invisibility)
            .AddPreparedSpellGroup(3, Counterspell, DispelMagic)
            .AddPreparedSpellGroup(4, Banishment, Confusion)
            .AddPreparedSpellGroup(5, DominatePerson, HoldMonster)
            .AddPreparedSpellGroup(6, GlobeOfInvulnerability)
            .AddToDB();

        var magicAffinityFieldManipulatorControlHeightened = FeatureDefinitionMagicAffinityBuilder
            .Create("MagicAffinityFieldManipulatorControlHeightened")
            .SetGuiPresentation(Category.Feature)
            .SetWarList(1,
                Banishment, // abjuration 4
                CharmPerson, // enchantment 1
                Confusion, // enchantment 4
                Counterspell, // abjuration 3
                DispelMagic, // abjuration 3
                DominatePerson, // Enchantment 5
                GlobeOfInvulnerability, // abjuration 6
                HoldMonster, // Enchantment 5
                HoldPerson, // enchantment 2
                Invisibility, // illusion 2
                Sleep) // enchantment 1
            .AddToDB();

        var magicAffinityFieldManipulatorDc = FeatureDefinitionMagicAffinityBuilder
            .Create("MagicAffinityFieldManipulatorDC")
            .SetGuiPresentation(Category.Feature)
            .SetCastingModifiers(0, SpellParamsModifierType.None, 2)
            .AddToDB();

        var proficiencyFieldManipulatorMentalSavingThrows = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyFieldManipulatorMentalSavingThrows")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(
                ProficiencyType.SavingThrow,
                AttributeDefinitions.Intelligence,
                AttributeDefinitions.Wisdom)
            .AddToDB();

        var powerFieldManipulatorDominatePerson = FeatureDefinitionPowerBuilder
            .Create("PowerFieldManipulatorDominatePerson")
            .SetGuiPresentation(Category.Feature, DominatePerson)
            .SetUsesAbilityBonus(ActivationTime.BonusAction, RechargeRate.LongRest, AttributeDefinitions.Charisma)
            .SetEffectDescription(DominatePerson.EffectDescription)
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("FieldManipulator")
            .SetGuiPresentation(Category.Subclass, RoguishShadowCaster)
            .AddFeaturesAtLevel(2,
                autoPreparedSpellsFieldManipulator,
                magicAffinityFieldManipulatorControlHeightened)
            .AddFeaturesAtLevel(6,
                magicAffinityFieldManipulatorDc)
            .AddFeaturesAtLevel(10,
                proficiencyFieldManipulatorMentalSavingThrows)
            .AddFeaturesAtLevel(14,
                powerFieldManipulatorDominatePerson)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceWizardArcaneTraditions;
}
