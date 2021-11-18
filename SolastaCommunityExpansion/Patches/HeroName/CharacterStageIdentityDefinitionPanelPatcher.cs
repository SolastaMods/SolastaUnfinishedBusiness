using HarmonyLib;
using TMPro;

namespace SolastaCommunityExpansion.Patches
{
    internal static class CharacterStageIdentityDefinitionPanelPatcher
    {
        [HarmonyPatch(typeof(CharacterStageIdentityDefinitionPanel), "EnterStage")]
        internal static class CharacterStageIdentityDefinitionPanel_EnterStage_Patch
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
    }

    [HarmonyPatch(typeof(CharacterStageIdentityDefinitionPanel), "RemoveUselessSpaces")]
    internal static class CharacterStageIdentityDefinitionPanel_RemoveUselessSpaces_Patch
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