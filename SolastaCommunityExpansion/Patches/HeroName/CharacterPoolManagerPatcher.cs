using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.HeroName
{
    internal static class CharacterPoolManagerPatcher
    {
        [HarmonyPatch(typeof(CharacterPoolManager), "SaveCharacter")]
        [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
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
