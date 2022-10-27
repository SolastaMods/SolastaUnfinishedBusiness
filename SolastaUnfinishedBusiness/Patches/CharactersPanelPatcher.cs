using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

public static class CharactersPanelPatcher
{
    //PATCH: Keeps last level up hero selected
    [HarmonyPatch(typeof(CharactersPanel), "OnBeginShow")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class OnBeginShow_Patch
    {
        public static void Prefix(CharactersPanel __instance, out float __state)
        {
            // Remember the current scroll position
            __state = __instance.charactersScrollview.verticalNormalizedPosition;
        }

        public static void Postfix(CharactersPanel __instance, float __state)
        {
            if (Global.LastLevelUpHeroName == null)
            {
                return;
            }

            __instance.OnSelectPlate(
                __instance.characterPlates.Find(x => x.GuiCharacter.Name == Global.LastLevelUpHeroName));

            Global.LastLevelUpHeroName = null;

            Main.Log($"setting position to {__state}");

            // Reset the scroll position because OnBeginShow sets it to 1.0f.
            // This keeps the selected character in view unless the panel is sorted by level
            // in which case the character will move.

            __instance.charactersScrollview.verticalNormalizedPosition = __state;
        }
    }
}
