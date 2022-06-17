#if false
using SolastaCommunityExpansion.Builders;
using static SolastaCommunityExpansion.Api.DatabaseHelper.SpellDefinitions;
using static SolastaCommunityExpansion.Spells.AceHighSpells;

namespace SolastaCommunityExpansion.Classes.Magus;

// keep public as CE:MC depends on it
public static class MagusSpells
{
    internal static SpellListDefinition MagusSpellList { get; } = SpellListDefinitionBuilder
        .Create("MagusSpellList", DefinitionBuilder.CENamespaceGuid)
        .SetGuiPresentation(Category.SpellList)
        .ClearSpells()
        .SetSpellsAtLevel(1, Shield, MageArmor, ExpeditiousRetreat, ProtectionFromEvilGood,
            HellishRebukeSpell, PactMarkSpell)
        .SetSpellsAtLevel(2, SeeInvisibility, HoldPerson, Invisibility, MistyStep, RayOfEnfeeblement, Shatter,
            SpiderClimb)
        .SetSpellsAtLevel(3, Counterspell, DispelMagic, Fear, Fly, Slow, Haste)
        .SetSpellsAtLevel(4, Banishment, Blight, DimensionDoor, GreaterInvisibility)
        .SetSpellsAtLevel(5, HoldMonster, MindTwist, ConjureElemental)
        .FinalizeSpells()
        .AddToDB();
}
#endif
