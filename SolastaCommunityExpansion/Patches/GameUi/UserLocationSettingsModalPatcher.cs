using HarmonyLib;
using TMPro;

namespace SolastaCommunityExpansion.Patches
{
    // allows extra characters on location names
    [HarmonyPatch(typeof(UserLocationSettingsModal), "RemoveUselessSpaces")]
    internal static class UserLocationSettingsModalRemoveUselessSpaces
    {
        public static bool Prefix(TMP_InputField textField)
        {
            if (!Main.Settings.AllowExtraKeyboardCharactersInNames)
            {
                return true;
            }

            return Models.GameUiContext.RemoveInvalidFilenameChars.Invoke(textField);
        }
    }
}
