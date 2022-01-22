using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.GameUi
{
    // allows extra characters on campaign names
    [HarmonyPatch(typeof(Gui), "TrimInvalidCharacterNameSymbols")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GUI_TrimInvalidCharacterNameSymbols
    {
        private static readonly HashSet<char> InvalidFilenameChars = Path.GetInvalidFileNameChars().ToHashSet();

        public static bool Prefix(string originString, ref string __result)
        {
            if (!Main.Settings.AllowExtraKeyboardCharactersInAllNames || originString == null)
            {
                return true;
            }

            // Solasta original code disallows invalid filename chars and an additional list of illegal chars.
            // We're disallowing invalid filename chars only.
            // We're trimming whitespace from start only as per original method.
            // This allows the users to create a name with spaces inside, but also allows trailing space.
            __result = new string(originString
                    .Where(n => !InvalidFilenameChars.Contains(n))
                    .ToArray())
                .TrimStart();

            return false;
        }
    }
}
