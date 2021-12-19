using HarmonyLib;
using SolastaModApi.Infrastructure;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SolastaCommunityExpansion.Patches.GameUiLevelUp
{
    [HarmonyPatch(typeof(ArchetypesPreviewModal), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class ArchetypesPreviewModal_Bind
    {
        internal static void Postfix(ArchetypesPreviewModal __instance)
        {
            if (!Main.Settings.FutureFeatureSorting)
            {
                return;
            }
            List<CharacterSubclassDefinition> subclasses = __instance.GetField<List<CharacterSubclassDefinition>>("subclasses");
            foreach (CharacterSubclassDefinition subclassDefinition in subclasses)
            {
                subclassDefinition.FeatureUnlocks.Sort((a, b) => a.Level - b.Level);
            }
        }
    }
}
