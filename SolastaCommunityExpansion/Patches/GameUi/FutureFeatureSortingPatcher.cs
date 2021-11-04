
using HarmonyLib;
using SolastaModApi.Infrastructure;
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
                if (!Main.Settings.FutureFeatureSorting)
                {
                    return;
                }
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
                if (!Main.Settings.FutureFeatureSorting)
                {
                    return;
                }
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
                if (!Main.Settings.FutureFeatureSorting)
                {
                    return;
                }
                List<CharacterSubclassDefinition> subclasses = __instance.GetField<List<CharacterSubclassDefinition>>("subclasses");
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
                if (!Main.Settings.FutureFeatureSorting)
                {
                    return;
                }
                classDefinition.FeatureUnlocks.Sort(delegate (FeatureUnlockByLevel a, FeatureUnlockByLevel b)
                {
                    return a.Level - b.Level;
                });
            }
        }
    }
}
