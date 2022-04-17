using System;
using System.Collections.Generic;
using System.Linq;

namespace SolastaMulticlass.Models
{
    internal static class CacheSpellsContext
    {
        private static readonly Dictionary<string, Dictionary<int, HashSet<SpellDefinition>>> classSpellList = new();
        private static readonly Dictionary<string, Dictionary<int, HashSet<SpellDefinition>>> subclassSpellList = new();

        private static int GetLowestCasterLevelFromSpellLevel(string name, int spellLevel, bool isSubclass = false)
        {
            CasterType casterType;

            if (isSubclass)
            {
                if (!SharedSpellsContext.SubclassCasterType.TryGetValue(name, out casterType))
                {
                    return 0;
                }
            }
            else
            {
                if (!SharedSpellsContext.ClassCasterType.TryGetValue(name, out casterType))
                {
                    return 0;
                }
            }

            var modifier = (int)casterType - (int)casterType % 2;
            var classLevel = Math.Abs(((spellLevel - 1) * modifier) + (spellLevel > 1 ? 1 : modifier / 2));

            return classLevel;
        }

        private static void RegisterSpell(string name, int level, List<SpellDefinition> spellList, bool isSubclass = false)
        {
            if (spellList != null)
            {
                var record = isSubclass ? subclassSpellList : classSpellList;

                record.TryAdd(name, new());
                record[name].TryAdd(level, new());

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
                //else if (featureDefinitionTypeName == "FeatureDefinitionExtraSpellSelection")
                //{
                //    var spellListDefinition = AccessTools.Field(featureDefinition.GetType(), "spell_list").GetValue(featureDefinition) as SpellListDefinition;

                //    Register(spellListDefinition);
                //}
            }
        }

        internal static void Load()
        {
            foreach (var characterClassDefinition in DatabaseRepository.GetDatabase<CharacterClassDefinition>())
            {
                var className = characterClassDefinition.Name;
                var featureUnlocks = characterClassDefinition.FeatureUnlocks;

                EnumerateSpells(className, featureUnlocks, false);
            }

            foreach (var characterSubclassDefinition in DatabaseRepository.GetDatabase<CharacterSubclassDefinition>())
            {
                var subclassName = characterSubclassDefinition.Name;
                var featureUnlocks = characterSubclassDefinition.FeatureUnlocks;

                EnumerateSpells(subclassName, featureUnlocks, true);
            }
        }

        internal static bool IsRepertoireFromSelectedClassSubclass(RulesetCharacterHero rulesetCharacterHero, RulesetSpellRepertoire rulesetSpellRepertoire)
        {
            var selectedClass = LevelUpContext.GetSelectedClass(rulesetCharacterHero);
            var selectedSubclass = LevelUpContext.GetSelectedSubclass(rulesetCharacterHero);

            return
                (rulesetSpellRepertoire.SpellCastingFeature.SpellCastingOrigin == FeatureDefinitionCastSpell.CastingOrigin.Class
                    && rulesetSpellRepertoire.SpellCastingClass == selectedClass) ||
                (rulesetSpellRepertoire.SpellCastingFeature.SpellCastingOrigin == FeatureDefinitionCastSpell.CastingOrigin.Subclass
                    && rulesetSpellRepertoire.SpellCastingSubclass == selectedSubclass);
        }

        internal static bool IsSpellKnownBySelectedClassSubclass(RulesetCharacterHero rulesetCharacterHero, SpellDefinition spellDefinition)
        {
            var selectedClass = LevelUpContext.GetSelectedClass(rulesetCharacterHero);
            var selectedSubclass = LevelUpContext.GetSelectedSubclass(rulesetCharacterHero);

            var spellRepertoire = rulesetCharacterHero.SpellRepertoires.Find(sr =>
                (sr.SpellCastingFeature.SpellCastingOrigin == FeatureDefinitionCastSpell.CastingOrigin.Class
                    && sr.SpellCastingClass == selectedClass) ||
                (sr.SpellCastingFeature.SpellCastingOrigin == FeatureDefinitionCastSpell.CastingOrigin.Subclass
                    && sr.SpellCastingSubclass == selectedSubclass));

            if (spellRepertoire == null)
            {
                return false;
            }

            return spellRepertoire.HasKnowledgeOfSpell(spellDefinition);
        }

        internal static bool IsSpellOfferedBySelectedClassSubclass(RulesetCharacterHero rulesetCharacterHero, SpellDefinition spellDefinition, bool onlyCurrentLevel = false)
        {
            var classLevel = LevelUpContext.GetSelectedClassLevel(rulesetCharacterHero);
            var selectedClass = LevelUpContext.GetSelectedClass(rulesetCharacterHero);
            var selectedSubclass = LevelUpContext.GetSelectedSubclass(rulesetCharacterHero);

            // NOTE: don't use SelectedClass?. which bypasses Unity object lifetime check
            var className = selectedClass ? selectedClass.Name : null;

            // NOTE: don't use SelectedSubclass?. which bypasses Unity object lifetime check
            var subClassName = selectedSubclass ? selectedSubclass.Name : null;

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
