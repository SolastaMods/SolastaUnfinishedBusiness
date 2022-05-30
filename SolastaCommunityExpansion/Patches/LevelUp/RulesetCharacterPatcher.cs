using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.LevelUp;

[HarmonyPatch(typeof(RulesetCharacter), "ComputeAutopreparedSpells")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class RulesetCharacter_ComputeAutopreparedSpells
{
    internal static bool Prefix(RulesetCharacter __instance, RulesetSpellRepertoire spellRepertoire)
    {
        if (!Main.Settings.SupportAutoPreparedSpellsOnSubclassCasters)
        {
            return true;
        }

        var spellcastingClass = spellRepertoire.SpellCastingClass;
        if (spellRepertoire.SpellCastingSubclass != null)
        {
            spellcastingClass = GetClassForSubclass(spellRepertoire.SpellCastingSubclass);
        }

        spellRepertoire.AutoPreparedSpells.Clear();
        __instance.EnumerateFeaturesToBrowse<FeatureDefinitionAutoPreparedSpells>(__instance.FeaturesToBrowse);
        foreach (var featureDefinition in __instance.FeaturesToBrowse)
        {
            var autoPreparedSpells = featureDefinition as FeatureDefinitionAutoPreparedSpells;
            if (autoPreparedSpells.SpellcastingClass == spellcastingClass)
            {
                foreach (var preparedSpellsGroup in autoPreparedSpells.AutoPreparedSpellsGroups)
                {
                    if (preparedSpellsGroup.ClassLevel <= GetSpellcastingLevel(__instance, spellRepertoire))
                    {
                        spellRepertoire.AutoPreparedSpells.AddRange(preparedSpellsGroup.SpellsList);
                        spellRepertoire.AutoPreparedTag = autoPreparedSpells.AutoPreparedTag;
                    }
                }
            }
        }

        // This includes all the logic for the base function and a little extra, so skip it.
        return false;
    }

    private static int GetSpellcastingLevel(RulesetCharacter character, RulesetSpellRepertoire spellRepertoire)
    {
        if (character is RulesetCharacterHero hero)
        {
            if (spellRepertoire.SpellCastingClass != null)
            {
                return hero.ClassesAndLevels[spellRepertoire.SpellCastingClass];
            }

            if (spellRepertoire.SpellCastingSubclass != null)
            {
                return hero.ComputeSubclassLevel(spellRepertoire.SpellCastingSubclass);
            }
        }

        return character.GetAttribute(AttributeDefinitions.CharacterLevel).BaseValue;
    }

    private static CharacterClassDefinition GetClassForSubclass(CharacterSubclassDefinition subclass)
    {
        return DatabaseRepository.GetDatabase<CharacterClassDefinition>().FirstOrDefault(klass =>
        {
            return klass.FeatureUnlocks.Any(unlock =>
            {
                if (unlock.FeatureDefinition is FeatureDefinitionSubclassChoice subclassChoice)
                {
                    return subclassChoice.Subclasses.Contains(subclass.Name);
                }

                return false;
            });
        });
    }
}
