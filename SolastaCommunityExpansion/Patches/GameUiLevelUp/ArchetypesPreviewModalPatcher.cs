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
            if (!Main.Settings.EnableSortingFutureFeatures)
            {
                return;
            }

            var subclasses = __instance.GetField<List<CharacterSubclassDefinition>>("subclasses");

            subclasses.ForEach(x => x.FeatureUnlocks.Sort((a, b) => a.Level - b.Level));
        }
    }
}
