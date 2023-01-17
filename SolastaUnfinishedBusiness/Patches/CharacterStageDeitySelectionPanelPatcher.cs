using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class CharacterStageDeitySelectionPanelPatcher
{
    [HarmonyPatch(typeof(CharacterStageDeitySelectionPanel), nameof(CharacterStageDeitySelectionPanel.UpdateRelevance))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class UpdateRelevance_Patch
    {
        [UsedImplicitly]
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
