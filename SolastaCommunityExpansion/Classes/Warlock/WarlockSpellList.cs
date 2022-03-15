using SolastaModApi;
using SolastaModApi.Extensions;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Classes.Warlock.AHSpells;
using System.Collections.Generic;
using SolastaCommunityExpansion.Spells;


//******************************************************************************************
//  DO NOT REFACTOR OR CHANGE WITHOUT TESTING OR TAKING RESPOSBILITY FOR CODE GOING FORWARD
//******************************************************************************************

namespace SolastaCommunityExpansion.Classes.Warlock
{
    internal static class ClassWarlockSpellList
    {
        public static SpellListDefinition WarlockSpellList;//{ get; private set; }

        public static void Build()
        {
             WarlockSpellList = SpellListDefinitionBuilder
                .Create(DatabaseHelper.SpellListDefinitions.SpellListWizard, "SpellListClassWarlock", DefinitionBuilder.CENamespaceGuid)

                .SetGuiPresentation(Category.SpellList)
                .SetMaxSpellLevel(9, true)
                .AddToDB();
            WarlockSpellList.ClearSpellsByLevel();
            WarlockSpellList.SpellsByLevel.AddRange(new List<SpellListDefinition.SpellsByLevelDuplet>()
            {
                new SpellListDefinition.SpellsByLevelDuplet
                {
                    Level =0,
                     Spells = new List<SpellDefinition>
                    {
                        Features.DHEldritchInvocationsBuilder.EldritchBlast,
                        DatabaseHelper.SpellDefinitions.AnnoyingBee,
                        DatabaseHelper.SpellDefinitions.ChillTouch,
                        DatabaseHelper.SpellDefinitions.DancingLights,
                        DatabaseHelper.SpellDefinitions.PoisonSpray,
                        DatabaseHelper.SpellDefinitions.TrueStrike
                    }
                },
                new SpellListDefinition.SpellsByLevelDuplet
                {
                    Level =1,
                     Spells = new List<SpellDefinition>
                    {
                        DatabaseHelper.SpellDefinitions.CharmPerson,
                        DatabaseHelper.SpellDefinitions.ComprehendLanguages,
                        DatabaseHelper.SpellDefinitions.ExpeditiousRetreat,
                        DatabaseHelper.SpellDefinitions.ProtectionFromEvilGood,
                        // seems like it must be unfinished?
                        // HellishRebukeSpellBuilder.HellishRebukeSpell,
                        PactMarkSpellBuilder.PactMarkSpell
                    }
                },
                new SpellListDefinition.SpellsByLevelDuplet
                {
                    Level =2,
                     Spells =new List<SpellDefinition>
                    {
                        DatabaseHelper.SpellDefinitions.Darkness,
                        DatabaseHelper.SpellDefinitions.HoldPerson,
                        DatabaseHelper.SpellDefinitions.Invisibility,
                        DatabaseHelper.SpellDefinitions.MistyStep,
                        DatabaseHelper.SpellDefinitions.RayOfEnfeeblement,
                        DatabaseHelper.SpellDefinitions.Shatter,
                        DatabaseHelper.SpellDefinitions.SpiderClimb
                    }
                },
                new SpellListDefinition.SpellsByLevelDuplet
                {
                    Level =3,
                     Spells =new List<SpellDefinition>
                    {
                        DatabaseHelper.SpellDefinitions.Counterspell,
                        DatabaseHelper.SpellDefinitions.DispelMagic,
                        DatabaseHelper.SpellDefinitions.Fear,
                        DatabaseHelper.SpellDefinitions.Fly,
                        DatabaseHelper.SpellDefinitions.HypnoticPattern,
                        DatabaseHelper.SpellDefinitions.RemoveCurse,
                        DatabaseHelper.SpellDefinitions.Tongues,
                        DatabaseHelper.SpellDefinitions.VampiricTouch
                    }
                },
                new SpellListDefinition.SpellsByLevelDuplet
                {
                    Level =4,
                     Spells =  new List<SpellDefinition>
                    {
                        DatabaseHelper.SpellDefinitions.Banishment,
                        DatabaseHelper.SpellDefinitions.Blight,
                        DatabaseHelper.SpellDefinitions.DimensionDoor
                    }
                },
                new SpellListDefinition.SpellsByLevelDuplet
                {
                    Level =5,
                     Spells = new List<SpellDefinition>
                   {
                       DatabaseHelper.SpellDefinitions.HoldMonster,
                       DatabaseHelper.SpellDefinitions.MindTwist
                   }
                },
                new SpellListDefinition.SpellsByLevelDuplet
                {
                    Level =6,
                     Spells = new List<SpellDefinition>
                   {
                       DatabaseHelper.SpellDefinitions.CircleOfDeath,
                       DatabaseHelper.SpellDefinitions.Eyebite,
                       DatabaseHelper.SpellDefinitions.ConjureFey,
                       DatabaseHelper.SpellDefinitions.TrueSeeing,
                   }
                },
                new SpellListDefinition.SpellsByLevelDuplet
                {
                    Level =7,
                     Spells =  new List<SpellDefinition>{}
                },
                new SpellListDefinition.SpellsByLevelDuplet
                {
                    Level =8,
                     Spells =new List<SpellDefinition>{}
                },
                new SpellListDefinition.SpellsByLevelDuplet
                {
                    Level =9,
                     Spells =new List<SpellDefinition>{}
                }
            });


            // 7th
            SpellDefinition FingerOfDeath = DatabaseRepository.GetDatabase<SpellDefinition>().TryGetElement("DHFingerOfDeathSpell", GuidHelper.Create(new System.Guid("05c1b1dbae144731b4505c1232fdc37e"), "DHFingerOfDeathSpell").ToString());
            // 8th
            SpellDefinition DominateMonster = DatabaseRepository.GetDatabase<SpellDefinition>().TryGetElement("DHDominateMonsterSpell", GuidHelper.Create(new System.Guid("05c1b1dbae144731b4505c1232fdc37e"), "DHDominateMonsterSpell").ToString());
            SpellDefinition Feeblemind = DatabaseRepository.GetDatabase<SpellDefinition>().TryGetElement("DHFeeblemindSpell", GuidHelper.Create(new System.Guid("05c1b1dbae144731b4505c1232fdc37e"), "DHFeeblemindSpell").ToString());
            SpellDefinition PowerWordStun = DatabaseRepository.GetDatabase<SpellDefinition>().TryGetElement("DHPowerWordStunSpell", GuidHelper.Create(new System.Guid("05c1b1dbae144731b4505c1232fdc37e"), "DHPowerWordStunSpell").ToString());
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


            foreach (KeyValuePair<SpellDefinition, int> entry in dictionaryofSpells)
            {
                if (entry.Key != null) WarlockSpellList.SpellsByLevel[entry.Value].Spells.Add(entry.Key);
             }


        }
    }
}
