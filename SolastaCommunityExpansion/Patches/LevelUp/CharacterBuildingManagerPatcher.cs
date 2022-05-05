using System.Linq;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.LevelUp
{
    internal static class CharacterBuildingManagerPatcher
    {
        // register the hero leveling up
        [HarmonyPatch(typeof(CharacterBuildingManager), "LevelUpCharacter")]
        internal static class CharacterBuildingManager_LevelUpCharacter
        {
            internal static void Prefix(RulesetCharacterHero hero)
            {
                var lastClass = hero.ClassesHistory.Last();

                hero.ClassesAndSubclasses.TryGetValue(lastClass, out var lastSubclass);

                LevelUpContext.RegisterHero(hero, lastClass, lastSubclass);               
            }
        }

        // unregister the hero leveling up
        [HarmonyPatch(typeof(CharacterBuildingManager), "FinalizeCharacter")]
        internal static class CharacterBuildingManager_FinalizeCharacter
        {
            internal static void Postfix(RulesetCharacterHero hero)
            {
                LevelUpContext.UnregisterHero(hero);
            }
        }
    }
}
