using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.GameUi.CharacterInspection
{
    [HarmonyPatch(typeof(GuiCharacter), "MainClassDefinition", MethodType.Getter)]
    internal static class GuiCharacterMainClassDefinitionGetter
    {
        internal static void Postfix(ref CharacterClassDefinition __result)
        {
            if (!Main.Settings.EnableEnhancedCharacterInspection)
            {
                return;
            }

            // NOTE: don't use SelectedClass??. which bypasses Unity object lifetime check
            if (InspectionPanelContext.SelectedClass)
            {
                __result = InspectionPanelContext.SelectedClass;
            }
        }
    }
}
