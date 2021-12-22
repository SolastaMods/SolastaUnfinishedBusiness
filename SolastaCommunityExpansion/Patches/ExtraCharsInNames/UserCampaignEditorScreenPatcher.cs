using HarmonyLib;
using TMPro;

namespace SolastaCommunityExpansion.Patches.ExtraCharsInNames
{
    // allows extra characters on campaign names
    [HarmonyPatch(typeof(UserCampaignEditorScreen), "RemoveUselessSpaces")]
    internal static class UserCampaignEditorScreenRemoveUselessSpaces
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
