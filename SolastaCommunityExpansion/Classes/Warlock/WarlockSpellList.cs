using System;
using System.Collections.Generic;
using SolastaModApi;
using SolastaModApi.Extensions;
using SolastaCommunityExpansion.Builders;

namespace SolastaCommunityExpansion.Classes.Warlock
{
    internal class WarlockSpellList
    {

        public static SpellListDefinition ClassWarlockSpellList;
        public static void Build()
        {

            ClassWarlockSpellList = SpellListDefinitionBuilder.CreateSpellList(
                "SpellListClassWarlock",
                GuidHelper.Create(new Guid(Settings.GUID), "SpellListClassWarlock").ToString(),
                "SpellList/&SpellListClassWarlockDescription",
                DatabaseHelper.SpellListDefinitions.SpellListWizard,
                new List<SpellDefinition>
                    {
                        DHEldritchInvocationsBuilder.EldritchBlast,
                        DatabaseHelper.SpellDefinitions.AnnoyingBee,
                        DatabaseHelper.SpellDefinitions.ChillTouch,
                        DatabaseHelper.SpellDefinitions.DancingLights,
                        DatabaseHelper.SpellDefinitions.PoisonSpray,
                        DatabaseHelper.SpellDefinitions.TrueStrike
                    },
                new List<SpellDefinition>
                    {
                        DatabaseHelper.SpellDefinitions.CharmPerson,
                        DatabaseHelper.SpellDefinitions.ComprehendLanguages,
                        DatabaseHelper.SpellDefinitions.ExpeditiousRetreat,
                        DatabaseHelper.SpellDefinitions.ProtectionFromEvilGood,
                        // seems like it must be unfinished?
                        // HellishRebukeSpellBuilder.HellishRebukeSpell,
                        PactMarkSpellBuilder.PactMarkSpell
                    },
                new List<SpellDefinition>
                    {
                        DatabaseHelper.SpellDefinitions.Darkness,
                        DatabaseHelper.SpellDefinitions.HoldPerson,
                        DatabaseHelper.SpellDefinitions.Invisibility,
                        DatabaseHelper.SpellDefinitions.MistyStep,
                        DatabaseHelper.SpellDefinitions.RayOfEnfeeblement,
                        DatabaseHelper.SpellDefinitions.Shatter,
                        DatabaseHelper.SpellDefinitions.SpiderClimb
                    },
                 new List<SpellDefinition>
                    {
                        DatabaseHelper.SpellDefinitions.Counterspell,
                        DatabaseHelper.SpellDefinitions.DispelMagic,
                        DatabaseHelper.SpellDefinitions.Fear,
                        DatabaseHelper.SpellDefinitions.Fly,
                        DatabaseHelper.SpellDefinitions.HypnoticPattern,
                        DatabaseHelper.SpellDefinitions.RemoveCurse,
                        DatabaseHelper.SpellDefinitions.Tongues,
                        DatabaseHelper.SpellDefinitions.VampiricTouch
                    },
                  new List<SpellDefinition>
                    {
                        DatabaseHelper.SpellDefinitions.Banishment,
                        DatabaseHelper.SpellDefinitions.Blight,
                        DatabaseHelper.SpellDefinitions.DimensionDoor
                    },
                  new List<SpellDefinition>
                   {
                       DatabaseHelper.SpellDefinitions.HoldMonster,
                       DatabaseHelper.SpellDefinitions.MindTwist
                   },
                  new List<SpellDefinition>
                   {
                       DatabaseHelper.SpellDefinitions.CircleOfDeath,
                       DatabaseHelper.SpellDefinitions.Eyebite,
                       DatabaseHelper.SpellDefinitions.ConjureFey,
                       DatabaseHelper.SpellDefinitions.TrueSeeing,
                   },
                  new List<SpellDefinition>
                  {

                  },
                  new List<SpellDefinition>
                  {

                  },
                  new List<SpellDefinition>
                  {

                  }
                )
                .SetHasCantrips(true)
                .SetGuiPresentation(new GuiPresentationBuilder("Feature/&NoContentTitle", "SpellList/&SpellListClassWarlockDescription").Build())
                .SetMaxSpellLevel(9);

            // 7th
            SpellDefinition  FingerOfDeath= DatabaseRepository.GetDatabase<SpellDefinition>().TryGetElement("DHFingerOfDeathSpell", GuidHelper.Create(new System.Guid("05c1b1dbae144731b4505c1232fdc37e"), "DHFingerOfDeathSpell").ToString());
            // 8th
            SpellDefinition DominateMonster = DatabaseRepository.GetDatabase<SpellDefinition>().TryGetElement("DHDominateMonsterSpell", GuidHelper.Create(new System.Guid("05c1b1dbae144731b4505c1232fdc37e"), "DHDominateMonsterSpell").ToString());
            SpellDefinition Feeblemind= DatabaseRepository.GetDatabase<SpellDefinition>().TryGetElement("DHFeeblemindSpell", GuidHelper.Create(new System.Guid("05c1b1dbae144731b4505c1232fdc37e"), "DHFeeblemindSpell").ToString());
            SpellDefinition PowerWordStun= DatabaseRepository.GetDatabase<SpellDefinition>().TryGetElement("DHPowerWordStunSpell", GuidHelper.Create(new System.Guid("05c1b1dbae144731b4505c1232fdc37e"), "DHPowerWordStunSpell").ToString());
            // 9th
            SpellDefinition Weird = DatabaseRepository.GetDatabase<SpellDefinition>().TryGetElement("DHWeirdSpell", GuidHelper.Create(new System.Guid("05c1b1dbae144731b4505c1232fdc37e"), "DHWeirdSpell").ToString());
            SpellDefinition Foresight = DatabaseRepository.GetDatabase<SpellDefinition>().TryGetElement("DHForesightSpell", GuidHelper.Create(new System.Guid("05c1b1dbae144731b4505c1232fdc37e"), "DHForesightSpell").ToString());
            SpellDefinition PowerWordKill = DatabaseRepository.GetDatabase<SpellDefinition>().TryGetElement("DHPowerWordKillSpell", GuidHelper.Create(new System.Guid("05c1b1dbae144731b4505c1232fdc37e"), "DHPowerWordKillSpell").ToString());
            //            SpellDefinition = DatabaseRepository.GetDatabase<SpellDefinition>().TryGetElement("DH", GuidHelper.Create(new System.Guid("05c1b1dbae144731b4505c1232fdc37e"), "DH").ToString());

            var dictionaryofSpells = new Dictionary<SpellDefinition, int>();
            dictionaryofSpells.Add(FingerOfDeath, 7);
            dictionaryofSpells.Add(DominateMonster, 8);
            dictionaryofSpells.Add(Feeblemind, 8);
            dictionaryofSpells.Add(PowerWordStun, 8);
            dictionaryofSpells.Add(Weird, 9);
            dictionaryofSpells.Add(Foresight, 9);
            dictionaryofSpells.Add(PowerWordKill, 9);

            foreach (KeyValuePair<SpellDefinition, int> entry  in dictionaryofSpells)
            {
               if(entry.Key!= null) ClassWarlockSpellList.SpellsByLevel[entry.Value].Spells.Add(entry.Key);
            }


        }
    }
}
