using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using TMPro;
using UnityEngine.EventSystems;
using static SolastaCommunityExpansion.Models.CharacterExportContext;

namespace SolastaCommunityExpansion.Patches.GameUi.CharacterExport;

// uses this patch to offer an input field when in the context of character export which is set if message content equals to \n\n\n
[HarmonyPatch(typeof(MessageModal), "OnEndShow")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class MessageModal_OnEndShow
{
    internal static void Postfix(MessageModal __instance)
    {
        if (!Main.Settings.EnableCharacterExport || __instance.contentLabel.Text != InputModalMark)
        {
            if (InputField != null)
            {
                InputField.gameObject.SetActive(false);
            }

            return;
        }

        // add this check here to avoid a restart required on this UI toggle
        if (InputField == null)
        {
            Load();
        }

        __instance.contentLabel.TMP_Text.alignment = TextAlignmentOptions.BottomLeft;

        InputField.gameObject.SetActive(true);
        InputField.ActivateInputField();
        InputField.text = string.Empty;

        EventSystem.current.SetSelectedGameObject(InputField.gameObject);
    }
}
