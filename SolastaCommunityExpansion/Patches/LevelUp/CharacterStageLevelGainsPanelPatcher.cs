using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.LevelUp
{
    [HarmonyPatch(typeof(CharacterStageLevelGainsPanel), "Refresh")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterStageLevelGainsPanel_Refresh
    {
        internal static void Prefix()
        {
            if (!Main.Settings.EnableEnforceUniqueFeatureSetChoices)
            {
                return;
            }

            //FeatureDescriptionItemPatcher.IsClassSelectionStage = false;
            FeatureDescriptionItemPatcher.FeatureDescriptionItems.Clear();
        }
    }
}
