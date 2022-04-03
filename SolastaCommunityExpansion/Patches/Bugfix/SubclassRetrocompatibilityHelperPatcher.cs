using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.Bugfix
{
    internal static class SubclassRetrocompatibilityHelperPatcher
    {
        // fixes a false positive on MC heroes that have at least one class without a subclass assigned (i.e.: level 1 Paladin, level 1 Druid, etc)
        [HarmonyPatch(typeof(SubclassRetrocompatibilityHelper), "RetrocompatibilityCheckMissingSubclasses")]
        internal static class SubclassRetrocompatibilityHelperRetrocompatibilityCheckMissingSubclasses
        {
            internal static bool Prefix(RulesetCharacterHero hero)
            {
                if (!Main.Settings.BugFixDeityOfferingOnMulticlassHeroes)
                {
                    return true;
                }

                return hero.ClassesAndLevels.Count == 1;
            }
        }
    }
}
