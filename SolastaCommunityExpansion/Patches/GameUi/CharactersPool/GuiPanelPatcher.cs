using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.GameUi.CharactersPool;

//
// this patch is protected by HeroName
//
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

        var charactersPanel =
            AccessTools.Field(mainMenuScreen.GetType(), "charactersPanel").GetValue(mainMenuScreen) as
                CharactersPanel;

        charactersPanel.Show();
    }
}
