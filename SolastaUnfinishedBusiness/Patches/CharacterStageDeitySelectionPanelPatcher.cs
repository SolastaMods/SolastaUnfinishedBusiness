using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

public static class CharacterStageDeitySelectionPanelPatcher
{
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
