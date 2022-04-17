using System;
using System.Collections.Generic;
using System.Linq;

namespace SolastaMulticlass.Models
{
    internal static class CacheSpellsContext
    {
        private static readonly Dictionary<CharacterClassDefinition, Dictionary<int, HashSet<SpellDefinition>>> classSpellList = new();
        private static readonly Dictionary<CharacterSubclassDefinition, Dictionary<int, HashSet<SpellDefinition>>> subclassSpellList = new();

        private static int GetLowestCasterLevelFromSpellLevel(BaseDefinition definition, int spellLevel, bool isSubclass = false)
        {
            CasterType casterType;

            if (isSubclass && definition is CharacterSubclassDefinition characterSubclassDefinition)
            {
                if (!SharedSpellsContext.SubclassCasterType.TryGetValue(characterSubclassDefinition, out casterType))
                {
                    return 0;
                }
            }
            else if (definition is CharacterClassDefinition characterClassDefinition)
            {
                if (!SharedSpellsContext.ClassCasterType.TryGetValue(characterClassDefinition, out casterType))
                {
                    return 0;
                }
            }
            else
            {
                casterType = CasterType.None;
            }

            var modifier = (int)casterType - (int)casterType % 2;
            var classLevel = Math.Abs(((spellLevel - 1) * modifier) + (spellLevel > 1 ? 1 : modifier / 2));

            return classLevel;
        }

        private static void RegisterSpell(BaseDefinition definition, int level, List<SpellDefinition> spellList, bool isSubclass = false)
        {
            if (spellList == null)
            {
                return;
            }

            if (isSubclass && definition is CharacterSubclassDefinition characterSubclassDefinition)
            {
                subclassSpellList.TryAdd(characterSubclassDefinition, new());
                subclassSpellList[characterSubclassDefinition].TryAdd(level, new());

                foreach (var spell in spellList)
                {
                    if (!subclassSpellList[characterSubclassDefinition][level].Contains(spell))
                    {
                        subclassSpellList[characterSubclassDefinition][level].Add(spell);
                    }
                }
            }
            else if (definition is CharacterClassDefinition characterClassDefinition)
            {
                classSpellList.TryAdd(characterClassDefinition, new());
                classSpellList[characterClassDefinition].TryAdd(level, new());

                foreach (var spell in spellList)
                {
                    if (!classSpellList[characterClassDefinition][level].Contains(spell))
                    {
                        classSpellList[characterClassDefinition][level].Add(spell);
                    }
                }
            }
        }

        private static void EnumerateSpells(BaseDefinition definition, List<FeatureUnlockByLevel> featureUnlocks, bool isSubClass = false)
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
                    var level = GetLowestCasterLevelFromSpellLevel(definition, spellListDefinition.SpellsByLevel[i].Level, true);
                    var spellList = spellListDefinition.SpellsByLevel[i].Spells;

                    RegisterSpell(definition, level, spellList, isSubClass);
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

                        RegisterSpell(definition, level, spellList, isSubClass);
                    }
                }
                else if (featureDefinition is FeatureDefinitionBonusCantrips featureDefinitionBonusCantrips)
                {
                    var level = featureUnlock.Level;
                    var spellList = featureDefinitionBonusCantrips.BonusCantrips;

                    RegisterSpell(definition, level, spellList, isSubClass);
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
                var featureUnlocks = characterClassDefinition.FeatureUnlocks;

                EnumerateSpells(characterClassDefinition, featureUnlocks, false);
            }

            foreach (var characterSubclassDefinition in DatabaseRepository.GetDatabase<CharacterSubclassDefinition>())
            {
                var featureUnlocks = characterSubclassDefinition.FeatureUnlocks;

                EnumerateSpells(characterSubclassDefinition, featureUnlocks, true);
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

            if (selectedClass != null && classSpellList.ContainsKey(selectedClass))
            {
                foreach (var levelSpell in classSpellList[selectedClass]
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

            if (selectedSubclass != null && subclassSpellList.ContainsKey(selectedSubclass))
            {
                foreach (var levelSpell in subclassSpellList[selectedSubclass]
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
