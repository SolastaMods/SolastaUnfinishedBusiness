using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static SolastaCommunityExpansion.Classes.Warlock.WarlockSpells;
using static FeatureDefinitionCastSpell;

namespace SolastaMulticlass.Models
{
    internal enum CasterType
    {
        None,
        Full,
        Half,
        HalfRoundUp,
        OneThird
    }

    internal static class SharedSpellsContext
    {
        internal static Color LightGreenSlot = new(0f, 1f, 0f, 1f);

        internal static Color DarkGreenSlot = new(0f, 0.5f, 0f, 1f);

        internal static Color WhiteSlot = new(1f, 1f, 1f, 1f);

        internal static Dictionary<string, CasterType> ClassCasterType { get; } = new()
        {
            { IntegrationContext.CLASS_ALCHEMIST, CasterType.HalfRoundUp },
            { IntegrationContext.CLASS_BARD, CasterType.Full },
            { IntegrationContext.CLASS_TINKERER, CasterType.HalfRoundUp },
            { IntegrationContext.CLASS_WITCH, CasterType.Full },
            { RuleDefinitions.ClericClass, CasterType.Full },
            { RuleDefinitions.DruidClass, CasterType.Full },
            { RuleDefinitions.SorcererClass, CasterType.Full },
            { RuleDefinitions.WizardClass, CasterType.Full },
            { RuleDefinitions.PaladinClass, CasterType.Half },
            { RuleDefinitions.RangerClass, CasterType.Half }
        };

        internal static Dictionary<string, CasterType> SubclassCasterType { get; } = new()
        {
            { "BarbarianSubclassPrimalPathOfWarShaman", CasterType.OneThird }, // Holic
            { "MartialEldritchKnight", CasterType.OneThird }, // Holic
            { "MartialSpellblade", CasterType.OneThird },
            { "RoguishShadowCaster", CasterType.OneThird },
            { "RoguishConArtist", CasterType.OneThird }, // ChrisJohnDigital
            { "FighterSpellShield", CasterType.OneThird } // ChrisJohnDigital
        };

        internal class CasterLevelContext
        {
            private readonly Dictionary<CasterType, int> levels;

            internal CasterLevelContext()
            {
                levels = new Dictionary<CasterType, int>
                {
                    { CasterType.None, 0 },
                    { CasterType.Full, 0 },
                    { CasterType.Half, 0 },
                    { CasterType.HalfRoundUp, 0 },
                    { CasterType.OneThird, 0 },
                };
            }

            internal void IncrementCasterLevel(CasterType casterType, int increment)
            {
                levels[casterType] += increment;
            }

            internal int GetCasterLevel()
            {
                var casterLevel = 0;

                // Full Casters
                casterLevel += levels[CasterType.Full];

                // Tinkerer / ...
                if (levels[CasterType.HalfRoundUp] == 1)
                {
                    casterLevel++;
                }
                // Half Casters
                else
                {
                    casterLevel += (int)Math.Floor(levels[CasterType.HalfRoundUp] / 2.0);
                }

                casterLevel += (int)Math.Floor(levels[CasterType.Half] / 2.0);

                // Con Artist / ...
                casterLevel += (int)Math.Floor(levels[CasterType.OneThird] / 3.0);

                return casterLevel;
            }

            internal int GetSpellLevel()
            {
                var spellLevel = 0;

                // Full Casters
                spellLevel += (int)Math.Ceiling(levels[CasterType.Full] / 2.0);

                // Tinkerer / ...
                spellLevel += (int)Math.Ceiling(levels[CasterType.HalfRoundUp] / 4.0);

                // Half Casters
                if (levels[CasterType.Half] > 1)
                {
                    spellLevel += (int)Math.Ceiling(levels[CasterType.Half] / 4.0);
                }

                // Con Artist / ...
                if (levels[CasterType.OneThird] > 2)
                {
                    spellLevel += (int)Math.Ceiling(levels[CasterType.OneThird] / 6.0);
                }

                return Math.Min(9, spellLevel);
            }
        }

        private static CasterType GetCasterTypeForClassOrSubclass(CharacterClassDefinition characterClassDefinition, CharacterSubclassDefinition characterSubclassDefinition)
        {
            if (characterClassDefinition != null && ClassCasterType.ContainsKey(characterClassDefinition.Name) && ClassCasterType[characterClassDefinition.Name] != CasterType.None)
            {
                return ClassCasterType[characterClassDefinition.Name];
            }

            if (characterSubclassDefinition != null && SubclassCasterType.ContainsKey(characterSubclassDefinition.Name))
            {
                return SubclassCasterType[characterSubclassDefinition.Name];
            }

            return CasterType.None;
        }

        internal static bool ForceLongRestSlot { get; set; }

        internal static RulesetCharacterHero GetHero(string name)
        {
            // try to get hero from game campaign
            var gameCampaign = Gui.GameCampaign;

            if (gameCampaign != null)
            {
                var gameCampaignCharacter = gameCampaign.Party.CharactersList.Find(x => x.RulesetCharacter.Name == name);

                if (gameCampaignCharacter != null)
                {
                    return gameCampaignCharacter.RulesetCharacter as RulesetCharacterHero;
                }
            }

            // otherwise gets hero from level up context
            var hero = LevelUpContext.GetHero(name);

            if (hero != null)
            {
                return hero;
            }

            // finally falls back to inspection
            return InspectionPanelContext.SelectedHero;
        }

        internal static bool IsMulticaster(RulesetCharacterHero rulesetCharacterHero)
        {
            var repertoires = rulesetCharacterHero?.SpellRepertoires?
                .FindAll(sr => sr.SpellCastingFeature.SpellCastingOrigin == CastingOrigin.Class || sr.SpellCastingFeature.SpellCastingOrigin == CastingOrigin.Subclass);

            return repertoires?.Count > 1;
        }

        internal static bool IsSharedcaster(RulesetCharacterHero rulesetCharacterHero)
        {
            var repertoires = rulesetCharacterHero?.SpellRepertoires?
                .FindAll(sr => (sr.SpellCastingFeature.SpellCastingOrigin == CastingOrigin.Class || sr.SpellCastingFeature.SpellCastingOrigin == CastingOrigin.Subclass) 
                                && !IsWarlock(sr.SpellCastingClass));

            return repertoires?.Count > 1;
        }

        internal static bool IsWarlock(CharacterClassDefinition characterClassDefinition)
        {
            return characterClassDefinition == IntegrationContext.WarlockClass;
        }

        internal static int GetWarlockLevel(RulesetCharacterHero rulesetCharacterHero)
        {
            var warlockLevel = 0;
            var warlock = rulesetCharacterHero?.ClassesAndLevels.Keys.ToList().Find(x => x == IntegrationContext.WarlockClass);

            if (warlock != null)
            {
                warlockLevel = rulesetCharacterHero.ClassesAndLevels[warlock];
            }

            return warlockLevel;
        }

        internal static int GetWarlockSpellLevel(RulesetCharacterHero rulesetCharacterHero)
        {
            var warlockLevel = GetWarlockLevel(rulesetCharacterHero);

            if (warlockLevel > 0)
            {
                return WarlockCastingSlots[warlockLevel - 1].Slots.IndexOf(0);
            }

            return 0;
        }

        internal static int GetWarlockMaxSlots(RulesetCharacterHero rulesetCharacterHero)
        {
            var warlockLevel = GetWarlockLevel(rulesetCharacterHero);

            if (warlockLevel > 0)
            {
                return WarlockCastingSlots[warlockLevel - 1].Slots[0];
            }

            return 0;
        }

        internal static RulesetSpellRepertoire GetWarlockSpellRepertoire(RulesetCharacterHero rulesetCharacterHero)
        {
            return rulesetCharacterHero?.SpellRepertoires.FirstOrDefault(x => IsWarlock(x.SpellCastingClass));
        }

        internal static int GetSharedCasterLevel(RulesetCharacterHero rulesetCharacterHero)
        {
            var casterLevelContext = new CasterLevelContext();

            if (rulesetCharacterHero?.ClassesAndLevels != null)
            {
                foreach (var classAndLevel in rulesetCharacterHero.ClassesAndLevels)
                {
                    var currentCharacterClassDefinition = classAndLevel.Key;

                    rulesetCharacterHero.ClassesAndSubclasses.TryGetValue(currentCharacterClassDefinition, out var currentCharacterSubclassDefinition);

                    var casterType = GetCasterTypeForClassOrSubclass(currentCharacterClassDefinition, currentCharacterSubclassDefinition);

                    casterLevelContext.IncrementCasterLevel(casterType, classAndLevel.Value);
                }
            }

            return casterLevelContext.GetCasterLevel();
        }

        internal static int GetSharedSpellLevel(RulesetCharacterHero rulesetCharacterHero)
        {
            if (IsSharedcaster(rulesetCharacterHero))
            {
                var sharedCasterLevel = GetSharedCasterLevel(rulesetCharacterHero);

                if (sharedCasterLevel > 0)
                {
                    return FullCastingSlots[sharedCasterLevel - 1].Slots.IndexOf(0);
                }

                return 0;
            }
            else
            {
                var otherCasterRepertoire = rulesetCharacterHero.SpellRepertoires.Find(sr =>
                    (sr.SpellCastingFeature.SpellCastingOrigin == CastingOrigin.Class || sr.SpellCastingFeature.SpellCastingOrigin == CastingOrigin.Subclass) && !IsWarlock(sr.SpellCastingClass));

                return GetClassSpellLevel(rulesetCharacterHero, otherCasterRepertoire?.SpellCastingClass, otherCasterRepertoire?.SpellCastingSubclass);
            }
        }

        internal static int GetClassCasterLevel(
            RulesetCharacterHero rulesetCharacterHero,
            CharacterClassDefinition filterCharacterClassDefinition,
            CharacterSubclassDefinition filterCharacterSublassDefinition = null)
        {
            var casterLevelContext = new CasterLevelContext();

            if (rulesetCharacterHero?.ClassesAndLevels == null)
            {
                return 0;
            }

            foreach (var classAndLevel in rulesetCharacterHero.ClassesAndLevels)
            {
                var currentCharacterClassDefinition = classAndLevel.Key;

                rulesetCharacterHero.ClassesAndSubclasses.TryGetValue(currentCharacterClassDefinition, out var currentCharacterSubclassDefinition);

                if (filterCharacterClassDefinition == currentCharacterClassDefinition
                    || (filterCharacterSublassDefinition != null && filterCharacterSublassDefinition == currentCharacterSubclassDefinition))
                {
                    var casterType = GetCasterTypeForClassOrSubclass(currentCharacterClassDefinition, currentCharacterSubclassDefinition);

                    casterLevelContext.IncrementCasterLevel(casterType, classAndLevel.Value);
                }
            }

            return casterLevelContext.GetCasterLevel();
        }

        internal static int GetClassSpellLevel(
            RulesetCharacterHero rulesetCharacterHero,
            CharacterClassDefinition filterCharacterClassDefinition,
            CharacterSubclassDefinition filterCharacterSubclassDefinition = null)
        {
            int classCasterLevel;

            if (IsWarlock(filterCharacterClassDefinition))
            {
                classCasterLevel = GetWarlockSpellLevel(rulesetCharacterHero);
            }
            else
            {
                var casterLevelContext = new CasterLevelContext();

                if (rulesetCharacterHero != null && rulesetCharacterHero.ClassesAndLevels != null)
                {
                    foreach (var classAndLevel in rulesetCharacterHero.ClassesAndLevels)
                    {
                        var currentCharacterClassDefinition = classAndLevel.Key;

                        rulesetCharacterHero.ClassesAndSubclasses.TryGetValue(currentCharacterClassDefinition, out var currentCharacterSubclassDefinition);

                        if (filterCharacterClassDefinition == currentCharacterClassDefinition
                            || (filterCharacterSubclassDefinition != null && filterCharacterSubclassDefinition == currentCharacterSubclassDefinition))
                        {
                            var casterType = GetCasterTypeForClassOrSubclass(currentCharacterClassDefinition, currentCharacterSubclassDefinition);

                            casterLevelContext.IncrementCasterLevel(casterType, classAndLevel.Value);
                        }
                    }
                }

                classCasterLevel = casterLevelContext.GetSpellLevel();
            }

            return classCasterLevel;
        }

        internal static int GetCombinedSpellLevel(RulesetCharacterHero rulesetCharacterHero)
        {
            return Math.Max(GetWarlockSpellLevel(rulesetCharacterHero), GetSharedSpellLevel(rulesetCharacterHero));
        }

        // allows casters to use slots above their caster level if multiclassed
        internal static void Load()
        {
            var spellListDefinition = DatabaseRepository.GetDatabase<SpellListDefinition>();

            foreach (var spellsByLevel in spellListDefinition.Select(x => x.SpellsByLevel))
            {
                while (spellsByLevel.Count < 9)
                {
                    spellsByLevel.Add(new SpellListDefinition.SpellsByLevelDuplet { Level = spellsByLevel.Count, Spells = new() });
                }
            }
        }

        internal const int PACT_MAGIC_SLOT_TAB_INDEX = -1;

        //internal static readonly List<SlotsByLevelDuplet> WarlockCastingSlots = new()
        //{
        //    new() { Slots = new List<int> { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, Level = 01 },
        //    new() { Slots = new List<int> { 2, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, Level = 02 },
        //    new() { Slots = new List<int> { 2, 2, 0, 0, 0, 0, 0, 0, 0, 0 }, Level = 03 },
        //    new() { Slots = new List<int> { 2, 2, 0, 0, 0, 0, 0, 0, 0, 0 }, Level = 04 },
        //    new() { Slots = new List<int> { 2, 2, 2, 0, 0, 0, 0, 0, 0, 0 }, Level = 05 },
        //    new() { Slots = new List<int> { 2, 2, 2, 0, 0, 0, 0, 0, 0, 0 }, Level = 06 },
        //    new() { Slots = new List<int> { 2, 2, 2, 2, 0, 0, 0, 0, 0, 0 }, Level = 07 },
        //    new() { Slots = new List<int> { 2, 2, 2, 2, 0, 0, 0, 0, 0, 0 }, Level = 08 },
        //    new() { Slots = new List<int> { 2, 2, 2, 2, 2, 0, 0, 0, 0, 0 }, Level = 09 },
        //    new() { Slots = new List<int> { 2, 2, 2, 2, 2, 0, 0, 0, 0, 0 }, Level = 10 },
        //    new() { Slots = new List<int> { 3, 3, 3, 3, 3, 0, 0, 0, 0, 0 }, Level = 11 },
        //    new() { Slots = new List<int> { 3, 3, 3, 3, 3, 0, 0, 0, 0, 0 }, Level = 12 },
        //    new() { Slots = new List<int> { 3, 3, 3, 3, 3, 0, 0, 0, 0, 0 }, Level = 13 },
        //    new() { Slots = new List<int> { 3, 3, 3, 3, 3, 0, 0, 0, 0, 0 }, Level = 14 },
        //    new() { Slots = new List<int> { 3, 3, 3, 3, 3, 0, 0, 0, 0, 0 }, Level = 15 },
        //    new() { Slots = new List<int> { 3, 3, 3, 3, 3, 0, 0, 0, 0, 0 }, Level = 16 },
        //    new() { Slots = new List<int> { 4, 4, 4, 4, 4, 0, 0, 0, 0, 0 }, Level = 17 },
        //    new() { Slots = new List<int> { 4, 4, 4, 4, 4, 0, 0, 0, 0, 0 }, Level = 18 },
        //    new() { Slots = new List<int> { 4, 4, 4, 4, 4, 0, 0, 0, 0, 0 }, Level = 19 },
        //    new() { Slots = new List<int> { 4, 4, 4, 4, 4, 0, 0, 0, 0, 0 }, Level = 20 },
        //};

        internal static readonly List<SlotsByLevelDuplet> FullCastingSlots = new()
        {
            new() { Slots = new List<int> { 2, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, Level = 01 },
            new() { Slots = new List<int> { 3, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, Level = 02 },
            new() { Slots = new List<int> { 4, 2, 0, 0, 0, 0, 0, 0, 0, 0 }, Level = 03 },
            new() { Slots = new List<int> { 4, 3, 0, 0, 0, 0, 0, 0, 0, 0 }, Level = 04 },
            new() { Slots = new List<int> { 4, 3, 2, 0, 0, 0, 0, 0, 0, 0 }, Level = 05 },
            new() { Slots = new List<int> { 4, 3, 3, 0, 0, 0, 0, 0, 0, 0 }, Level = 06 },
            new() { Slots = new List<int> { 4, 3, 3, 1, 0, 0, 0, 0, 0, 0 }, Level = 07 },
            new() { Slots = new List<int> { 4, 3, 3, 2, 0, 0, 0, 0, 0, 0 }, Level = 08 },
            new() { Slots = new List<int> { 4, 3, 3, 3, 1, 0, 0, 0, 0, 0 }, Level = 09 },
            new() { Slots = new List<int> { 4, 3, 3, 3, 2, 0, 0, 0, 0, 0 }, Level = 10 },
            new() { Slots = new List<int> { 4, 3, 3, 3, 2, 1, 0, 0, 0, 0 }, Level = 11 },
            new() { Slots = new List<int> { 4, 3, 3, 3, 2, 1, 0, 0, 0, 0 }, Level = 12 },
            new() { Slots = new List<int> { 4, 3, 3, 3, 2, 1, 1, 0, 0, 0 }, Level = 13 },
            new() { Slots = new List<int> { 4, 3, 3, 3, 2, 1, 1, 0, 0, 0 }, Level = 14 },
            new() { Slots = new List<int> { 4, 3, 3, 3, 2, 1, 1, 1, 0, 0 }, Level = 15 },
            new() { Slots = new List<int> { 4, 3, 3, 3, 2, 1, 1, 1, 0, 0 }, Level = 16 },
            new() { Slots = new List<int> { 4, 3, 3, 3, 2, 1, 1, 1, 1, 0 }, Level = 17 },
            new() { Slots = new List<int> { 4, 3, 3, 3, 3, 1, 1, 1, 1, 0 }, Level = 18 },
            new() { Slots = new List<int> { 4, 3, 3, 3, 3, 2, 1, 1, 1, 0 }, Level = 19 },
            new() { Slots = new List<int> { 4, 3, 3, 3, 3, 2, 2, 1, 1, 0 }, Level = 20 },
        };
    }
}
