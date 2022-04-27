using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using SolastaModApi.Infrastructure;

namespace SolastaCommunityExpansion.Patches.GameUi.LevelUp
{
    [HarmonyPatch(typeof(CharacterStageClassSelectionPanel), "Refresh")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterStageClassSelectionPanel_Refresh
    {
        internal static void Prefix()
        {
            if (!Main.Settings.EnableEnforceUniqueFeatureSetChoices)
            {
                return;
            }

            FeatureDescriptionItemPatcher.FeatureDescriptionItems.Clear();
        }
    }

    [HarmonyPatch(typeof(CharacterStageClassSelectionPanel), "Compare")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterStageClassSelectionPanel_Compare
    {
        internal static void Postfix(CharacterClassDefinition left, CharacterClassDefinition right, ref int __result)
        {
            if (!Main.Settings.EnableSortingClasses)
            {
                return;
            }

            __result = left.FormatTitle().CompareTo(right.FormatTitle());
        }
    }

    // avoids a restart when enabling / disabling on the Mod UI panel
    [HarmonyPatch(typeof(CharacterStageClassSelectionPanel), "OnBeginShow")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterStageClassSelectionPanel_OnBeginShow
    {
        internal static void Prefix(List<CharacterClassDefinition> ___compatibleClasses)
        {
            var visibleClasses = DatabaseRepository.GetDatabase<CharacterClassDefinition>().Where(x => !x.GuiPresentation.Hidden);

            ___compatibleClasses.SetRange(visibleClasses.OrderBy(x => x.FormatTitle()));
        }
    }
}
