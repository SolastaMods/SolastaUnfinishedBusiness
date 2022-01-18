using SolastaModApi;
using SolastaModApi.Extensions;
using System.Collections.Generic;

namespace SolastaArtificerMod
{
    internal class TinkererSpellList : BaseDefinitionBuilder<SpellListDefinition>
    {
        private TinkererSpellList(string name, string guid, GuiPresentation guiPresentation) : base(name, guid)
        {
            Definition.SetHasCantrips(true);
            Definition.SetMaxSpellLevel(5);
            SpellListDefinition.SpellsByLevelDuplet cantrips = new SpellListDefinition.SpellsByLevelDuplet
            {
                Spells = new List<SpellDefinition>()
            };
            cantrips.Spells.Add(DatabaseHelper.SpellDefinitions.AcidSplash);
            cantrips.Spells.Add(DatabaseHelper.SpellDefinitions.DancingLights);
            cantrips.Spells.Add(DatabaseHelper.SpellDefinitions.FireBolt);
            cantrips.Spells.Add(DatabaseHelper.SpellDefinitions.Guidance);
            cantrips.Spells.Add(DatabaseHelper.SpellDefinitions.Light);
            cantrips.Spells.Add(DatabaseHelper.SpellDefinitions.PoisonSpray);
            cantrips.Spells.Add(DatabaseHelper.SpellDefinitions.RayOfFrost);
            cantrips.Spells.Add(DatabaseHelper.SpellDefinitions.Resistance);
            cantrips.Spells.Add(DatabaseHelper.SpellDefinitions.ShockingGrasp);
            cantrips.Spells.Add(DatabaseHelper.SpellDefinitions.SpareTheDying);
            // melee weapon attack (booming blade/green flame blade)
            // pull enemy towards you (thorn whip/lightning lure)
            // I'm surrounded (thunderclap/sword burst)
            Definition.SpellsByLevel.Add(cantrips);

            SpellListDefinition.SpellsByLevelDuplet first = new SpellListDefinition.SpellsByLevelDuplet
            {
                Spells = new List<SpellDefinition>(),
                Level = 1
            };
            first.Spells.Add(DatabaseHelper.SpellDefinitions.CureWounds);
            first.Spells.Add(DatabaseHelper.SpellDefinitions.DetectMagic);
            first.Spells.Add(DatabaseHelper.SpellDefinitions.ExpeditiousRetreat);
            first.Spells.Add(DatabaseHelper.SpellDefinitions.FaerieFire);
            first.Spells.Add(DatabaseHelper.SpellDefinitions.FalseLife);
            first.Spells.Add(DatabaseHelper.SpellDefinitions.FeatherFall);
            first.Spells.Add(DatabaseHelper.SpellDefinitions.Grease);
            first.Spells.Add(DatabaseHelper.SpellDefinitions.Identify);
            first.Spells.Add(DatabaseHelper.SpellDefinitions.Jump);
            first.Spells.Add(DatabaseHelper.SpellDefinitions.Longstrider);
            //first.Spells.Add(DatabaseHelper.SpellDefinitions.PurifyFood);
            //first.Spells.Add(DatabaseHelper.SpellDefinitions.Sanctuary);
            //first.Spells.Add(DatabaseHelper.SpellDefinitions.Alarm);
            // absorb elements, snare, catapult, tasha's caustic brew
            Definition.SpellsByLevel.Add(first);

            SpellListDefinition.SpellsByLevelDuplet second = new SpellListDefinition.SpellsByLevelDuplet
            {
                Spells = new List<SpellDefinition>(),
                Level = 2
            };
            second.Spells.Add(DatabaseHelper.SpellDefinitions.Aid);
            second.Spells.Add(DatabaseHelper.SpellDefinitions.Blur);
            second.Spells.Add(DatabaseHelper.SpellDefinitions.Darkvision);
            second.Spells.Add(DatabaseHelper.SpellDefinitions.EnhanceAbility);
            second.Spells.Add(DatabaseHelper.SpellDefinitions.Invisibility);
            second.Spells.Add(DatabaseHelper.SpellDefinitions.LesserRestoration);
            second.Spells.Add(DatabaseHelper.SpellDefinitions.Levitate);
            second.Spells.Add(DatabaseHelper.SpellDefinitions.MagicWeapon);
            second.Spells.Add(DatabaseHelper.SpellDefinitions.ProtectionFromPoison);
            second.Spells.Add(DatabaseHelper.SpellDefinitions.SeeInvisibility);
            second.Spells.Add(DatabaseHelper.SpellDefinitions.SpiderClimb);
            // web, pyrotechnics, heat metal, enlarge/reduce
            Definition.SpellsByLevel.Add(second);

            SpellListDefinition.SpellsByLevelDuplet third = new SpellListDefinition.SpellsByLevelDuplet
            {
                Spells = new List<SpellDefinition>(),
                Level = 3
            };
            third.Spells.Add(DatabaseHelper.SpellDefinitions.CreateFood);
            third.Spells.Add(DatabaseHelper.SpellDefinitions.DispelMagic);
            third.Spells.Add(DatabaseHelper.SpellDefinitions.Fly);
            third.Spells.Add(DatabaseHelper.SpellDefinitions.Haste);
            third.Spells.Add(DatabaseHelper.SpellDefinitions.ProtectionFromEnergy);
            third.Spells.Add(DatabaseHelper.SpellDefinitions.Revivify);
            //third.Spells.Add(DatabaseHelper.SpellDefinitions.WaterBreathing);
            //third.Spells.Add(DatabaseHelper.SpellDefinitions.WaterWalk);
            // blink, elemental weapon, flame arrows
            Definition.SpellsByLevel.Add(third);

            SpellListDefinition.SpellsByLevelDuplet fourth = new SpellListDefinition.SpellsByLevelDuplet
            {
                Spells = new List<SpellDefinition>()
            };
            fourth.Spells.Add(DatabaseHelper.SpellDefinitions.FreedomOfMovement);
            fourth.Spells.Add(DatabaseHelper.SpellDefinitions.Stoneskin);
            fourth.Level = 4;
            // everything
            Definition.SpellsByLevel.Add(fourth);

            SpellListDefinition.SpellsByLevelDuplet fifth = new SpellListDefinition.SpellsByLevelDuplet
            {
                Level = 5,
                Spells = new List<SpellDefinition>()
            };
            fifth.Spells.Add(DatabaseHelper.SpellDefinitions.GreaterRestoration);
            // everything
            Definition.SpellsByLevel.Add(fifth);

            Definition.SetGuiPresentation(guiPresentation);
        }

        public static SpellListDefinition BuildSpellList()
        {
            return new TinkererSpellList("SpellListTinkerer", GuidHelper.Create(TinkererClass.GuidNamespace, "SpellListTinkerer").ToString(),
                new GuiPresentationBuilder("Feature/&NoContentTitle",
                "SpellList/&SpellListTinkererTitle").Build()).AddToDB();
        }
    }
}
