//
// TODO: Enable this on a future release
//

//using HarmonyLib;

//namespace SolastaUnofficialHotFixes.Patch
//{
//    internal static class CharacterPoolManagerPatcher
//    {
//        [HarmonyPatch(typeof(CharacterPoolManager), "SaveCharacter")]
//        internal static class CharacterPoolManager_SaveCharacter
//        {
//            public static void Prefix(RulesetCharacterHero heroCharacter, [HarmonyArgument("addToPool")] bool _ = false)
//            {
//                if (heroCharacter != null)
//                {
//                    heroCharacter.SurName = heroCharacter.SurName?.Trim();
//                    heroCharacter.Name = heroCharacter.Name?.Trim();
//                }
//            }
//        }
//    }
//}