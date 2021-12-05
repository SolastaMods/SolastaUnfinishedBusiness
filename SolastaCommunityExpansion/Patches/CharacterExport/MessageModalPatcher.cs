using HarmonyLib;
using TMPro;
using UnityEngine.EventSystems;
using static SolastaCommunityExpansion.Models.CharacterExportContext;

namespace SolastaCommunityExpansion.Patches.CharacterExport
{
    // uses this patch to offer an input field when in the context of character export which is set if message content equals to \n\n\n

    [HarmonyPatch(typeof(MessageModal), "Show")]
    internal static class MessageModal_Show
    {
        internal static void Postfix(string content, GuiLabel ___contentLabel)
        {
            if (Main.Settings.EnableCharacterExport && content == INPUT_MODAL_MARK)
            {
                // add this check here to avoid a restart required on this UI toggle
                if (InputField == null)
                {
                    LoadInputField();
                }

                InputField.gameObject.SetActive(true);
                InputField.ActivateInputField();
                InputField.text = string.Empty;
                ___contentLabel.TMP_Text.alignment = TextAlignmentOptions.BottomLeft;
            }
            else if (InputField != null)
            {
                InputField.gameObject.SetActive(false);
            }
        }
    }

    [HarmonyPatch(typeof(MessageModal), "OnEndShow")]
    internal static class MessageModal_OnEndShow
    {
        internal static void Postfix()
        {
            if (Main.Settings.EnableCharacterExport && InputField != null && InputField.IsActive())
            {
                EventSystem.current.SetSelectedGameObject(InputField.gameObject);
            }
        }
    }
}
