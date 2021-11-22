using HarmonyLib;
using TMPro;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches
{
    // uses this patch to offer an input field when in the context of character export which is set if message content equals to \n\n\n
    internal static class MessageModalPatcher
    {
        internal static TMP_InputField InputField { get; private set; }

        [HarmonyPatch(typeof(MessageModal), "Show")]
        internal static class MessageModal_Show
        {
            internal static void Postfix(string content, GuiLabel ___contentLabel)
            {
                if (InputField == null)
                {
                    CharacterCreationScreen characterCreationScreen = Gui.GuiService.GetScreen<CharacterCreationScreen>();
                    CharacterStageIdentityDefinitionPanel panel = characterCreationScreen.GetComponentInChildren<CharacterStageIdentityDefinitionPanel>();
                    InputField = Object.Instantiate(panel.transform.FindChildRecursive("FirstNameInputField").GetComponent<TMP_InputField>(), ___contentLabel.transform);

                    InputField.characterLimit = 20;
                    InputField.onValueChanged = null;
                    InputField.pointSize = ___contentLabel.TMP_Text.fontSize;
                    InputField.transform.position = ___contentLabel.transform.position;
                }
                if (InputField != null)
                {
                    InputField.text = string.Empty;
                    InputField.gameObject.SetActive(content == Models.CharacterExportContext.INPUT_MODAL_MARK);
                }
            }
        }
    }
}
