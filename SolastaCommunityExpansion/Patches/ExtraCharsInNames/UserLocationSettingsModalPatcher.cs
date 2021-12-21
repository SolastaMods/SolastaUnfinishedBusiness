using HarmonyLib;
using TMPro;

namespace SolastaCommunityExpansion.Patches.ExtraCharsInNames
{
    // allows extra characters on location names
    [HarmonyPatch(typeof(UserLocationSettingsModal), "RemoveUselessSpaces")]
    internal static class UserLocationSettingsModalRemoveUselessSpaces
    {
        public static bool Prefix(TMP_InputField textField)
        {
            //
            // TODO @ZAPPA: CHANGE SETTING
            //
            if (!Main.Settings.AllowExtraKeyboardCharactersInNames)
            {
                return true;
            }

            return Models.GameUiContext.RemoveInvalidFilenameChars.Invoke(textField);
        }
    }
}
