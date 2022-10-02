using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaUnfinishedBusiness.CustomUI;

namespace SolastaUnfinishedBusiness.Patches;

public static class RecoveredFeatureItemPatcher
{
    [HarmonyPatch(typeof(RecoveredFeatureItem), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class Bind_Patch
    {
        public static void Postfix(RecoveredFeatureItem __instance, RulesetCharacterHero character)
        {
            //PATCH: adds current character to recovered during rest feature's tooltip context, so it may properly update ts user-dependant stats
            Tooltips.AddContextToRecoveredFeature(__instance, character);
        }
    }
}
