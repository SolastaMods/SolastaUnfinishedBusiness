using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.Bugfix
{
    // Default implementation of this metod doesn't pass 'onlyIfCurrentLevel' argument to ICharacterBuildingService
    // This causes subclass to be lost on cancelled levelup
    // Default classes are then silently fixed in CharacterBuildingManager.RepairSubclass, but custom ones break
    [HarmonyPatch(typeof(LocalCommandManager), "UnassignLastSubclass")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class LocalCommandManager_UnassignLastSubclass
    {
        internal static bool Prefix(RulesetCharacterHero hero, bool onlyIfCurrentLevel)
        {
            if (!Main.Settings.BugFixUnassignLastSubclass)
            {
                return true;
            }

            ServiceRepository.GetService<ICharacterBuildingService>()
                .UnassignLastSubclass(hero, onlyIfCurrentLevel);

            return false;
        }
    }
}
