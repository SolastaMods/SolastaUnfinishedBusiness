using System;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

public static class CharacterStageSubclassSelectionPanelPatcher
{
    [HarmonyPatch(typeof(CharacterStageSubclassSelectionPanel), "OnBeginShow")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class OnBeginShow_Patch
    {
        public static void Prefix([NotNull] CharacterStageSubclassSelectionPanel __instance)
        {
            //PATCH: changes the subclasses layout to allow more offering
            LevelUpContext.ApplyCommonSelectionLayout(__instance.subclassesTable);

            //PATCH: sorts the sub classes panel by Title
            if (!Main.Settings.EnableSortingSubclasses)
            {
                return;
            }

            __instance.compatibleSubclasses
                .Sort((a, b) =>
                    String.Compare(a.FormatTitle(), b.FormatTitle(), StringComparison.CurrentCultureIgnoreCase));
        }
    }

    [HarmonyPatch(typeof(CharacterStageSubclassSelectionPanel), "UpdateRelevance")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class UpdateRelevance_Patch
    {
        public static void Postfix([NotNull] CharacterStageSubclassSelectionPanel __instance)
        {
            //PATCH: updates this panel relevance (MULTICLASS)
            if (LevelUpContext.IsLevelingUp(__instance.currentHero)
                && LevelUpContext.RequiresDeity(__instance.currentHero))
            {
                __instance.isRelevant = false;
            }
        }
    }
}
