using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;

namespace SolastaCommunityExpansion.Multiclass.Models
{
    internal static class CacheSpellsContext
    {
        public static readonly Dictionary<string, Dictionary<int, List<SpellDefinition>>> classSpellList = new();
        public static readonly Dictionary<string, Dictionary<int, List<SpellDefinition>>> subclassSpellList = new();

        private static int GetLowestCasterLevelFromSpellLevel(string name, int spellLevel, bool isSubclass = false)
        {
            CasterType casterType;

            if (isSubclass)
            {
                if (!SharedSpellsContext.SubclassCasterType.ContainsKey(name))
                {
                    return 0;
                }

                casterType = SharedSpellsContext.SubclassCasterType[name];
            }
            else
            {
                if (!SharedSpellsContext.ClassCasterType.ContainsKey(name))
                {
                    return 0;
                }

                casterType = SharedSpellsContext.ClassCasterType[name];
            }

            var modifier = casterType switch
            {
                CasterType.Full => 2,
                CasterType.Half or CasterType.HalfRoundUp => 4,
                CasterType.OneThird => 6,
                _ => 0,
            };

            var classLevel = Math.Abs(((spellLevel - 1) * modifier) + (spellLevel > 1 ? 1 : modifier / 2));

            return classLevel;
        }

        private static void RegisterSpell(string name, int level, List<SpellDefinition> spellList, bool isSubclass = false)
        {
            if (spellList != null)
            {
                var record = isSubclass ? subclassSpellList : classSpellList;

                if (!record.ContainsKey(name))
                {
                    record.Add(name, new Dictionary<int, List<SpellDefinition>>());
                }

                if (!record[name].ContainsKey(level))
                {
                    record[name].Add(level, new List<SpellDefinition>());
                }

                foreach (var spell in spellList)
                {
                    if (!record[name][level].Contains(spell))
                    {
                        record[name][level].Add(spell);
                    }
                }
            }
        }

        private static void EnumerateSpells(string name, List<FeatureUnlockByLevel> featureUnlocks, bool isSubClass = false)
        {
            void Register(SpellListDefinition spellListDefinition)
            {
                if (spellListDefinition == null)
                {
                    return;
                }

                var maxLevel = spellListDefinition.MaxSpellLevel;

                for (var i = 0; i < maxLevel; i++)
                {
                    var level = GetLowestCasterLevelFromSpellLevel(name, spellListDefinition.SpellsByLevel[i].Level, true);
                    var spellList = spellListDefinition.SpellsByLevel[i].Spells;

                    RegisterSpell(name, level, spellList, isSubClass);
                }
            }

            foreach (var featureUnlock in featureUnlocks)
            {
                var featureDefinition = featureUnlock.FeatureDefinition;
                var featureDefinitionTypeName = featureDefinition.GetType().Name;

                if (featureDefinition is FeatureDefinitionCastSpell featureDefinitionCastSpell)
                {
                    var spellListDefinition = featureDefinitionCastSpell.SpellListDefinition;

                    Register(spellListDefinition);
                }
                else if (featureDefinition is FeatureDefinitionMagicAffinity featureDefinitionMagicAffinity)
                {
                    var spellListDefinition = featureDefinitionMagicAffinity.ExtendedSpellList;

                    Register(spellListDefinition);
                }
                else if (featureDefinition is FeatureDefinitionAutoPreparedSpells featureDefinitionAutoPreparedSpells)
                {
                    foreach (var autoPreparedSpellsGroup in featureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroups)
                    {
                        var level = autoPreparedSpellsGroup.ClassLevel;
                        var spellList = autoPreparedSpellsGroup.SpellsList;

                        RegisterSpell(name, level, spellList, isSubClass);
                    }
                }
                else if (featureDefinition is FeatureDefinitionBonusCantrips featureDefinitionBonusCantrips)
                {
                    var level = featureUnlock.Level;
                    var spellList = featureDefinitionBonusCantrips.BonusCantrips;

                    RegisterSpell(name, level, spellList, isSubClass);
                }
                // unofficial
                else if (featureDefinitionTypeName == "FeatureDefinitionExtraSpellSelection")
                {
                    var spellListDefinition = AccessTools.Field(featureDefinition.GetType(), "spell_list").GetValue(featureDefinition) as SpellListDefinition;

                    Register(spellListDefinition);
                }
            }
        }

        internal static void Load()
        {
            foreach (var characterClassDefinition in DatabaseRepository.GetDatabase<CharacterClassDefinition>())
            {
                var className = characterClassDefinition.Name;
                var featureUnlocks = characterClassDefinition?.FeatureUnlocks;

                EnumerateSpells(className, featureUnlocks, false);
            }

            foreach (var characterSubclassDefinition in DatabaseRepository.GetDatabase<CharacterSubclassDefinition>())
            {
                var subclassName = characterSubclassDefinition.Name;
                var featureUnlocks = characterSubclassDefinition?.FeatureUnlocks;

                EnumerateSpells(subclassName, featureUnlocks, true);
            }
        }

        internal static bool IsRepertoireFromSelectedClassSubclass(RulesetSpellRepertoire rulesetSpellRepertoire)
        {
            var selectedClass = LevelUpContext.SelectedClass;
            var selectedSubclass = LevelUpContext.SelectedSubclass;

            return
                (rulesetSpellRepertoire.SpellCastingFeature.SpellCastingOrigin == FeatureDefinitionCastSpell.CastingOrigin.Class && rulesetSpellRepertoire.SpellCastingClass == selectedClass) ||
                (rulesetSpellRepertoire.SpellCastingFeature.SpellCastingOrigin == FeatureDefinitionCastSpell.CastingOrigin.Subclass && rulesetSpellRepertoire.SpellCastingSubclass == selectedSubclass);
        }

        internal static bool IsSpellKnownBySelectedClassSubclass(SpellDefinition spellDefinition)
        {
            var selectedHero = LevelUpContext.SelectedHero;
            var selectedClass = LevelUpContext.SelectedClass;
            var selectedSubclass = LevelUpContext.SelectedSubclass;
            var spellRepertoire = selectedHero.SpellRepertoires.Find(sr =>
                (sr.SpellCastingFeature.SpellCastingOrigin == FeatureDefinitionCastSpell.CastingOrigin.Class && sr.SpellCastingClass == selectedClass) ||
                (sr.SpellCastingFeature.SpellCastingOrigin == FeatureDefinitionCastSpell.CastingOrigin.Subclass && sr.SpellCastingSubclass == selectedSubclass));

            return spellRepertoire?.HasKnowledgeOfSpell(spellDefinition) == true;
        }

        internal static bool IsSpellOfferedBySelectedClassSubclass(SpellDefinition spellDefinition, bool onlyCurrentLevel = false)
        {
            var classLevel = LevelUpContext.GetClassLevel();
            var className = LevelUpContext.SelectedClass?.Name;
            var subClassName = LevelUpContext.SelectedSubclass?.Name;

            if (className != null && classSpellList.ContainsKey(className))
            {
                foreach (var levelSpell in classSpellList[className]
                    .Where(x => x.Key <= classLevel))
                {
                    if (levelSpell.Value.Contains(spellDefinition))
                    {
                        return true;
                    }
                    else if (onlyCurrentLevel)
                    {
                        break;
                    }
                }
            }

            if (subClassName != null && subclassSpellList.ContainsKey(subClassName))
            {
                foreach (var levelSpell in subclassSpellList[subClassName]
                    .Where(x => x.Key <= classLevel))
                {
                    if (levelSpell.Value.Contains(spellDefinition))
                    {
                        return true;
                    }
                    else if (onlyCurrentLevel)
                    {
                        break;
                    }
                }
            }

            return false;
        }
    }
}
