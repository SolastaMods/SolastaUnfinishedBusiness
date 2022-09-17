using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches.LevelUp;

//PATCH: ensures auto prepared spells from subclass are considered on level up
[HarmonyPatch(typeof(RulesetCharacter), "ComputeAutopreparedSpells")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class RulesetCharacter_ComputeAutopreparedSpells
{
    internal static bool Prefix([NotNull] RulesetCharacter __instance, [NotNull] RulesetSpellRepertoire spellRepertoire)
    {
        //BEGIN PATCH
        var spellcastingClass = spellRepertoire.SpellCastingClass;

        if (spellcastingClass == null && spellRepertoire.SpellCastingSubclass != null)
        {
            spellcastingClass = LevelUpContext.GetClassForSubclass(spellRepertoire.SpellCastingSubclass);
        }
        //END PATCH

        // this includes all the logic for the base function
        spellRepertoire.AutoPreparedSpells.Clear();
        __instance.EnumerateFeaturesToBrowse<FeatureDefinitionAutoPreparedSpells>(__instance.FeaturesToBrowse);

        foreach (var featureDefinition in __instance.FeaturesToBrowse)
        {
            var autoPreparedSpells = featureDefinition as FeatureDefinitionAutoPreparedSpells;

            if (autoPreparedSpells!.SpellcastingClass != spellcastingClass)
            {
                continue;
            }

            var classLevel = SharedSpellsContext.GetClassSpellLevel(spellRepertoire);

            foreach (var preparedSpellsGroup in autoPreparedSpells.AutoPreparedSpellsGroups
                         .Where(preparedSpellsGroup => preparedSpellsGroup.ClassLevel <= classLevel))
            {
                spellRepertoire.AutoPreparedSpells.AddRange(preparedSpellsGroup.SpellsList);
                spellRepertoire.AutoPreparedTag = autoPreparedSpells.AutoPreparedTag;
            }
        }

        return false;
    }

    // private static int GetSpellcastingLevel(
    //     [NotNull] RulesetEntity character,
    //     RulesetSpellRepertoire spellRepertoire)
    // {
    //     if (character is not RulesetCharacterHero hero)
    //     {
    //         return character.GetAttribute(AttributeDefinitions.CharacterLevel).BaseValue;
    //     }
    //
    //     if (spellRepertoire.SpellCastingClass != null)
    //     {
    //         return hero.ClassesAndLevels[spellRepertoire.SpellCastingClass];
    //     }
    //
    //     return spellRepertoire.SpellCastingSubclass != null
    //         ? hero.ComputeSubclassLevel(spellRepertoire.SpellCastingSubclass)
    //         : character.GetAttribute(AttributeDefinitions.CharacterLevel).BaseValue;
    // }
}
