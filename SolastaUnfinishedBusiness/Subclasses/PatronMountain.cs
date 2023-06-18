using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.Models;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;
/*
Level 1
Barrier of Stone: When an ally within 30 feet of you takes damage, you may use your reaction to reduce the damage by an amount equal to twice your warlock level plus your Charisma modifier. You may use this power a number of times equal to your Charisma modifier per long rest.

Knowledge of Aeons: Gain proficiency in Nature and Survival. Gain Mountain as a favored terrain type when traveling on the over world map.

Level 6
Clinging Strength: You may use a bonus action on your turn to give yourself or an ally within 5 feet of you the benefits of Longstrider and Spider Climb for 1 hour (no concentration requirement). Once you use this power, you cannot use it again until you finish a short or long rest.

Eternal Guardian: You may use your Barrier of Stone feature a number of times equal to your Charisma modifier per short rest.

Level 10
The Mountain Wakes: You may cast Ice Storm as a fourth level spell a number of times equal to your proficiency bonus per long rest.

Level 14
Icebound Soul: Gain immunity to cold damage. The first time you hit an enemy with an attack on each of your turns, they must make a Constitution saving throw against your warlock spell DC (8 plus proficiency bonus plus Charisma modifier) or become blinded until the end of your next turn.
 */

[UsedImplicitly]
internal class PatronMountain : AbstractSubclass
{
    private const string Name = "PatronMountain";

    internal PatronMountain()
    {
        var spellList = SpellListDefinitionBuilder
            .Create(SpellListDefinitions.SpellListWizard, $"SpellList{Name}")
            .SetGuiPresentationNoContent(true)
            .ClearSpells()
            .SetSpellsAtLevel(1, SpellsContext.EarthTremor, Sleep)
            .SetSpellsAtLevel(2, HeatMetal, LesserRestoration)
            .SetSpellsAtLevel(3, ProtectionFromEnergy, SleetStorm)
            .SetSpellsAtLevel(4, FreedomOfMovement, IceStorm)
            .SetSpellsAtLevel(5, ConeOfCold, GreaterRestoration)
            .FinalizeSpells(true, 9)
            .AddToDB();

        var magicAffinityExpandedSpells = FeatureDefinitionMagicAffinityBuilder
            .Create($"MagicAffinity{Name}ExpandedSpells")
            .SetGuiPresentation("MagicAffinityPatronExpandedSpells", Category.Feature)
            .SetExtendedSpellList(spellList)
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, CharacterSubclassDefinitions.MartialMountaineer)
            .AddFeaturesAtLevel(1,
                magicAffinityExpandedSpells)
            .AddFeaturesAtLevel(6)
            .AddFeaturesAtLevel(10)
            .AddFeaturesAtLevel(14)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceWarlockOtherworldlyPatrons;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }
}
