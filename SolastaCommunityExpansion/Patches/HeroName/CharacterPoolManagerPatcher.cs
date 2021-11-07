using HarmonyLib;

namespace SolastaCommunityExpansion.Patch
{
    internal static class CharacterPoolManagerPatcher
    {
        [HarmonyPatch(typeof(CharacterPoolManager), "SaveCharacter")]
        internal static class CharacterPoolManager_SaveCharacter
        {
            public static void Prefix(RulesetCharacterHero heroCharacter, [HarmonyArgument("addToPool")] bool _ = false)
            {
                if (Main.Settings.AllowExtraKeyboardCharactersInNames && heroCharacter != null)
                {
                    heroCharacter.SurName = heroCharacter.SurName?.Trim();
                    heroCharacter.Name = heroCharacter.Name?.Trim();
                }
            }
        }
    }
}