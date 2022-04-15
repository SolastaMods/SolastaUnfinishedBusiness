using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using static SolastaModApi.DatabaseHelper.CharacterRaceDefinitions;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.Races
{
    // Allows the Bolgrif race beard shapes to be correctly fetched from the pool
    [HarmonyPatch(typeof(GraphicsCharacterFactoryManager), "CollectBodyPartsToLoadWherePossible_Shape")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GraphicsCharacterFactoryManager_CollectBodyPartsToLoadWherePossible_Shape
    {
        private const string BEARD_SHAPE = "BeardShape";

        internal static void Postfix(GraphicsCharacter graphicsCharacter, string [] ___shapePartsToLoad)
        {
            if (graphicsCharacter.RulesetCharacter is not RulesetCharacterHero rulesetCharacterHero)
            {
                return;
            }

            if (rulesetCharacterHero.RaceDefinition != SolastaCommunityExpansion.Races.BolgrifRaceBuilder.BolgrifRace)
            {
                return;
            }

            for (var i = 0; i < ___shapePartsToLoad.Length; i++)
            {
                if (___shapePartsToLoad[i].Contains(BEARD_SHAPE))
                {
                    ___shapePartsToLoad[i] = ___shapePartsToLoad[i].Replace(Elf.Name, Dwarf.Name);
                }

            }
        }
    }
}
