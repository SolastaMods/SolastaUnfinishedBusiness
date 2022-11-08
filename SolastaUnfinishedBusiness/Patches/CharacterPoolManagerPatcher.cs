using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

public static class CharacterPoolManagerPatcher
{
    [HarmonyPatch(typeof(CharacterPoolManager), "SaveCharacter")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class SaveCharacter_Patch
    {
        public static void Prefix(RulesetCharacterHero heroCharacter)
        {
            //PATCH: Keeps last level up hero selected
            if (heroCharacter == null)
            {
                return;
            }

            Global.LastLevelUpHeroName = Main.Settings.KeepCharactersPanelOpenAndHeroSelectedAfterLevelUp
                ? heroCharacter.Name
                : null;
        }
    }
}
