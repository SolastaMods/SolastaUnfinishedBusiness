using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using UnityEngine.UI;

namespace SolastaCommunityExpansion.Patches.GameUi.CharactersPool
{
    //
    // this patch shouldn't be protected
    //
    [HarmonyPatch(typeof(CharactersPanel), "OnBeginShow")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharactersPanel_OnBeginShow
    {
        internal static void Prefix(ScrollRect ___charactersScrollview, out float __state)
        {
            // Remember the current scroll position
            __state = ___charactersScrollview.verticalNormalizedPosition;
        }

        internal static void Postfix(CharactersPanel __instance,
            ScrollRect ___charactersScrollview, List<CharacterPlateToggle> ___characterPlates, float __state)
        {
            if (CharacterPoolManagerPatcher.HeroName != null)
            {
                __instance.OnSelectPlate(___characterPlates.Find(x => x.GuiCharacter.Name == CharacterPoolManagerPatcher.HeroName));

                CharacterPoolManagerPatcher.HeroName = null;

                Main.Log($"setting position to {__state}");

                // Reset the scroll position because OnBeginShow sets it to 1.0f.
                // This keeps the selected character in view unless the panel is sorted by level
                // in which case the character will move.
                // TODO: calculate the required character position.
                ___charactersScrollview.verticalNormalizedPosition = __state;
            }
        }
    }
}
