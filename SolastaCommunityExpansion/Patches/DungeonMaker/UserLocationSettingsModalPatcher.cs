//using System.Diagnostics.CodeAnalysis;
//using HarmonyLib;
//using TMPro;

//namespace SolastaCommunityExpansion.Patches.DungeonMaker
//{
//    // allows extra characters on location names
//    [HarmonyPatch(typeof(UserLocationSettingsModal), "RemoveUselessSpaces")]
//    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
//    internal static class UserLocationSettingsModal_RemoveUselessSpaces
//    {
//        public static bool Prefix(TMP_InputField textField)
//        {
//            if (!Main.Settings.AllowExtraKeyboardCharactersInLocationNames)
//            {
//                return true;
//            }

//            return Models.GameUiContext.RemoveInvalidFilenameChars.Invoke(textField);
//        }
//    }
//}
