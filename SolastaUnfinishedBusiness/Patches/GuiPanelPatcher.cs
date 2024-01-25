using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.CustomSpecificBehaviors;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class GuiPanelPatcher
{
    [HarmonyPatch(typeof(GuiPanel), nameof(GuiPanel.Show))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Show_Patch
    {
        [UsedImplicitly]
        public static void Postfix(GuiPanel __instance)
        {
            //PATCH: Keeps last level up hero selected
            if (__instance is not MainMenuScreen mainMenuScreen || Global.LastLevelUpHeroName == null)
            {
                return;
            }

            mainMenuScreen.charactersPanel.Show();
        }
    }

    [HarmonyPatch(typeof(GuiPanel), nameof(GuiPanel.OnBeginHide))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class OnEndHide_Patch
    {
        [UsedImplicitly]
        public static void Prefix(GuiPanel __instance, bool instant)
        {
            //PATCH: Power Bundle: hide sub-power selector when some panels start closing
            if (__instance is PowerSelectionPanel or InvocationSelectionPanel)
            {
                PowerBundle.CloseSubPowerSelectionModal(instant);
            }
        }
    }
}
