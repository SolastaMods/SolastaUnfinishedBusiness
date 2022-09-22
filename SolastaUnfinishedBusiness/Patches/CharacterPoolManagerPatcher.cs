using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

internal static class CharacterPoolManagerPatcher
{
    [HarmonyPatch(typeof(CharacterPoolManager), "SaveCharacter")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class SaveCharacter_Patch
    {
        public static void Prefix(RulesetCharacterHero heroCharacter, [HarmonyArgument("addToPool")] bool _ = false)
        {
            //PATCH: Keeps last level up hero selected
            if (heroCharacter == null)
            {
                return;
            }

            Global.LastLevelUpHeroName =
                Main.Settings.KeepCharactersPanelOpenAndHeroSelectedOnLevelUp ? heroCharacter.Name : null;
        }
    }
}
