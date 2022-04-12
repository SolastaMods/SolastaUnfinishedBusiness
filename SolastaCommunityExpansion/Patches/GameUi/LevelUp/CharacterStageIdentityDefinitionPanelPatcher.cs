using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using TMPro;

namespace SolastaCommunityExpansion.Patches.GameUi.LevelUp
{
    [HarmonyPatch(typeof(CharacterStageIdentityDefinitionPanel), "EnterStage")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterStageIdentityDefinitionPanel_EnterStage
    {
        public static void Postfix(TMP_InputField ___firstNameInputField, TMP_InputField ___lastNameInputField)
        {
            if (Main.Settings.AllowExtraKeyboardCharactersInAllNames)
            {
                ___firstNameInputField.characterLimit = 20;
                ___lastNameInputField.characterLimit = 20;
            }
        }
    }
}
