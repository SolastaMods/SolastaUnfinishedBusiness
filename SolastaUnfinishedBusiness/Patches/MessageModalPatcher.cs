using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using TMPro;
using UnityEngine.EventSystems;
using static SolastaUnfinishedBusiness.Models.CharacterExportContext;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class MessageModalPatcher
{
    [HarmonyPatch(typeof(MessageModal), nameof(MessageModal.OnEndShow))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class OnEndShow_Patch
    {
        [UsedImplicitly]
        public static void Postfix(MessageModal __instance)
        {
            //PATCH: offers an input field when in the context of character export
            if (!Main.Settings.EnableCharacterExport || __instance.contentLabel.Text != InputModalMark)
            {
                if (InputField)
                {
                    InputField.gameObject.SetActive(false);
                }

                return;
            }

            // add this check here to avoid a restart required on this UI toggle
            if (!InputField)
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
}
