using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.GameUi.CharactersPool;

//
// this patch shouldn't be protected
//
[HarmonyPatch(typeof(CharactersPanel), "OnBeginShow")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class CharactersPanel_OnBeginShow
{
    internal static void Prefix(CharactersPanel __instance, out float __state)
    {
        // Remember the current scroll position
        __state = __instance.charactersScrollview.verticalNormalizedPosition;
    }

    internal static void Postfix(CharactersPanel __instance, float __state)
    {
        if (Global.LastLevelUpHeroName != null)
        {
            __instance.OnSelectPlate(
                __instance.characterPlates.Find(x => x.GuiCharacter.Name == Global.LastLevelUpHeroName));

            Global.LastLevelUpHeroName = null;

            Main.Log($"setting position to {__state}");

            // Reset the scroll position because OnBeginShow sets it to 1.0f.
            // This keeps the selected character in view unless the panel is sorted by level
            // in which case the character will move.
            // TODO: calculate the required character position.
            __instance.charactersScrollview.verticalNormalizedPosition = __state;
        }
    }
}
