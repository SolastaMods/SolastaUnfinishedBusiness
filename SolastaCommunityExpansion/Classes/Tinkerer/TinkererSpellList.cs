using SolastaCommunityExpansion.Builders;
using SolastaModApi;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;
using static SolastaModApi.DatabaseHelper.SpellDefinitions;
using static SolastaCommunityExpansion.Classes.Tinkerer.FeatureHelpers;

namespace SolastaCommunityExpansion.Classes.Tinkerer
{
    internal sealed class TinkererSpellList : BaseDefinitionBuilder<SpellListDefinition>
    {
        private TinkererSpellList(string name, string guid, GuiPresentation guiPresentation) : base(name, guid)
        {
            Definition.SetHasCantrips(true);
            Definition.SetMaxSpellLevel(5);
            Definition.SpellsByLevel.AddRange(
                BuildSpellList(0, AcidSplash, DancingLights, FireBolt, Guidance, Light, PoisonSpray, RayOfFrost, Resistance, ShockingGrasp, SpareTheDying),
                BuildSpellList(1, CureWounds, DetectMagic, ExpeditiousRetreat, FaerieFire, FalseLife, FeatherFall, Grease, Identify, Jump, Longstrider),
                BuildSpellList(2, Aid, Blur, Darkvision, EnhanceAbility, Invisibility, LesserRestoration, Levitate, MagicWeapon, ProtectionFromPoison, SeeInvisibility, SpiderClimb),
                BuildSpellList(3, CreateFood, DispelMagic, Fly, Haste, ProtectionFromEnergy, Revivify),
                BuildSpellList(4, FreedomOfMovement, Stoneskin),
                BuildSpellList(5, GreaterRestoration)
            );

            Definition.SetGuiPresentation(guiPresentation);
        }

        public static SpellListDefinition BuildAndAddToDB()
        {
            return new TinkererSpellList("SpellListTinkerer", GuidHelper.Create(TinkererClass.GuidNamespace, "SpellListTinkerer").ToString(),
                new GuiPresentationBuilder("Feature/&NoContentTitle", "SpellList/&SpellListTinkererTitle").Build())
                .AddToDB();
        }
    }
}
