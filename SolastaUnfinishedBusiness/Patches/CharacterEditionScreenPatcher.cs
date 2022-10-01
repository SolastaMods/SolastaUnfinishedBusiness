using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

internal static class CharacterEditionScreenPatcher
{
    [HarmonyPatch(typeof(CharacterEditionScreen), "LoadStagePanels")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class LoadStagePanels_Patch
    {
        internal static void Postfix(CharacterEditionScreen __instance)
        {
            //PATCH: Patch to support custom feature selection screen
            //adds new stage panel based on Spell Selection stage
            CustomInvocationSelectionPanel.InsertPanel(__instance);

            //PATCH: adds the Multiclass class selection panel to the level up screen (MULTICLASS)
            MulticlassGameUiContext.SetupLevelUpClassSelectionStep(__instance);
        }
    }

    [HarmonyPatch(typeof(CharacterEditionScreen), "DoAbort")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class DoAbort_Patch
    {
        internal static void Prefix([NotNull] CharacterEditionScreen __instance)
        {
            //PATCH: Unregisters hero from level up context (MULTICLASS)
            LevelUpContext.UnregisterHero(__instance.currentHero);
        }
    }
}
