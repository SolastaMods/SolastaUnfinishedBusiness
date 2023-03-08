using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
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
            .SetGuiPresentation("ExpandedSpells", Category.Feature)
            .SetAutoTag("Origin")
            .SetSpellcastingClass(CharacterClassDefinitions.Sorcerer)
            .AddPreparedSpellGroup(1, Sleep)
            .AddPreparedSpellGroup(2, Invisibility)
            .AddPreparedSpellGroup(3, Counterspell)
            .AddPreparedSpellGroup(4, Banishment)
            .AddPreparedSpellGroup(5, HoldMonster)
            .AddPreparedSpellGroup(6, GlobeOfInvulnerability)
            .AddToDB();

#if false
        // Feature/&MagicAffinityFieldManipulatorControlHeightenedDescription=When casting some Enchantment, Abjuration, and Illusion spells, the ones in your auto prepared list, they are cast at a spell slot 1 level higher than the one used.
        // Feature/&MagicAffinityFieldManipulatorControlHeightenedTitle=Arcane Manipulation
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
#endif

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
            .SetGuiPresentation(Category.Subclass,
                Sprites.GetSprite("SorcererFieldManipulator", Resources.SorcererFieldManipulator, 256))
            .AddFeaturesAtLevel(1,
                // magicAffinityFieldManipulatorControlHeightened
                autoPreparedSpellsFieldManipulator)
            .AddFeaturesAtLevel(6,
                magicAffinityFieldManipulatorDc)
            .AddFeaturesAtLevel(14,
                proficiencyFieldManipulatorMentalSavingThrows)
            .AddFeaturesAtLevel(18,
                powerFieldManipulatorDominatePerson)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceSorcerousOrigin;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }
}
