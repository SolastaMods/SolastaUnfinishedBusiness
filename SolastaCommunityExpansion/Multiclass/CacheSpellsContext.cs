using System;
using System.Collections.Generic;

namespace SolastaMulticlass.Models
{
    public static class CacheSpellsContext
    {
        public static readonly Dictionary<CharacterClassDefinition, Dictionary<int, HashSet<SpellDefinition>>> ClassSpellList = new();
        public static readonly Dictionary<CharacterSubclassDefinition, Dictionary<int, HashSet<SpellDefinition>>> SubclassSpellList = new();

        private static int GetLowestCasterLevelFromSpellLevel(BaseDefinition definition, int spellLevel)
        {
            CasterType casterType = CasterType.None;

            if (definition is CharacterClassDefinition characterClassDefinition)
            {
                SharedSpellsContext.ClassCasterType.TryGetValue(characterClassDefinition, out casterType);
            }
            else if (definition is CharacterSubclassDefinition characterSubclassDefinition)
            {
                SharedSpellsContext.SubclassCasterType.TryGetValue(characterSubclassDefinition, out casterType);
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
                SubclassSpellList.TryAdd(characterSubclassDefinition, new());
                SubclassSpellList[characterSubclassDefinition].TryAdd(level, new());

                foreach (var spell in spellList)
                {
                    if (!SubclassSpellList[characterSubclassDefinition][level].Contains(spell))
                    {
                        SubclassSpellList[characterSubclassDefinition][level].Add(spell);
                    }
                }
            }
            else if (definition is CharacterClassDefinition characterClassDefinition)
            {
                ClassSpellList.TryAdd(characterClassDefinition, new());
                ClassSpellList[characterClassDefinition].TryAdd(level, new());

                foreach (var spell in spellList)
                {
                    if (!ClassSpellList[characterClassDefinition][level].Contains(spell))
                    {
                        ClassSpellList[characterClassDefinition][level].Add(spell);
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
                    var level = GetLowestCasterLevelFromSpellLevel(definition, spellListDefinition.SpellsByLevel[i].Level);
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
            }
        }

        public static void Load()
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
    }
}
