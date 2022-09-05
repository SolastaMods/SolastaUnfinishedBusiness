using System;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.LevelUp;

//PATCH: sorts the deity panel by Title
[HarmonyPatch(typeof(CharacterStageDeitySelectionPanel), "Compare", typeof(DeityDefinition),
    typeof(DeityDefinition))]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class CharacterStageDeitySelectionPanel_Compare_DeityDefinition
{
    internal static void Postfix(DeityDefinition left, DeityDefinition right, ref int __result)
    {
        if (Main.Settings.EnableSortingDeities)
        {
            __result = String.Compare(left.FormatTitle(), right.FormatTitle(),
                StringComparison.CurrentCultureIgnoreCase);
        }
    }
}

//PATCH: sorts the deity panel by Title
[HarmonyPatch(typeof(CharacterStageDeitySelectionPanel), "Compare", typeof(CharacterSubclassDefinition),
    typeof(CharacterSubclassDefinition))]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class CharacterStageDeitySelectionPanel_Compare_CharacterSubclassDefinition
{
    internal static void Postfix(
        CharacterSubclassDefinition left,
        CharacterSubclassDefinition right,
        ref int __result)
    {
        if (Main.Settings.EnableSortingDeities)
        {
            __result = String.Compare(left.FormatTitle(), right.FormatTitle(),
                StringComparison.CurrentCultureIgnoreCase);
        }
    }
}

//PATCH: updates this panel relevance (MULTICLASS)
[HarmonyPatch(typeof(CharacterStageDeitySelectionPanel), "UpdateRelevance")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class CharacterStageDeitySelectionPanel_UpdateRelevance
{
    internal static void Postfix([NotNull] CharacterStageDeitySelectionPanel __instance)
    {
        if (LevelUpContext.IsLevelingUp(__instance.currentHero))
        {
            __instance.isRelevant = LevelUpContext.RequiresDeity(__instance.currentHero);
        }
    }
}
