using System;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Patches;

public static class CharacterStageFightingStyleSelectionPanelPatcher
{
    [HarmonyPatch(typeof(CharacterStageFightingStyleSelectionPanel), "OnBeginShow")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class OnBeginShow_Patch
    {
        public static void Prefix([NotNull] CharacterStageFightingStyleSelectionPanel __instance)
        {
#if false
            //PATCH: changes the fighting style layout to allow more offerings
            var table = __instance.fightingStylesTable;
            var gridLayoutGroup = table.GetComponent<GridLayoutGroup>();
            var fightingStylesCount = __instance.compatibleFightingStyles.Count;

            if (fightingStylesCount > 12)
            {
                gridLayoutGroup.constraintCount = Math.Min(4, ((fightingStylesCount - 1) / 5) + 1);
                table.localScale = new Vector3(0.75f, 0.75f, 0.75f);
            }
            else
            {
                gridLayoutGroup.constraintCount = Math.Min(3, ((fightingStylesCount - 1) / 4) + 1);
                table.localScale = new Vector3(1f, 1f, 1f);
            }
#endif
            //PATCH: sorts the fighting style panel by Title
            if (!Main.Settings.EnableSortingFightingStyles)
            {
                return;
            }

            __instance.compatibleFightingStyles
                .Sort((a, b) =>
                    String.Compare(a.FormatTitle(), b.FormatTitle(), StringComparison.CurrentCultureIgnoreCase));
        }
    }
}
