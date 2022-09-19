using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;

namespace SolastaUnfinishedBusiness.Patches;

public static class CharacterBuildingManagerPatcher
{
    [HarmonyPatch(typeof(CharacterBuildingManager), "IsFeatMatchingPrerequisites")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class FeatSubPanel_SetState
    {
        internal static void Postfix(CharacterBuildingManager __instance,
            ref bool __result,
            CharacterHeroBuildingData heroBuildingData,
            FeatDefinition feat,
            ref bool isSameFamilyPrerequisite)
        {
            //PATCH: fixes being able to select feats from same family when more than 1 feat selection is possible aat same time
            //vanilla code doesn't check if we already have selected feats from same family
            if (!__result || !feat.HasFamilyTag || string.IsNullOrEmpty(feat.FamilyTag))
            {
                return;
            }

            if (heroBuildingData.levelupTrainedFeats.Any(pair =>
                    pair.Value.Any(f => f.HasFamilyTag && f.FamilyTag == feat.FamilyTag)))
            {
                __result = false;
                isSameFamilyPrerequisite = true;
            }
        }
    }
}
