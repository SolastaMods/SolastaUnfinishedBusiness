using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.GameUi.CharactersPool;

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

        Global.LastLevelUpHeroName =
            Main.Settings.KeepCharactersPanelOpenAndHeroSelectedOnLevelUp ? heroCharacter.Name : null;
    }
}
