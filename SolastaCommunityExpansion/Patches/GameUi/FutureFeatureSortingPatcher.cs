
using HarmonyLib;
using System.Collections.Generic;

namespace SolastaCommunityExpansion.Patches
{
    class FutureFeatureSortingPatcher
    {

        [HarmonyPatch(typeof(CharacterStageSubclassSelectionPanel), "FillSubclassFeatures")]
        internal static class CharacterStageSubclassSelectionPanel_FillSubclassFeatures
        {
            internal static void Prefix(CharacterSubclassDefinition subclassDefinition)
            {
                subclassDefinition.FeatureUnlocks.Sort(delegate (FeatureUnlockByLevel a, FeatureUnlockByLevel b)
                {
                    return a.Level - b.Level;
                });
            }

       
        }

        [HarmonyPatch(typeof(CharacterStageDeitySelectionPanel), "FillSubclassFeatures")]
        internal static class CharacterStageDeitySelectionPanel_FillSubclassFeatures
        {
            internal static void Prefix(CharacterSubclassDefinition currentSubclassDefinition)
            {
                currentSubclassDefinition.FeatureUnlocks.Sort(delegate (FeatureUnlockByLevel a, FeatureUnlockByLevel b)
                {
                    return a.Level - b.Level;
                });
            }
        }

        [HarmonyPatch(typeof(ArchetypesPreviewModal), "Bind")]
        internal static class ArchetypesPreviewModal_Bind
        {
            internal static void Postfix(ArchetypesPreviewModal __instance)
            {
                List<CharacterSubclassDefinition> subclasses = (List<CharacterSubclassDefinition>)Traverse.Create(__instance).Field("subclasses").GetValue();
                foreach (CharacterSubclassDefinition subclassDefinition in subclasses)
                {
                    subclassDefinition.FeatureUnlocks.Sort(delegate (FeatureUnlockByLevel a, FeatureUnlockByLevel b)
                    {
                        return a.Level - b.Level;
                    });
                }
            }
        }

        [HarmonyPatch(typeof(CharacterStageClassSelectionPanel), "FillClassFeatures")]
        internal static class CharacterStageClassSelectionPanel_FillSubclassFeatures
        {
            internal static void Prefix(CharacterClassDefinition classDefinition)
            {
                classDefinition.FeatureUnlocks.Sort(delegate (FeatureUnlockByLevel a, FeatureUnlockByLevel b)
                {
                    return a.Level - b.Level;
                });
            }
        }
    }
}
