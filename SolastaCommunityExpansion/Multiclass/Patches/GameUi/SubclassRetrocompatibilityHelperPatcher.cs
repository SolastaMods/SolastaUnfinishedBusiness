using HarmonyLib;
using SolastaCommunityExpansion;

namespace SolastaMulticlass.Patches.GameUi
{
    internal static class SubclassRetrocompatibilityHelperPatcher
    {
        // this method is producing a false positive result on MC heroes that have at least one class without a subclass assigned (i.e.: level 1 Paladin, level 1 Druid, etc)
        [HarmonyPatch(typeof(SubclassRetrocompatibilityHelper), "RetrocompatibilityCheckMissingSubclasses")]
        internal static class SubclassRetrocompatibilityHelperRetrocompatibilityCheckMissingSubclasses
        {
            internal static bool Prefix(RulesetCharacterHero hero)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return true;
                }

                return hero.ClassesAndLevels.Count == 1;
            }
        }
    }
}
