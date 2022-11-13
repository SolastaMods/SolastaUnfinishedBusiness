using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.CustomBehaviors;

namespace SolastaUnfinishedBusiness.Patches;

public static class GraphicsCharacterHeroPatcher
{
    [HarmonyPatch(typeof(GraphicsCharacterHero), "GetAttackAnimationData")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class GetAttackAnimationData_Patch
    {
        public static void Postfix(
            ref string __result,
            [NotNull] RulesetAttackMode attackMode,
            ref bool isThrown,
            ref bool leftHand)
        {
            ShieldAttack.UseOffhandForShieldAttackAnimation(attackMode, ref __result, ref isThrown, ref leftHand);
        }
    }
}
