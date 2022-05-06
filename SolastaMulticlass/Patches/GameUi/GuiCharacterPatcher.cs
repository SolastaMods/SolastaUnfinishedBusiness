using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaMulticlass.Patches.GameUi
{
    internal static class GuiCharacterPatcher
    {
        [HarmonyPatch(typeof(GuiCharacter), "LevelAndClassAndSubclass", MethodType.Getter)]
        internal static class GuiCharacterLevelAndClassAndSubclassGetter
        {
            internal static void Postfix(GuiCharacter __instance, ref string __result)
            {
                __result = MulticlassGameUiContext.GetAllClassesLabel(__instance, '-') ?? __result;
            }
        }

        [HarmonyPatch(typeof(GuiCharacter), "ClassAndLevel", MethodType.Getter)]
        internal static class GuiCharacterClassAndLevelGetter
        {
            internal static void Postfix(GuiCharacter __instance, ref string __result)
            {
                __result = MulticlassGameUiContext.GetAllClassesLabel(__instance, '-') ?? __result;
            }
        }

        // shouldn't be protected as we touch translation terms and this will abort otherwise
        [HarmonyPatch(typeof(GuiCharacter), "LevelAndExperienceTooltip", MethodType.Getter)]
        internal static class GuiCharacterLevelAndExperienceTooltipGetter
        {
            internal static void Postfix(GuiCharacter __instance, ref string __result)
            {
                __result = MulticlassGameUiContext.GetLevelAndExperienceTooltip(__instance) ?? __result;
            }
        }
    }
}
