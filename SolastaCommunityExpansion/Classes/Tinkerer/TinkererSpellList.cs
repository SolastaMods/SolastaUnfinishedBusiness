using SolastaCommunityExpansion.Builders;
using SolastaModApi.Extensions;
using static SolastaModApi.DatabaseHelper.SpellDefinitions;

namespace SolastaCommunityExpansion.Classes.Tinkerer
{
    internal static class TinkererSpellList
    {
        private static SpellListDefinition _spellList;
        public static SpellListDefinition SpellList => _spellList ??=
             SpellListDefinitionBuilder.Create("SpellListTinkerer", TinkererClass.GuidNamespace)
                .SetGuiPresentation("SpellListTinkerer", Category.SpellList)
                .ClearSpells()
                // melee weapon attack (booming blade/green flame blade)
                // pull enemy towards you (thorn whip/lightning lure)
                // I'm surrounded (thunderclap/sword burst)
                .SetSpellsAtLevel(0, AcidSplash, DancingLights, FireBolt, Guidance, Light, PoisonSpray, RayOfFrost, Resistance, ShockingGrasp, SpareTheDying)
                // absorb elements, snare, catapult, tasha's caustic brew
                .SetSpellsAtLevel(1, CureWounds, DetectMagic, ExpeditiousRetreat, FaerieFire, FalseLife, FeatherFall, Grease, Identify, Jump, Longstrider)
                // web, pyrotechnics, heat metal, enlarge/reduce
                .SetSpellsAtLevel(2, Aid, Blur, Darkvision, EnhanceAbility, Invisibility, LesserRestoration, Levitate, MagicWeapon, ProtectionFromPoison, SeeInvisibility, SpiderClimb)
                // blink, elemental weapon, flame arrows
                .SetSpellsAtLevel(3, CreateFood, DispelMagic, Fly, Haste, ProtectionFromEnergy, Revivify)
                // everything
                .SetSpellsAtLevel(4, FreedomOfMovement, Stoneskin)
                // everything
                .SetSpellsAtLevel(5, GreaterRestoration)
                .FinalizeSpells()
                .AddToDB();
    }
}
