using SolastaModApi;
using SolastaModApi.Extensions;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Classes.Warlock.AHSpells;
using System.Collections.Generic;
using SolastaCommunityExpansion.Spells;

namespace SolastaCommunityExpansion.Classes.Warlock
{
    internal static class WarlockSpellList
    {
        public static SpellListDefinition ClassWarlockSpellList { get; private set; }

        public static void Build()
        {
            ClassWarlockSpellList = SpellListDefinitionBuilder
                .Create(DatabaseHelper.SpellListDefinitions.SpellListWizard, "SpellListClassWarlock", DefinitionBuilder.CENamespaceGuid)
                .SetSpellsAtLevel(0,
                    Features.DHEldritchInvocationsBuilder.EldritchBlast,
                    DatabaseHelper.SpellDefinitions.AnnoyingBee,
                    DatabaseHelper.SpellDefinitions.ChillTouch,
                    DatabaseHelper.SpellDefinitions.DancingLights,
                    DatabaseHelper.SpellDefinitions.PoisonSpray,
                    DatabaseHelper.SpellDefinitions.TrueStrike)
                .SetSpellsAtLevel(1,
                    DatabaseHelper.SpellDefinitions.CharmPerson,
                    DatabaseHelper.SpellDefinitions.ComprehendLanguages,
                    DatabaseHelper.SpellDefinitions.ExpeditiousRetreat,
                    DatabaseHelper.SpellDefinitions.ProtectionFromEvilGood,
                    // seems like it must be unfinished?
                    // HellishRebukeSpellBuilder.HellishRebukeSpell,
                    PactMarkSpellBuilder.PactMarkSpell)
                .SetSpellsAtLevel(2,
                    DatabaseHelper.SpellDefinitions.Darkness,
                    DatabaseHelper.SpellDefinitions.HoldPerson,
                    DatabaseHelper.SpellDefinitions.Invisibility,
                    DatabaseHelper.SpellDefinitions.MistyStep,
                    DatabaseHelper.SpellDefinitions.RayOfEnfeeblement,
                    DatabaseHelper.SpellDefinitions.Shatter,
                    DatabaseHelper.SpellDefinitions.SpiderClimb)
                .SetSpellsAtLevel(3,
                    DatabaseHelper.SpellDefinitions.Counterspell,
                    DatabaseHelper.SpellDefinitions.DispelMagic,
                    DatabaseHelper.SpellDefinitions.Fear,
                    DatabaseHelper.SpellDefinitions.Fly,
                    DatabaseHelper.SpellDefinitions.HypnoticPattern,
                    DatabaseHelper.SpellDefinitions.RemoveCurse,
                    DatabaseHelper.SpellDefinitions.Tongues,
                    DatabaseHelper.SpellDefinitions.VampiricTouch)
                .SetSpellsAtLevel(4,
                    DatabaseHelper.SpellDefinitions.Banishment,
                    DatabaseHelper.SpellDefinitions.Blight,
                    DatabaseHelper.SpellDefinitions.DimensionDoor)
                .SetSpellsAtLevel(5,
                    DatabaseHelper.SpellDefinitions.HoldMonster,
                    DatabaseHelper.SpellDefinitions.MindTwist)
                .SetSpellsAtLevel(6,
                    DatabaseHelper.SpellDefinitions.CircleOfDeath,
                    DatabaseHelper.SpellDefinitions.Eyebite,
                    DatabaseHelper.SpellDefinitions.ConjureFey,
                    DatabaseHelper.SpellDefinitions.TrueSeeing)
                .SetGuiPresentation(Category.SpellList)
                .SetMaxSpellLevel(9, true)
                .AddToDB();

            // 7th
            SpellDefinition FingerOfDeath = TryGetSpell("DHFingerOfDeathSpell");
            // 8th
            SpellDefinition DominateMonster = TryGetSpell("DHDominateMonsterSpell");
            SpellDefinition Feeblemind = TryGetSpell("DHFeeblemindSpell");
            SpellDefinition PowerWordStun = TryGetSpell("DHPowerWordStunSpell");
            // 9th
            SpellDefinition Weird = TryGetSpell("DHWeirdSpell");
            SpellDefinition Foresight = TryGetSpell("DHForesightSpell");
            SpellDefinition PowerWordKill = TryGetSpell("DHPowerWordKillSpell");

            var dictionaryofSpells = new Dictionary<SpellDefinition, int>
            {
                { FingerOfDeath, 7 },
                { DominateMonster, 8 },
                { Feeblemind, 8 },
                { PowerWordStun, 8 },
                { Weird, 9 },
                { Foresight, 9 },
                { PowerWordKill, 9 }
            };

            foreach (KeyValuePair<SpellDefinition, int> entry in dictionaryofSpells)
            {
                if (entry.Key != null) ClassWarlockSpellList.SpellsByLevel[entry.Value].Spells.Add(entry.Key);
            }

            static SpellDefinition TryGetSpell(string name)
            {
                return DatabaseRepository.GetDatabase<SpellDefinition>().TryGetElement(name, GuidHelper.Create(SrdSpells.DhBaseGuid, "DHFingerOfDeathSpell").ToString());
            }
        }
    }
}
