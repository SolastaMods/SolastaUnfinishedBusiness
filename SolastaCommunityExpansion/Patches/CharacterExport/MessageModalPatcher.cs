using HarmonyLib;
using TMPro;
using static SolastaCommunityExpansion.Models.CharacterExportContext;

namespace SolastaCommunityExpansion.Patches
{
    // uses this patch to offer an input field when in the context of character export which is set if message content equals to \n\n\n
    internal static class MessageModalPatcher
    {

        [HarmonyPatch(typeof(MessageModal), "Show")]
        internal static class MessageModal_Show
        {
            internal static void Postfix(string content, GuiLabel ___contentLabel)
            {
                if (Main.Settings.EnableCharacterExport && content == INPUT_MODAL_MARK)
                { 
                    InputField.gameObject.SetActive(true);
                    InputField.ActivateInputField();
                    InputField.text = string.Empty;
                    ___contentLabel.TMP_Text.alignment = TextAlignmentOptions.BottomLeft;
                }
                else
                {
                    InputField.gameObject.SetActive(false);
                }
            }
        }
    }
}
