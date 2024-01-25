using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class CharacterPoolManagerPatcher
{
    [HarmonyPatch(typeof(CharacterPoolManager), nameof(CharacterPoolManager.SaveCharacter))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class SaveCharacter_Patch
    {
        [UsedImplicitly]
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
