using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using TMPro;

namespace SolastaCommunityExpansion.Patches.HeroName
{
    [HarmonyPatch(typeof(CharacterStageIdentityDefinitionPanel), "EnterStage")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterStageIdentityDefinitionPanel_EnterStage
    {
        public static void Postfix(TMP_InputField ___firstNameInputField, TMP_InputField ___lastNameInputField)
        {
            if (Main.Settings.AllowExtraKeyboardCharactersInNames)
            {
                ___firstNameInputField.characterLimit = 20;
                ___lastNameInputField.characterLimit = 20;
            }
        }
    }

    [HarmonyPatch(typeof(CharacterStageIdentityDefinitionPanel), "RemoveUselessSpaces")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterStageIdentityDefinitionPanel_RemoveUselessSpaces
    {
        public static bool Prefix(TMP_InputField textField)
        {
            if (Main.Settings.AllowExtraKeyboardCharactersInNames)
            {
                return Models.HeroNameContext.Invoke(textField);
            }

            return true;
        }
    }
}
