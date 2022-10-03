using System;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

public static class CharacterStageDeitySelectionPanelPatcher
{
    [HarmonyPatch(typeof(CharacterStageDeitySelectionPanel), "Compare", typeof(DeityDefinition),
        typeof(DeityDefinition))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class Compare_Patch
    {
        public static void Postfix(DeityDefinition left, DeityDefinition right, ref int __result)
        {
            //PATCH: sorts the deity panel by Title
            if (Main.Settings.EnableSortingDeities)
            {
                __result = String.Compare(left.FormatTitle(), right.FormatTitle(),
                    StringComparison.CurrentCultureIgnoreCase);
            }
        }
    }

    [HarmonyPatch(typeof(CharacterStageDeitySelectionPanel), "Compare", typeof(CharacterSubclassDefinition),
        typeof(CharacterSubclassDefinition))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class Compare_Patch_2
    {
        public static void Postfix(
            CharacterSubclassDefinition left,
            CharacterSubclassDefinition right,
            ref int __result)
        {
            //PATCH: sorts the deity panel by Title
            if (Main.Settings.EnableSortingDeities)
            {
                __result = String.Compare(left.FormatTitle(), right.FormatTitle(),
                    StringComparison.CurrentCultureIgnoreCase);
            }
        }
    }

    [HarmonyPatch(typeof(CharacterStageDeitySelectionPanel), "UpdateRelevance")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class UpdateRelevance_Patch
    {
        public static void Postfix([NotNull] CharacterStageDeitySelectionPanel __instance)
        {
            //PATCH: updates this panel relevance (MULTICLASS)
            if (LevelUpContext.IsLevelingUp(__instance.currentHero))
            {
                __instance.isRelevant = LevelUpContext.RequiresDeity(__instance.currentHero);
            }
        }
    }
}
