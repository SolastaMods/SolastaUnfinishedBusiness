using SolastaCommunityExpansion.Builders;
using static SolastaCommunityExpansion.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaCommunityExpansion.Classes.Magus;

// keep public as CE:MC depends on it
public static class MagusSpells
{
    internal static SpellListDefinition MagusSpellList { get; } = SpellListDefinitionBuilder
        .Create("ClassMagusSpellList", DefinitionBuilder.CENamespaceGuid)
        .SetGuiPresentationNoContent()
        .ClearSpells()
        .SetSpellsAtLevel(0, Sparkle, Light, FireBolt, ChillTouch, RayOfFrost, ShockingGrasp, TrueStrike, ShadowDagger)
        .SetSpellsAtLevel(1, Shield, MageArmor, ExpeditiousRetreat, ProtectionFromEvilGood, FeatherFall, InflictWounds,
            Sleep, BurningHands, Jump, Longstrider, GuidingBolt)
        .SetSpellsAtLevel(2, SeeInvisibility, HoldPerson, MistyStep, FlameBlade, Shatter, SpiderClimb, ScorchingRay,
            Blur, FlamingSphere, AcidArrow, MagicWeapon, Blindness, RayOfEnfeeblement)
        .SetSpellsAtLevel(3, Counterspell, DispelMagic, Fear, Fly, Slow, Haste, BestowCurse, StinkingCloud, Fireball,
            LightningBolt, ProtectionFromEnergy, SleetStorm)
        .SetSpellsAtLevel(4, Banishment, Blight, DimensionDoor, GreaterInvisibility, PhantasmalKiller)
        .SetSpellsAtLevel(5, HoldMonster, MindTwist, ConjureElemental, Contagion)
        .SetSpellsAtLevel(6, Sunbeam, Disintegrate, GlobeOfInvulnerability, Eyebite)
        .FinalizeSpells()
        .AddToDB();
}
