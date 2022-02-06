using HarmonyLib;

namespace SolastaCommunityExpansion.Multiclass.Patches.GameUi
{
    internal static class SubclassRetrocompatibilityHelperPatcher
    {
        // this method is producing a false positive result on MC heroes that have at least one class without a subclass assigned (i.e.: level 1 Paladin, level 1 Druid, etc)
        // this patch only allows it to execute if hero isn't MC
        [HarmonyPatch(typeof(SubclassRetrocompatibilityHelper), "RetrocompatibilityCheckMissingSubclasses")]
        internal static class SubclassRetrocompatibilityHelperRetrocompatibilityCheckMissingSubclasses
        {
            internal static bool Prefix(RulesetCharacterHero hero)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return true;
                }

                return hero?.ClassesAndLevels.Count == 1;
            }
        }
    }
}
