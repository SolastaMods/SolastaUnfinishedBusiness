using HarmonyLib;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SolastaCommunityExpansion.Patches.GameUiCharactersPanel
{
    internal static class CharacterFilteringGroupPatcher
    {
        //
        // ATT: this patch is disabled as BugFix.CharacterSelectionModalPatcher is doing the same. Left here in case that bug gets fixed and we need this particular change as a standalone patch
        //

        // ensures MC heroes are correctly sorted by level on character selection modal
        // [HarmonyPatch(typeof(CharacterFilteringGroup), "Compare")]
        [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
        internal static class CharacterFilteringGroup_Compare
        {
            public static int MyLevels(int[] levels)
            {
                return levels.Sum();
            }

            internal static void Postfix(RulesetCharacterHero.Snapshot left, RulesetCharacterHero.Snapshot right, bool ___sortInverted, SortGroup.Category ___sortCategory, ref int __result)
            {
                if (___sortCategory == SortGroup.Category.CharacterLevel)
                {
                    var sortSign = ___sortInverted ? -1 : 1;

                    __result = sortSign * left.Levels.Sum().CompareTo(right.Levels.Sum());
                }
            }
        }
    }
}
