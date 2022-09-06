using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;

namespace SolastaCommunityExpansion.Patches;

// fixes a false positive on MC heroes that have at least one class without a subclass assigned (i.e.: level 1 Paladin, level 1 Druid, etc)
[HarmonyPatch(typeof(SubclassRetrocompatibilityHelper), "RetrocompatibilityCheckMissingSubclasses")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class SubclassRetrocompatibilityHelper_RetrocompatibilityCheckMissingSubclasses
{
    internal static bool Prefix([NotNull] RulesetCharacterHero hero)
    {
        //
        // BUGFIX: deity offering on MC heroes
        //

        return hero.ClassesAndLevels.Count == 1;
    }
}
