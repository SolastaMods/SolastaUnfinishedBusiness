using SolastaCommunityExpansion.Builders;
using SolastaModApi;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;
using static SolastaCommunityExpansion.Classes.Tinkerer.FeatureHelpers;
using static SolastaModApi.DatabaseHelper.SpellDefinitions;

namespace SolastaCommunityExpansion.Classes.Tinkerer
{
    internal sealed class TinkererSpellList : SpellListDefinitionBuilder
    {
        private TinkererSpellList(string name, string guid, GuiPresentation guiPresentation) : base(name, guid)
        {
            Definition.SetHasCantrips(true);
            Definition.SetMaxSpellLevel(5);
            Definition.SpellsByLevel.AddRange(
                // melee weapon attack (booming blade/green flame blade)
                // pull enemy towards you (thorn whip/lightning lure)
                // I'm surrounded (thunderclap/sword burst)
                BuildSpellList(0, AcidSplash, DancingLights, FireBolt, Guidance, Light, PoisonSpray, RayOfFrost, Resistance, ShockingGrasp, SpareTheDying),
                // absorb elements, snare, catapult, tasha's caustic brew
                BuildSpellList(1, CureWounds, DetectMagic, ExpeditiousRetreat, FaerieFire, FalseLife, FeatherFall, Grease, Identify, Jump, Longstrider),
                // web, pyrotechnics, heat metal, enlarge/reduce
                BuildSpellList(2, Aid, Blur, Darkvision, EnhanceAbility, Invisibility, LesserRestoration, Levitate, MagicWeapon, ProtectionFromPoison, SeeInvisibility, SpiderClimb),
                // blink, elemental weapon, flame arrows
                BuildSpellList(3, CreateFood, DispelMagic, Fly, Haste, ProtectionFromEnergy, Revivify),
                // everything
                BuildSpellList(4, FreedomOfMovement, Stoneskin),
                // everything
                BuildSpellList(5, GreaterRestoration)
            );

            Definition.SetGuiPresentation(guiPresentation);
        }

        public static SpellListDefinition BuildAndAddToDB()
        {
            return new TinkererSpellList("SpellListTinkerer", GuidHelper.Create(TinkererClass.GuidNamespace, "SpellListTinkerer").ToString(),
                new GuiPresentationBuilder("SpellList/&SpellListTinkererTitle", "Feature/&NoContentTitle").Build())
                .AddToDB();
        }
    }
}
