using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.GameUi.CharactersPool
{
    internal static class CharacterPoolManagerPatcher
    {
        internal static string HeroName { get; set; }

        [HarmonyPatch(typeof(CharacterPoolManager), "SaveCharacter")]
        [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
        internal static class CharacterPoolManager_SaveCharacter
        {
            public static void Prefix(RulesetCharacterHero heroCharacter, [HarmonyArgument("addToPool")] bool _ = false)
            {
                if (heroCharacter == null)
                {
                    return;
                }

                if (Main.Settings.AllowExtraKeyboardCharactersInAllNames)
                {
                    heroCharacter.SurName = heroCharacter.SurName?.Trim();
                    heroCharacter.Name = heroCharacter.Name?.Trim();
                }

                if (Main.Settings.KeepCharactersPanelOpenAndHeroSelectedOnLevelUp)
                {
                    HeroName = heroCharacter.Name;
                }
                else
                {
                    HeroName = null;
                }
            }
        }
    }
}
