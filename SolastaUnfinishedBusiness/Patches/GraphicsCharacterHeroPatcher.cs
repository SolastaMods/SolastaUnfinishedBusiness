using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.CustomFeatures;

namespace SolastaUnfinishedBusiness.Patches;

internal static class GraphicsCharacterHeroPatcher
{
    [HarmonyPatch(typeof(GraphicsCharacterHero), "GetAttackAnimationData")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GetAttackAnimationData_Patch
    {
        internal static void Postfix(
            ref string __result,
            [NotNull] RulesetAttackMode attackMode,
            ref bool isThrown,
            ref bool leftHand)
        {
            ShieldAttack.UseOffhandForShieldAttackAnimation(attackMode, ref __result, ref isThrown, ref leftHand);
        }
    }
}
