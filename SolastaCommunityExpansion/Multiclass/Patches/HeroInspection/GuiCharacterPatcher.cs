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

                __result = Models.InspectionPanelContext.SelectedClass ?? __result;
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

                __result = Models.GameUiContext.GetAllClassesLabel(__instance, " - ") ?? __result;
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

                __result = Models.GameUiContext.GetAllClassesLabel(__instance, " - ") ?? __result;
            }
        }

        [HarmonyPatch(typeof(GuiCharacter), "LevelAndExperienceTooltip", MethodType.Getter)]
        internal static class GuiCharacterLevelAndExperienceTooltip
        {
            internal static void Postfix(GuiCharacter __instance, ref string __result)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return;
                }

                __result = Models.GameUiContext.GetLevelAndExperienceTooltip(__instance) ?? __result;
            }
        }
    }
}
