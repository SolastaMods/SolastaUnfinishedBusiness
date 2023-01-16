using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.CustomUI;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class RecoveredFeatureItemPatcher
{
    [HarmonyPatch(typeof(RecoveredFeatureItem), nameof(RecoveredFeatureItem.Bind))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Bind_Patch
    {
        [UsedImplicitly]
        public static void Postfix(RecoveredFeatureItem __instance, RulesetCharacterHero character)
        {
            //PATCH: adds current character to recovered during rest feature's tooltip context, so it may properly update ts user-dependant stats
            Tooltips.AddContextToRecoveredFeature(__instance, character);
        }
    }
}
