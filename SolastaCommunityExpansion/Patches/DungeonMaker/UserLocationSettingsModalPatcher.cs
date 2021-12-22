using HarmonyLib;
using TMPro;

namespace SolastaCommunityExpansion.Patches.DungeonMaker
{
    // allows extra characters on location names
    [HarmonyPatch(typeof(UserLocationSettingsModal), "RemoveUselessSpaces")]
    internal static class UserLocationSettingsModalRemoveUselessSpaces
    {
        public static bool Prefix(TMP_InputField textField)
        {
            if (!Main.Settings.AllowExtraKeyboardCharactersInLocationNames)
            {
                return true;
            }

            return Models.GameUiContext.RemoveInvalidFilenameChars.Invoke(textField);
        }
    }
}
