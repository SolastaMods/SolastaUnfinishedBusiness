using HarmonyLib;
using TMPro;

namespace SolastaCommunityExpansion.Patches.DungeonMaker
{
    // allows extra characters on campaign names
    [HarmonyPatch(typeof(UserCampaignEditorScreen), "RemoveUselessSpaces")]
    internal static class UserCampaignEditorScreenRemoveUselessSpaces
    {
        public static bool Prefix(TMP_InputField textField)
        {
            if (!Main.Settings.AllowExtraKeyboardCharactersInCampaignNames)
            {
                return true;
            }

            return Models.GameUiContext.RemoveInvalidFilenameChars.Invoke(textField);
        }
    }
}
