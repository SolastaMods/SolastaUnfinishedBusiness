using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches;

//PATCH: Keeps last level up hero selected
[HarmonyPatch(typeof(GuiPanel), "Show")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class GuiPanel_Show
{
    internal static void Postfix(GuiPanel __instance)
    {
        if (__instance is not MainMenuScreen mainMenuScreen || Global.LastLevelUpHeroName == null)
        {
            return;
        }

        mainMenuScreen.charactersPanel.Show();
    }
}
