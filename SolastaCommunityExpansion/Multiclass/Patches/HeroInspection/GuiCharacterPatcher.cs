using HarmonyLib;

namespace SolastaCommunityExpansion.Multiclass.Patches.HeroInspection
{
    internal static class GuiCharacterPatcher
    {
        [HarmonyPatch(typeof(GuiCharacter), "MainClassDefinition", MethodType.Getter)]
        internal static class GuiCharacterMainClassDefinition
        {
            internal static void Postfix(ref CharacterClassDefinition __result)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return;
                }

                // NOTE: don't use SelectedClass??. which bypasses Unity object lifetime check
                if (Models.InspectionPanelContext.SelectedClass)
                {
                    __result = Models.InspectionPanelContext.SelectedClass;
                }
            }
        }

        [HarmonyPatch(typeof(GuiCharacter), "LevelAndClassAndSubclass", MethodType.Getter)]
        internal static class GuiCharacterLevelAndClassAndSubclass
        {
            internal static void Postfix(GuiCharacter __instance, ref string __result)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return;
                }

                __result = Models.MulticlassGameUiContext.GetAllClassesLabel(__instance, '-') ?? __result;
            }
        }

        [HarmonyPatch(typeof(GuiCharacter), "ClassAndLevel", MethodType.Getter)]
        internal static class GuiCharacterClassAndLevel
        {
            internal static void Postfix(GuiCharacter __instance, ref string __result)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return;
                }

                __result = Models.MulticlassGameUiContext.GetAllClassesLabel(__instance, '-') ?? __result;
            }
        }

        //
        // this patch shouldn't be protected as we overwrite the experience tooltip term in translations-en.txt with 4 params vs. game official 3
        //
        [HarmonyPatch(typeof(GuiCharacter), "LevelAndExperienceTooltip", MethodType.Getter)]
        internal static class GuiCharacterLevelAndExperienceTooltip
        {
            internal static void Postfix(GuiCharacter __instance, ref string __result)
            {
                __result = Models.MulticlassGameUiContext.GetLevelAndExperienceTooltip(__instance) ?? __result;
            }
        }
    }
}
