using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.GameUiCharactersPanel
{
    // Modify sorting on characters panel.
    // The default sort is by character class, which actually sorts on class+path.
    // Change this to sort by just class name which looks better.
    // Also add sorting by name after the chosen sort category.
    // Could do something similar with race/ancestry.
    [HarmonyPatch(typeof(CharacterFilteringGroup), "Compare")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterFilteringGroup_Compare
    {
        public static void Postfix(RulesetCharacterHero.Snapshot left, RulesetCharacterHero.Snapshot right,
            bool ___sortInverted, SortGroup.Category ___sortCategory, ref int __result)
        {

            if(left == null || right == null)
            {
                return;
            }

            int num = ___sortInverted ? -1 : 1;

            switch (___sortCategory)
            {
                case SortGroup.Category.CharacterClass:
                    __result = num * left.Classes[0].CompareTo(right.Classes[0]);
                    __result = __result == 0 ? num * SortByName() : __result;
                    break;
                case SortGroup.Category.CharacterLevel:
                case SortGroup.Category.CharacterAncestry:
                    __result = __result == 0 ? num * SortByName() : __result;
                    break;
                default:
                    // don't modify other categories
                    break;
            }

            int SortByName()
            {
                return num * left.Name.CompareTo(right.Name);
            }
        }
    }
}
