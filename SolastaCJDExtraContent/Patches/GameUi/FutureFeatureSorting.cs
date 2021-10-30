
using HarmonyLib;
using System.Collections.Generic;

namespace SolastaCJDExtraContent.Patches
{
    class FutureFeatureSorting
    {

        [HarmonyPatch(typeof(CharacterStageSubclassSelectionPanel), "FillSubclassFeatures")]
        internal static class CharacterStageSubclassSelectionPanel_SubclassSort
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
        internal static class CharacterStageDeitySelectionPanel_SubclassSort
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
        internal static class ArchetypesPreviewModal_SubclassSort
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
        internal static class CharacterStageClassSelectionPanel_SecondLineUnbind
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
